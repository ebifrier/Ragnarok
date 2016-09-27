using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico
{
    /// <summary>
    /// ニコニコ関連の例外です。
    /// </summary>
    public class NicoException : RagnarokException
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
        /// 対象となったIDを取得します。
        /// </summary>
        public string Id
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(string message)
            : base(message)
        {
            ErrorCode = NicoStatusCode.UnknownError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = NicoStatusCode.UnknownError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(string message, string id)
            : base(id + ": " + message)
        {
            ErrorCode = NicoStatusCode.UnknownError;
            Id = id;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(string message, string id,
                             Exception innerException)
            : base(id + ": " + message, innerException)
        {
            ErrorCode = NicoStatusCode.UnknownError;
            Id = id;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(NicoStatusCode code)
            : base(code.GetDescription())
        {
            ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(NicoStatusCode code, Exception innerException)
            : base(code.GetDescription(), innerException)
        {
            ErrorCode = code;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(NicoStatusCode code, string id)
            : base(id + ": " + code.GetDescription())
        {
            ErrorCode = code;
            Id = id;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(NicoStatusCode code, string id,
                             Exception innerException)
            : base(id + ": " + code.GetDescription(), innerException)
        {
            ErrorCode = code;
            Id = id;
        }
    }
}
