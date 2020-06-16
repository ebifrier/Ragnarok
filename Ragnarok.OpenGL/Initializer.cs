using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// 開始時に必要な処理を行います。
    /// </summary>
    public static class Initializer
    {
#if !MONO
        private static class NativeMethods
        {
            [DllImport("nvapi64.dll")]
            public static extern int LoadNvApi64();

            [DllImport("nvapi.dll")]
            public static extern int LoadNvApi32();
        }
#endif

        /// <summary>
        /// 最適なGPUを選択します。ウィンドウの作成前に呼んで下さい。
        /// </summary>
        public static void InitializeGraphics()
        {
#if !MONO
            try
            {
                if (Environment.Is64BitProcess)
                {
                    NativeMethods.LoadNvApi64();
                }
                else
                {
                    NativeMethods.LoadNvApi32();
                }
            }
            catch
            {
                // will always fail since the method entry point doesn't exists}
            }
#endif
        }
    }
}
