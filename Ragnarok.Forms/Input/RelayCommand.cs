using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Ragnarok.Forms.Input
{
    /// <summary>
    /// <seeref name="RelayCommand"/>の実行可否を調べるためのイベント引数です。
    /// </summary>
    public class CanExecuteRelayEventArgs : EventArgs
    {
        /// <summary>
        /// コマンドを取得します。
        /// </summary>
        public ICommand Command
        {
            get;
            private set;
        }

        /// <summary>
        /// コマンドパラメーターを取得します。
        /// </summary>
        public object Parameter
        {
            get;
            private set;
        }

        /// <summary>
        /// コマンドが実行可能かどうかを取得または設定します。
        /// </summary>
        public bool CanExecute
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CanExecuteRelayEventArgs(ICommand command)
        {
            Command = command;
            CanExecute = true;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CanExecuteRelayEventArgs(ICommand command, object parameter)
            : this(command)
        {
            Parameter = parameter;
        }
    }

    /// <summary>
    /// コマンドをデリゲートで実行するようなクラスです。
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly EventHandler<CanExecuteRelayEventArgs> canExecute;

        /// <summary>
        /// コマンドの実行可能状態の変更を調べるイベントを追加または削除します。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        public void Execute()
        {
            try
            {
                this.execute();
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        /// <summary>
        /// コマンドの実行可能状態を調べます。
        /// </summary>
        public bool CanExecute()
        {
            if (this.canExecute == null)
            {
                return true;
            }

            var e = new CanExecuteRelayEventArgs(this);
            this.canExecute(this, e);
            return e.CanExecute;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RelayCommand(Action execute,
                            EventHandler<CanExecuteRelayEventArgs> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }
    }

    /// <summary>
    /// コマンドをデリゲートで実行するようなクラスです。(パラメータ有り)
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly EventHandler<CanExecuteRelayEventArgs> canExecute;

        /// <summary>
        /// コマンドの実行可能状態の変更を調べるイベントを追加または削除します。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        public void Execute(object parameter)
        {
            try
            {
                this.execute((T)parameter);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// コマンドの実行可能状態を調べます。
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }

            var e = new CanExecuteRelayEventArgs(this, parameter);
            this.canExecute(this, e);
            return e.CanExecute;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RelayCommand(Action<T> execute,
                            EventHandler<CanExecuteRelayEventArgs> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }
    }
}