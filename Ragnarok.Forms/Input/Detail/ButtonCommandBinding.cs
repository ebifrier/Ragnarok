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
        private ButtonBase button;
        private Func<object> commandParameterCallback;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ButtonCommandBinding(ButtonBase button, ICommand command,
                                    Func<object> commandParameterCallback)
            : base(button, command)
        {
            this.button = button;
            this.commandParameterCallback = commandParameterCallback;
            this.button.Click += ButtonClick;

            UpdateEnabledProperty();
        }

        /// <summary>
        /// コマンドの実行可能性が変わった時に呼ばれます。
        /// </summary>
        protected override void OnCanExecuteChanged()
        {
            UpdateEnabledProperty();
        }

        /// <summary>
        /// 対応するコンポーネントが破棄されたときに呼ばれます。
        /// </summary>
        protected override void OnComponentDisposed()
        {
            this.button.Click -= ButtonClick;

            this.button = null;
            this.commandParameterCallback = null;
        }

        /// <summary>
        /// コマンドの状態に応じて、ボタンの有効／無効を切り替えます。
        /// </summary>
        private void UpdateEnabledProperty()
        {
            this.button.Enabled = Command.CanExecute(commandParameterCallback());
        }

        /// <summary>
        /// ボタンクリックを処理します。
        /// </summary>
        private void ButtonClick(object sender, EventArgs e)
        {
            Command.Execute(commandParameterCallback());
        }
    }
}
