#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.Go.Tests
{
    [TestFixture]
    public class BoardTest
    {
        [Test]
        public void CloneTest1()
        {
            var board1 = new Board(11);
            board1[1, 1] = Stone.Black;

            var board2 = board1.Clone();
            board2[3, 4] = Stone.White;

            Assert.AreEqual(Stone.Black, board1[1, 1]);
            Assert.AreEqual(Stone.Empty, board1[3, 4]);
            Assert.AreEqual(Stone.Black, board2[1, 1]);
            Assert.AreEqual(Stone.White, board2[3, 4]);
        }

        private void CloneSgf(ref Board board, string sgf)
        {
            var tuple = Move.ParseSgf(sgf, board.BoardSize);
            Assert.NotNull(tuple);
            Assert.False(tuple.Item1.IsEmpty);
            Assert.AreEqual(board.Turn, tuple.Item2);

            Assert.True(board.MakeMove(tuple.Item1));
            board = board.Clone(); // 毎回Cloneする
        }

        [Test]
        public void CloneTest2()
        {
            var allSgf = ";B[dd];W[cc];B[cd];W[dc];B[ec];W[eb];B[fb];W[fc];B[ed];W[gb]" +
                         ";B[db];W[fa];B[cb];W[bc];B[bb];W[bd];B[be];W[ce];B[ad];W[bf]" +
                         ";B[ac]";

            var board = new Board(11);
            allSgf.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .ForEach(_ => CloneSgf(ref board, _));

            Assert.AreEqual(Stone.Empty, board[1, 2]);
            Assert.AreEqual(Stone.Empty, board[1, 3]);
            Assert.AreEqual(Stone.Empty, board[2, 2]);
            Assert.AreEqual(Stone.Empty, board[3, 2]);
            Assert.AreEqual(Stone.Empty, board[5, 1]);
        }

        [Test]
        public void GetterSetterTest()
        {
            var board = new Board(5);
            Assert.AreEqual(5, board.BoardSize);
            Assert.AreEqual(0, board.BlackCaptureCount);
            Assert.AreEqual(0, board.WhiteCaptureCount);
            Assert.AreEqual(Stone.Empty, board[0, 0]);
            Assert.AreEqual(Stone.Empty, board[Square.Create(0, 0, 5)]);

            board[0, 0] = Stone.Black;
            Assert.AreEqual(Stone.Black, board[0, 0]);
            Assert.AreEqual(Stone.Black, board[Square.Create(0, 0, 5)]);
        }

        [Test]
        public void PlaceStoneTest()
        {
            var board = new Board(5);

            board.PlaceStone(Square.Create(0, 0, 5), Stone.Black);
            board.PlaceStone(Square.Create(0, 0, 5), Stone.Black);
            board.PlaceStone(Square.Create(0, 1, 5), Stone.White);
            board.PlaceStone(Square.Create(1, 0, 5), Stone.White);
            Assert.AreEqual(Stone.Empty, board[Square.Create(0, 0, 5)]);
        }

        [Test]
        public void PlaceStoneTest2()
        {
            var board = new Board(5);

            board.PlaceStone(Square.Create(0, 0, 5), Stone.Black);
            board.PlaceStone(Square.Create(0, 1, 5), Stone.Black);
            board.PlaceStone(Square.Create(1, 0, 5), Stone.Black);
            board.PlaceStone(Square.Create(2, 1, 5), Stone.Black);
            board.PlaceStone(Square.Create(2, 3, 5), Stone.Black);
            board.PlaceStone(Square.Create(3, 2, 5), Stone.Black);
            board.PlaceStone(Square.Create(1, 3, 5), Stone.Black);
            board.PlaceStone(Square.Create(1, 1, 5), Stone.White);
            board.PlaceStone(Square.Create(1, 2, 5), Stone.White);
            board.PlaceStone(Square.Create(2, 2, 5), Stone.White);
            board.PlaceStone(Square.Create(0, 2, 5), Stone.Black);
            Assert.AreEqual(Stone.Empty, board[1, 1]);
            Assert.AreEqual(Stone.Empty, board[1, 2]);
            Assert.AreEqual(Stone.Empty, board[2, 2]);

            board.PlaceStone(Square.Create(1, 1, 5), Stone.White);
            Assert.AreEqual(Stone.White, board[1, 1]);
        }

        [Test]
        public void PlaceStoneTest3()
        {
            var board = new Board(5);

            board.PlaceStone(Square.Create(0, 0, 5), Stone.White);
            board.PlaceStone(Square.Create(0, 1, 5), Stone.White);
            board.PlaceStone(Square.Create(1, 0, 5), Stone.White);
            board.PlaceStone(Square.Create(2, 1, 5), Stone.White);
            board.PlaceStone(Square.Create(2, 3, 5), Stone.White);
            board.PlaceStone(Square.Create(4, 2, 5), Stone.White);
            board.PlaceStone(Square.Create(1, 3, 5), Stone.White);
            board.PlaceStone(Square.Create(3, 1, 5), Stone.White);
            board.PlaceStone(Square.Create(3, 3, 5), Stone.White);
            board.PlaceStone(Square.Create(1, 1, 5), Stone.Black);
            board.PlaceStone(Square.Create(1, 2, 5), Stone.Black);
            board.PlaceStone(Square.Create(2, 2, 5), Stone.Black);
            board.PlaceStone(Square.Create(3, 2, 5), Stone.Black);
            board.PlaceStone(Square.Create(0, 2, 5), Stone.White);
            Assert.AreEqual(Stone.Empty, board[1, 1]);
            Assert.AreEqual(Stone.Empty, board[2, 2]);
            Assert.AreEqual(Stone.Empty, board[1, 2]);
            Assert.AreEqual(Stone.Empty, board[3, 2]);

            board.PlaceStone(Square.Create(1, 1, 5), Stone.Black);
            Assert.AreEqual(Stone.Black, board[1, 1]);
        }

        private void MakeMoveSgf(Board board, string sgf)
        {
            var tuple = Move.ParseSgf(sgf, board.BoardSize);
            Assert.NotNull(tuple);
            Assert.False(tuple.Item1.IsEmpty);
            Assert.AreEqual(board.Turn, tuple.Item2);

            Assert.True(board.MakeMove(tuple.Item1));
        }

        [Test]
        public void MakeMoveTest()
        {
            var allSgf = ";B[dd];W[cc];B[cd];W[dc];B[ec];W[eb];B[fb];W[fc];B[ed];W[gb]" +
                         ";B[db];W[fa];B[cb];W[bc];B[bb];W[bd];B[be];W[ce];B[ad];W[bf]" +
                         ";B[ac]";
            
            var board = new Board(11);
            allSgf.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .ForEach(_ => MakeMoveSgf(board, _));

            Assert.AreEqual(Stone.Empty, board[1, 2]);
            Assert.AreEqual(Stone.Empty, board[1, 3]);
            Assert.AreEqual(Stone.Empty, board[2, 2]);
            Assert.AreEqual(Stone.Empty, board[3, 2]);
            Assert.AreEqual(Stone.Empty, board[5, 1]);
        }

        [Test]
        public void IsLegalTest()
        {
            var board = new Board(5);

            board.MakeMove(Square.Create(1, 3, 5));
            board.MakeMove(Square.Create(1, 1, 5));
            board.MakeMove(Square.Create(0, 1, 5));
            board.MakeMove(Square.Create(1, 2, 5));
            board.MakeMove(Square.Create(1, 0, 5));
            board.MakeMove(Square.Create(2, 2, 5));
            board.MakeMove(Square.Create(2, 1, 5));
            board.MakeMove(Square.Create(3, 0, 5));
            board.MakeMove(Square.Create(2, 3, 5));
            board.MakeMove(Square.Create(4, 1, 5));
            board.MakeMove(Square.Create(3, 2, 5));
            Assert.False(board.IsLegal(Square.Create(0, 0, 5)));
            Assert.True(board.IsLegal(Square.Create(4, 0, 5)));

            board.MakeMove(Square.Create(4, 4, 5));
            Assert.True(board.IsLegal(Square.Create(0, 0, 5)));
            Assert.False(board.IsLegal(Square.Create(4, 0, 5)));

            board.MakeMove(Square.Create(0, 3, 5));
            Assert.False(board.IsLegal(Square.Create(0, 2, 5)));
            Assert.True(board.IsLegal(Square.Pass()));

            board.MakeMove(Square.Pass());
            Assert.True(board.IsLegal(Square.Create(0, 2, 5)));

            board.MakeMove(Square.Create(0, 2, 5));
            Assert.AreEqual(Stone.Empty, board[1, 1]);
        }

        [Test]
        public void KoTest()
        {
            var board = new Board(5);

            board.MakeMove(Square.Create(0, 1, 5));
            board.MakeMove(Square.Create(1, 1, 5));
            board.MakeMove(Square.Create(1, 0, 5));
            board.MakeMove(Square.Create(2, 0, 5));
            board.MakeMove(Square.Create(1, 2, 5));
            board.MakeMove(Square.Create(2, 2, 5));
            board.MakeMove(Square.Create(4, 4, 5));
            board.MakeMove(Square.Create(3, 1, 5));

            Assert.True(board.IsLegal(Square.Create(2, 1, 5)));
            board.MakeMove(Square.Create(2, 1, 5));
            Assert.AreEqual(Stone.Empty, board[1, 1]);

            // コウなので打てない
            Assert.AreEqual(Square.Create(1, 1, 5), board.Ko);
            Assert.False(board.IsLegal(Square.Create(1, 1, 5)));
            board.MakeMove(Square.Create(4, 0, 5));

            // 逆の手番なら打てる
            Assert.AreEqual(Square.Empty(), board.Ko);
            Assert.True(board.IsLegal(Square.Create(1, 1, 5)));
            board.MakeMove(Square.Create(0, 4, 5));

            // 次はコウが解除されているので打てる
            Assert.AreEqual(Square.Empty(), board.Ko);
            Assert.True(board.IsLegal(Square.Create(1, 1, 5)));
            board.MakeMove(Square.Create(1, 1, 5));

            // 違い場所がコウになる
            Assert.AreEqual(Stone.Empty, board[2, 1]);
            Assert.AreEqual(Square.Create(2, 1, 5), board.Ko);
            Assert.False(board.IsLegal(Square.Create(2, 1, 5)));
        }

        [Test]
        public void KoTest2()
        {
            var board = new Board(5);

            board.MakeMove(Square.Create(0, 1, 5));
            board.MakeMove(Square.Create(1, 1, 5));
            board.MakeMove(Square.Create(1, 0, 5));
            board.MakeMove(Square.Create(2, 0, 5));
            board.MakeMove(Square.Create(1, 2, 5));
            board.MakeMove(Square.Create(2, 2, 5));
            board.MakeMove(Square.Create(3, 1, 5));
            board.MakeMove(Square.Create(3, 0, 5));
            board.MakeMove(Square.Pass());
            board.MakeMove(Square.Create(3, 2, 5));
            board.MakeMove(Square.Pass());
            board.MakeMove(Square.Create(4, 1, 5));

            Assert.True(board.IsLegal(Square.Create(2, 1, 5)));
            board.MakeMove(Square.Create(2, 1, 5));
            Assert.AreEqual(Stone.Empty, board[1, 1]);

            Assert.AreEqual(Square.Empty(), board.Ko);
            Assert.True(board.IsLegal(Square.Create(1, 1, 5)));
            board.MakeMove(Square.Create(1, 1, 5));

            Assert.AreEqual(Square.Empty(), board.Ko);
            Assert.True(board.IsLegal(Square.Create(2, 1, 5)));
            board.MakeMove(Square.Create(2, 1, 5));
        }
    }
}
#endif
