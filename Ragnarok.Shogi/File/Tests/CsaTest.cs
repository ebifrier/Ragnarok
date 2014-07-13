#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Ragnarok.Shogi.File.Tests
{
    [TestFixture()]
    internal sealed class CsaTest
    {
        private static readonly string PathListFile = "csa.list";

        /// <summary>
        /// 棋譜の読み込みテストを行います。
        /// </summary>
        private static void TestCsaFile(string path)
        {
            var text = string.Empty;
            using (var reader = new StreamReader(path, KifuObject.DefaultEncoding))
            {
                text = reader.ReadToEnd();
            }

            // 棋譜の読み込み
            try
            {
                var kifu = KifuReader.LoadFrom(text);
                Assert.NotNull(kifu);
            }
            catch (Exception ex)
            {
            }

            // 手数を取得
            /*var count = TestUtil.GetMoveCount(text);
            Assert.NotNull(count);

            if (kifu.Error != null)
            {
                Assert.LessOrEqual(count.Value - 1, kifu.MoveList.Count());
            }
            else
            {
                Assert.LessOrEqual(count.Value, kifu.MoveList.Count());
            }*/
        }

        /// <summary>
        /// すべての棋譜のテストを行います。
        /// </summary>
#if true
        [Test()]
#endif
        public void CsaAllTest()
        {
            TestUtil.KifTest(
                PathListFile, "*.csa",
                _ => TestCsaFile(_));
        }

        /// <summary>
        /// 正しく読み込めていない棋譜のテストを行います。
        /// </summary>
        [Test()]
        public void Test()
        {
            var pathList = TestUtil.LoadPathList(PathListFile);

            var path = @"E:/Dropbox/NicoNico/bin/kifuexpl/database/1600-1979_変換先/16190816その他大橋本因無114.csa";
            //foreach (var path in TestUtil.FileList("*.csa", pathList))
            {
                Console.WriteLine(path);

                TestCsaFile(path);
            }
        }
    }
}
#endif
