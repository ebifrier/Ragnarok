using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Threading;
using ProtoBuf;

namespace Ragnarok.Net.ProtoBuf
{
    using Ragnarok.Utility;

    /// <summary>
    /// 受信したコマンドを処理するハンドラ型です。
    /// </summary>
    public delegate void PbCommandHandler<TCmd>(
        object sender, PbCommandEventArgs<TCmd> e);

    /// <summary>
    /// 受信したリクエストを処理するハンドラ型です。
    /// </summary>
    public delegate void PbRequestHandler<TReq, TRes>(
        object sender, PbRequestEventArgs<TReq, TRes> e);

    /// <summary>
    /// 取得したレスポンスを処理するハンドラ型です。
    /// </summary>
    public delegate void PbResponseHandler<TRes>(
        object sender, PbResponseEventArgs<TRes> e);

    /// <summary>
    /// ProtoBufによるデータ送受信を行うクラスです。
    /// </summary>
    /// <remarks>
    /// <see cref="AddCommandHandler{TCmd}"/>や
    /// <see cref="AddRequestHandler{TReq,TRes}"/>に
    /// 登録してあるコマンド(返信のない要求)やリクエスト(返信のある要求)を
    /// 処理します。
    /// 
    /// コマンド・リクエスト・レスポンスともにアプリケーションレベルの
    /// 応答確認を行い、もし正しく送信できていなかった場合は
    /// ３回までの再送要求を行います。
    /// データ送信指示順序とデータ到着順序が同じになるとは限りません。
    /// 
    /// また、プロトコルのバージョンチェックも行うことが可能です。
    /// 必要であればプロトコルのバージョンチェック要求を
    /// 接続開始時に送信し、相手方とのバージョンミスマッチを確認します。
    /// </remarks>
    /// 
    /// <seealso cref="PbPacketHeader"/>
    public class PbConnection : Connection
    {
        /// <summary>
        /// 各種リクエストなどのハンドラです。
        /// </summary>
        private sealed class HandlerInfo
        {
            /// <summary>
            /// 処理するメッセージの型を取得または設定します。
            /// </summary>
            public Type Type
            {
                get;
                set;
            }

            /// <summary>
            /// もしリクエストなら、そのレスポンスの型を取得または設定します。
            /// </summary>
            public Type ResponseType
            {
                get;
                set;
            }

            /// <summary>
            /// リクエストを処理するためのハンドラかどうかを取得します。
            /// </summary>
            public bool IsRequestHandler
            {
                get { return (ResponseType != null); }
            }

            /// <summary>
            /// 実際の処理を行うハンドラを取得または設定します。
            /// </summary>
            public Func<object, IPbResponse> Handler
            {
                get;
                set;
            }

            /// <summary>
            /// ログ出力を行うかどうかを取得または設定します。
            /// </summary>
            public bool IsOutLog
            {
                get;
                set;
            }
        }

        /// <summary>
        /// データIDとレスポンスかどうかで送受信データの一意性を保証します。
        /// </summary>
        private sealed class DataId : IEquatable<DataId>
        {
            /// <summary>
            /// コマンドやレスポンスのIDを取得します。
            /// </summary>
            public int Id
            {
                get;
                private set;
            }

            /// <summary>
            /// レスポンスかどうかを取得します。
            /// </summary>
            public bool IsResponse
            {
                get;
                private set;
            }

            /// <summary>
            /// Dictionary用
            /// </summary>
            public override bool Equals(object obj)
            {
                return Equals(obj as DataId);
            }

            /// <summary>
            /// Dictionary用
            /// </summary>
            public bool Equals(DataId other)
            {
                if ((object)other == null)
                {
                    return false;
                }

                return (Id == other.Id && IsResponse == other.IsResponse);
            }

            /// <summary>
            /// Dictionary用
            /// </summary>
            public override int GetHashCode()
            {
                return (Id.GetHashCode() ^ IsResponse.GetHashCode());
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DataId(int id, bool isResponse)
            {
                Id = id;
                IsResponse = isResponse;
            }
        }

#if false
        /// <summary>
        /// ACKによる応答確認が必要なデータを保持します。
        /// </summary>
        private sealed class NeedAckInfo : IDisposable
        {
            private PbConnection connection;
            private Timer timer;
            private int tryCount = 0;
            private bool disposed;

            /// <summary>
            /// データの送信IDを取得します。
            /// </summary>
            public DataId DataId
            {
                get;
                private set;
            }

            /// <summary>
            /// 実際に送信するデータを取得します。
            /// </summary>
            public PbSendData SendData
            {
                get;
                private set;
            }

            /// <summary>
            /// ACKのタイムアウト時間を取得します。
            /// </summary>
            public TimeSpan Timeout
            {
                get;
                private set;
            }

            /// <summary>
            /// 送信を試みた回数を一つ増加します。
            /// </summary>
            /// <remarks>
            /// データ送信に失敗した場合に呼ばれます。
            /// その場合はデータを再度送信します。
            /// </remarks>
            public int IncrementTryCount()
            {
                return Interlocked.Increment(ref this.tryCount);
            }

            /// <summary>
            /// ACKの受信タイムアウト時に呼ばれます。
            /// </summary>
            private void Timer_Callback(object state)
            {
                Log.Info(this.connection, "ACK受信がタイムアウトしました。");

                // 再送を続ける場合は真が返ります。
                if (this.connection.AckFailed(this))
                {
                    this.timer.Change(Timeout, TimeSpan.FromMilliseconds(-1));
                }
                else
                {
                    // タイマーを更新せず、Dispose処理のみを行います。
                    Dispose();
                }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public NeedAckInfo(PbConnection connection, DataId dataId,
                               PbSendData sendData, TimeSpan timeout)
            {
                DataId = dataId;
                SendData = sendData;
                Timeout = timeout;

                this.connection = connection;
                this.timer = new Timer(
                    Timer_Callback,
                    null,
                    timeout,
                    TimeSpan.FromMilliseconds(-1));
            }

            /// <summary>
            /// デストラクタ
            /// </summary>
            ~NeedAckInfo()
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
            /// ACK受信成功時にタイマーを無効化します。
            /// </summary>
            private void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        if (this.timer != null)
                        {
                            this.timer.Dispose();
                            this.timer = null;
                        }
                    }

                    this.disposed = true;
                }
            }
        }
#endif

        private static readonly Dictionary<string, Type> typeCache =
            new Dictionary<string, Type>();
        private readonly object receiveLock = new object();
        private int idCounter = 0;
        private PbPacketHeader packetHeader = new PbPacketHeader();
        private MemoryStream headerStream;
        private MemoryStream typenameStream;
        private MemoryStream payloadStream;
        private readonly Dictionary<int, PbRequestData> requestDataDic =
            new Dictionary<int, PbRequestData>();
        private readonly Dictionary<Type, HandlerInfo> handlerDic =
            new Dictionary<Type, HandlerInfo>();
        private readonly Timer keepAliveTimer;
        private TimeSpan keepAliveInterval;

        /// <summary>
        /// プロトコルのバージョンを取得または設定します。
        /// </summary>
        public PbProtocolVersion ProtocolVersion
        {
            get;
            set;
        }

        /// <summary>
        /// レスポンス応答のデフォルトタイムアウト時間を取得または設定します。
        /// </summary>
        /// <remarks>
        /// デフォルト値はタイムアウト時間が無制限に設定されます。
        /// </remarks>
        public TimeSpan DefaultRequestTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// 通信相手の存在確認を行う時間間隔を取得または設定します。
        /// </summary>
        /// <remarks>
        /// デフォルト値は１０分間隔に設定されます。
        /// </remarks>
        public TimeSpan KeepAliveInterval
        {
            get { return this.keepAliveInterval; }
            set
            {
                // 値が小さすぎる場合は無視します。
                if (value == TimeSpan.MinValue || value <= TimeSpan.Zero)
                {
                    return;
                }

                this.keepAliveInterval = value;

                // keepaliveの時間間隔を再設定します。
                this.keepAliveTimer.Change(
                    TimeSpan.FromSeconds(1), value);
            }
        }

        /// <summary>
        /// T型のメッセージを処理するハンドラを追加します。
        /// </summary>
        public void AddCommandHandler<TCmd>(PbCommandHandler<TCmd> handler,
                                            bool isOutLog = true)
        {
            if (handler == null)
            {
                return;
            }

            lock (this.handlerDic)
            {
                var handlerInfo = new HandlerInfo()
                {
                    Type = typeof(TCmd),
                    ResponseType = null,
                    Handler = (_ =>
                        HandleCommandInternal(handler, _)),
                    IsOutLog = isOutLog,
                };

                this.handlerDic.Add(typeof(TCmd), handlerInfo);
            }
        }

        /// <summary>
        /// コマンドを型付けするためのメソッドです。
        /// </summary>
        private IPbResponse HandleCommandInternal<TCmd>(PbCommandHandler<TCmd>
                                                        handler,
                                                        object commandObj)
        {
            var e = new PbCommandEventArgs<TCmd>((TCmd)commandObj);
            handler(this, e);
            
            return null; // 戻り値はありません。
        }

        /// <summary>
        /// T型のリクエストを処理するハンドラを追加します。
        /// </summary>
        public void AddRequestHandler<TReq, TRes>(PbRequestHandler<TReq, TRes>
                                                  handler,
                                                  bool isOutLog = true)
            where TRes: class
        {
            if (handler == null)
            {
                return;
            }

            lock (this.handlerDic)
            {
                var handlerInfo = new HandlerInfo()
                {
                    Type = typeof(TReq),
                    ResponseType = typeof(TRes),
                    Handler = (requestObj) =>
                        HandleRequestInternal(handler, requestObj),
                    IsOutLog = isOutLog,
                };
                
                this.handlerDic.Add(typeof(TReq), handlerInfo);
            }
        }

        /// <summary>
        /// リクエストを型付けするためのメソッドです。
        /// </summary>
        private IPbResponse HandleRequestInternal<TReq, TRes>(
            PbRequestHandler<TReq, TRes> handler,
            object requestObj)
            where TRes: class
        {
            var e = new PbRequestEventArgs<TReq, TRes>((TReq)requestObj);
            handler(this, e);

            return new PbResponse<TRes>()
            {
                Response = e.Response,
                ErrorCode = e.ErrorCode,
            };
        }

        /// <summary>
        /// T型を処理するハンドラを削除します。
        /// </summary>
        public bool RemoveHandler<T>()
        {
            lock (this.handlerDic)
            {
                return this.handlerDic.Remove(typeof(T));
            }
        }

        /// <summary>
        /// 通信プロトコルのバージョンを調べます。
        /// </summary>
        public PbVersionCheckResult CheckProtocolVersion(TimeSpan timeout)
        {
            if (ProtocolVersion == null)
            {
                throw new PbException("ProtocolVersionがnullです。");
            }

            // 待機用イベントを使い、非同期で確認を行います。
            using (var ev = new AutoResetEvent(false))
            {
                var request = new PbCheckProtocolVersionRequest(ProtocolVersion);
                var result = PbVersionCheckResult.Unknown;

                // 型を指定しないといくつかのコンパイラではコンパイルに失敗します。
                SendRequest<PbCheckProtocolVersionRequest,
                            PbCheckProtocolVersionResponse>(
                    request,
                    timeout,
                    (object sender,
                     PbResponseEventArgs<PbCheckProtocolVersionResponse> e) =>
                    {
                        // プロトコルのバージョンチェックの結果を受け取ります。
                        result = (e.Response == null ?
                            (e.ErrorCode == PbErrorCode.Timeout ?
                             PbVersionCheckResult.Timeout :
                             PbVersionCheckResult.Unknown) :
                            e.Response.Result);

                        ev.Set();
                    });

                ev.WaitOne();
                return result;
            }
        }

        /// <summary>
        /// プロトコルのバージョンチェックを行います。
        /// </summary>
        private void HandleCheckProtocolVersionRequest(
            object sender,
            PbRequestEventArgs<PbCheckProtocolVersionRequest,
                               PbCheckProtocolVersionResponse> e)
        {
            var clientVersion = e.Request.ProtocolVersion;
            var result = PbVersionCheckResult.Ok;

            if (clientVersion == null)
            {
                result = PbVersionCheckResult.InvalidValue;
            }
            else if (clientVersion < ProtocolVersion)
            {
                result = PbVersionCheckResult.TooLower;
            }
            else if (clientVersion > ProtocolVersion)
            {
                result = PbVersionCheckResult.TooUpper;
            }

            // バージョンチェックの結果を返します。
            e.Response = new PbCheckProtocolVersionResponse(result);
        }

        /// <summary>
        /// 存在確認リクエストを送信します。
        /// </summary>
        private void SendKeepAlive()
        {
            try
            {
                // 型を指定しないといくつかのコンパイラではコンパイルに失敗します。
                SendRequest<PbKeepAliveRequest, PbKeepAliveResponse>(
                    new PbKeepAliveRequest(),
                    TimeSpan.FromSeconds(60 * 2),
                    (object sender,
                     PbResponseEventArgs<PbKeepAliveResponse> e) =>
                    {
                        if (e.ErrorCode != 0)
                        {
                            // 異常検知
                            Disconnect();
                        }
                    },
                    false);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "KeepAliveの送信に失敗しました。");
            }
        }

        /// <summary>
        /// 存在確認リクエストを処理します。
        /// </summary>
        private void HandleKeepAliveRequest(
            object sender,
            PbRequestEventArgs<PbKeepAliveRequest, PbKeepAliveResponse> e)
        {
            e.Response = new PbKeepAliveResponse();
        }

        #region 型名変換
        private static readonly Dictionary<string, string> TypeConvertTable =
            new Dictionary<string, string>()
        {
            {"Ragnarok.Net.ProtoBuf.PbResponse", "${0}"},
            {"Ragnarok.Net.ProtoBuf.PbAck", "${1}"},
            {"Ragnarok.Net.ProtoBuf.PbNak", "${2}"},
            {"Ragnarok.Net.ProtoBuf", "${3}"},
        };

        /// <summary>
        /// 短縮する型名を登録します。
        /// </summary>
        /// <remarks>
        /// これは送受信双方で同じ設定をする必要があります。
        /// </remarks>
        public static void AddConvertType(string typename)
        {
            if (string.IsNullOrEmpty(typename))
            {
                throw new ArgumentNullException("typename");
            }

            lock (TypeConvertTable)
            {
                TypeConvertTable.Add(
                    typename,
                    string.Format("${{{0}}}", TypeConvertTable.Count));
            }
        }

        /// <summary>
        /// 型名を短くするためのエンコード処理を行います。
        /// </summary>
        public static string EncodeTypeName(string deTypeName)
        {
            if (string.IsNullOrEmpty(deTypeName))
            {
                throw new ArgumentNullException("deTypeName");
            }

            lock (TypeConvertTable)
            {
                return TypeConvertTable.Aggregate(
                    deTypeName,
                    (seed, pair) => seed.Replace(pair.Key, pair.Value));
            }
        }

        /// <summary>
        /// 短縮された型名を元に戻します。
        /// </summary>
        public static string DecodeTypeName(string enTypeName)
        {
            if (string.IsNullOrEmpty(enTypeName))
            {
                throw new ArgumentNullException("enTypeName");
            }

            lock (TypeConvertTable)
            {
                return TypeConvertTable.Aggregate(
                    enTypeName,
                    (seed, pair) => seed.Replace(pair.Value, pair.Key));
            }
        }
        #endregion

        #region receive data
        #region 受信データ解析
        /// <summary>
        /// データの取得後に呼ばれます。
        /// </summary>
        protected override void OnReceived(DataEventArgs e)
        {
            base.OnReceived(e);

            if (e.Error == null)
            {
                var data = new DataSegment<byte>(e.Data, 0, e.DataLength);

                OnReceivedPacket(data);
            }
        }
        
        /// <summary>
        /// 受信パケットの解析を行います。
        /// </summary>
        private void OnReceivedPacket(DataSegment<byte> data)
        {
            // このロックオブジェクトは受信処理を直列的に行うために
            // 使われます。受信処理中に他の受信データを扱うことはできません。
            lock (this.receiveLock)
            {
                while (data.Count > 0)
                {
                    bool parsed = false;

                    try
                    {
                        if (ReceivePacketHeader(data))
                        {
                            if (ReceiveTypename(data))
                            {
                                parsed = ReceivePayload(data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException(ex,
                            "データの受信に失敗しました。");

                        // もしパース中に例外が発生した場合、
                        // (パケットヘッダが正しくない等の場合)
                        // 受信したパケットデータをすべてクリアします。
                        InitReceivedPacket();

                        // 残りのデータの受信を行います。
                        parsed = false;
                    }

                    // データ受信に成功したらそのデータを処理します。
                    if (parsed)
                    {
                        HandleReceivedPacket(
                            this.packetHeader,
                            this.typenameStream.GetBuffer(),
                            this.payloadStream.GetBuffer());

                        InitReceivedPacket();
                    }
                }
            }
        }

        /// <summary>
        /// データをストリームに書き込む必要があるか調べます。
        /// </summary>
        private bool IsNeedWriteData(MemoryStream stream)
        {
            var leaveCount = (int)(stream.Capacity - stream.Position);

            return (leaveCount != 0);
        }

        /// <summary>
        /// バイトデータに受信データを追記します。
        /// </summary>
        private bool WriteReceivedData(MemoryStream stream, DataSegment<byte> data)
        {
            var leaveCount = (int)(stream.Capacity - stream.Position);
            if (leaveCount == 0)
            {
                return true;
            }

            var length = Math.Min(leaveCount, data.Count);
            stream.Write(data.Array, data.Offset, length);
            data.Increment(length);

            return (length == leaveCount);
        }

        /// <summary>
        /// パケットのヘッダ部分を読み込みます。
        /// </summary>
        private bool ReceivePacketHeader(DataSegment<byte> data)
        {
            if (!IsNeedWriteData(this.headerStream))
            {
                return true;
            }

            // ヘッダー読み込みが終わった後、コンテンツ用の
            // バッファを用意します。
            if (WriteReceivedData(this.headerStream, data))
            {
                PacketHeaderReceived();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 型名データを受信します。
        /// </summary>
        private bool ReceiveTypename(DataSegment<byte> data)
        {
            if (this.packetHeader == null)
            {
                return false;
            }

            return WriteReceivedData(this.typenameStream, data);
        }

        /// <summary>
        /// データ部分を受信します。
        /// </summary>
        private bool ReceivePayload(DataSegment<byte> data)
        {
            if (this.packetHeader == null)
            {
                return false;
            }

            return WriteReceivedData(this.payloadStream, data);
        }

        /// <summary>
        /// 受信データを初期化します。
        /// </summary>
        private void InitReceivedPacket()
        {
            this.headerStream = new MemoryStream(PbPacketHeader.HeaderLength);

            this.packetHeader = null;
            this.typenameStream = null;
            this.payloadStream = null;
        }

        /// <summary>
        /// ヘッダー受信完了後に呼ばれます。
        /// </summary>
        private void PacketHeaderReceived()
        {
            var header = new PbPacketHeader();
            header.SetDecodedHeader(this.headerStream.GetBuffer());

            // 10MB以上のデータはエラーとします。
            if (header.PayloadLength > 10 * 1024 * 1024)
            {
                Disconnect();
                return;
            }

            this.packetHeader = header;
            this.typenameStream = new MemoryStream(header.TypeNameLength);
            this.payloadStream = new MemoryStream(header.PayloadLength);

            Log.Trace(this,
                "Packet Header Received (payload={0}bytes)",
                header.PayloadLength);
        }
        #endregion

        /// <summary>
        /// 受信パケットを処理します。
        /// </summary>
        private void HandleReceivedPacket(PbPacketHeader header,
                                          byte[] typenameBuffer,
                                          byte[] payloadBuffer)
        {
            Type type = null;

            try
            {
                // 型名とメッセージをデシリアライズします。
                type = DeserializeType(typenameBuffer);
                var message = DeserializeMessage(payloadBuffer, type);

                // 対応するハンドラを呼びます。
                if (!header.IsResponse)
                {
                    HandleRequestOrCommand(header.Id, message);
                }
                else
                {
                    HandleResponse(header.Id, message);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(this, ex,
                    "データのデシリアライズに失敗しました。" +
                    "(content size={0}, type={1})",
                    (payloadBuffer == null ? -1 : payloadBuffer.Length),
                    type);
            }
        }

        /// <summary>
        /// 型のフルネームからその型のオブジェクトを取得します。
        /// </summary>
        private Type GetTypeFrom(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            // キャッシュに登録してあればそれをそのまま返します。
            Type type = null;
            lock (typeCache)
            {
                if (typeCache.TryGetValue(typeName, out type))
                {
                    return type;
                }
            }

            // 型のフルネームからその型を検索し、もしあれば
            // それをキャッシュに登録します。
            type = TypeSerializer.Deserialize(typeName);
            if (type == null)
            {
                return null;
            }

            // キャッシュに登録します。
            lock (typeCache)
            {
                typeCache.Add(typeName, type);
            }

            return type;
        }

        /// <summary>
        /// 型オブジェクトをデシリアライズします。
        /// </summary>
        private Type DeserializeType(byte[] typenameBuffer)
        {
            var typename = Encoding.UTF8.GetString(typenameBuffer);
            if (string.IsNullOrEmpty(typename))
            {
                throw new PbException(
                    "受信した型名が正しくありません。");
            }

            // 短縮された型名を元に戻します。
            typename = DecodeTypeName(typename);

            // デシリアライズする型のオブジェクトを取得します。
            var type = GetTypeFrom(typename);
            if (type == null)
            {
                throw new PbException(
                    string.Format(
                        "{0}: 適切な型が見つかりませんでした。",
                        typename));
            }

            return type;
        }

        /// <summary>
        /// メッセージオブジェクトをデシリアライズします。
        /// </summary>
        private object DeserializeMessage(byte[] payloadBuffer, Type type)
        {
            var message = PbUtil.Deserialize(payloadBuffer, type);
            if (message == null)
            {
                throw new PbException(
                    string.Format(
                        "データのデシリアライズに失敗しました。" +
                        "(content size={0}, type={1})",
                        payloadBuffer.Length, type));
            }

            return message;
        }

        /// <summary>
        /// リクエストかコマンドを処理します。
        /// </summary>
        private void HandleRequestOrCommand(int id, object message)
        {
            HandlerInfo handlerInfo = null;
            IPbResponse response = null;

            // コマンドを処理するハンドラオブジェクトを取得します。
            lock (this.handlerDic)
            {
                if (!this.handlerDic.TryGetValue(message.GetType(), out handlerInfo))
                {
                    Log.Error(this,
                        "{0}: 適切なハンドラが見つかりませんでした。",
                        message.GetType());
                    return;
                }
            }

            // ログを出力したくない場合もあります。
            if (handlerInfo.IsOutLog)
            {
                Log.Debug(this,
                    "{0}を受信しました。", message.GetType());
            }

            if (handlerInfo.Handler != null)
            {
                try
                {
                    // レスポンスはnullのことがありますが、
                    // それは合法です。
                    response = handlerInfo.Handler(message);
                }
                catch (Exception ex)
                {
                    Log.ErrorException(this, ex,
                        "受信データの処理ハンドラでエラーが発生しました。");

                    response = new PbResponse<PbDummy>()
                    {
                        ErrorCode = PbErrorCode.HandlerException,
                    };
                }
            }

            // もしリクエストなら、レスポンスを返します。
            if (handlerInfo.IsRequestHandler)
            {
                // responseはnullのことがあります。
                SendResponse(id, response);
            }
        }

        /// <summary>
        /// 受信したレスポンスを処理します。
        /// </summary>
        private void HandleResponse(int id, object message)
        {
            var response = message as IPbResponse;
            PbRequestData reqData = null;

            if (response == null)
            {
                throw new InvalidOperationException(
                    "レスポンスの型が正しくありません。");
            }

            // リクエストリストの中から、レスポンスと同じIdを持つ
            // リクエストを探します。
            lock (this.requestDataDic)
            {
                if (!this.requestDataDic.TryGetValue(id, out reqData))
                {
                    Log.Error(this,
                        "サーバーから不正なレスポンスが返されました。" +
                        "(id={0})", id);
                    return;
                }

                this.requestDataDic.Remove(id);
            }

            Log.Debug(this,
                 "{0}を受信しました。", message.GetType());

            // レスポンス処理用のハンドラを呼びます。
            if (reqData != null)
            {
                reqData.OnResponseReceived(response);

                // タイムアウト検出用タイマを殺すために必要です。
                reqData.Dispose();
            }
        }
        #endregion

        #region send data
        /// <summary>
        /// 送信用データのＩＤを取得します。
        /// </summary>
        private int GetNextSendId()
        {
            return Interlocked.Increment(ref this.idCounter);
        }

        /// <summary>
        /// リクエストを出します。
        /// </summary>
        public void SendRequest<TReq, TRes>(TReq request,
                                            PbResponseHandler<TRes> handler,
                                            bool isOutLog = true)
            where TReq : class
        {
            SendRequest(request, DefaultRequestTimeout, handler, isOutLog);
        }

        /// <summary>
        /// タイムアウト付でリクエストを出します。
        /// </summary>
        public void SendRequest<TReq, TRes>(TReq request,
                                            TimeSpan timeout,
                                            PbResponseHandler<TRes> handler,
                                            bool isOutLog = true)
            where TReq: class
        {
            if (request == null)
            {
                return;
            }

            var sendData = new PbSendData(request);

            var id = GetNextSendId();
            var reqData = new PbRequestData<TReq, TRes>()
            {
                Id = id,
                Connection = this,
                ResponseReceived = handler,
            };
            reqData.SetTimeout(timeout);

            // 未処理のリクエストとして、リストに追加します。
            lock (this.requestDataDic)
            {
                this.requestDataDic.Add(id, reqData);
            }

            // データを送信します。
            SendDataInternal(id, false, sendData, isOutLog);
        }

        /// <summary>
        /// コマンドを送ります。
        /// </summary>
        public void SendCommand<TCmd>(TCmd command, bool isOutLog = true)
            where TCmd : class
        {
            if (command == null)
            {
                return;
            }

            var sendData = new PbSendData(command);
            SendData(sendData, isOutLog);
        }

        /// <summary>
        /// protobufでシリアライズ後のデータを直接送ります。
        /// </summary>
        public void SendData(PbSendData sendData, bool isOutLog = true)
        {
            if (sendData == null)
            {
                return;
            }

            var id = GetNextSendId();
            SendDataInternal(id, false, sendData, isOutLog);
        }

        /// <summary>
        /// レスポンスを送ります。
        /// </summary>
        private void SendResponse(int id, IPbResponse response,
                                  bool isOutLog = true)
        {
            if (response == null)
            {
                return;
            }

            var sendData = new PbSendData(response);
            SendDataInternal(id, true, sendData, isOutLog);
        }

        /// <summary>
        /// データを送信します。
        /// </summary>
        private void SendDataInternal(int id, bool isResponse,
                                      PbSendData pbSendData, bool isOutLog)
        {
            if (pbSendData == null)
            {
                throw new ArgumentNullException("pbSendData");
            }

            if (pbSendData.SerializedData == null ||
                pbSendData.EncodedTypeName == null)
            {
                throw new PbException("送信データがシリアライズされていません。");
            }

            if (!IsConnected || !CanWrite)
            {
                // これをthrowすると対処が面倒なので
                return;
            }

            try
            {
                var typedata = pbSendData.EncodedTypeData;
                var payload = pbSendData.SerializedData;

                // パケットヘッダを用意します。
                var header = new PbPacketHeader
                {
                    Id = id,
                    IsResponse = isResponse,
                    TypeNameLength = typedata.Length,
                    PayloadLength = payload.Length,
                };
                var headerData = header.GetEncodedPacket();

                // 送信データは複数バッファのまま送信します。
                var sendData = new SendData()
                {
                    Socket = this.Socket,
                };
                sendData.AddBuffer(headerData);
                sendData.AddBuffer(typedata);
                sendData.AddBuffer(payload);

                // データを送信します。
                base.SendData(sendData);

                if (isOutLog)
                {
                    Log.Debug(this,
                        "{0}を送信しました。(content={1}bytes)",
                        pbSendData.TypeName,
                        (payload != null ? payload.Length : -1));
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(this, ex,
                    "{0}: 送信データのシリアライズに失敗しました。",
                    pbSendData.TypeName);
            }
        }
        #endregion

        private void OnKeepAliveCallback(object state)
        {
            SendKeepAlive();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbConnection()
        {
            // KeepAliveIntervalプロパティを設定する前にnewします。
            this.keepAliveTimer = new Timer(OnKeepAliveCallback);

            InitReceivedPacket();
            ProtocolVersion = new PbProtocolVersion();
            DefaultRequestTimeout = TimeSpan.MaxValue;
            KeepAliveInterval = TimeSpan.FromMinutes(10);

            AddRequestHandler<PbCheckProtocolVersionRequest,
                              PbCheckProtocolVersionResponse>(
                HandleCheckProtocolVersionRequest);
            AddRequestHandler<PbKeepAliveRequest,
                              PbKeepAliveResponse>(
                HandleKeepAliveRequest);
        }
    }
}
