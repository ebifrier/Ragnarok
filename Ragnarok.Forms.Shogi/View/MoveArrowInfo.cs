using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 矢印のデータを保持します。
    /// </summary>
    public sealed class MoveArrowInfo
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoveArrowInfo(Move move, Color color)
        {
            Move = move;
            Color = color;
        }

        /// <summary>
        /// 指し手を取得します。
        /// </summary>
        public Move Move
        {
            get;
            set;
        }

        /// <summary>
        /// 矢印の表示色を取得または設定します。
        /// </summary>
        public Color Color
        {
            get;
            set;
        }
    }
}
