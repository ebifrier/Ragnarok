using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    /// <summary>
    /// Color4bクラスのテストを行います。
    /// </summary>
    [TestFixture()]
    public sealed class Color4bTest
    {
        private static void Test(Color4b c, int a, int r, int g, int b)
        {
            Assert.AreEqual(c.A, a);
            Assert.AreEqual(c.R, r);
            Assert.AreEqual(c.G, g);
            Assert.AreEqual(c.B, b);
            Assert.AreEqual(Color4b.FromArgb(a, r, g, b), c);
        }

        [Test()]
        public void CreateTest()
        {
            var c = Color4b.FromArgb(50, 100, 150, 200);

            Test(Color4b.FromArgb(1, 2, 3, 4), 1, 2, 3, 4);
            Test(Color4b.FromArgb(1, 2, 3), 255, 1, 2, 3);
            Test(Color4b.FromArgb(10, c), 10, 100, 150, 200);
            Test(Color4b.FromValue(0x11223344), 0x11, 0x22, 0x33, 0x44);
        }

        [Test()]
        public void AddTest()
        {
            Test(Color4b.FromArgb(10, 20, 30, 40) + Color4b.FromArgb(100, 150, 200, 250),
                110, 170, 230, 255);
            Test(Color4b.FromArgb(255, 255, 255, 255) + Color4b.FromArgb(100, 150, 200, 250),
                255, 255, 255, 255);
        }

        [Test()]
        public void SubtractTest()
        {
            Test(Color4b.FromArgb(100, 150, 200, 250) - Color4b.FromArgb(10, 20, 30, 40),
                90, 130, 170, 210);
            Test(Color4b.FromArgb(10, 20, 30, 40) - Color4b.FromArgb(100, 150, 200, 250),
                0, 0, 0, 0);
        }

        [Test()]
        public void MultiplyTest()
        {
            Test(Color4b.FromArgb(100, 150, 200, 250) * 2, 200, 255, 255, 255);
            Test(2 * Color4b.FromArgb(100, 150, 200, 250), 200, 255, 255, 255);
            Test(Color4b.FromArgb(100, 150, 200, 130) * 1.1, 110, 165, 220, 143);

            Test(Color4b.FromArgb(255, 255, 255, 255) * Color4b.FromArgb(255, 128, 64, 32),
                255, 128, 64, 32);
        }

        /// <summary>
        /// Color.Parseのテストを行います。
        /// </summary>
        [Test]
        public void ParseTest()
        {
            Assert.AreEqual(Color4bs.Blue, Color4b.Parse("Blue"));
            Assert.AreEqual(Color4bs.Blue, Color4b.Parse("blue"));
            Assert.AreEqual(Color4bs.Blue, Color4b.Parse(" blue "));
            Assert.AreEqual(Color4bs.Blue, Color4b.Parse(" blUe"));
            Assert.AreEqual(Color4bs.Blue, Color4b.Parse("BLUE"));

            var color = Color4b.FromValue(0xFFCCBBAA);
            Assert.AreEqual(Color4bs.Black, Color4b.Parse("#FF000000"));
            Assert.AreEqual(Color4bs.YellowGreen, Color4b.Parse("#FF9ACD32"));
            Assert.AreEqual(Color4bs.YellowGreen, Color4b.Parse("#9ACD32"));
            Assert.AreEqual(color, Color4b.Parse("#FFCCBBAA"));
            Assert.AreEqual(color, Color4b.Parse("#CCbBaA"));
            Assert.AreEqual(color, Color4b.Parse("#FCBA"));
            Assert.AreEqual(color, Color4b.Parse("#cba"));
            Assert.AreEqual(color, Color4b.Parse(" #CBA "));

            Assert.Catch<FormatException>(() => Color4b.Parse(" bl Ue"));
            Assert.Catch<FormatException>(() => Color4b.Parse(" blxUe"));
            Assert.Catch<FormatException>(() => Color4b.Parse(" blUex"));

            Assert.Catch<FormatException>(() => Color4b.Parse("#0xFFCCBBAA"));
            Assert.Catch<FormatException>(() => Color4b.Parse("# C B A"));
            Assert.Catch<FormatException>(() => Color4b.Parse("#d0C0BcA"));
        }
    }
}
