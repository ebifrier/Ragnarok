using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Ragnarok.Net.CookieGetter;
using Ragnarok.NicoNico;
using Ragnarok.NicoNico.Login;

namespace Ragnarok.Presentation.NicoNico
{
    /// <summary>
    /// ログイン用クッキーを管理します。
    /// </summary>
    public class CookieData
    {
        /// <summary>
        /// クッキーを取得または設定します。
        /// </summary>
        public CookieContainer CookieContainer
        {
            get;
            set;
        }

        /// <summary>
        /// このクッキーをもつブラウザを取得または設定します。
        /// </summary>
        public BrowserType BrowserType
        {
            get;
            set;
        }

        /// <summary>
        /// ユーザーＩＤを取得します。
        /// </summary>
        public int UserId
        {
            get
            {
                if (this.CookieContainer == null)
                {
                    return 0;
                }

                return AccountInfo.GetUserIdFromCookie(
                    this.CookieContainer);
            }
        }
    }
}
