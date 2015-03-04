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
            var m = Regex.Match(text, @"^\s*まで.*(\d+)手", RegexOptions.Multiline);
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
        private static void TestKif(string path)
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
            var countObj = GetMoveCount(text);
            Assert.NotNull(countObj);

            var count = countObj.Value + (kifu.Error != null ? -1 : 0);
            Assert.LessOrEqual(count, kifu.MoveList.Count());

            // 入出力テストを行います。
            TestUtil.ReadWriteTest(kifu, KifuFormat.Ki2, count);
        }

        /// <summary>
        /// すべての棋譜のテストを行います。
        /// </summary>
#if false
        [Test()]
#endif
        public void Ki2AllTest()
        {
            TestUtil.KifTest(
                "file.list", "*.ki2",
                _ => TestKif(_));
        }

        /// <summary>
        /// 正しく読み込めていない棋譜のテストを行います。
        /// </summary>
        [Test()]
        public void KifTest()
        {
            /*var pathList = TestUtil.LoadPathList("file.list");

            //var path = @"E:/Dropbox/NicoNico/bin/kifuexpl/棋譜データベース/2005\20051017順位戦森下三浦無108.KI2";
            foreach (var path in TestUtil.FileList("*.ki2", pathList))
            {
                Console.WriteLine(path);

                TestKif(path);
            }*/
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
        }

        /// <summary>
        /// 不正な指し手が含まれる棋譜のテスト
        /// </summary>
        [Test()]
        public void ErrorTest()
        {
            var text =
                "開始日時：2004/06/22\n" +
                "棋戦：達人戦\n" +
                "戦型：先手三間飛車\n" +
                "先手戦術：早石田\n" +
                "後手戦術：角交換型\n" +
                "手順：▲７六歩△３四歩▲７五歩△４二玉▲７八飛△８八角成▲同銀△４五角\n" +
                "備考：▲升田式石田流\n" +
                "先手：谷川浩司\n" +
                "後手：田中寅彦\n" +
                "\n" +
                "場所：大阪「関西将棋会館」\n" +
                "持ち時間：3時間\n" +
                "*放映日：2004/07/09-16\n" +
                "*棋戦詳細：第12回富士通杯達人戦2回戦第3局\n" +
                "*「谷川浩司王位」vs「田中寅彦九段」\n" +
                "▲７六歩    △３四歩    ▲７五歩    △４二玉    ▲７八飛    △８八角成\n" +
                "▲同　銀    △４五角    ▲５八玉    △２七角成  ▲７四歩    △同　歩\n" +
                "▲５五角    △３三桂    ▲７四飛    △９二飛    ▲３四飛    △２二銀\n" +
                "▲３六歩    △５四歩    ▲７七角    △６二銀    ▲３八金    △２六馬\n" +
                "▲３五歩    △７二飛    ▲２八銀    △３六馬    ▲３七桂    △７七飛成\n" +
                "▲同　銀    △７六歩    ▲６六銀    △８八角    ▲２七銀    △同　馬\n" +
                "▲同　金    △９九角成  ▲５四飛    △５二香    ▲７四飛    △５五銀\n" +
                "▲同　銀    △同　馬    ▲６六銀    △７三銀    ▲３四飛    △４四馬\n" +
                "▲７四歩    △６四銀    ▲４四飛    △同　歩    ▲３四歩    △２九飛\n" +
                "▲１六角    △２五桂    ▲同　桂    △５七香成  ▲同　銀    △５六歩\n" +
                "▲３三歩成  △５一玉    ▲５四桂    △５七歩成  ▲同　玉    △５三銀上\n" +
                "▲４二角    △同　銀    ▲同　と    △同　金    ▲同桂成    △６二玉\n" +
                "▲５二飛\n" +
                "まで73手で先手の勝ち";

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(text);
            Assert.NotNull(kifu);
            Assert.NotNull(kifu.Error);

            Assert.AreEqual(65, kifu.MoveList.Count());
            Assert.IsInstanceOf(
                typeof(Ragnarok.Shogi.File.FileFormatException),
                kifu.Error);
        }
    }
}
#endif
