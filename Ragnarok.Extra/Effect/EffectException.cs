using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// エフェクト関係の例外を扱います。
    /// </summary>
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
    }
}
