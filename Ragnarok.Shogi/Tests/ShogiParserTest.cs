#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    [TestFixture()]
    public sealed class ShogiParserTest
    {
        [Test()]
        public void ResignTest()
        {
            var resign = new LiteralMove
            {
                SpecialMoveType = SpecialMoveType.Resign,
            };

            Assert.AreEqual(
                ShogiParser.ParseMove("投了", true),
                resign);
            Assert.AreEqual(
                ShogiParser.ParseMove("TORYO", true),
                resign);
            Assert.Null(ShogiParser.ParseMove("まけました", true));
        }

        [Test()]
        public void ParseMoveTest()
        {
            Assert.AreEqual(
                ShogiParser.ParseMove("同角", true),
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.Bishop,
                });

            var gin68 = new LiteralMove
            {
                DstSquare = SquareUtil.Create(6, 8),
                Piece = Piece.Silver,
            };
            Assert.AreEqual(
                ShogiParser.ParseMove("68銀", true),
                gin68);
            Assert.AreEqual(
                ShogiParser.ParseMove("６８銀", true),
                gin68);
            Assert.AreEqual(
                ShogiParser.ParseMove("六八銀", true),
                gin68);

            Assert.AreEqual(
                ShogiParser.ParseMove("４６歩", true),
                new LiteralMove
                {
                    DstSquare = SquareUtil.Create(4, 6),
                    Piece = Piece.Pawn,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("３２金", true),
                new LiteralMove
                {
                    DstSquare = SquareUtil.Create(3, 2),
                    Piece = Piece.Gold,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("ろくはち銀", true),
                null);
            Assert.AreEqual(
                ShogiParser.ParseMove("56ふ", true),
                null);
            Assert.AreEqual(
                ShogiParser.ParseMove("救急玉", true),
                null);
            Assert.AreEqual(
                ShogiParser.ParseMove("32RYUU", true),
                null);
            Assert.AreEqual(
                ShogiParser.ParseMove("32キン", true),
                null);
        }

        [Test()]
        public void ParseMoveTest2()
        {
            Assert.AreEqual(
                ShogiParser.ParseMove("１３馬右", true),
                new LiteralMove
                {
                    DstSquare = SquareUtil.Create(1, 3),
                    Piece = Piece.Horse,
                    RelFileType = RelFileType.Right,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("１３馬左引", true),
                new LiteralMove
                {
                    DstSquare = SquareUtil.Create(1, 3),
                    Piece = Piece.Horse,
                    RankMoveType = RankMoveType.Back,
                    RelFileType = RelFileType.Left,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("１３歩不成", true),
                new LiteralMove
                {
                    DstSquare = SquareUtil.Create(1, 3),
                    Piece = Piece.Pawn,
                    ActionType = ActionType.Unpromote,
                });
            Assert.AreEqual(
                ShogiParser.ParseMove("５５馬", true),
                new LiteralMove
                {
                    DstSquare = SquareUtil.Create(5, 5),
                    Piece = Piece.Horse,
                });
             Assert.AreEqual(
                ShogiParser.ParseMove("△６二角上", true),
                new LiteralMove
                {
                    Piece = Piece.Bishop,
                    DstSquare = SquareUtil.Create(6, 2),
                    RankMoveType = RankMoveType.Up,
                    Colour = Colour.White,
                });

            Assert.AreEqual(
                ShogiParser.ParseMove("43ほーす左", true),
                null);
            Assert.AreEqual(
                ShogiParser.ParseMove("３９ときんちゃん", true),
                null);
            Assert.AreEqual(
                ShogiParser.ParseMove("ごよんぽ", true),
                null);
        }

        [Test()]
        public void ParseSameAsTest()
        {
            Assert.AreEqual(
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.Pawn,
                },
                ShogiParser.ParseMove("同歩", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.Dragon,
                },
                ShogiParser.ParseMove("同　竜", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SameAsOld = true,
                    Piece = Piece.ProSilver,
                    RankMoveType = RankMoveType.Up,
                    RelFileType = RelFileType.Left,
                },
                ShogiParser.ParseMove("同成銀左上", true));

            Assert.AreEqual(
                null,
                ShogiParser.ParseMove("DOU　馬", true));
            Assert.AreEqual(
                null,
                ShogiParser.ParseMove("同衾右", true));
            Assert.AreEqual(
               null,
               ShogiParser.ParseMove("34同じくおふーさん", true));
            Assert.AreEqual(
               null,
               ShogiParser.ParseMove("34同 ぎん右上がる", true));
            Assert.AreEqual(
               null,
               ShogiParser.ParseMove("99dou kinghidarisagaru", true));
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
                ShogiParser.ParseMove("投了", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.Interrupt,
                    Colour = Colour.Black,
                },
                ShogiParser.ParseMove("▲中断", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.Sennichite,
                    Colour = Colour.White,
                },
                ShogiParser.ParseMove("△千日手", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.TimeUp,
                    Colour = Colour.Black,
                },
                ShogiParser.ParseMove("▼時間切れ", true));
            Assert.AreEqual(
                new LiteralMove
                {
                    SpecialMoveType = SpecialMoveType.Jishogi
                },
                ShogiParser.ParseMove("持将棋X", false));

            Assert.Null(ShogiParser.ParseMove("▽とうりょう", true));
            Assert.Null(ShogiParser.ParseMove("不明", true));
            Assert.Null(ShogiParser.ParseMove("投 了", true));
            Assert.Null(ShogiParser.ParseMove("▲投了A", true));
        }
    }
}
#endif
