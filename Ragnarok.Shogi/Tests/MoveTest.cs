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
    /// Moveに関するテストを行います。
    /// </summary>
    [TestFixture]
    public sealed class MoveTest
    {
        [Test]
        public void SerializeTest()
        {
            var validMoveList =
                from sq in Board.AllSquares()
                from pc in EnumUtil.GetValues<PieceType>()
                from promoted in new bool[] { false, true }
                from rm in EnumUtil.GetValues<RankMoveType>()
                from rf in EnumUtil.GetValues<RelFileType>()
                from at in EnumUtil.GetValues<ActionType>()
                from same in new bool[] { false, true }
                let move = new LiteralMove
                {
                    DstSquare = sq,
                    Piece = new Piece(pc, promoted),
                    RankMoveType = rm,
                    RelFileType = rf,
                    ActionType = at,
                    SameAsOld = same,
                }
                where move.Validate()
                select move;

            foreach (var move in validMoveList)
            {
                var text = JsonUtil.Serialize(move);
                Assert.NotNull(text);

                var newMove = JsonUtil.Deserialize<LiteralMove>(text);
                Assert.NotNull(newMove);
                Assert.True(newMove.Validate());

                Assert.AreEqual(move, newMove);
            }
        }
    }
}
#endif
