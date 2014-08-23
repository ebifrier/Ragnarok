using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input.Detail
{
    /// <summary>
    /// MenuItem用のバインディングクラスです。
    /// </summary>
    internal sealed class MenuItemCommandBinding : CommandBindingBase
    {
        private ToolStripMenuItem target;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MenuItemCommandBinding(ToolStripMenuItem target, ICommand command,
                                      Func<object> commandParameterCallback)
            : base(target, command, commandParameterCallback)
        {
            this.target = target;
            this.target.Click += (_, __) => DoExecute();

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
            this.target.Click -= (_, __) => DoExecute();
            this.target = null;
        }
    }
}
