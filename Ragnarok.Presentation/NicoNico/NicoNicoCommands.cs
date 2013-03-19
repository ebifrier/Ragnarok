using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

using Ragnarok.NicoNico;
using Ragnarok.NicoNico.Login;
using Ragnarok.NicoNico.Live;

namespace Ragnarok.Presentation.NicoNico
{
    /// <summary>
    /// ニコ生への接続時に使うモデルクラスです。
    /// </summary>
    public class NicoLiveCommandData
    {
        /// <summary>
        /// 放送URLを取得または設定します。
        /// </summary>
        public string LiveUrl
        {
            get;
            set;
        }

        /// <summary>
        /// ログインするクライアントクラスを取得または設定します。
        /// </summary>
        public NicoClient NicoClient
        {
            get;
            set;
        }

        /// <summary>
        /// 放送へ接続するためのオブジェクトを取得します。
        /// </summary>
        public CommentClient CommentClient
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoLiveCommandData()
        {
            NicoClient = new NicoClient();
            CommentClient = new CommentClient();
        }
    }

    /// <summary>
    /// ニコニコ関連のコマンドです。
    /// </summary>
    public static class NicoNicoCommands
    {
        /// <summary>
        /// ログインを行います。
        /// </summary>
        public static readonly ICommand Login =
            new RoutedUICommand(
                "ログインを行います。", "Login",
                typeof(FrameworkElement));
        /// <summary>
        /// 生放送への接続コマンド。
        /// </summary>
        public static readonly ICommand Connect =
            new RoutedUICommand(
                "生放送に接続。", "Connect",
                typeof(FrameworkElement));
        /// <summary>
        /// 生放送からの切断コマンド。
        /// </summary>
        public static readonly ICommand Disconnect =
            new RoutedUICommand(
                "生放送から切断。", "Disconnect",
                typeof(FrameworkElement));

        /// <summary>
        /// デフォルトのコマンドを接続します。
        /// </summary>
        public static void BindCommands(UIElement element)
        {
            element.CommandBindings.Add(
                new CommandBinding(
                    NicoNicoCommands.Login,
                    ExecuteLogin));
            element.CommandBindings.Add(
                new CommandBinding(
                    NicoNicoCommands.Connect,
                    ExecuteConnect));
            element.CommandBindings.Add(
                new CommandBinding(
                    NicoNicoCommands.Disconnect,
                    ExecuteDisconnect));
        }

        /// <summary>
        /// ログイン処理を行います。
        /// </summary>
        private static void ExecuteLogin(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var data = (NicoLiveCommandData)e.Parameter;
                var window = new LoginWindow(data.NicoClient);

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "ログインに失敗しました。");
            }
        }

        /// <summary>
        /// 生放送に接続します。
        /// </summary>
        private static void ExecuteConnect(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var data = (NicoLiveCommandData)e.Parameter;

                data.CommentClient.Connect(
                    data.LiveUrl,
                    data.NicoClient.CookieContainer);

                data.CommentClient.StartReceiveMessage(-1);
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                DialogUtil.ShowError(ex,
                    "生放送への接続に失敗しました。");
            }
        }

        /// <summary>
        /// 生放送から切断します。
        /// </summary>
        private static void ExecuteDisconnect(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var data = (NicoLiveCommandData)e.Parameter;

                data.CommentClient.Disconnect();
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
            }
        }
    }
}
