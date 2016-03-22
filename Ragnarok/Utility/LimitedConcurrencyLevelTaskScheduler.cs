using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 並列化数を絞って各タスクを並列実行します。
    /// </summary>
    public sealed class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        /// <summary>
        /// 現在のスレッドが処理を実行中かどうかです。
        /// </summary>
        [ThreadStatic]
        private static bool currentThreadIsProcessingItems;

        /// <summary>
        /// 実行すべきタスクのリスト
        /// </summary>
        private readonly LinkedList<Task> tasks = new LinkedList<Task>();

        /// <summary>
        /// 最大並列化数
        /// </summary>
        private readonly int maxDegreeOfParallelism;

        /// <summary>
        /// 現在実行中のタスクの数
        /// </summary>
        private int delegatesQueuedOrRunning = 0;

        /// <summary>
        /// 並列化最大数を指定してオブジェクトを作成します。
        /// </summary>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
            {
                throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            }

            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        /// <summary>
        /// サポートされている最大並列化数を取得します。
        /// </summary>
        public sealed override int MaximumConcurrencyLevel
        {
            get { return this.maxDegreeOfParallelism; }
        }

        /// <summary>
        /// キューにタスクを追加します。
        /// </summary>
        protected sealed override void QueueTask(Task task)
        {
            lock (this.tasks)
            {
                this.tasks.AddLast(task);

                // 処理中のタスクが少なければ、新たに処理を開始します。
                if (this.delegatesQueuedOrRunning < this.maxDegreeOfParallelism)
                {
                    ++this.delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <summary>
        /// ThreadPool上でタスクを実行します。
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                // このスレッド上でタスクを処理中なことを示します。
                currentThreadIsProcessingItems = true;

                try
                {
                    while (true)
                    {
                        Task item;
                        lock (this.tasks)
                        {
                            if (this.tasks.Count == 0)
                            {
                                --this.delegatesQueuedOrRunning;
                                break;
                            }

                            // 次のタスクを取得
                            item = this.tasks.First.Value;
                            this.tasks.RemoveFirst();
                        }

                        base.TryExecuteTask(item);
                    }
                }
                finally
                {
                    currentThreadIsProcessingItems = false;
                }
            }, null);
        }

        /// <summary>
        /// このスレッドで指定のタスクを実行できるか試みます。
        /// </summary>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (!currentThreadIsProcessingItems) return false;

            // タスクが既に登録済みであればキューから削除します。
            if (taskWasPreviouslyQueued)
            {
                return (TryDequeue(task) ? base.TryExecuteTask(task) : false);
            }
            else
            {
                return base.TryExecuteTask(task);
            }
        }

        /// <summary>
        /// キューからタスクを取り除いてみます。
        /// </summary>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (this.tasks)
            {
                return this.tasks.Remove(task);
            }
        }
        
        /// <summary>
        /// 予定されているタスクリストを取得します。
        /// </summary>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(this.tasks, ref lockTaken);
                if (lockTaken) return this.tasks;
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(this.tasks);
            }
        }
    }
}
