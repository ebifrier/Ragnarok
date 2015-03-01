using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using IrrKlang;

namespace Ragnarok.Extra.Sound.Backend
{
    /// <summary>
    /// IrrKlangの音声ファイルをラップします。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal sealed class SoundObjectBackend_IrrKlang :
        ISoundObjectBackend, ISoundStopEventReceiver
    {
        private ISound sound;

        /// <summary>
        /// サウンド停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<SoundStopEventArgs> Stopped;

        /// <summary>
        /// 音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double Volume
        {
            get { return this.sound.Volume; }
            set { this.sound.Volume = (float)value; }
        }

        /// <summary>
        /// 再生長さを取得します。
        /// </summary>
        public int Length
        {
            get { return (int)this.sound.PlayLength; }
        }

        /// <summary>
        /// 再生が終わったかどうかを取得します。
        /// </summary>
        public bool IsFinished
        {
            get { return this.sound.Finished; }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
            this.sound.Stop();
        }

        /// <summary>
        /// 再生が停止した時に呼ばれます。
        /// </summary>
        void ISoundStopEventReceiver.OnSoundStopped(ISound sound,
                                                    StopEventCause cause,
                                                    object userData)
        {
            var handler = Interlocked.Exchange(ref Stopped, null);

            if (handler != null)
            {
                var reason = ConvertReason(cause);

                handler(null, new SoundStopEventArgs(reason));
            }
        }

        private SoundStopReason ConvertReason(StopEventCause cause)
        {
            switch (cause)
            {
                case StopEventCause.SoundFinishedPlaying:
                    return SoundStopReason.FinishedPlaying;
                case StopEventCause.SoundStoppedByUser:
                    return SoundStopReason.StoppedByUser;
                case StopEventCause.SoundStoppedBySourceRemoval:
                    return SoundStopReason.StoppedBySourceRemoval;
            }

            throw new ArgumentException("cause");
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SoundObjectBackend_IrrKlang(ISound sound)
        {
            if (sound == null)
            {
                throw new ArgumentNullException("sound");
            }

            sound.setSoundStopEventReceiver(this);
            this.sound = sound;
        }
    }
}
