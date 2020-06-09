#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.Shogi
{
    [TestFixture()]
    public sealed class PieceTest
    {
        [Test()]
        public void GetColorTest()
        {
            PieceUtil.PieceTypes()
                .Select(_ => _.With(Colour.Black))
                .ForEach(_ => Assert.AreEqual(Colour.Black, _.GetColour()));

            PieceUtil.PieceTypes()
                .Select(_ => _.With(Colour.White))
                .ForEach(_ => Assert.AreEqual(Colour.White, _.GetColour()));
        }

        [Test()]
        public void IsPromotedTest()
        {
        }
    }
}
#endif
