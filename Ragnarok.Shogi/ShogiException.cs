using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// Ragnarok.Shogi用の例外クラスです。
    /// </summary>
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
    }
}
