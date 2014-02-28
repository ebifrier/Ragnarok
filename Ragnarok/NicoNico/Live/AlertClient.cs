using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace Ragnarok.NicoNico.Live
{
    using Ragnarok.Utility;

    /// <summary>
    /// 生放送のアラート情報を通知するときに使われます。
    /// </summary>
    public class LiveAlertedEventArgs : EventArgs
    {
        /// <summary>
        /// 開始した放送ＩＤを取得します。
        /// </summary>
        public int LiveId
        {
            get;
            private set;
        }

        /// <summary>
        /// 開始した放送ＩＤを取得します。
        /// </summary>
        public string LiveIdString
        {
            get { return string.Format("lv{0}", LiveId); }
        }

        /// <summary>
        /// 放送提供者を取得します。
        /// </summary>
        public ProviderData ProviderData
        {
            get;
            private set;
        }

        /// <summary>
        /// 放送主がいれば、そのIDを取得します。
        /// </summary>
        public int? UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LiveAlertedEventArgs(int liveId, ProviderData providerData,
                                    int userId)
        {
            this.LiveId = liveId;
            this.ProviderData = providerData;
            this.UserId = userId;
        }
    }

    /// <summary>
    /// ニコニコ生放送のアラート情報(ログイン無し)を受け取るクラスです。
    /// </summary>
    /// <remarks>
    /// <para>
    /// Connect後から、生放送が開始される度にLiveAlertedイベントが
    /// 非同期で呼ばれます。
    /// </para>
    /// <para>
    /// サーバーから送られる生放送IDには、同じ値が含まれることがあります。
    /// そのため、このクラスでは受信した放送IDの履歴を作っておき、
    /// それと照らし合わせることで重複を防ぐようになっています。
    /// </para>
    /// </remarks>
    public class AlertClient
    {
        private readonly object SyncRoot = new object();
        private Socket socket = null;
        private AlertInfo alertInfo = null;
        private readonly BinarySplitReader reader;
        private readonly LinkedList<long> historyBuffer = new LinkedList<long>();
        private bool useHistoryBuffer = true;
        private int historyBufferMaxSize = 256;

        /// <summary>
        /// 生放送の開始を通知するイベントです。
        /// </summary>
        public event EventHandler<LiveAlertedEventArgs> LiveAlerted;

        /// <summary>
        /// アラート情報の受信を通知します。
        /// </summary>
        private void OnLiveAlerted(LiveAlertedEventArgs e)
        {
            var handler = this.LiveAlerted;

            if (handler != null)
            {
                Util.SafeCall(() => handler(this, e));
            }
        }

        /// <summary>
        /// アラートサーバーに接続しているかどうかを取得します。
        /// </summary>
        public bool IsConnected
        {
            get
            {
                var socket = this.socket;

                return (socket != null && socket.Connected);
            }
        }

        /// <summary>
        /// 履歴バッファを使うかどうかを取得または設定します。
        /// </summary>
        public bool UseHistoryBuffer
        {
            get { return this.useHistoryBuffer; }
            set { this.useHistoryBuffer = value; }
        }

        /// <summary>
        /// 履歴バッファに含める放送IDの最大数を取得または設定します。
        /// </summary>
        public int HistoryBufferMaxSize
        {
            get { return this.historyBufferMaxSize; }
            set { this.historyBufferMaxSize = value; }
        }

        /// <summary>
        /// アラート情報サーバーに接続します。
        /// </summary>
        public void Connect()
        {
            // まずはアラート情報を取得します。
            var alertInfo = AlertInfo.Create();

            var socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            // 指定のアドレス・ポートにつなぎます。
            socket.Connect(
                alertInfo.MS.Address,
                alertInfo.MS.Port);

            if (!socket.Connected)
            {
                throw new NicoLiveException(LiveStatusCode.NetworkError);
            }

            lock (SyncRoot)
            {
                Disconnect();

                this.socket = socket;
                this.alertInfo = alertInfo;
            }

            StartReceiveAlert();
        }

        /// <summary>
        /// アラートサーバーから切断します。
        /// </summary>
        public void Disconnect()
        {
            lock (SyncRoot)
            {
                if (this.socket == null)
                {
                    return;
                }

                this.socket.Disconnect(false);
                this.socket = null;
                this.alertInfo = null;
            }
        }

        /// <summary>
        /// アラート情報の取得を開始します。
        /// </summary>
        protected void StartReceiveAlert()
        {
            lock (SyncRoot)
            {
                // アラート情報の受信を開始するためのメッセージです。
                var message = NicoString.MakeThreadStart(
                    this.alertInfo.MS.ThreadId, 1);

                var buffer = Encoding.UTF8.GetBytes(message);
                this.socket.Send(buffer);

                StartReceiveAlertLoop();
            }
        }

        /// <summary>
        /// アラート情報の取得ループを開始します。
        /// </summary>
        private void StartReceiveAlertLoop()
        {
            lock (SyncRoot)
            {
                if (!IsConnected)
                {
                    return;
                }

                var buffer = new byte[128];

                this.socket.BeginReceive(
                    buffer, 0, buffer.Length,
                    SocketFlags.Partial,
                    MessageReceived,
                    buffer);
            }
        }

        /// <summary>
        /// メッセージ受信時に呼ばれます。
        /// </summary>
        private void MessageReceived(IAsyncResult result)
        {
            var messageList = new List<string>();

            lock (SyncRoot)
            {
                try
                {
                    var buffer = (byte[])result.AsyncState;
                    var size = this.socket.EndReceive(result);
                    if (size <= 0)
                    {
                        Disconnect();
                        return;
                    }

                    this.reader.Write(buffer, 0, size);

                    // 受信データは'\0'で区切りになっています。
                    byte[] data;
                    while ((data = this.reader.ReadUntil(0)) != null)
                    {
                        var message = Encoding.UTF8.GetString(data);

                        messageList.Add(message);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "受信データの解析に失敗しました。");
                    return;
                }
            }

            // 分割されたメッセージを処理していきます。
            foreach (var message in messageList)
            {
                try
                {
                    HandleMessage(message);
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "アラートメッセージの処理に失敗しました。");
                }
            }

            StartReceiveAlertLoop();
        }

        /// <summary>
        /// アラートメッセージを処理します。
        /// </summary>
        private void HandleMessage(string message)
        {
            var doc = new XmlDocument();
            doc.LoadXml(message);

            var node = doc.DocumentElement;
            if (node.Name != "chat")
            {
                return;
            }

            Log.Trace("Alert {0}", node.InnerText);

            // 内部テキストは
            //   [放送ID],[チャンネル/コミュニティＩＤ/official],[ユーザーＩＤ]
            // となっています。
            var values = node.InnerText.Split(',');
            if (values.Length < 3)
            {
                return;
            }

            // 放送IDを取得します。
            var liveId = StrUtil.ToInt(values[0], 0);
            if (liveId <= 0)
            {
                return;
            }

            // 提供者を取得します。
            var providerData = LiveUtil.ParseProvider(values[1]);
            if (providerData == null)
            {
                return;
            }

            // ユーザー情報を取得します。
            var userId = StrUtil.ToInt(values[2], -1);

            // IDが重複して送られてくることがあるので、
            // すでに受信した放送IDならば無視します。
            if (!IsContainsLiveId(liveId))
            {
                var e = new LiveAlertedEventArgs(
                    liveId, providerData, userId);
                OnLiveAlerted(e);

                AddLiveIdToHistoryBuffer(liveId);
            }
        }

        /// <summary>
        /// 受信した放送IDを履歴バッファに追加します。
        /// </summary>
        /// <remarks>
        /// すでに受信したIDを再度処理しないように使います。
        /// </remarks>
        private void AddLiveIdToHistoryBuffer(long liveId)
        {
            if (!this.useHistoryBuffer)
            {
                return;
            }

            lock (this.historyBuffer)
            {
                var node = this.historyBuffer.Last;

                // nodeを後ろ方向に巡るため、放送IDは次第に
                // 小さい値になっていきます。
                while (node != null)
                {
                    // nodeの値が指定のIDより小さくなるまで巡回します。
                    if (node.Value < liveId)
                    {
                        break;
                    }

                    node = node.Previous;
                }

                // 適切な位置が見つからなければ、先頭に新要素を追加します。
                if (node == null)
                {
                    this.historyBuffer.AddFirst(liveId);
                }
                else
                {
                    this.historyBuffer.AddAfter(node, liveId);
                }

                // 履歴バッファのサイズは、指定のサイズ以上にはしません。
                while (this.historyBuffer.Count > this.historyBufferMaxSize)
                {
                    this.historyBuffer.RemoveFirst();
                }
            }
        }

        /// <summary>
        /// この放送IDがすでに受信したIDか調べます。
        /// </summary>
        private bool IsContainsLiveId(long liveId)
        {
            if (!this.useHistoryBuffer)
            {
                return false;
            }

            lock (this.historyBuffer)
            {
                var node = this.historyBuffer.Last;

                // nodeを後ろ方向に巡るため、放送IDは次第に
                // 小さい値になっていきます。
                while (node != null)
                {
                    if (node.Value == liveId)
                    {
                        return true;
                    }

                    if (node.Value < liveId)
                    {
                        return false;
                    }

                    node = node.Previous;
                }

                return false;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AlertClient()
        {
            //this.stream = new MemoryStream();
            this.reader = new BinarySplitReader(2048);
        }
    }
}
