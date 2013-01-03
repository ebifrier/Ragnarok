using System;
using System.Collections.Generic;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
	class SafariBrowserManager : IBrowserManager
	{
		const string COOKIEPATH = "%APPDATA%\\Apple Computer\\Safari\\Cookies\\Cookies.plist";
		const string COOKIEPATH51 = "%APPDATA%\\Apple Computer\\Safari\\Cookies\\Cookies.binarycookies";

		#region IBrowserManager メンバ

		public BrowserType BrowserType
		{
			get { return BrowserType.Safari; }
		}

		public ICookieGetter CreateDefaultCookieGetter()
		{
			string path = CookieUtil.ReplacePathSymbols(COOKIEPATH);
			if(!System.IO.File.Exists(path)) {
				path = CookieUtil.ReplacePathSymbols(COOKIEPATH51);
				if(!System.IO.File.Exists(path)) {
					path = null;
				}
			}

			CookieStatus status = new CookieStatus(this.BrowserType.ToString(), path, this.BrowserType, PathType.File);
			return new SafariCookieGetter(status);
		}

		public ICookieGetter[] CreateCookieGetters()
		{
			return new ICookieGetter[] { CreateDefaultCookieGetter() };
		}

		#endregion
	}
}
