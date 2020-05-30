using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// 特殊な指し手をCSA形式に変換します。
        /// </summary>
        public static string ToCsa(this SpecialMoveType smoveType)
        {
            switch (smoveType)
            {
                case SpecialMoveType.Interrupt:
                    return "%CHUDAN";
                case SpecialMoveType.Resign:
                    return "%TORYO";
                case SpecialMoveType.Sennichite:
                    return "%SENNICHITE";
                case SpecialMoveType.OuteSennichite:
                    return "%OUTE_SENNICHITE";
                case SpecialMoveType.IllegalMove:
                    return "%ILLEGAL_MOVE";
                case SpecialMoveType.TimeUp:
                    return "%TIME_UP";
                case SpecialMoveType.Jishogi:
                    return "%JISHOGI";
                case SpecialMoveType.CheckMate:
                    return "%TSUMI";
                case SpecialMoveType.Error:
                    return "%ERROR";
            }

            throw new ArgumentException(
                "Enumの値が不正です。", nameof(smoveType));
        }

        /// <summary>
        /// <paramref name="move"/>をCSA形式に変換します。
        /// </summary>
        public static string ToCsa(this Move move)
        {
            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }

            if (!move.Validate())
            {
                throw new ArgumentException(
                    "指し手が正しくありません。", nameof(move));
            }

            if (move.IsSpecialMove)
            {
                return move.SpecialMoveType.ToCsa();
            }

            var sb = new StringBuilder();
            sb.Append(
                move.BWType == BWType.Black ? "+" :
                move.BWType == BWType.White ? "-" :
                "");

            if (!move.SrcSquare.IsEmpty)
            {
                sb.Append(move.SrcSquare.File);
                sb.Append(move.SrcSquare.Rank);
            }
            else
            {
                // 駒打の場合
                sb.Append("00");
            }

            if (!move.DstSquare.IsEmpty)
            {
                sb.Append(move.DstSquare.File);
                sb.Append(move.DstSquare.Rank);
            }
            else
            {
                // ほんとはエラー
                sb.Append("00");
            }

            var piece = (
                move.ActionType == ActionType.Drop ?
                new Piece(move.DropPieceType) :
                move.MovePiece);
            if (move.IsPromote)
            {
                // 駒を成った場合は、なった後の駒を出力します。
                piece = new Piece(piece.PieceType, true);
            }

            sb.Append(CsaUtil.PieceToStr(piece));

            return sb.ToString();
        }

        private static readonly Regex SpecialMoveRegex = new Regex(
            @"^%(CHUDAN|TORYO|SENNICHITE|OUTE_SENNICHITE|ILLEGAL_MOVE|TIME_UP|JISHOGI" +
               @"KACHI|HIKIWAKE|MATTA|TSUMI|FUZUMI|ERROR)",
            RegexOptions.IgnoreCase);
        private static readonly Regex MoveRegex = new Regex(
            @"^(\+|\-)?(\d)(\d)(\d)(\d)([\w|\*][\w|\*])",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// CSA形式の指し手を特殊な指し手に変換します。
        /// </summary>
        public static Move CsaToSpecialMove(this Board board, string csa)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var m = SpecialMoveRegex.Match(csa);
            if (!m.Success)
            {
                return null;
            }

            var smoveType = SpecialMoveType.None;
            switch (m.Groups[1].Value)
            {
                case "CHUDAN":
                    smoveType = SpecialMoveType.Interrupt;
                    break;
                case "TORYO":
                    smoveType = SpecialMoveType.Resign;
                    break;
                case "SENNICHITE":
                    smoveType = SpecialMoveType.Sennichite;
                    break;
                case "OUTE_SENNICHITE":
                    smoveType = SpecialMoveType.OuteSennichite;
                    break;
                case "ILLEGAL_MOVE":
                    smoveType = SpecialMoveType.IllegalMove;
                    break;
                case "TIME_UP":
                    smoveType = SpecialMoveType.TimeUp;
                    break;
                case "JISHOGI":
                    smoveType = SpecialMoveType.Jishogi;
                    break;
                case "TSUMI":
                    smoveType = SpecialMoveType.CheckMate;
                    break;
                case "ERROR":
                    smoveType = SpecialMoveType.Error;
                    break;
                default:
                    throw new CsaException(
                        m.Groups[1].Value + ": 対応していないCSA形式の指し手です。");
            }

            return Move.CreateSpecialMove(board.Turn, smoveType);
        }

        /// <summary>
        /// CSA形式の指し手を解析します。
        /// </summary>
        public static Move CsaToMove(this Board board, string csa)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (string.IsNullOrEmpty(csa))
            {
                throw new ArgumentNullException(nameof(csa));
            }

            // 特殊な指し手
            var smove = board.CsaToSpecialMove(csa);
            if (smove != null)
            {
                return smove;
            }

            // 普通の指し手
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
            var srcFile = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
            var srcRank = int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
            var srcSquare =
                (srcFile == 0 || srcRank == 0
                ? Square.Empty
                : new Square(srcFile, srcRank));

            // 移動後の位置
            var dstFile = int.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture);
            var dstRank = int.Parse(m.Groups[5].Value, CultureInfo.InvariantCulture);
            var dstSquare =
                (dstFile == 0 || dstRank == 0
                ? Square.Empty
                : new Square(dstFile, dstRank));

            // 駒
            var nullablePiece = CsaUtil.StrToPiece(m.Groups[6].Value);
            if (nullablePiece == null)
            {
                return null;
            }

            var piece = nullablePiece.Value;
            if (srcSquare.IsEmpty)
            {
                // 駒打ちの場合
                return Move.CreateDrop(side, dstSquare, piece.PieceType);
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

                return Move.CreateMove(side, srcSquare, dstSquare, piece, isPromote);
            }
        }

        /// <summary>
        /// 連続したCSA形式の指し手を、連続した指し手に変換します。
        /// </summary>
        public static IEnumerable<Move> CsaToMoveList(this Board board,
                                                           IEnumerable<string> csaList)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (csaList == null)
            {
                throw new ArgumentNullException(nameof(csaList));
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
