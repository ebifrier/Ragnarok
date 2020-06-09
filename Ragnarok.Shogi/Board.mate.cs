using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Shogi
{
    public partial class Board
    {
        #region ListupMoves
        /// <summary>
        /// <paramref name="srcSquare"/>の駒を<paramref name="dstSquare"/>
        /// に動かすことが可能な指し手をすべて列挙します。
        /// </summary>
        private IEnumerable<Move> GetAvailableMove(Piece target,
                                                   Square srcSquare,
                                                   Square dstSquare)
        {
            var piece = this[srcSquare];
            if (piece.IsNone() || piece != target)
            {
                yield break;
            }

            var move = Move.CreateMove(piece, srcSquare, dstSquare);

            // 成り駒でなければ、成る可能性があります。
            if (!piece.IsPromoted())
            {
                move.IsPromote = true;
                if (CanMove(move, MoveFlags.CheckOnly))
                {
                    // yield returnなのでCloneしないとまずい。
                    yield return move.Clone();
                }
            }

            move.IsPromote = false;
            if (CanMove(move, MoveFlags.CheckOnly))
            {
                yield return move;
            }
        }

        /// <summary>
        /// 駒の種類と新しい位置から、可能な指し手をすべて検索します。
        /// </summary>
        public IEnumerable<Move> ListupMoves(Piece piece,
                                             Square dstSquare)
        {
            // 打てる駒をすべて列挙します。
            if (!piece.IsPromoted() && GetHand(piece) > 0)
            {
                var move = Move.CreateDrop(piece, dstSquare);

                // 駒打ちが可能なら、それも該当手となります。
                if (CanMove(move, MoveFlags.CheckOnly))
                {
                    yield return move;
                }
            }

            // 移動による指し手をすべて列挙します。
            var srcRange = GetCanMoveRange(piece.FlipColour(), dstSquare);
            foreach (var srcSquare in srcRange)
            {
                var moves = GetAvailableMove(piece, srcSquare, dstSquare);

                foreach (var move in moves)
                {
                    yield return move;
                }
            }
        }

        /// <summary>
        /// 駒の種類にかかわりなく、指定の位置に着手可能な指し手をすべて列挙します。
        /// </summary>
        public IEnumerable<Move> ListupMoves(Colour colour, Square dstSquare)
        {
            return
                from piece in PieceUtil.PieceTypes(colour)
                from move in ListupMoves(piece, dstSquare)
                select move;
        }

        /// <summary>
        /// 指定の局面で指せるすべての手を取得します。
        /// </summary>
        public IEnumerable<Move> ListupMoves()
        {
            return Board.Squares()
                .SelectMany(_ => ListupMoves(Turn, _));
        }
        #endregion

        /// <summary>
        /// 玉のいる位置を取得します。
        /// </summary>
        public Square GetKing(Colour colour)
        {
            var list =
                from sq in Board.Squares()
                let piece = this[sq]
                where piece == Piece.King.With(colour)
                select sq;

            return list.FirstOrDefault();
        }

        /// <summary>
        /// <paramref name="colour"/>側の玉が王手されているか調べます。
        /// </summary>
        public bool IsChecked(Colour colour)
        {
            var king = GetKing(colour);
            if (!king.Validate())
            {
                return false;
            }

            // 玉の位置に移動できる駒があれば王手されています。
            return ListupMoves(colour.Flip(), king).Any();
        }

        /// <summary>
        /// 手番側の玉が王手されているか調べます。
        /// </summary>
        public bool IsChecked()
        {
            return IsChecked(Turn);
        }

        /// <summary>
        /// 手番と反対側の玉が王手されているか調べます。
        /// </summary>
        public bool IsChecking()
        {
            return IsChecked(Turn.Flip());
        }

        /// <summary>
        /// <paramref name="square"/>に<paramref name="pieceType"/>を打ち、
        /// なお王手されているか確認します。
        /// </summary>
        private bool IsDropAndChecked(Piece dropPiece, Square square)
        {
            var piece = this[square];
            if (!piece.IsNone())
            {
                return true;
            }

            var flags = MoveFlags.DoMoveDefault & ~MoveFlags.CheckPawnDropCheckMate;
            var move = Move.CreateDrop(dropPiece, square);
            if (DoMove(move, flags))
            {
                var check = IsChecked(dropPiece.GetColour());
                Undo();
                return check;
            }

            return true;
        }

        /// <summary>
        /// <paramref name="srcSquare"/>の駒を<paramref name="dstSquare"/>
        /// に動かすことが可能な指し手を指してみて、なお王手されているか確認します。
        /// </summary>
        private bool IsMoveAndChecked(Colour colour, Square srcSquare,
                                      Square dstSquare)
        {
            var piece = this[srcSquare];
            if (piece.IsNone() || piece.GetColour() != colour)
            {
                return true;
            }

            // 玉を取るとエラーになってしまうため、
            // DoMoveまで行かないようにしておく。
            var dstPiece = this[dstSquare];
            if (dstPiece.GetPieceType() == Piece.King)
            {
                return true;
            }

            var move = Move.CreateMove(piece, srcSquare, dstSquare);

            // 成り駒でなければ、成る可能性があります。
            if (!piece.IsPromoted())
            {
                move.IsPromote = true;
                if (DoMove(move))
                {
                    var check = IsChecked(colour);
                    Undo();
                    return check;
                }
            }

            move.IsPromote = false;
            if (DoMove(move))
            {
                var check = IsChecked(colour);
                Undo();
                return check;
            }

            return true;
        }

        /// <summary>
        /// 手番側の玉が詰まされているか調べます。
        /// </summary>
        /// <remarks>
        /// 手番のある側が可能な指し手をすべて指しても
        /// 玉の王手が外れなければ詰みとなります。
        /// </remarks>
        public bool IsCheckMated()
        {
#if OUTE_SHOGI
            // 王手将棋では王手＝積みとなります。
            return IsChecked(Turn);
#else
            if (!IsChecked(Turn))
            {
                return false;
            }

            // 実際に駒を動かしながら調べるため、
            // コピーしたオブジェクトを使います。
            var clone = Clone();

            // 駒打ちは合い駒を調べればいいので、
            // 打てる範囲がもっとも広い駒を１つだけ調べます。
            var pieceList = new Piece[]
            {
                // どこでも打てる
                Piece.Rook,
                Piece.Bishop,
                Piece.Gold,
                Piece.Silver,
                // ２段目まで
                Piece.Lance,
                Piece.Pawn,
                // ３段目まで
                Piece.Knight,
            };
            var dropPieceType = pieceList
                .Select(_ => _.With(Turn))
                .Where(_ => clone.GetHand(_) > 0)
                .FirstOrDefault();
            if (!dropPieceType.IsNone() &&
                Squares().Any(_ => !clone.IsDropAndChecked(dropPieceType, _)))
            {
                return false;
            }

            // 駒の移動はすべてを試します。
            var sqList = Squares()
                .SelectMany(_ =>
                    clone.GetCanMoveRange(_, Turn)
                        .Select(__ => Tuple.Create(_, __)));
            if (sqList.Any(_ => !clone.IsMoveAndChecked(Turn, _.Item1, _.Item2)))
            {
                return false;
            }

            return true;
#endif
        }
    }
}
