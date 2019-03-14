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

            Assert.AreEqual("so34766508", list[0].Id);
            Assert.AreEqual("1552542002", list[0].ThreadId);
            Assert.AreEqual("57-74-865 第27期 銀河戦 本戦Dブロック 7回戦 佐々木勇気七段 vs 八代弥六段-n", list[0].Title);
            Assert.AreEqual(new DateTime(2019, 3, 12, 20, 41, 0), list[0].StartTime);
            Assert.False(list[0].IsVisible);
            Assert.False(list[0].IsMemberOnly);

            Assert.AreEqual("so34747664", list[1].Id);
            Assert.AreEqual("1552348983", list[1].ThreadId);
            Assert.AreEqual("【将棋】第27期 銀河戦 本戦Cブロック 7回戦 阿部隆八段 vs 青嶋未来五段", list[1].Title);
            Assert.AreEqual(new DateTime(2019, 3, 26, 20, 0, 0), list[1].StartTime);
            Assert.True(list[1].IsVisible);
            Assert.True(list[1].IsMemberOnly);

            Assert.AreEqual("so34492741", list[19].Id);
            Assert.AreEqual("1547811362", list[19].ThreadId);
            Assert.AreEqual("【将棋】第27期 銀河戦 本戦Aブロック 5回戦 堀口一史座七段 vs 折田翔吾アマ", list[19].Title);
            Assert.AreEqual(new DateTime(2019, 1, 22, 20, 0, 0), list[19].StartTime);
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
