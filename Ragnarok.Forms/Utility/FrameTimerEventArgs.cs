using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Forms.Utility
{
    /// <summary>
    /// 各フレームで呼ばれるイベントの引数です。
    /// </summary>
    public sealed class FrameEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrameEventArgs(double elapsed, bool frameSkipped)
        {
            ElapsedTime = TimeSpan.FromMilliseconds(elapsed);
            FrameSkipped = frameSkipped;
        }

        /// <summary>
        /// フレーム時間を取得します。
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 更新時間の関係でフレームが飛ばされたかどうかを取得します。
        /// </summary>
        public bool FrameSkipped
        {
            get;
            private set;
        }
    }
}
