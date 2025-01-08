using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ragnarok.Utility
{
    /// <summary>
    /// Updateにより更新されたデータの反映処理を
    /// Callbackとして最小間隔以上の時間をおいて行うようにします。
    /// </summary>
    public class ScheduleUpdater<T>
    {
        private readonly object syncObject = new();
        private bool updateScheduled = false;

        public ScheduleUpdater(Action<T> callback = null,
                               TimeSpan? interval = null,
                               Func<T, T, T> merger = null)
        {
            Callback = callback;
            Interval = interval ?? Interval;
            Merger = merger;
        }

        /// <summary>
        /// Callbackを呼び出す最小間隔
        /// </summary>
        public TimeSpan Interval { get; set; }
            = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// 最新データを取得します。
        /// </summary>
        public T LatestData { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<T, T, T> Merger { get; set; }

        /// <summary>
        /// 実際のデータ更新処理を行うCallbackです。
        /// </summary>
        public event Action<T> Callback;

        /// <summary>
        /// データを最新のものに更新します。
        /// </summary>
        public void Update(T data)
        {
            lock (syncObject)
            {
                // 最新データを記録
                LatestData = Merger != null
                    ? Merger(LatestData, data)
                    : data;

                // 更新がスケジュールされていない場合、処理をスケジュール
                if (!updateScheduled)
                {
                    updateScheduled = true;
                    ScheduleUpdate();
                }
            }
        }

        /// <summary>
        /// 更新処理をスケジュールし、一定間隔ごとに1度Callbackを呼び出します。
        /// </summary>
        private void ScheduleUpdate()
        {
            Task.Run(async () =>
            {
                // 次の更新まで待機
                await Task.Delay(Interval);

                T dataToUpdate;
                lock (syncObject)
                {
                    dataToUpdate = LatestData;
                    LatestData = default;
                    updateScheduled = false;
                }

                Callback?.Invoke(dataToUpdate);
            });
        }
    }
}
