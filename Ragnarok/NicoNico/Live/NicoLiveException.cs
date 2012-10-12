using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// コメントサーバーの例外です。
    /// </summary>
    public class NicoLiveException : NicoException
    {
        /// <summary>
        /// エラーコードを取得します。
        /// </summary>
        public LiveStatusCode ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message)
            : base(message)
        {
            ErrorCode = LiveStatusCode.UnknownError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = LiveStatusCode.UnknownError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message, string id)
            : base(id + ": " + message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message, string id,
                                 Exception innerException)
            : base(id + ": " + message, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(LiveStatusCode code)
            : base(LiveStatusCodeUtil.GetDescription(code))
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(LiveStatusCode code, Exception innerException)
            : base(LiveStatusCodeUtil.GetDescription(code), innerException)
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(LiveStatusCode code, string id)
            : base(id + ": " + LiveStatusCodeUtil.GetDescription(code))
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(LiveStatusCode code, string id,
                                 Exception innerException)
            : base(id + ": " + LiveStatusCodeUtil.GetDescription(code),
                   innerException)
        {
            this.ErrorCode = code;
        }
    }
}
