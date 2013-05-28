using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

using Ragnarok;
using Ragnarok.Net;

namespace Ragnarok.NicoNico
{
    /// <summary>
    /// アカウント情報を保持します。
    /// </summary>
    public class AccountInfo
    {
        /// <summary>
        /// ユーザーＩＤを取得または設定します。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ニックネームを取得または設定します。
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 性別を取得または設定します。
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// 誕生日を取得または設定します。
        /// </summary>
        public DateTime? BirthDay { get; set; }

        /// <summary>
        /// 年齢を取得または設定します。
        /// </summary>
        public int? Age
        {
            get
            {
                if (BirthDay == null)
                {
                    return null;
                }

                var birthDay = BirthDay.Value;
                var today = DateTime.Now;

                var tmp = new DateTime(birthDay.Year, 1, 1);
                tmp.AddDays(today.DayOfYear - 1);
                return (DateTime.Now.Year - birthDay.Year) + (birthDay <= tmp ? 0 : 1);
            }
        }

        /// <summary>
        /// 地域を取得または設定します。
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// プレミアム会員かどうかを取得または設定します。
        /// </summary>
        public bool IsPremium { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AccountInfo()
        {
        }

        /// <summary>
        /// クッキーから、ニコニコのユーザーＩＤを取得します。
        /// </summary>
        public static int GetUserIdFromCookie(CookieContainer cc)
        {
            if (cc == null)
            {
                return -1;
            }

            var cookieCollection = cc.GetCookies(
                new Uri(NicoString.GetLiveTopUrl()));
            if (cookieCollection == null)
            {
                return -1;
            }

            var cookie = cookieCollection["user_session"];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return -1;
            }

            var m = Regex.Match(
                cookie.Value,
                "^user_session_([0-9]+)_([0-9]+)$");
            if (!m.Success)
            {
                return -1;
            }

            return int.Parse(m.Groups[1].Value);
        }

        /// <summary>
        /// クッキーからそのユーザーのアカウント情報を取得します。
        /// </summary>
        /// <remarks>
        /// 失敗時には例外が投げられます。
        /// </remarks>
        public static AccountInfo Create(CookieContainer cc)
        {
            var id = GetUserIdFromCookie(cc);
            if (id < 0)
            {
                throw new NicoException(
                    "ユーザーIDの取得に失敗しました。");
            }

            return Create(id, cc);
        }

        /// <summary>
        /// 与えられたユーザーのアカウント情報を取得します。
        /// </summary>
        public static AccountInfo Create(int id, CookieContainer cc)
        {
            var url = NicoString.GetUserInfoUrl(id);
            var text = WebUtil.RequestHttpText(url, null, cc, Encoding.UTF8);

            if (string.IsNullOrEmpty(text))
            {
                throw new NicoException(
                    "ユーザーページの取得に失敗しました。");
            }

            return Create(id, text);
        }

        /// <summary>
        /// 与えられたユーザーのアカウント情報を取得します。
        /// </summary>
        public static AccountInfo Create(int id, string userPage)
        {
            if (string.IsNullOrEmpty(userPage))
            {
                throw new ArgumentException(
                    "与えられたページは正しくありません。");
            }

            if (userPage.IndexOf("Qバージョンに変更") >= 0)
            {
                return GetAccountHarajuku(id, userPage);
            }
            else
            {
                return GetAccountQ(id, userPage);
            }
        }

        /// <summary>
        /// ニコニコ動画(Q)バージョンのアカウント情報を取得します。
        /// </summary>
        private static AccountInfo GetAccountQ(int id, string userPage)
        {
            var result = new AccountInfo()
            {
                Id = id,
            };

            /*if (userPage.IndexOf("短時間での連続アクセスはご遠慮ください") >= 0)
            {
                throw new NicoException(
                    "連続アクセスエラーです。");
            }*/

            // ニックネームを取得します。
            var m = Regex.Match(
                userPage,
                "<span id=\"siteHeaderUserNickNameContainer\">([^\n]+) さん</span>");
            if (!m.Success)
            {
                throw new NicoException(
                    "ニックネームの取得に失敗しました。");
            }

            result.NickName = m.Groups[1].Value;

            // プレミアム会員かどうか調べます。
            m = Regex.Match(
                userPage,
                "<p class=\"accountNumber\">ID:<span>\\d+([(][^)]+[)])? ([^\n]+)会員</span></p>");
            if (!m.Success)
            {
                throw new NicoException(
                    "プレミアム会員かどうかの取得に失敗しました。");
            }

            result.IsPremium = (m.Groups[2].Value == "プレミアム");

            // 性別を調べます。
            m = Regex.Match(
                userPage,
                "<p>性別:<span>([^\n]*)</span></p>");
            if (!m.Success)
            {
                throw new NicoException(
                    "性別の取得に失敗しました。");
            }

            if (m.Groups[1].Value == "男性")
            {
                result.Gender = Gender.Male;
            }
            else if (m.Groups[1].Value == "女性")
            {
                result.Gender = Gender.Female;
            }
            else
            {
                result.Gender = Gender.Unknown;
            }

            // 生年月日を調べます。

            // 住んでいる地域を調べます。
            m = Regex.Match(
                userPage,
                "<p>お住まいの地域:<span>([^\n]*)</span></p>");
            if (!m.Success)
            {
                throw new NicoException(
                    "住んでいる地域の取得に失敗しました。");
            }

            result.Place =
                (m.Groups[1].Value != "非公開"
                     ? m.Groups[1].Value
                     : null);

            return result;
        }

        /// <summary>
        /// ニコニコ動画(原宿)バージョンのアカウント情報を取得します。
        /// </summary>
        private static AccountInfo GetAccountHarajuku(int id, string userPage)
        {
            var result = new AccountInfo()
            {
                Id = id,
            };

            if (userPage.IndexOf("短時間での連続アクセスはご遠慮ください") >= 0)
            {
                throw new NicoException(
                    "連続アクセスエラーです。");
            }

            // ニックネームを取得します。
            var m = Regex.Match(
                userPage,
                "<div id=\"headingUser\" class=\"cmnHeading\">" +
                "\\s*<h2><strong>([^\n]+)</strong>さん</h2>");
            if (!m.Success)
            {
                throw new NicoException(
                    "ニックネームの取得に失敗しました。");
            }

            result.NickName = m.Groups[1].Value;

            // プレミアム会員かどうか調べます。
            m = Regex.Match(
                userPage,
                "<dt>ID…</dt><dd>[^\n]+ ([^\n]+)会員</dd>");
            if (!m.Success)
            {
                throw new NicoException(
                    "プレミアム会員かどうかの取得に失敗しました。");
            }

            result.IsPremium = (m.Groups[1].Value == "プレミアム");

            // 性別を調べます。
            m = Regex.Match(
                userPage,
                "<dt>性別…</dt><dd>([^<]+)</dd>");
            if (!m.Success)
            {
                throw new NicoException(
                    "性別の取得に失敗しました。");
            }

            if (m.Groups[1].Value == "男性")
            {
                result.Gender = Gender.Male;
            }
            else if (m.Groups[1].Value == "女性")
            {
                result.Gender = Gender.Female;
            }
            else
            {
                result.Gender = Gender.Unknown;
            }

            // 生年月日を調べます。

            // 住んでいる地域を調べます。
            m = Regex.Match(
                userPage,
                "<dt>お住いの(地域|国)…</dt><dd class=\"last\">([^\n]+)</dd>");
            if (!m.Success)
            {
                throw new NicoException(
                    "住んでいる国の取得に失敗しました。");
            }

            result.Place =
                (m.Groups[1].Value != "非公開"
                     ? m.Groups[1].Value
                     : null);

            return result;
        }
    }
}
