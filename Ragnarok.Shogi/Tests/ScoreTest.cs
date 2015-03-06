#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi
{
    [TestFixture()]
    internal sealed class ScoreTest
    {
        private void AssertValue(string text, int value, string negText)
        {
            var score = Score.ParseValue(text, BWType.Black);
            Assert.AreEqual(ScoreType.Value, score.ScoreType);
            Assert.AreEqual(value, score.Value);
            Assert.AreEqual(text, score.Text);

            score.Neg();
            Assert.AreEqual(-value, score.Value);
            Assert.AreEqual(negText, score.Text);

            score.Neg();
            Assert.AreEqual(value, score.Value);
            Assert.AreEqual(text.TrimStart('+'), score.Text);
        }

        [Test()]
        public void ParseValueTest()
        {
            AssertValue("+89", 89, "-89");
            AssertValue("-99999999", -99999999, "99999999");
            AssertValue("109", 109, "-109");

            AssertValue("7899↑", 7899, "-7899↑");
            AssertValue("-47↑", -47, "47↑");
            AssertValue("+ 987↑", 987, "- 987↑");
            AssertValue("-\t009", -9, "\t009");

            Assert.Catch<OverflowException>(() => Score.ParseValue("44444444444444444", BWType.Black));
            Assert.Catch<OverflowException>(() => Score.ParseValue("-44444444444444444", BWType.White));
            Assert.Catch(() => Score.ParseValue("tus", BWType.Black));
            Assert.Catch(() => Score.ParseValue("テスト", BWType.White));
        }

        private void AssertMate(string text, int mate, bool isWin)
        {
            var score = Score.ParseMate(text, BWType.Black);
            Assert.AreEqual(ScoreType.Mate, score.ScoreType);
            Assert.AreEqual(mate, score.Mate);
            Assert.AreEqual(isWin, score.IsMateWin);
            Assert.AreEqual(text, score.Text);
            Assert.AreEqual(Score.MateScore * (isWin ? +1 : -1), score.Value);

            score.Neg();
            Assert.AreEqual(mate, score.Mate);
            Assert.AreEqual(!isWin, score.IsMateWin);
            Assert.AreEqual(Score.MateScore * (isWin ? -1 : +1), score.Value);

            score.Neg();
            Assert.AreEqual(mate, score.Mate);
            Assert.AreEqual(!!isWin, score.IsMateWin);
            //Assert.AreEqual(text, score.Text);
            Assert.AreEqual(Score.MateScore * (isWin ? +1 : -1), score.Value);
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

            Assert.Catch(() => Score.ParseMate("dde+3dts", BWType.Black));
        }
    }
}
#endif
