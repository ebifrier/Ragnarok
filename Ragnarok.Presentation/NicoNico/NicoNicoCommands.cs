using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation.NicoNico
{
    /// <summary>
    /// ニコニコ関連のコマンドです。
    /// </summary>
    public static class NicoNicoCommands
    {
        /// <summary>
        /// コマンド。
        /// </summary>
        public static readonly ICommand Connect =
            new RoutedUICommand(
                "OKボタン", "Connect",
                typeof(FrameworkElement));

        /// <summary>
        /// キャンセルボタンコマンド。
        /// </summary>
        public static readonly ICommand Disconnect =
            new RoutedUICommand(
                "キャンセルボタン", "Disconnect",
                typeof(FrameworkElement));

        /// <summary>
        /// デフォルトのコマンドを接続します。
        /// </summary>
        public static void BindCommands(UIElement element)
        {
            element.CommandBindings.Add(
                new CommandBinding(
                    NicoNicoCommands.Connect,
                    ExecuteYes));
            element.CommandBindings.Add(
                new CommandBinding(
                    NicoNicoCommands.Disconnect,
                    ExecuteNo));
        }

        /// <summary>
        /// OK/YES
        /// </summary>
        private static void ExecuteYes(object sender, ExecutedRoutedEventArgs e)
        {
            var window = (Window)sender;

            window.DialogResult = true;
        }

        /// <summary>
        /// Cancel/NO
        /// </summary>
        private static void ExecuteNo(object sender, ExecutedRoutedEventArgs e)
        {
            var window = (Window)sender;

            window.DialogResult = false;
        }
    }
}
