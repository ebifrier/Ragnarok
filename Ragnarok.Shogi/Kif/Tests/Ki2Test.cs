#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Ragnarok.Shogi.Kif.Tests
{
    using File.Tests;

    [TestFixture()]
    internal sealed class Ki2Test
    {
        /// <summary>
        /// 棋譜から手数を取得します。
        /// </summary>
        public static int? GetMoveCount(string text)
        {
            var m = Regex.Match(text, @"^\s*まで.*?(\d+)手", RegexOptions.Multiline);
            if (!m.Success)
            {
                m = Regex.Match(text, @"(\d+)手まで", RegexOptions.Multiline);
                if (!m.Success)
                {
                    return null;
                }
            }

            return int.Parse(m.Groups[1].Value);
        }

        /// <summary>
        /// 棋譜の読み込み＆書き込みテストを行います。
        /// </summary>
        private static void TestKif(string text)
        {
            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(text);
            Assert.NotNull(kifu);

            // 手数を取得
            var countObj = GetMoveCount(text);
            //Assert.NotNull(countObj);

            var count = (countObj.HasValue ? countObj.Value : kifu.MoveList.Count()) +
                        (kifu.Error != null ? -1 : 0);
            Assert.LessOrEqual(count, kifu.MoveList.Count());

            // 入出力テストを行います。
            TestUtil.ReadWriteTest(kifu, KifuFormat.Ki2, count);
        }

        /// <summary>
        /// 棋譜のテストを行います。
        /// </summary>
        [Test()]
        public void NormalTest()
        {
            TestKif(SampleKif.Get("Ki2_Test1.KI2"));
            TestKif(SampleKif.Get("Ki2_Test2.KI2"));
            TestKif(SampleKif.Get("Ki2_Test3.KI2"));
        }

        /// <summary>
        /// 棋譜のテストを行います。
        /// </summary>
        [Test()]
        public void IllegalTest()
        {
            TestKif(SampleKif.Get("Ki2_Illegal.KI2"));
        }

        /// <summary>
        /// 改行のみの行がない棋譜の読み込みテスト
        /// </summary>
        [Test()]
        public void NoNewlineTest()
        {
            var text =
                "先手：谷川浩司\n" +
                "後手：田中寅彦\n" +
                "▲７六歩";

            var kifu = KifuReader.LoadFrom(text);
            Assert.NotNull(kifu);
            Assert.AreEqual(1, kifu.MoveList.Count());

            TestKif(text);
        }

        /// <summary>
        /// 不正な指し手が含まれる棋譜のテスト
        /// </summary>
        [Test()]
        public void ErrorTest()
        {
            var sample = SampleKif.Get("Ki2_Error.ki2");

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(sample);
            Assert.NotNull(kifu);
            Assert.NotNull(kifu.Error);

            Assert.AreEqual(65, kifu.MoveList.Count());
            Assert.IsInstanceOf(
                typeof(File.FileFormatException),
                kifu.Error);
        }
    }
}
#endif
