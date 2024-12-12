using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Net
{
    /// <summary>
    /// コネクションの例外です。
    /// </summary>
    public class RagnarokNetException : RagnarokException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokNetException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokNetException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RagnarokNetException(string message,
                               Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
