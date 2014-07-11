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

        /*
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
         * */
    }
}
