using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Csa
{
    /// <summary>
    /// CSA形式の局面を各行ごとにパースするためのクラスです。
    /// </summary>
    public sealed class CsaBoardParser
    {
        private Board board;
        private int parsedRank;
        private BWType turn;

        /// <summary>
        /// 局面を読み取り中か取得します。
        /// </summary>
        public bool IsBoardParsing
        {
            get
            {
                return ((this.turn == BWType.None) ||
                        (this.parsedRank != 0 && this.parsedRank != 9));
            }
        }

        /// <summary>
        /// パースした局面が存在するかどうかを取得します。
        /// </summary>
        public bool HasBoard
        {
            get { return (this.board != null); }
        }

        /// <summary>
        /// パースした局面を取得します。
        /// </summary>
        public Board Board
        {
            get
            {
                if (this.board == null)
                {
                    throw new ShogiException(
                        "局面が正しく読み込まれていません。");
                }

                if (this.turn == BWType.None)
                {
                    throw new ShogiException(
                        "手番が正しく読み込まれていません。");
                }

                this.board.Turn = this.turn;
                return this.board;
            }
        }

        /// <summary>
        /// CSA形式の局面を１行ごと読み込んでみます。
        /// </summary>
        public bool TryParse(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                throw new ArgumentNullException("line");
            }

            var trimmedLine = line.TrimEnd('\r', '\n');

            // 手番読み取り
            var ch0 = trimmedLine[0];
            if (trimmedLine.Length == 1 && (ch0 == '+' || ch0 == '-'))
            {
                this.turn = (ch0 == '+' ? BWType.Black : BWType.White);
                return true;
            }

            // 各形式の局面読み取り
            if (ch0 == 'P' && trimmedLine.Length >= 2)
            {
                var ch1 = trimmedLine[1];
                if (ch1 == 'I')
                {
                    ParseBoardPI(trimmedLine);
                    return true;
                }

                if (ch1 == '+' || ch1 == '-')
                {
                    ParseBoardP(trimmedLine);
                    return true;
                }

                if ('1' <= ch1 && ch1 <= '9')
                {
                    ParseBoardPn(trimmedLine);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 内部で使う構造体
        /// </summary>
        private struct PieceSquare
        {
            public Square Square;
            public Piece Piece;
        }

        /// <summary>
        /// 落とす駒をパースします。
        /// </summary>
        /// <remarks>
        /// 00OU など４文字形式の駒をパースします。
        /// </remarks>
        private PieceSquare ParsePiece(string str)
        {
            var file = (int)(str[0] - '0');
            var rank = (int)(str[1] - '0');
            var pieceStr = str.Substring(2);
            var piece = CsaUtil.StrToPiece(pieceStr);

            if (piece == null)
            {
                throw new ShogiException(
                    str + ": CSA形式の駒を正しく読み込めませんでした。");
            }

            return new PieceSquare
            {
                Square = new Square(file, rank),
                Piece = piece,
            };
        }

        #region 平手初期配置と駒落ち形式
        /// <summary>
        /// 平手局面からの駒落ち形式を扱います。
        /// </summary>
        /// <example>
        /// PI82HI22KA -> PIの後に落とす駒を指定します。
        /// </example>
        private void ParseBoardPI(string line)
        {
            this.board = new Board();

            line.Skip(2).TakeBy(4)
                .Select(_ => new string(_.ToArray()))
                .Select(_ => ParsePiece(_))
                .ForEach(_ => this.board[_.Square] = null);
        }
        #endregion

        #region 駒別単独表現
        /// <summary>
        /// 駒を一つずつ指定する表現形式を扱います。
        /// </summary>
        /// <example>
        /// P+99KY89KE
        /// </example>
        private void ParseBoardP(string line)
        {
            var bwType = (line[1] == '+' ? BWType.Black : BWType.White);

            // 局面は駒を全くおかない状態で初期化します。
            if (this.board == null)
            {
                this.board = new Board(false);
            }

            if (line.Substring(2).StartsWith("00AL"))
            {
                // 残りの駒をすべて手番側の持ち駒に設定します。
                EnumEx.GetValues<PieceType>()
                    .ForEach(_ => Board.SetHandCount(
                        _, bwType, this.board.GetLeavePieceCount(_)));
            }
            else
            {
                line.Skip(2).TakeBy(4)
                    .Select(_ => new string(_.ToArray()))
                    .Select(_ => ParsePiece(_))
                    .ForEach(_ => SetPiece(bwType, _));
            }
        }

        /// <summary>
        /// 持ち駒の数を増やします。
        /// </summary>
        private void SetPiece(BWType bwType, PieceSquare ps)
        {
            // 駒位置が"00"の場合は持ち駒となります。
            if (ps.Square.File != 0)
            {
                this.board[ps.Square] = new BoardPiece(ps.Piece, bwType);
            }
            else
            {
                this.board.IncHandCount(ps.Piece.PieceType, bwType);
            }
        }
        #endregion

        #region 一括表現
        /// <summary>
        /// 一括表現形式の局面を読み取ります。
        /// </summary>
        /// <example>
        /// P1-KY-KE-GI-KI-OU-KI-GI-KE-KY
        /// P2 * -HI *  *  *  *  * -KA * 
        /// </example>
        private void ParseBoardPn(string line)
        {
            if (line[1] < '1' || '9' < line[1])
            {
                throw new ShogiException(
                    line + ": CSA形式の局面の段数が正しくありません。");
            }

            var currentRank = (int)(line[1] - '0');
            var rank = this.parsedRank + 1;
            if (currentRank != rank)
            {
                throw new ShogiException(
                    line + ": CSA形式の局面を正しく読み込めませんでした。");
            }

            var pieceList = line.Skip(2).TakeBy(3)
                .Select(_ => new string(_.ToArray()))
                .Select(_ => CsaUtil.StrToBoardPiece(_))
                .Where(_ => _ != null)
                .ToList();
            if (pieceList.Count() != Board.BoardSize ||
                pieceList.Any(_ => _ == null))
            {
                throw new ShogiException(
                    line + ": CSA形式の局面を正しく読み込めませんでした。");
            }

            // 局面は駒を全くおかない状態で初期化します。
            if (this.board == null)
            {
                this.board = new Board(false);
            }

            pieceList.ForEachWithIndex((piece, index) =>
            {
                var isNone = (piece.PieceType == PieceType.None);
                var file = Board.BoardSize - index;

                this.board[file, rank] = (isNone ? null : piece);
            });

            this.parsedRank += 1;
        }
        #endregion
    }
}
