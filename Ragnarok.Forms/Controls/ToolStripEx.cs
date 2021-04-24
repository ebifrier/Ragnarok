using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ragnarok.Forms.Controls
{
    /// <summary>
    /// .NET FrameworkのToolStrip、非アクティブ状態からだとボタンクリックが反応しなくて不便なので
    /// 非アクティブ状態からでも反応するものを用意。
    /// 
    /// cf.
    /// http://bbs.wankuma.com/index.cgi?mode=al2&namber=69912&KLOG=119
    /// https://blogs.msdn.microsoft.com/rickbrew/2006/01/09/how-to-enable-click-through-for-net-2-0-toolstrip-and-menustrip/
    /// </summary>
    public partial class ToolStripEx : ToolStrip
    {
        /// <summary>
        /// このToolStripのクリックされたときの動作。
        /// 上の4つの定数から選ぶ。MA_NOACTIVATEがこのToolStripExのデフォルト。
        /// </summary>
        public ClickAction ClickAction { get; set; } = ClickAction.MA_NOACTIVATE;

        // これMonoでもうまく動くのかどうかわからんが、たぶん動くのかな…。
        protected const int WM_MOUSEACTIVATE = 0x0021;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_MOUSEACTIVATE &&
                m.Result == (IntPtr)ClickAction.MA_ACTIVATEANDEAT)
            {
                // このメッセージを書き換える。
                m.Result = (IntPtr)ClickAction;
            }
        }
    }
}
