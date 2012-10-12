using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Sound
{
    /// <summary>
    /// 音声再生時に使うオブジェクトです。
    /// </summary>
    public class PlaySoundInfo
    {
        /// <summary>
        /// 音声ファイルのパスを取得または設定します。
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }

        /// <summary>
        /// 音量を取得または設定します。
        /// </summary>
        public double Volume
        {
            get;
            set;
        }
    }
}
