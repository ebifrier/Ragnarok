using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ragnarok.Shogi.Sfen
{
    /// <summary>
    /// SFEN形式で局面を扱うためのクラスです。
    /// </summary>
    public static class SfenBoard
    {
        #region Sfen To Board
        /// <summary>
        /// SFEN形式の文字列から、局面を読み取ります。
        /// </summary>
        public static Board Parse(string sfen)
        {
            if (string.IsNullOrEmpty(sfen))
            {
                throw new ArgumentNullException(nameof(sfen));
            }

            var split = sfen.Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 3)
            {
                throw new SfenException(
                    "SFEN形式の盤表現が正しくありません。");
            }

            var board = new Board(false);
            ParseBoard0(board, split[0]);
            ParseHand(board, split[2]);
            board.Turn = ParseTurn(split[1]);
            return board;
        }

        /// <summary>
        /// 手番を読み取ります。
        /// </summary>
        private static BWType ParseTurn(string text)
        {
            if (text.Length != 1 ||
                (text[0] != 'b' && text[0] != 'w'))
            {
                throw new SfenException(
                    "SFEN形式の手番表現が正しくありません。");
            }

            return (
                text[0] == 'b' ? BWType.Black :
                text[0] == 'w' ? BWType.White :
                BWType.None);
        }

        /// <summary>
        /// 局面を読み込みます。
        /// </summary>
        private static void ParseBoard0(Board board, string sfen)
        {
            var rank = 1;
            var file = 9;
            var promoted = false;

            foreach (var c in sfen)
            {
                if (rank > 9)
                {
                    throw new SfenException(
                        "局面の段数が９を超えます。");
                }

                if (c == '/')
                {
                    if (file != 0)
                    {
                        throw new SfenException(
                            "SFEN形式の" + rank + "段の駒数が合いません。");
                    }

                    rank += 1;
                    file = 9;
                    promoted = false;
                }
                else if (c == '+')
                {
                    promoted = true;
                }
                else if ('1' <= c && c <= '9')
                {
                    file -= (c - '0');
                    promoted = false;
                }
                else
                {
                    if (file < 1)
                    {
                        throw new SfenException(
                            "SFEN形式の" + rank + "段の駒数が多すぎます。");
                    }

                    var piece = SfenUtil.SfenToPiece(c);
                    if (piece.IsNone())
                    {
                        throw new SfenException(
                            "SFEN形式の駒'" + c + "'が正しくありません。");
                    }

                    if (promoted &&
                        piece.GetRawType() != Piece.King &&
                        piece.GetRawType() != Piece.Gold)
                    {
                        piece = piece.Modify(promoted);
                    }

                    board[file, rank] = piece;
                    file -= 1;
                    promoted = false;
                }
            }

            if (file != 0)
            {
                throw new SfenException(
                    "SFEN形式の" + rank + "段の駒数が合いません。");
            }
        }

        /// <summary>
        /// 持ち駒を読み込みます。
        /// </summary>
        private static void ParseHand(Board board, string sfen)
        {
            if (sfen[0] == '-')
            {
                // 何もする必要がありません。
                return;
            }

            var count = 0;
            foreach (var c in sfen)
            {
                if ('0' <= c && c <= '9')
                {
                    // 10と数字が並ぶ可能性があります。
                    count = count * 10 + (c - '0');
                }
                else
                {
                    var piece = SfenUtil.SfenToPiece(c);
                    if (piece.IsNone())
                    {
                        throw new SfenException(
                            "SFEN形式の持ち駒'" + c + "'が正しくありません。");
                    }

                    // 持ち駒の数は0以上に合わせます。
                    var pcount = Math.Max(count, 1);
                    board.SetHand(piece, pcount);
                    count = 0;
                }
            }
        }
        #endregion

        #region Board To Sfen
        /// <summary>
        /// 局面をSFEN形式に変換します。
        /// </summary>
        public static string BoardToSfen(Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1} {2} {3}",
                Board0ToSfen(board),
                TurnToSfen(board.Turn),
                HandToSfen(board),
                board.MoveCount);
        }

        /// <summary>
        /// 手番をSFEN形式に変換します。
        /// </summary>
        private static string TurnToSfen(BWType turn)
        {
            if (turn == BWType.None)
            {
                throw new SfenException(
                    "局面の手番が正しくありません。");
            }

            return (turn == BWType.Black ? "b" : "w");
        }

        /// <summary>
        /// 局面をSFEN形式に変換します。
        /// </summary>
        private static string Board0ToSfen(Board board)
        {
            var lineList =
                from rank in Enumerable.Range(1, Board.BoardSize)
                select string.Join("", RankToSfen(board, rank).ToArray());

            return string.Join("/", lineList.ToArray());
        }

        /// <summary>
        /// 各段の駒文字を返します。
        /// </summary>
        private static IEnumerable<string> RankToSfen(Board board, int rank)
        {
            var spaceCount = 0;

            for (var file = Board.BoardSize; file >= 1; --file)
            {
                var piece = board[file, rank];

                if (piece.IsNone())
                {
                    // 駒がない場合
                    spaceCount += 1;
                }
                else
                {
                    // 駒がある場合
                    if (spaceCount > 0)
                    {
                        yield return $"{spaceCount}";
                        spaceCount = 0;
                    }

                    yield return SfenUtil.PieceToSfen(piece);
                }
            }

            // 空白の数は数字で示します。
            if (spaceCount > 0)
            {
                yield return $"{spaceCount}";
            }
        }

        /// <summary>
        /// 持ち駒をSFEN形式に変換します。
        /// </summary>
        private static string HandToSfen(Board board)
        {
            var handList =
                from turn in BWTypeUtil.BlackWhite()
                from piece in PieceUtil.RawTypes(turn)
                let obj = new
                {
                    Piece = piece,
                    Count = board.GetHand(piece),
                }
                where obj.Count > 0
                let count = obj.Count > 1 ? $"{obj.Count}" : ""
                let pieceSfen = SfenUtil.PieceToSfen(obj.Piece)
                select $"{count}{pieceSfen}";

            // ToArray()しないと、MONOでstring.Joinのコンパイルに失敗します。
            var array = handList.ToArray();
            return (array.Any() ? string.Join("", array) : "-");
        }
        #endregion
    }
}
