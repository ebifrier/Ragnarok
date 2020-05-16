using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// OpenGL用の例外クラスです。
    /// </summary>
    [Serializable()]
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected GLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
