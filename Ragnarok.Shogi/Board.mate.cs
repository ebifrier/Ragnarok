using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    public partial class Board
    {
        /// <summary>
        /// 玉のいる位置を取得します。
        /// </summary>
        public Square GetGyoku(BWType bwType)
        {
            for (var file = 1; file <= Board.BoardSize; ++file)
            {
                for (var rank = 1; rank <= Board.BoardSize; ++rank)
                {
                    var sq = new Square(file, rank);
                    var piece = this[sq];

                    if (piece != null &&
                        piece.PieceType == PieceType.Gyoku &&
                        piece.BWType == bwType)
                    {
                        return sq;
                    }
                }
            }

            return null;
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
        /// <paramref name="square"/>に<paramref name="pieceType"/>を打ち、
        /// なお王手されているか確認します。
        /// </summary>
        private bool IsDropAndChecked(PieceType pieceType, Square square)
        {
            var piece = this[square];
            if (piece != null)
            {
                return true;
            }

            var move = new BoardMove()
            {
                DstSquare = square,
                BWType = piece.BWType,
                DropPieceType = pieceType,
            };
            if (DoMove(move))
            {
                if (!IsChecked(Turn))
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
        private bool IsMoveAndChecked(Square srcSquare, Square dstSquare)
        {
            var piece = this[srcSquare];
            if (piece == null)
            {
                return true;
            }

            var move = new BoardMove()
            {
                DstSquare = dstSquare,
                SrcSquare = srcSquare,
                MovePiece = piece.Piece,
                BWType = piece.BWType,
            };

            // 成り駒でなければ、成る可能性があります。
            if (!piece.IsPromoted)
            {
                move.IsPromote = true;
                if (DoMove(move))
                {
                    if (!IsChecked(Turn))
                    {
                        Undo();
                        return false;
                    }
                    Undo();

                    return true;
                }
            }

            move.IsPromote = false;
            if (DoMove(move))
            {
                if (!IsChecked(Turn))
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
        /// 手番側の玉が詰まされているか調べます。
        /// </summary>
        /// <remarks>
        /// 手番のある側が可能な指し手をすべて指しても
        /// 玉の王手が外れなければ詰みとなります。
        /// </remarks>
        public bool IsCheckMated()
        {
            if (!IsChecked(Turn))
            {
                return false;
            }

            var srcSquares = (
                from file in Enumerable.Range(1, BoardSize)
                from rank in Enumerable.Range(1, BoardSize)
                select new Square(file, rank))
                .ToList();

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
                .Where(_ => GetCapturedPieceCount(_, Turn) > 0)
                .FirstOrDefault();
            if (dropPieceType != PieceType.None)
            {
                if (srcSquares.Any(_ => !IsDropAndChecked(dropPieceType, _)))
                {
                    return false;
                }
            }

            // 駒の移動はすべてを試します。
            var sqList = srcSquares
                .SelectMany(_ =>
                    GetCanMoveRange(_, Turn)
                        .Select(__ => Tuple.Create(_, __)))
                .Where(_ => _.Item2.Validate());
            if (sqList.Any(_ => !IsMoveAndChecked(_.Item1, _.Item2)))
            {
                return false;
            }

            return true;
        }
    }
}
