using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.Shogi.Kif
{
    /// <summary>
    /// bod形式の局面を処理するクラスです。
    /// </summary>
    /// <seealso cref="http://d.hatena.ne.jp/mozuyama/20030909/p5" />
    public static class BodBoard
    {
        #region Bod To Board
        /// <summary>
        /// bod形式の文字列から、局面を読み取ります。
        /// </summary>
        public static Board Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var parser = new BodParser();
            var lines = text.Split(
                new char[] { '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (KifUtil.IsCommentLine(line))
                {
                    continue;
                }

                // 局面の読み取りを試みます。
                if (parser.TryParse(line))
                {
                    continue;
                }

                var item = KifUtil.ParseHeaderItem(line);
                if (item != null)
                {
                    continue;
                }

                break;
            }

            if (!parser.IsCompleted)
            {
                throw new ShogiException(
                    "局面の読み取りを完了できませんでした。");
            }

            return parser.Board;
        }
        #endregion

        #region Board To Bod
        /// <summary>
        /// 局面をbod形式に変換します。
        /// </summary>
        /// <example>
        /// 後手の持駒：飛　角　金　銀　桂　香　歩四　
        ///   ９ ８ ７ ６ ５ ４ ３ ２ １
        /// +---------------------------+
        /// |v香v桂 ・ ・ ・ ・ ・ ・ ・|一
        /// | ・ ・ ・ 馬 ・ ・ 龍 ・ ・|二
        /// | ・ ・v玉 ・v歩 ・ ・ ・ ・|三
        /// |v歩 ・ ・ ・v金 ・ ・ ・ ・|四
        /// | ・ ・v銀 ・ ・ ・v歩 ・ ・|五
        /// | ・ ・ ・ ・ 玉 ・ ・ ・ ・|六
        /// | 歩 歩 ・ 歩 歩v歩 歩 ・ 歩|七
        /// | ・ ・ ・ ・ ・ ・ ・ ・ ・|八
        /// | 香 桂v金 ・v金 ・ ・ 桂 香|九
        /// +---------------------------+
        /// 先手の持駒：銀二　歩四　
        /// 手数＝171  ▲６二角成  まで
        /// 
        /// 後手番
        /// </example>
        public static string BoardToBod(Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (!board.Validate())
            {
                throw new ArgumentException(
                    "局面の状態が正しくありません。", nameof(board));
            }

            var result = new List<string>();

            var type = BoardTypeUtil.GetBoardTypeFromBoard(board);
            if (type != BoardType.None)
            {
                result.Add("手合割：" + EnumEx.GetLabel(type));
            }
            else
            {
                result.Add("後手の持駒：" + HandToBod(board, BWType.White));
                result.Add("  ９ ８ ７ ６ ５ ４ ３ ２ １");
                result.Add("+---------------------------+");
                result.AddRange(
                    Enumerable.Range(1, Board.BoardSize)
                        .Select(_ => RankToBod(board, _)));
                result.Add("+---------------------------+");
                result.Add("先手の持駒：" + HandToBod(board, BWType.Black));
                result.Add("手数＝" + board.MoveCount);

                if (board.Turn == BWType.White)
                {
                    result.Add("後手番");
                }
            }

            return string.Join("\n", result);
        }

        /// <summary>
        /// 局面の各段をbod形式に直します。
        /// </summary>
        private static string RankToBod(Board board, int rank)
        {
            var sb = new StringBuilder();
            sb.Append("|");

            // ９筋が一番左で、１筋は右になります。
            for (var file = Board.BoardSize; file >= 1; --file)
            {
                sb.Append(KifUtil.PieceToStr(board[file, rank]));
            }

            sb.Append("|");
            sb.Append(IntConverter.Convert(NumberType.Kanji, rank));

            return sb.ToString();
        }

        /// <summary>
        /// 持ち駒を文字列に直します。
        /// </summary>
        private static string HandToBod(Board board, BWType turn)
        {
            var list =
                from pieceType in EnumEx.GetValues<PieceType>()
                let obj = new
                {
                    Piece = new Piece(pieceType, false),
                    Count = board.GetHand(pieceType, turn),
                }
                where obj.Count > 0
                select string.Format(
                    CultureInfo.InvariantCulture, 
                    "{0}{1}{2}　",
                    KifUtil.PieceToChar(obj.Piece),
                    (obj.Count >= 10 ? "十" : ""),
                    (obj.Count == 10 || obj.Count == 1 ? "" :
                        IntConverter.Convert(NumberType.Kanji, obj.Count % 10)));

            var array = list.ToArray();
            return (array.Any() ? string.Join("", array) : "なし");
        }
        #endregion
    }
}
