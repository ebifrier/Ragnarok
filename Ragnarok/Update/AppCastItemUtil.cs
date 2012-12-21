using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ragnarok.Update
{
    /// <summary>
    /// アプリの最新バージョン情報をネット上から取得します。
    /// </summary>
    internal static class AppCastItemUtil
    {
        private const string TopItemNode = "/rss/channel/item";
        private const string TitleNode = "title";
        private const string UpdatePackLinkNode = "sparkle:updatePackLink";
        private const string ReleaseNotesLinkNode = "sparkle:releaseNotesLink";
        private const string EnclosureNode = "enclosure";
        private const string UrlAttribute = "url";
        private const string VersionAttribute = "sparkle:version";
        private const string MD5Signature = "sparkle:md5Signature";

        /// <summary>
        /// バージョン情報をXMLノードから作成します。
        /// </summary>
        private static AppCastItem CreateAppCastItemFromNode(XmlDocument doc, XmlNode e)
        {
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("sparkle", "http://www.andymatuschak.org/xml-namespaces/sparkle");

            var titleNode = e.SelectSingleNode(TitleNode);
            if (titleNode == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にtitleタグがありません。");
            }

            var updatePackNode = e.SelectSingleNode(UpdatePackLinkNode, ns);
            if (updatePackNode == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にupdatePackLinkタグがありません。");
            }

            var releaseNoteNode = e.SelectSingleNode(ReleaseNotesLinkNode, ns);
            if (releaseNoteNode == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にreleaseNoteLinkタグがありません。");
            }

            var enclosureNode = e.SelectSingleNode(EnclosureNode);
            if (enclosureNode == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にenclosureタグがありません。");
            }

            var urlAttr = enclosureNode.Attributes[UrlAttribute];
            if (urlAttr == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にアプリのurlがありません。");
            }

            var versionAttr = enclosureNode.Attributes[VersionAttribute];
            if (versionAttr == null)
            {
                throw new RagnarokUpdateException(
                    "バージョン情報にアプリのバージョンがありません。");
            }

            // md5は無くても良しとする。
            var md5Attr = enclosureNode.Attributes[MD5Signature];

            return new AppCastItem
            {
                AppName = titleNode.InnerText,
                UpdatePackLink = updatePackNode.InnerText.Trim(),
                ReleaseNotesLink = releaseNoteNode.InnerText.Trim(),
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
        public static AppCastItem GetLatestVersion(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            /*using (var textReader = new StringReader(text))
            using (var reader = XmlReader.Create(textReader))*/
            {
                var doc = new XmlDocument
                {
                    PreserveWhitespace = false,
                };
                doc.LoadXml(text);                

                var itemNodes = doc.SelectNodes(TopItemNode);
                AppCastItem latestVersion = null;

                // rss中の全itemタグを検索します。
                foreach (var itemNode in itemNodes.OfType<XmlNode>())
                {
                    var currentItem = CreateAppCastItemFromNode(doc, itemNode);

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
