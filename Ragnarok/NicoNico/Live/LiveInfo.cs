using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using Ragnarok.Net;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// 生放送ページの情報を保持します。
    /// </summary>
    /// <remarks>
    /// playerstatusなどでは放送タイトルや放送概要の文字数制限に
    /// 引っかかってしまうことがあるため、別に取得します。
    /// </remarks>
    public class LiveInfo
    {
        /// <summary>
        /// 生放送ＩＤを取得します。
        /// </summary>
        public long Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 生放送ID文字列を取得します。(lvXXXX)
        /// </summary>
        public string IdString
        {
            get
            {
                return string.Format("lv{0}", Id);
            }
        }

        /// <summary>
        /// 放送タイトルを取得します。
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// 放送概要を取得します。
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// 総来場者数を取得します。
        /// </summary>
        public int CommunityTotalVisitors
        {
            get;
            private set;
        }

        /// <summary>
        /// コミュニティレベルを取得します。
        /// </summary>
        public int CommunityLevel
        {
            get;
            private set;
        }

        private LiveInfo()
        {
            CommunityTotalVisitors = -1;
            CommunityLevel = -1;
        }

        /// <summary>
        /// 生放送番号が含まれる文字列から情報を作成します。
        /// </summary>
        /// <remarks>
        /// 放送タイトルや概要だけなら、ログインしなくても見ることができます。
        /// </remarks>
        public static LiveInfo Create(string liveStr, CookieContainer cc)
        {
            // lvXXXX が含まれていればその放送ＩＤをそのまま使います。
            var m = Regex.Match(liveStr, "lv([0-9]+)");
            if (m.Success)
            {
                var id = long.Parse(m.Groups[1].Value);
                return Create(id, cc);
            }

            // 一度放送ページを取得して、そこから放送ＩＤを探します。
            // 放送URLはcoXXXXを含むURLのことがあります。
            var page = WebUtil.RequestHttpText(
                liveStr,
                null,
                cc,
                Encoding.UTF8);

            return CreateFromHtml(liveStr, page);
        }

        /// <summary>
        /// Urlから生放送情報を取得します。
        /// </summary>
        public static LiveInfo Create(long liveId, CookieContainer cc)
        {
            // urlを取得します。
            var responseData = WebUtil.RequestHttp(
                NicoString.GetLiveUrl(liveId),
                null,
                cc);

            // 失敗;; エラー時はレスポンスが空になります。
            if (responseData == null)
            {
                throw new NicoException(
                    "放送ページの取得に失敗しました。",
                    LiveUtil.LiveIdString(liveId));
            }

            var text = Encoding.UTF8.GetString(responseData);
            return CreateFromHtml(LiveUtil.LiveIdString(liveId), text);
        }

        private static readonly Regex VideoInfoRegex = new Regex(
            @"<script type=""text/javascript"">\s*<!--" +
            @"\s*var Video = [{]" +
            @"\s*((.|\s)+?)" +
            @"\s*[}];" +
            @"\s*-->\s*</script>",
            RegexOptions.IgnoreCase);
        private static readonly Regex IdRegex = new Regex(
            @"id:\s+'lv([0-9]+)',",
            RegexOptions.IgnoreCase);
        private static readonly Regex TitleRegex = new Regex(
            @"title:\s+'((.|\')*)',",
            RegexOptions.IgnoreCase);
        private static readonly Regex DescriptionRegex = new Regex(
            @"description:\s+'((.|\')*)',",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// html形式の生放送ページから生放送情報を作成します。
        /// </summary>
        public static LiveInfo CreateFromHtml(string idString, string pageStr)
        {
            var live = new LiveInfo();

            if (string.IsNullOrEmpty(pageStr))
            {
                throw new NicoLiveException(
                    "ページが空です。",
                    idString);
            }

            var m = VideoInfoRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "生放送情報を取得できませんでした。",
                    idString);
            }

            var infoStr = m.Groups[1].Value;

            m = IdRegex.Match(infoStr);
            if (!m.Success)
            {
                throw new NicoException(
                    "生放送IDを取得できませんでした。",
                    idString);
            }
            live.Id = int.Parse(m.Groups[1].Value);

            m = TitleRegex.Match(infoStr);
            if (!m.Success)
            {
                throw new NicoException(
                    "生放送タイトルを取得できませんでした。",
                    idString);
            }
            live.Title = m.Groups[1].Value
                .Replace("\\'", "'");

            m = DescriptionRegex.Match(infoStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "生放送概要を取得できませんでした。",
                    idString);
            }
            live.Description = m.Groups[1].Value
                .Replace("\\'", "'")
                .Replace("\\r\\n", "\n");

            if (SetCommunityInfoHarajuku(live, pageStr))
            {
                return live;
            }
            else if (SetCommunityInfoQ(live, pageStr))
            {
                return live;
            }

            return null;
        }

        private static readonly Regex HaraCommunityRegex = new Regex(
            @"<td colspan=""2"" class=""counter"">\s+" +
            @"((.|\n)+?)\s*</td>",
            RegexOptions.IgnoreCase);
        private static readonly Regex HaraCommunityVisitorRegex = new Regex(
            @"累計来場者数：<strong [^>]+>([0-9,]+)</strong>",
            RegexOptions.IgnoreCase);
        private static readonly Regex HaraCommunityLevelRegex = new Regex(
            @"/レベル：<strong [^>]+>([0-9,]+)</strong>",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// 原宿バージョンのコミュニティ情報(累計来場者数など)を取得します。
        /// </summary>
        private static bool SetCommunityInfoHarajuku(LiveInfo live, string pageStr)
        {
            // コミュニティであれば、コミュニティ情報を取得します。
            string communityStr;

            var m = HaraCommunityRegex.Match(pageStr);
            if (m.Success)
            {
                communityStr = m.Groups[1].Value;
            }
            else
            {
                m = Regex.Match(pageStr,
                    @"<h3 title=""COMMUNITY INFO"">COMMUNITY INFO</h3>");
                if (!m.Success)
                {
                    return false;
                }

                communityStr = pageStr.Substring(m.Index);
            }

            // 累計来場者数：<strong style="font-size: 14px;">104,497</strong>
            m = HaraCommunityVisitorRegex.Match(communityStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "累計来場者数の取得に失敗しました。",
                    live.IdString);
            }
            live.CommunityTotalVisitors = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);

#if false
            // /参加人数：<strong style="font-size: 14px;">1581</strong>
            m = haraCommunityMemberRegex.Match(communityStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "コミュニティ参加人数の取得に失敗しました。",
                    live.IdString);
            }
            live.CommunityMembers = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);
#endif

            // /レベル：<strong style="font-size: 14px;">40</strong>
            m = HaraCommunityLevelRegex.Match(communityStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "コミュニティレベルの取得に失敗しました。",
                    live.IdString);
            }
            live.CommunityLevel = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);

            return true;
        }

        private static readonly Regex QCommunityLevelRegex = new Regex(
            @"<span class=""commu_lv"">レベル：([0-9,]+)</span>",
            RegexOptions.IgnoreCase);
        private static readonly Regex QCommunityVisitorsRegex = new Regex(
            @"<span class=""visitor_score"">累計来場者数：([0-9,]+)</span>",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// Qバージョンのコミュニティ情報(累計来場者数など)を取得します。
        /// </summary>
        private static bool SetCommunityInfoQ(LiveInfo live, string pageStr)
        {
            // 累計来場者数
            var m = QCommunityVisitorsRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "累計来場者数の取得に失敗しました。",
                    live.IdString);
            }
            live.CommunityTotalVisitors = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);

            // /レベル
            m = QCommunityLevelRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "コミュニティレベルの取得に失敗しました。",
                    live.IdString);
            }
            live.CommunityLevel = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);

            return true;
        }
    }
}
