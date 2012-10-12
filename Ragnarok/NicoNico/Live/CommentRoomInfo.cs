using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// コメントサーバー各部屋のアドレスなどを保持します。
    /// </summary>
    public class CommentRoomInfo
    {
        /// <summary>
        /// メッセージサーバーのアドレスを取得します。
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// メッセージサーバーのポート番号を取得します。
        /// </summary>
        public int Port
        {
            get;
            private set;
        }

        /// <summary>
        /// スレッド番号を取得します。
        /// </summary>
        public int Thread
        {
            get;
            private set;
        }

        /// <summary>
        /// 座席を取得または設定します。
        /// </summary>
        public string RoomLabel
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomInfo()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomInfo(string roomLabel, string address, int port,
                               int thread)
        {
            this.RoomLabel = roomLabel;
            this.Address = address;
            this.Port = port;
            this.Thread = thread;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomInfo(XmlNode root)
        {
            XmlNode node;

            node = root.SelectSingleNode("/getplayerstatus/ms/addr");
            Address = node.InnerText;

            node = root.SelectSingleNode("/getplayerstatus/ms/port");
            Port = int.Parse(node.InnerText);

            node = root.SelectSingleNode("/getplayerstatus/ms/thread");
            Thread = int.Parse(node.InnerText);

            node = root.SelectSingleNode("/getplayerstatus/user/room_label");
            RoomLabel = node.InnerText;
        }
    }
}
