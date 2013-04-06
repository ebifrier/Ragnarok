#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Csa.Tests
{
    [TestFixture()]
    internal sealed class CsaGameTest
    {
        [Test()]
        public void ParseTest()
        {
            var summary =
                "BEGIN Game_Summary\n" +
                "Protocol_Version:1.1\n" +
                "Protocol_Mode:Server\n" +
                "Format:Shogi 1.0\n" +
                "Declaration:Jishogi 1.1\n" +
                "Game_ID:20060505-CSA14-3-5-7\n" +
                "Name+:TANUKI\n" +
                "Name-:KITSUNE\n" +
                "Your_Turn:+\n" +
                "Rematch_On_Draw:NO\n" +
                "To_Move:+\n" +
                "BEGIN Time\n" +
                "Time_Unit:1sec\n" +
                "Total_Time:1500\n" +
                "Least_Time_Per_Move:1\n" +
                "END Time\n" +
                "BEGIN Position\n" +
                "P1-KY-KE-GI-KI-OU-KI-GI-KE-KY\n" +
                "P2 * -HI *  *  *  *  * -KA * \n" +
                "P3-FU-FU-FU-FU-FU-FU-FU-FU-FU\n" +
                "P4 *  *  *  *  *  *  *  *  * \n" +
                "P5 *  *  *  *  *  *  *  *  * \n" +
                "P6 *  *  *  *  *  *  *  *  * \n" +
                "P7+FU+FU+FU+FU+FU+FU+FU+FU+FU\n" +
                "P8 * +KA *  *  *  *  * +HI * \n" +
                "P9+KY+KE+GI+KI+OU+KI+GI+KE+KY\n" +
                "P+\n" +
                "P-\n" +
                "+\n" +
                "+2726FU,T12\n" +
                "-3334FU,T6\n" +
                "END Position\n" +
                "END Game_Summary\n";

            var game = new CsaGameInfo();
            game.Parse(summary);

            Assert.AreEqual("1.1", game.ProtocolVersion);
            Assert.AreEqual("Server", game.ProtocolMode);
            Assert.AreEqual("Shogi 1.0", game.Format);
            Assert.AreEqual("Jishogi 1.1", game.Declaration);
            Assert.AreEqual("20060505-CSA14-3-5-7", game.GameId);
            Assert.AreEqual("TANUKI", game.BlackName);
            Assert.AreEqual("KITSUNE", game.WhiteName);
            Assert.AreEqual(BWType.Black, game.MyTurn);
            Assert.AreEqual(BWType.Black, game.BeginTurn);
            Assert.AreEqual(TimeSpan.FromSeconds(1), game.TimeUnit);
            Assert.AreEqual(false, game.TimeRoundup);
            Assert.AreEqual(TimeSpan.FromSeconds(1), game.LeastTimePerMove);
            Assert.AreEqual(TimeSpan.Zero, game.Byoyomi);
            Assert.AreEqual(TimeSpan.FromSeconds(1500), game.TotalTime);
            Assert.AreEqual(true, game.IsGameSummaryEnded);
        }

        [Test()]
        public void ParseTest2()
        {
            var summary =
                "BEGIN Game_Summary\n" +
                "Protocol_Version:1.1\n" +
                "Protocol_Mode:\n" +
                "Format:Shogi 1.0\n" +
                "Declaration:\n" +
                "Game_ID:\n" +
                "Name+:_\n" +
                "Name-:-\n" +
                "Your_Turn:-\n" +
                "Rematch_On_Draw:\n" +
                "To_Move:-\n" +
                "BEGIN Time\n" +
                "Time_Unit:\n" +
                "Time_Roundup:\n" +
                "Least_Time_Per_Move:\n" +
                "END Time\n" +
                "BEGIN Position\n" +
                "P1-KY-KE-GI-KI-OU-KI-GI-KE-KY\n" +
                "P2 * -HI *  *  *  *  * -KA * \n" +
                "P3-FU-FU-FU-FU-FU-FU-FU-FU-FU\n" +
                "P4 *  *  *  *  *  *  *  *  * \n" +
                "P5 *  *  *  *  *  *  *  *  * \n" +
                "P6 *  *  *  *  *  *  *  *  * \n" +
                "P7+FU+FU+FU+FU+FU+FU+FU+FU+FU\n" +
                "P8 * +KA *  *  *  *  * +HI * \n" +
                "P9+KY+KE+GI+KI+OU+KI+GI+KE+KY\n" +
                "P+\n" +
                "P-\n" +
                "END Position\n" +
                "END Game_Summary\n";

            var game = new CsaGameInfo();
            game.Parse(summary);

            Assert.AreEqual("1.1", game.ProtocolVersion);
            Assert.AreEqual("Server", game.ProtocolMode);
            Assert.AreEqual("Shogi 1.0", game.Format);
            Assert.AreEqual(string.Empty, game.Declaration);
            Assert.AreEqual(string.Empty, game.GameId);
            Assert.AreEqual("_", game.BlackName);
            Assert.AreEqual("-", game.WhiteName);
            Assert.AreEqual(BWType.White, game.MyTurn);
            Assert.AreEqual(BWType.White, game.BeginTurn);
            Assert.AreEqual(TimeSpan.FromSeconds(1), game.TimeUnit);
            Assert.AreEqual(false, game.TimeRoundup);
            Assert.AreEqual(TimeSpan.Zero, game.LeastTimePerMove);
            Assert.AreEqual(TimeSpan.Zero, game.Byoyomi);
            Assert.AreEqual(TimeSpan.Zero, game.TotalTime);
            Assert.AreEqual(true, game.IsGameSummaryEnded);
        }
    }
}
#endif
