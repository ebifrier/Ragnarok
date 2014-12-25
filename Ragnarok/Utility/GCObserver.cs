using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ragnarok.Utility
{
    public static class GCObserver
    {
        private static object SyncRoot = new object();
        private static Thread thread;
        private static volatile bool observing;

        /// <summary>
        /// GCの監視を開始します。
        /// </summary>
        public static void Start()
        {
#if MONO
            throw new NotImplementedException();
#else
            lock (SyncRoot)
            {
                if (observing)
                {
                    throw new InvalidOperationException(
                        "すでにGCObserverは開始されています。");
                }

                GC.RegisterForFullGCNotification(10, 10);

                thread = new Thread(ThreadMain)
                {
                    Name = "GCObserver",
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                };

                thread.Start();
            }
#endif
        }

        /// <summary>
        /// GCの監視を終了します。
        /// </summary>
        public static void Stop()
        {
#if MONO
            throw new NotImplementedException();
#else
            lock (SyncRoot)
            {
                if (!observing)
                {
                    return;
                }

                observing = false;

                if (thread != null)
                {
                    GC.CancelFullGCNotification();
                    thread.Join();
                    thread = null;
                }
            }
#endif
        }

#if !MONO
        /// <summary>
        /// GC監視用のメインスレッドです。
        /// </summary>
        private static void ThreadMain(object state)
        {
            observing = true;

            while (observing)
            {
                var s = GC.WaitForFullGCApproach(10 * 60 * 1000);
                if (s != GCNotificationStatus.Succeeded)
                {
                    if (observing) Thread.Sleep(200);
                    continue;
                }
                
                Log.Info("GC is approaching ... ");

                var status = GC.WaitForFullGCComplete(10 * 60 * 1000);
                if (status != GCNotificationStatus.Succeeded)
                {
                    if (observing) Thread.Sleep(200);
                    continue;
                }

                Log.Info("GC completed");
            }
        }
#endif
    }
}
