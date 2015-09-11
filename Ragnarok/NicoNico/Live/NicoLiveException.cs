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
        public NicoStatusCode ErrorCode
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
            ErrorCode = NicoStatusCode.UnknownError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = NicoStatusCode.UnknownError;
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
        public NicoLiveException(NicoStatusCode code)
            : base(NicoStatusCodeUtil.GetDescription(code))
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(NicoStatusCode code, Exception innerException)
            : base(NicoStatusCodeUtil.GetDescription(code), innerException)
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(NicoStatusCode code, string id)
            : base(id + ": " + NicoStatusCodeUtil.GetDescription(code))
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(NicoStatusCode code, string id,
                                 Exception innerException)
            : base(id + ": " + NicoStatusCodeUtil.GetDescription(code),
                   innerException)
        {
            this.ErrorCode = code;
        }
    }
}
