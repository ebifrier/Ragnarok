using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using Ragnarok.Net;

namespace Ragnarok.NicoNico.Video
{
    [DataContract]
    public sealed class NicoApiError
    {
        [DataMember(Name = "code")]
        public string Code
        {
            get;
            private set;
        }

        [DataMember(Name = "description")]
        public string Description
        {
            get;
            private set;
        }
    }

    [DataContract]
    public sealed class NicoApiResult
    {
        [DataMember(Name = "status")]
        public string Status
        {
            get;
            private set;
        }

        [DataMember(Name = "error")]
        public NicoApiError Error
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// 動画マイリストを管理します。
    /// </summary>
    public sealed class Mylist
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Mylist(int mylistId)
        {
            Id = mylistId;
        }

        /// <summary>
        /// マイリストIDを取得します。
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        private string GetToken(CookieContainer cc)
        {
            var page = WebUtil.RequestHttpText(
                "http://www.nicovideo.jp/my/mylist",
                null, cc, Encoding.UTF8);
            var regex = new Regex(@"^\s*NicoAPI\.token = ""([\d\w\-]+)"";", RegexOptions.Multiline);
            var m = regex.Match(page);
            if (!m.Success)
            {
                return string.Empty;
            }

            return m.Groups[1].Value;
        }

        private void Call(CookieContainer cc, string relativeUri,
                          Dictionary<string, object> paramDic)
        {
            var token = GetToken(cc);

            paramDic.Add("token", token);
            var param = WebUtil.EncodeParam(paramDic);
            var uri = new Uri(new Uri("http://www.nicovideo.jp/"), relativeUri + "?" + param);

            var json = WebUtil.RequestHttpText(uri.AbsoluteUri, null, cc, Encoding.UTF8);
            var result = Ragnarok.Utility.JsonUtil.Deserialize<NicoApiResult>(json);
            //Console.WriteLine(result.Status);
        }

        /// <summary>
        /// マイリストに動画を追加します。
        /// </summary>
        public void AddMovie(CookieContainer cc, string movieId,
                             string description = null)
        {
            Call(cc, "/api/mylist/add", new Dictionary<string,object>
            {
                { "group_id", Id },
                { "item_type", 0 },
                { "item_id", movieId },
                { "description", description },
            });
        }
    }
}
