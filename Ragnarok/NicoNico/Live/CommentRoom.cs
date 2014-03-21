using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;
using System.Threading;

using Ragnarok;
using Ragnarok.Net;
using Ragnarok.Utility;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// 各放送の各部屋のコメントの送受信を行います。
    /// </summary>
    /// <remarks>
    /// <para>
    /// デッドロックを避けるため、このクラスのロックをしているときは
    /// CommentClientのいくつかのメソッド（内部でロックするメソッド）
    /// は呼ばないようにしてください。
    /// CommentClient内部ではロック中に、CommentRoomのメソッド
    /// （内部でロックするメソッド）を呼ぶので、
    /// デッドロックする可能性があります。
    /// </para>
    /// 
    /// <para>
    /// CommentClientでlock → CommentRoomでlock
    /// と今の実装ではなっているため
    /// CommentRoomでlock → CommentClientでlock
    /// という処理を行うと、どこかでデッドロックします。
    /// </para>
    /// </remarks>
    internal sealed class CommentRoom : ILogObject, IDisposable
    {
        /// <summary>
        /// コメント投稿間隔の最小値です。
        /// </summary>
        public static readonly TimeSpan MinimumWaitTimeForPost =
            TimeSpan.FromSeconds(3.0);
        /// <summary>
        /// アクセス拒否になった場合の待ち時間です。
        /// </summary>
        public static readonly TimeSpan AccessDeniedWaitTime =
            TimeSpan.FromSeconds(5 * 60);
        /// <summary>
        /// 追い出しコマンドを判定します。
        /// </summary>
        public static readonly Regex HbRegex =
            new Regex(@"^/hb ifseetno\s+(\d+)");
        /// <summary>
        /// NGコメントを利用して自分の184IDを調べるために使います。
        /// </summary>
        private const string TextToCheckAnonymousId =
            "まんこ　ちんこ";

        private readonly object SyncRoot = new object();
        private Socket socket;
        private ReentrancyLock startingMessageLock = new ReentrancyLock();
        private bool isDisconnecting = false;
        private readonly ManualResetEvent startMessageEvent = new ManualResetEvent(false);
        private List<PostComment> postCommentList = new List<PostComment>();
        private PostComment sendingComment = null;
        private readonly MemoryStream receivedBuffer = new MemoryStream();
        private bool disposed = false;

        private readonly CommentClient commentClient;
        private readonly int index;
        private readonly CommentRoomInfo roomInfo;
        private string anonymousId;
        private string ticket;
        private DateTime lastGetPostKey = DateTime.Now - MinimumWaitTimeForPost;
        private string postKey;
        private DateTime lastPostTime = DateTime.Now - MinimumWaitTimeForPost;
        private TimeSpan waitForPostTime = TimeSpan.Zero;
        private string lastPostText;
        private int postTryCount = 0;
        private int lastRes = 0;

        /// <summary>
        /// ログ出力用の名前を取得します。
        /// </summary>
        public string LogName
        {
            get
            {
                var result = "コメント部屋[" +
                    this.commentClient.LiveIdString;

                if (!string.IsNullOrEmpty(this.RoomLabel))
                {
                    result += ": " + this.RoomLabel;
                }

                result += "]";
                return result;
            }
        }

        /// <summary>
        /// ルームのインデックスを取得します。
        /// </summary>
        public int Index
        {
            get { return this.index; }
        }

        /// <summary>
        /// コメントルームの情報を取得します。
        /// </summary>
        public CommentRoomInfo RoomInfo
        {
            get { return this.roomInfo; }
        }

        /// <summary>
        /// コメントサーバーのアドレスを取得します。
        /// </summary>
        public string Address
        {
            get { return this.roomInfo.Address; }
        }

        /// <summary>
        /// コメントサーバーのポートを取得します。
        /// </summary>
        public int Port
        {
            get { return this.roomInfo.Port; }
        }

        /// <summary>
        /// コメントサーバーのスレッド番号を取得します。
        /// </summary>
        public int ThreadId
        {
            get { return this.roomInfo.Thread; }
        }

        /// <summary>
        /// ルームラベルを取得します。
        /// </summary>
        public string RoomLabel
        {
            get { return this.roomInfo.RoomLabel; }
        }

        /// <summary>
        /// 最終レス番号を取得します。
        /// </summary>
        public int LastRes
        {
            get { return this.lastRes; }
        }

        /// <summary>
        /// コメントが投稿可能になるまでの時間を取得します。
        /// </summary>
        public TimeSpan WaitTime
        {
            get
            {
                using (new DebugLock(SyncRoot))
                {
                    var now = DateTime.Now;
                    var lastPostTime = this.lastPostTime;
                    var waitTime = this.waitForPostTime;

                    // 投稿可能になる時刻です。
                    var canPostTime = lastPostTime + waitTime;
                    if (canPostTime <= now)
                    {
                        return TimeSpan.Zero;
                    }
                    else
                    {
                        // 投稿可能になるまでの時間を返します。
                        return (canPostTime - now);
                    }
                }
            }
        }

        /// <summary>
        /// 今すぐコメントが投稿可能かどうかを取得します。
        /// </summary>
        public bool CanPostCommentNow
        {
            get
            {
                return (WaitTime <= TimeSpan.Zero);
            }
        }

        /// <summary>
        /// 未投稿のコメント数を取得します。
        /// </summary>
        public int LeaveCommentCount
        {
            get
            {
                using (new DebugLock(SyncRoot))
                {
                    return this.postCommentList.Count;
                }
            }
        }

        /// <summary>
        /// 自分の184IDを取得または設定します。
        /// </summary>
        public string AnonymousId
        {
            get { return this.anonymousId; }
            internal set { this.anonymousId = value; }
        }

        /// <summary>
        /// コメントサーバーに接続中かどうかを取得します。
        /// </summary>
        public bool IsConnected
        {
            get
            {
                using (new DebugLock(SyncRoot))
                {
                    return (this.socket != null && this.socket.Connected);
                }
            }
        }

        /// <summary>
        /// コメントサーバーから切断中かどうかを取得します。
        /// </summary>
        public bool IsDisconnecting
        {
            get { return this.isDisconnecting; }
        }

        /// <summary>
        /// メッセージを送信中かどうかを取得します。
        /// </summary>
        public bool IsSendingMessage
        {
            get { return (this.sendingComment != null); }
        }

        /// <summary>
        /// コメントサーバーに接続します。
        /// </summary>
        /// <remarks>
        /// CommentClientから呼ばれたときは、そこでのフィールド値が
        /// 正しく設定されていない可能性があるので(フィールド値は
        /// 全コメントルームへの接続が確認された後に設定されるため)、
        /// ここではcommentClientへの参照を扱わないでください。
        /// 
        /// また、同様の理由でcommentClient.OnConnectedRoomは呼びません。
        /// </remarks>
        public void Connect(CookieContainer cc, TimeSpan timeout)
        {
            var socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            // ソケットの接続処理を開始します。
            var result = socket.BeginConnect(
                this.Address,
                this.Port,
                result_ =>
                {
                    try
                    {
                        socket.EndConnect(result_);
                    }
                    catch (Exception)
                    {
                    }
                },
                null);

            // timeout[ms]以上たったら、接続失敗として扱います。
            if (!result.AsyncWaitHandle.WaitOne(timeout))
            {
                socket.Close();

                throw new NicoLiveException(
                    string.Format(
                        "コメントルーム'{0}'への接続がタイムアウトしました。",
                        roomInfo.RoomLabel));
            }

            Disconnect();

            using (new DebugLock(SyncRoot))
            {
                // フィールド群を保存します。
                this.socket = socket;
                this.startMessageEvent.Reset();
            }

            StartReceiveMessageProcess();

            Log.Debug(this,
                "コメントルームに接続しました。");
        }

        /// <summary>
        /// サーバーとの接続を切断します。
        /// </summary>
        public void Disconnect()
        {
            using (new DebugLock(SyncRoot))
            {
                if (this.isDisconnecting || this.socket == null)
                {
                    return;
                }

                try
                {
                    this.isDisconnecting = true;

                    // ソケットの切断処理を開始します。
                    this.socket.Disconnect(false);
                }
                finally
                {
                    this.isDisconnecting = false;
                }
            }

            NotifyDisconnected(DisconnectReason.Disconnected);
        }

        /// <summary>
        /// 次のコメント投稿までの待ち時間を設定します。
        /// </summary>
        private void SetPostMessageWait(DateTime lastPostTime, TimeSpan waitTime)
        {
            using (new DebugLock(SyncRoot))
            {
                this.lastPostTime = lastPostTime;
                this.waitForPostTime = waitTime;
            }
        }

        /// <summary>
        /// 次のコメント投稿までの待ち時間を追加します。
        /// </summary>
        private void AddPostMessageWait(TimeSpan waitTime)
        {
            using (new DebugLock(SyncRoot))
            {
                this.waitForPostTime += waitTime;
            }
        }

        /// <summary>
        /// 何らかの理由で接続が切断されたときに呼ばれます。
        /// </summary>
        private void NotifyDisconnected(DisconnectReason reason)
        {
            using (new DebugLock(SyncRoot))
            {
                if (this.socket != null)
                {
                    this.socket.Close();
                    this.socket = null;
                }

                this.isDisconnecting = false;
                this.sendingComment = null;
                this.postKey = null;
                this.postTryCount = 0;
                this.receivedBuffer.SetLength(0);
                ClearMessage();
            }

            this.commentClient.OnDisconnectedRoom(this, reason);

            Log.Debug(this,
                "コメントルームから切断されました。(理由={0})",
                reason);
        }

        #region メッセージ受信処理
        /// <summary>
        /// サーバーのメッセージ処理を開始します。
        /// </summary>
        /// <remarks>
        /// これ以後、メッセージが受信されるようになります。
        /// </remarks>
        public void StartReceiveMessage(int resFrom, int timeout, DateTime? when = null)
        {
            using (var result = this.startingMessageLock.Lock())
            {
                if (result == null)
                {
                    throw new NicoLiveException(
                        "メッセージの受信開始処理をすでに行っています。");
                }

                using (new DebugLock(SyncRoot))
                {
                    // 送信するメッセージの組み立て
                    var message = string.Empty;
                    if (when == null)
                    {
                        // 通常のコメント取得
                        message = NicoString.MakeThreadStart(
                            ThreadId, resFrom);
                    }
                    else
                    {
                        // 過去ログの取得
                        var waybackkey = LiveUtil.GetWaybackKey(
                            ThreadId, this.commentClient.CookieContainer);

                        message = NicoString.MakeThreadStart(
                            ThreadId, resFrom, this.commentClient.UserId,
                            waybackkey, when.Value);
                    }

                    // メッセージの受信を開始します。
                    this.startMessageEvent.Reset();
                    SendMessageSync(message);
                }

                // startMessageEventがシグナル状態にならなかった場合、
                // サーバーからのメッセージ受信に失敗しています。
                if (!this.startMessageEvent.WaitOne(timeout))
                {
                    throw new NicoLiveException(
                        "メッセージの受信開始処理に失敗しました。",
                        this.commentClient.LiveIdString);
                }
            }
        }

        /// <summary>
        /// 184IDを調べるためにNGコメントを投稿します。
        /// </summary>
        /// <remarks>
        /// NGコメントは投稿者からは普通のコメントと同じように同じように見える
        /// けれど、他の人からは投稿されていないように見えます。
        /// この性質を使い、自分だけに判るNGコメントを投稿し、
        /// そのコメントのIDを調べることで自分の184IDを判別します。
        /// </remarks>
        internal void SendCommentToCheckAnonymousId()
        {
            SendComment(TextToCheckAnonymousId, "184", DateTime.Now);
        }

        /// <summary>
        /// メッセージの受信処理を開始します。
        /// </summary>
        private void StartReceiveMessageProcess()
        {
            try
            {
                var buffer = new byte[512];
                var socket = null as Socket;

                using (new DebugLock(SyncRoot))
                {
                    if (!IsConnected || IsDisconnecting)
                    {
                        return;
                    }

                    socket = this.socket;
                }

                // コールバックが今のスレッドと同じスレッドから呼び出される
                // 可能性があるため、このメソッドはlockの外側で
                // 呼び出します。
                // こうしないとデッドロックの原因になることがあります。
                socket.BeginReceive(
                    buffer, 0, buffer.Length,
                    SocketFlags.Partial,
                    ReceiveMessageDone,
                    buffer);

                Log.Trace(this,
                    "メッセージの受信開始処理に成功しました。");
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
                Log.ErrorException(ex,
                    "メッセージの開始処理に失敗しました。");
            }
        }

        /// <summary>
        /// メッセージの受信が行われたときに呼ばれます。
        /// </summary>
        private void ReceiveMessageDone(IAsyncResult result)
        {
            byte[] buffer = null;
            int size = -1;

            try
            {
                using (new DebugLock(SyncRoot))
                {
                    if (!IsConnected || IsDisconnecting)
                    {
                        return;
                    }

                    buffer = (byte[])result.AsyncState;
                    size = this.socket.EndReceive(result);
                }

                // サイズが０ならばサーバーから接続が切断されています。
                if (size == 0)
                {
                    NotifyDisconnected(DisconnectReason.DisconnectedByOpposite);
                    return;
                }
            }
            catch (SocketException e)
            {
                Log.ErrorException(this, e,
                    "メッセージ受信時にエラーが発生しました。");

                NotifyDisconnected(DisconnectReason.Error);
                return;
            }
            catch (Exception e)
            {
                Util.ThrowIfFatal(e);
                Log.ErrorException(this, e,
                    "意図しないエラーが発生しました。");

                NotifyDisconnected(DisconnectReason.Error);
                return;
            }

            // lockの外側で呼び出します。
            var messages = ConvertMessageList(buffer, size);
            ProcessMessageList(messages);

            // 次のコメントの受信を開始します。
            if (IsConnected)
            {
                StartReceiveMessageProcess();
            }
        }

        /// <summary>
        /// バイナリデータをxmlの各メッセージに変換します。
        /// </summary>
        private List<string> ConvertMessageList(byte[] buffer, int size)
        {
            try
            {
                lock (this.receivedBuffer)
                {
                    var messages = new List<string>();
                    var begin = 0;

                    // 各メッセージは'\0'で区切られており、それぞれがxmlの
                    // フォーマットを持っています。
                    for (var i = 0; i < size; ++i)
                    {
                        if (buffer[i] != 0)
                        {
                            continue;
                        }

                        string message = null;
                        if (this.receivedBuffer.Position > 0)
                        {
                            // 以前の受信データがある場合は、それと新しく来た
                            // データをマージしてからメッセージに直します。
                            this.receivedBuffer.Write(buffer, begin, i - begin);
                            var messageBytes = this.receivedBuffer.ToArray();
                            receivedBuffer.SetLength(0);

                            // 文字列化します。
                            message = Encoding.UTF8.GetString(messageBytes);
                        }
                        else
                        {
                            // 以前の受信データが無い場合は、
                            // もう少し効率的な方法でメッセージを変換します。
                            message = Encoding.UTF8.GetString(
                                buffer, begin, i - begin);
                        }

                        messages.Add(message);
                        begin = i + 1; // '\0'の分はとばします。
                    }

                    // 未処理のデータは次の処理までとっておきます。
                    if (begin < size)
                    {
                        this.receivedBuffer.Write(buffer, begin, size - begin);
                    }

                    return messages;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "メッセージ変換中にエラーが発生しました。");

                return null;
            }
        }

        /// <summary>
        /// 各メッセージを処理します。
        /// </summary>
        private void ProcessMessageList(IEnumerable<string> messages)
        {
            if (messages == null)
            {
                return;
            }

            // HandleMessageはlockの外側で呼び出します。
            foreach (var message in messages)
            {
                try
                {
                    HandleMessage(message);
                }
                catch (Exception ex)
                {
                    Log.ErrorException(
                        this, ex,
                        "メッセージの処理に失敗しました。(Message = {0})",
                        message);
                }
            }
        }

        /// <summary>
        /// 各メッセージを処理します。
        /// </summary>
        private void HandleMessage(string message)
        {
            // このデバッグログは量が多いので制限します。
            if (!this.commentClient.IsSupressLog)
            {
                Log.Debug(this,
                    "メッセージの処理を開始します。{0}(Message = {1})",
                    Environment.NewLine,
                    message);
            }

            var doc = new XmlDocument();
            doc.LoadXml(message);

            var element = doc.DocumentElement;
            switch (element.Name)
            {
                case "thread":
                    HandleThreadMessage(element);
                    break;
                case "chat":
                    HandleChatMessage(element);
                    break;
                case "chat_result":
                    HandleChatResultMessage(element);
                    break;
                default:
                    Log.Error(this,
                        "メッセージの処理に失敗しました。{0}(Message = {1})",
                        Environment.NewLine,
                        message);
                    break;
            }

            Log.Trace(this,
                "メッセージの処理を終了しました。");
        }

        /// <summary>
        /// threadメッセージを処理します。
        /// </summary>
        private void HandleThreadMessage(XmlNode node)
        {
            using (new DebugLock(SyncRoot))
            {
                XmlAttribute attr = node.Attributes["resultcode"];
                var rc = int.Parse(attr.Value);
                if (rc != 0)
                {
                    Log.Error(this,
                        "threadメッセージの受信開始に失敗しました。");
                    return;
                }

                // スレッド番号が違う場合は、今接続したサーバーとは違う
                // サーバーからのレスポンスが返ってきている可能性があります。
                attr = node.Attributes["thread"];
                if (int.Parse(attr.Value) != this.ThreadId)
                {
                    Log.Error(this,
                        "threadメッセージにthreadがありません。");
                    return;
                }

                attr = node.Attributes["ticket"];
                this.ticket = attr.Value;

                // コメントが無い状態だとlast_res属性は存在しません。
                attr = node.Attributes["last_res"];
                this.lastRes = (attr != null ? int.Parse(attr.Value) : 0);

                // メッセージ処理の開始に成功したことを知らせます。
                this.startMessageEvent.Set();

                Log.Trace(this,
                    "threadメッセージの解析に成功しました。");
            }
        }

        /// <summary>
        /// chatメッセージを処理します。
        /// </summary>
        private void HandleChatMessage(XmlNode node)
        {
            var comment = new Comment(node);

            using (new DebugLock(SyncRoot))
            {
                if (comment.No < 0)
                {
                    comment.No = this.lastRes + 1;
                    this.lastRes = comment.No;
                }
                else
                {
                    this.lastRes = Math.Max(this.lastRes, comment.No);
                }
            }

            // 184チェック用のコメントと同じコメントが投稿されていたら、
            // 自分の184IDとして処理します。
            if (this.anonymousId == null &&
                comment.IsUserComment &&
                comment.IsAnonymous &&
                (comment.Text == TextToCheckAnonymousId || comment.IsYourpost))
            {
                this.commentClient.SetAnonymousId(comment.UserId);
            }

            // 投稿IDが自分の184IDなら、次のコメント投稿を少し後らせます。
            if (!string.IsNullOrEmpty(comment.UserId) &&
                comment.UserId == this.anonymousId)
            {
                SetPostMessageWait(DateTime.Now, MinimumWaitTimeForPost);
            }

            // コメントの受信処理を行います。
            this.commentClient.OnCommentReceivedRoom(this, comment);

            // 追い出しコマンドの判定を行います。
            if (comment.IsManagementComment)
            {
                var m = HbRegex.Match(comment.Text);
                if (m.Success)
                {
                    var seetNo = int.Parse(m.Groups[1].Value);

                    // 部屋番号が一致したら追い出されたと言うこと
                    if (seetNo == this.commentClient.CurrentRoomSeetNo)
                    {
                        //this.commentClient.Reconnect();
                        //Disconnect();
                    }
                }
            }

            // 最後に、切断処理を行います。
            if ((comment.CommentType == CommentType.Owner ||
                 comment.CommentType == CommentType.Alert) &&
                comment.Text == "/disconnect")
            {
                Disconnect();
            }

            Log.Trace(this,
                "chatメッセージの解析に成功しました。");
        }
        #endregion

        #region メッセージ送信処理
        /// <summary>
        /// メッセージをメッセージキューに追加します。
        /// </summary>
        private void AddMessage(PostComment comment)
        {
            if (comment == null)
            {
                return;
            }

            using (new DebugLock(SyncRoot))
            {
                this.postCommentList.Add(comment);

                // 投稿可能時刻が早い順にソートします。
                // List.Sortは安定なソートでないためこうしています。
                this.postCommentList =
                    this.postCommentList.OrderBy(
                        cm => cm.StartDate).ToList();
            }
        }

        /// <summary>
        /// メッセージキューからメッセージを一つ削除します。
        /// </summary>
        private void RemoveMessage(PostComment comment)
        {
            using (new DebugLock(SyncRoot))
            {
                if (!this.postCommentList.Any())
                {
                    return;
                }

                this.postCommentList.Remove(comment);
            }
        }

        /// <summary>
        /// メッセージキューのメッセージをすべて削除します。
        /// </summary>
        private void ClearMessage()
        {
            using (new DebugLock(SyncRoot))
            {
                this.postCommentList.Clear();
            }
        }

        /// <summary>
        /// メッセージキューからメッセージを一つ取り出します。
        /// </summary>
        private PostComment GetMessage()
        {
            using (new DebugLock(SyncRoot))
            {
                if (!this.postCommentList.Any())
                {
                    return null;
                }

                return this.postCommentList[0];
            }
        }

        /// <summary>
        /// コメントの投稿が可能かどうか調べます。
        /// </summary>
        private bool CanSendComment(PostComment comment)
        {
            using (new DebugLock(SyncRoot))
            {
                var now = DateTime.Now;

                // 投稿可能時刻に達していない場合は、コメントは投稿できません。
                if (now < comment.StartDate)
                {
                    return false;
                }

                // 前回の投稿から規定時間以上経っていない場合は、エラーとなります。
                if (now - this.lastPostTime < this.waitForPostTime)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// PostKeyを取得します。
        /// </summary>
        private string GetPostKey()
        {
            try
            {
                var newBlockNo = (this.lastRes + 1) / 100;

                /*// postkeyがあって、blocknoも変わっていなければ、
                // postkeyを新たに取得する必要はありません。
                if (!string.IsNullOrEmpty(this.postKey) &&
                    newBlockNo == this.prevBlockNo)
                {
                    return this.postKey;
                }*/

                // postkeyを短時間で取得し続けるとコメント投稿が不可能になって
                // しまうため、このような時間制限をつけます。
                // また、前回投稿よりきっちり3秒後にblocknoが変わった場合でも
                // 正しいpostkeyが取得できるよう、
                // 猶予時間を(3.0 - 0.1)秒 としています。
                var waitTime = MinimumWaitTimeForPost - TimeSpan.FromSeconds(0.1);
                var now = DateTime.Now;
                if (now < this.lastGetPostKey + waitTime)
                {
                    return this.postKey;
                }

                // エラー時にも対応できるよう、取得時間は先に設定します。
                this.lastGetPostKey = now;

                // 新たにpostkeyを取得。
                this.postKey = LiveUtil.GetPostKey(
                    this.ThreadId,
                    newBlockNo,
                    this.commentClient.CookieContainer);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "postkeyの取得に失敗しました。");

                this.postKey = null;
            }

            return this.postKey;
        }

        /// <summary>
        /// コメント投稿用の文字列を作成します。
        /// </summary>
        private XElement BuildCommentString(PostComment comment)
        {
            var postkey = GetPostKey();
            if (string.IsNullOrEmpty(postkey))
            {
                Log.Error(this,
                    "postkeyの取得に失敗しました。");

                ClearMessage();
                AddPostMessageWait(TimeSpan.FromSeconds(5.0));
                return null;
            }

            var playerStatus = this.commentClient.PlayerStatus;

            // 前回と同じ内容のコメントは投稿できないので、必要に応じて
            // ０幅文字を追加しています。
            //
            // ０幅文字についてはここを参考にしました。
            // http://nicowiki.com/空白・特殊記号.html
            var text = comment.Text +
                ((this.lastPostText != null && this.lastPostText == comment.Text)
                ? "\u200E" : "");

            // エラー時は同じ内容のコメントを何度も投稿することがあり、
            // 同じコメントの連続投稿規則にひっかっかってしまう可能性があるため、
            // 投稿内容をエラー回数によって少し変えています。
            text += new string('\u200C', this.postTryCount);

            // vposには放送開始からの経過時間を10ms単位で設定します。
            var elapse = DateTime.Now - playerStatus.Stream.BaseTime;

            // 投稿するメッセージを作成します。
            var elem = new XElement("chat",
                new XAttribute("thread", this.ThreadId),
                new XAttribute("ticket", this.ticket),
                new XAttribute("vpos", (int)elapse.TotalMilliseconds / 10),
                new XAttribute("postkey", postkey),
                new XAttribute("user_id", playerStatus.User.UserId),
                new XAttribute("mail", comment.Mail),
                new XAttribute("premium", (playerStatus.User.IsPremium ? "1" : "0")),
                new XAttribute("locale", "jp"),
                text);

            return elem;
        }

        /// <summary>
        /// 同期でメッセージを送信します。
        /// コメントではなく特定のメッセージにのみ使われます。
        /// </summary>
        private void SendMessageSync(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            using (new DebugLock(SyncRoot))
            {
                if (!IsConnected || IsDisconnecting)
                {
                    return;
                }

                var sendData = Encoding.UTF8.GetBytes(message);

                // データを同期的に送信します。
                this.socket.Send(sendData, SocketFlags.None);
            }
        }

        /// <summary>
        /// コメントを非同期で送信します。
        /// </summary>
        public void SendComment(string text, string mail, DateTime startTime)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var comment = new PostComment()
            {
                Text = text,
                Mail = mail,
                StartDate = startTime,
            };
            
            AddMessage(comment);

            // 別スレッド上で投稿します。
            //UpdateSendComment();
        }

        /// <summary>
        /// もしメッセージがあれば、非同期のメッセージ送信処理を行います。
        /// </summary>
        public void UpdateSendComment()
        {
            try
            {
                XElement elem;
                byte[] sendData;

                using (new DebugLock(SyncRoot))
                {
                    if (!IsConnected || IsDisconnecting)
                    {
                        //Log.ErrorMessage(this,
                        //    "コネクションは切断されています。");
                        return;
                    }

                    // 投稿用コメントを取得します。
                    var comment = GetMessage();
                    if (comment == null || !CanSendComment(comment))
                    {
                        return;
                    }

                    // コメント送信中でかつ、対応するchat_resultが返って来ていない場合、
                    // そのコメントは無視します。
                    if (this.sendingComment != null)
                    {
                        Log.Error(this,
                            "対応するchar_resultが返って来ませんでした(Text = {0})",
                            this.sendingComment.Text);

                        // 次のメッセージを送ってしまいます。
                        this.sendingComment = null;
                    }

                    elem = BuildCommentString(comment);
                    sendData = Encoding.UTF8.GetBytes(elem.ToString() + "\0");

                    // コメントの投稿時刻などを設定します。
                    // エラー処理に必要になります。
                    this.lastPostText = elem.Value;
                    this.sendingComment = comment;

                    SetPostMessageWait(DateTime.Now, MinimumWaitTimeForPost);
                }

                // コメントの投稿処理を開始します。
                this.socket.BeginSend(
                    sendData, 0, sendData.Length,
                    SocketFlags.None,
                    SendCommentDone,
                    null);

                Log.Trace(this,
                    "メッセージの送信処理を開始しました。(Text = {0})",
                    elem.Value);
            }
            catch (Exception ex)
            {
                Log.ErrorException(this, ex,
                    "メッセージの非同期送信に失敗しました。");

                using (new DebugLock(SyncRoot))
                {
                    this.sendingComment = null;
                }
            }
        }

        /// <summary>
        /// 非同期のメッセージ送信処理終了後に呼ばれます。
        /// </summary>
        private void SendCommentDone(IAsyncResult result)
        {
            try
            {
                // 他のコメント投稿終了処理はchat_resultメッセージを
                // 受け取った段階で行われます。
                this.socket.EndSend(result);

                Log.Trace(this,
                    "メッセージの送信処理を完了しました。");
            }
            catch (Exception ex)
            {
                Log.ErrorException(this, ex,
                    "メッセージの送信完了処理に失敗しました。");
            }
        }

        /// <summary>
        /// コメント投稿成功時に送られます。
        /// </summary>
        private void OnSendCommentSuccess()
        {
            var tmpComment = this.sendingComment;

            using (new DebugLock(SyncRoot))
            {
                // 送信したメッセージをキューから削除します。
                RemoveMessage(this.sendingComment);

                this.postTryCount = 0;
                this.sendingComment = null;
            }

            // メッセージを削除した後に、イベントを呼びます。
            // LeaveCommentCountの数を調整するためです。
            this.commentClient.OnCommentSentRoom(
                this, tmpComment);
        }

        /// <summary>
        /// コメント投稿失敗時に呼ばれます。
        /// </summary>
        private void OnSendCommentError(ChatResultStatus? status)
        {
            // もう一度投稿してみます。
            using (new DebugLock(SyncRoot))
            {
                Log.Error(this,
                    "chat_resultから失敗報告が来ました。(status={0}, Text={1})",
                    (status == null ? "不明" : status.ToString()),
                    (this.sendingComment == null ? "" : this.sendingComment.Text));

                // エラーコードによる個別のエラー処理を行います。
                switch (status)
                {
                    case ChatResultStatus.NormalError:
                        // 原因)
                        // ・同じ内容のコメントを投稿した
                        // ・連続アクセスエラー
                        // 対策)
                        // エラーカウントなどを使い次の投稿時にはコメントの内容を
                        // 変えるため、上の理由の場合は特にすることはありません。
                        // 連続アクセス時も、対応した長期休憩コードが別途あるため、
                        // 特になにもする必要がありません。
                        break;
                    case ChatResultStatus.Error2:
                        // 原因)
                        // ・分かりません><
                        break;
                    case ChatResultStatus.Error3:
                        // 原因)
                        // ・分かりません><
                        break;
                    case ChatResultStatus.PostKey:
                        // 原因)
                        // ・postkeyの値が不正。
                        // 対策)
                        // キャッシュされているpostkeyの値を初期化し、
                        // 次の投稿時には新しいpostkeyを取得するようにしています。
                        this.postKey = null;
                        break;
                    default:
                        break;
                }

                // コメントの投稿に３回以上連続で失敗した場合は、
                // アクセス多寡などの理由と判断し長い休憩時間を入れます。
                if (this.postTryCount > 2)
                {
                    AddPostMessageWait(AccessDeniedWaitTime);
                    this.postTryCount = 0;

                    // 送信に失敗したコメントも削除します。
                    RemoveMessage(this.sendingComment);
                }
                else
                {
                    this.postTryCount += 1;
                }

                // 投稿中のコメントをnullにし、新しいコメントを
                // 投稿出来るようにします。
                this.sendingComment = null;
            }
        }

        /// <summary>
        /// chat_resultメッセージを処理します。
        /// </summary>
        private void HandleChatResultMessage(XmlNode node)
        {
            using (new DebugLock(SyncRoot))
            {
                // コメントの受信処理がタイムアウトした場合など
                if (!IsSendingMessage)
                {
                    Log.Error(this,
                        "chat_resultに対応するメッセージが存在しません。");
                }

                // エラーコードを取得し、適切な処理を行います。
                var attr = node.Attributes["status"];
                if (attr == null)
                {
                    OnSendCommentError(null);
                }
                else
                {
                    var status = ChatResultUtil.GetStatus(attr.InnerText);
                    if (status == ChatResultStatus.None)
                    {
                        OnSendCommentSuccess();
                    }
                    else
                    {
                        OnSendCommentError(status);
                    }
                }

                // 次のメッセージがあれば、その送信処理を開始します。
                UpdateSendComment();

                Log.Trace(this,
                    "chat_resultメッセージの解析に成功しました。");
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoom(CommentClient client, CommentRoomInfo roomInfo,
                           int index)
        {
            this.commentClient = client;
            this.roomInfo = roomInfo;
            this.index = index;
        }

        ~CommentRoom()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Disconnect();
                }

                this.disposed = true;
            }
        }
    }
}
