using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Ragnarok.ObjectModel;

namespace Ragnarok.Forms.Utility
{
    /// <summary>
    /// Windows.Forms上でフレーム時間を固定するために使います。
    /// </summary>
    public sealed class FrameTimer : NotifyObject, IDisposable
    {
        private double prevTick;
        private Thread thread;
        private volatile bool alive;
        private bool disposed;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrameTimer(double fps = 60)
        {
            TargetFPS = fps;
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

        /// <summary>
        /// 各フレームごとに呼ばれるイベントです。
        /// </summary>
        public event EventHandler<FrameEventArgs> EnterFrame;

        /// <summary>
        /// 所望するFPSを取得または設定します。
        /// </summary>
        public double TargetFPS
        {
            get { return GetValue<double>("TargetFPS"); }
            set { SetValue("TargetFPS", value); }
        }

        /// <summary>
        /// フレーム時間を取得します。
        /// </summary>
        [DependOnProperty("TargetFPS")]
        public double FrameTime
        {
            get { return (1000.0 / TargetFPS); }
        }

        /// <summary>
        /// タイマー処理を開始します。
        /// </summary>
        public void Start()
        {
            if (this.alive)
            {
                throw new InvalidOperationException(
                    "タイマーは既に起動しています。");
            }
            
            // FPS管理用のスレッドを起動します。
            this.thread = new Thread(UpdateFrame)
            {
                Name = "FrameTimer",
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true,
            };
            this.thread.Start();
        }

        /// <summary>
        /// タイマー処理を停止します。
        /// </summary>
        public void Stop()
        {
            if (!this.alive)
            {
                return;
            }

            this.alive = false;

            // 手動でイベント処理しないと、
            // スレッドが終わらないことがあります。
            Application.DoEvents();

            if (this.thread != null)
            {
                // スレッドは停止しなくても良しとします。
                this.thread.Join(500);
                this.thread = null;
            }
        }

        /// <summary>
        /// 各フレームの処理を行います。
        /// </summary>
        private void UpdateFrame(object state)
        {
            // フレームタイマーの起動フラグをオンにします。
            this.alive = true;

            // this.aliveは他の場所で更新されます。
            while (this.alive)
            {
                var diff = WaitNextFrame();

                // 各フレームの処理を行います。
                // (念のため同期してメソッドを呼び出します)
                if (this.alive)
                {
                    FormsUtil.Synchronizer.Invoke(
                        new Action(() => DoEnterFrame(diff)));
                }
            }
        }

        /// <summary>
        /// 次フレームまで待ちます。
        /// </summary>
        private double WaitNextFrame()
        {
            var diff = Environment.TickCount - this.prevTick;
            if (diff > FrameTime)
            {
                // 時間が過ぎている。
                this.prevTick = Environment.TickCount;
                return diff;
            }

            // 3ms猶予を与え、必要な待ち時間だけ待ちます。
            var waitTime = FrameTime - diff;
            var sleepTime = waitTime - 3.0;
            if (sleepTime > 0.0)
            {
                Thread.Sleep((int)sleepTime);
            }

            var nextTime = this.prevTick + (FrameTime - 1.0);
            if (Environment.TickCount < nextTime)
            {
                Thread.Sleep(0);
            }

            // ぴったりに終わったと仮定します。
            // 差分は次フレームに持ち越されます。
            this.prevTick += FrameTime;
            return FrameTime;
        }

        /// <summary>
        /// 各フレームに必要な処理を行います。
        /// </summary>
        private void DoEnterFrame(double diff)
        {
            EnterFrame.SafeRaiseEvent(this, new FrameEventArgs(diff));
        }
    }
}
