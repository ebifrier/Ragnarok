using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ragnarok.Utility
{
    /// <summary>
    /// イベントからハンドラの除去するために使います。
    /// </summary>
    public delegate void UnregisterCallback<TEventArgs>(EventHandler<TEventArgs> eventHandler)
        where TEventArgs : EventArgs;

    /// <summary>
    /// 弱い参照をもつイベントハンドラの基本インターフェース。
    /// </summary>
    public interface IWeakEventHandler<TEventArgs>
        where TEventArgs : EventArgs
    {
        EventHandler<TEventArgs> Handler { get; }
    }

    /// <summary>
    /// 弱い参照をもつイベントハンドラ。
    /// </summary>
    public class WeakEventHandler<TTarget, TEventArgs> : IWeakEventHandler<TEventArgs>
        where TTarget : class
        where TEventArgs : EventArgs
    {
        private delegate void OpenEventHandler(TTarget target, object sender, TEventArgs e);

        private WeakReference targetRef;
        private OpenEventHandler openHandler;
        private EventHandler<TEventArgs> handler;
        private UnregisterCallback<TEventArgs> unregister;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WeakEventHandler(EventHandler<TEventArgs> eventHandler,
                                UnregisterCallback<TEventArgs> unregister)
        {
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
        public EventHandler<TEventArgs> Handler
        {
            get { return this.handler; }
        }

        /// <summary>
        /// 呼び出し可能なハンドラに変換します。
        /// </summary>
        public static implicit operator EventHandler<TEventArgs>(
            WeakEventHandler<TTarget, TEventArgs> weh)
        {
            return weh.handler;
        }

        /// <summary>
        /// イベントハンドラを呼び出します。
        /// </summary>
        public void Invoke(object sender, TEventArgs e)
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
