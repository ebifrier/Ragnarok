using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Ragnarok
{
    using Ragnarok.Utility;

    /// <summary>
    /// スタックトレースなどを表示してからロックします。
    /// </summary>
    /// <remarks>
    /// 必ずusingと一緒に使ってください。
    /// </remarks>
    public class DebugLock : IDisposable
    {
        private static long idCounter = 0;
        private object locker;
        private readonly long lockId;
        private readonly bool writeStackTrace = false;

        /// <summary>
        /// 次のIDを取得します。
        /// </summary>
        private static long GetNextId()
        {
            return Interlocked.Increment(ref idCounter);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DebugLock(object locker)
            : this(locker, false)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DebugLock(object locker, bool writeStackTrace)
        {
            this.lockId = GetNextId();
            this.writeStackTrace = writeStackTrace;

            if (writeStackTrace)
            {
                WriteStackTrace();
            }

#if true //DEBUG
            if (!Monitor.TryEnter(locker, TimeSpan.FromSeconds(30)))
            {
                Log.Error("DeadLock!!!");

                throw new InvalidOperationException(
                    "おそらくデッドロックしたと思われます。");
            }
#else
            Monitor.Enter(locker);
#endif

            this.locker = locker;
        }

        /// <summary>
        /// 全スレッドのスタックトレースを出力します。
        /// </summary>
        public void WriteStackTrace()
        {
#if !MONO
            var threads = Process.GetCurrentProcess().Threads;

            foreach (var th in threads)
            {
                var thread = th as Thread;
                if (thread == null)
                {
                    continue;
                }

                var stackTrace = new StackTrace(thread, true);

                var strBuilder = new StringBuilder();
                strBuilder.AppendLine("StackTrace:");

                foreach (var frame in StackTraceUtil.ToStackTraceString(stackTrace))
                {
                    strBuilder.AppendFormat(
                        "    {0}",
                        frame);
                    strBuilder.AppendLine();
                }

                Log.Info("begin lock {0}{1}{2}",
                    this.lockId,
                    Environment.NewLine,
                    strBuilder.ToString());
            }
#endif
        }

        /// <summary>
        /// ロックを解除します。
        /// </summary>
        public void Dispose()
        {
            var tmpLocker = Interlocked.Exchange(ref this.locker, null);
            if (tmpLocker == null)
            {
                throw new Exception(
                    "Disposeが２度呼び出されています。");
            }

            Monitor.Exit(tmpLocker);

            if (this.writeStackTrace)
            {
                Log.Debug("end lock {0}", this.lockId);
            }
        }
    }
}
