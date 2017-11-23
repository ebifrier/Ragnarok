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
        public ChannelLoginData(int channelId, string mail, string password)
        {
            ChannelId = channelId;
            Mail = mail;
            Password = password;
        }

        /// <summary>
        /// ログインIDを取得します。
        /// </summary>
        public int ChannelId
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
            return ChannelTool.Login(ChannelId, Mail, Password);
        }
    }
}
