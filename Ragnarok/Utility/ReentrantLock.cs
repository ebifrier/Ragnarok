using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 同一スレッドから同じコードブロックへの再入を防ぐために使います。
    /// </summary>
    /// <example>
    /// var locker = new ReentrancyLock();
    /// ...
    /// 
    /// using (var result = locker.Lock())
    /// {
    ///     // 再入できない場合はnullを返します。
    ///     if (result == null) return;
    /// 
    ///     ...
    /// }
    /// </example>
    public sealed class ReentrancyLock
    {
        private int entering;

        /// <summary>
        /// 再入可能か調べます。
        /// </summary>
        public bool CanReentrancy
        {
            get { return (this.entering == 0); }
        }

        /// <summary>
        /// 再入を禁止するためのロックを取得します。
        /// </summary>
        public ReentrancyResult Lock()
        {
            var flag = Interlocked.Exchange(ref this.entering, 1);

            return (flag == 0 ? new ReentrancyResult(this) : null);
        }

        /// <summary>
        /// ロックを解除します。
        /// </summary>
        public void Unlock()
        {
            Interlocked.Exchange(ref this.entering, 0);
        }
    }

    /// <summary>
    /// <see cref="ReentrancyLock"/>のロック状態を示す一時オブジェクトです。
    /// </summary>
    public sealed class ReentrancyResult : IDisposable
    {
        private ReentrancyLock locker;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal ReentrancyResult(ReentrancyLock locker)
        {
            this.locker = locker;
        }

        /// <summary>
        /// コードブロックが終了したときに呼ばれます。
        /// </summary>
        public void Dispose()
        {
            if (this.locker != null)
            {
                this.locker.Unlock();
                this.locker = null;
            }
        }
    }
}
