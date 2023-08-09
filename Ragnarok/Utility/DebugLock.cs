using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ragnarok.Utility
{
    /// <summary>
    /// ロックの確保に失敗した場合は、全スレッドのスタックトレースを出力します。
    /// </summary>
    /// <remarks>
    /// 必ずusingと一緒に使ってください。
    /// </remarks>
    public sealed class DebugLock : IDisposable
    {
        private object locker;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DebugLock(object locker, double seconds = 30)
        {
            if (!Monitor.TryEnter(locker, TimeSpan.FromSeconds(seconds)))
            {
                WriteStackTrace();

                throw new InvalidOperationException(
                    "おそらくデッドロックしたと思われます。");
            }

            this.locker = locker;
        }

        ~DebugLock()
        {
            Dispose(false);
        }

        /// <summary>
        /// ロックを解除します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// スタックトレースの出力を行います。
        /// </summary>
        public static void WriteStackTrace()
        {
            Log.Error("{0}",
                string.Join(Environment.NewLine, GetStackTrace()));
        }

        /// <summary>
        /// 全スレッドのスタックトレースを表示用に取得します。
        /// </summary>
        public static List<string> GetStackTrace()
        {
            var threadList =
#if !MONO
                PdbUtility.GetThreadList();
#else
                new List<PdbThread>();
#endif

            var messageList = new List<string>
            {
                "DeadLock!!! stacktrace",
                $"thread {Environment.CurrentManagedThreadId}:",
                Environment.StackTrace,
            };

            messageList.AddRange(threadList
                .SelectMany(_ =>
                    new List<string> { $"thread {_.ThreadID}:" }
                    .Concat(_.StackTrace)));

            return messageList;
        }

        /// <summary>
        /// ロックを解除します。
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                var tmp = Interlocked.Exchange(ref this.locker, null);
                if (tmp != null)
                {
                    Monitor.Exit(tmp);
                }
            }
        }
    }
}
