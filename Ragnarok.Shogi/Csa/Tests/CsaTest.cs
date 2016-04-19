#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ragnarok.Shogi.Csa.Tests
{
    using File.Tests;

    [TestFixture()]
    internal sealed class CsaTest
    {
        /// <summary>
        /// 指定の名前を持つ棋譜をリソースから読み込みます。
        /// </summary>
        public static string Get(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var ns = typeof(CsaTest).Namespace + ".";

            using (var stream = asm.GetManifestResourceStream(ns + resourceName))
            using (var reader = new StreamReader(stream, KifuObject.DefaultEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        private static string RemoteWasted(string csa, bool removeTime)
        {
            var initialBoard =
                "P1-KY-KE-GI-KI-OU-KI-GI-KE-KY\n" +
                "P2 * -HI *  *  *  *  * -KA * \n" +
                "P3-FU-FU-FU-FU-FU-FU-FU-FU-FU\n" +
                "P4 *  *  *  *  *  *  *  *  * \n" +
                "P5 *  *  *  *  *  *  *  *  * \n" +
                "P6 *  *  *  *  *  *  *  *  * \n" +
                "P7+FU+FU+FU+FU+FU+FU+FU+FU+FU\n" +
                "P8 * +KA *  *  *  *  * +HI * \n" +
                "P9+KY+KE+GI+KI+OU+KI+GI+KE+KY";

            csa = csa.Replace(initialBoard, "PI");
            csa = csa.Replace(initialBoard.Replace("\n", "\r\n"), "PI");
            csa = Regex.Replace(csa, @"^'.*$", "", RegexOptions.Multiline);
            if (removeTime)
            {
                csa = Regex.Replace(csa, @"^T.*$", "", RegexOptions.Multiline);
            }

            return csa.RemoveWhitespace();
        }

        /// <summary>
        /// 文字列を空白を考慮に入れずに比較します。
        /// </summary>
        public static void CompareCsaKif(string expected, string actual, bool removeTime)
        {
            if (expected == null || actual == null)
            {
                Assert.True(ReferenceEquals(expected, actual));
            }
            else
            {
                Assert.AreEqual(
                    RemoteWasted(expected, removeTime),
                    RemoteWasted(actual, removeTime));
            }
        }

        [Test()]
        public void NormalTest()
        {
            var content =
                "V2.2\n" +
                "N+大橋宗桂\n" +
                "N-本因坊算砂\n" +
                "PI\n+\n" +
                "+7776FU\n-3334FU\n+6766FU\n-7162GI\n+7978GI\n\n" +
                "-5354FU\n+5756FU\n-3142GI\n+7867GI\n-6253GI\n\n" +
                "+3948GI\n-4344FU\n+4746FU\n-4243GI\n+4857GI\n\n" +
                "-3435FU\n+2726FU\n-4334GI\n+2625FU\n-2233KA\n\n" +
                "+4938KI\n-8222HI\n+3847KI\n-5162OU\n+5968OU\n\n" +
                "-6272OU\n+6878OU\n-4152KI\n+6968KI\n-3351KA\n\n" +
                "+6665FU\n-2324FU\n+2524FU\n-5124KA\n+3736FU\n\n" +
                "-0025FU\n+3635FU\n-2435KA\n+4736KI\n-3526KA\n\n" +
                "+0027FU\n-2659UM\n+4645FU\n-2133KE\n+4544FU\n\n" +
                "-0035FU\n+3646KI\n-2526FU\n+2726FU\n-5926UM\n\n" +
                "+0036FU\n-2627UM\n+2848HI\n-3536FU\n+0035FU\n\n" +
                "-3425GI\n+0023FU\n-2223HI\n+4443TO\n-5243KI\n\n" +
                "+4636KI\n-0044FU\n+3637KI\n-2745UM\n+4828HI\n\n" +
                "-0026FU\n+5746GI\n-4342KI\n+4645GI\n-3345KE\n\n" +
                "+3746KI\n-2627TO\n+2827HI\n-2526GI\n+2747HI\n\n" +
                "-0038GI\n+4748HI\n-4537NK\n+2937KE\n-2637GI\n\n" +
                "%TORYO\n";

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(content);
            Assert.NotNull(kifu);

            // 書き込みテスト
            var kif = KifuWriter.WriteTo(kifu, KifuFormat.Csa);
            CompareCsaKif(content, kif, true);
        }

        [Test()]
        public void NormalTest2()
        {
            var content =
                "V2.2\n" +
                "PI\n-\n";

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(content);
            Assert.NotNull(kifu);

            // 書き込みテスト
            var kif = KifuWriter.WriteTo(kifu, KifuFormat.Csa);
            CompareCsaKif(content, kif, true);
        }

        [Test()]
        public void TimeTest()
        {
            var timeSeconds = new int[]
            {
                 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
                13,  1, 19, 21, 27, 15, 13,  6, 14, 15,
                13,  6, 11,  7, 19,  5, 17, 18,  1, 17,
                 1, 16, 22, 16, 21, 16,  1, 16,  1, 16,
                13,  6,  8, 12, 13,  7,  7, 13, 15, 16,
                12,  9,  2, 17,  9, 17, 12,  9, 41, 25,
                 2, 17, 42,  1, 30,  1, 44,  1, 10, 19,
                 1, 18,  1, 19, 40,  1, 15, 31,  1, 18,
                17,  1, 44, 19,  1, 18, 12, 22, 16, 19,
                 1, 19,  1, 18,  9, 19, 27, 18,  1, 18,
                25, 19,  1, 18, 17, 22, 12,  8,  1, 17,
                30,  1, 16,  1,  1,  1,  1,  2,  1,  1,
                 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
                 0, // 投了分
            };
            var sample = Get("SampleCsa.csa");

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(sample);
            Assert.NotNull(kifu);
            Assert.AreEqual(timeSeconds.Count(), kifu.MoveList.Count());

            // 消費時間の比較
            var node = kifu.RootNode;
            for (var i = 0; i < timeSeconds.Count(); ++i)
            {
                node = node.NextNode;
                Assert.AreEqual(timeSeconds[i], node.DurationSeconds);
            }

            // 書き込みテスト
            var kif = KifuWriter.WriteTo(kifu, KifuFormat.Csa);
            CompareCsaKif(sample, kif, false);
        }
    }
}
#endif
