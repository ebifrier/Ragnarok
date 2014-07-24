using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ragnarok.Forms.Input
{
    /// <summary>
    /// WinForms用のコマンドインターフェースです。
    /// </summary>
    /// <remarks>
    /// WPFと全く同じインターフェースです。
    /// </remarks>
    public interface ICommand
    {
        /// <summary>
        /// コマンドの実行可否状態が変わった可能性がある場合に呼ばれるイベントです。
        /// </summary>
        event EventHandler CanExecuteChanged;

        /// <summary>
        /// コマンドが実行可能かどうかを調べます。
        /// </summary>
        bool CanExecute(object parameter);

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        void Execute(object parameter);
    }
}
