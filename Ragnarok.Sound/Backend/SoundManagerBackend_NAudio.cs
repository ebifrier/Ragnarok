#if USE_SOUND_NAUDIO
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using Ragnarok;
using Ragnarok.MathEx;

namespace Ragnarok.Sound.Backend
{
    /// <summary>
    /// NAudioによる音声ファイルの再生を行います。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal sealed class SoundManagerBackend_NAudio : ISoundManagerBackend
    {
        private readonly Dictionary<string, byte[]> sampleCache = new();
        private readonly IWavePlayer player;
        private readonly MixingSampleProvider mixer;
        private readonly WaveFormat waveFormat;
        private double volume = 1.0;

        public SoundManagerBackend_NAudio()
        {
            this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            this.mixer = new MixingSampleProvider(this.waveFormat)
            {
                ReadFully = true,
            };

            this.player = new WaveOut
            {
                DesiredLatency = 300,
                NumberOfBuffers = 2,
            };

            this.player.Init(this.mixer);
            this.player.Play();
        }

        /// <summary>
        /// 音声を再生できるかどうかを取得します。
        /// </summary>
        public bool CanUseSound => true;

        /// <summary>
        /// ボリュームを0-1の間で取得または設定します。
        /// </summary>
        public double Volume
        {
            get => this.volume;
            set
            {
                // 設定可能な音量値は0.0～1.0
                this.volume = MathUtil.Between(0.0, 1.0, value);
            }
        }

        /// <summary>
        /// サウンドソースをキャッシュから取得し、なければファイルを読み込みます。
        /// </summary>
        private ISampleProvider GetSoundSample(string filename)
        {
            WaveStream LoadWaveStream(string filepath)
            {
                var sample = new AudioFileReader(filepath);
                var wave = new WaveChannel32(sample, 1.0f, 0.5f)
                {
                    PadWithZeroes = false,
                };

                if (!wave.WaveFormat.Equals(this.waveFormat))
                {
                    throw new ArgumentException(
                        "対応していないサウンドフォーマットです。");
                }

                return wave;
            }

            var filepath = Path.GetFullPath(filename);
            if (!File.Exists(filepath))
            {
                return null;
            }

            // キャッシュの確認を行います。
            if (!this.sampleCache.TryGetValue(filepath, out byte[] data))
            {
                var wave = LoadWaveStream(filepath);
                data = Util.ReadToEnd(wave);

                this.sampleCache.Add(filepath, data);
            }

            return new RawSourceWaveStream(
                data, 0, data.Length, this.waveFormat)
                .ToSampleProvider();
        }

        /// <summary>
        /// SEを再生します。
        /// </summary>
        public ISoundObjectBackend Play(IEnumerable<string> filenames, double volume)
        {
            if (filenames == null || !filenames.Any())
            {
                return null;
            }

            var samples = filenames
                .Select(_ => GetSoundSample(_))
                .ToList();
            if (samples.Any(_ => _ == null))
            {
                throw new InvalidDataException(
                    "音声ファイルの読み込みに失敗しました。");
            }

            // 音量を設定します。
            var sound = new SoundObjectBackend_NAudio(this.mixer, samples)
            {
                Volume = Volume * volume
            };
            sound.Play();

            return sound;
        }
    }
}
#endif
