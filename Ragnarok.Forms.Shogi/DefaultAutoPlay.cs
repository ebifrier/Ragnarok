using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi
{
    using View;

    /// <summary>
    /// 自動再生の種別です。
    /// </summary>
    public enum AutoPlayType
    {
        /// <summary>
        /// 指し手を動かしません。
        /// </summary>
        None,
        /// <summary>
        /// 与えられた指し手を自動再生します。
        /// </summary>
        Normal,
        /// <summary>
        /// 局面を元に戻しながら自動再生します。
        /// </summary>
        Undo,
        /// <summary>
        /// 局面を次に進めながら自動再生します。
        /// </summary>
        Redo,
    }

    /// <summary>
    /// 指し手の自動再生時に使われます。再生用の変化を保存します。
    /// </summary>
    public class DefaultAutoPlay : AutoPlayBase
    {
        private readonly List<Move> moveList;
        private int moveIndex;

        /// <summary>
        /// 自動再生の種類を取得します。
        /// </summary>
        public AutoPlayType AutoPlayType
        {
            get;
            private set;
        }

        /// <summary>
        /// 指し手の最大数を取得します。
        /// </summary>
        public int MaxMoveCount
        {
            get;
            set;
        }
        
        /// <summary>
        /// まだ指し手が残っているか取得します。
        /// </summary>
        private bool HasMove
        {
            get { return (this.moveIndex < MaxMoveCount); }
        }

        /// <summary>
        /// 指し手を一手だけ進めます。
        /// </summary>
        private void DoMove()
        {
            if (!HasMove || Board == null)
            {
                return;
            }

            switch (AutoPlayType)
            {
                case AutoPlayType.Normal:
                    var move = this.moveList[this.moveIndex++];
                    if (move != null)
                    {
                        Board.DoMove(move);
                    }
                    break;
                case AutoPlayType.Undo:
                    this.moveIndex += 1;
                    Board.Undo();
                    break;
                case AutoPlayType.Redo:
                    this.moveIndex += 1;
                    Board.Redo();
                    break;
            }
        }

        /// <summary>
        /// 指し手を進めます。
        /// </summary>
        protected override IEnumerable<bool> DoMoveExecutor()
        {
            // 最初の指し手はすぐに表示します。
            DoMove();

            while (HasMove)
            {
                if (PositionFromBase > Interval)
                {
                    BasePosition += Interval;
                    DoMove();
                }

                yield return true;
            }

            // 必要なら最後の指し手を動かした後に一手分だけ待ちます。
            // エフェクトを表示するためです。
            if (IsWaitForLastMove)
            {
                while (PositionFromBase < Interval)
                {
                    yield return true;
                }

                BasePosition += Interval;
            }
        }

        /// <summary>
        /// 自動再生の途中停止を行います。
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            this.moveIndex = 0;
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (StartBoard == null || !StartBoard.Validate())
            {
                return false;
            }

            if (AutoPlayType == AutoPlayType.Normal)
            {
                if (this.moveList == null)
                {
                    return false;
                }

                return StartBoard.CanMoveList(this.moveList);
            }

            return true;
        }

        /// <summary>
        /// 共通コンストラクタ
        /// </summary>
        private DefaultAutoPlay(Board board, bool isImportant)
            : base(board, isImportant)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DefaultAutoPlay(Board board, bool isImportant,
                               IEnumerable<Move> moveList)
            : this(board, isImportant)
        {
            if (moveList == null)
            {
                throw new ArgumentNullException("moveList");
            }

            AutoPlayType = AutoPlayType.Normal;
            MaxMoveCount = moveList.Count();

            this.moveList = new List<Move>(moveList);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DefaultAutoPlay(Board board, bool isImportant,
                               AutoPlayType autoPlayType, int maxMoveCount = -1)
            : this(board, isImportant)
        {
            if (autoPlayType != AutoPlayType.Undo &&
                autoPlayType != AutoPlayType.Redo)
            {
                throw new ArgumentException(
                    "アンドゥかリドゥを選択してください。",
                    "autoPlayType");
            }

            AutoPlayType = autoPlayType;
            MaxMoveCount = (
                maxMoveCount >= 0 ?
                maxMoveCount :
                (autoPlayType == AutoPlayType.Undo ?
                    board.CanUndoCount :
                    board.CanRedoCount));
        }
    }
}
