﻿#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok;
using Ragnarok.Shogi;

namespace Ragnarok.Test.Shogi
{
    [TestFixture()]
    internal class ShogiParserTest
    {
        [Test()]
        public void ResignTest()
        {
            var resign = new Move
            {
                IsResigned = true,
            };

            Assert.AreEqual(
                ShogiParser.ParseMove("まけました", true),
                resign);
            Assert.AreEqual(
                ShogiParser.ParseMove("あ、まけました", true),
                resign);
            Assert.AreNotEqual(
                ShogiParser.ParseMove("あ 負けました", true),
                resign);
        }

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

            Assert.AreEqual(
                ShogiParser.ParseMove("４６歩", true),
                new Move
                {
                    NewPosition = new Position(4, 6),
                    Piece = Piece.Hu,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("56ふぅ", true),
                new Move
                {
                    NewPosition = new Position(5, 6),
                    Piece = Piece.Hu,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("救急玉", true),
                new Move
                {
                    NewPosition = new Position(9, 9),
                    Piece = Piece.Gyoku,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("燦燦劉", true),
                new Move
                {
                    NewPosition = new Position(3, 3),
                    Piece = Piece.Ryu,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("32RYUU", true),
                new Move
                {
                    NewPosition = new Position(3, 2),
                    Piece = Piece.Ryu,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("32KINN", true),
                new Move
                {
                    NewPosition = new Position(3, 2),
                    Piece = Piece.Kin,
                });
        }

        [Test()]
        public void ParseMoveTest2()
        {
            Assert.AreEqual(
                ShogiParser.ParseMove("１３馬右", true),
                new Move
                {
                    NewPosition = new Position(1, 3),
                    Piece = Piece.Uma,
                    RelFileType = RelFileType.Right,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("１３馬右引く", true),
                new Move
                {
                    NewPosition = new Position(1, 3),
                    Piece = Piece.Uma,
                    RankMoveType = RankMoveType.Back,
                    RelFileType = RelFileType.Right,
                });
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
            Assert.IsTrue(EqualsPlayer(
                ShogiParser.ParsePlayer("急急＠てすと"),
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
#endif
