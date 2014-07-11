using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.File
{
    using Kif;

    /// <summary>
    /// kif, ki2, bod形式のファイルの読み込みを行います。
    /// </summary>
    internal sealed class KifReader : IKifuReader
    {
        /// <summary>
        /// kif形式の差し手行の正規表現
        /// </summary>
        /// <example>
        ///  81 同　飛成(62) ( 0:01/00:00:33)
        /// 112 ４四飛打
        /// 102 投了
        ///  18 ４二玉(51)   ( 0:30/00:01:53)+
        /// </example>
        private static readonly Regex MoveLineRegex = new Regex(
            string.Format(
                @"^\s*(\d+)\s*[:]?\s*(.*?(?:{0}|打|[(]\d\d[)]))(\s+[()\d:/ ]+)?\s*([\+])?\s*$",
                KifUtil.SpecialMoveText),
            RegexOptions.Compiled);

        /// <summary>
        /// 変化行の正規表現
        /// </summary>
        private static readonly Regex BeginVariationLineRegex = new Regex(
            @"^\s*変化：(\d+)手");

        private TextReader reader;
        private string currentLine;
        private int lineNumber;
        private Board startBoard;
        private bool isKif;

        #region ReadLine
        /// <summary>
        /// 次の行を読み込みます。
        /// </summary>
        private string ReadNextLine()
        {
            this.currentLine = this.reader.ReadLine();
            if (this.currentLine != null)
            {
                this.currentLine = this.currentLine.Trim();
            }

            this.lineNumber += 1;
            return this.currentLine;
        }

        /// <summary>
        /// コメント行かどうか調べます。
        /// </summary>
        private bool IsCommentLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            // 「まで77手で先手の勝ち」などの特殊コメント
            if (KifUtil.SpecialMoveRegex.IsMatch(line))
            {
                return true;
            }

            return KifUtil.IsCommentLine(line);
        }
        #endregion

        #region Header
        /// <summary>
        /// 開始局面を取得します。
        /// </summary>
        /// <remarks>
        /// 局面はbod形式と"手合割"ヘッダの両方で与えられることがあり、
        /// しかも両者が矛盾していることがあります。
        /// 
        /// そのため、開始局面については優先順位を与えて、
        /// その順序に従った局面を返すことにしています。
        /// 
        /// 具体的な優先順位は
        //   1, bod形式の局面をパースしたもの
        //   2, "手合割"ヘッダで与えられる局面
        //   3, 平手局面
        // となります。
        /// </remarks>
        private Board MakeStartBoard(BodParser parser)
        {
            return (
                parser.IsCompleted ? parser.Board :
                this.startBoard != null ? this.startBoard :
                new Board());
        }

        /// <summary>
        /// ヘッダー行をパースします。
        /// </summary>
        private bool ParseHeaderLine(string line, BodParser parser,
                                     Dictionary<string, string> header = null)
        {
            if (line == null)
            {
                // ファイルの終了を意味します。
                return false;
            }

            if (IsCommentLine(line))
            {
                return true;
            }

            // 読み飛ばすべき説明行
            if (line.Contains("手数----指手---------消費時間"))
            {
                this.isKif = true; // kif形式です。
                return true;
            }

            // 局面の読み取りを試みます。
            if (parser.TryParse(line))
            {
                return true;
            }

            var item = KifUtil.ParseHeaderItem(line);
            if (item != null)
            {
                if (item.Key == "手合割" && item.Value != "その他")
                {
                    this.startBoard = BoardTypeUtil.CreateBoardFromName(item.Value);
                }

                if (header != null)
                {
                    header[item.Key] = item.Value;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// ヘッダー部分をまとめてパースします。
        /// </summary>
        /// <remarks>
        /// 開始局面の設定も行います。
        /// </remarks>
        private Dictionary<string, string> ParseHeader()
        {
            var header = new Dictionary<string, string>();
            var parser = new BodParser();

            while (ParseHeaderLine(this.currentLine, parser, header))
            {
                ReadNextLine();
            }

            if (parser.IsBoardParsing)
            {
                throw new FileFormatException(
                    "局面の読み取りを完了できませんでした。");
            }

            this.startBoard = MakeStartBoard(parser);
            return header;
        }
        #endregion

        #region ki2
        /// <summary>
        /// ki2形式のファイルを読み込みます。
        /// </summary>
        private KifuObject LoadKi2(Dictionary<string, string> header, Board board)
        {
            // 先にリスト化しないと、ConvertMoveでエラーがあった時に
            // ファイルを最後までパースしなくなります。
            var moveList = ParseMoveLines().ToList();
            var bmoveList = board.ConvertMove(moveList);

            Exception error = null;
            if (moveList.Count() != bmoveList.Count())
            {
                if (moveList.Count() - 1 != bmoveList.Count())
                {
                    error = new FileFormatException(
                        string.Format(
                            "{0}手目の'{1}'を正しく処理できませんでした。",
                            bmoveList.Count() + 1,
                            moveList[bmoveList.Count()]));
                }
                else
                {
                    error = new FileFormatException(
                        "最終手が反則で終わっています。");
                }
            }

            return new KifuObject(header, board, bmoveList, error);
        }

        /// <summary>
        /// 複数の指し手行をパースします。
        /// </summary>
        private IEnumerable<Move> ParseMoveLines()
        {
            while (this.currentLine != null)
            {
                if (IsCommentLine(this.currentLine))
                {
                    ReadNextLine();
                    continue;
                }

                var c = this.currentLine[0];
                if (c != '▲' && c != '△' && c != '▼' && c != '▽')
                {
                    ReadNextLine();
                    continue;
                }

                var list = ParseMoveLine(this.currentLine);
                foreach (var move in list)
                {
                    yield return move;
                }

                ReadNextLine();
            }
        }

        /// <summary>
        /// 指し手行をパースします。
        /// </summary>
        private IEnumerable<Move> ParseMoveLine(string line)
        {
            while (!Util.IsWhiteSpaceOnly(line))
            {
                var parsedLine = string.Empty;
                var move = ShogiParser.ParseMoveEx(
                    line, false, false, ref parsedLine);

                if (move == null)
                {
                    throw new FileFormatException(
                        this.lineNumber,
                        "ki2形式の指し手が正しく読み込めません。");
                }

                line = line.Substring(parsedLine.Length);
                yield return move;
            }
        }
        #endregion

        #region kif
        /// <summary>
        /// kif形式の指し手を読み込みます。
        /// </summary>
        private KifuObject LoadKif(Dictionary<string, string> header, Board board)
        {
            var root = ParseNode(board);

            return new KifuObject(header, board, root);
        }

        /// <summary>
        /// 変化や指し手をパースします。
        /// </summary>
        private MoveNode ParseNode(Board board)
        {
            List<Tuple<Board, int>> variationList;

            // 後続する変化の手数も取得します。
            // これがないとどれだけ変化をパースすればいいのか分かりません。
            var root = ParseNode(board.Clone(), out variationList);
            if (root == null)
            {
                return null;
            }

            // 以下、変化リストをパースします。
            variationList.Reverse();
            foreach (var variation in variationList)
            {
                var node = ParseNode(variation.Item1);
                if (node.MoveCount != variation.Item2)
                {
                    throw new FileFormatException(
                        this.lineNumber,
                        string.Format(
                            "{0}手目に対応する変化がありません。",
                            node.MoveCount));
                }

                MergeVariation(root, node);
            }

            return root;
        }

        /// <summary>
        /// 変化を既存のノードに追加します。
        /// </summary>
        private void MergeVariation(MoveNode node, MoveNode source)
        {
            while (node.MoveCount + 1 != source.MoveCount)
            {
                if (node.NextNode == null)
                {
                    return;
                }

                node = node.NextNode;
            }

            // 変化を追加します。
            node.NextNodes.Add(source);
        }

        /// <summary>
        /// ひとかたまりになっている一連の指し手をパースします。
        /// </summary>
        private MoveNode ParseNode(Board board, out List<Tuple<Board, int>> variationList)
        {
            while (IsCommentLine(this.currentLine))
            {
                ReadNextLine();
            }

            // 新しい変化が始まる場合は、
            // 　変化 ○手目
            // という行から始まります。
            var m = BeginVariationLineRegex.Match(this.currentLine);
            if (m.Success)
            {
                ReadNextLine();
            }

            var head = new MoveNode();
            var last = head;
            variationList = new List<Tuple<Board, int>>();

            while (this.currentLine != null)
            {
                if (IsCommentLine(this.currentLine))
                {
                    ReadNextLine();
                    continue;
                }

                // 指し手行を１行だけパースします。
                var hasVariation = false;
                var node = ParseMoveLine(board, this.currentLine,
                                         out hasVariation);
                if (node == null)
                {
                    break;
                }

                // 変化がある場合は、その手数を記録します。
                if (hasVariation)
                {
                    variationList.Add(Tuple.Create(board.Clone(), node.MoveCount));
                }

                // 局面を進めます。
                if (!board.DoMove(node.Move))
                {
                    throw new FileFormatException(
                        this.lineNumber,
                        string.Format(
                            "{0}手目の手を指すことができませんでした。",
                            node.MoveCount));
                }

                last.NextNodes.Add(node);
                last = node;

                ReadNextLine();
            }

            return head.NextNode;
        }
        
        /// <summary>
        /// 指し手行をパースします。
        /// </summary>
        private MoveNode ParseMoveLine(Board board, string line,
                                       out bool hasVariation)
        {
            var m = MoveLineRegex.Match(line);
            if (!m.Success)
            {
                hasVariation = false;
                return null;
            }

            var moveCount = int.Parse(m.Groups[1].Value);
            hasVariation = m.Groups[4].Success;

            // 指し手'３六歩(37)'のような形式でパースします。
            var moveText = m.Groups[2].Value;
            if (KifUtil.SpecialMoveRegex.IsMatch(moveText))
            {
                return null;
            }

            var move = ShogiParser.ParseMove(moveText, false);
            if (move == null || !move.Validate())
            {
                throw new FileFormatException(
                    this.lineNumber,
                    string.Format(
                        "{0}手目: 指し手が正しくありません。",
                        m.Groups[1].Value));
            }

            var bmove = board.ConvertMove(move, true);
            if (bmove == null || !bmove.Validate())
            {
                throw new FileFormatException(
                    this.lineNumber,
                    string.Format(
                        "{0}手目: 指し手が正しくありません。",
                        m.Groups[1].Value));
            }

            return new MoveNode
            {
                MoveCount = moveCount,
                Move = bmove,
            };
        }
        #endregion

        /// <summary>
        /// このReaderで与えられたファイルを処理できるか調べます。
        /// </summary>
        public bool CanHandle(TextReader reader)
        {
            try
            {
                this.reader = reader;
                this.currentLine = null;
                this.lineNumber = 0;
                this.startBoard = null;

                // ヘッダを３行読めれば、kif or ki2 or bod 形式の
                // どれかのファイルとします。
                var parser = new BodParser();
                for (var i = 0; i < 3; ++i)
                {
                    ReadNextLine();
                    if (!ParseHeaderLine(this.currentLine, parser))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// ファイル内容から棋譜ファイルを読み込みます。
        /// </summary>
        public KifuObject Load(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.reader = reader;
            this.currentLine = null;
            this.lineNumber = 0;
            this.startBoard = null;

            // ヘッダーや局面の読み取り
            ReadNextLine();
            var header = ParseHeader();
            var board = this.startBoard;

            // 指し手の読み取りに入ります。
            if (this.currentLine == null)
            {
                // bod形式
                return new KifuObject(header, board);
            }
            else if (this.isKif)
            {
                // kif形式
                return LoadKif(header, board);
            }
            else
            {
                // ki2形式
                return LoadKi2(header, board);
            }
        }
    }
}
