#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;

namespace Ragnarok.NicoNico.Provider.Tests
{
    using Video;

    [TestFixture()]
    internal class ChannelToolEditTest
    {
        private const int ChannelId = 2587372;
        private const string VideoId = "so27075556";
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
            var path = System.IO.Path.Combine(dir, filename);

            return System.IO.File.ReadAllLines(path);
        }

        /// <summary>
        /// 各サイトの動画タイトルや動画詳細には'記号が使えないため、全幅文字に変換します。 
        /// </summary>
        private static string ReplaceQuote(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text.Replace("'", "’");
        }

        private void EditAndFetchTest(VideoData expectedVideo)
        {
            var param = ChannelTool.CreateDefaultPostParam();
            param["initialized"] = 1;
            param["title"] = expectedVideo.Title;
            param["description"] = expectedVideo.Description;
            param["thmb_path"] = 13; // サムネイルはアップしたものを使う

            param["is_apply_tag"] = 1;
            param["tag1"] = "ゲーム";
            var tags = expectedVideo.TagList;
            for (var i = 0; i <= 8; ++i)
            {
                param[$"tag{i + 2}"] = (
                    i < tags.Count() && !string.IsNullOrEmpty(tags[i]) ?
                    ReplaceQuote(tags[i]) : "");
            }

            param["specify_uploaddate"] = "";
            param["uploaddate"] = expectedVideo.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            param["visible_start_time"] = expectedVideo.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            param["visible_end_time"] = expectedVideo.StartTime.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            param["is_nicos"] = "0";
            param["hide_flag"] = 1; // テストなので非表示にする

            Assert.NotNull(cc);
            ChannelTool.RequestEdit(ChannelId, VideoId, param, cc);
        }

        [Test]
        public void EditTest()
        {
            var expectedVideo = new VideoData
            {
                Title = "test TEST3",
                Description =
                    "対穴熊△3三角 講義<br>講師：藤井猛\n" +
                    "\n" +
                    "テストサン23",
                TagList = new List<string> { "将棋", "テスト", "Abcd", "比較" },
                StartTime = new DateTime(2016, 10, 1, 0, 0, 0),
            };

            EditAndFetchTest(expectedVideo);
        }
    }
}
#endif
