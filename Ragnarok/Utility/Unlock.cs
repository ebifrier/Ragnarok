using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ragnarok.Utility
{
    /// <summary>
    /// オブジェクトを一時的にロック解除するためのクラスです。
    /// </summary>
    /// <remarks>
    /// 非同期的にコールバックが呼ばれるなど、
    /// 一時的にロックを解除したいときに使います。
    /// </remarks>
    public sealed class Unlock : IDisposable
    {
        private object monitor;

        /// <summary>
        /// <paramref name="monitor"/>のロックを解除します。
        /// </summary>
        public Unlock(object monitor)
        {
            this.monitor = monitor;

            Monitor.Exit(this.monitor);
        }

        /// <summary>
        /// オブジェクトをロックします。
        /// </summary>
        public void Dispose()
        {
            var target = Interlocked.Exchange(ref this.monitor, null);
            if (target != null)
            {
                GC.SuppressFinalize(this);

                Monitor.Enter(target);
            }
        }
    }
}
