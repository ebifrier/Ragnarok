using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Sound.Backend
{
    /// <summary>
    /// IrrKlangの音声ファイルを再生します。
    /// </summary>
    /// <remarks>
    /// 使用dllが正しく初期化できない場合、
    /// このクラスを使った時点で例外が発生します。
    /// </remarks>
    internal interface ISoundManagerBackend
    {
        /// <summary>
        /// 音声を再生できるかどうかを取得します。
        /// </summary>
        bool CanUseSound
        {
            get;
        }

        /// <summary>
        /// ボリュームを0-1の間で取得または設定します。
        /// </summary>
        double Volume
        {
            get;
            set;
        }

        /// <summary>
        /// SEを再生します。
        /// </summary>
        ISoundObjectBackend Play(string filename, double volume);
    }


    /// <summary>
    /// ダミーです。
    /// </summary>
    internal sealed class SoundManagerBackend_Dummy : ISoundManagerBackend
    {
        /// <summary>
        /// 音声を再生できるかどうかを取得します。
        /// </summary>
        public bool CanUseSound
        {
            get { return false; }
        }

        /// <summary>
        /// ボリュームを0-1の間で取得または設定します。
        /// </summary>
        public double Volume
        {
            get { return 0.0; }
            set { }
        }

        /// <summary>
        /// SEを再生します。
        /// </summary>
        public ISoundObjectBackend Play(string filename, double volume)
        {
            return new SoundObjectBackend_Dummy();
        }
    }
}
