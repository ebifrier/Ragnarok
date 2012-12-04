using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;

using Ragnarok.Net;
using Ragnarok.NicoNico;
using Ragnarok.NicoNico.Live;

namespace Ragnarok.Test.NicoNico
{
    [TestFixture()]
    public class PlayerStatusTest
    {
        [Test()]
        public void Test()
        {
            var cc = new CookieContainer();

            var text = WebUtil.RequestHttpText(
                NicoString.LoginUrl(),
                NicoString.MakeLoginData("chatchan@hotmail.co.jp", "freedom0"),
                cc,
                Encoding.UTF8);

            var playerStatus = PlayerStatus.Create(
                "http://live.nicovideo.jp/watch/lv36651588",
                cc);
        }
    }
}
