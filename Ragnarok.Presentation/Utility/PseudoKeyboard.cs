using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// キー操作を疑似的に行います。
    /// </summary>
    public static class PseudoKeyboard
    {
        #region
        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        #endregion

        /// <summary>
        /// 指定された時間[ms]、指定のキーを押します。
        /// </summary>
        public static void PressKeyWait(Key key, int elapsed)
        {
            PressKeyWait(key, TimeSpan.FromMilliseconds(elapsed));
        }

        /// <summary>
        /// 指定された時間、指定のキーを押します。
        /// </summary>
        public static void PressKeyWait(Key key, TimeSpan elapsed)
        {
            var vk = (byte)KeyInterop.VirtualKeyFromKey(key);

            keybd_event(vk, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(elapsed);
            keybd_event(vk, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
    }
}
