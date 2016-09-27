﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.CookieGetter
{
	/// <summary>
	/// Firefoxからクッキーを取得する
	/// </summary>
	internal class FirefoxCookieGetter : CookieGetter
	{
		private const string SELECT_QUERY =
            "SELECT value, name, host, path, expiry FROM moz_cookies";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FirefoxCookieGetter(CookieStatus status)
            : base(status)
        {
        }
        
        private string MakeQuery(Uri url, string key)
        {
            return string.Format(
                "{0} {1} AND name = \"{2}\" ORDER BY expiry",
                SELECT_QUERY, CookieUtil.MakeWhere(url, "host"), key);
        }
        
        public override Cookie GetCookie(Uri url, string key)
        {
            CookieContainer container = GetCookies(MakeQuery(url, key));
            CookieCollection collection = container.GetCookies(CookieUtil.AddSrashLast(url));
            return collection[key];
        }

        private string MakeQuery(Uri url)
        {
            return string.Format(
                "{0} {1} ORDER BY expiry",
                SELECT_QUERY, CookieUtil.MakeWhere(url, "host"));
        }

        public override CookieCollection GetCookieCollection(Uri url)
        {
            CookieContainer container = GetCookies(MakeQuery(url));
            return container.GetCookies(CookieUtil.AddSrashLast(url));
        }

        private string MakeQuery()
        {
            return string.Format(
                "{0} ORDER BY expiry",
                SELECT_QUERY);
        }

        public override CookieContainer GetAllCookies()
        {
            return GetCookies(MakeQuery());
        }

        /// <summary>
        /// SQLiteのデータからクッキーを復元します。
        /// </summary>
        private Cookie MakeCookie(List<object> data)
        {
            Cookie cookie = new Cookie();
            cookie.Value = data[0] as string;
            cookie.Name = data[1] as string;
            cookie.Domain = data[2] as string;
            cookie.Path = data[3] as string;

            if (cookie.Value != null)
            {
                cookie.Value = Uri.EscapeDataString(cookie.Value);
            }

            try
            {
                cookie.Expires = TimeUtil.UnixTimeToDateTime(data[4].ToString());
            }
            catch (Exception ex)
            {
                throw new CookieGetterException(
                    "Firefoxのexpires変換に失敗しました", ex);
            }

            return cookie;
        }

        /// <summary>
        /// クエリで指定されたクッキーをすべて取得します。
        /// </summary>
        private CookieContainer GetCookies(string query)
        {
            CookieContainer container = new CookieContainer();

            List<List<object>> itemsList = SQLite.GetCookies(CookiePath, query);
            foreach (List<object> items in itemsList)
            {
                Cookie cookie = null;

                try
                {
                    cookie = MakeCookie(items);

                    CookieUtil.AddCookieToContainer(container, cookie);
                }
                catch (Exception ex)
                {
                    Util.ThrowIfFatal(ex);

                    if (cookie == null)
                    {
                        Log.ErrorException(ex,
                            "Invalid Format cookie!");
                    }
                    else
                    {
                        Log.ErrorException(ex,
                            "Invalid Format cookie! domain:{0},key:{1},value:{2}",
                            cookie.Domain, cookie.Name, cookie.Value);
                    }
                }
            }

            return container;
        }
	}
}
