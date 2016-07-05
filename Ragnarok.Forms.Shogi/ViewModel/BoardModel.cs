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
    public sealed class BoardModel : NotifyObject
    {
        public BoardModel()
        {
            Board = new Board();
            CanMakeMoveByGui = true;
            EditMode = EditMode.Normal;
        }

        /// <summary>
        /// 局面が変わった時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<BoardChangedEventArgs> BoardChanged;

        /// <summary>
        /// 局面変更イベントを発行します。
        /// </summary>
        internal void FireBoardChanged(object sender, BoardChangedEventArgs e)
        {
            BoardChanged.SafeRaiseEvent(sender, e);
        }

        /// <summary>
        /// 捜査対象の将棋エレメントを取得します。
        /// </summary>
        public GLShogiElement Shogi
        {
            get;
            internal set;
        }

        /// <summary>
        /// 盤の表示局面を取得または設定します。
        /// </summary>
        public Board Board
        {
            get { return GetValue<Board>("Board"); }
            set { SetValue("Board", value); }
        }

        /// <summary>
        /// GUI側から駒の移動などができるかどうかを取得または設定します。
        /// </summary>
        public bool CanMakeMoveByGui
        {
            get { return GetValue<bool>("CanMakeMoveByGui"); }
            set { SetValue("CanMakeMoveByGui", value); }
        }

        /// <summary>
        /// 編集モードを取得または設定します。
        /// </summary>
        public EditMode EditMode
        {
            get { return GetValue<EditMode>("EditMode"); }
            set { SetValue("EditMode", value); }
        }

        /*/// <summary>
        /// 自動再生の状態を取得します。
        /// </summary>
        public AutoPlayState AutoPlayState
        {
            get { return GetValue<AutoPlayState>("AutoPlayState"); }
            internal set { SetValue("AutoPlayState", value); }
        }*/

        /// <summary>
        /// 局面の駒を操作中かどうかを取得します。
        /// </summary>
        public bool InManipulating
        {
            get { return GetValue<bool>("InManipulating"); }
            internal set { SetValue("InManipulating", value); }
        }

        /// <summary>
        /// 駒を動かすことができるかどうかを取得または設定します。
        /// </summary>
        [DependOnProperty("EditMode")]
        public bool CanMove
        {
            get
            {
                return (
                    CanMakeMoveByGui &&
                    EditMode == EditMode.Normal);
            }
        }

        /// <summary>
        /// 将棋盤の局面を設定します。
        /// </summary>
        public void SetBoard(Board board, Move move, bool isUndo = false)
        {
            if (Shogi == null)
            {
                return;
            }

            FormsUtil.UIProcess(() =>
                Shogi.SetBoard(board, move, isUndo));
        }

        /// <summary>
        /// 将棋盤の局面を進めます。
        /// </summary>
        public void DoMove(Move move, MoveFlags flags = MoveFlags.DoMoveDefault)
        {
            if (Shogi == null)
            {
                return;
            }

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

        /// <summary>
        /// 将棋盤の局面を戻します。
        /// </summary>
        public void Undo()
        {
            if (Shogi == null)
            {
                return;
            }

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
