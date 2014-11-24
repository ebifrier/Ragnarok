#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok.Net.ProtoBuf;

namespace Ragnarok.Shogi.Tests
{
    /// <summary>
    /// BoardMoveに関するテストを行います。
    /// </summary>
    [TestFixture]
    internal sealed class BoardMoveTest
    {
        [Test]
        public void SerializeTest()
        {
            // すべてのマスをテストすると時間がかかりすぎるため、
            // 移動元・移動先共にテストするマスを絞っています。
            var validMoveList =
                from turn in new BWType[] { BWType.Black, BWType.White }
                from dst in Board.AllSquares()
                from src in Board.AllSquares()
                from pc in EnumEx.GetValues<PieceType>()
                from promoted in new bool[] { false, true }
                from tookPc in EnumEx.GetValues<PieceType>()
                from tookPromoted in new bool[] { false, true }
                from promote in new bool[] { false, true }
                where pc != PieceType.None
                where tookPc != PieceType.None
                where (dst.Rank % 2) == 1 && (dst.File % 3) == 1
                where (src.Rank % 2) == 1 && (src.File % 3) == 1
                let bmove = BoardMove.CreateMove(
                    turn, src, dst,
                    new Piece(pc, promoted), promote,
                    new Piece(tookPc, tookPromoted))
                where bmove.Validate()
                select bmove;

            var validDropList =
                from turn in new BWType[] { BWType.Black, BWType.White }
                from dst in Board.AllSquares()
                from pc in EnumEx.GetValues<PieceType>()
                where pc != PieceType.None
                let bmove = BoardMove.CreateDrop(turn, dst, pc)
                where bmove.Validate()
                select bmove;

            var specialMoveList =
                from turn in new BWType[] { BWType.Black, BWType.White }
                from special in EnumEx.GetValues<SpecialMoveType>()
                where special != SpecialMoveType.None
                let bmove = BoardMove.CreateSpecialMove(turn, special)
                where bmove.Validate()
                select bmove;

            var count = 0;

            var moveList = validMoveList.Concat(validDropList).Concat(specialMoveList);
            foreach (var bmove in moveList)
            {
                var bytes = PbUtil.Serialize(bmove);
                Assert.NotNull(bytes);

                var newMove = PbUtil.Deserialize<BoardMove>(bytes);
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
