using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    [TestFixture()]
    public class ScannerTest
    {
        [Test()]
        public void DoubleTest()
        {
            var scanner = new Scanner("+9.999, -9.99, \"\"");
            scanner.SetDelimiters(",");

            Assert.AreEqual(9.999, scanner.ParseDouble());
            Assert.AreEqual(-9.99, scanner.ParseDouble());
            Assert.AreEqual("", scanner.ParseText());
            Assert.AreEqual(true, scanner.IsEof);
        }

        [Test()]
        public void QuotedTest()
        {
            var scanner = new Scanner("\"\\\"\", \",,,\", test");

            Assert.AreEqual("\\\"", scanner.ParseText());
            Assert.AreEqual(",,,", scanner.ParseText());
            Assert.AreEqual("test", scanner.ParseText());
            Assert.AreEqual(true, scanner.IsEof);
        }

        [Test()]
        public void QuotedNewLineTest()
        {
            var scanner = new Scanner("item1,\"item2\r\n\", item3");

            Assert.AreEqual("item1", scanner.ParseText());
            Assert.AreEqual("item2\r\n", scanner.ParseText());
            Assert.AreEqual("item3", scanner.ParseText());
            Assert.AreEqual(true, scanner.IsEof);
        }
    }
}
