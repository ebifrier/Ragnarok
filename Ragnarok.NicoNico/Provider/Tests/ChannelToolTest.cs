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
        /// GetUploadedVideoListのテスト
        /// </summary>
        [Test()]
        public void GetUploadedVideoListTest()
        {
            var pageContent = LoadTestPage("channelpage_sample.html");
            var list = ChannelTool.GetUploadedVideoList(pageContent);
            Assert.NotNull(list);
            Assert.AreEqual(2, list.Count());

            Assert.AreEqual(UploadedVideoStatus.Uploading, list[0].Status);
            Assert.AreEqual("so34006431", list[0].Id);
            Assert.AreEqual("57-07-752 小林泉美の囲碁初級講座「４路詰碁で脳トレーニング」4 ～めざせ！5級～ 石の連絡と切断(2)-n.mp4", list[0].Title);

            Assert.AreEqual(UploadedVideoStatus.Success, list[1].Status);
            Assert.AreEqual("so34006415", list[1].Id);
            Assert.AreEqual("57-07-754 小林泉美の囲碁初級講座「４路詰碁で脳トレーニング」6 ～めざせ！5級～ ウッテガエシ(2)-n.mp4", list[1].Title);

            // エラーなど
            list = ChannelTool.GetUploadedVideoList("");
            Assert.NotNull(list);
            Assert.AreEqual(0, list.Count());
            
            Assert.Catch<ArgumentNullException>(() => ChannelTool.GetUploadedVideoList(null));
        }
    }
}
#endif
