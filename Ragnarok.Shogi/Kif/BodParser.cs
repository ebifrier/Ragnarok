using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Ragnarok.Utility;

namespace Ragnarok.Shogi.Kif
{
    /// <summary>
    /// bod形式の局面を読み取ります。
    /// </summary>
    /// <remarks>
    /// .kifや.ki2ファイルの局面も読み取れるようにします。
    /// </remarks>
    public sealed class BodParser
    {
        private readonly Board board;
        private bool turnHandled;
        private int state;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BodParser()
        {
            // 駒・持駒、共になしの状態で初期化します。
            this.board = new Board(false);
        }

        /// <summary>
        /// 読み込んだ局面を取得します。
        /// </summary>
        public Board Board
        {
            get { return (IsCompleted ? this.board : new Board()); }
        }

        /// <summary>
        /// パースが完了したか取得します。
        /// </summary>
        public bool IsCompleted
        {
            get { return (this.state == 12); }
        }

        /// <summary>
        /// 局面のパースが行われているか取得します。
        /// </summary>
        public bool IsBoardParsing
        {
            get { return (this.state != 0 && this.state != 12); }
        }

        /// <summary>
        /// 局面を示す各行の読み込みを試行します。
        /// </summary>
        public bool TryParse(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException("line");
            }

            if (IsBoardParsing)
            {
                ParseBoardLine(line);
                return true;
            }

            // 局面の解析
            if (line.Contains("９ ８ ７ ６ ５ ４ ３ ２ １"))
            {
                this.state = 1;
                return true;
            }

            // 手数
            if (Regex.IsMatch(line, @"^\s*手数＝"))
            {
                // 手数は無視します。
                return true;
            }

            // 手番
            if (Regex.IsMatch(line, @"^\s*(下手|先手)番"))
            {
                SetTurn(BWType.Black);
                return true;
            }

            if (Regex.IsMatch(line, @"^\s*(上手|後手)番"))
            {
                SetTurn(BWType.White);
                return true;
            }

            // ヘッダの解析
            var item = KifUtil.ParseHeaderItem(line);
            if (item != null)
            {
                if (item.Key.Contains("上手") || item.Key.Contains("下手"))
                {
                    SetHandicapTurn();
                }

                // 持駒の解析
                if (item.Key == "先手の持駒" || item.Key == "下手の持駒")
                {
                    ParseHand(BWType.Black, item.Value);
                    return true;
                }

                if (item.Key == "後手の持駒" || item.Key == "上手の持駒")
                {
                    ParseHand(BWType.White, item.Value);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 駒落ち局面の場合、デフォルトの手番は後手になります。
        /// </summary>
        /// <remarks>
        /// 駒落ち局面を扱っている節があれば、手番を後手に設定します。
        /// 
        /// ただし、明示的に手番を設定する場合もあるので、
        /// 一度しか設定しないようにします。
        /// </remarks>
        private void SetHandicapTurn()
        {
            if (!this.turnHandled)
            {
                this.board.Turn = BWType.White;
                this.turnHandled = true;
            }
        }

        /// <summary>
        /// 強制的に手番を設定します。
        /// </summary>
        private void SetTurn(BWType bwType)
        {
            this.board.Turn = bwType;
            this.turnHandled = true;
        }

        /// <summary>
        /// bod形式の局面を読み取ります。
        /// </summary>
        /// <example>
        /// 局面表現は以下のようになっています。
        /// 
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
        /// </example>
        private void ParseBoardLine(string line)
        {
            if (this.state == 1 || this.state == 11)
            {
                // 盤面の上下を示すサインを読み込みます。
                if (!line.Contains("+---------------------------+"))
                {
                    throw new ShogiException(
                        "局面が正しくありません。");
                }

                this.state += 1;
            }
            else
            {
                var rank = this.state - 1;

                // 局面の各段を読み込みます。
                if (line.Length < 21)
                {
                    throw new ShogiException(
                        string.Format(
                            "局面の{0}段目が正しくありません。",
                            rank));
                }

                for (var file = 1; file <= Board.BoardSize; ++file)
                {
                    this.board[file, rank] = ParsePiece(file, rank, line);
                }

                // 行の最後につく'|二'などの記号はチェックしません。
                this.state += 1;
            }
        }

        /// <summary>
        /// bod形式の各駒を読み取ります。
        /// </summary>
        private BoardPiece ParsePiece(int file, int rank, string line)
        {
            var index = (Board.BoardSize - file) * 2 + 1;
            var pieceStr = line.Substring(index, 2);

            var piece = KifUtil.StrToPiece(pieceStr);
            if (piece == null)
            {
                throw new ShogiException(
                    string.Format(
                        "局面の{0}段目の駒'{1}'が正しくありません。",
                        rank, pieceStr));
            }

            if (piece.PieceType == PieceType.None)
            {
                return null;
            }

            return piece;
        }

        /// <summary>
        /// 持ち駒を読み込みます。
        /// </summary>
        /// <example>
        /// 後手の持駒：飛　角　金　銀　桂　香　歩四　
        /// </example>
        private void ParseHand(BWType bwType, string handText)
        {
            if (handText == "なし")
            {
                return;
            }

            // 持ち駒をまとめて設定します。
            handText
                .Split(new char[] { '　', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(_ => ParseHandPiece(bwType, _))
                .ForEach(_ => this.board.SetCapturedPieceCount(_.Item1, bwType, _.Item2));
        }

        /// <summary>
        /// 持ち駒の各駒をパースします。
        /// </summary>
        /// <remarks>
        /// 各駒文字を最初の漢字で表し、後に続く漢数字でその数を示します。
        /// </remarks>
        private Tuple<PieceType, int> ParseHandPiece(BWType bwType,
                                                     string handPieceText)
        {
            // 駒の種類を取得します。
            var piece = KifUtil.CharToPiece(handPieceText[0]);
            if (piece == null)
            {
                throw new ShogiException(
                    string.Format(
                        "{0}手の持ち駒'{1}'が正しくありません。",
                        bwType == BWType.Black ? "先" : "後",
                        handPieceText));
            }

            if (piece.IsPromoted)
            {
                throw new ShogiException(
                    string.Format(
                        "{0}手の持ち駒に成り駒があります。",
                        bwType == BWType.Black ? "先" : "後"));
            }

            // 駒の数指定があれば、それを解析します。
            int count = 1;
            if (handPieceText.Length > 1)
            {
                var numText = handPieceText.Substring(1);
                var normalized = StringNormalizer.NormalizeNumber(numText, true);
                
                count = int.Parse(normalized);
            }

            return Tuple.Create(piece.PieceType, count);
        }
    }
}
