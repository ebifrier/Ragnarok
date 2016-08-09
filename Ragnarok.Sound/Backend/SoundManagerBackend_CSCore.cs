#if USE_SOUND_CSCORE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;

namespace Ragnarok.Sound.Backend
{
    /// <summary>
    /// IrrKlangの音声ファイルを再生します。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal sealed class SoundManagerBackend_CSCore : ISoundManagerBackend
    {
        private readonly ISoundOut soundOut;
        private readonly CSCoreMixier mixer;

        /// <summary>
        /// 音声プレイヤーオブジェクトを初期化します。
        /// </summary>
        public SoundManagerBackend_CSCore()
        {
            this.mixer = new CSCoreMixier();

            this.soundOut = new WasapiOut
            {
                Latency = 100, // better use a quite high latency
            };
            this.soundOut.Initialize(this.mixer.ToWaveSource());
            this.soundOut.Play();
        }

        /// <summary>
        /// 音声を再生できるかどうかを取得します。
        /// </summary>
        public bool CanUseSound
        {
            get
            {
                return (this.soundOut != null);
            }
        }

        /// <summary>
        /// ボリュームを0-1の間で取得または設定します。
        /// </summary>
        public double Volume
        {
            get
            {
                if (this.soundOut == null)
                {
                    return 0;
                }

                return this.soundOut.Volume;
            }
            set
            {
                if (this.soundOut == null)
                {
                    return;
                }

                // 設定可能な音量値は0.0～1.0
                this.soundOut.Volume = MathEx.Between(0.0f, 1.0f, (float)value);
            }
        }

        /// <summary>
        /// 音声ファイルを読み込みます。
        /// </summary>
        private IWaveSource LoadWaveSource(string filepath)
        {
            try
            {
                return CodecFactory.Instance.GetCodec(filepath);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// サウンドソースをキャッシュから取得し、なければファイルを読み込みます。
        /// </summary>
        private IWaveSource GetSoundSource(string filename)
        {
            // 音声のフルパスを取得します。
            var filepath = Path.GetFullPath(filename);
            if (!File.Exists(filepath))
            {
                return null;
            }

            // irrKlangは日本語のファイル名が読めないので、
            // ストリームから再生する。
            /*var fileid = GetFileId(filepath);
            var source = engine.GetSoundSource(fileid, false);
            if (source == null)
            {
                source = LoadSoundSource(filepath);
                if (source == null)
                {
                    return null;
                }
            }*/

            return LoadWaveSource(filepath);
        }

        /// <summary>
        /// SEを再生します。
        /// </summary>
        public ISoundObjectBackend Play(string filename, double volume)
        {
            if (this.soundOut == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            var source = GetSoundSource(filename);
            if (source == null)
            {
                throw new InvalidDataException(
                    "音声ファイルの読み込みに失敗しました。");
            }

            // 音量を設定します。
            var backend = new SoundObjectBackend_CSCore(source)
            {
                Volume = volume
            };

            // 再生
            this.mixer.AddSource(backend);
            if (this.soundOut.PlaybackState != PlaybackState.Playing)
            {
                this.soundOut.Play();
            }

            return backend;
        }
    }
}
#endif
