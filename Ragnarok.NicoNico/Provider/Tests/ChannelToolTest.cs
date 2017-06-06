#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.NicoNico.Provider.Tests
{
    [TestFixture()]
    internal sealed class ChannelToolTest
    {
        private static readonly string UploadedListText =
            @"			<li class=""video so28459181" +
            @"	not_contributed	ppv_type_0"">" +
            @"" +
            @"					<p class=""info"">" +
            @"							<a href=""http://www.nicovideo.jp/watch/so28459181""" +
            @"				target=""_blank"" class=""icon watch optional ghost""" +
            @"				>so28459181</a>" +
            @"						<span class=""optional"" style=""visibility:hidden"">ID先払</span>" +
            @"						</p>" +
            @"		" +
            @"		<div class=""video_left"">" +
            @"			<div class=""thumbnail_container thumbnail_queued"">" +
            @"				<span class=""thumb_wrapper thumb_100"">" +
            @"				" +
            @"				<img class=""thumbnail""" +
            @"				title=""57-50-592 棋力向上委員会 The PASSION！！592-n.mp4""" +
            @"				src=""/video/thumbnail_display?channel_id=2587372&fileid=28459181&num=0"">" +
            @"				</span>" +
            @"			</div>" +
            @"		</div>" +
            @"		<div class=""video_right"">" +
            @"			" +
            @"			<h6 class=""video_title"" title=""57-50-592 棋力向上委員会 The PASSION！！592-n.mp4"">" +
            @"								57-50-592 棋力向上委員会 The PASSION！！592-n.mp4" +
            @"			</h6>" +
            @"" +
            @"							<p class=""info"">処理順番待ち</p>" +
            @"						" +
            @"			" +
            @"			<menu class=""command"" id=""menu_command_28459181"">" +
            @"				<ul>" +
            @"		<li class=""minor"">" +
            @"		<a class=""icon delete""" +
            @"				href=""javascript://削除""" +
            @"				fileid=""28459181"" video_title=""57-50-592 棋力向上委員会 The PASSION！！592-n.mp4"">" +
            @"		削除" +
            @"		</a>" +
            @"	</li>" +
            @"" +
            @"	</ul>" +
            @"" +
            @"			</menu>" +
            @"" +
            @"			" +
            @"			" +
            @"			" +
            @"		</div>" +
            @"	</li>" +
            @"" +
            @"" +
            @"			<li class=""video so28459142" +
            @"	not_contributed ppv_type_0"">" +
            @"" +
            @"					<p class=""info"">" +
            @"							<a href=""http://www.nicovideo.jp/watch/so28459142""" +
            @"				target=""_blank"" class=""icon watch optional ghost""" +
            @"				>so28459142</a>" +
            @"						<span class=""optional"" style=""visibility:hidden"">ID先払</span>" +
            @"						</p>" +
            @"		" +
            @"		<div class=""video_left"">" +
            @"			<div class=""thumbnail_container thumbnail_encoded"">" +
            @"				<span class=""thumb_wrapper thumb_100"">" +
            @"				" +
            @"				<img class=""thumbnail""" +
            @"				title=""57-50-592 棋力向上委員会 The PASSION！！592-n.mp4""" +
            @"				src=""/video/thumbnail_display?channel_id=2587372&fileid=28459142&num=0"">" +
            @"				</span>" +
            @"			</div>" +
            @"		</div>" +
            @"		<div class=""video_right"">" +
            @"			" +
            @"			<h6 class=""video_title"" title=""57-50-592 棋力向上委員会 The PASSION！！592-n.mp4"">" +
            @"								57-50-592 棋力向上委員会 The PASSION！！592-n.mp4" +
            @"			</h6>" +
            @"" +
            @"						" +
            @"			" +
            @"			<menu class=""command"" id=""menu_command_28459142"">" +
            @"				<ul>" +
            @"			<li>" +
            @"			<a href=""video_edit?channel_id=2587372&amp;fileid=28459142&amp;pageID=""" +
            @"			class=""button"">" +
            @"			<img src=""/img/icon_edit.gif"" width=""16""" +
            @"			>編集</a>" +
            @"		</li>" +
            @"" +
            @"									" +
            @"		<li class=""minor"">" +
            @"		<a class=""icon delete""" +
            @"				href=""javascript://削除""" +
            @"				fileid=""28459142"" video_title=""57-50-592 棋力向上委員会 The PASSION！！592-n.mp4"">" +
            @"		削除" +
            @"		</a>" +
            @"	</li>" +
            @"" +
            @"	</ul>" +
            @"" +
            @"			</menu>" +
            @"" +
            @"							" +
            @"							" +
            @"			" +
            @"			" +
            @"		</div>" +
            @"	</li>";

        /// <summary>
        /// GetUploadedVideoListのテスト
        /// </summary>
        [Test()]
        public void GetUploadedVideoListTest()
        {
            var list = ChannelTool.GetUploadedVideoList(UploadedListText);
            Assert.NotNull(list);
            Assert.AreEqual(2, list.Count());

            Assert.AreEqual(UploadedVideoStatus.Uploading, list[0].Status);
            Assert.AreEqual("so28459181", list[0].Id);
            Assert.AreEqual("57-50-592 棋力向上委員会 The PASSION！！592-n.mp4", list[0].Title);

            Assert.AreEqual(UploadedVideoStatus.Success, list[1].Status);
            Assert.AreEqual("so28459142", list[1].Id);
            Assert.AreEqual("57-50-592 棋力向上委員会 The PASSION！！592-n.mp4", list[1].Title);

            // エラーなど
            list = ChannelTool.GetUploadedVideoList("");
            Assert.NotNull(list);
            Assert.AreEqual(0, list.Count());

            Assert.Catch<ArgumentNullException>(() => ChannelTool.GetUploadedVideoList(null));
        }
    }
}
#endif
