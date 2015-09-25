using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Ragnarok.NicoNico.Provider
{
    /// <summary>
    /// チャンネルへのログイン用データを保持します。
    /// </summary>
    public sealed class ChannelLoginData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChannelLoginData(string id, string mail, string password)
        {
            Id = id;
            Mail = mail;
            Password = password;
        }

        /// <summary>
        /// ログインIDを取得します。
        /// </summary>
        public string Id
        {
            get;
            private set;
        }

        /// <summary>
        /// ログイン用のメールアドレスを取得します。
        /// </summary>
        public string Mail
        {
            get;
            private set;
        }

        /// <summary>
        /// ログイン用のパスワードを取得します。
        /// </summary>
        public string Password
        {
            get;
            private set;
        }

        /// <summary>
        /// チャンネルページへのログイン処理を行います。
        /// </summary>
        public CookieContainer Login()
        {
            return ChannelTool.Login(Id, Mail, Password);
        }
    }
}
