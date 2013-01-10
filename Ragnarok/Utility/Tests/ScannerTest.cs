#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    [TestFixture()]
    internal class ScannerTest
    {
        [Test()]
        public void Test1()
        {
            var scanner = new Scanner("+9.999, -9.99, \"\"");
            scanner.SetDelimiters(",");

            Assert.AreEqual(9.999, scanner.ParseDouble());
            Assert.AreEqual(-9.99, scanner.ParseDouble());
            Assert.AreEqual("", scanner.ParseText());
            Assert.AreEqual(true, scanner.IsEof);
        }

        [Test()]
        public void Test2()
        {
            var scanner = new Scanner("\"\\\"\", \",,,\", test");

            Assert.AreEqual("\\\"", scanner.ParseText());
            Assert.AreEqual(",,,", scanner.ParseText());
            Assert.AreEqual("test", scanner.ParseText());
            Assert.AreEqual(true, scanner.IsEof);
        }
    }
}
#endif
