﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ragnarok.CookieGetter
{
	/// <summary>
	/// IEコンポーネントでアクセス可能なクッキーのみを取得する
	/// </summary>
	class IEComponentBrowserManager : IBrowserManager
	{
		public BrowserType BrowserType
		{
			get { return BrowserType.IEComponent; }
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
