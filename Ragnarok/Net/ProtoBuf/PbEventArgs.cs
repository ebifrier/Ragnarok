using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// コマンド受信時の引数です。
    /// </summary>
    public class PbCommandEventArgs<T> : EventArgs
    {
        /// <summary>
        /// コマンドを取得します。
        /// </summary>
        public T Command
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbCommandEventArgs(T command)
        {
            this.Command = command;
        }
    }

    /// <summary>
    /// リクエスト受信時の引数です。
    /// </summary>
    public class PbRequestEventArgs<TReq, TRes> : EventArgs
    {
        /// <summary>
        /// リクエストを取得します。
        /// </summary>
        public TReq Request
        {
            get;
            private set;
        }

        /// <summary>
        /// レスポンスを取得または設定します。
        /// </summary>
        public TRes Response
        {
            get;
            set;
        }

        /// <summary>
        /// レスポンスのエラーコードを取得または設定します。
        /// </summary>
        public int ErrorCode
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbRequestEventArgs(TReq request)
        {
            this.ErrorCode = PbErrorCode.None;
            this.Request = request;
        }
    }

    /// <summary>
    /// レスポンス受信時の引数です。
    /// </summary>
    public class PbResponseEventArgs<TRes> : EventArgs
    {
        /// <summary>
        /// リクエストのIDを取得します。
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// エラーコードを取得します。
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// レスポンスデータを取得します。
        /// </summary>
        public TRes Response
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbResponseEventArgs(int id, int errorCode, TRes response)
        {
            this.Id = id;
            this.ErrorCode = errorCode;
            this.Response = response;
        }
    }
}
