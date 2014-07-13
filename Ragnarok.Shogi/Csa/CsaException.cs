using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Csa
{
    /// <summary>
    /// CSA用の例外クラスです。
    /// </summary>
    public class CsaException : ShogiException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CsaException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CsaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CsaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
