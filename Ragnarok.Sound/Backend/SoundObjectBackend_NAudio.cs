#if USE_SOUND_NAUDIO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NAudio.Wave;

namespace Ragnarok.Sound.Backend
{
    /// <summary>
    /// IrrKlangの音声ファイルをラップします。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal sealed class SoundObjectBackend_NAudio : ISoundObjectBackend
    {
        private IWavePlayer player;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SoundObjectBackend_NAudio(ISampleProvider sample)
        {
            if (sample == null)
            {
                throw new ArgumentNullException(nameof(sample));
            }

            this.player = new WaveOut();
            this.player.Init(sample);
            this.player.PlaybackStopped += OnSoundStopped;
        }

        /// <summary>
        /// サウンド停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<SoundStopEventArgs> Stopped;

        /// <summary>
        /// 内部オブジェクトを取得します。
        /// </summary>
        public object State
        {
            get { return this.player; }
        }

        /// <summary>
        /// 音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double Volume
        {
            get { return this.player.Volume; }
            set { this.player.Volume = (float)value; }
        }

        /// <summary>
        /// 再生長さを取得します。
        /// </summary>
        public TimeSpan Length
        {
            get { return TimeSpan.FromMilliseconds(0); }
        }

        /// <summary>
        /// 再生中かどうかを取得します。
        /// </summary>
        public bool IsPlaying
        {
            get { return (this.player.PlaybackState == PlaybackState.Playing); }
        }

        /// <summary>
        /// 再生します。
        /// </summary>
        public void Play()
        {
            this.player.Play();
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop(SoundStopReason reason)
        {
            this.player.Stop();
        }

        /// <summary>
        /// 再生が停止した時に呼ばれます。
        /// </summary>
        private void OnSoundStopped(object sender, StoppedEventArgs e)
        {
            var handler = Interlocked.Exchange(ref Stopped, null);

            if (handler != null)
            {
                var reason = SoundStopReason.FinishedPlaying;

                handler(null, new SoundStopEventArgs(reason));
            }
        }
    }
}
#endif
