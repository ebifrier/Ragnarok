using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Sound
{
    /// <summary>
    /// サウンドの管理用クラスです。
    /// </summary>
    public class SoundObject : MarshalByRefObject
    {
        private Backend.SoundObjectBackend bsound;

        /// <summary>
        /// サウンド停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<SoundStopEventArgs> Stopped
        {
            add { this.bsound.Stopped += value; }
            remove { this.bsound.Stopped -= value; }
        }

        /// <summary>
        /// 音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double Volume
        {
            get { return this.bsound.Volume; }
            set { this.bsound.Volume = MathEx.Between(0.0, 1.0, value); }
        }

        /// <summary>
        /// 再生長さを取得します。
        /// </summary>
        public int Length
        {
            get { return this.bsound.Length; }
        }

        /// <summary>
        /// 再生が終わったかどうかを取得します。
        /// </summary>
        public bool IsFinished
        {
            get { return this.bsound.IsFinished; }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
            this.bsound.Stop();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal SoundObject(Backend.SoundObjectBackend bsound)
        {
            if (bsound == null)
            {
                throw new ArgumentNullException("bsound");
            }

            this.bsound = bsound;
        }
    }
}
