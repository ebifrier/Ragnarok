using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ragnarok.Utility
{
    /// <summary>
    /// イベントからハンドラの除去するために使います。
    /// </summary>
    public delegate void UnregisterCallback(EventHandler eventHandler);

    /// <summary>
    /// 弱い参照をもつイベントハンドラの基本インターフェース。
    /// </summary>
    public interface IWeakEventHandler
    {
        EventHandler Handler { get; }
    }

    /// <summary>
    /// 弱い参照をもつイベントハンドラ。
    /// </summary>
    public class WeakEventHandler<TTarget> : IWeakEventHandler
        where TTarget : class
    {
        private delegate void OpenEventHandler(TTarget target, object sender, EventArgs e);

        private WeakReference targetRef;
        private OpenEventHandler openHandler;
        private EventHandler handler;
        private UnregisterCallback unregister;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WeakEventHandler(EventHandler eventHandler,
                                UnregisterCallback unregister)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException(nameof(eventHandler));
            }

            this.targetRef = new WeakReference(eventHandler.Target);
            this.openHandler = (OpenEventHandler)Delegate.CreateDelegate(
                typeof(OpenEventHandler),
                null, eventHandler.Method);
            this.handler = Invoke;
            this.unregister = unregister;
        }

        /// <summary>
        /// 呼び出し可能なハンドラを取得します。
        /// </summary>
        public EventHandler Handler
        {
            get { return this.handler; }
        }

        /// <summary>
        /// 呼び出し可能なハンドラに変換します。
        /// </summary>
        public EventHandler ToEventHandler(
            WeakEventHandler<TTarget> weh)
        {
            return weh?.handler;
        }

        /// <summary>
        /// イベントハンドラを呼び出します。
        /// </summary>
        public void Invoke(object sender, EventArgs e)
        {
            var target = (TTarget)this.targetRef.Target;

            if (target != null)
            {
                this.openHandler(target, sender, e);
            }
            else
            {
                Interlocked.Exchange(ref this.unregister, null)
                    ?.Invoke(this.handler);
            }
        }
    }
}
