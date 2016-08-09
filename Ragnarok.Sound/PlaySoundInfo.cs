using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Sound
{
    /// <summary>
    /// 音声再生時に使うオブジェクトです。
    /// </summary>
    public class PlaySoundInfo
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlaySoundInfo()
        {
            Volume = 1.0;
            UsePlayInterval = true;
        }

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

        public bool UsePlayInterval
        {
            get;
            set;
        }

        public bool IgnoreError
        {
            get;
            set;
        }
    }
}
