using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
    /// <summary>
    /// 指定したブラウザからクッキーを取得する
    /// </summary>
    abstract public class CookieGetter : ICookieGetter, IEquatable<CookieGetter>
    {
        private static IBrowserManager[] browserManagers = new IBrowserManager[]
        {
			new IEBrowserManager(),
			new IEComponentBrowserManager(),
			new IESafemodeBrowserManager(),
		//	new IE9SafemodeBrowserManager(),
			new FirefoxBrowserManager(),
			new GoogleChromeBrowserManager(),
			new SafariBrowserManager(),
        };
        public static Queue<Exception> Exceptions = new Queue<Exception>();

        /// <summary>
        /// 指定したブラウザ用のクッキーゲッターを取得する
        /// </summary>
        public static ICookieGetter CreateInstance(BrowserType type)
        {
            foreach (IBrowserManager manager in browserManagers)
            {
                if (manager.BrowserType == type)
                {
                    return manager.CreateDefaultCookieGetter();
                }
            }

            return null;
        }

        /// <summary>
        /// すべてのクッキーゲッターを取得する
        /// </summary>
        /// <param name="availableOnly">利用可能なものだけを選択するかどうか</param>
        public static ICookieGetter[] CreateInstances(bool availableOnly)
        {
            return (
                from manager in browserManagers
                from cg in manager.CreateCookieGetters()
                where !availableOnly || cg.Status.IsAvailable
                select cg)
                .ToArray();
        }

        /// <summary>
        /// Cookieの状態を取得します。
        /// </summary>
        public CookieStatus Status
        {
            get;
            private set;
        }

        /// <summary>
        /// クッキーが保存されているファイル・ディレクトリへのパスを取得・設定します。
        /// </summary>
        internal string CookiePath
        {
            get { return Status.CookiePath; }
            set { Status.CookiePath = value; }
        }

        /// <summary>
        /// 対象URL上の名前がKeyであるクッキーを取得します。
        /// </summary>
        /// <returns>対象のクッキー。なければnull</returns>
        public virtual Cookie GetCookie(Uri url, string key)
        {
            CookieCollection collection = GetCookieCollection(url);
            return collection[key];
        }

        /// <summary>
        /// urlに関連付けられたクッキーを取得します。
        /// </summary>
        /// <exception cref="CookieGetterException"></exception>
        public virtual CookieCollection GetCookieCollection(Uri url)
        {
            CookieContainer container = GetAllCookies();
            return container.GetCookies(url);
        }

        /// <summary>
        /// すべてのクッキーを取得します。
        /// </summary>
        public abstract CookieContainer GetAllCookies();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal CookieGetter(CookieStatus status)
        {
            if (status == null)
            {
                throw new ArgumentNullException("status");
            }

            Status = status;
        }

        #region オーバーライド
        /// <summary>
        /// 設定の名前を返します。
        /// </summary>
        public override string ToString()
        {
            return Status.ToString();
        }

        /// <summary>
        /// クッキーゲッターを比較して等しいか検査します
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(obj as CookieGetter);
        }

        /// <summary>
        /// クッキーゲッターを比較して等しいか検査します
        /// </summary>
        public bool Equals(CookieGetter obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return Status.Equals(obj.Status);
        }

        /// <summary>
        /// ハッシュ値を計算します
        /// </summary>
        public override int GetHashCode()
        {
            return Status.GetHashCode();
        }
        #endregion
    }
}
