using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// Ragnarok用の例外クラスです。
    /// </summary>
    public class PbException : RagnarokException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
