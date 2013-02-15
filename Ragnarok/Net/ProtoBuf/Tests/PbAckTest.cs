#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;

namespace Ragnarok.Net.ProtoBuf.Tests
{
    /// <summary>
    /// テスト用のサーバークラス。
    /// </summary>
    internal sealed class PbAckServer
    {
        private PbConnection connection;

        private PbConnection Accept()
        {
            using (var socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 5398));
                socket.Listen(5);

                var client = socket.Accept();

                return new PbConnection
                {
                    ProtocolVersion = new PbProtocolVersion(1, 0),
                }
                .Apply(_ => _.SetSocket(client));
            }
        }

        public void Start()
        {
            this.connection = Accept();
        }
    }

    /// <summary>
    /// テスト用のクライアントクラス。
    /// </summary>
    internal sealed class PbAckClient
    {
        public void Start()
        {
            var connection = new PbConnection
            {
                ProtocolVersion = new PbProtocolVersion(1, 0),
            };
            connection.Connect("localhost", 5398);

            connection.CheckProtocolVersion(TimeSpan.FromSeconds(30));

            Thread.Sleep(5 * 60 * 1000);
        }
    }

    [TestFixture()]
    internal sealed class PbAckTest
    {
        [Test()]
        public void AckTest()
        {
            // 別スレッドでサーバー処理を開始。
            var server = new PbAckServer();
            ThreadPool.QueueUserWorkItem(_ => server.Start());

            // クライアントの処理を開始。
            var client = new PbAckClient();
            client.Start();
        }
    }
}
#endif
