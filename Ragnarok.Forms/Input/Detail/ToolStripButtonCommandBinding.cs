using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input.Detail
{
    /// <summary>
    /// ToolStripButton用のバインディングクラスです。
    /// </summary>
    internal sealed class ToolStripButtonCommandBinding : CommandBindingBase
    {
        private ToolStripButton target;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ToolStripButtonCommandBinding(ToolStripButton target, ICommand command,
                                             Func<object> commandParameterCallback)
            : base(target, command, commandParameterCallback)
        {
            this.target = target;
            this.target.Click += event_DoExecute;

            OnUpdatedEnabled();
        }

        /// <summary>
        /// コマンドの実行可能性が変わった時に呼ばれます。
        /// </summary>
        protected override void OnUpdatedEnabled()
        {
            this.target.Enabled = DoCanExecute();
        }

        /// <summary>
        /// オブジェクトが破棄されたときに呼ばれます。
        /// </summary>
        protected override void OnDisposed()
        {
            this.target.Click -= event_DoExecute;
            this.target = null;
        }
    }
}
