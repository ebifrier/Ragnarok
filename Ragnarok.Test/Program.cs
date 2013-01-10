using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var ntpTest = new Net.NtpClientTest();
            ntpTest.Test();

            var pbTest = new Net.ProtoBufTest();
            pbTest.BigDataTest();
        }
    }
}
