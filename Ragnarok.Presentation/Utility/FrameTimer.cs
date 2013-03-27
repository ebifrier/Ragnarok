using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

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
    public sealed class FrameTimer : IDisposable
    {
        private Dispatcher dispatcher;
        private DateTime prevTime;
        private bool disposed;

        /// <summary>
        /// 各フレームごとに呼ばれるイベントです。
        /// </summary>
        public event EventHandler<FrameEventArgs> EnterFrame;

        /// <summary>
        /// 所望するFPSを取得または設定します。
        /// </summary>
        public double TargetFPS
        {
            get;
            set;
        }

        /// <summary>
        /// フレーム時間を取得または設定します。
        /// </summary>
        public TimeSpan FrameTime
        {
            get { return TimeSpan.FromSeconds(1.0 / TargetFPS); }
        }

        /// <summary>
        /// 次のフレームのためのイベント呼び出しを準備します。
        /// </summary>
        private void PrepareToNextRender()
        {
            this.dispatcher.BeginInvoke(
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
            // フレーム時間が長すぎると、バグるエフェクトがあるため、
            // 時間を適度な短さに調整しています。
            var MaxFrameTime = TimeSpan.FromMilliseconds(1000.0 / 20);

            // アイドル時間を強制的に発生させます。
            using (new ActionOnDispose(PrepareToNextRender))
            {
                var diff = WaitNextFrame();
                diff = MathEx.Min(diff, MaxFrameTime);

                // 各フレームの処理を行います。
                EnterFrame.SafeRaiseEvent(this, new FrameEventArgs(diff));
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrameTimer()
            : this(WPFUtil.UIDispatcher)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrameTimer(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            TargetFPS = 60;

            ComponentDispatcher.ThreadIdle += UpdateFrame;
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
                    EnterFrame = null;
                    ComponentDispatcher.ThreadIdle -= UpdateFrame;
                }

                this.disposed = true;
            }
        }
    }
}
