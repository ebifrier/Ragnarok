using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// エフェクト関係の例外を扱います。
    /// </summary>
    [Serializable()]
    public class EffectException : RagnarokException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected EffectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
