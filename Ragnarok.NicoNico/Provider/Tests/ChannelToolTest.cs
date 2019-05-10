﻿#if TESTS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using NUnit.Framework;

namespace Ragnarok.NicoNico.Provider.Tests
{
    [TestFixture()]
    internal sealed class ChannelToolTest
    {
        private const int ChannelId = 2587372;
        private CookieContainer cc;

        [SetUp()]
        public void Test()
        {
            var data = GetEmailPassword(".niconico.txt");

            cc = ChannelTool.Login(ChannelId, data[0], data[1]);
            Assert.NotNull(cc);
        }

        private static string[] GetEmailPassword(string filename)
        {
            var dir = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(dir, filename);

            return File.ReadAllLines(path);
        }

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

            Assert.AreEqual("so35102475", list[0].Id);
            Assert.AreEqual("1557456483", list[0].ThreadId);
            Assert.AreEqual("【将棋】将棋連盟が選ぶ 注目の一局#913 中飛車特集 丸山忠久九段 vs 佐々木慎六段", list[0].Title);
            Assert.AreEqual(new DateTime(2019, 5, 23, 18, 0, 0), list[0].StartTime);
            Assert.True(list[0].IsVisible);
            Assert.True(list[0].IsMemberOnly);

            Assert.AreEqual("so35032290", list[1].Id);
            Assert.AreEqual("1556772487", list[1].ThreadId);
            Assert.AreEqual("【将棋】お好み将棋道場 第261回 プロアマ指導対局(二枚落ち) 戸辺誠七段 vs 田口和博", list[1].Title);
            Assert.AreEqual(new DateTime(2019, 5, 25, 20, 0, 0), list[1].StartTime);
            Assert.True(list[1].IsVisible);
            Assert.True(list[1].IsMemberOnly);

            Assert.AreEqual("so34994914", list[19].Id);
            Assert.AreEqual("1555981813", list[19].ThreadId);
            Assert.AreEqual("浪花道場 3手詰#123", list[19].Title);
            Assert.AreEqual(new DateTime(2019, 5, 1, 0, 0, 0), list[19].StartTime);
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
            Assert.AreEqual("so35102761", list[0].Id);
            Assert.AreEqual("57-38-913 将棋連盟が選ぶ 注目の一局913 中飛車特集 丸山忠久九段 vs 佐々木慎六段-n.mp4", list[0].Title);

            Assert.AreEqual(UploadedVideoStatus.Success, list[1].Status);
            Assert.AreEqual("so35102755", list[1].Id);
            Assert.AreEqual("57-56-097 浪花道場 3手詰97-n.mp4", list[1].Title);

            // エラーなど
            list = ChannelTool.ParseUploadedVideoList("");
            Assert.NotNull(list);
            Assert.AreEqual(0, list.Count());
            
            Assert.Catch<ArgumentNullException>(() => ChannelTool.ParseUploadedVideoList(null));
        }

        /// <summary>
        /// Searchのテスト
        /// </summary>
        [Test()]
        public void SearchTest()
        {
            var list = ChannelTool.Search(cc, ChannelId, "", SearchOrder.BiggerFileId);
            Assert.NotNull(list);
            Assert.AreEqual(20, list.Count());

            foreach (var item in list)
            {
                Assert.NotNull(item.Id);
                Assert.NotNull(item.ThreadId);
                Assert.NotNull(item.Title);
            }

            //Assert.Catch<ArgumentNullException>(() => ChannelTool.ParseVideoList(null));
        }
    }
}
#endif
