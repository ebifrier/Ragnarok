using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Sound
{
    /// <summary>
    /// 音声ファイルを再生します。
    /// </summary>
    public interface ISoundManager
    {
        /// <summary>
        /// 音声を再生できるかどうかを取得します。
        /// </summary>
        bool CanUseSound
        {
            get;
        }

        /// <summary>
        /// ボリュームを0-100の間で取得または設定します。
        /// </summary>
        double Volume
        {
            get;
            set;
        }

        /// <summary>
        /// SEを再生します。
        /// </summary>
        SoundObject PlaySE(string filename, double volume,
                           bool checkTime, bool ignoreError);
    }
}
