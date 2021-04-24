using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ragnarok.Forms.Controls
{
    /// <summary>
    /// ToolStripExと同じ理由により作ったもの。
    /// </summary>
    public class MenuStripEx : MenuStrip
    {
        /// <summary>
        /// このToolStripのクリックされたときの動作。
        /// 上の4つの定数から選ぶ。MA_NOACTIVATEがこのToolStripExのデフォルト。
        /// </summary>
        public ClickAction ClickAction { get; set; } = ClickAction.MA_ACTIVATE;

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
