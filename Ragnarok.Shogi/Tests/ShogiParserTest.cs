#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    [TestFixture()]
    internal sealed class ShogiParserTest
    {
        [Test()]
        public void ResignTest()
        {
            var resign = new Move
            {
                SpecialMoveType = SpecialMoveType.Resign,
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
                DstSquare = new Square(6, 8),
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
                    DstSquare = new Square(4, 6),
                    Piece = Piece.Hu,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("56ふぅ", true),
                new Move
                {
                    DstSquare = new Square(5, 6),
                    Piece = Piece.Hu,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("救急玉", true),
                new Move
                {
                    DstSquare = new Square(9, 9),
                    Piece = Piece.Gyoku,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("燦燦劉", true),
                new Move
                {
                    DstSquare = new Square(3, 3),
                    Piece = Piece.Ryu,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("32RYUU", true),
                new Move
                {
                    DstSquare = new Square(3, 2),
                    Piece = Piece.Ryu,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("32KINN", true),
                new Move
                {
                    DstSquare = new Square(3, 2),
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
                    DstSquare = new Square(1, 3),
                    Piece = Piece.Uma,
                    RelFileType = RelFileType.Right,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("１３馬右引く", true),
                new Move
                {
                    DstSquare = new Square(1, 3),
                    Piece = Piece.Uma,
                    RankMoveType = RankMoveType.Back,
                    RelFileType = RelFileType.Right,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("43ほーす左", true),
                new Move
                {
                    DstSquare = new Square(4, 3),
                    Piece = Piece.Uma,
                    RelFileType = RelFileType.Left,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("１３不不成り", true),
                new Move
                {
                    DstSquare = new Square(1, 3),
                    Piece = Piece.Hu,
                    ActionType = ActionType.Unpromote,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("５５うまごん", true),
                new Move
                {
                    DstSquare = new Square(5, 5),
                    Piece = Piece.Uma,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("３９ときんちゃん", true),
                new Move
                {
                    DstSquare = new Square(3, 9),
                    Piece = Piece.To,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("ごよんぽ", true),
                new Move
                {
                    DstSquare = new Square(5, 4),
                    Piece = Piece.Hu,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("シックスナイン不", true),
                new Move
                {
                    DstSquare = new Square(6, 9),
                    Piece = Piece.Hu,
                });

             Assert.AreEqual(
                ShogiParser.ParseMove("△６二角行", true),
                new Move
                {
                    Piece = Piece.Kaku,
                    DstSquare = new Square(6, 2),
                    RankMoveType = RankMoveType.Up,
                    BWType = BWType.White,
                });
        }

        [Test()]
        public void ParseSameAsTest()
        {
            Assert.AreEqual(
                new Move
                {
                    SameAsOld = true,
                    Piece = Piece.Hu,
                },
                ShogiParser.ParseMove("同歩", true));
            Assert.AreEqual(
                new Move
                {
                    SameAsOld = true,
                    Piece = Piece.Ryu,
                },
                ShogiParser.ParseMove("同　流", true));
            Assert.AreEqual(
                new Move
                {
                    SameAsOld = true,
                    Piece = Piece.Uma,
                },
                ShogiParser.ParseMove("DOU　馬", true));

            Assert.AreEqual(
                new Move
                {
                    SameAsOld = true,
                    Piece = Piece.Kin,
                    RelFileType = RelFileType.Right,
                },
                ShogiParser.ParseMove("同衾右", true));

            Assert.AreEqual(
               new Move
               {
                   SameAsOld = true,
                   Piece = Piece.Hu,
               },
               ShogiParser.ParseMove("34同じくおふーさん", true));
            Assert.AreEqual(
               new Move
               {
                   SameAsOld = true,
                   Piece = Piece.Gin,
                   RelFileType = RelFileType.Right,
                   RankMoveType = RankMoveType.Up,
               },
               ShogiParser.ParseMove("34同 ぎん右上がる", true));
            Assert.AreEqual(
               new Move
               {
                   SameAsOld = true,
                   Piece = Piece.Gyoku,
                   RelFileType = RelFileType.Left,
                   RankMoveType = RankMoveType.Back,
               },
               ShogiParser.ParseMove("99dou kinghidarisagaru", true));
        }

        private void AssertPlayer(ShogiPlayer expected, ShogiPlayer actual)
        {
            Assert.AreEqual(expected.Nickname, actual.Nickname);
            Assert.AreEqual(expected.SkillLevel, actual.SkillLevel);
        }

        private void AssertPlayerNot(ShogiPlayer expected, ShogiPlayer actual)
        {
            Assert.AreEqual(expected.Nickname, actual.Nickname);
            Assert.AreNotEqual(expected.SkillLevel, actual.SkillLevel);
        }

        [Test()]
        public void ParsePlayerTest()
        {
            var player1 = new ShogiPlayer()
            {
                Nickname = "てすと",
                SkillLevel = new SkillLevel(SkillKind.Kyu, 9),
            };
            AssertPlayer(
                player1,
                ShogiParser.ParsePlayer("てすと 9級"));
            AssertPlayer(
                player1,
                ShogiParser.ParsePlayer("　てすと きゅうきゅう"));
            AssertPlayer(
                player1,
                ShogiParser.ParsePlayer("  てすと　　きゅう級"));
            AssertPlayer(
                player1,
                ShogiParser.ParsePlayer("  てすと　９級"));
            AssertPlayerNot(
                player1,
                ShogiParser.ParsePlayer("  てすと 級きゅう"));
            AssertPlayer(
                player1,
                ShogiParser.ParsePlayer("てすと 急急"));

            var player2 = new ShogiPlayer()
            {
                Nickname = "三級",
            };
            /*AssertPlayer(
                player2,
                ShogiParser.ParsePlayer("三級"));*/
            /*AssertPlayer(
                player2,
                ShogiParser.ParsePlayer(" ＠ 三級"));*/
            /*AssertPlayer(
                player2,
                ShogiParser.ParsePlayer("三級 三級"));*/
        }
    }
}
#endif
