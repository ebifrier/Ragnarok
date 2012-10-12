using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NLog
{
    /// <summary>
    /// ログレベルです。
    /// </summary>
    public enum SyslogSeverity
    {
        /// <summary>
        /// 危険レベル
        /// </summary>
        Emergency = 0,
        /// <summary>
        /// 危険レベル
        /// </summary>
        Alert = 1,
        /// <summary>
        /// 重大エラー
        /// </summary>
        Critical = 2,
        /// <summary>
        /// エラー
        /// </summary>
        Error = 3,
        /// <summary>
        /// 警告
        /// </summary>
        Warning = 4,
        /// <summary>
        /// 通知
        /// </summary>
        Notice = 5,
        /// <summary>
        /// 情報表示
        /// </summary>
        Information = 6,
        /// <summary>
        /// デバッグ用
        /// </summary>
        Debug = 7,
    }

    /// <summary>
    /// ログの種類です。
    /// </summary>
    public enum SyslogFacility
    {
        /// <summary>
        /// カーネルログ
        /// </summary>
        Kernel = 0,
        /// <summary>
        /// ユーザーログ
        /// </summary>
        User = 1,
        /// <summary>
        /// メールログ
        /// </summary>
        Mail = 2,
        /// <summary>
        /// デーモンログ
        /// </summary>
        Daemon = 3,
        /// <summary>
        /// 権限ログ
        /// </summary>
        Auth = 4,
        /// <summary>
        /// syslogのログ
        /// </summary>
        Syslog = 5,
        /// <summary>
        /// プリンタログ
        /// </summary>
        Printer = 6,
        /// <summary>
        /// ニュースログ
        /// </summary>
        News = 7,
        /// <summary>
        /// UUCPログ
        /// </summary>
        UUCP = 8,
        /// <summary>
        /// cronログ
        /// </summary>
        Cron = 9,
        /// <summary>
        /// 権限ログ
        /// </summary>
        AuthPriv = 10,
        /// <summary>
        /// ftpログ
        /// </summary>
        Ftp = 11,
        /// <summary>
        /// ローカルログ０
        /// </summary>
        Local0 = 16,
        /// <summary>
        /// ローカルログ１
        /// </summary>
        Local1 = 17,
        /// <summary>
        /// ローカルログ２
        /// </summary>
        Local2 = 18,
        /// <summary>
        /// ローカルログ３
        /// </summary>
        Local3 = 19,
        /// <summary>
        /// ローカルログ４
        /// </summary>
        Local4 = 20,
        /// <summary>
        /// ローカルログ５
        /// </summary>
        Local5 = 21,
        /// <summary>
        /// ローカルログ６
        /// </summary>
        Local6 = 22,
        /// <summary>
        /// ローカルログ７
        /// </summary>
        Local7 = 23,
    }
}
