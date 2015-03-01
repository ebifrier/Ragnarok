using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Sound.Backend
{
    /// <summary>
    /// IrrKlangの音声ファイルをラップします。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal interface ISoundObjectBackend
    {
        /// <summary>
        /// サウンド停止時に呼ばれるイベントです。
        /// </summary>
        event EventHandler<SoundStopEventArgs> Stopped;

        /// <summary>
        /// 音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        double Volume
        {
            get;
            set;
        }

        /// <summary>
        /// 再生長さを取得します。
        /// </summary>
        int Length
        {
            get;
        }

        /// <summary>
        /// 再生が終わったかどうかを取得します。
        /// </summary>
        bool IsFinished
        {
            get;
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        void Stop();
    }


    /// <summary>
    /// 
    /// </summary>
    internal sealed class SoundObjectBackend_Dummy : ISoundObjectBackend
    {
        /// <summary>
        /// サウンド停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler<SoundStopEventArgs> Stopped;

        /// <summary>
        /// 音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double Volume
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// 再生長さを取得します。
        /// </summary>
        public int Length
        {
            get { return 0; }
        }

        /// <summary>
        /// 再生が終わったかどうかを取得します。
        /// </summary>
        public bool IsFinished
        {
            get { return true; }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
        }
    }
}
