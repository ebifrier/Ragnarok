using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Sound.Backend
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
        /// 内部で使うオブジェクトを取得します。
        /// </summary>
        object State
        {
            get;
        }

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
        TimeSpan Length
        {
            get;
        }

        /// <summary>
        /// 再生中かどうかを取得します。
        /// </summary>
        bool IsPlaying
        {
            get;
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        void Stop(SoundStopReason reason);
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
        /// 内部で使うオブジェクトを取得します。
        /// </summary>
        public object State => null;

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
        public TimeSpan Length => TimeSpan.Zero;

        /// <summary>
        /// 再生中かどうかを取得します。
        /// </summary>
        public bool IsPlaying => true;

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop(SoundStopReason reason)
        {
            Stopped.SafeRaiseEvent(this, new SoundStopEventArgs(reason));
        }
    }
}
