#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    [TestFixture()]
    public class CalclatorTest
    {
        [Test()]
        public void SimpleTest()
        {
            Assert.AreEqual(Calculator.Default.Run("2.03456"), 2.03456);

            Assert.AreEqual(Calculator.Default.Run("10+10"), 20.0);
            Assert.AreEqual(Calculator.Default.Run("2.5*4+5"), 15.0);
            Assert.AreEqual(Calculator.Default.Run("2.5*(4+6)"), 25.0);
            Assert.AreEqual(Calculator.Default.Run("(2.4-0.4) * -(4.5+0.5)"), -10);

            Assert.AreEqual(Calculator.Default.Run("2.5*(4+6)**2"), 250.0);
            Assert.AreEqual(Calculator.Default.Run("10**2**1.5*4"), Math.Pow(10, Math.Pow(2, 1.5)) * 4);

            //Assert.AreEqual(Calculator.Default.Run("2.4e14"), 2.4e14);
        }

        [Test()]
        public void ConstTest()
        {
            Assert.AreEqual(Calculator.Default.Run("pi"), Math.PI);
            Assert.AreEqual(Calculator.Default.Run("pi * e ** 10"), Math.PI * Math.Pow(Math.E, 10));

            Assert.Throws(
                typeof(RagnarokException),
                () => Calculator.Default.Run("pii"));
        }

        [Test()]
        public void FuncTest()
        {
            Assert.AreEqual(Calculator.Default.Run("log(1)"), Math.Log(1));
            Assert.AreEqual(Calculator.Default.Run("sin(3.2)"), Math.Sin(3.2));
            Assert.DoesNotThrow(() => Calculator.Default.Run("rand()"));

            Assert.Throws(
                typeof(RagnarokException),
                () => Calculator.Default.Run("sin(1, 2)"));
            Assert.Throws(
                typeof(RagnarokException),
                () => Calculator.Default.Run("cos(1, 2, 3))))"));

            Assert.Throws(
                typeof(RagnarokException),
                () => Calculator.Default.Run("f()"));
        }

        [Test()]
        public void OtherSeparatorTest()
        {
            Assert.AreEqual(Calculator.Default.Run("max(1# 4)"), 4.0);
            Assert.AreEqual(Calculator.Default.Run("min(3# 2# 10)"), 2.0);
        }
    }
}
#endif
