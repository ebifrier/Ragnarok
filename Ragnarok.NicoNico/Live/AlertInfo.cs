using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Ragnarok;
using Ragnarok.Net;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// メッセージサーバー情報です。
    /// </summary>
    public class Alert_MS
    {
        /// <summary>
        /// アドレスを取得します。
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// ポート番号を取得します。
        /// </summary>
        public int Port
        {
            get;
            private set;
        }

        /// <summary>
        /// スレッド番号を取得します。
        /// </summary>
        public int ThreadId
        {
            get;
            private set;
        }

        internal Alert_MS()
        {
        }

        internal Alert_MS(XmlNode node)
            : this()
        {
            foreach (var childObj in node.ChildNodes)
            {
                var child = (XmlNode)childObj;
                var value = child.InnerText;

                switch (child.Name)
                {
                    case "addr":
                        Address = value;
                        break;
                    case "port":
                        Port = StrUtil.ToInt(value, -1);
                        break;
                    case "thread":
                        ThreadId = StrUtil.ToInt(value, -1);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// アラート情報を受け取るために必要な情報です。(無名ユーザー用)
    /// </summary>
    public class AlertInfo : LiveInfomationBase
    {
        /// <summary>
        /// ユーザーＩＤを取得します。
        /// </summary>
        public string UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// ユーザーハッシュを取得します。
        /// </summary>
        public string UserHash
        {
            get;
            private set;
        }

        /// <summary>
        /// メッセージサーバー情報を取得します。
        /// </summary>
        public Alert_MS MS
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected AlertInfo(XmlNode node)
            : base(node, "getalertstatus", "alert")
        {
            this.MS = new Alert_MS();

            foreach (var childObj in RootNode.ChildNodes)
            {
                var child = (XmlNode)childObj;
                var value = child.InnerText;

                switch (child.Name)
                {
                    case "user_id":
                        this.UserId = value;
                        break;
                    case "user_hash":
                        this.UserHash = value;
                        break;
                    case "ms":
                        this.MS = new Alert_MS(child);
                        break;
                }
            }
        }

        /// <summary>
        /// アラート情報を作成します。
        /// </summary>
        public static AlertInfo Create()
        {
            var node = NicoUtil.GetXml(
                "http://video.nicovideo.jp/api/getalertinfo",
                null);

            if (node == null)
            {
                throw new NicoLiveException(NicoStatusCode.NetworkError);
            }

            return new AlertInfo(node);
        }
    }
}
