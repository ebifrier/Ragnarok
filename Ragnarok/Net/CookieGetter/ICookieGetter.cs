using System;
using System.Collections.Generic;
using System.Net;

namespace Ragnarok.Net.CookieGetter
{
	/// <summary>
	/// 指定されたパスからブラウザのクッキーを取得するためのインターフェース
	/// </summary>
	public interface ICookieGetter
	{
		/// <summary>
		/// クッキーに関する情報を取得します。
		/// </summary>
		CookieStatus Status { get; }

		/// <summary>
		/// 対象URL上の名前がKeyであるクッキーを取得します。
		/// </summary>
		/// <exception cref="CookieGetterException"></exception>
		/// <returns>対象のクッキー。なければnull</returns>
		Cookie GetCookie(Uri url, string key);

		/// <summary>
		/// urlに関連付けられたクッキーを取得します。
		/// </summary>
		/// <exception cref="CookieGetterException"></exception>
		CookieCollection GetCookieCollection(Uri url);
		
		/// <summary>
		/// すべてのクッキーを取得します。
		/// </summary>
		/// <exception cref="CookieGetterException"></exception>
		CookieContainer GetAllCookies();
	}
}
