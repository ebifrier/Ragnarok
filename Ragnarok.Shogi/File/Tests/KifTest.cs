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
    using Kif;

    [TestFixture()]
    internal sealed class KifTest
    {
        /// <summary>
        /// 棋譜から手数を取得します。
        /// </summary>
        public static int? GetMoveCount(string text)
        {
            var m = Regex.Match(text,
                string.Format(@"(\d+)\s+({0})", KifUtil.SpecialMoveText),
                RegexOptions.Multiline);
            if (!m.Success)
            {
                return null;
            }

            return int.Parse(m.Groups[1].Value);
        }

        /// <summary>
        /// 棋譜の読み込みテストを行います。
        /// </summary>
        private static void TestKif(string path)
        {
            var text = string.Empty;
            using (var reader = new StreamReader(path, KifuObject.DefaultEncoding))
            {
                text = reader.ReadToEnd();
            }

            // 棋譜の読み込み
            var kifu1 = KifuReader.LoadFrom(text);
            Assert.NotNull(kifu1);

            // 手数を取得
            var countObj = GetMoveCount(text);
            Assert.NotNull(countObj);

            var count = countObj.Value + (kifu1.Error != null ? -2 : -1);
            Assert.LessOrEqual(count, kifu1.MoveList.Count());

            // 棋譜の書き出し
            var wrote = KifuWriter.WriteTo(kifu1, KifuFormat.Kif);
            Assert.IsNotNullOrEmpty(wrote);

            // 棋譜の読み込み パート２
            var kifu2 = KifuReader.LoadFrom(wrote);
            Assert.NotNull(kifu1);

            // 読み込んだ棋譜の確認
            Assert.LessOrEqual(count, kifu2.MoveList.Count());

            // 局面の比較を行います。
            var board1 = kifu1.StartBoard.Clone();
            kifu1.MoveList.ForEach(_ => board1.DoMove(_));

            var board2 = kifu2.StartBoard.Clone();
            kifu2.MoveList.ForEach(_ => board2.DoMove(_));

            Assert.True(Board.BoardEquals(kifu1.StartBoard, kifu2.StartBoard));
            Assert.True(kifu1.RootNode.NodeEquals(kifu2.RootNode));
            Assert.True(Board.BoardEquals(board1, board2));
        }

        /// <summary>
        /// すべての棋譜のテストを行います。
        /// </summary>
#if false
        [Test()]
#endif
        public void AllKifTest()
        {
            TestUtil.KifTest(
                "file.list", "*.kif",
                _ => TestKif(_));
        }

        /// <summary>
        /// 正しく読み込めていない棋譜のテストを行います。
        /// </summary>
        [Test()]
        public void Test()
        {
            var pathList = TestUtil.LoadPathList("file.list");

            //var path = @"E:\Dropbox\NicoNico\shogi\test_kif\1600-1979\kif\18720111その他大矢小野有105.KIF";
            foreach (var path in TestUtil.FileList("*.kif", pathList))
            {
                Console.WriteLine(path);

                TestKif(path);
            }
        }

        /// <summary>
        /// 変化が含まれる棋譜のテスト
        /// </summary>
        [Test()]
        public void VariationTest()
        {
            var path = @"E:\Dropbox\NicoNico\shogi\test_kif\variation.kif_";

            TestKif(path);
        }
    }
}
#endif
