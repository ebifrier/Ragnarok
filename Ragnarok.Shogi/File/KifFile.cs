using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// kifファイルの読み込み／書き出しなどを行います。
    /// </summary>
    public sealed class KifFile
    {
        /// <summary>
        /// sjisが基本。
        /// </summary>
        private static Encoding DefaultEncoding =
            Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// ヘッダー部分の正規表現
        /// </summary>
        private static readonly Regex HeaderRegex = new Regex(
            @"^(.+?)[：]?\s*(.*?)\s*$",
            RegexOptions.Compiled);

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
            @"^\s*(\d+)\s*[:]?\s*(.*?(?:投了|中断|打|[(]\d\d[)]))(\s+[()\d:/ ]+)?\s*([\+])?\s*$",
            RegexOptions.Compiled);

        /// <summary>
        /// 変化行の正規表現
        /// </summary>
        private static readonly Regex BeginVariationLineRegex = new Regex(
            @"^\s*変化：(\d+)手");

        /// <summary>
        /// 手合割を取得または設定します。
        /// </summary>
        public string Handicap
        {
            get;
            set;
        }

        /// <summary>
        /// ヘッダ部分の情報を取得します。
        /// </summary>
        public Dictionary<string, string> Headers
        {
            get;
            private set;
        }

        /// <summary>
        /// 変化を含んだ指し手の開始ノードを取得します。
        /// </summary>
        public VariationNode RootNode
        {
            get;
            private set;
        }

        /// <summary>
        /// 差し手リストを取得します。
        /// </summary>
        public IEnumerable<Move> MoveList
        {
            get
            {
                for (var node = RootNode; node != null; node = node.NextChild)
                {
                    yield return node.Move;
                }
            }
        }

        #region 読み込み
        private StringReader reader = null;
        private string currentLine;

        /// <summary>
        /// 次の行を読み込みます。
        /// </summary>
        private string ReadNextLine()
        {
            this.currentLine = this.reader.ReadLine();

            return this.currentLine;
        }

        /// <summary>
        /// コメント行かどうか調べます。
        /// </summary>
        private bool IsCommentLine()
        {
            // コメントでは無いけど^^
            if (this.currentLine.IndexOf("手で中断") >= 0)
            {
                return true;
            }

            // コメントでは無いけど^^
            if (this.currentLine.IndexOf("まで") >= 0)
            {
                return true;
            }

            return (
                this.currentLine.Length == 0 ||
                this.currentLine[0] == '#' ||
                this.currentLine[0] == '*');
        }

        /// <summary>
        /// kifファイルのヘッダー部分をパースします。
        /// </summary>
        private void ParseHeader(TextReader reader)
        {
            while (this.currentLine != null)
            {
                if (IsCommentLine())
                {
                    ReadNextLine();
                    continue;
                }

                // 読み飛ばすべき説明行
                if (this.currentLine == "手数----指手---------消費時間--")
                {
                    ReadNextLine();
                    return;
                }

                var m = HeaderRegex.Match(this.currentLine);
                if (!m.Success)
                {
                    // ヘッダ情報がなくなったら、
                    // 差し手の情報が始まります。
                    return;
                }

                var key = m.Groups[1].Value;
                var value = m.Groups[2].Value;
                if (key == "手合割")
                {
                    Handicap = value;
                }
                else
                {
                    Headers[key] = value;
                }

                ReadNextLine();
            }
        }

        /// <summary>
        /// 指し手行をパースします。
        /// </summary>
        private Move ParseMoveLine(string line, out int moveNumber,
                                   out bool hasVariation)
        {
            var m = MoveLineRegex.Match(line);
            if (!m.Success)
            {
                moveNumber = -1;
                hasVariation = false;
                return null;
            }

            moveNumber = int.Parse(m.Groups[1].Value);
            hasVariation = m.Groups[4].Success;

            // 差し手'３六歩(37)'のような形式でパースします。
            var moveText = m.Groups[2].Value;
            if (moveText == "中断")
            {
                return new Move()
                {
                    IsResigned = true,
                };
            }

            var move = ShogiParser.ParseMove(moveText, false);
            if (move == null || !move.Validate())
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0}手目: 差し手が正しくありません。",
                        m.Groups[1].Value));
            }
            
            return move;
        }

        /// <summary>
        /// ひとかたまりになっている一連の変化をパースします。
        /// </summary>
        private List<Move> ParseVariationList(out int variationBeginNumber,
                                              out HashSet<int> variationNumSet)
        {
            while (IsCommentLine())
            {
                ReadNextLine();
            }

            // 新しい変化が始まる場合は、
            // 　変化 ○手目
            // という行から始まります。
            var m = BeginVariationLineRegex.Match(this.currentLine);
            if (m.Success)
            {
                variationBeginNumber = int.Parse(m.Groups[1].Value);
                ReadNextLine();
            }
            else
            {
                variationBeginNumber = -1;
            }

            var result = new List<Move>();
            variationNumSet = new HashSet<int>();

            while (this.currentLine != null)
            {
                if (IsCommentLine())
                {
                    ReadNextLine();
                    continue;
                }

                // 指し手行を１行だけパースします。
                var moveNumber = -1;
                var hasVariation = false;
                var move = ParseMoveLine(this.currentLine, out moveNumber,
                                         out hasVariation);
                if (move == null)
                {
                    break;
                }

                // 投了はスキップします。
                if (move.IsResigned)
                {
                    ReadNextLine();
                    continue;
                }

                // 変化の開始行がない場合は、指し手の番号をそれとします。
                if (variationBeginNumber < 0)
                {
                    variationBeginNumber = moveNumber;
                }

                // 変化がある場合は、その手数を記録します。
                if (hasVariation)
                {
                    variationNumSet.Add(moveNumber);
                }

                result.Add(move);
                ReadNextLine();
            }

            return result;
        }

        /// <summary>
        /// 変化を既存のノードに追加します。
        /// </summary>
        private void MergeVariation(VariationNode node, VariationNode source)
        {
            while (node.MoveCount != source.MoveCount)
            {
                if (node.NextChild == null)
                {
                    return;
                }

                node = node.NextChild;
            }

            // 変化を追加します。
            while (node.NextVariation != null)
            {
                node = node.NextVariation;
            }

            node.NextVariation = source;
        }

        /// <summary>
        /// 指し手リストをツリー形式に変換します。
        /// </summary>
        private VariationNode ConvertNode(List<Move> moveList, int beginNumber)
        {
            VariationNode root = null;

            moveList.Reverse();
            beginNumber += moveList.Count();

            foreach (var move in moveList)
            {
                var node = new VariationNode()
                {
                    MoveCount = --beginNumber,
                    Move = move,
                    NextChild = root,
                };

                root = node;
            }

            return root;
        }

        /// <summary>
        /// 変化をパースします。
        /// </summary>
        private VariationNode ParseVariationNode()
        {
            var numSet = new HashSet<int>();
            var moveNumber = -1;

            // 後続する変化の手数も取得します。
            // これがないとどれだけ変化をパースすればいいのか分かりません。
            var moveList = ParseVariationList(out moveNumber, out numSet);
            if (!moveList.Any())
            {
                return null;
            }

            var root = ConvertNode(moveList, moveNumber);

            // 以下、変化リストをパースします。
            while (numSet.Any())
            {
                var node = ParseVariationNode();
                if (!numSet.Remove(node.MoveCount))
                {
                    throw new InvalidDataException(
                        string.Format(
                            "{0}手目に対応する変化がありません。",
                            node.MoveCount));
                }

                MergeVariation(root, node);
            }

            return root;
        }

        /// <summary>
        /// ファイル名から棋譜ファイルを読み込みます。
        /// </summary>
        public void LoadFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("filepath");
            }

            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                var text = Util.ReadToEnd(stream, DefaultEncoding);

                LoadFrom(text);
            }
        }

        /// <summary>
        /// ファイル内容から棋譜ファイルを読み込みます。
        /// </summary>
        public void LoadFrom(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            this.reader = new StringReader(text);
            this.currentLine = null;

            ReadNextLine();
            ParseHeader(reader);

            if (!string.IsNullOrEmpty(Handicap) &&
                Handicap != "平手")
            {
                throw new NotSupportedException(
                    "手合割は平手にしか対応していません。");
            }

            RootNode = ParseVariationNode();

            this.reader.Close();
            this.reader = null;
        }

        /// <summary>
        /// 現在読み込まれている差し手から、局面を作成します。
        /// </summary>
        public Board CreateBoard()
        {
            var board = new Board();

            var boardMoveList = board.ConvertMove(MoveList);
            if (boardMoveList.Count != MoveList.Count())
            {
                throw new InvalidDataException(
                    string.Format(
                        "{0}手目: 差し手が正しくないため、局面の作成に失敗しました。",
                        boardMoveList.Count + 1));
            }

            var num = 1;
            foreach(var boardMove in boardMoveList)
            {
                if (!board.DoMove(boardMove))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "{0}手目: 差し手が正しくありません。",
                            num));
                }

                num += 1;
            }

            return board;
        }
        #endregion

        #region 書き込み
        /// <summary>
        /// ヘッダ部分を出力します。
        /// </summary>
        private void WriteHeader(StreamWriter writer)
        {
            writer.WriteLine("# ----  投票将棋 棋譜ファイル  ----");
            writer.WriteLine("手合割：{0}", (Handicap ?? "平手"));

            foreach (var header in this.Headers)
            {
                writer.WriteLine("{0}：{1}", header.Key, header.Value);
            }

            writer.WriteLine("手数----指手---------消費時間--");
        }

        /// <summary>
        /// ファイルに出力するための差し手情報を取得します。
        /// </summary>
        private List<Move> CreateMoveList(Board board)
        {
            // 差し手を移動前情報を含めて変換します。
            var moveList = board.MakeMoveList(0, true);

            // kif形式では、不成りは何も書きません。
            foreach (var move in moveList)
            {
                if (move.ActionType == ActionType.Unpromote)
                {
                    move.ActionType = ActionType.None;
                }
            }

            return moveList;
        }

        /// <summary>
        /// 指し手を出力します。
        /// </summary>
        private string MakeMoveLine(Move move, int number, bool hasVariation)
        {
            // 半角文字相当の文字数で空白を計算します。
            var moveText = Stringizer.ToString(move, MoveTextStyle.KifFile);
            var hanLen = moveText.HankakuLength();

            return string.Format("{0,4} {1}{2} ( 0:00/00:00:00){3}",
                number,
                moveText,
                new string(' ', Math.Max(0, 14 - hanLen)),
                (hasVariation ? "+" : ""));
        }

        /// <summary>
        /// 差し手部分を出力します。
        /// </summary>
        private void WriteMoveList(StreamWriter writer, IEnumerable<Move> moveList)
        {
            var num = 1;

            foreach (var move in moveList)
            {
                var line = MakeMoveLine(move, num, false);

                writer.WriteLine(line);
                num += 1;
            }
        }

        /// <summary>
        /// 局面と差し手をファイルに保存します。
        /// </summary>
        public void SaveFile(string filepath, Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            // まず最初に出力用の差し手を作成します。
            var moveList = CreateMoveList(board);

            using (var stream = new FileStream(filepath, FileMode.Create))
            using (var writer = new StreamWriter(stream, DefaultEncoding))
            {
                WriteHeader(writer);
                WriteMoveList(writer, moveList);
            }
        }

        /// <summary>
        /// 変化の分岐を含めて出力します。
        /// </summary>
        private void WriteMoveNode(StreamWriter writer, VariationNode node)
        {
            if (node == null || node.Move == null)
            {
                return;
            }

            // とりあえず指し手を書きます。
            var line = MakeMoveLine(
                node.Move,
                node.MoveCount,
                (node.NextVariation != null));
            writer.WriteLine(line);

            // 次の指し手があればそれも出力します。
            if (node.NextChild != null)
            {
                WriteMoveNode(writer, node.NextChild);
            }

            var child = node.NextVariation;
            if (child != null)
            {
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine("変化：{0}手", child.MoveCount);

                WriteMoveNode(writer, child);
            }
        }

        /// <summary>
        /// 局面と差し手をファイルに保存します。
        /// </summary>
        /// <remarks>
        /// <paramref name="board"/>は初期盤面の決定にのみ使用されます。
        /// </remarks>
        public void SaveFile(string filepath, VariationNode root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            using (var stream = new FileStream(filepath, FileMode.Create))
            using (var writer = new StreamWriter(stream, DefaultEncoding))
            {
                WriteHeader(writer);
                WriteMoveNode(writer, root);
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifFile()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}
