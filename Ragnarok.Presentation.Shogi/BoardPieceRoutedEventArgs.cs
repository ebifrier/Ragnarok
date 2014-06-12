using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Ragnarok.Shogi;

namespace Ragnarok.Presentation.Shogi
{
    /// <summary>
    /// 局面が変わった時に呼ばれるイベントの引数です。
    /// </summary>
    public sealed class BoardPieceRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardPieceRoutedEventArgs(RoutedEvent ev, Board board, BoardMove move)
            : base(ev)
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
