using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ragnarok.Shogi.Csa
{
    /// <summary>
    /// CSA形式の局面を扱うためのクラスです。
    /// </summary>
    public static class CsaBoard
    {
        #region Csa To Board
        /// <summary>
        /// CSA形式の文字列から、局面を読み取ります。
        /// </summary>
        public static Board Parse(string csa)
        {
            if (string.IsNullOrEmpty(csa))
            {
                throw new ArgumentNullException(nameof(csa));
            }

            var parser = new CsaBoardParser();

            csa.Split(
                    new char[] { '\n', '\r' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(_ => !CsaUtil.IsCommentLine(_))
                .ForEach(_ => parser.TryParse(_));

            return parser.Board;
        }
        #endregion

        #region Board To Csa
        /// <summary>
        /// 局面をCSA形式に変換します。
        /// </summary>
        public static string BoardToCsa(Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var comp = new Board();
            var comp2 = new Board();
            comp2.Turn = Colour.White;

            if (Board.BoardEquals(board, comp))
            {
                return "PI\n+";
            }
            else if (Board.BoardEquals(board, comp2))
            {
                return "PI\n-";
            }
            else
            {
                var bhand = HandToCsa(board, Colour.Black);
                var whand = HandToCsa(board, Colour.White);

                if (!string.IsNullOrEmpty(bhand))
                {
                    bhand += "\n";
                }

                if (!string.IsNullOrEmpty(whand))
                {
                    whand += "\n";
                }

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}\n{1}{2}{3}",
                    BoardToCsa2(board),
                    bhand, whand,
                    TurnToCsa(board.Turn));
            }
        }

        /// <summary>
        /// 手番をCSA形式に変換します。
        /// </summary>
        private static string TurnToCsa(Colour turn)
        {
            if (turn == Colour.None)
            {
                throw new CsaException(
                    "局面の手番が正しくありません。");
            }

            return (turn == Colour.Black ? "+" : "-");
        }

        /// <summary>
        /// 局面をCSA形式に変換します。
        /// </summary>
        private static string BoardToCsa2(Board board)
        {
            var lineList =
                from rank in Enumerable.Range(1, Board.BoardSize)
                let strs = RankToCsa(board, rank).ToArray()
                select "P" + rank + string.Join("", strs);

            return string.Join("\n", lineList.ToArray());
        }

        /// <summary>
        /// 各段のCSA局面を返します。
        /// </summary>
        private static IEnumerable<string> RankToCsa(Board board, int rank)
        {
            for (var file = Board.BoardSize; file >= 1; --file)
            {
                var piece = board[file, rank];

                yield return CsaUtil.PieceToStr(piece);
            }
        }

        /// <summary>
        /// 持ち駒をCSA形式に変換します。
        /// </summary>
        /// <example>
        /// P+00KIOOFU
        /// </example>
        private static string HandToCsa(Board board, Colour turn)
        {
            var handList =
                from rawType in PieceUtil.RawTypes()
                let count = board.GetHand(rawType.Modify(turn))
                let str = $"00{CsaUtil.PieceTypeToStr(rawType)}"
                let list = Enumerable.Range(1, count).Select(_ => str)
                select string.Join("", list.ToArray());

            // ToArray()しないと、MONOでstring.Joinのコンパイルに失敗します。
            var array = handList.ToArray();
            if (!array.Any())
            {
                return string.Empty;
            }
            else
            {
                var turnStr = (turn == Colour.Black ? "+" : "-");
                var arrayStr = string.Join("", array);
                return $"P{turnStr}{arrayStr}";
            }
        }
        #endregion
    }
}
