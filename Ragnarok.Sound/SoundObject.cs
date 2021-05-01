using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Sound
{
    using MathEx;

    /// <summary>
    /// サウンドの管理用クラスです。
    /// </summary>
    public class SoundObject : MarshalByRefObject
    {
        private readonly Backend.ISoundObjectBackend backend;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal SoundObject(Backend.ISoundObjectBackend backend)
        {
            if (backend == null)
            {
                throw new ArgumentNullException(nameof(backend));
            }

            this.backend = backend;
        }

        /// <summary>
        /// サウンド停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<SoundStopEventArgs> Stopped
        {
            add { this.backend.Stopped += value; }
            remove { this.backend.Stopped -= value; }
        }

        /// <summary>
        /// 音声ソースを取得します。
        /// </summary>
        internal Backend.ISoundObjectBackend Backend
        {
            get { return this.backend; }
        }

        /// <summary>
        /// 音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double Volume
        {
            get { return this.backend.Volume; }
            set { this.backend.Volume = MathUtil.Between(0.0f, 1.0f, value); }
        }

        /// <summary>
        /// 再生長さを取得します。
        /// </summary>
        public TimeSpan Length
        {
            get { return this.backend.Length; }
        }

        /// <summary>
        /// 再生が終わったかどうかを取得します。
        /// </summary>
        public bool IsPlaying
        {
            get { return this.backend.IsPlaying; }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
            Stop(SoundStopReason.StoppedByUser);
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        internal void Stop(SoundStopReason reason)
        {
            this.backend.Stop(reason);
        }
    }
}
