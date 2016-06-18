using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 移動中の駒の情報を保持します。
    /// </summary>
    internal sealed class MovingPiece
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MovingPiece(BoardPiece piece, Square sq)
        {
            BoardPiece = piece;
            Square = sq;
        }

        /// <summary>
        /// 移動中の駒の種類を取得します。
        /// </summary>
        public BoardPiece BoardPiece
        {
            get;
            private set;
        }

        /// <summary>
        /// 移動元の駒があったマスを取得します。
        /// </summary>
        public Square Square
        {
            get;
            private set;
        }

        /// <summary>
        /// 駒の中心位置を取得または設定します。
        /// </summary>
        public PointF Center
        {
            get;
            set;
        }
    }
}
