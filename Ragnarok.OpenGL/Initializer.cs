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
        /// <summary>
        /// 優先的にNVIDIA GPUを使うようにします。ウィンドウの作成前に呼んで下さい。
        /// </summary>
        /// <remarks>
        /// ノートパソコンなどでは内臓GPUは優先的に使われることがあるため
        /// 外付けのNVIDIA GPUを優先的に使うようにします。
        /// </remarks>
        public static void PrioritizeNvidiaGPU()
        {
#if !MONO
            try
            {
                if (Environment.Is64BitProcess)
                {
                    NativeLibrary.Load("nvapi64.dll");
                }
                else
                {
                    NativeLibrary.Load("nvapi.dll");
                }
            }
            catch (Exception ex)
            {
                // will always fail since the method entry point doesn't exists}
                Log.ErrorException(ex,
                    "Nvidia GPUの初期化に失敗しました。");
            }
#endif
        }
    }
}
