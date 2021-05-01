#if USE_SOUND_NAUDIO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Ragnarok.Sound.Backend
{
    using Utility;

    /// <summary>
    /// IrrKlangの音声ファイルをラップします。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal sealed class SoundObjectBackend_NAudio : ISoundObjectBackend
    {
        private readonly MixingSampleProvider mixer;
        private readonly ISampleProvider sample;
        private ISampleProvider playingSample;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SoundObjectBackend_NAudio(MixingSampleProvider mixer,
                                         IEnumerable<ISampleProvider> samples)
        {
            this.mixer = mixer ?? throw new ArgumentNullException(nameof(mixer));
            this.sample = samples.Count() == 1
                ? samples.FirstOrDefault()
                : new ConcatenatingSampleProvider(samples);
        }

        private void Mixer_MixerInputEnded(object sender, SampleProviderEventArgs e)
        {
            if (this.playingSample != null &&
                this.playingSample == e.SampleProvider)
            {
                OnSoundStopped(sender, new StoppedEventArgs());
            }
        }

        /// <summary>
        /// サウンド停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<SoundStopEventArgs> Stopped;

        /// <summary>
        /// 内部オブジェクトを取得します。
        /// </summary>
        public object State => this.mixer;

        /// <summary>
        /// 音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double Volume
        {
            get;
            set;
        }

        /// <summary>
        /// 再生長さを取得します。
        /// </summary>
        public TimeSpan Length => throw new NotImplementedException();

        /// <summary>
        /// 再生中かどうかを取得します。
        /// </summary>
        public bool IsPlaying => this.playingSample != null;

        /// <summary>
        /// 再生します。
        /// </summary>
        public void Play()
        {
            if (this.playingSample != null)
            {
                return;
            }

            var playingSample = new VolumeSampleProvider(sample)
            {
                Volume = (float)Volume
            };

            this.mixer.AddMixerInput(playingSample);
            this.mixer.MixerInputEnded += Mixer_MixerInputEnded;
            this.playingSample = playingSample;
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop(SoundStopReason reason)
        {
            if (this.playingSample == null)
            {
                return;
            }

            this.mixer.MixerInputEnded -= Mixer_MixerInputEnded;
            this.mixer.RemoveMixerInput(this.playingSample);
            this.playingSample = null;
        }

        /// <summary>
        /// 再生が停止した時に呼ばれます。
        /// </summary>
        private void OnSoundStopped(object sender, StoppedEventArgs e)
        {
            var handler = Interlocked.Exchange(ref Stopped, null);

            this.mixer.MixerInputEnded -= Mixer_MixerInputEnded;
            this.playingSample = null;

            if (handler != null)
            {
                var reason = SoundStopReason.FinishedPlaying;

                handler(null, new SoundStopEventArgs(reason));
            }
        }
    }
}
#endif
