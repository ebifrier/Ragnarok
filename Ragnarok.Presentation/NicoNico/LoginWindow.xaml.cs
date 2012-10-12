using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

using Ragnarok;
using Ragnarok.NicoNico;
using Ragnarok.NicoNico.Login;
using Ragnarok.NicoNico.Live;

namespace Ragnarok.Presentation.NicoNico
{
    /// <summary>
    /// LoginWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// ログイン成功/失敗時にダイアログを表示するかどうかです。
        /// </summary>
        public static readonly DependencyProperty IsShowMessageDialogProperty =
            DependencyProperty.Register(
                "IsShowMessageDialog",
                typeof(bool), typeof(LoginWindow),
                new UIPropertyMetadata(true));

        /// <summary>
        /// ログインコマンドです。
        /// </summary>
        public static RoutedUICommand Login =
            new RoutedUICommand(
                "ニコニコにログインします。",
                "LoginCommand",
                typeof(LoginWindow));

        private readonly LoginModel model;
        private readonly NicoClient nicoClient;

        /// <summary>
        /// ログイン後にメッセージを表示するかを取得または設定します。
        /// </summary>
        public bool IsShowMessageDialog
        {
            get { return (bool)GetValue(IsShowMessageDialogProperty); }
            set { SetValue(IsShowMessageDialogProperty, value); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginWindow()
            : this(new NicoClient())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginWindow(NicoClient nicoClient)
        {
            InitializeComponent();
            InitializeCommand();

            this.model = new LoginModel()
            {
                Data =
                    ( nicoClient.LoginData != null
                    ? nicoClient.LoginData.Clone()
                    : new LoginData()),
            };

            this.nicoClient = nicoClient;
            this.layoutBase.DataContext = this.model;
            this.passwordBox.Password = this.model.Data.Password;
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;

            this.model.Data.Password = passwordBox.Password;
        }

        #region コマンド
        /// <summary>
        /// コマンドを初期化します。
        /// </summary>
        private void InitializeCommand()
        {
            CommandBindings.Add(
                new CommandBinding(Login,
                    ExecuteLogin, CanExecuteLogin));
        }

        /// <summary>
        /// ログイン操作が行えるか調べます。
        /// </summary>
        private void CanExecuteLogin(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.model == null)
            {
                e.CanExecute = false;
                return;
            }

            // ログインの種類は複数あります。
            switch (this.model.Data.LoginMethod)
            {
                case LoginMethod.Direct:
                    e.CanExecute = (
                        !string.IsNullOrEmpty(this.model.Data.Mail) &&
                        !string.IsNullOrEmpty(this.model.Data.Password));
                    return;
                case LoginMethod.WithBrowser:
                    e.CanExecute = true;
                    return;
                case LoginMethod.AvailableCookie:
                    e.CanExecute = (
                        this.model.AvailableCookieContainer != null &&
                        this.model.AvailableCookieBrowser != null);
                    return;
            }

            e.CanExecute = false;
        }

        /// <summary>
        /// ログインデータからログインを行います。
        /// 自動ログイン時などに使われます。
        /// </summary>
        private void ExecuteLogin(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.model == null)
            {
                return;
            }

            try
            {
                // ログイン処理を行い、必要ならダイアログを表示します。
                var loginData = LoginInternal();
                if (loginData != null)
                {
                    if (IsShowMessageDialog)
                    {
                        DialogUtil.Show(
                            "ログインできました。ヽ( *^∇^*)ﾉ*:･'ﾟ☆",
                            "OK",
                            MessageBoxButton.OK);
                    }

                    DialogResult = true;
                }
                else
                {
                    DialogUtil.ShowError(
                        "ログインできませんでした。( ´ω`)");
                }
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "ログインに失敗しました。( ´ω`)");
            }
        }

        /// <summary>
        /// 実際のログイン処理を行います。
        /// </summary>
        private LoginData LoginInternal()
        {
            if (this.model == null || this.model.Data == null)
            {
                return null;
            }

            // 使用可能なクッキーリストから選ぶときは、データの内容を
            // 変えることがあるため、必ずコピーしたものを渡します。
            var loginData = this.model.Data.Clone();
            var result = false;

            // ログインの種類は複数あります。
            switch (this.model.Data.LoginMethod)
            {
                case LoginMethod.Direct:
                case LoginMethod.WithBrowser:
                    // 通常の方法でログインします。
                    result = this.nicoClient.Login(loginData);
                    break;
                case LoginMethod.AvailableCookie:
                    // ブラウザからのログインとし、クッキーなどを指定します。
                    loginData.LoginMethod = LoginMethod.WithBrowser;
                    loginData.BrowserType =
                        this.model.AvailableCookieBrowser.Value;

                    // ログインデータがクッキーを持っていたらそれを使い、
                    // 持っていなければブラウザから取得します。
                    if (this.model.AvailableCookieContainer == null)
                    {
                        result = this.nicoClient.Login(loginData);
                    }
                    else
                    {
                        result = this.nicoClient.Login(
                            this.model.AvailableCookieContainer,
                            loginData);
                    }
                    break;
            }

            // ログインが成功したときはそのデータを返します。
            return (result ? loginData : null);
        }
        #endregion
    }
}
