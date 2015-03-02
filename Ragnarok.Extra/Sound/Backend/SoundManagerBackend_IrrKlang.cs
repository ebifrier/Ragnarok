#if !MONO
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using IrrKlang;

namespace Ragnarok.Extra.Sound.Backend
{
    /// <summary>
    /// IrrKlangの音声ファイルを再生します。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal sealed class SoundManagerBackend_IrrKlang : ISoundManagerBackend
    {
        private readonly ISoundEngine engine;

        /// <summary>
        /// 音声を再生できるかどうかを取得します。
        /// </summary>
        public bool CanUseSound
        {
            get
            {
                return (engine != null);
            }
        }

        /// <summary>
        /// ボリュームを0-1の間で取得または設定します。
        /// </summary>
        public double Volume
        {
            get
            {
                if (engine == null)
                {
                    return 0;
                }

                return engine.SoundVolume;
            }
            set
            {
                if (engine == null)
                {
                    return;
                }

                // 設定可能な音量値は0.0～1.0
                engine.SoundVolume = MathEx.Between(0.0f, 1.0f, (float)value);
            }
        }

        /// <summary>
        /// ファイルパスを英数字のIDに変換します。
        /// </summary>
        /// <remarks>
        /// IrrKlangでは日本語のファイル名が使えないため、
        /// base64変換したファイル名をファイルIDとして利用しています。
        /// </remarks>
        private static string GetFileId(string filepath)
        {
            var byteArray = Encoding.UTF8.GetBytes(filepath);

            // base64を使います。
            return Convert.ToBase64String(byteArray);
        }

        /// <summary>
        /// 音声ファイルを読み込みます。
        /// </summary>
        private ISoundSource LoadSoundSource(string filepath)
        {
            // irrKlangは日本語ファイルが読めないので、
            // ストリームから再生することにする。
            var stream = new FileStream(filepath, FileMode.Open);

            // 日本語のファイル名をbase64で英数字の羅列に変換します。
            var fileid = GetFileId(filepath);

            return engine.AddSoundSourceFromIOStream(stream, fileid);
        }

        /// <summary>
        /// サウンドソースをキャッシュから取得し、なければファイルを読み込みます。
        /// </summary>
        private ISoundSource GetSoundSource(string filename)
        {
            // 音声のフルパスを取得します。
            var filepath = Path.GetFullPath(filename);
            if (!File.Exists(filepath))
            {
                return null;
            }

            // irrKlangは日本語のファイル名が読めないので、
            // ストリームから再生する。
            var fileid = GetFileId(filepath);
            var source = engine.GetSoundSource(fileid, false);
            if (source == null)
            {
                source = LoadSoundSource(filepath);
                if (source == null)
                {
                    return null;
                }
            }

            return source;
        }

        /// <summary>
        /// SEを再生します。
        /// </summary>
        public ISoundObjectBackend Play(string filename, double volume)
        {
            if (engine == null)
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

            // 再生
            var sound = engine.Play2D(source, false, true, false);
            if (sound == null)
            {
                throw new InvalidOperationException(
                    "音声ファイルの再生に失敗しました。");
            }

            // 音量を設定します。
            sound.Volume = MathEx.Between(0.0f, 1.0f, (float)(engine.SoundVolume * volume));
            sound.Paused = false;

            return new SoundObjectBackend_IrrKlang(sound);
        }

        /// <summary>
        /// 音声プレイヤーオブジェクトを初期化します。
        /// </summary>
        public SoundManagerBackend_IrrKlang()
        {
            engine = new ISoundEngine();
        }
    }
}
#endif
