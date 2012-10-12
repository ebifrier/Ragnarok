using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok
{
    /// <summary>
    /// Ragnarok用の例外クラスです。
    /// </summary>
    public class RagnarokException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
