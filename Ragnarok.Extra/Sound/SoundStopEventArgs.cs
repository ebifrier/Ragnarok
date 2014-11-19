using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ragnarok.Extra.Sound
{
    /// <summary>
    /// サウンドが停止した理由です。
    /// </summary>
    public enum SoundStopReason
    {
        FinishedPlaying,
        StoppedByUser,
        StoppedBySourceRemoval,
    }

    /// <summary>
    /// サウンドの停止イベントの引数です。
    /// </summary>
    public sealed class SoundStopEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SoundStopEventArgs(SoundStopReason reason)
        {
            Reason = reason;
        }

        /// <summary>
        /// サウンドが停止した理由を取得します。
        /// </summary>
        public SoundStopReason Reason
        {
            get;
            private set;
        }
    }
}
