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

namespace Ragnarok.NicoNico
{
    using Provider;

    /// <summary>
    /// ニコニコ関連の共通コードを持ちます。
    /// </summary>
    public static class NicoUtil
    {
        /// <summary>
        /// 指定のURLをxmlとして取得します。
        /// </summary>
        public static XmlNode GetXml(string url, CookieContainer cc)
        {
            // 生放送ＩＤから放送情報を取得します。
            var responseData = WebUtil.RequestHttp(url, null, cc);
            if (responseData == null)
            {
                return null;
            }

            return GetXml(responseData);
        }
        
        /// <summary>
        /// 指定のデータをxmlとして取得します。
        /// </summary>
        public static XmlNode GetXml(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            // 生データから読み込まないとエンコーディングの不一致で
            // エラーが返ってくる。
            // 無幅空白が含まれてるせいらしいのだけど、よく分からない。
            using (var stream = new MemoryStream(data))
            {
                var reader = XmlReader.Create(stream);
                var doc = new XmlDocument();
                doc.Load(reader);

                return doc;
            }
        }

        /// <summary>
        /// 提供者を文字列から解析します。
        /// </summary>
        public static ProviderData ParseProvider(string providerStr)
        {
            if (string.IsNullOrEmpty(providerStr))
            {
                return null;
            }

            // 提供者を取得します。
            if (providerStr.StartsWith("co", StringComparison.InvariantCultureIgnoreCase))
            {
                return new ProviderData(
                    ProviderType.Community,
                    StrUtil.ToInt(providerStr.Substring(2), -1));
            }
            else if (providerStr.StartsWith("ch", StringComparison.InvariantCultureIgnoreCase))
            {
                return new ProviderData(
                    ProviderType.Channel,
                    StrUtil.ToInt(providerStr.Substring(2), -1));
            }
            else if (providerStr.Equals("official", StringComparison.InvariantCultureIgnoreCase))
            {
                return new ProviderData(
                    ProviderType.Official, -1);
            }

            return null;
        }

        /// <summary>
        /// コミュニティＩＤを取得します。
        /// </summary>
        public static int GetCommunityId(string communityStr)
        {
            var m = Regex.Match(communityStr, "co([0-9]+)");
            if (m.Success)
            {
                return int.Parse(m.Groups[1].Value);
            }

            return -1;
        }
    }
}
