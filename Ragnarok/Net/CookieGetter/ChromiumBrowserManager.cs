using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
    internal sealed class ChromiumBrowserManager : IBrowserManager
    {
        private const string COOKIEPATH =
            "%LOCALAPPDATA%\\Chromium\\User Data\\Default\\Cookies";

        public BrowserType BrowserType
        {
            get { return BrowserType.Chromium; }
        }

        public ICookieGetter CreateDefaultCookieGetter()
        {
            string path = CookieUtil.ReplacePathSymbols(COOKIEPATH);

            if (!File.Exists(path))
            {
                path = null;
            }

            CookieStatus status = new CookieStatus(
                BrowserType.ToString(), path, BrowserType, PathType.File);
            return new GoogleChromeCookieGetter(status);
        }

        public ICookieGetter[] CreateCookieGetters()
        {
            return new ICookieGetter[] { CreateDefaultCookieGetter() };
        }
    }
}
