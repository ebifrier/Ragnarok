using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.File
{
    /// <summary>
    /// ki2ファイルの読み込み／書き出しなどを行います。
    /// </summary>
    internal sealed class Ki2Reader : IKifuReader
    {
        /// <summary>
        /// ヘッダー部分の正規表現
        /// </summary>
        private static readonly Regex HeaderRegex = new Regex(
            @"^(.+)\s*[：:]\s*(.*?)\s*$",
            RegexOptions.Compiled);

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
            return (
                this.currentLine.Length == 0 ||
                this.currentLine[0] == '#' ||
                this.currentLine[0] == '*');
        }

        /// <summary>
        /// kifファイルのヘッダー部分をパースします。
        /// </summary>
        private Dictionary<string, string> ParseHeader(TextReader reader)
        {
            var header = new Dictionary<string, string>();

            while (this.currentLine != null)
            {
                if (IsCommentLine())
                {
                    ReadNextLine();
                    continue;
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
        private IEnumerable<Move> ParseMoveLine(string line)
        {
            while (!Util.IsWhiteSpaceOnly(line))
            {
                var parsedLine = string.Empty;
                var move = ShogiParser.ParseMoveEx(
                    line, false, false, ref parsedLine);

                if (move == null)
                {
                    throw new InvalidOperationException(
                        "ki2形式のファイルが間違っています。");
                }

                line = line.Substring(parsedLine.Length);
                yield return move;
            }
        }

        /// <summary>
        /// 複数の指し手行をパースします。
        /// </summary>
        private IEnumerable<Move> ParseMoveLines()
        {
            while (this.currentLine != null)
            {
                if (IsCommentLine())
                {
                    ReadNextLine();
                    continue;
                }

                // "まで81手で先手の勝ち"などの行が入ることがある。
                var c = this.currentLine[0];
                if (c != '△' && c != '▲')
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
            var moveList = ParseMoveLines().ToList();

            return new KifuObject(header, moveList);
        }
    }
}
