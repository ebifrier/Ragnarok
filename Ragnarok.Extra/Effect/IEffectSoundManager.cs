using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Extra.Effect
{
    using Sound;

    /// <summary>
    /// 音声ファイルを再生します。
    /// </summary>
    public interface IEffectSoundManager
    {
        /// <summary>
        /// SEを再生します。
        /// </summary>
        SoundObject PlayEffectSound(string filename, double volume);
    }
}
