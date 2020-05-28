using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Runtime.Serialization;

using Ragnarok;
using Ragnarok.Net;

namespace Ragnarok.NicoNico
{
    /// <summary>
    /// コメントの種類です。
    /// </summary>
    [DataContract()]
    public enum CommentType
    {
        /// <summary>
        /// 一般会員によるコメント
        /// </summary>
        Normal = 0,
        /// <summary>
        /// プレミアム会員によるコメント
        /// </summary>
        Premium = 1,
        /// <summary>
        /// アラートコメント
        /// </summary>
        Alert = 2,
        /// <summary>
        /// 放送主によるコメント
        /// </summary>
        Owner = 3,
        /// <summary>
        /// 運営によるコメント
        /// </summary>
        Management1 = 4,
        /// <summary>
        /// 運営によるコメント
        /// </summary>
        Management2 = 5,
        /// <summary>
        /// 運営によるコメント
        /// </summary>
        Management3 = 6,
    }

    /// <summary>
    /// コメントの色です。
    /// </summary>
    [DataContract()]
    public enum CommentColor
    {
        #region 一般会員・プレミアム会員共通色
        /// <summary>
        /// デフォルト色(指定無し)
        /// </summary>
        Default,
        /// <summary>
        /// 白
        /// </summary>
        White,
        /// <summary>
        /// 赤
        /// </summary>
        Red,
        /// <summary>
        /// ピンク
        /// </summary>
        Pink,
        /// <summary>
        /// オレンジ
        /// </summary>
        Orange,
        /// <summary>
        /// 黄
        /// </summary>
        Yellow,
        /// <summary>
        /// 緑
        /// </summary>
        Green,
        /// <summary>
        /// シアン
        /// </summary>
        Cyan,
        /// <summary>
        /// 青
        /// </summary>
        Blue,
        /// <summary>
        /// 紫
        /// </summary>
        Purple,
        /// <summary>
        /// 黒
        /// </summary>
        Black,
        #endregion

        #region プレミアム会員専用色
        /// <summary>
        /// 白２
        /// </summary>
        White2,
        /// <summary>
        /// 赤２
        /// </summary>
        Red2,
        /// <summary>
        /// オレンジ２
        /// </summary>
        Orange2,
        /// <summary>
        /// 黄２
        /// </summary>
        Yellow2,
        /// <summary>
        /// 緑２
        /// </summary>
        Green2,
        /// <summary>
        /// 青２
        /// </summary>
        Blue2,
        /// <summary>
        /// 紫２
        /// </summary>
        Purple2,
        #endregion

        #region プレミアム会員専用色（ニコ動のみ）
        /// <summary>
        /// シアン２
        /// </summary>
        Cyan2,
        /// <summary>
        /// ピンク２
        /// </summary>
        Pink2,
        /// <summary>
        /// 黒２
        /// </summary>
        Black2,
        #endregion

        /// <summary>
        /// カスタム色
        /// </summary>
        /// <remarks>
        /// プレミアム会員は任意の色が指定できます
        /// </remarks>
        Custom,
    }

    /// <summary>
    /// コメントサイズです。
    /// </summary>
    [DataContract()]
    public enum CommentSize
    {
        /// <summary>
        /// デフォルトサイズ(指定無し)
        /// </summary>
        Default,
        /// <summary>
        /// 大きい
        /// </summary>
        Big,
        /// <summary>
        /// 小さい
        /// </summary>
        Small,
        /// <summary>
        /// 普通
        /// </summary>
        Medium
    }

    /// <summary>
    /// コメントの流れる位置です。
    /// </summary>
    [DataContract()]
    public enum CommentPosition
    {
        /// <summary>
        /// デフォルト位置(指定無し)
        /// </summary>
        Default,
        /// <summary>
        /// 上です。
        /// </summary>
        Ue,
        /// <summary>
        /// 下です。
        /// </summary>
        Shita,
        /// <summary>
        /// 真ん中を流れます。
        /// </summary>
        Naka,
    }

    /// <summary>
    /// コメントクラスです。
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// コメントを表現したXMLを取得または設定します。
        /// </summary>
        public string OriginalXml { get; set; }

        /// <summary>
        /// スレッド番号を取得または設定します。
        /// </summary>
        public int Thread { get; set; }

        /// <summary>
        /// レス番号を取得または設定します。
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 放送開始からコメント投稿までの経過時刻を10ms単位で取得または設定します。
        /// </summary>
        public int VPos { get; set; }

        /*// <summary>
        /// 放送開始からコメント投稿までの経過時刻を取得または設定します。
        /// </summary>
        public TimeSpan ElapseTime
        {
            get { return TimeSpan.FromMilliseconds(VPos * 10); }
            set { VPos = value.Milliseconds / 10; }
        }*/

        /// <summary>
        /// コメント投稿時刻を取得または設定します。
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// コメント投稿時刻のμ秒を取得または設定します。
        /// </summary>
        public double DateMicroSeconds { get; set; }

        /// <summary>
        /// ユーザーＩＤを取得または設定します。
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// コメントの情報を取得または設定します。
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// コメントの種類を取得または設定します。
        /// </summary>
        public CommentType CommentType { get; set; }

        /// <summary>
        /// 自分で投稿したコメントかどうかを取得または設定します。
        /// </summary>
        public bool IsYourpost { get; set; }

        /// <summary>
        /// コメント内容を取得または設定します。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// NGスコアを取得または設定します。
        /// </summary>
        public int NGScore { get; set; }

        /// <summary>
        /// origin属性を取得または設定します。
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// ユーザーによって投稿されたコメントかどうかを取得します。
        /// </summary>
        public bool IsUserComment
        {
            get
            {
                return (
                    this.CommentType == CommentType.Normal ||
                    this.CommentType == CommentType.Premium);
            }
        }

        /// <summary>
        /// 放送主によって投稿されたコメントかどうかを取得します。
        /// </summary>
        public bool IsOwnerComment
        {
            get
            {
                return (this.CommentType == CommentType.Owner);
            }
        }

        /// <summary>
        /// 運営コメントかどうかを取得します。
        /// </summary>
        public bool IsManagementComment
        {
            get
            {
                return (
                    this.CommentType == CommentType.Management1 ||
                    this.CommentType == CommentType.Management2 ||
                    this.CommentType == CommentType.Management3);
            }
        }

        /// <summary>
        /// 匿名ユーザーかどうかを取得します。
        /// </summary>
        public bool IsAnonymous
        {
            get { return CommentParser.IsAnonymous(this.Mail); }
        }

        /// <summary>
        /// コメント色を取得します。
        /// </summary>
        public CommentColor Color
        {
            get { return CommentParser.ParseColor(this.Mail); }
        }

        /// <summary>
        /// コメントサイズを取得します。
        /// </summary>
        public CommentSize Size
        {
            get { return CommentParser.ParseSize(this.Mail); }
        }

        /// <summary>
        /// コメント位置を取得します。
        /// </summary>
        public CommentPosition Position
        {
            get { return CommentParser.ParsePosition(this.Mail); }
        }

        /// <summary>
        /// リレーコメントかどうかを取得します。
        /// </summary>
        public bool IsRelayComment
        {
            get { return (Origin == "C"); }
        }

        /// <summary>
        /// 送信用のコメントを作成します。
        /// </summary>
        public Comment(string text, string mail)
        {
            Text = text;
            Mail = mail;
        }

        /// <summary>
        /// 整数型をパースします。
        /// </summary>
        private static int IntTryParse(XmlAttribute attr, int defaultValue)
        {
            if (attr == null)
            {
                return defaultValue;
            }

            int value = -1;
            if (!int.TryParse(attr.Value, out value))
            {
                return defaultValue;
            }

            return value;
        }

        private static CommentType ParseCommentType(XmlAttribute attr)
        {
            if (attr == null)
            {
                return CommentType.Normal;
            }

            // コメントタイプを取得します。
            var attrValue = int.Parse(attr.Value);
            foreach (var enumValue in EnumUtil.GetValues<CommentType>())
            {
                if (attrValue == (int)enumValue)
                {
                    return enumValue;
                }
            }

            return CommentType.Normal;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Comment(XmlNode node)
        {
            OriginalXml = node.OuterXml;

            XmlAttribute attr = node.Attributes["thread"];
            Thread = IntTryParse(attr, -1);

            // noは無いことがあります。
            attr = node.Attributes["no"];
            No = IntTryParse(attr, -1);

            attr = node.Attributes["vpos"];
            VPos = IntTryParse(attr, -1);

            attr = node.Attributes["date"];
            Date = (
                attr == null ?
                DateTime.MinValue :
                Utility.TimeUtil.UnixTimeToDateTime(attr.Value));

            attr = node.Attributes["date_usec"];
            if (attr != null)
            {
                var value = IntTryParse(attr, 0);
                Date = Date.AddMilliseconds(value / 1000);
                DateMicroSeconds = value % 1000;
            }

            attr = node.Attributes["user_id"];
            UserId = attr.Value;

            attr = node.Attributes["mail"];
            Mail = (attr == null ? string.Empty : attr.Value);

            // 自分で投稿したコメントなら'1'、そうでないなら'0'か
            // もしくは属性自体が存在しない。
            attr = node.Attributes["yourpost"];
            IsYourpost = (IntTryParse(attr, 0) > 0);

            attr = node.Attributes["premium"];
            CommentType = ParseCommentType(attr);

            attr = node.Attributes["score"];
            NGScore = IntTryParse(attr, 0);

            attr = node.Attributes["origin"];
            Origin = (attr == null ? string.Empty : attr.Value);

            // 意味あるのか？
            Text = node.InnerText ?? "";
        }
    }
}
