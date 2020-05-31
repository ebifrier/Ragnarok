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
                .Select(_ => _.Modify(BWType.Black))
                .ForEach(_ => Assert.AreEqual(BWType.Black, _.GetColor()));

            PieceUtil.PieceTypes()
                .Select(_ => _.Modify(BWType.White))
                .ForEach(_ => Assert.AreEqual(BWType.White, _.GetColor()));
        }

        [Test()]
        public void IsPromotedTest()
        {
        }
    }
}
#endif
