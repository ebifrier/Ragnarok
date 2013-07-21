using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

#if MONO
using Mono.Unix;
using Mono.Unix.Native;
#endif

namespace Ragnarok.Utility
{
    /// <summary>
    /// Unixシステムのシグナル処理に使うイベント引数です。
    /// </summary>
    public sealed class SignalEventArgs : EventArgs
    {
        /// <summary>
        /// 受信したシグナル番号を取得します。
        /// </summary>
        public int Signum
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SignalEventArgs(int signum)
        {
            Signum = signum;
        }
    }

    /// <summary>
    /// Unixシステムのシグナル処理を行います。
    /// </summary>
    public static class Signal
    {
        /// <summary>
        /// シグナル受信時に呼ばれるイベントハンドラです。
        /// </summary>
        public static event EventHandler<SignalEventArgs> SignalReceived;

        /// <summary>
        /// 登録されたシグナル処理ハンドラを呼び出します。
        /// </summary>
        private static void OnSignal(int signum)
        {
            var handler = SignalReceived;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(null, new SignalEventArgs(signum)));
            }
        }

#if MONO
        private static Thread signalThread;

        /// <summary>
        /// 別スレッドでシグナル処理を開始します。
        /// </summary>
        static Signal()
        {
            signalThread = new Thread(SignalThread)
            {
                Name = "SignalThread",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
            };

            signalThread.Start();
        }

        /// <summary>
        /// 別スレッドでシグナル処理を行います。
        /// </summary>
        static void SignalThread()
        {
            var signals = new UnixSignal[]
            {
                new UnixSignal(Signum.SIGUSR2),
            };

            while (true)
            {
                ProcessSignal(signals);
            }
        }

        /// <summary>
        /// シグナル処理を行います。
        /// </summary>
        static void ProcessSignal(UnixSignal[] signals)
        {
            try
            {
                var index = UnixSignal.WaitAny(signals, -1);
                if (index < 0)
                {
                    return;
                }

                var signal = signals[index].Signum;
                Log.Info("シグナルを受信しました。({0})", signal);

                OnSignal((int)signal);
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "シグナル処理に失敗しました。");
            }
        }
#endif
    }
}
