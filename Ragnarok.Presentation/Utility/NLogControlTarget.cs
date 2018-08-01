using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// TextBoxBaseコントロールにログ文字列を追加
    /// </summary>
    [Target("Control")]
    public class NLogControlTarget : TargetWithLayout
    {
        /// <summary>
        /// 出力対象とするコントロール名を取得または設定します。
        /// </summary>
        [RequiredParameter]
        public string ControlName
        {
            get;
            set;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);

            AddLog(logMessage);
        }

        /// <summary>
        /// 新しいログを出力
        /// </summary>
        public void AddLog(string logMessage)
        {
            var control = Log.FindTarget<TextBoxBase>(ControlName);
            if (control == null)
            {
                return;
            }

            WPFUtil.UIProcess(() =>
            {
                control.AppendText(logMessage + Environment.NewLine);
            });
        }
    }
}
