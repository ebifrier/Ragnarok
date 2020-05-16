#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    [TestFixture()]
    public sealed class ShogiParserExTest
    {
        [Test()]
        public void ResignTest()
        {
            var resign = new LiteralMove
            {
                SpecialMoveType = SpecialMoveType.Resign,
            };

            Assert.AreEqual(
                ShogiParserEx.ParseMove("まけました", true),
                resign);
            Assert.AreEqual(
                ShogiParserEx.ParseMove("あ、まけました", true),
                resign);
            Assert.AreNotEqual(
                ShogiParserEx.ParseMove("あ 負けました", true),
                resign);
        }

        [Test()]
        public void ParseMoveTest()
        {
            Assert.AreEqual(
                ShogiParserEx.ParseMove("同格", true),
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.Kaku,
                });

            var gin68 = new LiteralMove
            {
                DstSquare = new Square(6, 8),
                Piece = Piece.Gin,
            };
            Assert.AreEqual(
                ShogiParserEx.ParseMove("ろっぱち銀", true),
                gin68);
            Assert.AreEqual(
                ShogiParserEx.ParseMove("ロッパチ銀", true),
                gin68);

            Assert.AreEqual(
                ShogiParserEx.ParseMove("４６歩", true),
                new LiteralMove
                {
                    DstSquare = new Square(4, 6),
                    Piece = Piece.Hu,
                });

            Assert.AreEqual(
                ShogiParserEx.ParseMove("56ふぅ", true),
                new LiteralMove
                {
                    DstSquare = new Square(5, 6),
                    Piece = Piece.Hu,
                });
            Assert.AreEqual(
                ShogiParserEx.ParseMove("救急玉", true),
                new LiteralMove
                {
                    DstSquare = new Square(9, 9),
                    Piece = Piece.Gyoku,
                });
            Assert.AreEqual(
                ShogiParserEx.ParseMove("燦燦劉", true),
                new LiteralMove
                {
                    DstSquare = new Square(3, 3),
                    Piece = Piece.Ryu,
                });
            Assert.AreEqual(
                ShogiParserEx.ParseMove("32RYUU", true),
                new LiteralMove
                {
                    DstSquare = new Square(3, 2),
                    Piece = Piece.Ryu,
                });
            Assert.AreEqual(
                ShogiParserEx.ParseMove("32KINN", true),
                new LiteralMove
                {
                    DstSquare = new Square(3, 2),
                    Piece = Piece.Kin,
                });
        }

        [Test()]
        public void ParseMoveTest2()
        {
            Assert.AreEqual(
                ShogiParserEx.ParseMove("１３馬右", true),
                new LiteralMove
                {
                    DstSquare = new Square(1, 3),
                    Piece = Piece.Uma,
                    RelFileType = RelFileType.Right,
                });
            Assert.AreEqual(
                ShogiParserEx.ParseMove("１３馬右引く", true),
                new LiteralMove
                {
                    DstSquare = new Square(1, 3),
                    Piece = Piece.Uma,
                    RankMoveType = RankMoveType.Back,
                    RelFileType = RelFileType.Right,
                });
            Assert.AreEqual(
                ShogiParserEx.ParseMove("43ほーす左", true),
                new LiteralMove
                {
                    DstSquare = new Square(4, 3),
                    Piece = Piece.Uma,
                    RelFileType = RelFileType.Left,
                });

            Assert.AreEqual(
                ShogiParserEx.ParseMove("１３不不成り", true),
                new LiteralMove
                {
                    DstSquare = new Square(1, 3),
                    Piece = Piece.Hu,
                    ActionType = ActionType.Unpromote,
                });

            Assert.AreEqual(
                ShogiParserEx.ParseMove("５５うまごん", true),
                new LiteralMove
                {
                    DstSquare = new Square(5, 5),
                    Piece = Piece.Uma,
                });

            Assert.AreEqual(
                ShogiParserEx.ParseMove("３９ときんちゃん", true),
                new LiteralMove
                {
                    DstSquare = new Square(3, 9),
                    Piece = Piece.To,
                });
            Assert.AreEqual(
                ShogiParserEx.ParseMove("ごよんぽ", true),
                new LiteralMove
                {
                    DstSquare = new Square(5, 4),
                    Piece = Piece.Hu,
                });

            Assert.AreEqual(
                ShogiParserEx.ParseMove("シックスナイン不", true),
                new LiteralMove
                {
                    DstSquare = new Square(6, 9),
                    Piece = Piece.Hu,
                });

             Assert.AreEqual(
                ShogiParserEx.ParseMove("△６二角行", true),
                new LiteralMove
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
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.Hu,
                },
                ShogiParserEx.ParseMove("同歩", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.Ryu,
                },
                ShogiParserEx.ParseMove("同　流", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.Uma,
                },
                ShogiParserEx.ParseMove("DOU　馬", true));

            Assert.AreEqual(
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.Kin,
                    RelFileType = RelFileType.Right,
                },
                ShogiParserEx.ParseMove("同衾右", true));

            Assert.AreEqual(
               new LiteralMove
               {
                   SameAsOld = true,
                   Piece = Piece.Hu,
               },
               ShogiParserEx.ParseMove("34同じくおふーさん", true));
            Assert.AreEqual(
               new LiteralMove
               {
                   SameAsOld = true,
                   Piece = Piece.Gin,
                   RelFileType = RelFileType.Right,
                   RankMoveType = RankMoveType.Up,
               },
               ShogiParserEx.ParseMove("34同 ぎん右上がる", true));
            Assert.AreEqual(
               new LiteralMove
               {
                   SameAsOld = true,
                   Piece = Piece.Gyoku,
                   RelFileType = RelFileType.Left,
                   RankMoveType = RankMoveType.Back,
               },
               ShogiParserEx.ParseMove("99dou kinghidarisagaru", true));
        }

        /// <summary>
        /// 投了や中断などの特殊な指し手のパーステストを行います。
        /// </summary>
        [Test()]
        public void ParseSpecialMoveTest()
        {
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.Resign,
                },
                ShogiParserEx.ParseMove("投了", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.Interrupt,
                    BWType = BWType.Black,
                },
                ShogiParserEx.ParseMove("▲中断", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.Sennichite,
                    BWType = BWType.White,
                },
                ShogiParserEx.ParseMove("△千日手", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.TimeUp,
                    BWType = BWType.Black,
                },
                ShogiParserEx.ParseMove("▼時間切れ", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.Resign,
                    BWType = BWType.White,
                },
                ShogiParserEx.ParseMove("▽とうりょう", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.Jishogi,
                },
                ShogiParserEx.ParseMove("持将棋X", false));

            Assert.Null(ShogiParserEx.ParseMove("不明", true));
            Assert.Null(ShogiParserEx.ParseMove("投 了", true));
            Assert.Null(ShogiParserEx.ParseMove("▲投了A", true));
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
                ShogiParserEx.ParsePlayer("てすと 9級"));
            AssertPlayer(
                player1,
                ShogiParserEx.ParsePlayer("　てすと きゅうきゅう"));
            AssertPlayer(
                player1,
                ShogiParserEx.ParsePlayer("  てすと　　きゅう級"));
            AssertPlayer(
                player1,
                ShogiParserEx.ParsePlayer("  てすと　９級"));
            AssertPlayerNot(
                player1,
                ShogiParserEx.ParsePlayer("  てすと 級きゅう"));
            AssertPlayer(
                player1,
                ShogiParserEx.ParsePlayer("てすと 急急"));

			/*var player2 = new ShogiPlayer()
            {
                Nickname = "三級",
            };
            AssertPlayer(
                player2,
                ShogiParserEx.ParsePlayer("三級"));*/
            /*AssertPlayer(
                player2,
                ShogiParserEx.ParsePlayer(" ＠ 三級"));*/
            /*AssertPlayer(
                player2,
                ShogiParserEx.ParsePlayer("三級 三級"));*/
        }
    }
}
#endif
