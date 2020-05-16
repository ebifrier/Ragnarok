using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ragnarok.Utility
{
    /// <summary>
    /// Dispose時に与えられた手続きを実行します。
    /// </summary>
    public class ActionOnDispose : IDisposable
    {
        private Action action;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ActionOnDispose(Action action)
        {
            this.action = action;
        }

        ~ActionOnDispose()
        {
            Dispose(false);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Interlocked.Exchange(ref this.action, null)?.Invoke();
            }
        }
    }
}
