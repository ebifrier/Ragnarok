using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                throw new ArgumentNullException("sfen");
            }

            var split = sfen.Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
            if (split.Count() < 3)
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
                    if (piece == null)
                    {
                        throw new SfenException(
                            "SFEN形式の駒'" + c + "'が正しくありません。");
                    }

                    if (promoted)
                    {
                        piece = new BoardPiece(piece.PieceType, promoted, piece.BWType);
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

            var count = 1;
            foreach (var c in sfen)
            {
                if ('1' <= c && c <= '9')
                {
                    count = c - '0';
                }
                else
                {
                    var piece = SfenUtil.SfenToPiece(c);
                    if (piece == null)
                    {
                        throw new SfenException(
                            "SFEN形式の持ち駒'" + c + "'が正しくありません。");
                    }

                    board.SetCapturedPieceCount(
                        piece.PieceType, piece.BWType, count);
                    count = 1;
                }
            }
        }
        #endregion

        #region Board To Sfen
        /// <summary>
        /// 局面をSFEN形式に変換します。
        /// </summary>
        public static string ToSfen(this Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            return string.Format(
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

                if (piece == null)
                {
                    // 駒がない場合
                    spaceCount += 1;
                }
                else
                {
                    // 駒がある場合
                    if (spaceCount > 0)
                    {
                        yield return spaceCount.ToString();
                        spaceCount = 0;
                    }

                    yield return SfenUtil.PieceToSfen(piece);
                }
            }

            // 空白の数は数字で示します。
            if (spaceCount > 0)
            {
                yield return spaceCount.ToString();
            }
        }

        /// <summary>
        /// 持ち駒をSFEN形式に変換します。
        /// </summary>
        private static string HandToSfen(Board board)
        {
            var handList =
                from turn in new BWType[] { BWType.Black, BWType.White }
                from pieceType in EnumEx.GetValues<PieceType>()
                let obj = new
                {
                    Piece = new BoardPiece(pieceType, false, turn),
                    Count = board.GetCapturedPieceCount(pieceType, turn),
                }
                where obj.Count > 0
                select string.Format("{0}{1}",
                    (obj.Count > 1 ? obj.Count.ToString() : ""),
                    SfenUtil.PieceToSfen(obj.Piece));

            // ToArray()しないと、MONOでstring.Joinのコンパイルに失敗します。
            var array = handList.ToArray();
            return (array.Any() ? string.Join("", array) : "-");
        }
        #endregion
    }
}
