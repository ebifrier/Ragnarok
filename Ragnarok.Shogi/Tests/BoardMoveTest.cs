#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok.Utility;

namespace Ragnarok.Shogi.Tests
{
    /// <summary>
    /// BoardMoveに関するテストを行います。
    /// </summary>
    [TestFixture]
    public sealed class BoardMoveTest
    {
        [Test]
        public void SerializeTest()
        {
            // すべてのマスをテストすると時間がかかりすぎるため、
            // 移動元・移動先共にテストするマスを絞っています。
            var validMoveList =
                from turn in ColourUtil.BlackWhite()
                from dst in Board.Squares()
                from src in Board.Squares()
                from pc in PieceUtil.PieceTypes(turn)
                from tookPc in PieceUtil.PieceTypes().Concat(new []{ Piece.None })
                from promote in new bool[] { false, true }
                where (dst.GetRank() % 2) == 1 && (dst.GetFile() % 3) == 1
                where (src.GetRank() % 2) == 1 && (src.GetFile() % 3) == 1
                let bmove = Move.CreateMove(pc, src, dst, promote, tookPc.Modify(turn.Flip()))
                where bmove.Validate()
                select bmove;

            var validDropList =
                from turn in ColourUtil.BlackWhite()
                from dst in Board.Squares()
                from pc in PieceUtil.RawTypes(turn)
                let bmove = Move.CreateDrop(pc, dst)
                where bmove.Validate()
                select bmove;

            var specialMoveList =
                from turn in ColourUtil.BlackWhite()
                from special in EnumUtil.GetValues<SpecialMoveType>()
                where special != SpecialMoveType.None
                let bmove = Move.CreateSpecialMove(turn, special)
                where bmove.Validate()
                select bmove;

            var count = 0;

            var moveList = validMoveList.Concat(validDropList).Concat(specialMoveList);
            foreach (var bmove in moveList)
            {
                var bytes = JsonUtil.Serialize(bmove);
                Assert.NotNull(bytes);

                var newMove = JsonUtil.Deserialize<Move>(bytes);
                Assert.NotNull(newMove);
                Assert.True(newMove.Validate());

                Assert.AreEqual(bmove, newMove);

                // 多くのテストがあるため、一応
                if (count++ % 50000 == 0)
                {
                    Console.WriteLine("BoardMoveTest.SerializeTest testing...");
                }
            }
        }
    }
}
#endif
