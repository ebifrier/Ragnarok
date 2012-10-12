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
        /// コンストラクタ
        /// </summary>
        public CommentRoomEventArgs(int roomIndex)
        {
            RoomIndex = roomIndex;
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
        public CommentRoomDisconnectedEventArgs(int roomIndex,
            DisconnectReason reason)
            : base(roomIndex)
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
        public CommentRoomReceivedEventArgs(int roomIndex, Comment comment)
            : base(roomIndex)
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
        public CommentRoomSentEventArgs(int roomIndex, bool isError)
            : base(roomIndex)
        {
            IsError = isError;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommentRoomSentEventArgs(int roomIndex, PostComment comment)
            : base(roomIndex)
        {
            IsError = false;
            Comment = comment;
        }
    }
}
