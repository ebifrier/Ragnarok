#if TESTS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Ragnarok.NicoNico.Provider.Tests
{
    [TestFixture()]
    internal sealed class ChannelToolTest
    {
        /// <summary>
        /// 指定の名前を持つ棋譜をリソースから読み込みます。
        /// </summary>
        public static string LoadTestPage(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var ns = typeof(ChannelToolTest).Namespace + ".";

            using (var stream = asm.GetManifestResourceStream(ns + resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        /// <summary>
        /// ParseVideoListTestのテスト
        /// </summary>
        [Test()]
        public void ParseVideoListTest()
        {
            var pageContent = LoadTestPage("videos_sample.html");
            var list = ChannelTool.ParseVideoList(pageContent);
            Assert.NotNull(list);
            Assert.AreEqual(20, list.Count());

            Assert.AreEqual("so34211191", list[0].Id);
            Assert.AreEqual("1542886922", list[0].ThreadId);
            Assert.AreEqual("【将棋】第27期 銀河戦 本戦Fブロック 3回戦 中村亮介六段 vs 及川拓馬六段", list[0].Title);
            Assert.AreEqual(new DateTime(2018, 12, 13, 20, 0, 0), list[0].StartTime);
            Assert.False(list[0].IsVisible);
            Assert.True(list[0].IsMemberOnly);

            Assert.AreEqual("so34210702", list[1].Id);
            Assert.AreEqual("1542881823", list[1].ThreadId);
            Assert.AreEqual("【将棋】第27期 銀河戦 本戦Eブロック 3回戦 阿部光瑠六段 vs 佐藤慎一五段", list[1].Title);
            Assert.AreEqual(new DateTime(2018, 12, 11, 20, 0, 0), list[1].StartTime);
            Assert.True(list[1].IsVisible);
            Assert.True(list[1].IsMemberOnly);

            Assert.AreEqual("so33924522", list[19].Id);
            Assert.AreEqual("1538011325", list[19].ThreadId);
            Assert.AreEqual("【将棋】第27期 銀河戦 本戦Dブロック 1回戦 古森悠太四段 vs 菅野倫太郎アマ", list[19].Title);
            Assert.AreEqual(new DateTime(2018, 10, 11, 20, 0, 0), list[19].StartTime);
            Assert.True(list[19].IsVisible);
            Assert.True(list[19].IsMemberOnly);

            // エラーなど
            list = ChannelTool.ParseVideoList("");
            Assert.NotNull(list);
            Assert.AreEqual(0, list.Count());

            Assert.Catch<ArgumentNullException>(() => ChannelTool.ParseVideoList(null));
        }

        /// <summary>
        /// ParseUploadedVideoListのテスト
        /// </summary>
        [Test()]
        public void ParseUploadedVideoListTest()
        {
            var pageContent = LoadTestPage("uploads_sample.html");
            var list = ChannelTool.ParseUploadedVideoList(pageContent);
            Assert.NotNull(list);
            Assert.AreEqual(2, list.Count());

            Assert.AreEqual(UploadedVideoStatus.Uploading, list[0].Status);
            Assert.AreEqual("so34006431", list[0].Id);
            Assert.AreEqual("57-07-752 小林泉美の囲碁初級講座「４路詰碁で脳トレーニング」4 ～めざせ！5級～ 石の連絡と切断(2)-n.mp4", list[0].Title);

            Assert.AreEqual(UploadedVideoStatus.Success, list[1].Status);
            Assert.AreEqual("so34006415", list[1].Id);
            Assert.AreEqual("57-07-754 小林泉美の囲碁初級講座「４路詰碁で脳トレーニング」6 ～めざせ！5級～ ウッテガエシ(2)-n.mp4", list[1].Title);

            // エラーなど
            list = ChannelTool.ParseUploadedVideoList("");
            Assert.NotNull(list);
            Assert.AreEqual(0, list.Count());
            
            Assert.Catch<ArgumentNullException>(() => ChannelTool.ParseUploadedVideoList(null));
        }
    }
}
#endif
