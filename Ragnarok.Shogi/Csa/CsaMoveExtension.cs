using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Csa
{
    /// <summary>
    /// CsaMove用のエクステンションを定義します。
    /// </summary>
    public static class CsaMoveExtension
    {
        /// <summary>
        /// <see cref="CsaMove"/>を<see cref="BoardMove"/>に変換します。
        /// </summary>
        public static BoardMove ConvertCsaMove(this Board board, CsaMove csaMove)
        {
            if (csaMove == null)
            {
                return null;
            }

            var newPiece = board[csaMove.DstSquare];
            if (csaMove.IsDrop)
            {
                if (newPiece != null)
                {
                    return null;
                }

                return new BoardMove
                {
                    BWType = board.Turn,
                    DstSquare = csaMove.DstSquare,
                    DropPieceType = csaMove.Piece.PieceType,
                };
            }
            else
            {
                var oldPiece = board[csaMove.SrcSquare];
                if (oldPiece == null)
                {
                    return null;
                }

                return new BoardMove
                {
                    DstSquare = csaMove.DstSquare,
                    SrcSquare = csaMove.SrcSquare,
                    MovePiece = csaMove.Piece,
                    TookPiece = BoardPiece.GetPiece(newPiece),
                    IsPromote = (!oldPiece.IsPromoted && csaMove.Piece.IsPromoted),
                    BWType = board.Turn,
                };
            }
        }

        /// <summary>
        /// <see cref="BoardMove"/>を<see cref="CsaMove"/>に変換します。
        /// </summary>
        public static CsaMove ConvertBoardMove(this Board board, BoardMove bmove)
        {
            if (bmove == null || !bmove.Validate())
            {
                return null;
            }

            var newPiece = board[bmove.DstSquare];
            if (bmove.ActionType == ActionType.Drop)
            {
                if (newPiece != null)
                {
                    return null;
                }

                return new CsaMove
                {
                    Side = board.Turn,
                    DstSquare = bmove.DstSquare,
                    Piece = new Piece(bmove.DropPieceType, false),
                };
            }
            else
            {
                var oldPiece = board[bmove.SrcSquare];
                if (oldPiece == null)
                {
                    return null;
                }

                return new CsaMove
                {
                    Side = board.Turn,
                    DstSquare = bmove.DstSquare,
                    SrcSquare = bmove.SrcSquare,
                    Piece = new Piece(
                        oldPiece.PieceType,
                        oldPiece.IsPromoted ||
                        bmove.ActionType == ActionType.Promote),
                };
            }
        }
    }
}
