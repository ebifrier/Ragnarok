using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Sound
{
    /// <summary>
    /// 音声再生時に使うオブジェクトです。
    /// </summary>
    public class SoundInfo
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SoundInfo()
        {
            Volume = 1.0;
            CheckPlayInterval = false;
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

        /// <summary>
        /// 必要であればSEの再生間隔が短くなりすぎないように間隔調整を行います。
        /// </summary>
        public bool CheckPlayInterval
        {
            get;
            set;
        }

        /// <summary>
        /// ファイルがない場合など、エラーが出てもそれを出力しないようにします。
        /// </summary>
        public bool IgnoreError
        {
            get;
            set;
        }
    }
}
