using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Threading;

using Ragnarok;
using Ragnarok.Net;

namespace Ragnarok.NicoNico.Login
{
    /// <summary>
    /// ニコニコにログインするためのクッキーが
    /// 正しいものかどうか調べます。
    /// </summary>
    /// <remarks>
    /// 以前はニコニコ動画トップでユーザー名が表示されるか否かによって
    /// ログイン確認をしていました。が、取得したトップページに
    /// 名前が表示されているにもかかわらず、
    /// 実際にはログインできないという現象が何度か確認されました。
    /// 
    /// なので、今はマイページにログインできるかどうかによって
    /// クッキーの確認を行っています。
    /// ただし、マイページは短時間に連続アクセスするとサーバーがはじく
    /// 仕様になっているため、最大でも１秒に１つ確認をしています。
    /// </remarks>
    public static class CookieValidator
    {
        /// <summary>
        /// 内部で使います。
        /// </summary>
        private class InternalData
        {
            public CookieContainer CookieContainer;
            public int UserId;
            public ValidateCallback Callback;
        }

        private static readonly List<InternalData> targetList =
            new List<InternalData>();
        private static readonly Thread validateThread;

        /// <summary>
        /// クッキー確認後に呼ばれるコールバック型です。
        /// </summary>
        public delegate void ValidateCallback(CookieContainer cookieContainer,
                                              AccountInfo account);

        static CookieValidator()
        {
            // 一応変数に入れておきます。
            // ガーベッジされたらたまらないので。
            validateThread = new Thread(ThreadMain)
            {
                Name = "Cookie Validater",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
            };

            validateThread.Start();
        }

        /// <summary>
        /// 内部データを作成します。
        /// </summary>
        private static InternalData MakeData(CookieContainer cc,
                                             ValidateCallback callback)
        {
            var userId = AccountInfo.GetUserIdFromCookie(cc);
            if (userId < 0)
            {
                return null;
            }

            if (callback == null)
            {
                return null;
            }

            return new InternalData()
            {
                CookieContainer = cc,
                UserId = userId,                
                Callback = callback,
            };
        }

        /// <summary>
        /// チェック用データをキューに入れます。
        /// </summary>
        private static void PushData(InternalData data, bool highPriority)
        {
            if (data == null)
            {
                return;
            }

            lock (targetList)
            {
                if (highPriority)
                {
                    targetList.Insert(0, data);
                }
                else
                {
                    targetList.Add(data);
                }

                Monitor.PulseAll(targetList);
            }
        }

        /// <summary>
        /// チェック用データをキューから取り出します。
        /// </summary>
        private static InternalData PopData()
        {
            lock (targetList)
            {
                if (targetList.Count == 0)
                {
                    return null;
                }

                var data = targetList[0];
                targetList.RemoveAt(0);
                return data;
            }
        }

        /// <summary>
        /// 与えられたクッキーでニコニコにログインできるか確かめます。
        /// </summary>
        public static AccountInfo Validate(CookieContainer cookieContainer)
        {
            // ページの取得がタイムアウトすることがあるので、
            // 一応何回か試してみます。
            for (var i = 0; i < 5; ++i)
            {
                try
                {
                    /*var text = WebUtil.RequestHttpText(
                        NicoString.GetLiveTopUrl(),
                        null,
                        cookieContainer,
                        Encoding.UTF8);
                    if (string.IsNullOrEmpty(text))
                    {
                        continue;
                    }*/

                    // アカウント情報を取得します。
                    return AccountInfo.Create(cookieContainer);
                }
                catch (TimeoutException)
                {
                    // タイムアウト例外は無視します。
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// 与えられたクッキーでニコニコにログインできるか確かめます。
        /// </summary>
        private static AccountInfo Validate(int userId,
                                            CookieContainer cookieContainer)
        {
            // ページの取得がタイムアウトすることがあるので、
            // 一応何回か試してみます。
            for (var i = 0; i < 5; ++i)
            {
                try
                {
                    /*var text = WebUtil.RequestHttpText(
                        NicoString.GetLiveTopUrl(),
                        null,
                        cookieContainer,
                        Encoding.UTF8);
                    if (string.IsNullOrEmpty(text))
                    {
                        continue;
                    }*/

                    // アカウント情報を取得します。
                    var account = AccountInfo.Create(userId, cookieContainer);
                    if (account == null)
                    {
                        return null;
                    }

                    return account;
                }
                catch
                {
                    // 例外は無視します。
                }
            }

            return null;
        }

        /// <summary>
        /// クッキー確認用のスレッドです。１秒ごとに１クッキー確認します。
        /// </summary>
        private static void ThreadMain(object state)
        {
            while (true)
            {
                try
                {
                    InternalData data;

                    lock (targetList)
                    {
                        // データをリストから取得します。
                        data = PopData();
                        if (data == null)
                        {
                            Monitor.Wait(targetList, 1000);
                            continue;
                        }
                    }

                    // 取得したアカウントをコールバックで通知します。
                    // nullならクッキーが無効だということです。
                    var account = Validate(data.UserId, data.CookieContainer);

                    data.Callback(data.CookieContainer, account);

                    // 確認は１秒に一回行います。
                    Thread.Sleep(1000);
                }
                catch
                {
                    // 例外は無視します。
                }
            }
        }

        /// <summary>
        /// 与えられたクッキーでニコニコにログインできるか確かめます。
        /// </summary>
        public static void BeginValidate(CookieContainer cc,
                                         ValidateCallback callback,
                                         bool highPriority)
        {
            var data = MakeData(cc, callback);
            if (data == null)
            {
                if (callback != null)
                {
                    callback(cc, null);
                }

                return;
            }

            PushData(data, highPriority);
        }

        /// <summary>
        /// 与えられたクッキーでニコニコにログインできるか確かめます。
        /// </summary>
        public static void BeginValidate(CookieCollection cookieCollection,
                                         ValidateCallback callback,
                                         bool highPriority)
        {
            var cc = new CookieContainer();
            cc.Add(cookieCollection);

            BeginValidate(cc, callback, highPriority);
        }

        /// <summary>
        /// 与えられたクッキーでニコニコにログインできるか確かめます。
        /// </summary>
        public static void BeginValidate(Cookie cookie, ValidateCallback callback,
                                         bool highPriority)
        {
            var cc = new CookieContainer();
            cc.Add(cookie);

            BeginValidate(cc, callback, highPriority);
        }
    }
}
