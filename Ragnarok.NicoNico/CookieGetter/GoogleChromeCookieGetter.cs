using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Security.Cryptography;

using Ragnarok.Utility;

namespace Ragnarok.CookieGetter
{
	/// <summary>
	/// GoogleChromeからクッキーを取得する
	/// </summary>
	internal sealed class GoogleChromeCookieGetter : CookieGetter
	{
		private const string SELECT_QUERY =
            "SELECT value, name, host_key, path, expires_utc, encrypted_value FROM cookies";

		public GoogleChromeCookieGetter(CookieStatus status)
            : base(status)
		{
		}

        private string MakeQuery(Uri url, string key)
        {
            return string.Format(
                "{0} {1} AND name = \"{2}\" ORDER BY creation_utc DESC",
                SELECT_QUERY, CookieUtil.MakeWhere(url, "host_key"), key);
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
                "{0} {1} ORDER BY creation_utc DESC",
                SELECT_QUERY, CookieUtil.MakeWhere(url, "host_key"));
        }

        public override CookieCollection GetCookieCollection(Uri url)
        {
            CookieContainer container = GetCookies(MakeQuery(url));
            return container.GetCookies(CookieUtil.AddSrashLast(url));
        }

        private string MakeQuery()
        {
            return string.Format(
                "{0} ORDER BY creation_utc DESC",
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
            cookie.Name = data[1] as string;
            cookie.Domain = data[2] as string;
            cookie.Path = data[3] as string;

            var value = data[0] as string;
            var encryptedValue = (data.Count > 5 ? data[5] : null);
            cookie.Value = GetValue(value, encryptedValue as byte[]);

            try
            {
                long exp = long.Parse(data[4].ToString());

                // クッキー有効期限が正確に取得されていなかったので修正
                cookie.Expires = TimeUtil.UnixTimeToDateTime(
                    ((exp / 1000000L) - 11644473600L));
            }
            catch (Exception ex)
            {
                throw new CookieGetterException(
                    "Chromeのexpires変換に失敗しました", ex);
            }

            return cookie;
        }

        /// <summary>
        /// クッキー値の複合化などを行います。
        /// </summary>
        private string GetValue(string value, byte[] encryptedValue)
        {
            if (encryptedValue != null)
            {
                try
                {
                    var newValue = ProtectedData.Unprotect(
                        encryptedValue, null,
                        DataProtectionScope.CurrentUser);
                    if (newValue != null)
                    {
                        value = Encoding.UTF8.GetString(newValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "クッキー値の復号化に失敗しました。");
                }
            }

            if (value != null)
            {
                value = Uri.EscapeDataString(value);
            }

            return value;
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
