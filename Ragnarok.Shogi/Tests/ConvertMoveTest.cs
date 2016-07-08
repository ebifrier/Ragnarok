#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    [TestFixture]
    internal sealed class ConvertMoveTest
    {
        private static void TestMove(Board board, LiteralMove lmove, bool makeMove = false)
        {
            var newMove = board.NormalizeMove(lmove);
            Assert.NotNull(newMove);
            Assert.True(newMove.Validate());

            // 移動元の情報は使わない
            var move1 = board.ConvertMoveFromLiteral(newMove, false);
            Assert.NotNull(move1);
            Assert.True(move1.Validate());
            Assert.True(board.CanMove(move1));

            // 移動元の情報を使う
            var move2 = board.ConvertMoveFromLiteral(newMove, true);
            Assert.NotNull(move2);
            Assert.True(move2.Validate());
            Assert.True(board.CanMove(move2));

            if (makeMove)
            {
                Assert.True(board.DoMove(move1));
            }
        }

        private static void TestInvalidMove(Board board, LiteralMove move)
        {
            Assert.Null(board.NormalizeMove(move));
        }

        private static void TestMove(Board board, string moveStr, bool makeMove = false)
        {
            var move = ShogiParserEx.ParseMove(moveStr, true);
            Assert.NotNull(move);
            Assert.True(move.Validate());

            TestMove(board, move, makeMove);
        }

        private static void TestInvalidMove(Board board, string moveStr)
        {
            var move = ShogiParser.ParseMove(moveStr, true);
            if (move == null || !move.Validate())
            {
                // 成功
                return;
            }

            TestInvalidMove(board, move);
        }

        private static LiteralMove M(string moveStr)
        {
            var move = ShogiParser.ParseMove(moveStr, true);
            Assert.NotNull(move);
            Assert.True(move.Validate());

            return move;
        }

        /// <summary>
        /// 指し手の正規化ができるか調べます。
        /// </summary>
        [Test]
        public void NormalizeTest1()
        {
            var board = new Board();

            foreach (var move in SampleMove.ChoKaigi.Select(_ => _.Move))
            {
                // 実際に着手します。
                TestMove(board, move, true);
            }
        }

        [Test]
        public void NormalizeTest2()
        {
            var boardStr =
                "# ----  棋譜ファイル  ----\n" +
                "先手：先手\n" +
                "後手：後手\n" +
                "後手の持駒：歩　\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "| ・ ・ ・ ・v玉 ・ ・ ・ ・|一\n" +
                "| 角 ・ ・ ・v歩 ・ ・ ・ ・|二\n" +
                "| ・ ・ ・ ・ ・ 金v銀 ・ ・|三\n" +
                "| 角 ・ ・ ・ ・ 金 金 金 ・|四\n" +
                "| ・ 桂 ・ 桂 ・ ・ ・ ・ ・|五\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|六\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|七\n" +
                "| ・ ・ ・ ・ 飛 ・ 飛 ・ ・|八\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ 香|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：桂　\n" +
                "手数＝0\n" +
                "後手番\n" +
                "手数----指手---------消費時間--\n" +
                "   1 △１五歩打     (00:00/00:00:00)";
            var kifu = KifuReader.LoadFrom(boardStr);
            Assert.NotNull(kifu);

            var board = kifu.CreateBoard();
            Assert.NotNull(board);
            Assert.True(board.Validate());
            Assert.AreEqual(BWType.Black, board.Turn);

            // 桂
            TestMove(board, "73桂左");
            TestMove(board, "73桂右");
            TestMove(board, "73桂右成る");
            TestMove(board, "73桂左不成り");
            TestMove(board, "73桂打");
            TestInvalidMove(board, "73桂");
            TestInvalidMove(board, "73桂成らず");
            TestInvalidMove(board, "73桂直");
            TestInvalidMove(board, "73桂寄");

            Assert.AreEqual(M("▲23桂"), board.NormalizeMove(M("23桂打")));

            // 金
            TestMove(board, "33金寄");
            TestMove(board, "33金直");
            TestMove(board, "33金左上");
            TestMove(board, "33金右");
            TestMove(board, "33金右上");
            TestInvalidMove(board, "33金");
            TestInvalidMove(board, "33金打");
            TestInvalidMove(board, "33金左");

            // 飛車
            TestMove(board, "48飛車右");
            TestMove(board, "48飛車左");
            TestMove(board, "48飛車左成らず");
            TestInvalidMove(board, "48飛車");
            TestInvalidMove(board, "48飛車成る");
            TestInvalidMove(board, "48飛車左成る");
            TestInvalidMove(board, "48飛車直");
            TestInvalidMove(board, "48飛車寄");
            TestInvalidMove(board, "48飛車引");

            // 角
            TestMove(board, "83角行");
            TestMove(board, "83角上");
            TestMove(board, "83角引");
            TestMove(board, "83角行成る");
            TestInvalidMove(board, "83角成らず");
            TestInvalidMove(board, "83角右");
            TestInvalidMove(board, "83角左");
            TestInvalidMove(board, "83角直");
            TestInvalidMove(board, "83角左成");
            TestInvalidMove(board, "83角打");

            // 香車
            TestMove(board, "15香");
            TestMove(board, "同香");
            TestMove(board, "同香成らず");
            TestMove(board, "同香直");
            TestInvalidMove(board, "同香左");
            TestInvalidMove(board, "同香打");
            TestInvalidMove(board, "同香成");
            TestInvalidMove(board, "同香寄");
            TestInvalidMove(board, "同香引");
            Assert.AreEqual(M("▲同香"), board.NormalizeMove(M("同香直")));
        }

        [Test]
        public void SpecialMove_NormalizeTest()
        {
            var board = new Board();

            var resign = new LiteralMove { SpecialMoveType = SpecialMoveType.Resign };
            var move = board.NormalizeMove(resign);
            Assert.NotNull(move);
            Assert.True(move.Validate());
            Assert.True(move.IsSpecialMove);
            Assert.True(move.SpecialMoveType == SpecialMoveType.Resign);

            var invalid = new LiteralMove();
            Assert.Catch(() => board.NormalizeMove(invalid));
        }
    }
}
#endif
