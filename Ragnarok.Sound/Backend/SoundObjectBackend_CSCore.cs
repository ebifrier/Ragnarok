#if USE_SOUND_CSCORE
using System;
using System.Collections.Generic;
using System.Linq;

using CSCore;

namespace Ragnarok.Sound.Backend
{
    /// <summary>
    /// IrrKlangの音声ファイルをラップします。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal sealed class SoundObjectBackend_CSCore : ISoundObjectBackend
    {
        private IWaveSource source;
        private double volume;
        private bool isPlaying;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SoundObjectBackend_CSCore(IWaveSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
            this.volume = 1.0;
            this.isPlaying = false;
        }

        /// <summary>
        /// サウンド停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<SoundStopEventArgs> Stopped;

        /// <summary>
        /// 内部で使うオブジェクトを取得します。
        /// </summary>
        public object State
        {
            get { return this.source; }
        }

        /// <summary>
        /// 音量を取得します。
        /// </summary>
        public double Volume
        {
            get { return this.volume; }
            set { this.volume = value; }
        }

        /// <summary>
        /// 再生長さを取得します。
        /// </summary>
        public TimeSpan Length
        {
            get { return this.source.GetLength(); }
        }

        /// <summary>
        /// 再生中かどうかを取得します。
        /// </summary>
        public bool IsPlaying
        {
            get { return this.isPlaying; }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop(SoundStopReason reason)
        {
            this.source.Position = 0;
            this.isPlaying = false;

            Stopped.RaiseEvent(this, new SoundStopEventArgs(reason));
        }
    }
}
#endif
