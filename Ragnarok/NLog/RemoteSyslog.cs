#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Reflection;
using System.Globalization;

using NLog;
using NLog.Config;

namespace Ragnarok.NLog
{
    /// <summary>
    /// UDPでsyslogにログを送信します。
    /// </summary>
    [Target("RemoteSyslog")]
    public class RemoteSyslogTarget : TargetWithLayout
    {
        private Layout senderLayout;
        private Layout machineLayout;

        /// <summary>
        /// ファシリティを取得または設定します。
        /// </summary>
        public SyslogFacility Facility
        {
            get;
            set;
        }

        /// <summary>
        /// ログサーバーのアドレスを取得または設定します。
        /// </summary>
        [AcceptsLayout]
        public string Server
        {
            get;
            set;
        }

        /// <summary>
        /// ログサーバーのポート番号を取得または設定します。
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// マシン名を取得または設定します。
        /// </summary>
        [AcceptsLayout]
        public string Machine
        {
            get { return this.machineLayout.Text; }
            set { this.machineLayout = new Layout(value); }
        }

        /// <summary>
        /// ログ出力時の名前を取得または設定します。
        /// </summary>
        [AcceptsLayout]
        public string Sender
        {
            get { return this.senderLayout.Text; }
            set { this.senderLayout = new Layout(value); }
        }

        /// <summary>
        /// 使用中のすべてのレイアウトをコレクションに追加します。
        /// </summary>
        public override void PopulateLayouts(LayoutCollection layouts)
        {
            base.PopulateLayouts(layouts);

            this.senderLayout.PopulateLayouts(layouts);
        }

        /// <summary>
        /// syslogに送るログのプライオリティを計算します。
        /// </summary>
        private int CalcSyslogPriority(SyslogSeverity priority)
        {
            return ((int)this.Facility * 8 + (int)priority);
        }

        /// <summary>
        /// NLogのログレベルをsyslogのログレベルに変換します。
        /// </summary>
        private SyslogSeverity GetSyslogSeverity(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Fatal)
            {
                return SyslogSeverity.Emergency;
            }
            if (logLevel >= LogLevel.Error)
            {
                return SyslogSeverity.Error;
            }
            if (logLevel >= LogLevel.Warn)
            {
                return SyslogSeverity.Warning;
            }
            if (logLevel >= LogLevel.Info)
            {
                return SyslogSeverity.Information;
            }
            if (logLevel >= LogLevel.Debug)
            {
                return SyslogSeverity.Debug;
            }
            if (logLevel >= LogLevel.Trace)
            {
                return SyslogSeverity.Notice;
            }
            return SyslogSeverity.Notice;
        }

        /// <summary>
        /// UDPを使い、syslogにログを送ります。
        /// </summary>
        private void SendMessage(byte[] message)
        {
            UdpClient udp = null;

            try
            {
                udp = new UdpClient(Server, Port);
                udp.Send(message, message.Length);
            }
            finally
            {
                if (udp != null)
                {
                    udp.Close();
                    udp = null;
                }
            }
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        protected override void Write(LogEventInfo logEvent)
        {
            // "Apr 16 10:55:22" などど出したいので、
            // ここでは米国カルチャを選択します。
            var culture = CultureInfo.GetCultureInfo("en-US");
            var priority = GetSyslogSeverity(logEvent.Level);
            var machine = this.machineLayout.GetFormattedMessage(logEvent);
            var sender = this.senderLayout.GetFormattedMessage(logEvent);
            var body = CompiledLayout.GetFormattedMessage(logEvent);

            var message = string.Format(
                "<{0}>{1} {2} {3}: {4}",
                CalcSyslogPriority(priority),
                logEvent.TimeStamp.ToString("MMM dd HH:mm:ss", culture),
                machine,
                sender,
                body);

            var buffer = Encoding.UTF8.GetBytes(message);
            SendMessage(buffer);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RemoteSyslogTarget()
        {
            this.Server = "127.0.0.1";
            this.Port = 514;
            this.Machine = "${machinename}";
            this.Sender = "${processname}";
            this.Facility = SyslogFacility.Local0;
        }
    }
}
#endif
