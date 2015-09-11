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
    using Provider;

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
        /// 生放送IDを取得します。
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
        public int TotalVisitors
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
            TotalVisitors = -1;
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
                    NicoString.LiveIdString(liveId));
            }

            var text = Encoding.UTF8.GetString(responseData);
            return CreateFromHtml(NicoString.LiveIdString(liveId), text);
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

        private static readonly Regex ProviderInfoRegex = new Regex(
            @"<script type=""text/javascript"">\s*<!--" +
            @"\s*Nicolive_JS_Conf.Watch = {" +
            @"[\s\S]+?,""provider_type"":""(\w+)"",[\s\S]+?" +
            @"\s*--></script>");

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

            var m = ProviderInfoRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "放送の提供元情報を取得できませんでした。",
                    idString);
            }
            var provider = NicoUtil.ParseProvider(m.Groups[1].Value);

            m = VideoInfoRegex.Match(pageStr);
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

            switch (provider.ProviderType)
            {
                case ProviderType.Community:
                    SetCommunityInfo(live, pageStr);
                    break;
                case ProviderType.Channel:
                    SetChannelInfo(live, pageStr);
                    break;
                case ProviderType.Official:
                    SetOfficialInfo(live, pageStr);
                    break;
                default:
                    throw new NotImplementedException(
                        "実装されていない放送提供元です。");
            }

            return live;
        }

        #region ユーザー生放送
        private static readonly Regex CommunityLevelRegex = new Regex(
            @"<span class=""commu_lv"">レベル：([0-9,]+)</span>",
            RegexOptions.IgnoreCase);
        private static readonly Regex CommunityVisitorsRegex = new Regex(
            @"<span class=""visitor_score"">累計来場者数：([0-9,]+)</span>",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// コミュニティ情報(累計来場者数など)を取得します。
        /// </summary>
        private static void SetCommunityInfo(LiveInfo live, string pageStr)
        {
            // 累計来場者数
            var m = CommunityVisitorsRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "累計来場者数の取得に失敗しました。",
                    live.IdString);
            }
            live.TotalVisitors = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);

            // /レベル
            m = CommunityLevelRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoLiveException(
                    "コミュニティレベルの取得に失敗しました。",
                    live.IdString);
            }
            live.CommunityLevel = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);
        }
        #endregion

        #region チャンネル放送
        private static readonly Regex ChannelVisitorsRegex = new Regex(
            @"<span class=""total_score"">累計来場者数：([0-9,]+)</span>",
            RegexOptions.IgnoreCase);

        private static void SetChannelInfo(LiveInfo live, string pageStr)
        {
            // 累計来場者数
            var m = ChannelVisitorsRegex.Match(pageStr);
            if (!m.Success)
            {
                live.TotalVisitors = 0;
            }
            else
            {
                live.TotalVisitors = int.Parse(
                    m.Groups[1].Value,
                    NumberStyles.AllowThousands);
            }
        }
        #endregion

        #region 公式生放送
        private static readonly Regex OfficialVisitorsRegex = new Regex(
            @"<span class=""total_score"">累計来場者数：([0-9,]+)</span>",
            RegexOptions.IgnoreCase);

        private static void SetOfficialInfo(LiveInfo live, string pageStr)
        {
            // 累計来場者数
            var m = OfficialVisitorsRegex.Match(pageStr);
            if (!m.Success)
            {
                live.TotalVisitors = 0;
            }
            else
            {
                live.TotalVisitors = int.Parse(
                    m.Groups[1].Value,
                    NumberStyles.AllowThousands);
            }
        }
        #endregion
    }
}
