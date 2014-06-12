using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Sfen
{
    /// <summary>
    /// SFEN用の例外クラスです。
    /// </summary>
    public class SfenException : Exception
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
    }
}
