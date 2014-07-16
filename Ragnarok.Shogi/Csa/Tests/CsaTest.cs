#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Ragnarok.Shogi.Csa.Tests
{
    using File.Tests;

    [TestFixture()]
    internal sealed class CsaTest
    {
        private static readonly string PathListFile = "csa.list";

        private static int? GetMoveCount(string path)
        {
            var dir = Path.GetDirectoryName(path);
            var filename = Path.GetFileNameWithoutExtension(path);
            var newPath = Path.Combine(dir, "..", filename + ".ki2");

            using (var reader = new StreamReader(newPath, KifuObject.DefaultEncoding))
            {
                var text = reader.ReadToEnd();

                return Kif.Tests.Ki2Test.GetMoveCount(text);
            }
        }

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
            var kifu = KifuReader.LoadFrom(text);
            Assert.NotNull(kifu);

            // 手数を取得
            var countObj = GetMoveCount(path);
            Assert.NotNull(countObj);

            var count = countObj.Value + (kifu.Error != null ? -1 : 0);
            Assert.LessOrEqual(count, kifu.MoveList.Count());

            // 入出力テストを行います。
            TestUtil.ReadWriteTest(kifu, KifuFormat.Csa, count);
        }

        /// <summary>
        /// すべての棋譜のテストを行います。
        /// </summary>
#if false
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

            //var path = @"E:/Dropbox/NicoNico/bin/kifuexpl/database/1600-1979/csa/16190816その他大橋本因無114.csa";
            foreach (var path in TestUtil.FileList("*.csa", pathList))
            {
                Console.WriteLine(path);

                TestCsaFile(path);
            }
        }
    }
}
#endif
