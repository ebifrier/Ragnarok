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
    /// 動画検索時の結果を保持するクラスです。
    /// </summary>
    [Serializable()]
    [DataContract()]
    public sealed class VideoData : XmlInfomationBase
    {
        private List<string> taglist = new List<string>();
        private string tags;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VideoData()
        {
            TagList = new List<string>();
            StartTime = DateTime.MinValue;
            ViewCounter = -1;
            CommentCounter = -1;
            MylistCounter = -1;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private VideoData(string videoId, XmlNode node)
            : base(node, "nicovideo_thumb_response", videoId)
        {
            TagList = new List<string>();
            StartTime = DateTime.MinValue;
            ViewCounter = -1;
            CommentCounter = -1;
            MylistCounter = -1;
            IsVisible = true;
            Timestamp = Time;

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
                    case "thumbnail_url":
                        this.ThumbnailUrl = text;
                        break;
                    case "first_retrieve":
                        this.StartTime = DateTime.Parse(text);
                        break;
                    case "length":
                        var values = text.Split(':');
                        this.Length = new TimeSpan(
                            0,
                            int.Parse(values[0]),
                            int.Parse(values[1]));
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
        public override string IdString
        {
            get;
            protected set;
        }

        /// <summary>
        /// 動画IDを取得します。(内容はIdStringと同じ)
        /// </summary>
        [DataMember(Name = "cmsid")]
        public string Id
        {
            get { return IdString; }
            private set { IdString = value; }
        }

        /// <summary>
        /// 動画URLに使われる数字のみのIDを取得します。
        /// </summary>
        [DataMember(Name = "thread_id")]
        public string ThreadId
        {
            get;
            set;
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
            get { return this.taglist; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.tags = string.Join(" ", value);
                this.taglist = value;
            }
        }

        /// <summary>
        /// タグを空白で区切った文字列を取得または設定します。
        /// </summary>
        [DataMember(Name = "tags")]
        public string Tags
        {
            get { return this.tags; }
            set
            {
                if (value != null)
                {
                    TagList = value.Split(new char[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }
                else
                {
                    TagList = new List<string>();
                }

                this.tags = value;
            }
        }

        /// <summary>
        /// サムネイルのURLを取得します。
        /// </summary>
        [DataMember(Name = "thumbnail_url")]
        public string ThumbnailUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 動画の公開開始日時を取得します。
        /// </summary>
        [DataMember(Name = "start_time")]
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 動画尺を取得します。
        /// </summary>
        public TimeSpan Length
        {
            get;
            set;
        }

        /// <summary>
        /// 動画尺の秒数を取得します。
        /// </summary>
        [DataMember(Name = "length_seconds")]
        public int LengthSeconds
        {
            get { return (int)Math.Floor(Length.TotalSeconds); }
            set { Length = TimeSpan.FromSeconds(value); }
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
        /// 公開／非公開の状態を取得します。
        /// </summary>
        [DataMember()]
        public bool? IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// 会員限定・全員公開などの状態を取得します。
        /// </summary>
        [DataMember()]
        public bool? IsMemberOnly
        {
            get;
            set;
        }

        /// <summary>
        /// データの取得時刻を取得します。
        /// </summary>
        [DataMember()]
        public DateTime Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// 再生数などの情報をまとめて取得または設定します。
        /// </summary>
        public ViewCountData ViewData
        {
            get
            {
                return new ViewCountData(Id, ViewCounter, CommentCounter, MylistCounter);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                ViewCounter = value.ViewCounter;
                CommentCounter = value.CommentCounter;
                MylistCounter = value.MylistCounter;
            }
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
        /// デシリアライズ後に時刻などのデータをオブジェクトに格納します。
        /// </summary>
        [OnDeserialized()]
        private void OnDeserialized(StreamingContext context)
        {
            IsVisible = true;
            Timestamp = DateTime.Now;
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
        private static readonly Regex ThumbnailRegex = new Regex(
           @"&quot;thumbnail&quot;:&quot;(.*?)&quot;,",
           RegexOptions.IgnoreCase);
        private static readonly Regex StartTimeRegex = new Regex(
            @"&quot;postedAt&quot;:&quot;(.*?)&quot;,",
            RegexOptions.IgnoreCase);
        private static readonly Regex LengthRegex = new Regex(
            @"&quot;length&quot;:([0-9]+?),",
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
        private static readonly Regex IsPublicRegex = new Regex(
            @"&quot;is_public&quot;:(.+?),",
            RegexOptions.IgnoreCase);
        private static readonly Regex ThreadPublicRegex = new Regex(
            @"&quot;threadPublic&quot;:&quot;([0-9]+)&quot;,",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// 動画ページのhtmlから動画情報を作成します。
        /// </summary>
        public static VideoData CreateFromPageHtml(string pageStr)
        {
            var video = new VideoData
            {
                Timestamp = DateTime.Now
            };

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
                var value = long.Parse(m.Groups[1].Value);
                video.ThreadId = value.ToString();
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

            // サムネイルURL
            m = ThumbnailRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "サムネイルURLの取得に失敗しました。",
                    video.IdString);
            }
            video.ThumbnailUrl = m.Groups[1].Value.Replace("\\", "");

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
            
            // 動画尺 (2290)
            m = LengthRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "動画尺の取得に失敗しました。",
                    video.IdString);
            }
            video.LengthSeconds = int.Parse(m.Groups[1].Value);

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

            // 表示・非表示
            m = IsPublicRegex.Match(pageStr);
            if (!m.Success)
            {
                throw new NicoVideoException(
                    "表示状態の取得に失敗しました。",
                    video.IdString);
            }
            video.IsVisible = (m.Groups[1].Value == "true"); 

            // メンバー限定 (チャンネル動画限定)
            m = ThreadPublicRegex.Match(pageStr);
            video.IsMemberOnly = (m.Success ?
                int.Parse(m.Groups[1].Value) != 1 :
                false);

            return video;
        }
        #endregion
    }
}
