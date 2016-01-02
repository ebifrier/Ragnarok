using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.Kif
{
    using File;

    /// <summary>
    /// kif, ki2, bod形式のファイルの読み込みを行います。
    /// </summary>
    internal sealed class KifReader : IKifuReader
    {
        /// <summary>
        /// kif形式の指し手行の正規表現
        /// </summary>
        /// <example>
        ///  81 同　飛成(62) ( 0:01/00:00:33)
        /// 112 ４四飛打
        /// 102 投了
        ///  18 ４二玉(51)   ( 0:30/00:01:53)+
        ///   9: ４五飛
        /// </example>
        private static readonly Regex MoveLineRegex = new Regex(
            string.Format(
                @"^\s*(\d+)\s*[:]?\s*(.*?(?:{0}|打|[(]\d\d[)])?)(\s+[(]?\s*([\s\d:/sS]+)\s*[)]?)?\s*([\+])?\s*$",
                KifUtil.SpecialMoveText),
            RegexOptions.Compiled);

        /// <summary>
        /// 変化行の正規表現
        /// </summary>
        private static readonly Regex BeginVariationLineRegex = new Regex(
            @"^\s*変化\s*：\s*(\d+)手");

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
                // 最後の空白文字や改行文字は削除します。
                this.currentLine = this.currentLine.TrimEnd('\n', '\r');
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

            return KifUtil.IsCommentLine(line);
        }

        /// <summary>
        /// コメント行を読み込み、それをノードに設定します。
        /// </summary>
        /// <remarks>
        /// コメント行は複数行に渡ることがあるため、
        /// コメント行がある間はずっとコメントを追加し続けます。
        /// </remarks>
        private void ReadCommentLines(KifMoveNode node, bool forceOneLine)
        {
            var alreadyRead = false;

            while (this.currentLine != null)
            {
                var commentData = KifUtil.ParseCommentLine(this.currentLine);
                if (commentData == null)
                {
                    if (forceOneLine && !alreadyRead)
                    {
                        ReadNextLine();
                    }

                    break;
                }

                // 必要なコメントのみ棋譜から取り出します。
                if (commentData.IsMoveComment)
                {
                    node.AddComment(commentData.Comment);
                }

                ReadNextLine();
                alreadyRead = true;
            }
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
                                     KifuHeader header = null, KifMoveNode head = null)
        {
            if (line == null)
            {
                // ファイルの終了を意味します。
                return false;
            }

            var commentData = KifUtil.ParseCommentLine(line);
            if (commentData != null)
            {
                // コメントはパース結果に含めます。
                if (head != null && commentData.IsMoveComment)
                {
                    head.AddComment(commentData.Comment);
                }

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
                    // 可能ならヘッダアイテムを設定します。
                    var type = KifUtil.GetHeaderType(item.Key);
                    if (type != null)
                    {
                        header[type.Value] = item.Value;
                    }
                }

                return true;
            }

            // ヘッダが正しく読めない場合、
            // 区切りなしに指し手行に入っている可能性があります。
            if (MoveLineRegex.IsMatch(line))
            {
                this.isKif = true;
            }

            return false;
        }

        /// <summary>
        /// ヘッダー部分をまとめてパースします。
        /// </summary>
        /// <remarks>
        /// 開始局面の設定も行います。
        /// </remarks>
        private KifuHeader ParseHeader(KifMoveNode head)
        {
            var header = new KifuHeader();
            var parser = new BodParser();

            while (ParseHeaderLine(this.currentLine, parser, header, head))
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
        private KifuObject LoadKi2(KifuHeader header, Board startBoard, KifMoveNode head)
        {
            var knodeList = ParseMoveLinesKi2().ToList();
            var last = head;
            foreach (var knode in knodeList)
            {
                last.NextNode = knode;
                last = knode;
            }

            // KifNodeMoveのツリーをNodeMoveのツリーに直します。
            // 変換時にエラーが出た場合は、それまでの指し手を変換します。
            var board = startBoard.Clone();
            Exception error = null;
            var root = head.NextNode.ConvertToMoveNode(board, head, out error);
            root.Regulalize();

            return new KifuObject(header, startBoard, root, error);
        }

        /// <summary>
        /// Ki2形式の複数の指し手をパースします。
        /// </summary>
        private IEnumerable<KifMoveNode> ParseMoveLinesKi2()
        {
            while (this.currentLine != null)
            {
                var smove = KifUtil.ParseSpecialMove(this.currentLine);
                if (smove != null)
                {
                    yield return new KifMoveNode
                    {
                        Move = smove,
                    };
                }

                var c = this.currentLine[0];
                if (c != '▲' && c != '△' && c != '▼' && c != '▽')
                {
                    ReadNextLine();
                    continue;
                }

                var list = ParseMoveLineKi2(this.currentLine);
                foreach (var move in list)
                {
                    yield return new KifMoveNode
                    {
                        Move = move,
                    };
                }

                ReadNextLine();
            }
        }

        /// <summary>
        /// 指し手行をパースします。
        /// </summary>
        private IEnumerable<Move> ParseMoveLineKi2(string line)
        {
            while (!Util.IsWhiteSpaceOnly(line))
            {
                var parsedLine = string.Empty;
                var move = ShogiParser.ParseMoveEx(
                    line, false, ref parsedLine);

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
        private KifuObject LoadKif(KifuHeader header, Board board, KifMoveNode head_)
        {
            var head = ParseNodeKif(board, false);
            head.SetupVariationInfo(board);

            // KifMoveNodeからMoveNodeへ変換します。
            Exception error;
            var root = head.ConvertToMoveNode(board, head_, out error);
            root.Regulalize();

            return new KifuObject(header, board, root, error);
        }

        /// <summary>
        /// 変化や指し手をパースします。
        /// </summary>
        private KifMoveNode ParseNodeKif(Board board, bool isVariation)
        {
            // 後続する変化の手数も取得します。
            // これがないとどれだけ変化をパースすればいいのか分かりません。
            var head = ParseMoveLinesKif(isVariation);
            if (head == null)
            {
                return null;
            }

            // 以下、変化リストをパースします。
            var variationLineSet = MakeVariationLineSet(head);
            while(variationLineSet.Any())
            {
                var variationNode = ParseNodeKif(board, true);
                if (variationNode == null)
                {
                    continue;
                }

                if (!variationLineSet.Contains(variationNode.MoveCount))
                {
                    Log.Error(
                        "{0}行目: {1}手目に対応する変化がありません。",
                        this.lineNumber, variationNode.MoveCount);

                    /*throw new FileFormatException(
                        this.lineNumber,
                        string.Format(
                            "{0}手目に対応する変化がありません。",
                            variationNode.MoveCount));*/
                }

                variationLineSet.Remove(variationNode.MoveCount);

                // 変化を棋譜にマージします。
                MergeVariation(head, variationNode);
            }

            return head;
        }

        /// <summary>
        /// ノードの中に含まれる'変化がある指し手番号のセット'を作成します。
        /// </summary>
        private HashSet<int> MakeVariationLineSet(KifMoveNode head)
        {
            var result = new HashSet<int>();

            for (var node = head; node != null; node = node.NextNode)
            {
                if (node.HasVariationNode)
                {
                    result.Add(node.MoveCount);
                }
            }

            return result;
        }

        /// <summary>
        /// 変化を既存のノードに追加します。
        /// </summary>
        private void MergeVariation(KifMoveNode node, KifMoveNode source)
        {
            while (node.MoveCount != source.MoveCount)
            {
                if (node.NextNode == null)
                {
                    //throw 
                    return;
                }

                node = node.NextNode;
            }

            for (var next = node; next != null; next = next.VariationNode)
            {
                if (next == null)
                {
                    break;
                }

                node = next;
            }

            // 変化を追加します。
            node.VariationNode = source;
        }

        /// <summary>
        /// ひとかたまりになっている一連の指し手をパースします。
        /// </summary>
        private KifMoveNode ParseMoveLinesKif(bool isVariation)
        {
            var head = new KifMoveNode();
            var last = head;

            // もし変化を読み取る場合は、開始記号を探します。
            if (isVariation)
            {
                while (this.currentLine != null)
                {
                    // 新しい変化が始まる場合は、
                    // 　変化 ○手目
                    // という行から始まります。
                    var m = BeginVariationLineRegex.Match(this.currentLine);
                    if (m.Success)
                    {
                        head.MoveCount = int.Parse(m.Groups[1].Value);
                        ReadNextLine();
                        break;
                    }

                    // 複数のコメント行を読み込みます。
                    ReadCommentLines(head, true);
                }
            }

            while (this.currentLine != null)
            {
                // 指し手行を１行だけパースします。
                var node = ParseMoveLineKif(this.currentLine);
                if (node == null)
                {
                    break;
                }

                ReadNextLine();

                // 指し手の後に続く複数のコメント行を読み込みます。
                ReadCommentLines(node, false);

                // 指し手はリンクリストで保存します。
                last.NextNode = node;
                last = node;
            }

            return head.NextNode;
        }
        
        /// <summary>
        /// 指し手行をパースします。
        /// </summary>
        private KifMoveNode ParseMoveLineKif(string line)
        {
            var m = MoveLineRegex.Match(line);
            if (!m.Success)
            {
                return null;
            }

            var moveCount = int.Parse(m.Groups[1].Value);
            var hasVariation = m.Groups[5].Success;

            // 指し手'３六歩(37)'のような形式でパースします。
            var moveText = m.Groups[2].Value;
            var move = ShogiParser.ParseMove(moveText, false);
            if (move == null || !move.Validate())
            {
                throw new FileFormatException(
                    this.lineNumber,
                    string.Format(
                        "{0}手目: 指し手が正しくありません。({1})",
                        m.Groups[1].Value, moveText));
            }

            // 時間を取得します。
            var duration = ParseDuration(m.Groups[4].Value);

            return new KifMoveNode
            {
                Move = move,
                MoveCount = moveCount,
                Duration = duration,
                LineNumber = this.lineNumber,
                HasVariationNode = hasVariation,
            };
        }

        private static readonly string TimeFormat =
            @"(?:(\d+)\s*\:\s*)?(\d+)\s*\:\s*(\d+)";
        private static readonly Regex DurationRegex1 = new Regex(
            string.Format(@"{0}\s*(?:/\s*{0})?", TimeFormat),
            RegexOptions.Compiled);
        private static readonly Regex DurationRegex2 = new Regex(
            @"(\d+)\s*[sS]?",
            RegexOptions.Compiled);

        /// <summary>
        /// 着手時間を取得します。
        /// </summary>
        private TimeSpan ParseDuration(string input)
        {
            var m = DurationRegex1.Match(input);
            if (m.Success)
            {
                var hourStr = m.Groups[1].Success ? m.Groups[1].Value : "0";
                return new TimeSpan(
                    int.Parse(hourStr),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(m.Groups[3].Value));
            }

            m = DurationRegex2.Match(input);
            if (m.Success)
            {
                return TimeSpan.FromSeconds(
                    int.Parse(m.Groups[1].Value));
            }

            return TimeSpan.Zero;
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

                // ヘッダを1行読めれば、kif or ki2 or bod 形式の
                // どれかのファイルとします。
                var parser = new BodParser();
                for (var i = 0; i < 1; ++i)
                {
                    var line = ReadNextLine();

                    if (line == null)
                    {
                        return true;
                    }

                    if (!ParseHeaderLine(line, parser))
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
            var head = new KifMoveNode();
            var header = ParseHeader(head);
            var board = this.startBoard;

            // 指し手の読み取りに入ります。
            if (this.currentLine == null)
            {
                // ファイルが終わっている場合はbod形式
                return new KifuObject(header, board);
            }
            else if (this.isKif)
            {
                // kif形式
                return LoadKif(header, board, head);
            }
            else
            {
                // ki2形式
                return LoadKi2(header, board, head);
            }
        }
    }
}
