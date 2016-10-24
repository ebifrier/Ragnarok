using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Ragnarok.NicoNico.Video
{
    /// <summary>
    /// 動画の再生数などのデータを保持します。
    /// </summary>
    [Serializable()]
    [DataContract()]
    public sealed class ViewCountData
    {
        private DateTime? timestamp;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewCountData()
        {
            ViewCounter = -1;
            CommentCounter = -1;
            MylistCounter = -1;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewCountData(string id, int viewCounter, int commentCounter, int mylistCounter)
        {
            Id = id;
            ViewCounter = viewCounter;
            CommentCounter = commentCounter;
            MylistCounter = mylistCounter;
        }

        /// <summary>
        /// 対象となる動画のIDを取得または設定します。
        /// </summary>
        [DataMember()]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// サーバーからデータを取得した時刻を取得または設定します。
        /// </summary>
        [DataMember(Name = "Timestamp")]
        public string TimestampStr
        {
            get;
            set;
        }

        private static readonly Regex TimestampRegex = new Regex(
            @"(\d\d\d\d)(\d\d)(\d\d)(\d\d)");

        /// <summary>
        /// サーバーからデータを取得した時刻を取得または設定します。
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                if (this.timestamp != null)
                {
                    return this.timestamp.Value;
                }

                var m = TimestampRegex.Match(TimestampStr);
                if (m.Success)
                {
                    this.timestamp = new DateTime(
                        int.Parse(m.Groups[1].Value),
                        int.Parse(m.Groups[2].Value),
                        int.Parse(m.Groups[3].Value),
                        int.Parse(m.Groups[4].Value),
                        0, 0);

                    return this.timestamp.Value;
                }

                return DateTime.MinValue;
            }
            set
            {
                this.timestamp = value;
                TimestampStr = value.ToString("yyyyMMddHH");
            }
        }

        /// <summary>
        /// 再生数を取得または設定します。
        /// </summary>
        [DataMember()]
        public int ViewCounter
        {
            get;
            set;
        }

        /// <summary>
        /// コメント数を取得または設定します。
        /// </summary>
        [DataMember()]
        public int CommentCounter
        {
            get;
            set;
        }

        /// <summary>
        /// マイリスト数を取得または設定します。
        /// </summary>
        [DataMember()]
        public int MylistCounter
        {
            get;
            set;
        }
    }
}
