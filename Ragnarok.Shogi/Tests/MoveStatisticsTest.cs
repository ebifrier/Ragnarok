#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    [TestFixture()]
    public sealed class MoveStatisticsTest
    {
        [Test()]
        public void VoteTest()
        {
            /*var statistics = new MoveStatistics();
            var move1 = ShogiParser.ParseMove("33K", true);
            var move2 = ShogiParser.ParseMove("51GIN", true);

            statistics.Vote(
                new ShogiPlayer { PlayerId = "0", },
                move1,
                DateTime.Now);
            statistics.Vote(
                new ShogiPlayer { PlayerId = "1", },
                move2,
                DateTime.Now);

            var timestamp1 = DateTime.Now;
            statistics.Vote(
                new ShogiPlayer { PlayerId = "2", },
                move1,
                timestamp1);

            var timestamp2 = DateTime.Now + TimeSpan.FromSeconds(1);
            statistics.Vote(
                new ShogiPlayer { PlayerId = "3", },
                move2,
                timestamp2);

            var expect = new MovePointPair[]
            {
                new MovePointPair
                {
                    Move = move2,
                    Point = 20,
                    Timestamp = timestamp2,
                },
                new MovePointPair
                {
                    Move = move1,
                    Point = 20,
                    Timestamp = timestamp1,
                },
            };
            var value = statistics.MoveList.ToArray();

            value.ForEachWithIndex((_, i) =>
                Assert.AreEqual(expect[i], _));*/
        }
    }
}
#endif
