using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// 来場者数などの情報を保持します。
    /// </summary>
    public class Heartbeat : LiveInfomationBase
    {
        /// <summary>
        /// 来場者数を取得します。
        /// </summary>
        public int WatchCount { get; private set; }

        /// <summary>
        /// コメント数を取得します。
        /// </summary>
        public int CommentCount { get; private set; }

        /// <summary>
        /// チケット(?)を取得します。
        /// </summary>
        public string Ticket { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Heartbeat(long liveId, XmlNode node)
            : base(node, "heartbeat", NicoString.LiveIdString(liveId))
        {
            foreach (var childObj in RootNode.ChildNodes)
            {
                var child = (XmlNode)childObj;
                var value = child.InnerText;

                switch (child.Name)
                {
                    case "watchCount":
                        this.WatchCount = StrUtil.ToInt(value, -1);
                        break;
                    case "commentCount":
                        this.CommentCount = StrUtil.ToInt(value, -1);
                        break;
                    case "ticket":
                        this.Ticket = value;
                        break;
                }
            }
        }

        /// <summary>
        /// ニコニコ生放送の情報を取得します。
        /// </summary>
        public static Heartbeat Create(string liveStr, CookieContainer cc)
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
        public static Heartbeat Create(long liveId, CookieContainer cc)
        {
            // 生放送ＩＤから放送情報を取得します。
            var node = NicoUtil.GetXml(
                NicoString.GetHeartbeatUrl(liveId),
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
        /// xmlからheartbeatを作成します。
        /// </summary>
        public static Heartbeat CreateFromXml(long liveId, XmlNode doc)
        {
            return new Heartbeat(liveId, doc);
        }
    }
}
