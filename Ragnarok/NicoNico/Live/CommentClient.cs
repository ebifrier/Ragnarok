using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Threading;
using System.ComponentModel;

using Ragnarok;
using Ragnarok.Net;
using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// 複数のコメントサーバーをまとめて扱います。
    /// </summary>
    /// <remarks>
    /// アリーナ、立ち見などのコメントルームをまとめて扱います。
    /// 
    /// 接続後は必ず<see cref="StartReceiveMessage(int)"/>を呼び出して、
    /// コメントの受信を開始してください。
    /// </remarks>
    public class CommentClient : ILogObject, IModel
    {
        /// <summary>
        /// デフォルトの接続タイムアウトです。
        /// </summary>
        public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        private readonly object SyncRoot = new object();
        private readonly object ConnectLock = new object();
        private readonly ReentrancyLock sendCommentLock = new ReentrancyLock();
        private List<CommentRoom> roomList = new List<CommentRoom>();
        private int currentRoomIndex = -1;
        private int connectedRoomCount = 0;
        private PlayerStatus playerStatus;
        private PublishStatus publishStatus;
        private LiveInfo liveInfo;
        private CookieContainer cookieContainer;
        private List<PostComment> ownerCommentList = new List<PostComment>();
        private Timer sendTimer;
        private bool isSupressLog = false;

        /// <summary>
        /// プロパティ値変更イベントです。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 放送接続時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<EventArgs> Connected;

        /// <summary>
        /// 放送切断時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<EventArgs> Disconnected;

        /// <summary>
        /// 放送ルーム接続時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<CommentRoomEventArgs> ConnectedRoom;

        /// <summary>
        /// 放送ルーム切断時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<CommentRoomDisconnectedEventArgs> DisconnectedRoom;

        /// <summary>
        /// コメント受信時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<CommentRoomReceivedEventArgs> CommentReceived;

        /// <summary>
        /// コメント送信後に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<CommentRoomSentEventArgs> CommentSent;

        /// <summary>
        /// ログ出力用の名前を取得します。
        /// </summary>
        [DependOnProperty("LiveIdString")]
        public string LogName
        {
            get
            {
                return string.Format(
                    "コメントクライアント[{0}]",
                    LiveIdString);
            }
        }

        /// <summary>
        /// ログイン用クッキーを取得します。
        /// </summary>
        [DependOnProperty("IsConnected")]
        public CookieContainer CookieContainer
        {
            get { return this.cookieContainer; }
        }

        /// <summary>
        /// 放送情報を取得します。
        /// </summary>
        [DependOnProperty("IsConnected")]
        public PlayerStatus PlayerStatus
        {
            get{  return this.playerStatus; }
        }

        /// <summary>
        /// 放送主のみが得られる放送情報を取得します。
        /// </summary>
        [DependOnProperty("IsConnected")]
        public PublishStatus PublishStatus
        {
            get { return this.publishStatus; }
        }

        /// <summary>
        /// 放送情報を取得します。
        /// </summary>
        [DependOnProperty("IsConnected")]
        public LiveInfo LiveInfo
        {
            get { return this.liveInfo; }
        }

        /// <summary>
        /// 放送ＩＤを取得します。
        /// </summary>
        [DependOnProperty("PlayerStatus")]
        public long LiveId
        {
            get
            {
                // CommentRoomから呼ばれる可能性があるため、
                // lockできません。
                var playerStatus = this.playerStatus;
                if (playerStatus == null)
                {
                    return -1;
                }

                return playerStatus.Stream.Id;
            }
        }

        /// <summary>
        /// 放送ＩＤを取得します。
        /// </summary>
        [DependOnProperty("PlayerStatus")]
        public string LiveIdString
        {
            get
            {
                // CommentRoomから呼ばれる可能性があるため、
                // lockできません。
                var playerStatus = this.playerStatus;
                if (playerStatus == null)
                {
                    return "";
                }

                return playerStatus.Stream.IdString;
            }
        }

        /// <summary>
        /// コメントサーバーへ接続しているユーザーのIDを取得します。
        /// </summary>
        [DependOnProperty("PlayerStatus")]
        public int UserId
        {
            get
            {
                // CommentRoomから呼ばれる可能性があるため、
                // lockできません。
                var playerStatus = this.playerStatus;
                if (playerStatus == null)
                {
                    return -1;
                }

                return playerStatus.User.UserId;
            }
        }

        /// <summary>
        /// コメントルーム数を取得します。
        /// </summary>
        [DependOnProperty("IsConnected")]
        public int RoomCount
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.roomList.Count;
                }
            }
        }

        /// <summary>
        /// リスト巡回中にリスト自体が変更されることがあるので、
        /// 一時オブジェクトを作成してから使用します。
        /// </summary>
        private CommentRoom[] ClonedCommentRoomList
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.roomList.ToArray();
                }
            }
        }

        /// <summary>
        /// 全ルーム中で有効なルームのリストを取得します。
        /// </summary>
        /// <remarks>
        /// lockしないので、必ずlockしてから使って下さい。
        /// </remarks>
        private IEnumerable<CommentRoom> AvailableRoomList
        {
            get
            {
                return this.roomList.Where(room => room != null);
            }
        }

        /// <summary>
        /// 今いるコメント部屋のインデックスを取得します。
        /// </summary>
        [DependOnProperty("IsConnected")]
        public int CurrentRoomIndex
        {
            get
            {
                return this.currentRoomIndex;
            }
        }

        /// <summary>
        /// 今いるコメント部屋のシート番号を取得します。
        /// </summary>
        [DependOnProperty("PlayerStatus")]
        public int CurrentRoomSeetNo
        {
            get
            {
                // CommentRoomから呼ばれる可能性があるため、
                // lockできません。
                var playerStatus = this.playerStatus;
                if (playerStatus == null)
                {
                    return -1;
                }

                return playerStatus.User.RoomSeetNo;
            }
        }

        /// <summary>
        /// 各部屋に残っている最大の未投稿コメント数を取得します。
        /// </summary>
        [DependOnProperty("IsConnected")]
        public int LeaveCommentCount
        {
            get
            {
                lock (SyncRoot)
                {
                    var availableRoomList = AvailableRoomList;
                    if (!availableRoomList.Any())
                    {
                        return 0;
                    }

                    return availableRoomList.Max(
                        (room => room.LeaveCommentCount));
                }
            }
        }
  
        /// <summary>
        /// コメント部屋に接続されているか調べます。
        /// </summary>
        public bool IsConnected
        {
            get
            {
                lock (SyncRoot)
                {
                    var availableRoomList = AvailableRoomList;
                    if (!availableRoomList.Any())
                    {
                        return false;
                    }

                    return availableRoomList.Any(
                        (room => room.IsConnected));
                }
            }
        }

        /// <summary>
        /// すべてのコメント部屋に接続されているか調べます。　
        /// </summary>
        public bool IsConnectedAll
        {
            get
            {
                lock (SyncRoot)
                {
                    var availableRoomList = AvailableRoomList;
                    if (!availableRoomList.Any())
                    {
                        return false;
                    }

                    return availableRoomList.All(
                        (room => room.IsConnected));
                }
            }
        }

        /// <summary>
        /// すべてのコメント部屋から接続が切れているか調べます。
        /// </summary>
        [DependOnProperty("IsConnected")]
        public bool IsDisconnectedAll
        {
            get { return !IsConnected; }
        }

        /// <summary>
        /// 受信コメントを破棄するかどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// ニコ生の仕様として、コメントを受信しないと送信できませんが、
        /// 送信だけしたい場合もあると思うので。
        /// </remarks>
        public bool IsSupressLog
        {
            get { return this.isSupressLog; }
            set
            {
                var changed = false;

                lock (SyncRoot)
                {
                    if (this.isSupressLog != value)
                    {
                        this.isSupressLog = value;
                        changed = true;
                    }
                }

                if (changed)
                {
                    this.RaisePropertyChanged("IsSupressLog");
                }
            }
        }

        /// <summary>
        /// <paramref name="roomIndex"/>番目のルームオブジェクトを取得します。
        /// </summary>
        internal CommentRoom GetRoom(int roomIndex)
        {
            lock (SyncRoot)
            {
                if (roomIndex < 0 || this.roomList.Count <= roomIndex)
                {
                    return null;
                }

                return this.roomList[roomIndex];
            }
        }

        /// <summary>
        /// <paramref name="roomIndex"/>番目のルーム情報を取得します。
        /// </summary>
        public CommentRoomInfo GetRoomInfo(int roomIndex)
        {
            lock (SyncRoot)
            {
                if (roomIndex < 0 || this.roomList.Count <= roomIndex)
                {
                    return null;
                }

                return this.roomList[roomIndex].RoomInfo;
            }
        }

        /// <summary>
        /// コメントが今すぐ投稿可能か取得します。
        /// </summary>
        public bool CanPostCommentNow(int roomIndex)
        {
            // CommentRoomのロックとこのオブジェクトのロックを同時に行うと
            // デッドロックする可能性があります。
            //lock (SyncRoot)
            {
                var room = GetRoom(roomIndex);
                if (room == null)
                {
                    return false;
                }

                return room.CanPostCommentNow;
            }
        }

        /// <summary>
        /// プロパティ値の変更を通知します。
        /// </summary>
        public virtual void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            Util.CallPropertyChanged(
                this.PropertyChanged, this, e);
        }

        /// <summary>
        /// ルームへの接続時にイベントを発生させます。
        /// </summary>
        internal void FireConnected()
        {
            this.Connected.SafeRaiseEvent(
                this, EventArgs.Empty);
        }

        /// <summary>
        /// 全ルームからの切断時にイベントを発生させます。
        /// </summary>
        internal void FireDisconnected()
        {
            this.Disconnected.SafeRaiseEvent(
                this, EventArgs.Empty);
        }

        /// <summary>
        /// ルームへの接続時にイベントを発生させます。
        /// </summary>
        internal void FireConnectedRoom(int roomIndex)
        {
            this.ConnectedRoom.SafeRaiseEvent(
                this, new CommentRoomEventArgs(roomIndex));
        }

        /// <summary>
        /// ルームからの切断時にイベントを発生させます。
        /// </summary>
        internal void FireDisconnectedRoom(int roomIndex,
                                           DisconnectReason reason)
        {
            this.DisconnectedRoom.SafeRaiseEvent(
                this, new CommentRoomDisconnectedEventArgs(roomIndex, reason));
        }

        /// <summary>
        /// コメント部屋に接続したときに呼ばれます。
        /// </summary>
        internal void OnConnectedRoom(CommentRoom sender)
        {
            // 接続しているルーム数を増やします。
            // プロパティ値変更を正しく伝えるために必要です。
            var count = Interlocked.Increment(ref this.connectedRoomCount);
            if (count == 1)
            {
                this.RaisePropertyChanged("IsConnected");
            }
            if (count == this.RoomCount)
            {
                this.RaisePropertyChanged("IsConnectedAll");
            }

            if (sender != null)
            {
                FireConnectedRoom(sender.Index);
            }
        }

        /// <summary>
        /// コメント部屋から切断されたときに呼ばれます。
        /// </summary>
        internal void OnDisconnectedRoom(CommentRoom sender, DisconnectReason reason)
        {
            // Connectedとの関係で先に呼びます。
            if (sender != null)
            {
                FireDisconnectedRoom(sender.Index, reason);
            }

            // 接続しているルーム数を減らします。
            // プロパティ値変更を正しく伝えるために必要です。
            var count = Interlocked.Decrement(ref this.connectedRoomCount);

            // 全ルームからの接続が途切れたときにのみ。
            if (count == this.RoomCount - 1)
            {
                this.RaisePropertyChanged("IsConnectedAll");
            }

            // 全部屋の切断時はオブジェクトを初期化します。
            if (count == 0)
            {
                NotifyDisconnect();

                // 切断処理後じゃないと、プロパティの値が変わっていません。
                this.RaisePropertyChanged("IsConnected");
            }
        }

        /// <summary>
        /// コメント受信時に呼ばれます。
        /// </summary>
        internal void OnCommentReceivedRoom(CommentRoom sender, Comment comment)
        {
            this.CommentReceived.SafeRaiseEvent(
                this,
                new CommentRoomReceivedEventArgs(sender.Index, comment));
        }

        /// <summary>
        /// コメント送信時に呼ばれます。
        /// </summary>
        internal void OnCommentSentRoom(CommentRoom sender, PostComment comment)
        {
            this.CommentSent.SafeRaiseEvent(
                this,
                new CommentRoomSentEventArgs(sender.Index, comment));

            // 未送信のコメント数が変わった可能性があります。
            this.RaisePropertyChanged("LeaveCommentCount");
        }

        /// <summary>
        /// コメントサーバーに接続します。
        /// </summary>
        public void Connect(string liveUrl, CookieContainer cc)
        {
            // １０秒間は待ちます。
            Connect(liveUrl, cc, false, DefaultTimeout);
        }

        /// <summary>
        /// タイムアウトつきでコメントサーバーに接続します。
        /// </summary>
        public void Connect(string liveUrl, CookieContainer cc,
                            bool currentRoomOnly)
        {
            var playerStatus = PlayerStatus.Create(liveUrl, cc);

            Connect(playerStatus, cc, currentRoomOnly, DefaultTimeout);
        }

        /// <summary>
        /// タイムアウトつきでコメントサーバーに接続します。
        /// </summary>
        public void Connect(string liveUrl, CookieContainer cc,
                            bool currentRoomOnly, TimeSpan timeout)
        {
            var playerStatus = PlayerStatus.Create(liveUrl, cc);

            Connect(playerStatus, cc, currentRoomOnly, timeout);
        }

        /// <summary>
        /// コメントサーバーに接続します。
        /// </summary>
        /// <remarks>
        /// 接続失敗時には例外が返されます。
        /// </remarks>
        public void Connect(PlayerStatus playerStatus, CookieContainer cc,
                            TimeSpan timeout)
        {
            Connect(playerStatus, cc, false, timeout);
        }

        /// <summary>
        /// コメントサーバーに接続します。
        /// </summary>
        /// <remarks>
        /// 接続失敗時には例外が返されます。
        /// </remarks>
        public void Connect(PlayerStatus playerStatus, CookieContainer cc,
                            bool currentRoomOnly, TimeSpan timeout)
        {
            lock (ConnectLock)
            {
                if (playerStatus == null)
                {
                    throw new ArgumentNullException("playerStatus");
                }

                // 例外が発生する可能性があります。
                var streamInfo = LiveUtil.GetLiveStreamInfo(playerStatus, cc);
                
                // 各コメントルームの情報を取得します。
                var roomInfoList = GetAllRoomInfo(
                    streamInfo.PlayerStatus,
                    streamInfo.LiveInfo.CommunityLevel);
                var currentRoomIndex = FindRoomIndex(
                    playerStatus.MS.Port, roomInfoList);
                var roomList = new List<CommentRoom>();

                // 各コメントルームに接続します。
                for (var i = 0; i < roomInfoList.Count(); ++i)
                {
                    if (currentRoomOnly && i != currentRoomIndex)
                    {
                        roomList.Add(null);
                        continue;
                    }

                    var room = new CommentRoom(this, roomInfoList[i], i);
                    // 接続に失敗した場合、例外が返ります。
                    room.Connect(cc, timeout);
                    roomList.Add(room);
                }

                Disconnect();
                lock (SyncRoot)
                {
                    this.connectedRoomCount = 0;
                    this.playerStatus = streamInfo.PlayerStatus;
                    this.publishStatus = streamInfo.PublishStatus;
                    this.liveInfo = streamInfo.LiveInfo;
                    this.roomList = roomList;
                    this.currentRoomIndex = currentRoomIndex;
                    this.cookieContainer = cc;
                }

                // フィールド値を設定した後に、OnConnectedRoomを呼びます。
                foreach (var room in ClonedCommentRoomList)
                {
                    OnConnectedRoom(room);
                }

                // 接続時のイベントを発生させます。
                FireConnected();
            }
        }

        /// <summary>
        /// 全コメントルームのポート情報などを取得します。
        /// </summary>
        private CommentRoomInfo[] GetAllRoomInfo(PlayerStatus playerStatus,
                                                 int communityLevel)
        {
            var creator = Detail.LiveInfoCreatorUtil.CreateCreator(
                playerStatus.Stream.ProviderType);

            return creator.GetAllRoomInfo(playerStatus, communityLevel);
        }

        /// <summary>
        /// 指定のポートを持つコメント部屋のインデックスを取得します。
        /// </summary>
        private int FindRoomIndex(int port, CommentRoomInfo[] roomList)
        {
            return Array.FindIndex(roomList, room => (room.Port == port));
        }

        /// <summary>
        /// コメントの受信処理を開始します。
        /// </summary>
        public void StartReceiveMessage(int resFrom)
        {
            // デフォルトでは５秒間待ちます。
            StartReceiveMessage(resFrom, 5 * 1000);
        }

        /// <summary>
        /// コメントの受信処理を開始します。
        /// </summary>
        public void StartReceiveMessage(int resFrom, int timeout)
        {
            if (!IsConnected)
            {
                throw new NicoLiveException("放送に接続されていません。");
            }

            // this.roomListにroomListを設定してからコメントを受け付けたいため、
            // ここでメッセージの受信処理を開始しています。
            //
            // コメント受信後にコメントを投稿するような設定になっている場合、
            // コメント投稿にはこのオブジェクトの情報を使うため、
            // ロックオブジェクトの競合が起こる可能性があります。
            // なので、ロックしない状態でこの処理を行っています。
            var tmpRoomList = ClonedCommentRoomList;
            foreach (var room in tmpRoomList)
            {
                if (room != null)
                {
                    room.StartReceiveMessage(resFrom, timeout);
                }
            }
        }

        /// <summary>
        /// コメントサーバーから切断します。
        /// </summary>
        public void Disconnect()
        {
            lock (ConnectLock)
            {
                if (!IsConnected)
                {
                    // 接続されておりません。
                    return;
                }

                // this.roomListはDisconnect中に変更されることがあるので、
                // ここで一時オブジェクトを作成します。
                var tmpRoomList = ClonedCommentRoomList;

                // ここで各ルームの接続が切られ、
                // OnDisconnectedRoomが呼ばれることがあります。
                foreach (var room in tmpRoomList)
                {
                    if (room != null)
                    {
                        room.Disconnect();
                    }
                }
            }
        }

        /// <summary>
        /// 全部屋からの接続が切られたときに呼ばれます。
        /// </summary>
        private void NotifyDisconnect()
        {
            // ここでイベントが呼ばれます。
            FireDisconnected();

            lock (SyncRoot)
            {
                this.connectedRoomCount = 0;
                this.currentRoomIndex = -1;
                this.cookieContainer = null;
                this.playerStatus = null;
                this.publishStatus = null;
                this.roomList.Clear();
            }

            ClearOwnerComment();
        }

        /// <summary>
        /// 184IDを設定します。
        /// </summary>
        internal void SetAnonymousId(string id)
        {
            var tmpRoomList = ClonedCommentRoomList;

            // CommentRoomから呼ばれるメソッドのため、
            // 下手にロックするとデッドロックする可能性があります。
            foreach (var room in tmpRoomList)
            {
                if (room != null)
                {
                    room.AnonymousId = id;
                }
            }
        }

        /// <summary>
        /// 184idを調べるためのコメントを送ります。
        /// </summary>
        public void SendCommentToCheckAnonymousId()
        {
            var room = GetRoom(this.currentRoomIndex);
            if (room == null)
            {
                return;
            }

            // 184IDは全部屋共通なので、一つの部屋に
            // コメントを送るだけで調べることができます。
            room.SendCommentToCheckAnonymousId();
        }

        /// <summary>
        /// 今いる部屋のみにコメントを送信します。
        /// </summary>
        public void SendComment(string text, string mail)
        {
            SendComment(text, mail, DateTime.Now);
        }

        /// <summary>
        /// 今いる部屋のみにコメントを送信します。
        /// </summary>
        public void SendComment(string text, string mail, DateTime startTime)
        {
            SendComment(CurrentRoomIndex, text, mail, startTime);
        }

        /// <summary>
        /// 指定の部屋にコメントを送信します。
        /// </summary>
        public void SendComment(int roomIndex, string text, string mail)
        {
            SendComment(roomIndex, text, mail, DateTime.Now);
        }

        /// <summary>
        /// 指定の部屋にコメントを送信します。
        /// </summary>
        public void SendComment(int roomIndex, string text, string mail,
                                DateTime startTime)
        {
            var room = GetRoom(roomIndex);
            if (room == null)
            {
                return;
            }

            room.SendComment(text, mail, startTime);
        }

        /// <summary>
        /// 全部屋にコメントを送信します。
        /// </summary>
        public void BroadcastComment(string text, string mail)
        {
            BroadcastComment(text, mail, DateTime.Now);
        }

        /// <summary>
        /// 全部屋にコメントを送信します。
        /// </summary>
        public void BroadcastComment(string text, string mail,
                                     DateTime startTime)
        {
            var tmpRoomList = ClonedCommentRoomList;

            foreach (var room in tmpRoomList)
            {
                if (room == null)
                {
                    continue;
                }

                room.SendComment(text, mail, startTime);
            }
        }

        /// <summary>
        /// メッセージをメッセージキューに追加します。
        /// </summary>
        private void EnqueueOwnerComment(PostComment comment)
        {
            if (comment == null)
            {
                return;
            }

            lock (this.ownerCommentList)
            {
                this.ownerCommentList.Add(comment);

                // 投稿可能時刻が早い順にソートします。
                // List.Sortは安定なソートでないためこうしています。
                this.ownerCommentList =
                    this.ownerCommentList.OrderBy(
                        cm => cm.StartDate).ToList();
            }
        }

        /// <summary>
        /// メッセージキューからメッセージを削除し、それを返します。
        /// </summary>
        private PostComment DequeueOwnerComment(DateTime time)
        {
            lock (this.ownerCommentList)
            {
                if (!this.ownerCommentList.Any())
                {
                    return null;
                }

                // 投稿時刻が来てなければ投稿しません。
                var comment = this.ownerCommentList[0];
                if (time < comment.StartDate)
                {
                    return null;
                }

                this.ownerCommentList.RemoveAt(0);
                return comment;
            }
        }

        /// <summary>
        /// メッセージキューのメッセージをすべて削除します。
        /// </summary>
        private void ClearOwnerComment()
        {
            lock (this.ownerCommentList)
            {
                this.ownerCommentList.Clear();
            }
        }

        /// <summary>
        /// 放送主コメントを送信します。
        /// </summary>
        public void SendOwnerComment(string text, string mail)
        {
            var comment = new PostComment()
            {
                Text = text,
                Mail = mail,
                StartDate = DateTime.Now,
            };

            SendOwnerComment(comment);
        }

        /*/// <summary>
        /// 放送主コメントを送信します。
        /// </summary>
        public void SendOwnerComment(string text, string mail, DateTime sendTime)
        {
            var comment = new PostComment()
            {
                Text = text,
                Mail = mail,
                StartDate = sendTime,
            };

            SendOwnerComment(comment);
        }*/

        /// <summary>
        /// 放送主コメントを送信します。
        /// </summary>
        public void SendOwnerComment(string text, string mail, string name)
        {
            var comment = new PostComment()
            {
                Text = text,
                Mail = mail,
                Name = name,
                StartDate = DateTime.Now,
            };

            SendOwnerComment(comment);
        }

        /// <summary>
        /// 放送主コメントを送信します。
        /// </summary>
        public void SendOwnerComment(PostComment comment)
        {
            if (comment == null || !comment.Validate())
            {
                return;
            }

            lock (SyncRoot)
            {
                if (this.publishStatus == null)
                {
                    throw new NicoLiveException(LiveStatusCode.PermissionDenied);
                }
                
                if (string.IsNullOrEmpty(this.publishStatus.Stream.Token))
                {
                    Log.Error(this,
                        "tokenがないため放送主コメントを送ることができません。");

                    throw new NicoLiveException(LiveStatusCode.PermissionDenied);
                }

                // コメントを追加した後、コメント処理を行います。
                EnqueueOwnerComment(comment);

                // コメント投稿に時間がかかる場合があるため、
                // 送信処理は必ず別スレッド上で行うようにします。
                //UpdateOwnerComment();
            }
        }

        /// <summary>
        /// 放送主コメントの投稿処理を行います。
        /// </summary>
        private void UpdateOwnerComment()
        {
            try
            {
                var publishStatus = this.publishStatus;
                if (publishStatus == null)
                {
                    return;
                }

                // コメントを取り出します。
                var comment = DequeueOwnerComment(DateTime.Now);
                if (comment == null)
                {
                    return;
                }

                // 実際の投稿処理を行います。
                WebUtil.RequestHttpAsync(
                    NicoString.GetBroadcastCommentUrl(this.LiveId),
                    NicoString.MakeBroadcastCommentData(
                        comment.Text,
                        comment.Mail,
                        comment.Name,
                        publishStatus.Stream.Token),
                    this.cookieContainer,
                    OwnerCommentSentDone);
            }
            catch (Exception ex)
            {
                Log.ErrorException(this, ex,
                    "放送主コメントの投稿に失敗しました。");

                OwnerCommentSentDone(null, null);
            }
        }

        /// <summary>
        /// 放送主コメント投稿後に呼ばれます。
        /// </summary>
        private void OwnerCommentSentDone(IAsyncResult result, byte[] data)
        {
        }

        /// <summary>
        /// コメント投稿の更新を行います。
        /// </summary>
        public void UpdateSendComment()
        {
            using (var result = this.sendCommentLock.Lock())
            {
                // タイマークラスは処理が終わっていなくても
                // 等間隔でメソッドを呼び続けます。
                // そのための対策です。
                // http://msdn.microsoft.com/ja-jp/library/system.threading.timer(v=vs.80).aspx
                if (result == null) return;

                // 通常コメントの更新を行います。
                foreach (var room in ClonedCommentRoomList)
                {
                    if (room == null)
                    {
                        continue;
                    }

                    room.UpdateSendComment();
                }

                // オーナーコメントの更新を行います。
                UpdateOwnerComment();
            }
        }

        /// <summary>
        /// コメント送信処理用のタイマーのオン/オフを切り替えます。
        /// </summary>
        /// <remarks>
        /// コメント投稿は指定のタイミングで行う必要があるため、タイマーが
        /// 必要になります。
        /// 
        /// デフォルトではオンですが、コメントを送信しないことが分かっている
        /// 場合や、大量のCommencClientオブジェクトを作成する場合などは
        /// コメント投稿用のタイマーを切るとパフォーマンスが良くなります。
        /// </remarks>
        public void SetSendTimer(bool on)
        {
            if (on)
            {
                this.sendTimer.Change(
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(200));
            }
            else
            {
                this.sendTimer.Change(
                    Timeout.Infinite,
                    Timeout.Infinite);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentClient()
        {
            this.sendTimer = new Timer(
                _ => UpdateSendComment());

            SetSendTimer(true);
        }
    }
}
