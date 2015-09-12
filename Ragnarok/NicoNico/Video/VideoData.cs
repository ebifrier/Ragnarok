using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Ragnarok.NicoNico.Video
{
    using Net;
    using Utility;

    /// <summary>
    /// 動画検索時の結果を保持するための簡易クラスです。
    /// </summary>
    [Serializable()]
    [DataContract()]
    public sealed class VideoData : XmlInfomationBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VideoData()
        {
            ThreadId = -1;
            TagList = new List<string>();
            StartTime = DateTime.MinValue;
            ViewCounter = -1;
            CommentCounter = -1;
            MylistCounter = -1;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private VideoData(string videoId, XmlNode node)
            : base(node, "nicovideo_thumb_response", videoId)
        {
            ThreadId = -1;
            TagList = new List<string>();
            StartTime = DateTime.MinValue;
            ViewCounter = -1;
            CommentCounter = -1;
            MylistCounter = -1;
            IsVisible = true;

            var root = RootNode.SelectSingleNode("thumb");
            if (root == null)
            {
                throw new NicoException(
                    "XMLがパースできません。", videoId);
            }

            foreach (var childObj in root.ChildNodes)
            {
                var child = (XmlNode)childObj;
                var text = child.InnerText;
                
                switch (child.Name)
                {
                    case "video_id":
                        this.IdString = text;
                        break;
                    case "title":
                        this.Title = text;
                        break;
                    case "description":
                        this.Description = text;
                        break;
                    case "first_retrieve":
                        this.StartTime = DateTime.Parse(text);
                        break;
                    case "view_counter":
                        this.ViewCounter = StrUtil.ToInt(text, 0);
                        break;
                    case "comment_num":
                        this.CommentCounter = StrUtil.ToInt(text, 0);
                        break;
                    case "mylist_counter":
                        this.MylistCounter = StrUtil.ToInt(text, 0);
                        break;
                    case "tags":
                        this.TagList = child.ChildNodes
                            .OfType<XmlNode>()
                            .Where(_ => _.Name == "tag")
                            .Select(_ => _.InnerText)
                            .ToList();
                        break;
                }
            }
        }

        /// <summary>
        /// soやsmから始まる動画のファイルIDを取得します。
        /// </summary>
        [DataMember(Name = "cmsid")]
        public override string IdString
        {
            get;
            protected set;
        }

        /// <summary>
        /// 動画URLに使われる数字のみのIDを取得します。
        /// </summary>
        [DataMember(Name = "thread")]
        public long ThreadId
        {
            get;
            private set;
        }

        /// <summary>
        /// 動画URLに使われる数字のみのIDを取得します。
        /// </summary>
        public string ThreadIdString
        {
            get
            {
                if (ThreadId < 0)
                {
                    return null;
                }

                return string.Format("{0}", ThreadId);
            }
        }

        /// <summary>
        /// 動画タイトルを取得します。
        /// </summary>
        [DataMember(Name = "title")]
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// この動画の詳細を取得します。
        /// </summary>
        [DataMember(Name = "description")]
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// タグ一覧を取得します。
        /// </summary>
        public List<string> TagList
        {
            get;
            set;
        }

        /// <summary>
        /// 公開／非公開の状態を取得します。
        /// </summary>
        [DataMember(Name = "visible")]
        public bool IsVisible
        {
            get;
            private set;
        }

        /// <summary>
        /// 会員限定・全員公開などの状態を取得します。
        /// </summary>
        [DataMember(Name = "memberonly")]
        public bool IsMemberOnly
        {
            get;
            private set;
        }

        /// <summary>
        /// 動画の公開開始日時を取得します。
        /// </summary>
        public DateTime StartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 動画の閲覧数を取得します。
        /// </summary>
        [DataMember(Name = "view_counter")]
        public int ViewCounter
        {
            get;
            set;
        }

        /// <summary>
        /// 動画のコメント数を取得します。
        /// </summary>
        [DataMember(Name = "comment_counter")]
        public int CommentCounter
        {
            get;
            set;
        }

        /// <summary>
        /// 動画のマイリスト数を取得します。
        /// </summary>
        [DataMember(Name = "mylist_counter")]
        public int MylistCounter
        {
            get;
            set;
        }
        
        /// <summary>
        /// 公開までの残り時間を文字列で取得します。
        /// </summary>
        public string LeaveTimeString
        {
            get
            {
                if (StartTime <= DateTime.Now)
                {
                    return string.Empty;
                }

                var leaveTime = StartTime - DateTime.Now;
                if (leaveTime.TotalHours < 24.0)
                {
                    return string.Format(
                        "{0}時間後に公開",
                        (int)Math.Ceiling(leaveTime.TotalHours));
                }
                else
                {
                    return string.Format(
                        "{0}日後に公開",
                        (int)Math.Ceiling(leaveTime.TotalDays));
                }
            }
        }

        /// <summary>
        /// DataContractによるシリアライズ・デシリアライズ時に使います。
        /// </summary>
        [DataMember()]
        private string tags;

        /// <summary>
        /// DataContractによるシリアライズ・デシリアライズ時に使います。
        /// </summary>
        [DataMember()]
        private string start_time;

        /// <summary>
        /// シリアライズ前に時刻のデータをオブジェクトに設定します。
        /// </summary>
        [OnSerializing()]
        private void OnSerializing(StreamingContext context)
        {
            if (StartTime != DateTime.MinValue)
            {
                //2014-07-23 20:00:00
                start_time = StartTime.ToString("yyyy-MM-dd hh:mm:ss");
            }

            tags = string.Join(" ", TagList);
        }

        /// <summary>
        /// デシリアライズ後に時刻などのデータをオブジェクトに格納します。
        /// </summary>
        [OnDeserialized()]
        private void OnDeserialized(StreamingContext context)
        {
            DateTime date = DateTime.MinValue;

            if (!string.IsNullOrEmpty(start_time))
            {
                if (!DateTime.TryParse(start_time, out date))
                {
                    // エラー時は適当な値を入れる。
                    date = DateTime.MinValue;
                }
            }

            if (tags != null)
            {
                TagList = tags.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
            else
            {
                TagList = new List<string>();
            }

            IsVisible = true;
            StartTime = date;
        }

        #region ニコニコのgetthumbinfo APIから動画情報を作成
        /// <summary>
        /// ニコニコのgetthumbinfo APIから動画情報を取得します。
        /// </summary>
        /// <remarks>
        /// 参考URL)
        /// http://dic.nicovideo.jp/a/%E3%83%8B%E3%82%B3%E3%83%8B%E3%82%B3%E5%8B%95%E7%94%BBapi
        /// </remarks>
        public static VideoData CreateFromApi(string videoStr)
        {
            if (string.IsNullOrEmpty(videoStr))
            {
                throw new ArgumentNullException("videoStr");
            }

            var videoId = VideoUtil.GetVideoId(videoStr);
            if (string.IsNullOrEmpty(videoId))
            {
                throw new NicoVideoException(
                    string.Format("{0}: 不正な動画IDです。"));
            }

            var url = string.Format(
                "http://ext.nicovideo.jp/api/getthumbinfo/{0}",
                videoId);
            var node = NicoUtil.GetXml(url, null);

            return new VideoData(videoId, node);
        }
        #endregion

        #region 動画ページから動画情報を作成
        /// <summary>
        /// 動画番号が含まれる文字列から情報を作成します。
        /// </summary>
        public static VideoData CreateFromPage(string videoStr, CookieContainer cc)
        {
            if (string.IsNullOrEmpty(videoStr))
            {
                throw new ArgumentNullException("videoStr");
            }

            if (cc == null)
            {
                throw new ArgumentNullException("cc");
            }

            var videoId = VideoUtil.GetVideoId(videoStr);
            if (string.IsNullOrEmpty(videoId))
            {
                throw new NicoVideoException(
                    string.Format("{0}: 不正な動画IDです。"));
            }

            // urlを取得します。
            var responseData = WebUtil.RequestHttp(
                NicoString.GetVideoUrl(videoId),
                null,
                cc);

            // 失敗;; エラー時はレスポンスが空になります。
            if (responseData == null)
            {
                throw new NicoVideoException(
                    "放送ページの取得に失敗しました。",
                    videoId);
            }

            var text = Encoding.UTF8.GetString(responseData);
            return CreateFromPageHtml(text);
        }

        private static readonly Regex IdRegex = new Regex(
            @"&quot;videoId&quot;:&quot;((sm|so)([0-9]+))&quot;,",
            RegexOptions.IgnoreCase);
        private static readonly Regex ThreadIdRegex = new Regex(
            @"&quot;v&quot;:&quot;([0-9]+)&quot;,",
            RegexOptions.IgnoreCase);
        private static readonly Regex TitleRegex = new Regex(
            @"<h2><span class=""videoHeaderTitle"" style=""font-size:[\d\.]+px"">(.*?)</span></h2>",
            RegexOptions.IgnoreCase);
        private static readonly Regex DescriptionRegex = new Regex(
            @"<p class=""videoDescription description"">(.*?)</p>",
            RegexOptions.IgnoreCase);
        private static readonly Regex StartTimeRegex = new Regex(
            @"&quot;postedAt&quot;:&quot;(.*?)&quot;,",
            RegexOptions.IgnoreCase);
        private static readonly Regex ViewCountRegex = new Regex(
            @"<li class=""videoStatsView"">再生:<span class=""viewCount"">\s*([\d,]+)\s*</span></li>",
            RegexOptions.IgnoreCase);
        private static readonly Regex CommentCountRegex = new Regex(
            @"<li class=""videoStatsComment"">コメント:<span class=""commentCount"">\s*([\d,]+)\s*</span></li>",
            RegexOptions.IgnoreCase);
        private static readonly Regex MylistCountRegex = new Regex(
            @"<li class=""videoStatsMylist"">マイリスト:<span class=""mylistCount"">\s*([\d,]+)\s*</span></li>",
            RegexOptions.IgnoreCase);
        private static readonly Regex TagRegex = new Regex(
            @"<a href=""/tag/[\w%#\$&\?\(\)~\.=\+\-]+"" data-playerscreenmode-change=""(\d)+"" class=""videoHeaderTagLink"">(.+?)</a>",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// 動画ページのhtmlから動画情報を作成します。
        /// </summary>
        public static VideoData CreateFromPageHtml(string pageStr)
        {
            var video = new VideoData();

            if (string.IsNullOrEmpty(pageStr))
            {
                throw new NicoVideoException(
                    "ページが空です。");
            }

            // 動画ID
            var m = IdRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "動画IDを取得できませんでした。");
            }
            video.IdString = m.Groups[1].Value;

            // スレッドID(無い場合もある)
            m = ThreadIdRegex.Match(pageStr);
            if (m.Success)
            {
                video.ThreadId = long.Parse(m.Groups[1].Value);
            }

            // 動画タイトル
            m = TitleRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "動画タイトルを取得できませんでした。",
                    video.IdString);
            }
            video.Title = m.Groups[1].Value;

            // 動画詳細
            m = DescriptionRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "動画概要を取得できませんでした。",
                    video.IdString);
            }
            video.Description = m.Groups[1].Value;

            // 投稿時間 (2015\/09\/04 11:45:00)
            m = StartTimeRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "投稿時間の取得に失敗しました。",
                    video.IdString);
            }
            video.StartTime = DateTime.ParseExact(
                m.Groups[1].Value.Replace("\\", ""),
                "yyyy/MM/dd HH:mm:ss", null);

            // 再生数
            m = ViewCountRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "再生数の取得に失敗しました。",
                    video.IdString);
            }
            video.ViewCounter = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);

            // コメント数
            m = CommentCountRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "コメント数の取得に失敗しました。",
                    video.IdString);
            }
            video.CommentCounter = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);

            // マイリスト数
            m = MylistCountRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "マイリスト数の取得に失敗しました。",
                    video.IdString);
            }
            video.MylistCounter = int.Parse(
                m.Groups[1].Value,
                NumberStyles.AllowThousands);

            // タグを収集します。
            var mc = TagRegex.Matches(pageStr);
            video.TagList = mc.OfType<Match>()
                .Where(_ => _.Success)
                .Select(_ => _.Groups[2].Value)
                .ToList();

            return video;
        }
        #endregion

        #region チャンネルツールの検索結果から動画情報を作成
        /// <summary>
        /// チャンネルツール上の探索結果から、動画情報を作成します。
        /// </summary>
        private static VideoData FromChannelToolSearchResult(string text)
        {
            var result = new VideoData();

            var regex = new Regex(
                @"<li class=""video so[^>]+>\s*<div class=""video_left"">[\S\s]+?</div>\s*</li>");
            var m = regex.Match(text);
            if (!m.Success)
            {
                return null;
            }

            // 動画ID
            var idRegex = new Regex(@"<var class=""video_id""\s*title=""動画ID"">(so\d+)</var>");
            m = idRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }
            result.IdString = m.Groups[1].Value;

            // スレッドID
            var threadRegex = new Regex(@"<var class=""thread_id""\s*title=""スレッドID"">(\d+)</var>");
            m = threadRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }
            result.ThreadId = int.Parse(m.Groups[1].Value);

            // 動画タイトル
            var titleRegex = new Regex(@"<h6 class=""video_title"" title=""([^""]*)"">");
            m = titleRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }
            result.Title = m.Groups[1].Value;

            // 動画の公開日時
            var dateRegex = new Regex(@">([\d\s\-:]+) 公開</time>");
            m = dateRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }
            result.StartTime = DateTime.Parse(m.Groups[1].Value);

            // 表示／非表示
            var hidden = "<span class=\"label hide_flag hide_flag_1\">非公開</span>";
            result.IsVisible = !text.Contains(hidden);

            // 会員限定／全員公開
            var memberOnly = "<span class=\"label permission_4\">会員限定</span>";
            result.IsMemberOnly = text.Contains(memberOnly);

            return result;
        }

        private static readonly Regex SearchResultRegex = new Regex(
            @"<li class=""video so[^>]+>\s*<div class=""video_left"">[\S\s]+?</div>\s*</li>");

        /// <summary>
        /// チャンネルツール上の探索結果から、動画情報を作成します。
        /// </summary>
        public static List<VideoData> FromChannelToolSearchResults(string text)
        {
            return SearchResultRegex.Matches(text)
                .OfType<Match>()
                .Select(_ =>
                {
                    var movie = FromChannelToolSearchResult(_.Value);
                    if (movie == null)
                    {
                        Log.Error("FromSearchResult Error");
                    }
                    return movie;
                })
                .ToList();
        }
        #endregion
    }
}
