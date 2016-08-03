using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// OpenGL用の例外クラスです。
    /// </summary>
    public class GLException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
