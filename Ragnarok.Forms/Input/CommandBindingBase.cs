using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input
{
    public abstract class CommandBindingBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected CommandBindingBase(Component component, ICommand command)
        {
            Component = component;
            Command = command;

            Component.Disposed += ComponentDisposed;
            Command.CanExecuteChanged += CanExecuteChanged;
        }

        /// <summary>
        /// コマンドを取得します。
        /// </summary>
        public ICommand Command
        {
            get;
            private set;
        }

        /// <summary>
        /// 対応するコンポーネントを取得します。
        /// </summary>
        public Component Component
        {
            get;
            private set;
        }

        protected abstract void OnCanExecuteChanged();

        /// <summary>
        /// コマンドの実行可能性が変わった時に呼ばれます。
        /// </summary>
        private void CanExecuteChanged(object sender, EventArgs e)
        {
            OnCanExecuteChanged();
        }

        protected abstract void OnComponentDisposed();

        /// <summary>
        /// 対応するコンポーネントが破棄されたときに呼ばれます。
        /// </summary>
        private void ComponentDisposed(object sender, EventArgs e)
        {
            OnComponentDisposed();

            Command.CanExecuteChanged -= CanExecuteChanged;
            Component.Disposed -= ComponentDisposed;

            Component = null;
            Command = null;
        }
    }
}
