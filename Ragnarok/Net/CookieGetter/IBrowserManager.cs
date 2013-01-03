using System;
using System.Collections.Generic;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
	/// <summary>
	/// CookieGetterを生成するためのインターフェース
	/// </summary>
	public interface IBrowserManager
	{
		/// <summary>
		/// ブラウザの種類
		/// </summary>
		BrowserType BrowserType { get; }

		/// <summary>
		/// 既定のCookieGetterを取得します
		/// </summary>
		ICookieGetter CreateDefaultCookieGetter();

		/// <summary>
		/// 利用可能なすべてのCookieGetterを取得します
		/// </summary>
		ICookieGetter[] CreateCookieGetters();
	}
}
