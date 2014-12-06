using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Utility
{
    using System.Runtime.InteropServices;
    static class NativeMethods
    {
        /*[StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr hWnd;
            public uint Msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint Time;
            public System.Drawing.Point Point;
        }*/

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(out Message message, IntPtr hWnd, uint filterMin, uint filterMax, uint flags);
    }

    /// <summary>
    /// Windows.Forms上でフレーム時間を固定するために使います。
    /// </summary>
    public sealed class FrameTimer : NotifyObject, IDisposable
    {
        private double prevTick;
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
            Application.Idle += UpdateFrame;
        }

        /// <summary>
        /// タイマー処理を停止します。
        /// </summary>
        public void Stop()
        {
            Application.Idle -= UpdateFrame;
        }

        /// <summary>
        /// 次のフレームのためのイベント呼び出しを準備します。
        /// </summary>
        private void PrepareToNextRender()
        {
            // こうするとWindowsメッセージが発生し
            // 次のApplication.Idleが呼ばれるようになります。　
            FormsUtil.Synchronizer.UIProcess(() => { FormsUtil.Synchronizer.Invalidate(); });
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
                Console.WriteLine("come");
            }

            var nextTime = this.prevTick + (FrameTime - 1.0);
            while (Environment.TickCount < nextTime)
            {
                Thread.Sleep(0);
                //Console.WriteLine("come2");
            }

            // ぴったりに終わったと仮定します。
            // 差分は次フレームに持ち越されます。
            this.prevTick += FrameTime;
            return FrameTime;
        }

        /// <summary>
        /// 各フレームの処理を行います。
        /// </summary>
        private void UpdateFrame(object sender, EventArgs e)
        {
            /*// アイドル時間を強制的に発生させます。
            using (new ActionOnDispose(PrepareToNextRender))
            {
                var diff = WaitNextFrame();

                // 各フレームの処理を行います。
                EnterFrame.SafeRaiseEvent(this, new FrameEventArgs(diff));
            }*/

            Message message;

            while (!NativeMethods.PeekMessage(out message, IntPtr.Zero, 0, 0, 0))
            {
                var diff = WaitNextFrame();

                // 各フレームの処理を行います。
                EnterFrame.SafeRaiseEvent(this, new FrameEventArgs(diff));
            }
        }
    }
}
