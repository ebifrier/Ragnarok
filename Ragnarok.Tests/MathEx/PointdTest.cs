using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.MathEx.Tests
{
    [TestFixture()]
    public sealed class PointdTest
    {
        [Test()]
        public void AddTest()
        {
            var p1 = new Pointd(3, 4);
            var p2 = new Pointd(-1999, 34344);
            var p3 = new Pointd(-1996, 34348);

            Assert.AreEqual(p3, Pointd.Add(p1, p2));
            Assert.AreEqual(p3, Pointd.Add(p2, p1));
            Assert.AreEqual(p3, (p1 + p2));
            Assert.AreEqual(p3, (p2 + p1));
        }

        [Test()]
        public void SubtractTest()
        {
            var p1 = new Pointd(3, 4);
            var p2 = new Pointd(-1999, 34344);
            var p3 = new Pointd(2002, -34340);
            var p4 = new Pointd(-2002, 34340);

            Assert.AreEqual(p3, Pointd.Subtract(p1, p2));
            Assert.AreNotEqual(Pointd.Subtract(p1, p2), Pointd.Subtract(p2, p1));
            Assert.AreEqual(p3, p1 - p2);
            Assert.AreEqual(p4, p2 - p1);
        }

        [Test()]
        public void ParseTest()
        {
            Assert.AreEqual(new Pointd(0, 0), Pointd.Parse("0,0"));
            Assert.AreEqual(new Pointd(0, 0), Pointd.Parse(" 0 , 0 "));
            Assert.AreEqual(new Pointd(-93.9, 590), Pointd.Parse("-93.9,590"));
            Assert.AreEqual(new Pointd(-1E-5, 555.87), Pointd.Parse("-1E-5,+555.87"));

            Assert.Catch<ArgumentNullException>(() => Pointd.Parse(null));
            Assert.Catch<FormatException>(() => Pointd.Parse(""));
            Assert.Catch<FormatException>(() => Pointd.Parse("test"));
            Assert.Catch<FormatException>(() => Pointd.Parse("4,"));
            Assert.Catch<FormatException>(() => Pointd.Parse("-5,-9d"));
        }
    }
}
