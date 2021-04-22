#if USE_SOUND_NAUDIO
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;

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
        private Dictionary<string, ISampleProvider> sampleCache = new();
        private double volume = 1.0;

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
            var filepath = Path.GetFullPath(filename);
            if (!File.Exists(filepath))
            {
                return null;
            }

            if (sampleCache.TryGetValue(filepath, out ISampleProvider sample))
            {
                return sample;
            }

            return new AudioFileReader(filepath);
        }

        /// <summary>
        /// SEを再生します。
        /// </summary>
        public ISoundObjectBackend Play(string filename, double volume)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            var sample = GetSoundSample(filename);
            if (sample == null)
            {
                throw new InvalidDataException(
                    "音声ファイルの読み込みに失敗しました。");
            }

            // 再生
            var sound = new SoundObjectBackend_NAudio(sample);
            if (sound == null)
            {
                throw new InvalidOperationException(
                    "音声ファイルの作成に失敗しました。");
            }

            // 音量を設定します。
            sound.Volume = MathUtil.Between(
                0.0f, 1.0f, (float)(Volume * volume));
            sound.Play();

            return sound;
        }
    }
}
#endif
