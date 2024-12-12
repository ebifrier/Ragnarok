using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok
{
    /// <summary>
    /// Ragnarok.Update用の例外クラスです。
    /// </summary>
    public class RagnarokUpdateException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokUpdateException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokUpdateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
