using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Ragnarok.Update
{
    /// <summary>
    /// アプリの最新バージョン情報をネット上から取得します。
    /// </summary>
    internal static class AppCastItemUtil
    {
        private const string TopItemNode = "/rss/channel/item";
        private const string TitleNode = "title";
        private const string ReleaseNotesLinkNode = "sparkle:releaseNotesLink";
        private const string EnclosureNode = "enclosure";
        private const string UrlAttribute = "url";
        private const string VersionAttribute = "sparkle:version";
        private const string MD5Signature = "sparkle:md5Signature";

        /// <summary>
        /// バージョン情報をネット上から取得します。
        /// </summary>
        private static Stream GetLatestVersionStream(string url)
        {
            // build a http web request stream
            var request = HttpWebRequest.Create(url);

            // request the cast and build the stream
            var response = request.GetResponse();

            return response.GetResponseStream();
        }

        /// <summary>
        /// バージョン情報をXMLノードから作成します。
        /// </summary>
        private static AppCastItem CreateAppCastItemFromNode(XElement e)
        {
            var titleNode = e.XPathSelectElement(TitleNode);
            if (titleNode == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にtitleタグがありません。");
            }

            var releaseNoteNode = e.XPathSelectElement(ReleaseNotesLinkNode);
            if (releaseNoteNode == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にreleaseNoteLinkタグがありません。");
            }

            var enclosureNode = e.XPathSelectElement(EnclosureNode);
            if (enclosureNode == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にenclosureタグがありません。");
            }

            var urlAttr = enclosureNode.Attribute(UrlAttribute);
            if (urlAttr == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にアプリのurlがありません。");
            }

            var versionAttr = enclosureNode.Attribute(VersionAttribute);
            if (versionAttr == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にアプリのバージョンがありません。");
            }

            // md5は無くても良しとする。
            var md5Attr = enclosureNode.Attribute(MD5Signature);

            return new AppCastItem
            {
                AppName = titleNode.Value,
                ReleaseNotesLink = releaseNoteNode.Value,
                DownloadLink = urlAttr.Value,
                Version = versionAttr.Value,
                MD5Signature = (md5Attr != null ? md5Attr.Value : null),
            };
        }

        /// <summary>
        /// アプリの最新バージョン情報を取得します。
        /// </summary>
        /// <remarks>
        /// このメソッドは例外を投げる可能性があります。
        /// </remarks>
        public static AppCastItem GetLatestVersion(string castUrl)
        {
            using (var stream = GetLatestVersionStream(castUrl))
            using (var reader = XmlReader.Create(stream))
            {
                var topNode = XElement.Load(reader);
                var itemNodes = topNode.XPathSelectElements(TopItemNode);

                AppCastItem latestVersion = null;

                // rss中の全itemタグを検索します。
                foreach (var itemNode in itemNodes)
                {
                    var currentItem = CreateAppCastItemFromNode(itemNode);

                    if (latestVersion == null)
                    {
                        latestVersion = currentItem;
                    }
                    else if (currentItem.CompareTo(latestVersion) > 0)
                    {
                        latestVersion = currentItem;
                    }
                }
            
                // go ahead
                return latestVersion;
            }
        }
    }
}
