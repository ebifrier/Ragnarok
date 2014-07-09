using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

using Ragnarok.Presentation.Extra.Effect;

namespace Ragnarok.Presentation.Shogi
{
    /// <summary>
    /// Ragnarok.Presentation.Shogi.dllを使うための初期化用クラスです。
    /// </summary>
    public static class ShogiInitializer
    {
        /// <summary>
        /// パスなどを初期化します。
        /// </summary>
        public static void Initialize(Assembly asm, string dataDir)
        {
            WPFUtil.Init();

            // パーティクルシステムの想定画面サイズを設定。
            FlintSharp.Utils.ScreenSize = new Size(640, 360);

            var uri = new Uri(
                new Uri(asm.Location),
                dataDir + "/xxx");

            EffectInfo.BaseDir = uri;

            var sm = EffectObject.SoundManager;
            sm.DefaultPath = Path.GetDirectoryName(uri.LocalPath);
            sm.PlayInterval = TimeSpan.FromSeconds(0.5);
            sm.Volume = 50;
        }
    }
}
