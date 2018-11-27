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
            set;
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
    }
}
