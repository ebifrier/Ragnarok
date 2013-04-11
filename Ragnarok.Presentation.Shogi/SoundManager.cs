using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ragnarok.Presentation.Shogi
{
    /// <summary>
    /// 音声ファイルを再生します。
    /// </summary>
    [CLSCompliant(false)]
    public sealed class SoundManager : Ragnarok.Extra.Sound.SoundManager
    {
        private static SoundManager instance = new SoundManager();

        /// <summary>
        /// シングルトンを取得します。
        /// </summary>
        public static SoundManager Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// 音声プレイヤーオブジェクトを初期化します。
        /// </summary>
        public SoundManager()
        {
            DefaultPath = Path.Combine(AssemblyLocation, "ShogiData");
            PlayInterval = TimeSpan.FromSeconds(0.5);
            Volume = 50;
        }
    }
}
