using System;
using System.Collections.Generic;
using System.Linq;

using Ragnarok.Net;

namespace Ragnarok
{
    /// <summary>
    /// 開始時に呼んでほしい処理があります。
    /// </summary>
    public static class Initializer
    {
        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public static void Initialize()
        {
            // 静的コンストラクタで時刻同期を行います。
            NtpClient.GetTime();
        }
    }
}
