#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Kif.Tests
{
    [TestFixture()]
    internal sealed class BodTest
    {
        /// <summary>
        /// bod形式の局面を読み取り、実際の局面に直します。
        /// </summary>
        public Board ParseBoard(string text)
        {
            var board = Board.ParseBod(text);
            Assert.NotNull(board);
            Assert.True(board.Validate());

            var bod = board.ToBod();
            Assert.True(bod.Contains(text));

            return board;
        }

        [Test()]
        public void ParseTest1()
        {
            var board1 = ParseBoard(
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "| ・ ・ ・ ・ ・ ・ ・v桂v香|一\n" +
                "| ・ ・ ・ ・ ・ ・v金v角 ・|二\n" +
                "| ・ ・ ・ ・ ・ ・v歩 ・v歩|三\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|四\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|五\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|六\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|七\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|八\n" +
                "| ・ ・ ・ ・ ・ ・ ・ 飛 ・|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：歩");

            var board0 = new Board(false);
            board0[2, 1] = new BoardPiece(Piece.Kei, BWType.White);
            board0[1, 1] = new BoardPiece(Piece.Kyo, BWType.White);
            board0[3, 2] = new BoardPiece(Piece.Kin, BWType.White);
            board0[2, 2] = new BoardPiece(Piece.Kaku, BWType.White);
            board0[3, 3] = new BoardPiece(Piece.Hu, BWType.White);
            board0[1, 3] = new BoardPiece(Piece.Hu, BWType.White);
            board0[2, 9] = new BoardPiece(Piece.Hisya, BWType.Black);
            board0.SetHandCount(PieceType.Hu, BWType.Black, 1);

            Assert.True(Board.BoardEquals(board0, board1));
        }

        public void ParseTest2()
        {
            var board1 = ParseBoard(
                "上手の持駒：なし\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|一\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|二\n" +
                "| ・ ・ ・v歩 ・ ・ ・ ・ ・|三\n" +
                "| ・ ・ ・v銀v歩 ・ ・ ・ ・|四\n" +
                "| ・ ・v歩 ・ ・ ・ ・ ・ ・|五\n" +
                "| ・ ・ 歩 歩 歩 ・ ・ ・ ・|六\n" +
                "| ・ ・ ・ 銀 ・ ・ ・ ・ ・|七\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|八\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|九\n" +
                "+---------------------------+\n" +
                "下手の持駒：なし");

            var board0 = new Board(false);
            board0[6, 3] = new BoardPiece(Piece.Hu, BWType.White);
            board0[6, 4] = new BoardPiece(Piece.Gin, BWType.White);
            board0[5, 4] = new BoardPiece(Piece.Hu, BWType.White);
            board0[7, 5] = new BoardPiece(Piece.Hu, BWType.White);
            board0[7, 6] = new BoardPiece(Piece.Hu, BWType.Black);
            board0[6, 6] = new BoardPiece(Piece.Hu, BWType.Black);
            board0[5, 6] = new BoardPiece(Piece.Hu, BWType.Black);
            board0[6, 7] = new BoardPiece(Piece.Gin, BWType.Black);

            Assert.True(Board.BoardEquals(board0, board1));
        }
    }
}
#endif
