using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Ragnarok;
using Ragnarok.Net;

// コメント不足への警告を無くします。
#pragma warning disable 1591

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// getplayerstatusを扱うクラスです。
    /// </summary>
    public class PlayerStatus : LiveInfomationBase
    {
        /// <summary>
        /// 放送情報です。
        /// </summary>
        public class StreamType
        {
            /// <summary>
            /// 放送ＩＤを取得します。
            /// </summary>
            public long Id { get; private set; }

            /// <summary>
            /// 放送ＩＤを取得します。
            /// </summary>
            public string IdString
            {
                get { return string.Format("lv{0}", Id); }
            }

            /// <summary>
            /// 放送タイトル(長いと省略)を取得します。
            /// </summary>
            public string Title { get; private set; }

            /// <summary>
            /// 放送概要(長いと省略)を取得します。
            /// </summary>
            public string Description { get; private set; }

            /// <summary>
            /// 総コメント数を取得します。
            /// </summary>
            public int CommentCount { get; private set; }

            /// <summary>
            /// 総来場者数を取得します。
            /// </summary>
            public int WatchCount { get; private set; }

            /// <summary>
            /// 男女コメントモードか取得します。
            /// </summary>
            public bool DanjoCommentMode { get; private set; }

            /// <summary>
            /// ネットデュエット可能か取得します。
            /// </summary>
            public bool AllowNetduetto { get; private set; }

            /// <summary>
            /// ニコニコ電話が有効か取得します。
            /// </summary>
            public bool IsNicoDen { get; private set; }

            /// <summary>
            /// ニコニコ遊園地タグがついているか取得します。
            /// </summary>
            public bool IsPark { get; private set; }

            public string RelayComment { get; private set; }

            public string NdToken { get; private set; }

            /// <summary>
            /// 放送が見れなかったときに映される動画のURLを取得します。
            /// </summary>
            public string BourbonUrl { get; private set; }

            /// <summary>
            /// 満員で見れなかったときに映される動画のURLを取得します。
            /// </summary>
            public string FullVideo { get; private set; }

            public string AfterVideo { get; private set; }

            public string BeforeVideo { get; private set; }

            /// <summary>
            /// 放送から追い出されたときに映される動画のURLを取得します。
            /// </summary>
            public string KickoutVideo { get; private set; }

            public string HeaderComment { get; private set; }

            public string FooterComment { get; private set; }

            public string PluginDelay { get; private set; }

            public string PluginUrl { get; private set; }

            /// <summary>
            /// 放送が所属するコミュニティの種別を取得します。
            /// </summary>
            public Live.ProviderType ProviderType { get; private set; }

            /// <summary>
            /// 放送が所属するコミュニティを取得します。
            /// </summary>
            public string DefaultCommunity { get; private set; }

            /// <summary>
            /// タイムシフトかどうかを取得します。
            /// </summary>
            public bool IsArchive { get; private set; }

            public bool IsDjStream { get; private set; }

            /// <summary>
            /// twitterのハッシュタグを取得します。
            /// </summary>
            public string TwitterTag { get; private set; }

            /// <summary>
            /// 放送主かどうかを取得します。
            /// </summary>
            public bool IsOwner { get; private set; }

            /// <summary>
            /// 放送主のＩＤを取得します。
            /// </summary>
            public int OwnerId { get; private set; }

            /// <summary>
            /// 放送主の名前を取得します。
            /// </summary>
            public string OwnerName { get; private set; }

            /// <summary>
            /// 予約枠か取得します。
            /// </summary>
            public bool IsReserved { get; private set; }

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

            public bool IsIchibaNoticeEnable { get; private set; }

            /// <summary>
            /// コメントがロックされているかどうかを取得します。
            /// </summary>
            public bool IsCommentLock { get; private set; }

            /// <summary>
            /// コメントが裏流し状態かどうかを取得します。
            /// </summary>
            public bool IsBackgroundComment { get; private set; }

            //public string Telop { get; private set; }

            /// <summary>
            /// 再生中のコンテンツリストを取得します。
            /// </summary>
            public List<ContentType> ContentList { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public PressType Press { get; private set; }

            internal StreamType()
            {
                this.ContentList = new List<ContentType>();
                this.Press = new PressType();
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
                            this.Id = int.Parse(text.Substring(2));
                            break;
                        case "title":
                            this.Title = text;
                            break;
                        case "description":
                            this.Description = text;
                            break;
                        case "comment_count":
                            this.CommentCount = StrUtil.ToInt(text, 0);
                            break;
                        case "watch_count":
                            this.WatchCount = StrUtil.ToInt(text, 0);
                            break;
                        case "danjo_comment_mode":
                            this.DanjoCommentMode = StrUtil.ToBool(text, false);
                            break;
                        case "nicoden":
                            this.IsNicoDen = StrUtil.ToBool(text, false);
                            break;
                        case "allow_netduetto":
                            this.AllowNetduetto = StrUtil.ToBool(text, false);
                            break;
                        case "relay_comment":
                            this.RelayComment = text;
                            break;
                        case "park":
                            this.IsPark = StrUtil.ToBool(text, false);
                            break;
                        case "nd_token":
                            this.NdToken = text;
                            break;
                        case "bourbon_url":
                            this.BourbonUrl = text;
                            break;
                        case "full_video":
                            this.FullVideo = text;
                            break;
                        case "after_video":
                            this.AfterVideo = text;
                            break;
                        case "before_video":
                            this.BeforeVideo = text;
                            break;
                        case "kickout_video":
                            this.KickoutVideo = text;
                            break;
                        case "header_comment":
                            this.HeaderComment = text;
                            break;
                        case "footer_comment":
                            this.FooterComment = text;
                            break;
                        case "plugin_delay":
                            this.PluginDelay = text;
                            break;
                        case "plugin_url":
                            this.PluginUrl = text;
                            break;
                        case "provider_type":
                            this.ProviderType = StrUtil.ToProvider(text);
                            break;
                        case "default_community":
                            this.DefaultCommunity = text;
                            break;
                        case "archive":
                            this.IsArchive = StrUtil.ToBool(text, false);
                            break;
                        case "is_dj_stream":
                            this.IsDjStream = StrUtil.ToBool(text, false);
                            break;
                        case "twitter_tag":
                            this.TwitterTag = text;
                            break;
                        case "is_owner":
                            this.IsOwner = StrUtil.ToBool(text, false);
                            break;
                        case "owner_id":
                            this.OwnerId = StrUtil.ToInt(text, -1);
                            break;
                        case "owner_name":
                            this.OwnerName = text;
                            break;
                        case "is_reserved":
                            this.IsReserved = StrUtil.ToBool(text, false);
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
                        case "ichiba_notice_enable":
                            this.IsIchibaNoticeEnable = StrUtil.ToBool(text, false);
                            break;
                        case "comment_lock":
                            this.IsCommentLock = StrUtil.ToBool(text, false);
                            break;
                        case "background_comment":
                            this.IsBackgroundComment = StrUtil.ToBool(text, false);
                            break;
                        case "contents_list":
                            foreach (var childObj2 in child.ChildNodes)
                            {
                                var child2 = (XmlNode)childObj2;

                                if (child2.Name == "contents")
                                {
                                    var content = new ContentType(child2);
                                    this.ContentList.Add(content);
                                }
                            }
                            break;
                        case "press":
                            this.Press = new PressType(child);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 再生しているコンテンツ情報です。
        /// </summary>
        public class ContentType
        {
            /// <summary>
            /// ＩＤ(種別)を取得します。
            /// </summary>
            public string Id { get; private set; }

            /// <summary>
            /// 映像出力が有効か取得します。
            /// </summary>
            public bool EnableVideo { get; private set; }

            /// <summary>
            /// 音声出力が有効か取得します。
            /// </summary>
            public bool EnableAudio { get; private set; }

            /// <summary>
            /// コンテンツの開始時刻を取得します。
            /// </summary>
            public DateTime StartTime { get; private set; }

            /// <summary>
            /// コンテンツの再生時間を取得します。
            /// </summary>
            public TimeSpan Duration { get; private set; }

            /// <summary>
            /// コンテンツの中身を取得します。
            /// </summary>
            public string Content { get; private set; }

            internal ContentType(XmlNode node)
            {
                foreach (var attrObj in node.Attributes)
                {
                    var attr = (XmlAttribute)attrObj;
                    var text = attr.Value;

                    switch (attr.Name)
                    {
                        case "id":
                            this.Id = attr.Value;
                            break;
                        case "disableVideo":
                            this.EnableVideo = !StrUtil.ToBool(text, true);
                            break;
                        case "disableAudio":
                            this.EnableAudio = !StrUtil.ToBool(text, true);
                            break;
                        case "start_time":
                            this.StartTime = StrUtil.ToDateTime(text);
                            break;
                        case "duration":
                            this.Duration = TimeSpan.FromSeconds(
                                StrUtil.ToInt(text, 0));
                            break;
                    }
                }

                this.Content = node.InnerText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class PressType
        {
            /// <summary>
            /// 表示行数を取得します。
            /// </summary>
            public int DisplayLines { get; private set; }

            /// <summary>
            /// 表示時間を取得します。
            /// </summary>
            public int DisplayTime { get; private set; }

            internal PressType()
            {
            }

            internal PressType(XmlNode node)
            {
                this.DisplayLines = -1;
                this.DisplayTime = -1;
            }
        }

        /// <summary>
        /// ユーザー情報です。
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// 所属する部屋名を取得します。
            /// </summary>
            public string RoomLabel { get; private set; }

            /// <summary>
            /// シートNoを取得します。
            /// </summary>
            public int RoomSeetNo { get; private set; }

            /// <summary>
            /// ユーザーＩＤを取得します。
            /// </summary>
            public int UserId { get; private set; }

            /// <summary>
            /// ニックネームを取得します。
            /// </summary>
            public string NickName { get; private set; }

            /// <summary>
            /// プレミアム会員か取得します。
            /// </summary>
            public bool IsPremium { get; private set; }

            /// <summary>
            /// ユーザーの年齢を取得します。
            /// </summary>
            public int UserAge { get; private set; }

            /// <summary>
            /// ユーザーの性別を取得します。
            /// </summary>
            public Gender UserGender { get; private set; }

            /// <summary>
            /// ユーザーの都道府県を取得します。
            /// </summary>
            public string UserPrefecture { get; private set; }

            public string HKey { get; private set; }

            /// <summary>
            /// コミュティに参加しているかどうかを取得します。
            /// </summary>
            public bool IsJoin { get; private set; }

            public string ImmuComment { get; private set; }

            public string CanBroadcast { get; private set; }

            public string CanForceLogin { get; private set; }

            /// <summary>
            /// ユーザーのtwitter情報を取得します。
            /// </summary>
            public TwitterInfoType TwitterInfo { get; private set; }

            internal UserType()
            {
                this.TwitterInfo = new TwitterInfoType();
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
                        case "room_label":
                            this.RoomLabel = text;
                            break;
                        case "room_seetno":
                            this.RoomSeetNo = StrUtil.ToInt(text, 0);
                            break;
                        case "userAge":
                            this.UserAge = StrUtil.ToInt(text, 0);
                            break;
                        case "userSex":
                            this.UserGender = StrUtil.ToGender(text);
                            break;
                        case "userPrefecture":
                            this.UserPrefecture = text;
                            break;
                        case "nickname":
                            this.NickName = text;
                            break;
                        case "is_premium":
                            this.IsPremium = StrUtil.ToBool(text, false);
                            break;
                        case "user_id":
                            this.UserId = int.Parse(text);
                            break;
                        case "hkey":
                            this.HKey = text;
                            break;
                        case "is_join":
                            this.IsJoin = StrUtil.ToBool(text, false);
                            break;
                        case "immu_comment":
                            this.ImmuComment = text;
                            break;
                        case "can_broadcast":
                            this.CanBroadcast = text;
                            break;
                        case "can_forcelogin":
                            this.CanForceLogin = text;
                            break;
                        case "twitter_info":
                            this.TwitterInfo = new TwitterInfoType(child);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// ユーザーのtwitter情報です。
        /// </summary>
        public class TwitterInfoType
        {
            /// <summary>
            /// ユーザーのtwitterへのログイン状態を取得します。
            /// </summary>
            public string Status { get; private set; }

            public string AfterAuth { get; private set; }

            /// <summary>
            /// 表示名を取得します。
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// フォロワー数を取得します。
            /// </summary>
            public int FollowersCount { get; private set; }

            /// <summary>
            /// Vipかどうか取得します。
            /// </summary>
            public bool IsVip { get; private set; }

            /// <summary>
            /// 画像へのURLを取得します。
            /// </summary>
            public string ImageUrl { get; private set; }

            /// <summary>
            /// tweetするときに必要なトークンを取得します。
            /// </summary>
            public string TweetToken { get; private set; }

            internal TwitterInfoType()
            {
            }

            internal TwitterInfoType(XmlNode node)
            {
                foreach (var childObj in node.ChildNodes)
                {
                    var child = (XmlNode)childObj;
                    var text = child.InnerText;

                    switch (child.Name)
                    {
                        case "status":
                            this.Status = text;
                            break;
                        case "after_auth":
                            this.AfterAuth = text;
                            break;
                        case "screen_name":
                            this.Name = text;
                            break;
                        case "followers_count":
                            this.FollowersCount = StrUtil.ToInt(text, 0);
                            break;
                        case "is_vip":
                            this.IsVip = StrUtil.ToBool(text, false);
                            break;
                        case "profile_image_url":
                            this.ImageUrl = text;
                            break;
                        case "tweet_token":
                            this.TweetToken = text;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// RTMP(Real Time Messaging Protocol)の情報です。
        /// </summary>
        public class RTMPType
        {
            public bool IsFms { get; private set; }

            /// <summary>
            /// ポート番号を取得します。
            /// </summary>
            public int Port { get; private set; }

            /// <summary>
            /// URLを取得します。
            /// </summary>
            public string Url { get; private set; }

            /// <summary>
            /// チケットを取得します。
            /// </summary>
            public string Ticket { get; private set; }

            internal RTMPType()
            {
            }

            internal RTMPType(XmlNode node)
            {
                foreach (var attrObj in node.Attributes)
                {
                    var attr = (XmlAttribute)attrObj;
                    var text = attr.Value;

                    switch (attr.Name)
                    {
                        case "is_fms":
                            this.IsFms = StrUtil.ToBool(text, false);
                            break;
                        case "rtmpt_port":
                            this.Port = StrUtil.ToInt(text, 0);
                            break;
                    }
                }

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
        /// メッセージサーバーの情報です。
        /// </summary>
        public class MSType
        {
            /// <summary>
            /// アドレスを取得します。
            /// </summary>
            public string Address { get; private set; }

            /// <summary>
            /// ポート番号を取得します。
            /// </summary>
            public int Port { get; private set; }

            /// <summary>
            /// スレッド番号を取得します。
            /// </summary>
            public int Thread { get; private set; }

            internal MSType()
            {
            }

            internal MSType(XmlNode node)
            {
                foreach (var childObj in node.ChildNodes)
                {
                    var child = (XmlNode)childObj;
                    var text = child.InnerText;

                    switch (child.Name)
                    {
                        case "addr":
                            this.Address = text;
                            break;
                        case "port":
                            this.Port = StrUtil.ToInt(text, 0);
                            break;
                        case "thread":
                            this.Thread = StrUtil.ToInt(text, 0);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// twitter情報です。
        /// </summary>
        public class TwitterType
        {
            /// <summary>
            /// twitterによるコメント投稿が可能かどうかです。
            /// </summary>
            public bool IsEnabled { get; private set; }

            public int VipModeCount { get; private set; }

            /// <summary>
            /// 生放送APIへのURLを取得します。
            /// </summary>
            public string LiveApiUrl { get; private set; }

            internal TwitterType()
            {
            }

            internal TwitterType(XmlNode node)
            {
                foreach (var childObj in node.ChildNodes)
                {
                    var child = (XmlNode)childObj;
                    var text = child.InnerText;

                    switch (child.Name)
                    {
                        case "live_enabled":
                            this.IsEnabled = StrUtil.ToBool(text, false);
                            break;
                        case "vip_mode_count":
                            this.VipModeCount = StrUtil.ToInt(text, -1);
                            break;
                        case "live_api_url":
                            this.LiveApiUrl = text;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private PlayerStatus(long liveId, XmlNode node)
            : base(node, "getplayerstatus", LiveUtil.LiveIdString(liveId))
        {
            this.Stream = new StreamType();
            this.User = new UserType();
            this.RTMP = new RTMPType();
            this.MS = new MSType();
            this.Twitter = new TwitterType();

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
                    case "ms":
                        this.MS = new MSType(child);
                        break;
                    case "twitter":
                        this.Twitter = new TwitterType(child);
                        break;
                }
            }
        }

        /// <summary>
        /// stream情報を取得します。
        /// </summary>
        public StreamType Stream { get; private set; }

        /// <summary>
        /// user情報を取得します。
        /// </summary>
        public UserType User { get; private set; }

        /// <summary>
        /// rtmp情報を取得します。
        /// </summary>
        public RTMPType RTMP { get; private set; }

        /// <summary>
        /// MessageServer情報を取得します。
        /// </summary>
        public MSType MS { get; private set; }

        /// <summary>
        /// twitter情報を取得します。
        /// </summary>
        public TwitterType Twitter { get; private set; }

        /// <summary>
        /// ニコニコ生放送の情報を取得します。
        /// </summary>
        public static PlayerStatus Create(string liveStr, CookieContainer cc)
        {
            var liveId = LiveUtil.GetLiveId(liveStr);
            if (liveId < 0)
            {
                throw new NicoLiveException(LiveStatusCode.InvalidLiveId);
            }

            return Create(liveId, cc);
        }

        /// <summary>
        /// ニコニコ生放送の情報を取得します。
        /// </summary>
        public static PlayerStatus Create(long liveId, CookieContainer cc)
        {
            // 生放送ＩＤから放送情報を取得します。
            var node = LiveUtil.GetXml(
                NicoString.GetPlayerStatusUrl(liveId),
                cc);
            if (node == null)
            {
                throw new NicoLiveException(
                    LiveStatusCode.NetworkError,
                    LiveUtil.LiveIdString(liveId));
            }

            return CreateFromXml(liveId, node);
        }

        /// <summary>
        /// xmlデータからplayerstatusオブジェクトを作成します。
        /// </summary>
        public static PlayerStatus CreateFromXml(long liveId, XmlNode doc)
        {
            return new PlayerStatus(liveId, doc);
        }
    }
}
