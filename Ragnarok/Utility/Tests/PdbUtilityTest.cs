#if !MONO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    [TestFixture()]
    public sealed class PdbUtilityTest
    {
        [Test()]
        public void GetAllThreadStackTraceTest()
        {
            var traceList = PdbUtility.GetAllThreadStackTrace();
            Assert.Greater(traceList.Count(), 0);
        }
    }
}
#endif
