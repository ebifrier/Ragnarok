using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                                                   BWType bwType,
                                                   Square srcSquare,
                                                   Square dstSquare)
        {
            var piece = this[srcSquare];
            if (piece == null || piece.Piece != target || piece.BWType != bwType)
            {
                yield break;
            }

            var move = Move.CreateMove(
                bwType, srcSquare, dstSquare, piece.Piece, false);

            // 成り駒でなければ、成る可能性があります。
            if (!piece.IsPromoted)
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
        public IEnumerable<Move> ListupMoves(Piece piece, BWType bwType,
                                             Square dstSquare)
        {
            if (piece == null)
            {
                throw new ArgumentNullException(nameof(piece));
            }

            // 打てる駒をすべて列挙します。
            if (!piece.IsPromoted && GetHand(piece.PieceType, bwType) > 0)
            {
                var move = Move.CreateDrop(bwType, dstSquare, piece.PieceType);

                // 駒打ちが可能なら、それも該当手となります。
                if (CanMove(move, MoveFlags.CheckOnly))
                {
                    yield return move;
                }
            }

            // 移動による指し手をすべて列挙します。
            var srcRange = GetCanMoveRange(piece, dstSquare, bwType.Flip());
            foreach (var srcSquare in srcRange)
            {
                var moves = GetAvailableMove(piece, bwType, srcSquare, dstSquare);

                foreach (var move in moves)
                {
                    yield return move;
                }
            }
        }

        /// <summary>
        /// 駒の種類にかかわりなく、指定の位置に着手可能な指し手をすべて列挙します。
        /// </summary>
        public IEnumerable<Move> ListupMoves(BWType bwType, Square dstSquare)
        {
            return
                from type in EnumEx.GetValues<PieceType>()
                where type != PieceType.None
                from promoted in new bool[] { true, false }
                let piece = new Piece(type, promoted)
                from move in ListupMoves(piece, bwType, dstSquare)
                select move;
        }

        /// <summary>
        /// 指定の局面で指せるすべての手を取得します。
        /// </summary>
        public IEnumerable<Move> ListupMoves()
        {
            return Board.AllSquares()
                .SelectMany(_ => ListupMoves(Turn, _));
        }
        #endregion

        /// <summary>
        /// 玉のいる位置を取得します。
        /// </summary>
        public Square GetGyoku(BWType bwType)
        {
            var list =
                from sq in Board.AllSquares()
                let piece = this[sq]
                where piece != null
                where piece.PieceType == PieceType.Gyoku
                where piece.BWType == bwType
                select sq;

            return list.FirstOrDefault();
        }

        /// <summary>
        /// <paramref name="bwType"/>側の玉が王手されているか調べます。
        /// </summary>
        public bool IsChecked(BWType bwType)
        {
            var gyoku = GetGyoku(bwType);
            if (gyoku == null || !gyoku.Validate())
            {
                return false;
            }

            // 玉の位置に移動できる駒があれば王手されています。
            return ListupMoves(bwType.Flip(), gyoku).Any();
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
        private bool IsDropAndChecked(BWType bwType, PieceType pieceType,
                                      Square square)
        {
            var piece = this[square];
            if (piece != null)
            {
                return true;
            }

            var move = Move.CreateDrop(bwType, square, pieceType);
            if (DoMove(move, MoveFlags.DoMoveDefault & ~MoveFlags.CheckPawnDropCheckMate))
            {
                if (!IsChecked(bwType))
                {
                    Undo();
                    return false;
                }
                Undo();

                return true;
            }

            return true;
        }

        /// <summary>
        /// <paramref name="srcSquare"/>の駒を<paramref name="dstSquare"/>
        /// に動かすことが可能な指し手を指してみて、なお王手されているか確認します。
        /// </summary>
        private bool IsMoveAndChecked(BWType bwType, Square srcSquare,
                                      Square dstSquare)
        {
            var piece = this[srcSquare];
            if (piece == null || piece.BWType != bwType)
            {
                return true;
            }

            // 玉を取るとエラーになってしまうため、
            // DoMoveまで行かないようにしておく。
            var dstPiece = this[dstSquare];
            if (dstPiece?.PieceType == PieceType.Gyoku)
            {
                return true;
            }

            var move = Move.CreateMove(
                bwType, srcSquare, dstSquare, piece.Piece, false);

            // 成り駒でなければ、成る可能性があります。
            if (!piece.IsPromoted)
            {
                move.IsPromote = true;
                if (DoMove(move))
                {
                    var check = IsChecked(bwType);
                    Undo();
                    return check;
                }
            }

            move.IsPromote = false;
            if (DoMove(move))
            {
                var check = IsChecked(bwType);
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
            var pieceList = new PieceType[]
            {
                // どこでも打てる
                PieceType.Hisya,
                PieceType.Kaku,
                PieceType.Kin,
                PieceType.Gin,
                // ２段目まで
                PieceType.Kyo,
                PieceType.Hu,
                // ３段目まで
                PieceType.Kei,
            };
            var dropPieceType = pieceList
                .Where(_ => clone.GetHand(_, Turn) > 0)
                .FirstOrDefault();
            if (dropPieceType != PieceType.None &&
                AllSquares().Any(_ => !clone.IsDropAndChecked(Turn, dropPieceType, _)))
            {
                return false;
            }

            // 駒の移動はすべてを試します。
            var sqList = AllSquares()
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
