using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.File
{
    /// <summary>
    /// kifファイルの読み込みを行います。
    /// </summary>
    internal sealed class KifReader : IKifuReader
    {
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

        private TextReader reader = null;
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
        private Dictionary<string,string> ParseHeader(TextReader reader)
        {
            var header = new Dictionary<string, string>();

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
                    return header;
                }

                var m = HeaderRegex.Match(this.currentLine);
                if (!m.Success)
                {
                    // ヘッダ情報がなくなったら、
                    // 差し手の情報が始まります。
                    return header;
                }

                var key = m.Groups[1].Value;
                var value = m.Groups[2].Value;
                header[key] = value;

                ReadNextLine();
            }

            return header;
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
        private void MergeVariation(MoveNode node, MoveNode source)
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
        /// 変化をパースします。
        /// </summary>
        private MoveNode ParseVariationNode()
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

            var root = KifuObject.Convert2Node(moveList, moveNumber);

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

            ReadNextLine();
            var header = ParseHeader(reader);
            var root = ParseVariationNode();

            return new KifuObject(header, root);
        }
    }
}
