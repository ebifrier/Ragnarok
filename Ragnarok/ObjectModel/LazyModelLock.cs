using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ragnarok.ObjectModel
{
    using Utility;

    /// <summary>
    /// ILazyModelのロックを行います。
    /// </summary>
    /// <remarks>
    /// ロックするとプロパティの変更通知が送られなくなります。
    /// ロックを解除した瞬間にまとめておくられます。
    /// </remarks>
    public sealed class LazyModelLock : IDisposable
    {
        private ILazyModel self;
        private DebugLock debugLock;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LazyModelLock(ILazyModel self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            self.LazyModelObject.Enter(self);

            this.self = self;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LazyModelLock(ILazyModel self, object lockObject)
            : this(self)
        {
            if (lockObject != null)
            {
                // ここからロックを開始します。
                this.debugLock = new DebugLock(lockObject);
            }
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            if (this.debugLock != null)
            {
                this.debugLock.Dispose();
                this.debugLock = null;
            }

            if (this.self != null)
            {
                this.self.LazyModelObject.Exit(self);
                this.self = null;
            }
        }
    }
}
