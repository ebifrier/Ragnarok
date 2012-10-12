using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation.Command
{
    /// <summary>
    /// コマンドをデリゲートで実行するようなクラスです。
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

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
            this.execute();
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
            return (this.canExecute == null ||
                    this.canExecute());
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RelayCommand(Action execute, Func<bool> canExecute)
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
        private readonly Func<T, bool> canExecute;

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
            this.execute((T)parameter);
        }

        /// <summary>
        /// コマンドの実行可能状態を調べます。
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return (this.canExecute == null ||
                    this.canExecute((T)parameter));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
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