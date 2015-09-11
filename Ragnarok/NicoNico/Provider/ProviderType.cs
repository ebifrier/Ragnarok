using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Provider
{
    /// <summary>
    /// 各放送の提供者です。
    /// </summary>
    public enum ProviderType
    {
        /// <summary>
        /// 不明な提供者です。
        /// </summary>
        Unknown,

        /// <summary>
        /// コミュニティ提供の放送です。
        /// </summary>
        Community,

        /// <summary>
        /// チャンネル提供の放送です。
        /// </summary>
        Channel,

        /// <summary>
        /// 公式生提供の放送です。
        /// </summary>
        Official,
    }
}
