using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Net;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// コメントクライアントの各ルームのイベント引数となります。
    /// </summary>
    public class CommentRoomEventArgs : EventArgs
    {
        /// <summary>
        /// 放送のルームインデックスを取得します。
        /// </summary>
        public int RoomIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// 放送のルームラベルを取得します。
        /// </summary>
        public string RoomLabel
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomEventArgs(int roomIndex, string roomLabel)
        {
            RoomIndex = roomIndex;
            RoomLabel = roomLabel;
        }
    }

    /// <summary>
    /// ルームから切断されたときのイベント引数となります。
    /// </summary>
    public class CommentRoomDisconnectedEventArgs : CommentRoomEventArgs
    {
        /// <summary>
        /// 切断理由を取得します。
        /// </summary>
        public DisconnectReason Reason
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomDisconnectedEventArgs(int roomIndex, string roomLabel,
                                                DisconnectReason reason)
            : base(roomIndex, roomLabel)
        {
        }
    }

    /// <summary>
    /// コメント受信時のイベント引数となります。
    /// </summary>
    public class CommentRoomReceivedEventArgs : CommentRoomEventArgs
    {
        /// <summary>
        /// 処理されたコメントがもしあればそれを取得します。
        /// </summary>
        public Comment Comment
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomReceivedEventArgs(int roomIndex, string roomLabel,
                                            Comment comment)
            : base(roomIndex, roomLabel)
        {
            Comment = comment;
        }
    }

    /// <summary>
    /// コメント送信時のイベント引数となります。
    /// </summary>
    public class CommentRoomSentEventArgs : CommentRoomEventArgs
    {
        /// <summary>
        /// 送信されたコメントを取得します。
        /// </summary>
        public PostComment Comment
        {
            get;
            private set;
        }

        /// <summary>
        /// 送信エラーかどうかを取得します。
        /// </summary>
        public bool IsError
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomSentEventArgs(int roomIndex, string roomLabel,
                                        bool isError)
            : base(roomIndex, roomLabel)
        {
            IsError = isError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomSentEventArgs(int roomIndex, string roomLabel,
                                        PostComment comment)
            : base(roomIndex, roomLabel)
        {
            IsError = false;
            Comment = comment;
        }
    }
}
