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
    /// また、プロトコルのバージョンチェックも行います。
    /// 必要であればプロトコルのバージョンチェック要求を
    /// 接続開始時に送信することができます。
    /// </remarks>
    /// 
    /// <seealso cref="PbPacketHeader"/>
    public class PbConnection : Connection
    {
        /// <summary>
        /// 各種リクエストなどのハンドラです。
        /// </summary>
        internal class HandlerInfo
        {
            /// <summary>
            /// リクエストを処理するためのハンドラかどうかを取得します。
            /// </summary>
            public bool IsRequestHandler
            {
                get { return (ResponseType != null); }
            }

            /// <summary>
            /// 処理するメッセージの型です。
            /// </summary>
            public Type Type;

            /// <summary>
            /// もしリクエストなら、そのレスポンスの型です。
            /// </summary>
            public Type ResponseType;

            /// <summary>
            /// 実際の処理を行うハンドラです。
            /// </summary>
            public Func<object, IPbResponse> Handler;
        }

        private static readonly Dictionary<string, Type> typeCache =
            new Dictionary<string, Type>();
        private readonly object receiveLock = new object();
        private int idCounter = 0;
        private PbPacketHeader packetHeader = new PbPacketHeader();
        private readonly byte[] headerBuffer = new byte[PbPacketHeader.HeaderLength];
        private byte[] typenameBuffer = null;
        private byte[] payloadBuffer = null;
        private int headerReadSize = 0;
        private int typenameReadSize = 0;
        private int payloadReadSize = 0;
        private readonly Dictionary<int, PbRequestData> requestDataDic =
            new Dictionary<int, PbRequestData>();
        private readonly Dictionary<Type, HandlerInfo> handlerDic =
            new Dictionary<Type, HandlerInfo>();

        /// <summary>
        /// レスポンス応答のデフォルトタイムアウト時間を取得または設定します。
        /// </summary>
        public TimeSpan DefaultRequestTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// プロトコルのバージョンを取得または設定します。
        /// </summary>
        public PbProtocolVersion ProtocolVersion
        {
            get;
            set;
        }

        /// <summary>
        /// T型のメッセージを処理するハンドラを追加します。
        /// </summary>
        public void AddCommandHandler<TCmd>(PbCommandHandler<TCmd> handler)
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
                    Handler = (commandObj) =>
                        HandleCommandInternal(handler, commandObj),
                };

                this.handlerDic.Add(typeof(TCmd), handlerInfo);
            }
        }

        /// <summary>
        /// コマンドを型付けするためのメソッドです。
        /// </summary>
        private IPbResponse HandleCommandInternal<TCmd>(
            PbCommandHandler<TCmd> handler,
            object commandObj)
        {
            var e = new PbCommandEventArgs<TCmd>((TCmd)commandObj);
            handler(this, e);
            
            return null; // 戻り値はありません。
        }

        /// <summary>
        /// T型のリクエストを処理するハンドラを追加します。
        /// </summary>
        public void AddRequestHandler<TReq, TRes>(
            PbRequestHandler<TReq, TRes> handler)
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
                throw new ArgumentNullException("version");
            }

            // 待機用イベントです。
            var ev = new ManualResetEvent(false);
            var result = PbVersionCheckResult.Unknown;

            // リクエストを送ります。
            var request = new PbCheckProtocolVersionRequest(ProtocolVersion);

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
                         PbVersionCheckResult.Unknown ) :
                        e.Response.Result);

                    ev.Set();
                });

            ev.WaitOne();
            return result;
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

        #region receive data
        /// <summary>
        /// データの取得後に呼ばれます。
        /// </summary>
        protected override void OnReceived(DataEventArgs e)
        {
            base.OnReceived(e);

            if (e.Error == null)
            {
                OnReceivedPacket(e.Data, e.DataLength, 0);
            }
        }

        /// <summary>
        /// 受信パケットの解析を行います。
        /// </summary>
        private void OnReceivedPacket(byte[] buffer, int bufferLength,
                                      int offset)
        {
            // このロックオブジェクトは受信処理を直列的に行うために
            // 使われます。受信処理中に他の受信データを扱うことはできません。
            lock (this.receiveLock)
            {
                while (offset < bufferLength)
                {
                    bool parsed = false;

                    try
                    {
                        if (ReceivePacketHeader(buffer, bufferLength, ref offset))
                        {
                            if (ReceiveTypename(buffer, bufferLength, ref offset))
                            {
                                parsed = ReceivePayload(
                                    buffer, bufferLength, ref offset);
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

                        // 残りのデータはすべて破棄します。
                        return;
                    }

                    // データ受信に成功したらそのデータを処理します。
                    if (parsed)
                    {
                        HandleReceivedPacket(
                            this.packetHeader,
                            this.typenameBuffer,
                            this.payloadBuffer);

                        InitReceivedPacket();
                    }
                }
            }
        }

        /// <summary>
        /// パケットのヘッダ部分を読み込みます。
        /// </summary>
        private bool ReceivePacketHeader(byte[] buffer, int bufferLength,
                                         ref int offset)
        {
            // ヘッダーデータが読み込まれていない場合は、それを読み込みます。
            if (this.headerReadSize == PbPacketHeader.HeaderLength)
            {
                return true;
            }

            // ヘッダーデータの読み込みを行います。
            var length = Math.Min(
                PbPacketHeader.HeaderLength - this.headerReadSize,
                buffer.Length - offset);

            Array.Copy(
                buffer,
                offset,
                this.headerBuffer,
                this.headerReadSize,
                length);

            // データ長の設定。
            this.headerReadSize += length;
            offset += length;

            // ヘッダー読み込みが終わった後、コンテンツ用の
            // バッファを用意します。
            if (this.headerReadSize == PbPacketHeader.HeaderLength)
            {
                PacketHeaderReceived();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 型名データを受信します。
        /// </summary>
        private bool ReceiveTypename(byte[] buffer, int bufferLength,
                                     ref int offset)
        {
            if (this.packetHeader == null)
            {
                return false;
            }

            if (this.typenameReadSize == this.packetHeader.TypenameLength)
            {
                return true;
            }

            // 型名部分を読み込みます。
            var length = Math.Min(
                this.packetHeader.TypenameLength - this.typenameReadSize,
                buffer.Length - offset);

            Array.Copy(
                buffer,
                offset,
                this.typenameBuffer,
                this.typenameReadSize,
                length);

            // データ長の設定。
            this.typenameReadSize += length;
            offset += length;

            return (this.typenameReadSize == this.packetHeader.TypenameLength);
        }

        /// <summary>
        /// データ部分を受信します。
        /// </summary>
        private bool ReceivePayload(byte[] buffer, int bufferLength,
                                    ref int offset)
        {
            if (this.packetHeader == null)
            {
                return false;
            }

            if (this.payloadReadSize == this.packetHeader.PayloadLength)
            {
                return true;
            }

            // ペイロード部分を読み込みます。
            var length = Math.Min(
                this.packetHeader.PayloadLength - this.payloadReadSize,
                buffer.Length - offset);

            Array.Copy(
                buffer,
                offset,
                this.payloadBuffer,
                this.payloadReadSize,
                length);

            // データ長の設定。
            this.payloadReadSize += length;
            offset += length;

            // データを処理したら、受信データ長を知らせ帰ります。
            return (this.payloadReadSize == this.packetHeader.PayloadLength);
        }

        /// <summary>
        /// 受信データを初期化します。
        /// </summary>
        private void InitReceivedPacket()
        {
            Array.Clear(this.headerBuffer, 0, this.headerBuffer.Length);

            this.packetHeader = null;
            this.typenameBuffer = null;
            this.payloadBuffer = null;
            this.headerReadSize = 0;
            this.typenameReadSize = 0;
            this.payloadReadSize = 0;
        }

        /// <summary>
        /// ヘッダー受信完了後に呼ばれます。
        /// </summary>
        private void PacketHeaderReceived()
        {
            this.packetHeader = new PbPacketHeader();
            this.packetHeader.SetDecodedHeader(this.headerBuffer);

            // 1MB以上のデータはエラーとします。
            /*if (this.receivingData.ContentLength > 1 * 1024 * 1024)
            {
                Disconnect();
                return;
            }*/

            this.typenameBuffer = new byte[this.packetHeader.TypenameLength];
            this.typenameReadSize = 0;

            this.payloadBuffer = new byte[this.packetHeader.PayloadLength];
            this.payloadReadSize = 0;

            Log.Trace(this,
                "Packet Header Received (payload={0}bytes)",
                this.packetHeader.PayloadLength);
        }

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
                if (typenameBuffer == null)
                {
                    Log.Error(this,
                        "型名の受信に失敗しました。");
                    return;
                }

                if (payloadBuffer == null)
                {
                    Log.Error(this,
                        "コンテンツの受信に失敗しました。");
                    return;
                }

                // 型名とメッセージをデシリアライズします。
                type = DeserializeType(typenameBuffer);
                if (type == null)
                {
                    return;
                }

                var message = DeserializeMessage(payloadBuffer, type);
                if (message == null)
                {
                    return;
                }

                Log.Debug(this,
                    "{0}を受信しました。", message.GetType());

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
                Log.Error(this,
                    "受信した型名が正しくありません。");
                return null;
            }

            // デシリアライズする型のオブジェクトを取得します。
            var type = GetTypeFrom(typename);
            if (type == null)
            {
                Log.Error(this,
                    "{0}: 適切な型が見つかりませんでした。",
                    typename);
                return null;
            }

            return type;
        }

        /// <summary>
        /// メッセージオブジェクトをデシリアライズします。
        /// </summary>
        private object DeserializeMessage(byte[] payloadBuffer, Type type)
        {
            // サーバーから送られてきたデータを処理します。
            var message = PbUtil.Deserialize(payloadBuffer, type);
            if (message == null)
            {
                Log.Error(this,
                    "データのデシリアライズに失敗しました。" +
                    "(content size={0}, type={1})",
                    payloadBuffer.Length, type);
                return null;
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
                                            PbResponseHandler<TRes> handler)
            where TReq : class
        {
            SendRequest(request, DefaultRequestTimeout, handler);
        }

        /// <summary>
        /// タイムアウト付でリクエストを出します。
        /// </summary>
        public void SendRequest<TReq, TRes>(TReq request,
                                            TimeSpan timeout,
                                            PbResponseHandler<TRes> handler)
            where TReq: class
        {
            if (request == null)
            {
                return;
            }

            var id = GetNextSendId();

            // 未処理のリクエストとして、リストに追加します。
            lock (this.requestDataDic)
            {
                var reqData = new PbRequestData<TReq, TRes>()
                {
                    Id = id,
                    Connection = this,
                    ResponseReceived = handler,
                };
                reqData.SetTimeout(timeout);

                // リクエストに追加します。
                this.requestDataDic.Add(id, reqData);
            }

            // データを送信します。
            SendData(id, false, request);
        }

        /// <summary>
        /// コマンドを送ります。
        /// </summary>
        public void SendCommand<TCmd>(TCmd command)
            where TCmd : class
        {
            if (command == null)
            {
                return;
            }

            // コマンドを送信します。
            var id = GetNextSendId();

            SendData(id, false, command);
        }

        /// <summary>
        /// レスポンスを送ります。
        /// </summary>
        private void SendResponse(int id, IPbResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            SendData(id, true, response);
        }

        /// <summary>
        /// データを送信します。
        /// </summary>
        private void SendData(int id, bool isResponse, object sendData)
        {
            if (sendData == null)
            {
                throw new ArgumentNullException("sendData");
            }

            try
            {
                // 送信データをシリアライズします。
                var payload = PbUtil.Serialize(sendData, sendData.GetType());
                var typename = TypeSerializer.Serialize(sendData.GetType());
                var typeData = Encoding.UTF8.GetBytes(typename);

                // パケットヘッダを用意します。
                var header = new PbPacketHeader()
                {
                    Id = id,
                    IsResponse = isResponse,
                    TypenameLength = typeData.Length,
                    PayloadLength = payload.Length,
                };
                var headerData = header.GetEncodedPacket();

                var internalSendData = new SendData()
                {
                    Socket = this.Socket,
                };
                internalSendData.AddBuffer(headerData);
                internalSendData.AddBuffer(typeData);
                internalSendData.AddBuffer(payload);

                // データを送信します。
                base.SendData(internalSendData);

                Log.Debug(this,
                    "{0}を送信しました。(content={1}bytes)",
                    typename,
                    (payload != null ? payload.Length : -1));
            }
            catch (Exception ex)
            {
                Log.ErrorException(this, ex,
                    "{0}: 送信データのシリアライズに失敗しました。",
                    sendData.GetType());
            }
        }

        /// <summary>
        /// データ受信時に呼ばれます。
        /// </summary>
        protected override void OnSent(DataEventArgs e)
        {
            base.OnSent(e);

            /*if (ex.Error != null)
            {
                return;
            }

            var data = new PbData();
            var offset = 0;
            while (offset < ex.Data.Length)
            {
                if (ParseReceivedData(data, ex.Data, ref offset))
                {
                    if (!data.IsResponse)
                    {
                        Log.TraceMessage(this,
                            "リクエスト({0})を送信しました。",
                            data.Id);
                    }
                }
            }*/
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbConnection()
        {
            InitReceivedPacket();

            ProtocolVersion = new PbProtocolVersion();
            DefaultRequestTimeout = TimeSpan.MaxValue;

            AddRequestHandler<PbCheckProtocolVersionRequest,
                              PbCheckProtocolVersionResponse>(
                HandleCheckProtocolVersionRequest);
        }
    }
}
