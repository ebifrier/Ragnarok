using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// コメントサーバーの例外です。
    /// </summary>
    public class NicoLiveException : NicoException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message, string id)
            : base(message, id)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(string message, string id,
                                 Exception innerException)
            : base(message, id, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(NicoStatusCode code)
            : base(code)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(NicoStatusCode code, Exception innerException)
            : base(code, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(NicoStatusCode code, string id)
            : base(code, id)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveException(NicoStatusCode code, string id,
                                 Exception innerException)
            : base(code, id, innerException)
        {
        }
    }
}
