﻿using System;
using System.Collections.Generic;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.CookieGetter
{
	/// <summary>
	/// ブラウザの種類
	/// </summary>
	public enum BrowserType
	{
		/// <summary>
		/// IE系ブラウザ(IEComponent + IESafemode)
		/// </summary>
		IE,

		/// <summary>
		/// XPのIEやトライデントエンジンを使用しているブラウザ
		/// </summary>
		IEComponent,

		/// <summary>
		/// Vista以降のIE
		/// </summary>
		IESafemode,

		/*/// <summary>
		/// Vista以降のIE - ファイルの中身からホスト名取得
		/// </summary>
		IElSafemode,*/

		/// <summary>
		/// Firefox
		/// </summary>
		Firefox,

		/*/// <summary>
		/// PaleMoon
		/// </summary>
		PaleMoon,

		/// <summary>
		/// Songbird
		/// </summary>
		Songbird,

		/// <summary>
		/// SeaMonkey
		/// </summary>
		SeaMonkey,*/

        /// <summary>
		/// Google Chrome
		/// </summary>
		GoogleChrome,

		/*/// <summary>
		/// Comodo Dragon
		/// </summary>
		ComodoDragon,

        /// <summary>
        /// Chrome Plus
        /// </summary>
        ChromePlus,*/
        
        /// <summary>
		/// Opera
		/// </summary>
		Opera,

        /// <summary>
		/// Safari
		/// </summary>
		Safari,

		/*/// <summary>
		/// Lunascape6 Geckoエンジン
		/// </summary>
		LunascapeGecko,

		/// <summary>
		/// Lunascape6 Webkitエンジン
		/// </summary>
		LunascapeWebkit,
		
		/// <summary>
		/// RockMelt
		/// </summary>
		RockMelt,

		/// <summary>
		/// Maxthon
		/// </summary>
		Maxthon,*/

		/// <summary>
		/// Chromium
		/// </summary>
		Chromium
	}
}
