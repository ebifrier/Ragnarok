using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.Csa
{
    using File;

    /// <summary>
    /// kifファイルの読み込みを行います。
    /// </summary>
    internal sealed class CsaReader : IKifuReader
    {
        private TextReader reader;
        private string[] currentLines;
        private int currentLineIndex;
        private int lineNumber;
        private KifuHeader header;
        private Board startBoard;
        private Board board;
        private MoveNode rootNode;
        private MoveNode lastNode;

        /// <summary>
        /// 次の行を読み込みます。
        /// </summary>
        /// <remarks>
        /// CSAファイルは','で、行を区切ることができるため、
        /// その対策も入れています。
        /// </remarks>
        private string ReadNextLine()
        {
            // 次の行を読み込みます。
            while (this.currentLines == null ||
                   this.currentLineIndex >= this.currentLines.Count())
            {
                var line = this.reader.ReadLine();
                if (line == null)
                {
                    // ファイル終了
                    return null;
                }

                this.currentLines = line.Split(',');
                this.currentLineIndex = 0;
                this.lineNumber += 1;
            }

            var split = this.currentLines[this.currentLineIndex++];
            return split.TrimStart(' ', '\t');
        }

        /// <summary>
        /// ヘッダー部分をまとめてパースします。
        /// </summary>
        /// <remarks>
        /// 開始局面の設定も行います。
        /// </remarks>
        private void Parse()
        {
            var parser = new CsaBoardParser();
            while (ParseLine(ReadNextLine(), parser))
            {
            }

            if (parser.IsBoardParsing)
            {
                throw new FileFormatException(
                    "局面の読み取りを完了できませんでした。");
            }
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
                    this.startBoard = parser.Board.Clone();
                    this.board = this.startBoard.Clone();
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

                case '+': case '-':
                case '%':
                    var move = ParseMove(line);
                    var node = new MoveNode
                    {
                        Move = move,
                    };

                    this.lastNode.AddNextNode(node);
                    this.lastNode = node;
                    return true;

                case 'T':
                    var seconds = ParseTime(line);
                    if (this.lastNode != null)
                    {
                        this.lastNode.DurationSeconds = seconds;
                    }
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
                throw new FileFormatException(
                    this.lineNumber,
                    line + ": CSAファイル形式が正しくありません。");
            }

            if (line[1] == '+')
            {
                this.header[KifuHeaderType.BlackName] = line.Substring(2);
            }
            else if (line[1] == '-')
            {
                this.header[KifuHeaderType.WhiteName] = line.Substring(2);
            }
            else
            {
                throw new FileFormatException(
                    this.lineNumber,
                    line + ": 対局者名が先手でも後手でもありません。");
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
                throw new FileFormatException(
                    this.lineNumber,
                    line + ": CSA形式のヘッダ行が正しくありません。");
            }

            var type = CsaUtil.GetHeaderType(item.Key);
            if (type != null)
            {
                this.header[type.Value] = item.Value;
            }
        }

        /// <summary>
        /// 指し手行をパースします。
        /// </summary>
        private BoardMove ParseMove(string line)
        {
            if (this.board == null)
            {
                throw new FileFormatException(
                    this.lineNumber,
                    "指し手の前に局面が設定されていません。");
            }

            var move = this.board.CsaToMove(line);
            if (move == null)
            {
                throw new FileFormatException(
                    this.lineNumber,
                    line + ": CSA形式の指し手が正しく読み込めません。");
            }

            if (!this.board.DoMove(move))
            {
                throw new FileFormatException(
                    this.lineNumber,
                    line + ": CSA形式の指し手を正しく指せません。");
            }

            return move;
        }

        /// <summary>
        /// 思考時間をパースします。
        /// </summary>
        private int ParseTime(string line)
        {
            var sub = line.Substring(1);
            int result;

            if (!int.TryParse(sub, out result))
            {
                throw new FileFormatException(
                    this.lineNumber,
                    line + ": CSA形式の消費時間を正しく取得できません。");
            }

            return result;
        }

        /// <summary>
        /// このReaderで与えられたファイルを処理できるか調べます。
        /// </summary>
        public bool CanHandle(TextReader reader)
        {
            try
            {
                this.reader = reader;
                this.currentLines = null;
                this.currentLineIndex = 0;
                this.lineNumber = 0;

                this.header = new KifuHeader();
                this.startBoard = null;
                this.board = null;
                this.rootNode = new MoveNode();
                this.lastNode = this.rootNode;

                // ファイルを３行読めれば、CSA形式であると判断します。
                var parser = new CsaBoardParser();
                for (var i = 0; i < 3; ++i)
                {
                    var line = ReadNextLine();

                    if (line == null)
                    {
                        return true;
                    }

                    if (!ParseLine(line, parser))
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
        /// ファイルの読み込み処理を行います。
        /// </summary>
        private KifuObject LoadCsa()
        {
            // ヘッダーや局面の読み取り
            Parse();           
            this.rootNode.Regulalize();

            return new KifuObject(this.header, this.startBoard, this.rootNode);
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
            this.currentLines = null;
            this.currentLineIndex = 0;
            this.lineNumber = 0;

            this.header = new KifuHeader();
            this.startBoard = null;
            this.board = null;
            this.rootNode = new MoveNode();
            this.lastNode = this.rootNode;

            return LoadCsa();
        }
    }
}
