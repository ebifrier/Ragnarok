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
    internal sealed class MoveTest
    {
        [Test]
        public void SerializeTest()
        {
            var validMoveList =
                from sq in Board.AllSquares()
                from pc in EnumEx.GetValues<PieceType>()
                from promoted in new bool[] { false, true }
                from rm in EnumEx.GetValues<RankMoveType>()
                from rf in EnumEx.GetValues<RelFileType>()
                from at in EnumEx.GetValues<ActionType>()
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
