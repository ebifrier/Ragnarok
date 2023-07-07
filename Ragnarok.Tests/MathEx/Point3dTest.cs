using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.MathEx.Tests
{
    [TestFixture()]
    public sealed class Point3dTest
    {
        [Test()]
        public void AddTest()
        {
            var p1 = new Point3d(3, 4, 19);
            var p2 = new Point3d(-1999, 34344, 9119);
            var p3 = new Point3d(-1996, 34348, 9138);

            Assert.AreEqual(p3, Point3d.Add(p1, p2));
            Assert.AreEqual(p3, Point3d.Add(p2, p1));
            Assert.AreEqual(p3, (p1 + p2));
            Assert.AreEqual(p3, (p2 + p1));
        }

        [Test()]
        public void SubtractTest()
        {
            var p1 = new Point3d(3, 4, 19);
            var p2 = new Point3d(-1999, 34344, 9119);
            var p3 = new Point3d(2002, -34340, -9100);
            var p4 = new Point3d(-2002, 34340, 9100);

            Assert.AreEqual(p3, Point3d.Subtract(p1, p2));
            Assert.AreNotEqual(Point3d.Subtract(p1, p2), Point3d.Subtract(p2, p1));
            Assert.AreEqual(p3, p1 - p2);
            Assert.AreEqual(p4, p2 - p1);
        }

        [Test()]
        public void ParseTest()
        {
            Assert.AreEqual(new Point3d(0, 0, 0), Point3d.Parse("0,0,0"));
            Assert.AreEqual(new Point3d(0, 0, 0), Point3d.Parse(" 0 , 0 , 0 "));
            Assert.AreEqual(new Point3d(-93.9, 590, 90.888), Point3d.Parse("-93.9,590,90.888"));
            Assert.AreEqual(new Point3d(-1E-5, 555.87, 30.5), Point3d.Parse("-1E-5,+555.87,30.5"));

            Assert.Catch<ArgumentNullException>(() => Point3d.Parse(null));
            Assert.Catch<FormatException>(() => Point3d.Parse(""));
            Assert.Catch<FormatException>(() => Point3d.Parse("test"));
            Assert.Catch<FormatException>(() => Point3d.Parse("4,5"));
            Assert.Catch<FormatException>(() => Point3d.Parse("-5,-9,44d"));
        }
    }
}
