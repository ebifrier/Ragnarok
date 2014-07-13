using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                throw new ArgumentNullException("csa");
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
        public static string ToCsa(this Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (Board.BoardEquals(board, new Board()))
            {
                return "PI, +";
            }
            else
            {
                var bhand = HandToCsa(board, BWType.Black);
                var whand = HandToCsa(board, BWType.White);

                if (string.IsNullOrEmpty(bhand))
                {
                    bhand += "\n";
                }

                if (string.IsNullOrEmpty(whand))
                {
                    whand += "\n";
                }

                return string.Format(
                    "{0}\n{1}{2}{3}",
                    BoardToCsa(board),
                    bhand, whand,
                    TurnToCsa(board.Turn));
            }
        }

        /// <summary>
        /// 手番をCSA形式に変換します。
        /// </summary>
        private static string TurnToCsa(BWType turn)
        {
            if (turn == BWType.None)
            {
                throw new CsaException(
                    "局面の手番が正しくありません。");
            }

            return (turn == BWType.Black ? "+" : "-");
        }

        /// <summary>
        /// 局面をCSA形式に変換します。
        /// </summary>
        private static string BoardToCsa(Board board)
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

                yield return CsaUtil.BoardPieceToStr(piece);
            }
        }

        /// <summary>
        /// 持ち駒をCSA形式に変換します。
        /// </summary>
        /// <example>
        /// P+00KIOOFU
        /// </example>
        private static string HandToCsa(Board board, BWType turn)
        {
            var handList =
                from pieceType in EnumEx.GetValues<PieceType>()
                let count = board.GetCapturedPieceCount(pieceType, turn)
                let str = string.Format("00{0}",
                    CsaUtil.PieceToStr(new Piece(pieceType)))
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
                return string.Format("P{0}{1}",
                    string.Join("", array),
                    turn == BWType.Black ? "+" : "-");
            }
        }
        #endregion
    }
}
