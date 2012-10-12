using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Hal.CookieGetterSharp;
using Ragnarok.Net;
using Ragnarok.NicoNico;
using Ragnarok.NicoNico.Login;

namespace Ragnarok.Wpf.NicoNico
{
    public static partial class LoginCommands
    {
        public static RoutedUICommand LoginCommand =
            new RoutedUICommand(
                "ニコニコにログインします。",
                "LoginCommand",
                typeof(LoginCommands));

        /// <summary>
        /// クッキーの妥当性の確認を行います。
        /// </summary>
        private static bool ValidateCookie(LoginModel loginModel,
                                           CookieContainer cc)
        {
            var validator = loginModel.CookieValidator;

            if (validator == null)
            {
                return true;
            }

            return validator(cc);
        }

        public static bool CanLogin(object parameter)
        {
            var loginModel = parameter as LoginModel;
            if (loginModel == null)
            {
                return false;
            }

            switch (loginModel.Data.LoginMethod)
            {
                case LoginMethod.Direct:
                    return CanDirectLogin(loginModel);
                case LoginMethod.WithBrowser:
                    return CanLoginWithBrowser(loginModel);
                case LoginMethod.AvailableCookie:
                    return CanLoginWithAvailableCookie(loginModel);
            }

            return false;
        }

        /// <summary>
        /// ログインデータからログインを行います。
        /// 自動ログイン時などに使われます。
        /// </summary>
        public static void Login(object parameter)
        {
            var loginModel = parameter as LoginModel;
            if (loginModel == null)
            {
                return;
            }

            switch (loginModel.Data.LoginMethod)
            {
                case LoginMethod.Direct:
                    DirectLogin(loginModel);
                    break;
                case LoginMethod.WithBrowser:
                    LoginWithBrowser(loginModel);
                    break;
                case LoginMethod.AvailableCookie:
                    LoginWithAvailableCookie(loginModel);
                    break;
            }
        }

        /// <summary>
        /// 指定のブラウザのクッキーを使って、ログインします。
        /// </summary>
        /*private static void LoginWithBrowserInternal(LoginModel loginModel,
                                                     BrowserType browser)
        {
            try
            {
                var cookieGetter = CookieGetter.CreateInstance(browser);
                if (cookieGetter == null)
                {
                    MessageUtil.ErrorMessage(
                        "クッキーの取得に失敗しました (≧◇≦)\n" +
                        "ブラウザの種類を変えて試してみてください。");
                    return;
                }

                // 与えられたブラウザのログインクッキーを取得します。
                var cookieCollection = cookieGetter.GetCookieCollection(
                    new Uri(MessageString.NicoLiveUrl()));

                var cc = new CookieContainer();
                cc.Add(cookieCollection);

                ValidateCookie(loginModel, cc);
            }
            catch (Exception ex)
            {
                MessageUtil.ErrorMessage(ex,
                    "クッキーの取得に失敗しました (≧◇≦)\n" +
                    "ブラウザの種類を変えて試してみてください。");
            }
        }*/

        /// <summary>
        /// ニコニコに直接ログインできるか調べます。
        /// </summary>
        public static bool CanDirectLogin(LoginModel loginModel)
        {
            return (!string.IsNullOrEmpty(loginModel.Data.Mail) &&
                    !string.IsNullOrEmpty(loginModel.Data.Password));
        }

        /// <summary>
        /// ニコニコにログインします。
        /// </summary>
        public static void DirectLogin(LoginModel loginModel)
        {
            if (!CanDirectLogin(loginModel))
            {
                MessageUtil.ErrorMessage(
                    "ログイン出来ません (･ｪ･)\n" +
                    "メールアドレスとパスワードを設定してください。");
                return;
            }

            try
            {
                var cc = Ragnarok.NicoNico.Login.Loginer.DirectLogin(
                    loginModel.Data.Mail,
                    loginModel.Data.Password);
                if (cc == null)
                {
                    MessageUtil.ErrorMessage("ログインに失敗しました (･ｪ･)");
                    return;
                }

                ValidateCookie(loginModel, cc);
            }
            catch (Exception ex)
            {
                MessageUtil.ErrorMessage(ex,
                    "ログインに失敗しました (･ｪ･)");
            }
        }

        /// <summary>
        /// 指定のブラウザのクッキーを使ってログインできるか調べます。
        /// </summary>
        public static bool CanLoginWithBrowser(LoginModel loginModel)
        {
            return true;
        }

        /// <summary>
        /// 指定のブラウザのクッキーを使って、ログインします。
        /// </summary>
        public static void LoginWithBrowser(LoginModel loginModel)
        {
            var cc = Loginer.LoginWithBrowser(
                loginModel.Data.BrowserType,
                false);

            ValidateCookie(loginModel, cc);
        }

        /// <summary>
        /// 使用可能なクッキーを使ってニコニコにログインできるか調べます。
        /// </summary>
        public static bool CanLoginWithAvailableCookie(LoginModel loginModel)
        {
            return (loginModel.AvailableCookieContainer != null);
        }

        /// <summary>
        /// 使用可能なクッキーを使ってニコニコにログインします。
        /// </summary>
        public static void LoginWithAvailableCookie(LoginModel loginModel)
        {
            var browserType = loginModel.AvailableCookieBrowser;
            if (browserType == null)
            {
                MessageUtil.ErrorMessage(
                    "ブラウザが指定されていません (>◇< )");
                return;
            }

            // ログインデータがクッキーを持っていたらそれを使い、
            // 持っていなければブラウザから取得します。
            if (loginModel.AvailableCookieContainer == null)
            {
                var cc = Loginer.LoginWithBrowser(
                    loginModel.BrowserValue.Key,
                    false);

                ValidateCookie(loginModel, cc);
            }
            else
            {
                ValidateCookie(loginModel, loginModel.AvailableCookieContainer);
            }
        }
    }
}
