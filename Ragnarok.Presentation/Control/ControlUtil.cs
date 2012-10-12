using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// WPFコントロール用の便利クラス
    /// </summary>
    public static class ControlUtil
    {
        #region PInvoke
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;

        const int ICON_SMALL = 0;
        const int ICON_BIG = 1;
        #endregion

        /// <summary>
        /// ウィンドウの左上のアイコンを非表示にします。
        /// windows限定のコード
        /// </summary>
        /// <remarks>
        /// OnSourceInitializedで呼ぶのが一般的です。
        /// </remarks>
        public static void RemoveIcon(Window window)
        {
            var hwnd = (new WindowInteropHelper(window)).Handle;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_SMALL, IntPtr.Zero);
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_BIG, IntPtr.Zero);
            SetWindowPos(
                hwnd, IntPtr.Zero, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }
    }
}
