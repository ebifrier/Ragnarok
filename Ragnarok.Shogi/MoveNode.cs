using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 変化を木構造で表すためのクラスです。
    /// </summary>
    public sealed class MoveNode
    {
        /// <summary>
        /// 手数を取得または設定します。
        /// </summary>
        public int MoveCount
        {
            get;
            set;
        }

        /// <summary>
        /// 指し手を取得または設定します。
        /// </summary>
        public Move Move
        {
            get;
            set;
        }

        /// <summary>
        /// 次の指し手を取得または設定します。
        /// </summary>
        public MoveNode NextChild
        {
            get;
            set;
        }

        /// <summary>
        /// 次の変化の手を取得または設定します。
        /// </summary>
        public MoveNode NextVariation
        {
            get;
            set;
        }

        /// <summary>
        /// 指定の局面でこの手が指されたとして、指し手などを正規化します。
        /// </summary>
        public void Normalize(Board board)
        {
            var bmove = board.ConvertMove(Move);
            if (bmove == null || !bmove.Validate())
            {
                throw new ShogiException(string.Format(
                    "{0}手目: {1}が指せません。",
                    MoveCount, Move));
            }

            // 棋譜の形式によっては指し手の記号が変わっていることがあります。
            // これを一意にするため、再度指し手文字列を取得しています。
            var move = board.ConvertMove(bmove, false);
            if (move == null || !move.Validate())
            {
                throw new ShogiException(string.Format(
                    "{0}手目: {1}が指せません。",
                    MoveCount, Move));
            }

            if (NextChild != null)
            {
                var nextBoard = board.Clone();
                if (!nextBoard.DoMove(bmove))
                {
                    throw new ShogiException(string.Format(
                        "{0}手目: {1}が指せません。",
                        MoveCount, Move));
                }

                NextChild.Normalize(nextBoard);
            }

            if (NextVariation != null)
            {
                NextVariation.Normalize(board);
            }

            Move = move;
        }
    }
}
