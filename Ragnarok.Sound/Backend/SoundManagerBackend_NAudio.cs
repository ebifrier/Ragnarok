#if USE_SOUND_NAUDIO
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

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

            this.player = new DirectSoundOut(200);
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
        /// 読み込んだ音声を再生用フォーマット(this.waveFormat)へ変換します。
        /// </summary>
        /// <remarks>
        /// サンプルレートやチャンネル数が異なるファイルでも、
        /// リサンプリングとチャンネル変換を行うことで再生できるようにします。
        /// </remarks>
        private ISampleProvider ConvertToWaveFormat(ISampleProvider sample)
        {
            // チャンネル数を合わせます。
            if (sample.WaveFormat.Channels != this.waveFormat.Channels)
            {
                if (sample.WaveFormat.Channels == 1 &&
                    this.waveFormat.Channels == 2)
                {
                    sample = new MonoToStereoSampleProvider(sample);
                }
                else if (sample.WaveFormat.Channels == 2 &&
                         this.waveFormat.Channels == 1)
                {
                    sample = new StereoToMonoSampleProvider(sample);
                }
                else
                {
                    throw new ArgumentException(
                        "対応していないチャンネル数です。");
                }
            }

            // サンプルレートを合わせます。
            if (sample.WaveFormat.SampleRate != this.waveFormat.SampleRate)
            {
                sample = new WdlResamplingSampleProvider(
                    sample, this.waveFormat.SampleRate);
            }

            return sample;
        }

        /// <summary>
        /// IWaveProviderの内容をすべてバイト列として読み出します。
        /// </summary>
        private static byte[] ReadToEnd(IWaveProvider provider)
        {
            using var result = new MemoryStream();
            var buffer = new byte[provider.WaveFormat.AverageBytesPerSecond];

            int size;
            while ((size = provider.Read(buffer, 0, buffer.Length)) > 0)
            {
                result.Write(buffer, 0, size);
            }

            return result.ToArray();
        }

        /// <summary>
        /// サウンドソースをキャッシュから取得し、なければファイルを読み込みます。
        /// </summary>
        private ISampleProvider GetSoundSample(string filename)
        {
            var filepath = Path.GetFullPath(filename);
            if (!File.Exists(filepath))
            {
                return null;
            }

            // キャッシュの確認を行います。
            if (!this.sampleCache.TryGetValue(filepath, out byte[] data))
            {
                // AudioFileReaderは32bit floatのISampleProviderを返すため、
                // フォーマットを合わせたうえで生バイト列に変換して保持します。
                using var reader = new AudioFileReader(filepath);
                var sample = ConvertToWaveFormat(reader);
                data = ReadToEnd(sample.ToWaveProvider());

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
