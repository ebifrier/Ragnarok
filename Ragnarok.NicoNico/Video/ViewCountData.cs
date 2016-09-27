using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.NicoNico.Video
{
    /// <summary>
    /// 動画の再生数などのデータを保持します。
    /// </summary>
    [Serializable()]
    [DataContract()]
    public sealed class ViewCountData
    {
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
        [DataMember()]
        public DateTime Timestamp
        {
            get;
            set;
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
