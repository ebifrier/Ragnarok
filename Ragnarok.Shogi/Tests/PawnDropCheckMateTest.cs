#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    /// <summary>
    /// 打ち歩詰のテストを行います。
    /// </summary>
    [TestFixture]
    public sealed class PawnDropCheckMateTest
    {
        [Test]
        public void MateTest1()
        {
            var bod =
                "後手の持駒：角二　桂二　歩五　\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "|v香 ・ ・ ・ ・ ・ ・ ・v香|一\n" +
                "| ・ ・ ・ ・ 銀 ・v金 ・ ・|二\n" +
                "| ・ ・ とv歩v歩 ・ ・v歩 ・|三\n" +
                "| ・ ・ ・ ・ ・ ・v歩 ・v歩|四\n" +
                "| 歩 ・ 銀 金 ・ 飛 ・ ・ ・|五\n" +
                "| ・ ・ 銀 歩 ・ 金 ・v玉 歩|六\n" +
                "| 玉 歩 桂 ・ ・ ・ ・ ・ ・|七\n" +
                "| ・ ・ 歩 ・ ・ 歩 金 ・ ・|八\n" +
                "| 香v飛 ・ ・ ・ 銀 ・ ・ 香|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：桂　歩　\n" +
                "手数＝130";
            var board = Board.ParseBod(bod);

            var move = Move.CreateDrop(
                BWType.Black,
                new Square(2, 7),
                PieceType.Hu);

            Assert.False(board.CanMove(move));
            Assert.False(board.DoMove(move));
        }

        [Test]
        public void MateTest2()
        {
            var bod =
                "# ----  棋譜ファイル  ----\n" +
                "先手：先手\n" +
                "後手：後手\n" +
                "後手の持駒：歩三　\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "|v香v桂 ・ ・ ・ ・ ・ ・ ・|一\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|二\n" +
                "|v玉v桂v歩 金 ・ ・ ・ ・ ・|三\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|四\n" +
                "| ・ 銀 ・ 歩 ・ ・ ・ ・ ・|五\n" +
                "| 玉 ・ 歩 ・ 歩 香 ・ ・ ・|六\n" +
                "| ・v金 銀 金 ・ ・ ・ ・ ・|七\n" +
                "| ・ ・v馬 銀 ・ ・ ・ ・ ・|八\n" +
                "| 香 ・ ・ ・ ・ ・ ・ ・ ・|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：飛二　角　金　銀　桂二　香　歩十一　\n" +
                "手数＝0\n" +
                "後手番\n";
            var board = Board.ParseBod(bod);

            var move = Move.CreateDrop(
                BWType.White,
                new Square(9, 5),
                PieceType.Hu);

            Assert.False(board.CanMove(move));
            Assert.False(board.DoMove(move));
        }
    }
}
#endif
