using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.Net
{
    /// <summary>
    /// コネクションの例外です。
    /// </summary>
    [Serializable()]
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected RagnarokNetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
