using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Ragnarok.Net;
using Ragnarok.NicoNico;

namespace Ragnarok.NicoNico.Login
{
    /// <summary>
    /// ログインを行うためのメソッドを提供します。
    /// </summary>
    public static class Loginer
    {
        /// <summary>
        /// ニコニコに直接ログインします。
        /// </summary>
        public static CookieContainer DirectLogin(string mail,
                                                  string password)
        {
            if (string.IsNullOrEmpty(mail) ||
                string.IsNullOrEmpty(password))
            {
                Log.Error(
                    "ログイン出来ません。" + Environment.NewLine +
                    "メールアドレスとパスワードを設定してください。");
                return null;
            }

            try
            {
                var cc = new CookieContainer();

                // ニコニコにログインします。
                var text = WebUtil.RequestHttpText(
                    NicoString.LoginUrl(),
                    NicoString.MakeLoginData(mail, password),
                    cc,
                    Encoding.UTF8);

                if (string.IsNullOrEmpty(text) ||
                    text.IndexOf(NicoString.LoginErrorText()) >= 0)
                {
                    Log.Error("ログインに失敗しました。");
                    return null;
                }

                return cc;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "直接ログインに失敗しました。");
            }

            return null;
        }

        /// <summary>
        /// 指定のブラウザのクッキーを使って、ログインします。
        /// </summary>
        public static CookieContainer LoginWithBrowser(BrowserType browser,
                                                       bool validate)
        {
#if !RGN_NOT_USE_COOKIEGETTERSHARP
            try
            {
                var enumType = typeof(Hal.CookieGetterSharp.BrowserType);
                var browserType = (Hal.CookieGetterSharp.BrowserType)browser;
                if (!Enum.IsDefined(enumType, browserType))
                {
                    throw new ArgumentException(
                        "ブラウザの種類が正しくありません。", "browser");
                }

                var cookieGetter = Hal.CookieGetterSharp
                    .CookieGetter.CreateInstance(browserType);
                if (cookieGetter == null)
                {
                    Log.Error(
                        "クッキーの取得オブジェクトの作成に失敗しました。");
                    return null;
                }

                // 与えられたブラウザのログインクッキーを取得します。
                var cookie = cookieGetter.GetCookie(
                    new Uri(NicoString.LiveTopUrl()),
                    "user_session");
                if (cookie == null)
                {
                    return null;
                }

                var cc = new CookieContainer();
                cc.Add(cookie);

                // 本当にこのクッキーでログインできるか確認します。
                if (validate)
                {
                    var account = CookieValidator.Validate(cc);
                    if (account == null)
                    {
                        return null;
                    }
                }

                return cc;
            }
            catch (Exception)
            {
                /*Log.ErrorMessage(ex,
                    "クッキーの取得に失敗しました。");*/
            }
#endif
            return null;
        }

        /// <summary>
        /// ログイン用データからログインを試みます。
        /// </summary>
        public static CookieContainer LoginFromData(LoginData loginData,
                                                    bool validate)
        {
            try
            {
                if (loginData == null)
                {
                    return null;
                }

                // データに沿ったログイン方法でログインします。
                CookieContainer cc = null;
                switch (loginData.LoginMethod)
                {
                    case LoginMethod.Direct:
                        // 直接ログインします。
                        cc = DirectLogin(
                            loginData.Mail,
                            loginData.Password);
                        break;
                    case LoginMethod.WithBrowser:
                    case LoginMethod.AvailableCookie:
                        // ブラウザからログインします。
                        cc = LoginWithBrowser(
                            loginData.BrowserType,
                            validate);
                        break;
                }

                return cc;
            }
            catch (Exception)
            {
                // 例外は無視します。
            }

            return null;
        }
    }
}
