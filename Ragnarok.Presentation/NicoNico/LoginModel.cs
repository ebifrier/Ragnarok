using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;

using Ragnarok;
using Ragnarok.ObjectModel;
using Ragnarok.NicoNico;

namespace Ragnarok.Presentation.NicoNico
{
    using Ragnarok.NicoNico.Login;

    /// <summary>
    /// LoginWindowが使うモデルデータです。
    /// </summary>
    internal class LoginModel : DynamicViewModel
    {
        private static readonly Dictionary<BrowserType, string> BrowserDic =
            new Dictionary<BrowserType, string>()
            {
                {BrowserType.IEComponent, "IEComponent (Windows XPのIEやトライデントエンジンを使っているIE)"},
                {BrowserType.IESafemode, "IESafemode (Windows Vista以降のIE)"},
                {BrowserType.Firefox, "Firefox"},
                {BrowserType.GoogleChrome, "Google Chrome"},
                {BrowserType.Opera, "Opera"},
                {BrowserType.Safari, "Safari"},
                {BrowserType.Chromium, "Chromium"},
            };

        private LoginData loginData;
        private readonly ObservableCollection<CookieData> availableCookieList =
            new ObservableCollection<CookieData>();
        private CookieData availableCookieData = null;

        /// <summary>
        /// ログイン用データを取得します。
        /// </summary>
        public LoginData Data
        {
            get
            {
                return this.loginData;
            }
            set
            {
                using (LazyLock())
                {
                    this.RemoveDependModel(this.loginData);
                    this.loginData = value;
                    this.AddDependModel(this.loginData);

                    this.RaisePropertyChanged("Data");
                }
            }
        }

        /// <summary>
        /// ブラウザのリストを取得します。
        /// </summary>
        public static Dictionary<BrowserType, string> BrowserList
        {
            get { return BrowserDic; }
        }

        /// <summary>
        /// ログインに使われるブラウザを取得または設定します。
        /// </summary>
        [DependOnProperty(typeof(LoginData), "BrowserType")]
        public KeyValuePair<BrowserType, string> BrowserValue
        {
            get
            {
                using (LazyLock())
                {
                    return new KeyValuePair<BrowserType, string>(
                        this.loginData.BrowserType,
                        BrowserDic[this.loginData.BrowserType]);
                }
            }
            set
            {
                using (LazyLock())
                {
                    this.loginData.BrowserType = value.Key;
                }
            }
        }

        /// <summary>
        /// 使用可能なクッキーのリストを取得します。
        /// </summary>
        [DependOnProperty(typeof(LoginData), "LoginMethod")]
        public ObservableCollection<CookieData> AvailableCookieList
        {
            get
            {
                return this.availableCookieList;
            }
        }

        /// <summary>
        /// 使用可能なクッキーリストから選ばれたクッキーを取得または設定します。
        /// </summary>
        public CookieData AvailableCookieData
        {
            get
            {
                return this.availableCookieData;
            }
            set
            {
                using (LazyLock())
                {
                    this.availableCookieData = value;

                    this.RaisePropertyChanged("AvailableCookieData");
                }
            }
        }

        /// <summary>
        /// 使用可能なクッキーのブラウザタイプを取得します。
        /// </summary>
        [DependOnProperty("AvailableCookieData")]
        public BrowserType? AvailableCookieBrowser
        {
            get
            {
                using (LazyLock())
                {
                    if (this.availableCookieData == null)
                    {
                        return null;
                    }

                    return this.availableCookieData.BrowserType;
                }
            }
        }

        /// <summary>
        /// 使用可能なクッキーのクッキーを取得します。
        /// </summary>
        [DependOnProperty("AvailableCookieData")]
        public CookieContainer AvailableCookieContainer
        {
            get
            {
                using (LazyLock())
                {
                    if (this.availableCookieData == null)
                    {
                        return null;
                    }

                    return this.availableCookieData.CookieContainer;
                }
            }
        }

        /// <summary>
        /// 使用可能なクッキーのリストをクリアします。
        /// </summary>
        private void ClearAvailableCookieList()
        {
            // ObservableCollectionを使っているため、
            // 処理はUIスレッド上で行います。
            WPFUtil.UIProcess(() =>
            {
                AvailableCookieData = null;
                this.availableCookieList.Clear();
            });
        }

        /// <summary>
        /// 使用可能なクッキーのリストを更新します。
        /// </summary>
        public void UpdateAvailableCookieList()
        {
            ClearAvailableCookieList();

            ThreadPool.QueueUserWorkItem(
                state => EnumerateAllCookies());
        }

        /// <summary>
        /// 使用可能なクッキーをすべて列挙します。
        /// </summary>
        private void EnumerateAllCookies()
        {
            foreach (var browserType in EnumEx.GetValues<BrowserType>())
            {
                try
                {
                    var cc = Loginer.LoginWithBrowser(
                        browserType,
                        false);

                    GetCookieDone(cc, browserType);
                }
                catch (Exception)
                {
                    // 例外は無視します。
                    //Log.ErrorMessage(ex,
                    //    "使用可能なクッキーの列挙に失敗しました。");
                }
            }
        }

        /// <summary>
        /// クッキー列挙後に呼ばれます。
        /// </summary>
        private void GetCookieDone(CookieContainer cc, BrowserType browserType)
        {
            // 妥当なクッキーでない場合はアカウント情報がnullになります。
            if (cc == null)
            {
                return;
            }

            var cookieData = new CookieData()
            {
                CookieContainer = cc,
                BrowserType = browserType,
            };

            // UserIdは設定されたクッキーから取得します。
            if (cookieData.UserId < 0)
            {
                return;
            }

            WPFUtil.UIProcess(() =>
                this.availableCookieList.Add(cookieData));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginModel()
        {
            this.Data = new LoginData();

            UpdateAvailableCookieList();
        }
    }
}
