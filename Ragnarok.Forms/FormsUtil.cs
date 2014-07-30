using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Ragnarok.Forms
{
    public static class FormsUtil
    {
        /// <summary>
        /// BeginInvokeなどを実行するコントロールを取得または設定します。
        /// </summary>
        public static Control InvokeControl
        {
            get;
            set;
        }

        /// <summary>
        /// UIThread上でメソッドを実行します。
        /// </summary>
        public static void UIProcess(Action func)
        {
            if (InvokeControl == null)
            {
                throw new InvalidOperationException(
                    "InvokeControlがnullです。");
            }

            if (InvokeControl.InvokeRequired)
            {
                InvokeControl.BeginInvoke(func);
            }
            else
            {
                func();
            }
        }

        /// <summary>
        /// UIThread上でメソッドを実行します。
        /// </summary>
        public static void UIProcess(this Control control, Action func)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(func);
            }
            else
            {
                func();
            }
        }
    }
}
