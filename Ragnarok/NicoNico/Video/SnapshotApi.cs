using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Ragnarok.NicoNico.Video
{
    using Utility;

    /// <summary>
    /// json形式のニコニコ検索APIの使用時に使います。
    /// </summary>
    [DataContract()]
    internal sealed class NicoSnapshotData
    {
        [DataMember()]
        public string dqnid;

        [DataMember()]
        public string type;

        [DataMember()]
        public VideoData[] values;
    }

    /// <summary>
    /// ニコニコスナップショットAPIによる高速・シンプルな動画検索を行います。
    /// </summary>
    public static class SnapshotApi
    {
        /// <summary>
        /// このアプリの標準的なHTTPリクエストを作成します。
        /// </summary>
        private static HttpWebRequest MakeJsonRequest(string url, string json)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var data = Encoding.UTF8.GetBytes(json);

            request.UserAgent = "Ragnarok.NicoNico";
            request.Accept = "application/json";
            request.AutomaticDecompression =
                DecompressionMethods.Deflate |
                DecompressionMethods.GZip;

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
            }

            return request;
        }

        /// <summary>
        /// jsonリクエストのレスポンスを取得します。
        /// </summary>
        private static string RequestJson(string url, string json)
        {
            var request = MakeJsonRequest(url, json);
            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                var bytes = Util.ReadToEnd(stream);
                return Encoding.UTF8.GetString(bytes);
            }
        }

        /// <summary>
        /// 動画のキーワード検索を行います。
        /// </summary>
        /// <remarks>
        /// http://search.nicovideo.jp/docs/api/snapshot.html
        /// </remarks>
        public static VideoData[] Search(string keyword, bool isKeyword = true)
        {
            var keywordQuery = "[\"title\", \"description\", \"tags\"]";
            var tagQuery = "[\"tags_exact\"]";

            var json =
                "{\"query\" : \"" + keyword + "\",\n" +
                "\"service\" : [\"video\"],\n" +
                "\"search\" : " + (isKeyword ? keywordQuery : tagQuery) + ",\n" +
                "\"join\" : [\"cmsid\", \"title\", \"description\", \"start_time\", " +
                            "\"view_counter\", \"comment_counter\", " +
                            "\"mylist_counter\", \"tags\"],\n" +
                "\"sort_by\" : \"start_time\",\n" +
                "\"order\" : \"desc\",\n" +
                "\"from\" : 0,\n" +
                "\"size\" : 99,\n" +
                "\"issuer\" : \"Ragnarok\"}";

            var text = RequestJson(
                @"http://api.search.nicovideo.jp/api/snapshot/",
                json);
            //File.WriteAllText("json.json", text);

            // 結果となるJSONには複数の結果が含まれているため、
            // データの最初の一行分のみを抜き出します。
            var index = text.IndexOf('\n');
            if (index > 0)
            {
                text = text.Substring(0, index);
            }

            var result = JsonUtil.Deserialize<NicoSnapshotData>(text);
            if (result == null || result.values == null)
            {
                throw new FormatException(
                    "jsonファイルの内容が正しくありません。");
            }

            return result.values
                .Where(_ => !string.IsNullOrEmpty(_.IdString))
                .ToArray();
        }
    }
}
