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
        /// コンストラクタ
        /// </summary>
        public NicoException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(string errorCode, string id)
            : base(id + ": " + errorCode)
        {
            ErrorCode = errorCode;
            Id = id;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(string errorCode, string id, Exception innerException)
            : base(id + ": " + errorCode, innerException)
        {
            ErrorCode = errorCode;
            Id = id;
        }

        /// <summary>
        /// エラーコードを取得します。
        /// </summary>
        public string ErrorCode
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
    }
}
