using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.ComponentModel;

using Ragnarok.ObjectModel;

namespace Ragnarok.NicoNico
{
    using Login;

    /// <summary>
    /// ログイン状態を示します。
    /// </summary>
    public enum LoginState
    {
        /// <summary>
        /// 待機中です。
        /// </summary>
        Waiting,
        /// <summary>
        /// ログイン中です。
        /// </summary>
        Logining,
        /// <summary>
        /// ログインしています。
        /// </summary>
        Logined,
    }

    /// <summary>
    /// ニコニコのログイン情報を管理します。
    /// </summary>
    public class NicoClient : NotifyObject
    {
        /// <summary>
        /// このデータを使ったログイン成功時のイベントです。
        /// </summary>
        public event EventHandler LoginSucceeded;

        /// <summary>
        /// ログイン成功を通知します。
        /// </summary>
        protected virtual void NotifyLoginSucceeded()
        {
            var handler = this.LoginSucceeded;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, new EventArgs()));
            }
        }

        /// <summary>
        /// ログイン用のデータを取得します。
        /// </summary>
        public LoginData LoginData
        {
            get { return GetValue<LoginData>(nameof(LoginData)); }
            private set { SetValue(nameof(LoginData), value); }
        }

        /// <summary>
        /// 非同期ログインの状態を取得します。
        /// </summary>
        public LoginState LoginState
        {
            get { return GetValue<LoginState>(nameof(LoginState)); }
            private set { SetValue(nameof(LoginState), value); }
        }

        /// <summary>
        /// ニコニコへのログイン用クッキーを取得または設定します。
        /// </summary>
        public CookieContainer CookieContainer
        {
            get { return GetValue<CookieContainer>(nameof(CookieContainer)); }
            private set { SetValue(nameof(CookieContainer), value); }
        }

        /// <summary>
        /// ニコニコにログインしているかどうかを取得します。
        /// </summary>
        [DependOnProperty("CookieContainer")]
        public bool IsLogin
        {
            get
            {
                using (LazyLock())
                {
                    return (CookieContainer != null);
                }
            }
        } 

        /// <summary>
        /// オーナーのアカウント情報を取得します。
        /// </summary>
        public AccountInfo AccountInfo
        {
            get { return GetValue<AccountInfo>(nameof(AccountInfo)); }
            private set { SetValue(nameof(AccountInfo), value); }
        }

        /// <summary>
        /// ログインしているユーザーＩＤを取得します。
        /// </summary>
        [DependOnProperty("AccountInfo")]
        public int LoginId
        {
            get
            {
                using (LazyLock())
                {
                    if (AccountInfo == null)
                    {
                        return -1;
                    }

                    return AccountInfo.Id;
                }
            }
        }

        /// <summary>
        /// ログイン名を取得します。
        /// </summary>
        [DependOnProperty("AccountInfo")]
        public string LoginName
        {
            get
            {
                using (LazyLock())
                {
                    if (AccountInfo == null)
                    {
                        return null;
                    }

                    return AccountInfo.NickName;
                }
            }
        }

        /// <summary>
        /// ニコニコへログイン済みのクッキーを設定します。
        /// </summary>
        /// <param name="cookieContainer">
        /// クッキーデータを指定します。
        /// </param>
        /// <param name="loginData">
        /// 指定のクッキーが指定のデータから取得したことが分かっている場合は
        /// 指定してください。nullでもかまいません。
        /// </param>
        public bool Login(CookieContainer cookieContainer,
                          LoginData loginData = null)
        {
            if (cookieContainer == null)
            {
                return false;
            }

            // アカウント情報を取得します。
            // 取得できなければ、このクッキーはログインされていません。
            var accountInfo = CookieValidator.Validate(
                cookieContainer);
            if (accountInfo == null)
            {
                return false;
            }

            // ロックせずに設定します。
            CookieContainer = cookieContainer;
            AccountInfo = accountInfo;
            LoginData = loginData;

            NotifyLoginSucceeded();
            return true;
        }

        /// <summary>
        /// ニコニコへログインします。
        /// </summary>
        public bool Login(LoginData loginData)
        {
            if (loginData == null)
            {
                return false;
            }

            // 得られたクッキーでログインを試みます。
            var cc = Loginer.LoginFromData(loginData, false);
            if (cc == null)
            {
                return false;
            }

            return Login(cc, loginData);
        } 

        /// <summary>
        /// 非同期的にログインを行います。
        /// </summary>
        public void LoginAsync(LoginData loginData)
        {
            if (loginData == null)
            {
                return;
            }

            using (new LazyModelLock(this, SyncRoot))
            {
                try
                {
                    if (LoginState == LoginState.Logining)
                    {
                        return;
                    }

                    LoginState = LoginState.Logining;

                    // ログインの非同期処理を開始します。
                    ThreadPool.QueueUserWorkItem(
                        LoginAsync,
                        loginData);
                }
                catch (Exception)
                {
                    LoginState =
                        (IsLogin ? LoginState.Logined : LoginState.Waiting);
                }
            }
        }

        /// <summary>
        /// 非同期的なログイン処理を行います。
        /// </summary>
        private void LoginAsync(object state)
        {
            try
            {
                var loginData = (LoginData)state;

                Login(loginData);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "非同期ログインに失敗しました。");
            }

            // ログイン処理終了を通知します。
            LoginState =
                 (IsLogin ? LoginState.Logined : LoginState.Waiting);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NicoClient()
        {
            LoginState = LoginState.Waiting;
            LoginData = new LoginData();
        }
    }
}
