using System;
using System.Collections.Generic;
using System.Linq;

using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi
{
    /// <summary>
    /// 局面が変わった時に呼ばれるイベントの引数です。
    /// </summary>
    public sealed class BoardChangedEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardChangedEventArgs(Board board, Move move, bool isUndo,
                                     bool isMovedByGui)
        {
            Board = board;
            Move = move;
            IsUndo = isUndo;
            IsMovedByGui = isMovedByGui;
        }

        /// <summary>
        /// 局面が全部更新された場合のコンストラクタ
        /// </summary>
        public BoardChangedEventArgs(Board board, bool isMovedByGui)
        {
            Board = board;
            IsMovedByGui = IsMovedByGui;
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
        public Move Move
        {
            get;
            private set;
        }

        /// <summary>
        /// Undoされたかどうかを取得します。
        /// </summary>
        public bool IsUndo
        {
            get;
            private set;
        }

        /// <summary>
        /// 局面がまとめて更新されたかどうかを取得します。
        /// </summary>
        public bool IsBoardUpdated
        {
            get { return (Move == null); }
        }

        /// <summary>
        /// GUIによって局面を操作されたかどうかを取得します。
        /// </summary>
        public bool IsMovedByGui
        {
            get;
            private set;
        }
    }
}
