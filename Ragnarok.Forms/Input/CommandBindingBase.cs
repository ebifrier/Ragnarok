using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input
{
    public abstract class CommandBindingBase : IDisposable
    {
        private bool disposed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected CommandBindingBase(Component component, ICommand command,
                                     Func<object> commandParameterCallback)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            Component = component;
            Command = command;
            CommandParameterCallback = commandParameterCallback;

            Component.Disposed += DoDisposeEvent;
            Command.CanExecuteChanged += CanExecuteChanged;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~CommandBindingBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void OnDisposed();

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    OnDisposed();

                    Command.CanExecuteChanged -= CanExecuteChanged;
                    Component.Disposed -= DoDisposeEvent;
                }

                CommandParameterCallback = null;
                Command = null;
                Component = null;

                this.disposed = true;
            }
        }

        private void DoDisposeEvent(object sender, EventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// 対応するコンポーネントを取得します。
        /// </summary>
        public Component Component
        {
            get;
            private set;
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
        /// コマンドパラメーターを返すコールバックを取得します。
        /// </summary>
        public Func<object> CommandParameterCallback
        {
            get;
            private set;
        }

        protected void DoExecuteEvent(object sender, EventArgs e)
        {
            DoExecute();
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        protected void DoExecute()
        {
            Command.Execute(CommandParameterCallback());
        }

        /// <summary>
        /// コマンドが実行可能か調べます。
        /// </summary>
        protected bool DoCanExecute()
        {
            return Command.CanExecute(CommandParameterCallback());
        }

        protected abstract void OnUpdatedEnabled();

        /// <summary>
        /// コマンドの実行可能性が変わった時に呼ばれます。
        /// </summary>
        private void CanExecuteChanged(object sender, EventArgs e)
        {
            OnUpdatedEnabled();
        }
    }
}
