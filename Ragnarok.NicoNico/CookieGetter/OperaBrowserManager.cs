using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ragnarok.CookieGetter
{
	class OperaBrowserManager : IBrowserManager
	{
		private const string COOKIEPATH = "%APPDATA%\\Opera\\Opera\\cookies4.dat";

		public BrowserType BrowserType
		{
			get { return BrowserType.Opera; }
		}

		public ICookieGetter CreateDefaultCookieGetter()
		{
			string path = CookieUtil.ReplacePathSymbols(COOKIEPATH);

			if (!File.Exists(path)) {
				path = null;
			}

			CookieStatus status = new CookieStatus(
                this.BrowserType.ToString(),
                path, 
                this.BrowserType,
                PathType.File);
			return new OperaCookieGetter(status);
		}

		public ICookieGetter[] CreateCookieGetters()
		{
			return new ICookieGetter[] { CreateDefaultCookieGetter() };
		}
	}
}
