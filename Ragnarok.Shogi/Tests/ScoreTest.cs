#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi
{
    [TestFixture()]
    public sealed class ScoreTest
    {
        private void AssertValue(string text, int value, ScoreBound bound)
        {
            var score = Score.ParseValue(text, Colour.Black);
            Assert.AreEqual(ScoreType.Value, score.ScoreType);
            Assert.AreEqual(bound, score.ScoreBound);
            Assert.AreEqual(value, score.Value);
            Assert.AreEqual(Colour.Black, score.Turn);

            score.Neg();
            Assert.AreEqual(-value, score.Value);
            Assert.AreEqual(bound.Flip(), score.ScoreBound);
            Assert.AreEqual(Colour.White, score.Turn);

            score.Neg();
            Assert.AreEqual(value, score.Value);
            Assert.AreEqual(bound, score.ScoreBound);
            Assert.AreEqual(Colour.Black, score.Turn);
        }

        [Test()]
        public void ParseValueTest()
        {
            AssertValue("+89", 89, ScoreBound.Exact);
            AssertValue("-99999999", -99999999, ScoreBound.Exact);
            AssertValue("109", 109, ScoreBound.Exact);

            AssertValue("7899↑", 7899, ScoreBound.Lower);
            AssertValue("-47↑", -47, ScoreBound.Lower);
            AssertValue("+ 987↓", 987, ScoreBound.Upper);
            AssertValue("-\t009", -9, ScoreBound.Exact);

            Assert.Catch<OverflowException>(() => Score.ParseValue("44444444444444444", Colour.Black));
            Assert.Catch<OverflowException>(() => Score.ParseValue("-44444444444444444", Colour.White));
            Assert.Catch(() => Score.ParseValue("tus", Colour.Black));
            Assert.Catch(() => Score.ParseValue("テスト", Colour.White));
        }

        private void AssertMate(string text, int mate, bool isWin)
        {
            var score = Score.ParseMate(text, Colour.Black);
            Assert.AreEqual(ScoreType.Mate, score.ScoreType);
            Assert.AreEqual(mate, score.Mate);
            Assert.AreEqual(isWin, score.IsMateWin);
            Assert.AreEqual((Score.MateScore - mate) * (isWin ? +1 : -1), score.Value);

            score.Neg();
            Assert.AreEqual(mate, score.Mate);
            Assert.AreEqual(!isWin, score.IsMateWin);
            Assert.AreEqual((Score.MateScore - mate) * (isWin ? -1 : +1), score.Value);

            score.Neg();
            Assert.AreEqual(mate, score.Mate);
            Assert.AreEqual(!!isWin, score.IsMateWin);
            //Assert.AreEqual(text, score.Text);
            Assert.AreEqual((Score.MateScore - mate) * (isWin ? +1 : -1), score.Value);
        }

        [Test()]
        public void ParseMateTest()
        {
            AssertMate("+4", 4, true);
            AssertMate("-54", 54, false);
            AssertMate("+", 0, true);
            AssertMate("-", 0, false);

            AssertMate("32", 32, true);

            AssertMate("+3dts", 3, true);

            Assert.Catch(() => Score.ParseMate("dde+3dts", Colour.Black));
        }

        [Test()]
        public void WinRateTest()
        {
            var Delta = 1e-5;

            Assert.AreEqual(100.0,
                Score.CreateMate(Colour.Black, 1, true).WinRate,
                Delta);
            Assert.AreEqual(100.0,
                Score.CreateValue(Colour.Black, 3000).WinRate,
                1.0);
            Assert.AreEqual(100.0,
                Score.CreateValue(Colour.White, 3000).WinRate,
                1.0);

            Assert.AreEqual(0.0,
                Score.CreateMate(Colour.White, 1, false).WinRate,
                Delta);
            Assert.AreEqual(0.0,
                Score.CreateValue(Colour.White, -3000).WinRate,
                1.0);

            Assert.AreEqual(50.0,
                Score.CreateValue(Colour.Black, 0).WinRate,
                Delta);
            Assert.AreEqual(50.0,
                Score.CreateValue(Colour.White, 0).WinRate,
                Delta);
        }
    }
}
#endif
