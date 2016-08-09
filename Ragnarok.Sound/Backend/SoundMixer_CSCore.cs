#if USE_SOUND_CSCORE
using System;
using System.Collections.Generic;
using System.Linq;
using CSCore;

namespace Ragnarok.Sound.Backend
{
    internal sealed class CSCoreMixier : ISampleSource
    {
        private readonly Dictionary<SoundObjectBackend_CSCore, ISampleSource> sourceList =
            new Dictionary<SoundObjectBackend_CSCore, ISampleSource>();
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
                    foreach (var pair in sourceList)
                    {
                        var wave = pair.Key.State as IWaveSource;
                        wave.Dispose();
                    }

                    this.sourceList.Clear();
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
        /// <paramref name="backend"/>が再生リストに含まれているか調べます。
        /// </summary>
        public bool Contains(SoundObjectBackend_CSCore backend)
        {
            if (backend == null)
            {
                return false;
            }

            return this.sourceList.ContainsKey(backend);
        }

        /// <summary>
        /// <paramref name="backend"/>を再生リストに追加します。
        /// </summary>
        public void AddSource(SoundObjectBackend_CSCore backend)
        {
            if (backend == null)
            {
                throw new ArgumentNullException("backend");
            }

            /*if (source.WaveFormat.Channels != WaveFormat.Channels ||
                source.WaveFormat.SampleRate != WaveFormat.SampleRate)
            {
                throw new ArgumentException("Invalid format.", "source");
            }*/

            if (!Contains(backend))
            {
                var source = backend.State as IWaveSource;

                this.sourceList.Add(
                    backend,
                    source
                        .ChangeSampleRate(WaveFormat.SampleRate)
                        .ToStereo()
                        .ToSampleSource());
            }
        }

        /// <summary>
        /// <paramref name="backend"/>を再生リストから削除します。
        /// </summary>
        public void RemoveSource(SoundObjectBackend_CSCore backend)
        {
            if (Contains(backend))
            {
                this.sourceList.Remove(backend);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Read(float[] buffer, int offset, int count)
        {
            var numberOfStoredSamples = 0;

            if (count > 0 && this.sourceList.Any())
            {
                // 読み取りバッファを確保
                this.mixerBuffer = this.mixerBuffer.CheckBuffer(count);
                Array.Clear(this.mixerBuffer, 0, count);

                var numberOfReadSamples = new List<int>();
                foreach (var pair in this.sourceList.ToArray())
                {
                    var backend = pair.Key;
                    var source = pair.Value;

                    // バッファ読み込み時は、すでに読み込んだ部分だけ加算するようにします。
                    var read = source.Read(this.mixerBuffer, 0, count);
                    for (var n = 0; n < read; ++n)
                    {
                        var initial = (numberOfStoredSamples <= n + offset ? 0 : buffer[n + offset]);
                        var add = this.mixerBuffer[n] * backend.Volume;

                        buffer[n + offset] = (float)(initial + add); // MathEx.Between(-1.0f, +1.0f, (float)(initial + add));
                    }

                    numberOfStoredSamples = Math.Max(numberOfStoredSamples, read);

                    if (read > 0)
                    {
                        numberOfReadSamples.Add(read);
                    }
                    else if (source.Position >= source.Length)
                    {
                        // remove the input to make sure that the event gets only raised once.
                        RemoveSource(backend);
                    }
                }

                if (false)
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

            /*if (numberOfStoredSamples != count)
            {
                Array.Clear(
                    buffer,
                    Math.Max(offset + numberOfStoredSamples, 0),
                    count - numberOfStoredSamples);

                return count;
            }*/

            return numberOfStoredSamples;
        }
    }
}
#endif
