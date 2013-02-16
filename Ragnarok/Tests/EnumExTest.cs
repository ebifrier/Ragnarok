#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Tests
{
    [TestFixture()]
    internal class EnumExTest
    {
        [Test()]
        public void HasFlagTest()
        {
            var flag1 = FileAccess.Read | FileAccess.Write;
            var flag2 = FileAccess.Read;

            Assert.True(EnumEx.HasFlag(flag1, FileAccess.Read));
            Assert.True(EnumEx.HasFlag(flag1, FileAccess.Write));
            Assert.True(EnumEx.HasFlag(flag2, FileAccess.Read));
            Assert.False(EnumEx.HasFlag(flag2, FileAccess.Write));
        }
    }
}
#endif
