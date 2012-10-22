using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok;
using Ragnarok.Shogi;

namespace RagnarokTest.Shogi
{
    public class ShogiParserTest
    {
        [Test()]
        public void ParseMoveTest()
        {
            Assert.AreEqual(
                ShogiParser.ParseMove("同格", true),
                new Move
                {
                    SameAsOld = true,
                    Piece = Piece.Kaku,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("ろっぱち銀", true),
                new Move {
                    NewPosition = new Position(6, 8),
                    Piece = Piece.Gin,
                });
        }
    }
}
