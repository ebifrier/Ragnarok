#if !MONO
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    [TestFixture()]
    public sealed class PdbUtilityTest
    {
        [Test()]
        public void GetThreadListTest()
        {
            var threadList = PdbUtility.GetThreadList();
            Assert.Greater(threadList.Count(), 0);

            var found = threadList
                .SelectMany(_ => _.StackTrace)
                .Where(_ => _.Contains(nameof(GetThreadListTest)))
                .Any();
            Assert.True(found,
                $"スタックトレースに{nameof(GetThreadListTest)}が見つかりませんでした。");
        }
    }
}
#endif
