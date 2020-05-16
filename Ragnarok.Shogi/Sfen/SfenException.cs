using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Ragnarok.Shogi.Sfen
{
    /// <summary>
    /// SFEN用の例外クラスです。
    /// </summary>
    [Serializable()]
    public class SfenException : ShogiException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SfenException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SfenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SfenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected SfenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
