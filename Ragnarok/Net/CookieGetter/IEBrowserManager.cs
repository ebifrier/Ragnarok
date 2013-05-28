using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
	/// <summary>
	/// IE系のすべてのクッキーを取得する
	/// </summary>
	internal class IEBrowserManager : IBrowserManager
	{
		public BrowserType BrowserType
		{
			get { return BrowserType.IE; }
		}

		public ICookieGetter CreateDefaultCookieGetter()
		{
			string cookieFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.Cookies);

			CookieStatus status = new CookieStatus(
                this.BrowserType.ToString(),
                cookieFolder,
                this.BrowserType,
                PathType.Directory);

			return new IECookieGetter(status, true);
		}

        public ICookieGetter[] CreateCookieGetters()
        {
            string cookieFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.Cookies);
            string lowFolder = Path.Combine(cookieFolder, "low");

            if (Directory.Exists(lowFolder))
            {
                IEComponentBrowserManager iec = new IEComponentBrowserManager();
                IESafemodeBrowserManager ies = new IESafemodeBrowserManager();
                return new ICookieGetter[]
                {
                    iec.CreateDefaultCookieGetter(),
                    ies.CreateDefaultCookieGetter(),
                };
            }
            else
            {
                return new ICookieGetter[]
                {
                    CreateDefaultCookieGetter()
                };
            }
        }
	}
}
