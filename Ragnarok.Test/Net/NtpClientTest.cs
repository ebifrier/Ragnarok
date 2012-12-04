using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok.Net;

namespace Ragnarok.Test.Net
{
    [TestFixture()]
    public class NtpClientTest
    {
        [Test()]
        public void Test()
        {
            var time = NtpClient.GetTime();

            Console.WriteLine("{0}", time);
        }
    }
}
