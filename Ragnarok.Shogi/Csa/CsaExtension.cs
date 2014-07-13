using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.Csa
{
    /// <summary>
    /// Csa用のエクステンションを定義します。
    /// </summary>
    public static class CsaExtension
    {
        /// <summary>
        /// <paramref name="move"/>をCSA形式に変換します。
        /// </summary>
        public static string ToCsa(this BoardMove move)
        {
            if (move == null)
            {
                throw new ArgumentNullException("move");
            }

            if (!move.Validate())
            {
                throw new ArgumentException("move");
            }

            var sb = new StringBuilder();
            sb.Append(
                move.BWType == BWType.Black ? "+" :
                move.BWType == BWType.White ? "-" :
                "");

            if (move.SrcSquare != null)
            {
                sb.Append(move.SrcSquare.File);
                sb.Append(move.SrcSquare.Rank);
            }
            else
            {
                // 駒打の場合
                sb.Append("00");
            }

            if (move.DstSquare != null)
            {
                sb.Append(move.DstSquare.File);
                sb.Append(move.DstSquare.Rank);
            }
            else
            {
                // ほんとはエラー
                sb.Append("00");
            }

            if (move.ActionType == ActionType.Drop)
            {
                sb.Append(CsaUtil.PieceToChar(new Piece(move.DropPieceType)));
            }
            else
            {
                sb.Append(CsaUtil.PieceToChar(move.MovePiece));
            }

            return sb.ToString();
        }

        private static readonly Regex MoveRegex = new Regex(
            @"^(\+|\-)?(\d)(\d)(\d)(\d)([\w|\*][\w|\*])",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// CSA形式の指し手を解析します。
        /// </summary>
        public static BoardMove CsaToMove(this Board board, string csa)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (string.IsNullOrEmpty(csa))
            {
                throw new ArgumentNullException("csa");
            }

            if (csa.Length < 6)
            {
                throw new ArgumentException("csa");
            }

            var m = MoveRegex.Match(csa);
            if (!m.Success)
            {
                return null;
            }

            var c = m.Groups[1].Value;
            var side = (
                c == "+" ? BWType.Black :
                c == "-" ? BWType.White :
                BWType.None);

            // 移動前の位置
            var srcFile = int.Parse(m.Groups[2].Value);
            var srcRank = int.Parse(m.Groups[3].Value);
            var srcSquare =
                (srcFile == 0 || srcRank == 0
                ? (Square)null
                : new Square(srcFile, srcRank));

            // 移動後の位置
            var dstFile = int.Parse(m.Groups[4].Value);
            var dstRank = int.Parse(m.Groups[5].Value);
            var dstSquare =
                (dstFile == 0 || dstRank == 0
                ? (Square)null
                : new Square(dstFile, dstRank));

            // 駒
            var piece = CsaUtil.StrToPiece(m.Groups[6].Value);
            if (piece == null || piece.PieceType == PieceType.None)
            {
                return null;
            }

            if (srcSquare == null)
            {
                // 駒打ちの場合
                return new BoardMove
                {
                    DstSquare = dstSquare,
                    DropPieceType = piece.PieceType,
                    BWType = side,
                };
            }
            else
            {
                // 駒の移動の場合、成りの判定を行います。
                var srcPiece = board[srcSquare];
                if (srcPiece == null || !srcPiece.Validate())
                {
                    return null;
                }

                // CSA形式の場合、駒が成ると駒の種類が変わります。
                var isPromote = (!srcPiece.IsPromoted && piece.IsPromoted);
                if (isPromote)
                {
                    piece = new Piece(piece.PieceType, false);
                }

                return new BoardMove
                {
                    DstSquare = dstSquare,
                    SrcSquare = srcSquare,
                    MovePiece = piece,
                    IsPromote = isPromote,
                    BWType = side,
                };
            }
        }

        /// <summary>
        /// 連続したCSA形式の指し手を、連続した指し手に変換します。
        /// </summary>
        public static IEnumerable<BoardMove> CsaToMoveList(this Board board,
                                                           IEnumerable<string> csaList)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (csaList == null)
            {
                throw new ArgumentNullException("csaList");
            }

            var tmpBoard = board.Clone();
            foreach (var csa in csaList)
            {
                var move = tmpBoard.CsaToMove(csa);
                if (move == null || !move.Validate())
                {
                    yield break;
                }

                if (!tmpBoard.DoMove(move))
                {
                    yield break;
                }

                yield return move;
            }
        }
    }
}
