using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.File
{
    using Csa;

    /// <summary>
    /// kifファイルの読み込みを行います。
    /// </summary>
    internal sealed class CsaReader : IKifuReader
    {
        private TextReader reader;
        private string currentLine;
        private int lineNumber;

        public Dictionary<string, string> Header
        {
            get;
            private set;
        }

        public Board Board
        {
            get;
            private set;
        }

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
        /// ヘッダー部分をまとめてパースします。
        /// </summary>
        /// <remarks>
        /// 開始局面の設定も行います。
        /// </remarks>
        private void Parse()
        {
            Header = new Dictionary<string, string>();

            var parser = new CsaBoardParser();
            while (ParseLine(ReadNextLine(), parser))
            {
            }

            if (parser.IsBoardParsing)
            {
                throw new FileFormatException(
                    "局面の読み取りを完了できませんでした。");
            }

            //Board = MakeStartBoard(parser);
        }

        /// <summary>
        /// ヘッダー行をパースします。
        /// </summary>
        private bool ParseLine(string line, CsaBoardParser parser)
        {
            if (line == null)
            {
                // ファイルの終了を意味します。
                return false;
            }

            if (CsaUtil.IsCommentLine(line))
            {
                return true;
            }

            // 局面の読み取りを試みます。
            if (parser.TryParse(line))
            {
                if (parser.HasBoard && !parser.IsBoardParsing)
                {
                    Board = parser.Board.Clone();
                }

                return true;
            }

            switch (line[0])
            {
                case 'V':
                    return true;

                case 'N':
                    ParseName(line);
                    return true;

                case '$':
                    ParseHeader(line);
                    return true;

                case 'T':
                    return true;

                case '+': case '-':
                    ParseMove(line);
                    return true;

                case '%':
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 対局者名をパースします。
        /// </summary>
        private void ParseName(string line)
        {
            if (line.Length < 2)
            {
            }

            if (line[1] == '+')
            {
                Header["先手"] = line.Substring(2);
            }
            else if (line[1] == '-')
            {
                Header["後手"] = line.Substring(2);
            }
            else
            {
            }
        }

        /// <summary>
        /// 棋戦情報などをパースします。
        /// </summary>
        private void ParseHeader(string line)
        {
            var item = CsaUtil.ParseHeaderItem(line);
            if (item == null)
            {
            }

            if (Header != null)
            {
                Header[item.Key] = item.Value;
            }
        }

        private void ParseTime(string line)
        {
        }

        /// <summary>
        /// 指し手行をパースします。
        /// </summary>
        private BoardMove ParseMove(string line)
        {
            var move = Board.CsaToMove(line);
            if (move == null)
            {
                throw new FileFormatException(
                    this.lineNumber,
                    line + ": CSA形式の指し手が正しく読み込めません。");
            }

            /*if (!Board.DoMove(move))
            {
                throw new FileFormatException(
                    this.lineNumber,
                    line + ": CSA形式の指し手を正しく指せません。");
            }*/

            return move;
        }

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
                Header = null;

                // ファイルを３行読めれば、CSA形式であると判断します。
                var parser = new CsaBoardParser();
                for (var i = 0; i < 3; ++i)
                {
                    if (!ParseLine(ReadNextLine(), parser))
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

            // ヘッダーや局面の読み取り
            Parse();

            return null; // new KifuObject(header, board);
        }
    }
}
