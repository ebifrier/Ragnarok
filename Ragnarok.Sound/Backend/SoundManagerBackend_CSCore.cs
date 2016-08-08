#if true
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;

namespace Ragnarok.Sound.Backend
{
    internal sealed class CSCoreMixier : ISampleSource
    {
        private readonly List<ISampleSource> sourceList = new List<ISampleSource>();
        private float[] mixerBuffer;
        private bool disposed = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CSCoreMixier()
        {
            WaveFormat = new WaveFormat(44100, 32, 2, AudioEncoding.IeeeFloat);
        }

        /// <summary>
        /// ファイナライザ
        /// </summary>
        ~CSCoreMixier()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// サウンドフォーマットを取得します。
        /// </summary>
        public WaveFormat WaveFormat
        {
            get;
            private set;
        }

        bool IAudioSource.CanSeek
        {
            get { return false; }
        }

        long IAudioSource.Position
        {
            get { return 0; }
            set { throw new NotSupportedException(); }
        }

        long IAudioSource.Length
        {
            get { return 0; }
        }

        /// <summary>
        /// <paramref name="source"/>が再生リストに含まれているか調べます。
        /// </summary>
        public bool Contains(ISampleSource source)
        {
            if (source == null)
            {
                return false;
            }

            return this.sourceList.Contains(source);
        }

        /// <summary>
        /// <paramref name="source"/>を再生リストに追加します。
        /// </summary>
        public void AddSource(ISampleSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.WaveFormat.Channels != WaveFormat.Channels ||
                source.WaveFormat.SampleRate != WaveFormat.SampleRate)
            {
                throw new ArgumentException("Invalid format.", "source");
            }

            if (!Contains(source))
            {
                this.sourceList.Add(source);
            }
        }

        /// <summary>
        /// <paramref name="source"/>を再生リストから削除します。
        /// </summary>
        public void RemoveSource(ISampleSource source)
        {
            if (Contains(source))
            {
                this.sourceList.Remove(source);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Read(float[] buffer, int offset, int count)
        {
            var numberOfStoredSamples = 0;

            if (count > 0 && this.sourceList.Count > 0)
            {
                // 読み取りバッファを確保
                this.mixerBuffer = this.mixerBuffer.CheckBuffer(count);

                var numberOfReadSamples = new List<int>();
                for (int m = this.sourceList.Count - 1; m >= 0; --m)
                {
                    // バッファ読み込み時は、すでに読み込んだ部分だけ加算するようにします。
                    var source = this.sourceList[m];
                    int read = source.Read(this.mixerBuffer, 0, count);
                    for (var n = 0; n < read; ++n)
                    {
                        var initial = (numberOfStoredSamples <= n + offset ? 0 : buffer[n + offset]);
                        var add = this.mixerBuffer[n] * 0.5f;

                        buffer[n + offset] = initial + add;
                    }

                    numberOfStoredSamples = Math.Max(numberOfStoredSamples, read);

                    if (read > 0)
                    {
                        numberOfReadSamples.Add(read);
                    }
                    else
                    {
                        RemoveSource(source); //remove the input to make sure that the event gets only raised once.
                    }
                }

                if (false) //DivideResult)
                {
                    var currentOffset = offset;

                    numberOfReadSamples.Sort();
                    numberOfReadSamples.ForEachWithIndex((readSamples, i) =>
                    {
                        while (currentOffset < offset + readSamples)
                        {
                            buffer[currentOffset] /= numberOfReadSamples.Count - i;
                            buffer[currentOffset] = MathEx.Between(-1.0f, +1.0f, buffer[currentOffset]);
                            ++currentOffset;
                        }
                    });
                }
            }

            if (numberOfStoredSamples != count)
            {
                Array.Clear(
                    buffer,
                    Math.Max(offset + numberOfStoredSamples - 1, 0),
                    count - numberOfStoredSamples);

                return count;
            }

            return numberOfStoredSamples;
        }
    }

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
            var soundOut = new WasapiOut
            {
                Latency = 200, // better use a quite high latency
            };

            this.mixer = new CSCoreMixier();

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
            var backend = new SoundObjectBackend_CSCore(source);

            // 再生
            this.mixer.AddSource(source.ToSampleSource());

            return backend;
        }
    }
}
#endif
