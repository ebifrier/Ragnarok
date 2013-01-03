using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
    /// <summary>
    /// Firefox用のCookieGetterを作成するクラスです。
    /// </summary>
	internal class FirefoxBrowserManager : IBrowserManager
	{
		const string DATAFOLDER = "%APPDATA%\\Mozilla\\Firefox\\";
		const string INIFILE_NAME = "profiles.ini";
		const string COOKEFILE_NAME = "cookies.sqlite";

        /// <summary>
        /// ブラウザの種別を取得します。
        /// </summary>
		public BrowserType BrowserType
		{
			get { return BrowserType.Firefox; }
		}

		public ICookieGetter CreateDefaultCookieGetter()
		{
			FirefoxProfile prof = FirefoxProfile.GetDefaultProfile(
                CookieUtil.ReplacePathSymbols(DATAFOLDER),
                INIFILE_NAME);

			return CreateCookieGetter(prof);
		}

        public ICookieGetter[] CreateCookieGetters()
        {
            FirefoxProfile[] profs = FirefoxProfile.GetProfiles(
                CookieUtil.ReplacePathSymbols(DATAFOLDER),
                INIFILE_NAME);

            if (profs.Length == 0)
            {
                return new ICookieGetter[] { CreateCookieGetter(null) };
            }

            ICookieGetter[] cgs = new ICookieGetter[profs.Length];
            for (int i = 0; i < profs.Length; i++)
            {
                cgs[i] = CreateCookieGetter(profs[i]);
            }

            return cgs;
        }

        /// <summary>
        /// 指定のfirefoxプロファイルからクッキーを取得します。
        /// </summary>
        private ICookieGetter CreateCookieGetter(FirefoxProfile prof)
        {
            string name = "Firefox";
            string path = null;

            if (prof != null)
            {
                name += " " + prof.Name;
                path = Path.Combine(prof.FilePath, COOKEFILE_NAME);
            }

            CookieStatus status = new CookieStatus(
                name, path, BrowserType, PathType.File);
            return new FirefoxCookieGetter(status);
        }
	}
}
