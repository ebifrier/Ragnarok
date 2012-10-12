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
        public NicoException(string message, string id)
            : base(id + ": " + message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoException(string message, string id, Exception innerException)
            : base(id + ": " + message, innerException)
        {
        }
    }
}
