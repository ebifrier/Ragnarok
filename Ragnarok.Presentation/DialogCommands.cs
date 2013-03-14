using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation
{
    /// <summary>
    /// ダイアログ関連のコマンドです。
    /// </summary>
    public static class DialogCommands
    {
        /// <summary>
        /// OKボタンコマンド。
        /// </summary>
        public static readonly ICommand OK =
            new RoutedUICommand(
                "OKボタン", "OK",
                typeof(FrameworkElement));

        /// <summary>
        /// キャンセルボタンコマンド。
        /// </summary>
        public static readonly ICommand Cancel =
            new RoutedUICommand(
                "キャンセルボタン", "Cancel",
                typeof(FrameworkElement));

        /// <summary>
        /// Yesボタンコマンド。
        /// </summary>
        public static readonly ICommand Yes =
            new RoutedUICommand(
                "Yesボタン", "Yes",
                typeof(FrameworkElement));

        /// <summary>
        /// Noボタンコマンド。
        /// </summary>
        public static readonly ICommand No =
            new RoutedUICommand(
                "Noボタン", "No",
                typeof(FrameworkElement));

        /// <summary>
        /// デフォルトのコマンドを接続します。
        /// </summary>
        public static void BindCommands(UIElement element)
        {
            BindCommands(element.CommandBindings);
        }

        /// <summary>
        /// デフォルトのコマンドを接続します。
        /// </summary>
        public static void BindCommands(CommandBindingCollection bindings)
        {
            bindings.Add(
                new CommandBinding(
                    DialogCommands.OK,
                    ExecuteYes));
            bindings.Add(
                new CommandBinding(
                    DialogCommands.Cancel,
                    ExecuteNo));

            bindings.Add(
                new CommandBinding(
                    DialogCommands.Yes,
                    ExecuteYes));
            bindings.Add(
                new CommandBinding(
                    DialogCommands.No,
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
