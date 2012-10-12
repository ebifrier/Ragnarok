using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
