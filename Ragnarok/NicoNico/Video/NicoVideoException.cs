using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Video
{
    /// <summary>
    /// 動画関係の例外です。
    /// </summary>
    public class NicoVideoException : NicoException
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
        public NicoVideoException(string message)
            : base(message)
        {
            ErrorCode = NicoStatusCode.UnknownError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoVideoException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = NicoStatusCode.UnknownError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoVideoException(string message, string id)
            : base(id + ": " + message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoVideoException(string message, string id,
                                  Exception innerException)
            : base(id + ": " + message, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoVideoException(NicoStatusCode code)
            : base(code.GetDescription())
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoVideoException(NicoStatusCode code, Exception innerException)
            : base(code.GetDescription(), innerException)
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoVideoException(NicoStatusCode code, string id)
            : base(id + ": " + code.GetDescription())
        {
            this.ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoVideoException(NicoStatusCode code, string id,
                                  Exception innerException)
            : base(id + ": " + code.GetDescription(),
                   innerException)
        {
            this.ErrorCode = code;
        }
    }
}
