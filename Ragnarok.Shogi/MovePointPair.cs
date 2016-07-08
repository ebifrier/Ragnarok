using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 指し手とそれに対するポイントを保持します。
    /// </summary>
    [DataContract()]
    public class MovePointPair
    {
        /// <summary>
        /// 指し手を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public LiteralMove Move
        {
            get;
            set;
        }

        /// <summary>
        /// 指し手が持つポイントを取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int Point
        {
            get;
            set;
        }

        /// <summary>
        /// 投票した時刻を取得または設定します。
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public DateTime Timestamp
        {
            get;
            set;
        }
    }
}
