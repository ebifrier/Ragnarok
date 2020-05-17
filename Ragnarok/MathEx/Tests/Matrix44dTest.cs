#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.MathEx.Tests
{
    [TestFixture()]
    public sealed class Matrix44dTest
    {
        private static readonly Matrix44d m1 = Matrix44d.FromRowMajorArray(new double[]
        {
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16,
        });
        private static readonly Matrix44d m2 = Matrix44d.FromRowMajorArray(new double[]
        {
            -1, 2, -3, 4,
            5, -6, 7, -8,
            9, -10, 11, -12,
            13, -14, 15, -16,
        });

        [Test()]
        public void AddTest()
        {
            var result = Matrix44d.FromRowMajorArray(new double[]
            {
                0, 4, 0, 8,
                10, 0, 14, 0,
                18, 0, 22, 0,
                26, 0, 30, 0,
            });

            var m = m1.Clone(); m.Add(m2);
            Assert.AreEqual(result, m);

            m = m2.Clone(); m.Add(m1);
            Assert.AreEqual(result, m);

            Assert.AreEqual(result, m1 + m2);
            Assert.AreEqual(result, m2 + m1);
        }

        [Test()]
        public void SubtractTest()
        {
            var result1 = Matrix44d.FromRowMajorArray(new double[]
            {
                2, 0, 6, 0,
                0, 12, 0, 16,
                0, 20, 0, 24,
                0, 28, 0, 32,
            });
            var result2 = Matrix44d.FromRowMajorArray(new double[]
            {
                -2, 0, -6, 0,
                0, -12, 0, -16,
                0, -20, 0, -24,
                0, -28, 0, -32,
            });

            var m = m1.Clone(); m.Subtract(m2);
            Assert.AreEqual(result1, m);

            m = m2.Clone(); m.Subtract(m1);
            Assert.AreEqual(result2, m);

            Assert.AreEqual(result1, m1 - m2);
            Assert.AreEqual(result2, m2 - m1);
        }

        [Test()]
        public void InvertTest()
        {
            var m1 = new Matrix44d();
            m1.Translate(100, 20, -899);
            m1.Scale(20, 0.5, 0.5);
            m1.Rotate(30, 0.0, 0.0, 1.0);
            m1.Translate(40, -50, 30);

            var m2 = new Matrix44d();
            m2.Translate(-40, 50, -30);
            m2.Rotate(-30, 0.0, 0.0, 1.0);
            m2.Scale(1.0 / 20, 1.0 / 0.5, 1.0 / 0.5);
            m2.Translate(-100, -20, 899);

            Assert.True(m1.HasInverse);
            Assert.True(m2.HasInverse);
            Assert.AreEqual(m2, m1.Invert());

            var m = Matrix44d.FromRowMajorArray(new double[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
            });
            Assert.False(m.HasInverse);
            Assert.Catch<MatrixException>(() => m.Invert());
        }
    }
}
#endif
