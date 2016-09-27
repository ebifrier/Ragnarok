using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web;

using Ragnarok.Net;

namespace Ragnarok.NicoNico.Provider
{
    /// <summary>
    /// コミュニティ情報を保持します。
    /// </summary>
    public class CommunityInfo
    {
        /// <summary>
        /// コミュニティIDを取得します。
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// coXXX形式のコミュニティＩＤを取得します。
        /// </summary>
        public string IdString
        {
            get
            {
                return string.Format("co{0}", Id);
            }
        }

        /// <summary>
        /// コミュニティがすでに閉じているか、またはクローズなコミュニティ
        /// であるかどうかを取得します。
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return (this.CloseDate != null);
            }
        }

        /// <summary>
        /// コミュニティ名を取得します。
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// コミュニティプロフィールを取得します。
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// コミュニティレベルを取得します。
        /// </summary>
        public int Level
        {
            get;
            private set;
        }

        /// <summary>
        /// コミュニティの参加人数を取得します。
        /// </summary>
        public int NumberOfMembers
        {
            get;
            private set;
        }

        /// <summary>
        /// コミュニティサムネイルのURLを取得します。
        /// </summary>
        public Uri ThumbnailUrl
        {
            get
            {
                return new Uri(string.Format(
                    "http://icon.nimg.jp/community/co{0}.jpg",
                    Id),
                    UriKind.Absolute);
            }
        }

        /// <summary>
        /// サムネイル画像のデータを取得します。画像フォーマットはjpgです。
        /// </summary>
        public byte[] ThumbnailImage
        {
            get
            {
                return WebUtil.RequestHttp(
                    ThumbnailUrl.ToString(),
                    null);
            }
        }

        /// <summary>
        /// オーナーのユーザーIDを取得します。
        /// </summary>
        public int OwnerId
        {
            get;
            private set;
        }

        /// <summary>
        /// オーナー名を取得します。
        /// </summary>
        public string OwnerName
        {
            get;
            private set;
        }

        /// <summary>
        /// 開設日を取得します。
        /// </summary>
        public DateTime OpenDate
        {
            get;
            private set;
        }

        /// <summary>
        /// もし確認できれば、コミュニティが閉じた日付を取得します。
        /// </summary>
        public DateTime? CloseDate
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommunityInfo()
        {
            CloseDate = null;
        }

        /// <summary>
        /// コミュニティ番号が含まれる文字列から情報を作成します。
        /// </summary>
        public static CommunityInfo Create(string communityStr,
                                           CookieContainer cc)
        {
            var id = NicoUtil.GetCommunityId(communityStr);
            if (id < 0)
            {
                throw new NicoProviderException(
                    "コミュニティIDが取得できませんでした。");
            }

            return Create(id, cc);
        }

        /// <summary>
        /// Urlからコミュニティ情報を取得します。
        /// </summary>
        /// <exception cref="System.TimeoutException" />
        /// <exception cref="System.Net.WebException" />        
        public static CommunityInfo Create(int id, CookieContainer cc)
        {
            // ページを取得します。
            var responseData = WebUtil.RequestHttp(
                NicoString.GetCommunityUrl(id),
                null, cc);

            // 失敗;; コミュニティエラー時はレスポンスが空になります。
            if (responseData == null)
            {
                throw new NicoProviderException(
                    "コミュニティページの取得に失敗しました。",
                    NicoString.CommunityIdString(id));
            }

            var text = Encoding.UTF8.GetString(responseData);
            return CreateFromHtml(id, text);
        }

        /// <summary>
        /// html形式のコミュニティページからコミュニティ情報を作成します。
        /// </summary>
        public static CommunityInfo CreateFromHtml(int id, string pageStr)
        {
            var community = new CommunityInfo();

            if (string.IsNullOrEmpty(pageStr))
            {
                throw new NicoProviderException(
                    "与えられたページがありません。",
                    NicoString.CommunityIdString(id));
            }

            var m = Regex.Match(pageStr,
                @"<img src=""http://icon\.nimg\.jp/community/\d+/co([0-9]+)\.jpg\?[0-9]+""");
            if (!m.Success)
            {
                throw new NicoProviderException(
                    "コミュニティIDを取得できませんでした。",
                    NicoString.CommunityIdString(id));
            }
            community.Id = int.Parse(m.Groups[1].Value);

            m = Regex.Match(pageStr,
                @"<title>\s*(.*)\s*-?\s*ニコニコ[^<]*?\s*</title>");
            if (!m.Success)
            {
                throw new NicoProviderException(
                    "コミュニティタイトルを取得できませんでした。",
                    NicoString.CommunityIdString(id));
            }
            community.Title = WebUtil.DecodeHtmlText(m.Groups[1].Value);

            m = Regex.Match(pageStr,
                @"<div class=""cnt2"" style=""[^""]+?"">\s*" +
                @"((.|\s)*?)\s*</div></div></div>\s*" +
                @"<!-- subbox profile -->");
            if (!m.Success)
            {
                m = Regex.Match(pageStr,
                    @"<div id=""community_description""><!-- ID は暫定 -->\s*" +
                    @"()</div>\s*</div>\s*<div id=""cbox_news"">");

                if (!m.Success)
                {
                    throw new NicoProviderException(
                        "コミュニティプロフィールを取得できませんでした。",
                        NicoString.CommunityIdString(id));
                }
            }
            community.Description = WebUtil.DecodeHtmlText(m.Groups[1].Value);            

            // <td nowrap>レベル：</td> 
            //   <td> 
            //      <strong>13</strong>
            m = Regex.Match(pageStr,
                @"<td nowrap>レベル：</td>\s*<td>\s*<strong>([0-9]+)</strong>");
            if (!m.Success)
            {
                throw new NicoProviderException(
                    "コミュニティのレベルを取得できませんでした。",
                    NicoString.CommunityIdString(id));
            }
            community.Level = int.Parse(m.Groups[1].Value);

            // <td nowrap>メンバー：</td> 
            //   <td> 
            //      <strong>87</strong>
            m = Regex.Match(pageStr,
                @"<td nowrap>メンバー：</td>\s*<td>\s*<strong>([0-9]+)</strong>");
            if (!m.Success)
            {
                throw new NicoProviderException(
                    "コミュニティの参加メンバー数を取得できませんでした。",
                    NicoString.CommunityIdString(id));
            }
            community.NumberOfMembers = int.Parse(m.Groups[1].Value);

            m = Regex.Match(pageStr,
                @"オーナー：<a href=""http://www\.nicovideo\.jp/user/([0-9]+)"" " +
                @"style=""color:#FFF;"" target=""_blank"">\s*" +
                @"<strong>([^<]+)</strong></a>");
            if (!m.Success)
            {
                throw new NicoProviderException(
                    "コミュニティのオーナーを取得できませんでした。",
                    NicoString.CommunityIdString(id));
            }
            community.OwnerId = int.Parse(m.Groups[1].Value);
            community.OwnerName = WebUtil.DecodeHtmlText(m.Groups[2].Value);

            m = Regex.Match(pageStr,
                @"開設日：<strong>(\d+)年(\d+)月(\d+)日</strong>");
            if (!m.Success)
            {
                throw new NicoProviderException(
                    "コミュニティの開設日が取得できませんでした。",
                    NicoString.CommunityIdString(id));
            }
            community.OpenDate = new DateTime(
                int.Parse(m.Groups[1].Value),
                int.Parse(m.Groups[2].Value),
                int.Parse(m.Groups[3].Value));

            return community;
        }
    }
}
