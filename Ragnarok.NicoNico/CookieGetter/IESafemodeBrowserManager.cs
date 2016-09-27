﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ragnarok.CookieGetter
{

	/// <summary>
	/// IEのクッキーのうちVista以降の保護モードで使われるクッキーのみを取得する
	/// </summary>
	class IESafemodeBrowserManager : IBrowserManager
	{
		public BrowserType BrowserType
		{
			get { return BrowserType.IESafemode; }
		}

		public ICookieGetter CreateDefaultCookieGetter()
		{
			string cookieFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.Cookies);
			string lowFolder = Path.Combine(cookieFolder, "low");

			CookieStatus status = new CookieStatus(
                this.BrowserType.ToString(),
                lowFolder,
                this.BrowserType,
                PathType.Directory);
			return new IECookieGetter(status, false);
		}

		/// <summary>
		/// IEBrowserManagerで環境にあわせて適切な物を返すようにしてあるので、ここでは何もしない
		/// </summary>
		public ICookieGetter[] CreateCookieGetters()
		{
			return new ICookieGetter[0];
		}
	}
}
