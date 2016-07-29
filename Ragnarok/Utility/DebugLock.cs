using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Ragnarok.Utility
{
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
        {
            this.lockId = GetNextId();

            if (!Monitor.TryEnter(locker, TimeSpan.FromSeconds(30)))
            {
                WriteStackTrace();

                throw new InvalidOperationException(
                    "おそらくデッドロックしたと思われます。");
            }

            this.locker = locker;
        }

        /// <summary>
        /// スタックトレースの出力を行います。
        /// </summary>
        public void WriteStackTrace()
        {
            var traceList =
#if !MONO
                PdbUtility.GetAllThreadStackTrace();
#else
                new List<string>();
#endif

            // ヘッダー
            traceList.Insert(0, "DeadLock!!! stacktrace");
            
            // カレントスレッドの情報を追加します。
            traceList.Add($"  thread {Environment.CurrentManagedThreadId}:");
            traceList.Add(Environment.StackTrace);

            Log.Error("{0}", string.Join(Environment.NewLine, traceList));
        }

        /// <summary>
        /// ロックを解除します。
        /// </summary>
        public void Dispose()
        {
            var tmpLocker = Interlocked.Exchange(ref this.locker, null);
            if (tmpLocker == null)
            {
                Log.Error("Disposeが２度呼び出されています。");
            }

            Monitor.Exit(tmpLocker);
        }
    }
}
