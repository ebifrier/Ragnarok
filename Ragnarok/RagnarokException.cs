using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
namespace Ragnarok
{
    /// <summary>
    /// Ragnarok用の例外クラスです。
    /// </summary>
    [Serializable()]
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected RagnarokException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
