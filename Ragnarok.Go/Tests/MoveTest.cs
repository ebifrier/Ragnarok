#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.Go.Tests
{
    [TestFixture]
    public class MoveTest
    {
        [Test]
        public void CreateTest1()
        {
            var sq = Square.Create(4 - 1, 7 - 1, 13);
            Assert.AreEqual("B[dg]", Move.ToSgf(sq, Stone.Black));
            Assert.AreEqual("B4-7", Move.ToJstr(sq, Stone.Black));

            sq = Square.Create(1 - 1, 3 - 1, 9);
            Assert.AreEqual("W[ac]", Move.ToSgf(sq, Stone.White));
            Assert.AreEqual("W1-3", Move.ToJstr(sq, Stone.White));
        }


        [Test]
        public void ParseSgfTest()
        {
            var tuple = Move.ParseSgf("W[dk]", 13);
            Assert.AreEqual(Stone.White, tuple.Item2);
            Assert.AreEqual(4 - 1, tuple.Item1.Col);
            Assert.AreEqual(11 - 1, tuple.Item1.Row);

            tuple = Move.ParseSgf("B[ss]", 19);
            Assert.AreEqual(Stone.Black, tuple.Item2);
            Assert.AreEqual(19 - 1, tuple.Item1.Col);
            Assert.AreEqual(19 - 1, tuple.Item1.Row);

            Assert.Null(Move.ParseSgf("B[ss]", 13));
            Assert.Null(Move.ParseSgf("B{ss}", 13));
            Assert.Null(Move.ParseSgf("Bxxxys", 13));
            Assert.Null(Move.ParseSgf("rxxxys", 13));
            Assert.Null(Move.ParseSgf("W[zz]", 5));

            Assert.Catch(() => Move.ParseSgf("rxxxys", -1));
        }

        private void TestSgf(string sgf, int boardSize)
        {
            var tuple = Move.ParseSgf(sgf, boardSize);
            Assert.NotNull(tuple);

            var sgf2 = Move.ToSgf(tuple.Item1, tuple.Item2);
            Assert.AreEqual(sgf, sgf2);
        }

        [Test]
        public void SgfTest()
        {
            TestSgf("W[dk]", 19);
            TestSgf("B[ss]", 19);
            TestSgf("B[rb]", 19);
            TestSgf("B[ll]", 19);
            TestSgf("W[aa]", 5);

            Assert.Null(Move.ParseSgf("B[ss]", 13));
            Assert.Null(Move.ParseSgf("X[ss]", 13));
            Assert.Null(Move.ParseSgf("日本語", 13));

            Assert.Catch(() => Move.ParseSgf("", -1));
            Assert.Catch(() => Move.ParseSgf("", 13));
        }
    }
}
#endif
