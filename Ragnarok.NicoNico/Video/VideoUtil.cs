using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

using Ragnarok.Net;

namespace Ragnarok.NicoNico.Video
{
    /// <summary>
    /// 動画関連の共通コードを持ちます。
    /// </summary>
    public static class VideoUtil
    {
        /// <summary>
        /// 動画URLや他の文字列からsmXXXXなどを含む動画IDを取得します。
        /// </summary>
        public static string GetVideoId(string videoStr)
        {
            if (string.IsNullOrEmpty(videoStr))
            {
                throw new ArgumentNullException("videoStr");
            }

            // smXXXXやsoXXXXが含まれていればその動画IDをそのまま使います。
            var m = Regex.Match(videoStr, "(sm|so)([0-9]+)");
            if (m.Success)
            {
                return m.Value;
            }

            // XXXX が含まれていればそのスレッドIDをそのまま使います。
            m = Regex.Match(videoStr, @"(^|/)([0-9]+)([^\w]|$)");
            if (m.Success)
            {
                return m.Groups[2].Value;
            }

            return null;
        }
    }
}
