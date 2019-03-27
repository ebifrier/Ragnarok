#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.Go.Tests
{
    [TestFixture]
    public class SquareTest
    {
        [Test]
        public void CreateTest()
        {
            var pt = Square.Create(1 - 1, 1 - 1, 9);
            Assert.AreEqual(1 - 1, pt.Col);
            Assert.AreEqual(1 - 1, pt.Row);
            Assert.AreEqual(12, pt.Index);
            Assert.AreEqual(9, pt.BoardSize);
            Assert.False(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("[aa]", pt.ToSgf());
            Assert.AreEqual("1-1", pt.ToJstr());

            pt = Square.Create(7 - 1, 2 - 1, 9);
            Assert.AreEqual(7 - 1, pt.Col);
            Assert.AreEqual(2 - 1, pt.Row);
            Assert.AreEqual(7 + 2 * 11, pt.Index);
            Assert.AreEqual(9, pt.BoardSize);
            Assert.False(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("[gb]", pt.ToSgf());
            Assert.AreEqual("7-2", pt.ToJstr());

            pt = Square.Create(19 - 1, 4 - 1, 19);
            Assert.AreEqual(19 - 1, pt.Col);
            Assert.AreEqual(4 - 1, pt.Row);
            Assert.AreEqual(19 + 4 * 21, pt.Index);
            Assert.AreEqual(19, pt.BoardSize);
            Assert.False(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("[sd]", pt.ToSgf());
            Assert.AreEqual("19-4", pt.ToJstr());
        }

        /*[Test]
        public void FromIndexTest()
        {
            var pt = Square.FromIndex(0, 7);
            Assert.AreEqual(1, pt.Col);
            Assert.AreEqual(1, pt.Row);
            Assert.AreEqual(0, pt.Index);
            Assert.AreEqual(7, pt.BoardSize);
            Assert.False(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("aa", pt.ToSgf());
            Assert.AreEqual("1-1", pt.ToJstr());

            pt = Square.FromIndex(19 * (19 + 2) - 1, 19);
            Assert.AreEqual(19, pt.Col);
            Assert.AreEqual(19, pt.Row);
            Assert.AreEqual(19 * 19 - 1, pt.Index);
            Assert.AreEqual(19, pt.BoardSize);
            Assert.False(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("ss", pt.ToSgf());
            Assert.AreEqual("19-19", pt.ToJstr());

            pt = Square.FromIndex(100, 13);
            Assert.AreEqual(10, pt.Col);
            Assert.AreEqual(8, pt.Row);
            Assert.AreEqual(100, pt.Index);
            Assert.AreEqual(13, pt.BoardSize);
            Assert.False(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("jh", pt.ToSgf());
            Assert.AreEqual("10-8", pt.ToJstr());
        }*/

        [Test]
        public void NotOkTest()
        {
            Assert.Catch(() => Square.Create(7 - 1, 2 - 1, 5));
            Assert.Catch(() => Square.Create(1 - 1, 7 - 1, 5));
            Assert.Catch(() => Square.Create(7 - 1, 2 - 1, 4));
            Assert.Catch(() => Square.Create(7 - 1, 2 - 1, -5));
            Assert.Catch(() => Square.Create(0 - 1, 2 - 1, 5));
            Assert.Catch(() => Square.Create(1 - 1, 2 - 1, 29));
        }

        [Test]
        public void EmptyTest()
        {
            var pt = Square.Empty();
            Assert.True(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.False(pt.IsOk());
            Assert.AreEqual("", pt.ToString());

            pt = Square.Pass();
            Assert.False(pt.IsEmpty);
            Assert.True(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("[]", pt.ToString());
        }

        [Test]
        public void InvTest()
        {
            var pt = Square.Create(4 - 1, 2 - 1, 13).Inv();
            Assert.AreEqual(10 - 1, pt.Col);
            Assert.AreEqual(12 - 1, pt.Row);
            Assert.AreEqual(13, pt.BoardSize);
            Assert.False(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("[jl]", pt.ToSgf());
            Assert.AreEqual("10-12", pt.ToJstr());

            pt = Square.Create(2 - 1, 7 - 1, 7).Inv();
            Assert.AreEqual(6 - 1, pt.Col);
            Assert.AreEqual(1 - 1, pt.Row);
            Assert.AreEqual(7, pt.BoardSize);
            Assert.False(pt.IsEmpty);
            Assert.False(pt.IsPass);
            Assert.True(pt.IsOk());
            Assert.AreEqual("[fa]", pt.ToSgf());
            Assert.AreEqual("6-1", pt.ToJstr());
        }

        [Test]
        public void RotateTest()
        {
            var pt = Square.Create(1 - 1, 1 - 1, 13);
            Assert.AreEqual(Square.Create(1 - 1, 1 - 1, 13), pt.Rotate(0));
            Assert.AreEqual(Square.Create(13 - 1, 1 - 1, 13), pt.Rotate(1));
            Assert.AreEqual(Square.Create(13 - 1, 13 - 1, 13), pt.Rotate(2));
            Assert.AreEqual(Square.Create(1 - 1, 13 - 1, 13), pt.Rotate(3));
            Assert.AreEqual(pt.Rotate(1), pt.Rotate(-3));
            Assert.AreEqual(pt.Rotate(2), pt.Rotate(-2));
            Assert.AreEqual(pt.Rotate(3), pt.Rotate(-1));

            pt = Square.Create(7 - 1, 7 - 1, 13);
            Assert.AreEqual(pt, pt.Rotate(0));
            Assert.AreEqual(pt, pt.Rotate(1));
            Assert.AreEqual(pt, pt.Rotate(2));
            Assert.AreEqual(pt, pt.Rotate(3));

            pt = Square.Create(2 - 1, 4 - 1, 19);
            Assert.AreEqual(Square.Create(2 - 1, 4 - 1, 19), pt.Rotate(0));
            Assert.AreEqual(Square.Create(16 - 1, 2 - 1, 19), pt.Rotate(1));
            Assert.AreEqual(Square.Create(18 - 1, 16 - 1, 19), pt.Rotate(2));
            Assert.AreEqual(Square.Create(4 - 1, 18 - 1, 19), pt.Rotate(3));
        }

        [Test]
        public void EqualTest()
        {
            var pt1 = Square.Create(1 - 1, 1 - 1, 7);
            var pt2 = Square.Create(1 - 1, 1 - 1, 7);
            var pt3 = Square.Create(1 - 1, 1 - 1, 19);
            var pt4 = Square.Create(4 - 1, 3 - 1, 7);

            Assert.AreEqual(pt1, pt2);
            Assert.AreNotEqual(pt1, pt3);
            Assert.AreNotEqual(pt1, pt4);

            Assert.AreNotEqual(pt2, pt3);
            Assert.AreNotEqual(pt2, pt4);

            Assert.AreEqual(Square.Empty(), Square.Empty());
            Assert.AreEqual(Square.Pass(), Square.Pass());
            Assert.AreNotEqual(Square.Pass(), Square.Empty());

            Assert.AreNotEqual(Square.Empty(), pt1);
            Assert.AreNotEqual(Square.Pass(), pt1);
        }

        private void TestAllPoint(int boardSize)
        {
            var all = Square.All(boardSize);

            Assert.AreEqual(boardSize * boardSize, all.Count());
            for (var i = 0; i < all.Count(); ++i)
            {
                var sq = all[i];
                var f = sq.Index % (boardSize + 2) - 1;
                var r = sq.Index / (boardSize + 2) - 1;

                Assert.AreEqual(f, sq.Col);
                Assert.AreEqual(r, sq.Row);
                //Assert.AreEqual(i, sq.Index);
                Assert.AreEqual(boardSize, sq.BoardSize);
                Assert.False(sq.IsPass);
                Assert.True(sq.IsOk());
                Assert.AreEqual($"[{(char)('a' + f)}{(char)('a' + r)}]", sq.ToSgf());
                Assert.AreEqual($"{f + 1}-{r + 1}", sq.ToJstr());
            }
        }

        [Test]
        public void AllTest()
        {
            TestAllPoint(3);
            TestAllPoint(9);
            TestAllPoint(13);
            TestAllPoint(19);
        }
    }
}
#endif
