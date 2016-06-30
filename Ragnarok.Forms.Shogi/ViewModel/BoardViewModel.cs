using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Ragnarok.ObjectModel;
using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi.ViewModel
{
    using View;

    /// <summary>
    /// 将棋盤の局面を外部から操作するためのクラスです。
    /// </summary>
    public sealed class BoardViewModel : NotifyObject
    {
        public BoardViewModel(GLShogiElement shogi)
        {
            Shogi = shogi;
            Board = new Board();
        }

        /// <summary>
        /// GUIによって局面が変わった時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<BoardPieceEventArgs> MovedByGui
        {
            add { Shogi.MovedByGui += value; }
            remove { Shogi.MovedByGui -= value; }
        }

        /// <summary>
        /// 局面編集によって局面が変わった時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<BoardPieceEventArgs> BoardEdited
        {
            add { Shogi.BoardEdited += value; }
            remove { Shogi.BoardEdited -= value; }
        }

        /// <summary>
        /// 捜査対象の将棋エレメントを取得します。
        /// </summary>
        public GLShogiElement Shogi
        {
            get;
            private set;
        }

        /// <summary>
        /// 局面の駒を操作中かどうかを取得します。
        /// </summary>
        public bool InManipulating
        {
            get { return Shogi.InManipulating; }
        }

        /// <summary>
        /// 盤の表示局面を取得または設定します。
        /// </summary>
        public Board Board
        {
            get { return GetValue<Board>("Board"); }
            set { SetValue("Board", value); }
        }

        public bool CanMove
        {
            get { return (
                    Shogi.AutoPlayState == AutoPlayState.None &&
                    Shogi.EditMode == EditMode.Normal); }
        }

        public void DoMove(Move move, MoveFlags flags = MoveFlags.DoMoveDefault)
        {
            using (LazyLock())
            {
                if (!CanMove || !Board.DoMove(move, flags))
                {
                    return;
                }

                FormsUtil.UIProcess(() =>
                    Shogi.SetBoard(Board, move));
            }
        }

        public void Undo()
        {
            using (LazyLock())
            {
                if (!CanMove)
                {
                    return;
                }

                var move = Board.Undo();
                if (move == null)
                {
                    return;
                }

                FormsUtil.UIProcess(() =>
                    Shogi.SetBoard(Board, move, true));
            }
        }
    }
}
