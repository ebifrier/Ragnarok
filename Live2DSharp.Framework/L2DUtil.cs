using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Live2DSharp.Framework
{
    /// <summary>
    /// Live2D用のユーティリティクラスです。
    /// </summary>
    public static class L2DUtil
    {
        /// <summary>
        /// Live2Dの初期化を行います。
        /// </summary>
        public static void Initialize()
        {
            Live2D.init(new LDAllocator());
        }

        /// <summary>
        /// Live2Dを終了します。
        /// </summary>
        public static void Quit()
        {
            Live2D.dispose();
        }
    }
}
