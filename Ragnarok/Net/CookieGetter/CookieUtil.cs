using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Net.CookieGetter
{
    public static class CookieUtil
    {
        /// <summary>
        /// Unix時間をDateTimeに変換する
        /// </summary>
        /// <param name="UnixTime"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToDateTime(int UnixTime)
        {
            return new DateTime(1970, 1, 1, 9, 0, 0).AddSeconds(UnixTime);
        }

        /// <summary>
        /// DateTimeをUnix時間に変換する
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int DateTimeToUnixTime(DateTime time)
        {
            TimeSpan t = time.Subtract(new DateTime(1970, 1, 1, 9, 0, 0));
            return (int)t.TotalSeconds;
        }

        /// <summary>
        /// %APPDATA%などを実際のパスに変換する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReplacePathSymbols(string path)
        {
            path = path.Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            path = path.Replace("%LOCALAPPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            path = path.Replace("%COOKIES%", Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
            return path;
        }

        /// <summary>
        /// 必要があればuriの最後に/をつける
        /// Pathの指定がある場合、uriの最後に/があるかないかで取得できない場合があるので
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Uri AddSrashLast(Uri uri)
        {
            string o = uri.Segments[uri.Segments.Length - 1];
            string no = uri.OriginalString;//.Replace("http://", "http://o.");
            if (!o.Contains(".") && o[o.Length - 1] != '/')
            {
                no += "/";
            }

            return new Uri(no);
        }

        /// <summary>
        /// クッキーコンテナにクッキーを追加する
        /// domainが.hal.fscs.jpなどだと http://hal.fscs.jp でクッキーが有効にならないので.ありとなし両方指定する
        /// </summary>
        public static void AddCookieToContainer(System.Net.CookieContainer container, System.Net.Cookie cookie)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (cookie == null)
            {
                throw new ArgumentNullException("cookie");
            }

            container.Add(cookie);
            if (cookie.Domain.StartsWith("."))
            {
                container.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain.Substring(1)));
            }
        }

        /// <summary>
        /// SQLクエリのwhere区を作成します。
        /// </summary>
        /// <remarks>
        /// <paramref name="url"/> = http://www.niconico.jp/ なら、
        /// WHERE ( host = ".niconico.jp" OR host LIKE "%www.niconico.jp" )
        /// という文字列を作成します。
        /// </remarks>
        public static string MakeWhere(Uri url, string hostKey)
        {
            string[] hostParts = url.Host.Split('.');
            List<string> hostList = new List<string>();
            string host = "";

            for (int i = hostParts.Length - 1; i >= 0; --i)
            {
                host = (i == 0 ? "%" : ".") + hostParts[i] + host;

                hostList.Add(string.Format(
                    "{0} {1} \"{2}\"",
                    hostKey, (i == 0 ? "LIKE" : "="), host));
            }

            return string.Format(
                "WHERE ({0})",
                string.Join(" OR ", hostList.Skip(1).ToArray()));
        }
    }
}
