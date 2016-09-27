using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ragnarok.NicoNico.Provider
{
    /// <summary>
    /// チャンネルツール上で発生した例外を扱います。
    /// </summary>
    public class ChannelToolException : NicoProviderException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChannelToolException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChannelToolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
