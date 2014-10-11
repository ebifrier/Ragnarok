using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input.Detail
{
    /// <summary>
    /// ButtonBase用のバインディングクラスです。
    /// </summary>
    internal sealed class ButtonCommandBinding : CommandBindingBase
    {
        private ButtonBase target;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ButtonCommandBinding(ButtonBase button, ICommand command,
                                    Func<object> commandParameterCallback)
            : base(button, command, commandParameterCallback)
        {
            this.target = button;
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
