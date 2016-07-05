using System;
using System.Collections.Generic;
using System.Linq;

using Ragnarok.Forms.Shogi.ViewModel;
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
        /// 局面モデルを取得します。
        /// </summary>
        public BoardModel BoardModel
        {
            get;
            private set;
        }

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
        protected override bool DoMove()
        {
            if (!HasMove || BoardModel == null)
            {
                return false;
            }

            switch (AutoPlayType)
            {
                case AutoPlayType.Normal:
                    var move = this.moveList[this.moveIndex++];
                    if (move != null)
                    {
                        BoardModel.DoMove(move);
                    }
                    break;
                case AutoPlayType.Undo:
                    this.moveIndex += 1;
                    BoardModel.Undo();
                    break;
                case AutoPlayType.Redo:
                    this.moveIndex += 1;
                    BoardModel.Redo();
                    break;
            }

            return HasMove;
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (BoardModel == null || !BoardModel.Board.Validate())
            {
                return false;
            }

            var startBoard = BoardModel.Board.Clone(false);
            if (AutoPlayType == AutoPlayType.Normal)
            {
                if (this.moveList == null)
                {
                    return false;
                }

                return startBoard.CanMoveList(this.moveList);
            }

            return true;
        }

        protected override void OnStopped()
        {
            base.OnStopped();

            this.moveIndex = 0;
        }

        /// <summary>
        /// 共通コンストラクタ
        /// </summary>
        private DefaultAutoPlay(BoardModel boardModel, bool isImportant)
            : base(isImportant)
        {
            BoardModel = boardModel;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DefaultAutoPlay(BoardModel boardModel, bool isImportant,
                               IEnumerable<Move> moveList)
            : this(boardModel, isImportant)
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
        public DefaultAutoPlay(BoardModel boardModel, bool isImportant,
                               AutoPlayType autoPlayType, int maxMoveCount = -1)
            : this(boardModel, isImportant)
        {
            if (autoPlayType != AutoPlayType.Undo &&
                autoPlayType != AutoPlayType.Redo)
            {
                throw new ArgumentException(
                    "アンドゥかリドゥを選択してください。",
                    "autoPlayType");
            }

            AutoPlayType = autoPlayType;
            MaxMoveCount = maxMoveCount;
        }
    }
}
