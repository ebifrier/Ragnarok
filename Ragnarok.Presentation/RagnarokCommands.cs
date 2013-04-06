using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation
{
    using Debug;
    using Control;

    /// <summary>
    /// ダイアログ関連のコマンドです。
    /// </summary>
    public static class RagnarokCommands
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
        /// 指定のURLを開きます。
        /// </summary>
        public readonly static ICommand NavigateUrl =
            new RoutedUICommand(
                "指定のURLを開きます。",
                "NavigateUrl",
                typeof(FrameworkElement));
        /// <summary>
        /// エラーログを送信します。
        /// </summary>
        public readonly static ICommand SendErrorLog =
            new RoutedUICommand(
                "エラーログを送信します。",
                "SendErrorLog",
                typeof(FrameworkElement));
        /// <summary>
        /// 新バージョンの確認を行います。
        /// </summary>
        public readonly static ICommand CheckToUpdate =
            new RoutedUICommand(
                "新バージョンの確認を行います。",
                "CheckToUpdate",
                typeof(FrameworkElement));
        /// <summary>
        /// バージョンを表示します。
        /// </summary>
        public readonly static ICommand ShowVersion =
            new RoutedUICommand(
                "バージョンを表示します。",
                "ShowVersion",
                typeof(FrameworkElement));

        /// <summary>
        /// デフォルトのコマンドを接続します。
        /// </summary>
        public static void Bind(UIElement element)
        {
            Bind(element.CommandBindings);
        }

        /// <summary>
        /// デフォルトのコマンドを接続します。
        /// </summary>
        public static void Bind(CommandBindingCollection bindings)
        {
            bindings.Add(
                new CommandBinding(
                    RagnarokCommands.OK,
                    ExecuteYes));
            bindings.Add(
                new CommandBinding(
                    RagnarokCommands.Cancel,
                    ExecuteNo));
            bindings.Add(
                new CommandBinding(
                    RagnarokCommands.Yes,
                    ExecuteYes));
            bindings.Add(
                new CommandBinding(
                    RagnarokCommands.No,
                    ExecuteNo));
        }

        /// <summary>
        /// デフォルトコマンドをバインディングします。
        /// </summary>
        static RagnarokCommands()
        {
            CommandManager.RegisterClassCommandBinding(
                typeof(Window),
                new CommandBinding(
                    RagnarokCommands.NavigateUrl,
                    ExecuteNavigateUrl));
            CommandManager.RegisterClassCommandBinding(
                typeof(Window),
                new CommandBinding(
                    RagnarokCommands.SendErrorLog,
                    ExecuteSendErrorLog));
            CommandManager.RegisterClassCommandBinding(
                typeof(Window),
                new CommandBinding(
                    RagnarokCommands.CheckToUpdate,
                    ExecuteCheckToUpdate));
            CommandManager.RegisterClassCommandBinding(
                typeof(Window),
                new CommandBinding(
                    RagnarokCommands.ShowVersion,
                    ExecuteShowVersion));
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

        /// <summary>
        /// 指定のURLをブラウザで開きます。
        /// </summary>
        /// <remarks>
        /// Parameterにurlを指定してください。
        /// </remarks>
        private static void ExecuteNavigateUrl(object sender,
                                               ExecutedRoutedEventArgs e)
        {
            string uri = string.Empty;

            try
            {
                if (e.Parameter == null)
                {
                    return;
                }

                uri = e.Parameter.ToString();
                if (string.IsNullOrEmpty(uri))
                {
                    return;
                }

                // 与えられたURLを開きます。
                Process.Start(uri);
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    string.Format(
                        "'{0}'の実行に失敗しました。", uri));
            }
        }

        /// <summary>
        /// エラーログを送信します。
        /// </summary>
        /// <remarks>
        /// Parameterにエラーログのファイル名を指定してください。
        /// </remarks>
        private static void ExecuteSendErrorLog(object sender,
                                                ExecutedRoutedEventArgs e)
        {
            try
            {
                var filename = e.Parameter as string;
                if (string.IsNullOrEmpty(filename))
                {
                    DialogUtil.ShowError(
                        "エラーログのファイル名が指定されていません(ToT)");
                    return;
                }

                var model = new ReportDialogModel();
                model.OpenErrorLog(filename);

                var dialog = new SendLogDialog(model);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "ダイアログの表示に失敗しました(ToT)");
            }
        }

        /// <summary>
        /// 新バージョンの確認を行います。
        /// </summary>
        /// <remarks>
        /// ParameterにPresentationUpdaterのオブジェクトを
        /// 指定してください。
        /// </remarks>
        private static void ExecuteCheckToUpdate(object sender,
                                                 ExecutedRoutedEventArgs e)
        {
            try
            {
                var updater = (Update.PresentationUpdater)e.Parameter;
                var timeout = TimeSpan.FromSeconds(20);

                updater.CheckToUpdate(timeout);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "新バージョンの確認に失敗しました。");
            }
        }

        /// <summary>
        /// バージョンを表示します。
        /// </summary>
        /// <remarks>
        /// Parameterにアセンブリ名を指定してください。
        /// nullの場合はEntryAssemblyのバージョンを表示します。
        /// </remarks>
        private static void ExecuteShowVersion(object sender,
                                               ExecutedRoutedEventArgs e)
        {
            try
            {
                var assemblyName = e.Parameter as string;
                var dialog = new VersionWindow(assemblyName);

                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "ダイアログの表示に失敗しました(ToT)");
            }
        }
    }
}
