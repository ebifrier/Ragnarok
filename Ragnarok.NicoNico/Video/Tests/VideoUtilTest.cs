#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;

namespace Ragnarok.NicoNico.Video.Tests
{
    [TestFixture()]
    internal sealed class VideoUtilTest
    {
        /// <summary>
        /// GetVideoIdのテスト
        /// </summary>
        [Test()]
        public void GetVideoIdTest()
        {
            Assert.AreEqual("sm44422222222222222", VideoUtil.GetVideoId("sm44422222222222222"));
            Assert.AreEqual("so44422222", VideoUtil.GetVideoId("so44422222"));
            Assert.AreEqual("sm9", VideoUtil.GetVideoId("http://www.niconico.jp/sm9"));
            Assert.AreEqual("sm9", VideoUtil.GetVideoId("sm9 koreha"));
            Assert.AreEqual("123456", VideoUtil.GetVideoId("123456"));
            Assert.AreEqual("123456", VideoUtil.GetVideoId("http://www.niconico.jp/123456"));
            Assert.AreEqual("123456", VideoUtil.GetVideoId("http://www.niconico.jp/123456?eco=1"));
            Assert.AreEqual("123456", VideoUtil.GetVideoId("123456 xxx"));

            Assert.AreEqual(null, VideoUtil.GetVideoId("ss123456"));
            Assert.AreEqual(null, VideoUtil.GetVideoId("日本語"));
            Assert.AreEqual(null, VideoUtil.GetVideoId("xxxxxxxxxxxxxxxxx"));
            Assert.Catch(() => VideoUtil.GetVideoId(null));
            Assert.AreEqual(null, VideoUtil.GetVideoId("12x3456"));
        }
    }
}
#endif
