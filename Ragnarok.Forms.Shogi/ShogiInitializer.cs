using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using FlintSharp;

namespace Ragnarok.Forms.Shogi
{
    using FlintSharpEx;

    /// <summary>
    /// Ragnarok.Forms.Shogi.dllを使うための初期化用クラスです。
    /// </summary>
    public static class ShogiInitializer
    {
        /// <summary>
        /// パスなどを初期化します。
        /// </summary>
        public static void Initialize()
        {
            FormsUtil.Initialize();

            // パーティクルシステムの想定画面サイズを設定。
            Utils.ScreenSize = new Size(640, 360);

            // 画像のローダーを設定。
            Utils.ImageLoader = new GLImageLoader();
        }
    }
}
