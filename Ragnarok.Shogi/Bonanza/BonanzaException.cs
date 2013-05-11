using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Bonanza
{
    /// <summary>
    /// ボナンザ用の例外クラスです。
    /// </summary>
    public class BonanzaException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BonanzaException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BonanzaException(string message)
            : base(message)
        {
        }
    }
}
