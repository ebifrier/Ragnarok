using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Ragnarok.Net
{
    /// <summary>
    /// コネクションを扱います。
    /// </summary>
    /// <remarks>
    /// コネクションの接続、切断、ログ出力などの機能を備えます。
    /// </remarks>
    public class Connection : ILogObject, IDisposable
    {
        private static readonly int[] idCounter = new int[] { 0 };
        private readonly object socketLock = new object();
        private int id = -1;
        private string name = "コネクション";
        private Socket socket;
        private volatile bool isConnecting = false;
        private volatile bool isDisconnecting = false;
        private readonly List<byte[]> sendDataList = new List<byte[]>();
        private byte[] sendingData = null;
        private bool disposed = false;

        /// <summary>
        /// Connectのタイムアウト時間です。
        /// </summary>
        public static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 接続時のイベントです。
        /// </summary>
        public event EventHandler<ConnectEventArgs> Connected;

        /// <summary>
        /// 切断時のイベントです。
        /// </summary>
        public event EventHandler<DisconnectEventArgs> Disconnected;

        /// <summary>
        /// データ受信時のイベントです。
        /// </summary>
        public event EventHandler<DataEventArgs> Received;

        /// <summary>
        /// データ送信時のイベントです。
        /// </summary>
        public event EventHandler<DataEventArgs> Sent;

        /// <summary>
        /// ログ出力用の名前を取得します。
        /// </summary>
        public string LogName
        {
            get
            {
                var name = this.Name ?? "";

                return string.Format("{0}[{1,8:D8}]", name, this.Id);
            }
        }

        /// <summary>
        /// ログ出力用のコネクションＩＤを取得します。
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }
            protected set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// ログ出力用の名前を取得します。
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// ソケットを取得します。
        /// </summary>
        public Socket Socket
        {
            get
            {
                return this.socket;
            }
            private set
            {
                this.socket = value;
            }
        }

        /// <summary>
        /// 接続中のSocketを持っているかどうかを取得します。
        /// </summary>
        public bool IsConnected
        {
            get
            {
                var sock = this.socket;

                return (sock != null && sock.Connected);
            }
        }

        /// <summary>
        /// ソケットが切断中かどうかを取得します。
        /// </summary>
        public bool IsDisconnecting
        {
            get
            {
                return this.isDisconnecting;
            }
        }

        /// <summary>
        /// メッセージを送信中かどうかを取得します。
        /// </summary>
        public bool IsSendingData
        {
            get
            {
                return (this.sendingData != null);
            }
        }

        /// <summary>
        /// ソケットを直接設定します。
        /// </summary>
        public void SetSocket(Socket socket)
        {
            // 前のソケットの接続を切り、新しいソケットを設定します。
            Disconnect();

            lock (this.socketLock)
            {
                this.socket = socket;
                this.isDisconnecting = false;
                this.sendDataList.Clear();
                this.sendingData = null;

                // keepaliveを設定し、不要なソケットを削除するようにします。
                this.socket.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.KeepAlive,
                    true);
            }

            Log.Debug(this,
                "ソケットが設定されました。");

            // 接続済みのソケットの場合は、接続処理を行います。
            // 処理は必ずロックの外側で行います。
            if (IsConnected)
            {
                OnConnected();

                // データの受信を開始します。
                StartReceiveData();
            }
        }

        /// <summary>
        /// サーバーに接続します。
        /// </summary>
        public void Connect(string address, int port)
        {
            if (this.isConnecting)
            {
                throw new InvalidOperationException(
                    "すでにサーバーに接続中です。");
            }

            this.isConnecting = true;
            try
            {
                var socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                // ソケットの接続処理を開始します。
                var ar = socket.BeginConnect(
                    address,
                    port,
                    (result) =>
                    {
                        try
                        {
                            socket.EndConnect(result);
                        }
                        catch (Exception)
                        {
                        }
                    },
                    null);

                if (!ar.AsyncWaitHandle.WaitOne(ConnectTimeout))
                {
                    socket.Close();

                    throw new RagnarokNetException(
                        "コネクションの接続がタイムアウトしました。");
                }

                // 接続に失敗したら例外を投げます。
                if (!socket.Connected)
                {
                    socket.Close();

                    throw new RagnarokNetException(
                        "コネクションの接続に失敗しました。");
                }

                SetSocket(socket);
            }
            finally
            {
                this.isConnecting = false;
            }
        }

        /*/// <summary>
        /// 非同期的な接続を開始します。
        /// </summary>
        public void BeginConnect(IPAddress address, int port,
                                 Action<object, Exception> callback)
        {
            lock (this.connectLock)
            {
                try
                {
                    // ソケット接続中に接続することはできません。
                    if (this.isConnecting)
                    {
                        throw new InvalidOperationException(
                            "すでに接続処理が開始しています。");
                    }

                    this.isConnecting = true;

                    var socket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Stream,
                        ProtocolType.Tcp);

                    // ソケットの接続処理を開始します。
                    socket.BeginConnect(
                        address,
                        port,
                        (result) => EndConnect(socket, result, callback),
                        socket);
                }
                catch
                {
                    this.isConnecting = false;

                    throw;
                }
            }
        }

        /// <summary>
        /// 非同期接続終了時に呼ばれます。
        /// </summary>
        private void EndConnect(Socket socket, IAsyncResult result,
                                Action<object, Exception> callback)
        {
            lock (this.connectLock)
            {
                try
                {
                    socket.EndConnect(result);

                    SetSocket(socket);

                    if (callback != null)
                    {
                        callback(this, null);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(this, ex,
                        "ソケットの非同期接続に失敗しました。");

                    if (callback != null)
                    {
                        callback(this, ex);
                    }
                }

                this.isConnecting = false;
            }
        }*/

        /// <summary>
        /// サーバーに接続したときに呼ばれます。
        /// </summary>
        protected virtual void OnConnected()
        {
            var handler = Connected;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, new ConnectEventArgs()));
            }
        }

        /// <summary>
        /// ソケットの終了処理を行います。
        /// </summary>
        public void Shutdown(SocketShutdown shutdown)
        {
            try
            {
                lock (this.socketLock)
                {
                    if (this.socket == null || this.isDisconnecting)
                    {
                        return;
                    }

                    // ソケットの切断処理を開始します。
                    this.isDisconnecting = true;
                    this.socket.Shutdown(shutdown);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(this, ex,
                    "ソケットのShutdownに失敗しました。");

                NotifyDisconnected(DisconnectReason.Error);
            }
        }

        /// <summary>
        /// サーバーとの接続を切断します。
        /// </summary>
        public void Disconnect()
        {
            try
            {
                lock (this.socketLock)
                {
                    if (this.socket == null)
                    {
                        return;
                    }

                    // ソケットの切断処理を開始します。
                    this.socket.Shutdown(SocketShutdown.Both);
                }

                NotifyDisconnected(DisconnectReason.Disconnected);
            }
            catch (Exception ex)
            {
                Log.ErrorException(this, ex,
                    "ソケットの切断に失敗しました。");

                NotifyDisconnected(DisconnectReason.Error);
            }
        }

        /// <summary>
        /// 何らかの理由で接続が切断されたときに呼ばれます。
        /// </summary>
        protected void NotifyDisconnected(DisconnectReason reason)
        {
            lock (this.socketLock)
            {
                if (this.socket == null)
                {
                    return;
                }

                this.socket.Disconnect(false);
                this.socket.Close();
                this.socket = null;

                this.sendDataList.Clear();
                this.sendingData = null;
                this.isDisconnecting = false;
            }

            Log.Debug(this,
                "ソケットが切断されました。(理由={0})", reason);

            OnDisconnected(reason);
        }

        /// <summary>
        /// サーバーから切断されたときに呼ばれます。
        /// </summary>
        protected virtual void OnDisconnected(DisconnectReason reason)
        {
            var handler = Disconnected;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, new DisconnectEventArgs(reason)));
            }
        }

        #region メッセージ受信処理
        /// <summary>
        /// データの受信を開始します。
        /// </summary>
        private void StartReceiveData()
        {
            var socket = null as Socket;

            lock (this.socketLock)
            {
                if (!IsConnected || IsDisconnecting)
                {
                    return;
                }

                socket = this.socket;
            }

            // BeginReceiveから呼ばれるコールバックは
            // このメソッドが走っているスレッドと同じになることがあります。
            // 
            // コールバックをlockされた状態で呼んでしまうと
            // 紆余曲折を経てデッドロックされてしまうことがあるため、
            // BeginReceiveはlockステートメントの外側から呼んでいます。
            var buffer = new byte[512];

            socket.BeginReceive(
                buffer, 0, buffer.Length,
                SocketFlags.None,
                ReceiveDataDone,
                buffer);

            Log.Trace(this,
                "データの受信処理を開始しました。");
        }

        /// <summary>
        /// メッセージの受信が行われたときに呼ばれます。
        /// </summary>
        private void ReceiveDataDone(IAsyncResult result)
        {
            try
            {
                byte[] data = null;
                int size = -1;

                lock (this.socketLock)
                {
                    var socket = this.socket;
                    if (socket == null)
                    {
                        return;
                    }

                    data = (byte[])result.AsyncState;
                    size = socket.EndReceive(result);
                }

                // サイズが０ならばサーバーから接続が切断されています。
                if (size == 0)
                {
                    NotifyDisconnected(DisconnectReason.DisconnectedByOpposite);
                    return;
                }

                Log.Trace(this,
                    "データを受信しました。({0}bytes)",
                    size);

                // HandleReceivedDataはlockの外側で呼び出します。
                RaiseReceived(data, size, null);

                // 次のコメントの受信を開始します。
                StartReceiveData();
            }
            catch (SocketException error)
            {
                RaiseReceived(null, -1, error);

                // エラー判定にはしません。
                NotifyDisconnected(DisconnectReason.DisconnectedByOpposite);
            }
            catch (Exception error)
            {
                RaiseReceived(null, -1, error);

                // エラー判定にします。
                NotifyDisconnected(DisconnectReason.Error);
            }
        }

        /// <summary>
        /// データ受信時に呼ばれます。
        /// </summary>
        protected void RaiseReceived(byte[] data, int length, Exception error)
        {
            var e = new DataEventArgs(data, length, error);
            var handler = Received;

            Util.SafeCall(() =>
                OnReceived(e));

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, e));
            }
        }

        /// <summary>
        /// データ受信時に呼ばれます。(継承用)
        /// </summary>
        protected virtual void OnReceived(DataEventArgs e)
        {
        }
        #endregion

        #region メッセージ送信処理
        /// <summary>
        /// 同期でデータを送信します。
        /// </summary>
        public void SendDataSync(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            lock (this.socketLock)
            {
                if (!IsConnected || IsDisconnecting)
                {
                    return;
                }

                // データを同期的に送信します。
                this.socket.Send(data, SocketFlags.None);
            }
        }

        /// <summary>
        /// データを非同期で送信します。
        /// </summary>
        public void SendData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            lock (this.socketLock)
            {
                if (!IsConnected || IsDisconnecting)
                {
                    return;
                }

                var sendData = new SendData()
                {
                    Socket = this.socket,
                };
                sendData.Callback += RaiseSent;
                sendData.AddBuffer(data, 0, data.Length);

                SendThread.AddSendData(sendData);
            }
        }

        /// <summary>
        /// データを非同期で送信します。
        /// </summary>
        public void SendData(SendData data)
        {
            if (data == null)
            {
                return;
            }

            data.Callback += RaiseSent;
            SendThread.AddSendData(data);
        }

        /// <summary>
        /// データの送信終了後に呼び出されます。
        /// </summary>
        private void RaiseSent(SendData sendData, Exception error)
        {
            var handler = this.Sent;
            var e = new DataEventArgs(
                null, /*(sendData != null ? sendData.Buffer : null)*/
                error);

            Util.SafeCall(() =>
                OnSent(e));

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, e));
            }
        }

        /// <summary>
        /// データ送信時に呼ばれます。(継承用)
        /// </summary>
        protected virtual void OnSent(DataEventArgs e)
        {
        }
        #endregion

        /// <summary>
        /// 次のコネクションＩＤを取得します。
        /// </summary>
        protected static int GetNextConnectionId()
        {
            lock (idCounter)
            {
                if (idCounter[0] == int.MaxValue)
                {
                    idCounter[0] = 0;
                }

                idCounter[0] += 1;

                return idCounter[0];
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Connection()
        {
            this.id = GetNextConnectionId();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Connection(string name)
            : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        [Obsolete("Connection()を使ってください。")]
        public Connection(Socket socket)
            : this()
        {
            SetSocket(socket);
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Connection()
        {
            Dispose(false);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        protected void Dispose(bool disposing)
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
