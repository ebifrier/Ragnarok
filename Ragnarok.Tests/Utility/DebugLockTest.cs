#if !MONO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    [TestFixture()]
    public sealed class DebugLockTest
    {
        private readonly object locker = new();
        private readonly ManualResetEventSlim ev = new();

        [Test()]
        public void GetStackTraceTest()
        {
            var stacktrace = DebugLock.GetStackTrace();

            Assert.Greater(stacktrace.Count, 0);
            var found = stacktrace
                .Where(_ => _.Contains(nameof(GetStackTraceTest)))
                .Any();
            Assert.True(found, $"{nameof(GetStackTraceTest)}が見つかりません。");
        }

        private void ThreadMain()
        {
            using var dlock1 = new DebugLock(this.locker);

            this.ev.Set();
            Thread.Sleep(120 * 1000);
        }

        [Test()]
        public void LockTest()
        {
            using (new DebugLock(this.locker))
            {
            }

            var thread = new Thread(ThreadMain)
                {
                    Name = nameof(ThreadMain),
                    IsBackground = true,
                };
            thread.Start();
            this.ev.Wait();

            Assert.Catch<InvalidOperationException>(() =>
                new DebugLock(this.locker, 5));
        }
    }
}
#endif
