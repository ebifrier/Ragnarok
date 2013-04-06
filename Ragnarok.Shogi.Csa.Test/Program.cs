using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ragnarok.Shogi.Csa.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new CsaClient();
            client.Received += (_, __) => Console.Write("> {0}", __.Line);
            client.Sent += (_, __) => Console.Write("< {0}", __.Line);

            var th = new Thread(InputThread)
            {
                IsBackground = true,
            };
            th.Start(client);

            client.Connect("garnet-alice.net", 4081);
            client.Login("xxx", "test-99999-99999");

            var game = client.WaitGameSummary();
            if (game != null)
            {
                client.StartGame(game);
            }

            while (true)
            {
                var command = client.WaitGameCommand();
                if (command == null)
                {
                    client.Close();
                    break;
                }

                if (command.Error != null)
                {
                    Console.WriteLine("ERROR: {0}", command.Error);
                    continue;
                }

                if (command.Result != null)
                {
                    client.Close();
                    break;
                }
            }
        }

        static void InputThread(object state)
        {
            var client = (CsaClient)state;

            while (true)
            {
                var line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }

                client.WriteLine(line);
            }
        }
    }
}
