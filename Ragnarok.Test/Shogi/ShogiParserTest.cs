using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok;
using Ragnarok.Shogi;

namespace Ragnarok.Test.Shogi
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

            var gin68 = new Move
            {
                NewPosition = new Position(6, 8),
                Piece = Piece.Gin,
            };
            Assert.AreEqual(
                ShogiParser.ParseMove("ろっぱち銀", true),
                gin68);
            Assert.AreEqual(
                ShogiParser.ParseMove("ロッパチ銀", true),
                gin68);
        }

        private bool EqualsPlayer(ShogiPlayer x, ShogiPlayer y)
        {
            return (x.Nickname == y.Nickname && x.SkillLevel == y.SkillLevel);
        }

        [Test()]
        public void ParsePlayerTest()
        {
            var player1 = new ShogiPlayer()
            {
                Nickname = "てすと",
                SkillLevel = new SkillLevel(SkillKind.Kyu, 9),
            };
            Assert.IsTrue(EqualsPlayer(
                ShogiParser.ParsePlayer("9級＠てすと"),
                player1));
            Assert.IsTrue(EqualsPlayer(
                ShogiParser.ParsePlayer("　きゅうきゅう＠てすと"),
                player1));
            Assert.IsTrue(EqualsPlayer(
                ShogiParser.ParsePlayer("  きゅう級＠てすと"),
                player1));
            Assert.IsTrue(EqualsPlayer(
                ShogiParser.ParsePlayer("  ９級  @ てすと "),
                player1));
            Assert.IsFalse(EqualsPlayer(
                ShogiParser.ParsePlayer("  級きゅう＠てすと"),
                player1));

            var player2 = new ShogiPlayer()
            {
                Nickname = "三級",
            };
            Assert.IsTrue(EqualsPlayer(
                ShogiParser.ParsePlayer("三級"),
                player2));
            Assert.IsTrue(EqualsPlayer(
                ShogiParser.ParsePlayer(" ＠ 三級"),
                player2));
            Assert.IsFalse(EqualsPlayer(
                ShogiParser.ParsePlayer("三級＠三級"),
                player2));
        }
    }
}
