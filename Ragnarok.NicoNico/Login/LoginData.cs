using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;

using Ragnarok.CookieGetter;
using Ragnarok.ObjectModel;

namespace Ragnarok.NicoNico.Login
{
    /// <summary>
    /// ログイン方法を判別します。
    /// </summary>
    public enum LoginMethod
    {
        /// <summary>
        /// 直接ログインします。
        /// </summary>
        Direct,
        /// <summary>
        /// 指定のブラウザとクッキーを共有します。
        /// </summary>
        WithBrowser,
        /// <summary>
        /// 使用可能なクッキーリストから選びます。
        /// </summary>
        AvailableCookie,
    }

    /// <summary>
    /// ログイン時に使う保存/復元可能なデータです。
    /// </summary>
    [DataContract()]
    public sealed class LoginData : NotifyObject, IEquatable<LoginData>
    {
        /// <summary>
        /// このオブジェクトの簡易コピーを作成します。
        /// </summary>
        public LoginData Clone()
        {
            // MemberwiseCloneはイベントの内容もコピーするため使えません。
            return new LoginData()
            {
                LoginMethod = LoginMethod,
                Mail = Mail,
                Password = Password,
                BrowserType = BrowserType,
            };
        }

        /// <summary>
        /// ログイン手段を取得または設定します。
        /// </summary>
        [DataMember(Order = 1)]
        public LoginMethod LoginMethod
        {
            get { return GetValue<LoginMethod>("LoginMethod"); }
            set { SetValue("LoginMethod", value); }
        }

        /// <summary>
        /// 直接ログイン時に使うメールアドレスを取得または設定します。
        /// </summary>
        [DataMember(Order = 2)]
        public string Mail
        {
            get { return GetValue<string>("Mail"); }
            set { SetValue("Mail", value); }
        }

        /// <summary>
        /// 直接ログイン時に使うパスワードを取得または設定します。
        /// </summary>
        [DataMember(Order = 3)]
        public string Password
        {
            get { return GetValue<string>("Password"); }
            set { SetValue("Password", value); }
        }

        /// <summary>
        /// ブラウザからのログイン時に使うブラウザを取得または設定します。
        /// </summary>
        [DataMember(Order = 4)]
        public BrowserType BrowserType
        {
            get { return GetValue<BrowserType>("BrowserType"); }
            set { SetValue("BrowserType", value); }
        }

        /// <summary>
        /// 値の等値性を判定します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }
            
            return Equals(obj as LoginData);
        }

        /// <summary>
        /// 値の等値性を判定します。
        /// </summary>
        public bool Equals(LoginData other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (LoginMethod != other.LoginMethod)
            {
                return false;
            }

            if (Mail != other.Mail ||
                Password != other.Password)
            {
                return false;
            }

            if (BrowserType != other.BrowserType)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator ==(LoginData x, LoginData y)
        {
            return Util.GenericEquals(x, y);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(LoginData x, LoginData y)
        {
            return !(x == y);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                LoginMethod.GetHashCode() ^
                (Mail != null ? Mail.GetHashCode() : 0) ^
                (Password != null ? Password.GetHashCode() : 0) ^
                BrowserType.GetHashCode());
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginData()
        {
            LoginMethod = LoginMethod.Direct;
            BrowserType = BrowserType.IESafemode;
        }
    }
}
