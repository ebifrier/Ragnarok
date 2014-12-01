using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi
{
    /// <summary>
    /// 局面が変わった時に呼ばれるイベントの引数です。
    /// </summary>
    public sealed class BoardPieceEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardPieceEventArgs(Board board, BoardMove move)
        {
            Board = board;
            Move = move;
        }

        /// <summary>
        /// 局面を取得します。
        /// </summary>
        public Board Board
        {
            get;
            private set;
        }

        /// <summary>
        /// 実際に指される指し手を取得します。
        /// </summary>
        public BoardMove Move
        {
            get;
            private set;
        }
    }
}
