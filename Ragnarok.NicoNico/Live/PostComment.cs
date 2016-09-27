using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// コメントサーバー内で使われる、投稿用のコメントです。
    /// </summary>
    public class PostComment : IComparable<PostComment>
    {
        /// <summary>
        /// コメント内容を取得または設定します。
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// コメントの属性を取得または設定します。
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// コメントの投稿名を取得または設定します。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 投稿開始時刻を取得または設定します。
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 投稿可能時刻で比較します。
        /// </summary>
        public int CompareTo(PostComment other)
        {
            return StartDate.CompareTo(other.StartDate);
        }

        /// <summary>
        /// オブジェクトの妥当性を確認します。
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(Text))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PostComment()
        {
            StartDate = DateTime.Now;
        }
    }
}
