using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Ragnarok.NicoNico.Provider
{
    using Net;
    using Utility;

    /// <summary>
    /// チャンネル動画検索時の結果を保持するクラスです。
    /// </summary>
    [Serializable()]
    public sealed class ChannelVideoData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChannelVideoData()
        {
            StartTime = DateTime.MinValue;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// 動画IDを取得します。(内容はIdStringと同じ)
        /// </summary>
        public string Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 動画URLに使われる数字のみのIDを取得します。
        /// </summary>
        public string ThreadId
        {
            get;
            set;
        }

        /// <summary>
        /// 動画タイトルを取得します。
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// サムネイルのURLを取得します。
        /// </summary>
        public string ThumbnailUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 動画の公開開始日時を取得します。
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 公開／非公開の状態を取得します。
        /// </summary>
        public bool IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// 会員限定・全員公開などの状態を取得します。
        /// </summary>
        public bool IsMemberOnly
        {
            get;
            set;
        }

        /// <summary>
        /// データの取得時刻を取得します。
        /// </summary>
        public DateTime Timestamp
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

        #region チャンネルツールの検索結果から動画情報を作成
        private static readonly Regex VideoRegex = new Regex(
            @"<li class=""video so[^>]+>\s*<div class=""video_left"">[\S\s]+?</div>\s*</li>");
        private static readonly Regex IdRegex = new Regex(
            @"<var class=""video_id""\s*title=""動画ID"">(so\d+)</var>");
        private static readonly Regex ThreadRegex = new Regex(
            @"<var class=""thread_id""\s*title=""スレッドID"">(\d+)</var>");
        private static readonly Regex TitleRegex = new Regex(
            @"<h6 class=""video_title"" title=""([^""]*)"">");
        private static readonly Regex DataRegex = new Regex(
            @">([\d\s\-:]+) 公開</time>");

        /// <summary>
        /// チャンネルツール上の探索結果から、動画情報を作成します。
        /// </summary>
        private static ChannelVideoData FromSearchResult(string text)
        {
            var result = new ChannelVideoData
            {
                Timestamp = DateTime.Now,
            };

            // 動画ID
            var m = IdRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }
            result.Id = m.Groups[1].Value;

            // スレッドID
            m = ThreadRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }
            var value = int.Parse(m.Groups[1].Value);
            result.ThreadId = value.ToString();

            // 動画タイトル
            m = TitleRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }
            result.Title = m.Groups[1].Value;

            // 動画の公開日時
            m = DataRegex.Match(text);
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

        /// <summary>
        /// チャンネルツール上の探索結果から、動画情報を作成します。
        /// </summary>
        public static IEnumerable<ChannelVideoData> FromSearchResults(string text)
        {
            return VideoRegex.Matches(text)
                .OfType<Match>()
                .Select(_ =>
                {
                    var movie = FromSearchResult(_.Value);
                    if (movie == null)
                    {
                        Log.Error("FromSearchResult Error");
                    }
                    return movie;
                });
        }
        #endregion
    }
}
