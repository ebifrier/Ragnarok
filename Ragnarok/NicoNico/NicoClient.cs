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
    /// ログイン状態です。
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
    public class NicoClient : MarshalByRefObject, ILazyModel
    {
        private readonly object SyncRoot = new object();
        private readonly LazyModelObject lazyModelObject = new LazyModelObject();
        private LoginState loginState = LoginState.Waiting;
        private LoginData loginData = new LoginData();
        private CookieContainer cookieContainer = null;
        private AccountInfo accountInfo = null;

        /// <summary>
        /// プロパティ値の変更イベントです。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// このデータを使ったログイン成功時のイベントです。
        /// </summary>
        public event EventHandler LoginSucceeded;

        /// <summary>
        /// プロパティ値が変更されたときに呼ばれます。
        /// </summary>
        public virtual void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, e));
            }
        }

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
        /// プロパティ変更通知を遅延するためのオブジェクトを取得します。
        /// </summary>
        LazyModelObject ILazyModel.LazyModelObject
        {
            get
            {
                return this.lazyModelObject;
            }
        }

        /// <summary>
        /// ログイン用のデータを取得します。
        /// </summary>
        public LoginData LoginData
        {
            get
            {
                return this.loginData;
            }
            private set
            {
                using (new LazyModelLock(this, SyncRoot))
                {
                    this.loginData = value;

                    this.RaisePropertyChanged("LoginData");
                }
            }
        }

        /// <summary>
        /// 非同期ログインの状態を取得します。
        /// </summary>
        public LoginState LoginState
        {
            get
            {
                return this.loginState;
            }
            private set
            {
                using (new LazyModelLock(this, SyncRoot))
                {
                    if (this.loginState != value)
                    {
                        this.loginState = value;

                        this.RaisePropertyChanged("LoginState");
                    }
                }
            }
        }

        /// <summary>
        /// ニコニコへのログイン用クッキーを取得または設定します。
        /// </summary>
        public CookieContainer CookieContainer
        {
            get
            {
                return this.cookieContainer;
            }
            protected set
            {
                using (new LazyModelLock(this, SyncRoot))
                {
                    if (this.cookieContainer != value)
                    {
                        this.cookieContainer = value;

                        this.RaisePropertyChanged("CookieContainer");
                    }
                }
            }
        }

        /// <summary>
        /// ニコニコにログインしているかどうかを取得します。
        /// </summary>
        [DependOnProperty("CookieContainer")]
        public bool IsLogin
        {
            get
            {
                using (new LazyModelLock(this, SyncRoot))
                {
                    return (this.cookieContainer != null);
                }
            }
        } 

        /// <summary>
        /// オーナーのアカウント情報を取得します。
        /// </summary>
        public AccountInfo AccountInfo
        {
            get
            {
                return this.accountInfo;
            }
            protected set
            {
                using (new LazyModelLock(this, SyncRoot))
                {
                    if (this.accountInfo != value)
                    {
                        this.accountInfo = value;

                        this.RaisePropertyChanged("AccountInfo");
                    }
                }
            }
        }

        /// <summary>
        /// ログインしているユーザーＩＤを取得します。
        /// </summary>
        [DependOnProperty("AccountInfo")]
        public int LoginId
        {
            get
            {
                using (new LazyModelLock(this, SyncRoot))
                {
                    if (this.accountInfo == null)
                    {
                        return -1;
                    }

                    return this.accountInfo.Id;
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
                using (new LazyModelLock(this, SyncRoot))
                {
                    if (this.accountInfo == null)
                    {
                        return null;
                    }

                    return this.accountInfo.NickName;
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
            var accountInfo = NicoNico.Login.CookieValidator.Validate(
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
                    if (this.LoginState == LoginState.Logining)
                    {
                        return;
                    }

                    this.LoginState = LoginState.Logining;

                    // ログインの非同期処理を開始します。
                    ThreadPool.QueueUserWorkItem(
                        LoginAsync,
                        loginData);
                }
                catch (Exception)
                {
                    this.LoginState =
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
            this.LoginState =
                 (IsLogin ? LoginState.Logined : LoginState.Waiting);
        }
    }
}
