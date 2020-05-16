using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// Ragnarok.Shogi用の例外クラスです。
    /// </summary>
    [Serializable()]
    public class ShogiException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShogiException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShogiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShogiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected ShogiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
