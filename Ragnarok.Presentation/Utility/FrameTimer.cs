using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// 各フレームで呼ばれるイベントの引数です。
    /// </summary>
    public sealed class FrameEventArgs : EventArgs
    {
        /// <summary>
        /// フレーム時間を取得または設定します。
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrameEventArgs(TimeSpan elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }
    }

    /// <summary>
    /// 各フレームの時間を固定します。
    /// </summary>
    public sealed class FrameTimer : NotifyObject, IDisposable
    {
        private DateTime prevTime;
        private bool disposed;

        /// <summary>
        /// 各フレームごとに呼ばれるイベントです。
        /// </summary>
        public event EventHandler<FrameEventArgs> EnterFrame;

        /// <summary>
        /// 所望するFPSを取得または設定します。
        /// </summary>
        public Dispatcher Dispatcher
        {
            get { return GetValue<Dispatcher>(nameof(Dispatcher)); }
            set { SetValue(nameof(Dispatcher), value); }
        }

        /// <summary>
        /// 所望するFPSを取得または設定します。
        /// </summary>
        public double TargetFPS
        {
            get { return GetValue<double>(nameof(TargetFPS)); }
            set { SetValue(nameof(TargetFPS), value); }
        }

        /// <summary>
        /// フレーム時間を取得します。
        /// </summary>
        [DependOn(nameof(TargetFPS))]
        public TimeSpan FrameTime
        {
            get { return TimeSpan.FromSeconds(1.0 / TargetFPS); }
        }

        /// <summary>
        /// 次のフレームのためのイベント呼び出しを準備します。
        /// </summary>
        private void PrepareToNextRender()
        {
            Dispatcher.BeginInvoke(
                new Action(() => { }),
                DispatcherPriority.SystemIdle);
        }

        /// <summary>
        /// 次フレームまで待ちます。
        /// </summary>
        private TimeSpan WaitNextFrame()
        {
            var diff = DateTime.Now - this.prevTime;
            if (diff > FrameTime)
            {
                // 時間が過ぎている。
                this.prevTime = DateTime.Now;
                return diff;
            }

            // 3ms猶予を与え、必要な待ち時間だけ待ちます。
            var waitTime = FrameTime - diff;
            var sleepTime = waitTime - TimeSpan.FromMilliseconds(3);
            if (sleepTime > TimeSpan.Zero)
            {
                Thread.Sleep(sleepTime);
            }

            var nextTime = FrameTime - TimeSpan.FromMilliseconds(1);
            while (DateTime.Now - this.prevTime < nextTime)
            {
                Thread.Sleep(0);
            }

            // ぴったりに終わったと仮定します。
            // 差分は次フレームに持ち越されます。
            this.prevTime += FrameTime;
            return FrameTime;
        }

        /// <summary>
        /// 各フレームの処理を行います。
        /// </summary>
        private void UpdateFrame(object sender, EventArgs e)
        {
            // アイドル時間を強制的に発生させます。
            using (new ActionOnDispose(PrepareToNextRender))
            {
                var diff = WaitNextFrame();

                // 各フレームの処理を行います。
                EnterFrame.SafeRaiseEvent(this, new FrameEventArgs(diff));
            }
        }

        /// <summary>
        /// タイマー処理を開始します。
        /// </summary>
        public void Start()
        {
            ComponentDispatcher.ThreadIdle += UpdateFrame;
        }

        /// <summary>
        /// タイマー処理を停止します。
        /// </summary>
        public void Stop()
        {
            ComponentDispatcher.ThreadIdle -= UpdateFrame;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrameTimer()
            : this(30, null, WPFUtil.UIDispatcher)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrameTimer(double fps, EventHandler<FrameEventArgs> handler,
                          Dispatcher dispatcher)
        {
            TargetFPS = fps;
            Dispatcher = dispatcher;

            if (handler != null)
            {
                EnterFrame += handler;
            }
        }

        /// <summary>
        /// ファイナライズ
        /// </summary>
        ~FrameTimer()
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
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Stop();
                    EnterFrame = null;
                }

                this.disposed = true;
            }
        }
    }
}
