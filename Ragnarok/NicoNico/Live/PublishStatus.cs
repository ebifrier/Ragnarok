using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;

namespace Ragnarok.NicoNico.Live
{
    using Provider;

    /// <summary>
    /// 各放送に関する放送主のみが参照できる情報を保持します。
    /// </summary>
    public class PublishStatus : LiveInfomationBase
    {
        /// <summary>
        /// stream タグの内容です。
        /// </summary>
        public class StreamType
        {
            /// <summary>
            /// 放送IDを取得します。
            /// </summary>
            public string Id { get; private set; }

            /// <summary>
            /// トークンを取得します。
            /// </summary>
            public string Token { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public bool Exclude { get; private set; }

            /// <summary>
            /// 放送が所属するコミュニティの種別を取得します。
            /// </summary>
            public ProviderType ProviderType { get; private set; }

            /// <summary>
            /// 基準時刻を取得します。
            /// </summary>
            public DateTime BaseTime { get; private set; }

            /// <summary>
            /// 開場時刻(枠を取った時刻)を取得します。
            /// </summary>
            public DateTime OpenTime { get; private set; }

            /// <summary>
            /// 開演時刻(枠を開始した時刻)を取得します。
            /// </summary>
            public DateTime StartTime { get; private set; }

            /// <summary>
            /// 終了時刻(予定)を取得します。
            /// </summary>
            public DateTime EndTime { get; private set; }

            /// <summary>
            /// アンケートが許可されているかどうかを取得します。
            /// </summary>
            public bool AllowVote { get; private set; }

            /// <summary>
            /// 可変長ビットレートを使っているかどうかを取得します。
            /// </summary>
            public bool IsAdaptiveBitrateEnabled { get; private set; }

            /// <summary>
            /// 予約枠かどうかを取得します。
            /// </summary>
            public bool IsReserved { get; private set; }

            /// <summary>
            /// カテゴリを取得します。
            /// </summary>
            public string Category { get; private set; }

            /// <summary>
            /// モバイル対応かどうかを取得します。
            /// </summary>
            public bool ForMobile { get; private set; }

            internal StreamType()
            {
            }

            internal StreamType(XmlNode node)
                : this()
            {
                foreach (var childObj in node.ChildNodes)
                {
                    var child = (XmlNode)childObj;
                    var text = child.InnerText;

                    switch (child.Name)
                    {
                        case "id":
                            this.Id = text;
                            break;
                        case "token":
                            this.Token = text;
                            break;
                        case "exclude":
                            this.Exclude = StrUtil.ToBool(text, false);
                            break;
                        case "provider_type":
                            this.ProviderType = StrUtil.ToProvider(text);
                            break;
                        case "base_time":
                            this.BaseTime = StrUtil.ToDateTime(text);
                            break;
                        case "open_time":
                            this.OpenTime = StrUtil.ToDateTime(text);
                            break;
                        case "start_time":
                            this.StartTime = StrUtil.ToDateTime(text);
                            break;
                        case "end_time":
                            this.EndTime = StrUtil.ToDateTime(text);
                            break;
                        case "allow_vote":
                            this.AllowVote = StrUtil.ToBool(text, false);
                            break;
                        case "disable_adaptive_bitrate":
                            this.IsAdaptiveBitrateEnabled = !StrUtil.ToBool(text, true);
                            break;
                        case "is_reserved":
                            this.IsReserved = StrUtil.ToBool(text, false);
                            break;
                        case "category":
                            this.Category = text;
                            break;
                        case "for_mobile":
                            this.ForMobile = StrUtil.ToBool(text, false);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// user タグの内容です。
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// ニックネームを取得します。
            /// </summary>
            public string NickName { get; private set; }

            /// <summary>
            /// プレミアム会員かどうかを取得します。
            /// </summary>
            public bool IsPremium { get; private set; }

            /// <summary>
            /// ユーザーＩＤを取得します。
            /// </summary>
            public int UserId { get; private set; }

            internal UserType()
            {
            }

            internal UserType(XmlNode node)
                : this()
            {
                foreach (var childObj in node.ChildNodes)
                {
                    var child = (XmlNode)childObj;
                    var text = child.InnerText;

                    switch (child.Name)
                    {
                        case "nickname":
                            this.NickName = text;
                            break;
                        case "is_premium":
                            this.IsPremium = StrUtil.ToBool(text, true);
                            break;
                        case "user_id":
                            this.UserId = StrUtil.ToInt(text, -1);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// rtmp タグの内容です。
        /// </summary>
        public class RTMPType
        {
            /// <summary>
            /// ニックネームを取得します。
            /// </summary>
            public string Url { get; private set; }

            /// <summary>
            /// プレミアム会員かどうかを取得します。
            /// </summary>
            public string Ticket { get; private set; }

            internal RTMPType()
            {
            }

            internal RTMPType(XmlNode node)
                : this()
            {
                foreach (var childObj in node.ChildNodes)
                {
                    var child = (XmlNode)childObj;
                    var text = child.InnerText;

                    switch (child.Name)
                    {
                        case "url":
                            this.Url = text;
                            break;
                        case "ticket":
                            this.Ticket = text;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 放送情報を取得します。
        /// </summary>
        public StreamType Stream { get; private set; }

        /// <summary>
        /// ユーザー情報を取得します。
        /// </summary>
        public UserType User { get; private set; }

        /// <summary>
        /// ライブ放送情報を取得します。
        /// </summary>
        public RTMPType RTMP { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private PublishStatus(long liveId, XmlNode node)
            : base(node, "getpublishstatus", NicoString.LiveIdString(liveId))
        {
            this.Stream = new StreamType();
            this.User = new UserType();
            this.RTMP = new RTMPType();

            foreach (var childObj in RootNode.ChildNodes)
            {
                var child = (XmlNode)childObj;

                switch (child.Name)
                {
                    case "stream":
                        this.Stream = new StreamType(child);
                        break;
                    case "user":
                        this.User = new UserType(child);
                        break;
                    case "rtmp":
                        this.RTMP = new RTMPType(child);
                        break;
                }
            }
        }

        /// <summary>
        /// ニコニコ生放送の情報を取得します。
        /// </summary>
        public static PublishStatus Create(string liveStr, CookieContainer cc)
        {
            var liveId = LiveUtil.GetLiveId(liveStr);
            if (liveId < 0)
            {
                throw new NicoLiveException(NicoStatusCode.InvalidLiveId);
            }

            return Create(liveId, cc);
        }

        /// <summary>
        /// ニコニコ生放送の情報を取得します。
        /// </summary>
        public static PublishStatus Create(long liveId, CookieContainer cc)
        {
            // 生放送ＩＤから放送情報を取得します。
            var node = NicoUtil.GetXml(
                NicoString.GetPublishStatusUrl(liveId),
                cc);
            if (node == null)
            {
                throw new NicoLiveException(
                    NicoStatusCode.NetworkError,
                    NicoString.LiveIdString(liveId));
            }

            return CreateFromXml(liveId, node);
        }

        /// <summary>
        /// xmlからpublishstatusを作成します。
        /// </summary>
        public static PublishStatus CreateFromXml(long liveId, XmlNode doc)
        {
            return new PublishStatus(liveId, doc);
        }
    }
}
