using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Ragnarok;
using Ragnarok.ObjectModel;
using Ragnarok.Presentation.Control;

namespace Ragnarok.Presentation.Debug
{
    /// <summary>
    /// 報告する内容の種別です。
    /// </summary>
    public enum ReportType
    {
        /// <summary>
        /// 要望
        /// </summary>
        Demand,
        /// <summary>
        /// エラー報告
        /// </summary>
        Error,
        /// <summary>
        /// エラー報告（ログ付き）
        /// </summary>
        ErrorWithLog,
        /// <summary>
        /// その他
        /// </summary>
        Other,
    }

    /// <summary>
    /// エラー報告時に使うダイアログのモデルオブジェクトです。
    /// </summary>
    public class ReportDialogModel : NotifyObject
    {
        private static readonly Encoding DefaultEncoding =
            Encoding.GetEncoding(
#if MONO
                "euc-jp"
#else
                "Shift_JIS"
#endif
                );

        private ReportType reportType = ReportType.Demand;
        private string reportText = "";
        private string errorLogText = "";

        /// <summary>
        /// 報告する内容種別を取得または設定します。
        /// </summary>
        public ReportType ReportType
        {
            get
            {
                return this.reportType;
            }
            set
            {
                if (this.reportType != value)
                {
                    this.reportType = value;

                    this.RaisePropertyChanged("ReportType");
                }
            }
        }

        /// <summary>
        /// エラーログを送信するかどうかを取得します。
        /// </summary>
        [DependOn("ReportType")]
        public bool IsUseErrorLog
        {
            get
            {
                return (ReportType == ReportType.ErrorWithLog);
            }
        }

        /// <summary>
        /// 報告内容を取得または設定します。
        /// </summary>
        public string ReportText
        {
            get
            {
                return this.reportText;
            }
            set
            {
                if (this.reportText != value)
                {
                    this.reportText = value;

                    this.RaisePropertyChanged("ReportText");
                }
            }
        }

        /// <summary>
        /// エラーログ内容を取得または設定します。
        /// </summary>
        public string ErrorLogText
        {
            get
            {
                return this.errorLogText;
            }
            set
            {
                if (this.errorLogText != value)
                {
                    this.errorLogText = value;

                    this.RaisePropertyChanged("ErrorLogText");
                }
            }
        }

        /// <summary>
        /// ファイルからログ内容を読み込みます。エンコーディングは
        /// 環境ごとのデフォルトが使われます。
        /// </summary>
        public bool OpenErrorLog(string logfile)
        {
            return OpenErrorLog(logfile, DefaultEncoding);
        }

        /// <summary>
        /// ファイルからログ内容を読み込みます。
        /// </summary>
        public bool OpenErrorLog(string logfile, Encoding encoding)
        {
            if (!File.Exists(logfile))
            {
                // ログはありませぬ。
                return false;
            }

            try
            {
                using (var reader = new StreamReader(logfile, encoding))
                {
                    // 不要な部分を切り取る可能性があるため、
                    // 各エラーのまとまりごとに読み込みます。
                    var errorLinesList = ReadErrorLines(reader)
                        .Where(IsNotIgnoreError);

                    // 最後から数えて500行分までの情報を送ります。
                    // 変数キャプチャの関係でTakeWhileは一度しか呼べません。
                    var count = 0;
                    var linesList = errorLinesList.Reverse()
                        .TakeWhile(lines =>
                        {
                            count += lines.Count;
                            return (count < 500);
                        });

                    // 変数キャプチャを使っているため、Reverseの実装で
                    // 結果が変わることもありえる！？
                    // 他の方法で実装した方がいいかも。
                    ErrorLogText = string.Join(
                        Environment.NewLine,
                        linesList.Reverse().SelectMany(_ => _).ToArray());

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "エラーログの読み込みに失敗しました。");

                return false;
            }
        }

        /// <summary>
        /// 各エラー行は空行だけの行で区切られています。
        /// その区切りごとにエラーを取得します。
        /// </summary>
        private static IEnumerable<List<string>> ReadErrorLines(StreamReader reader)
        {
            var result = new List<string>();

            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (line.IsWhiteSpaceOnly())
                {
                    yield return result;

                    result = new List<string>();
                }

                result.Add(line);
            }
        }

        /// <summary>
        /// ログ送信するエラーか調べます。
        /// </summary>
        private static bool IsNotIgnoreError(IEnumerable<string> lines)
        {
            return !lines.Any(line =>
                line.IndexOf("ログイン出来ません。") >= 0);
        }

        /// <summary>
        /// 送信するデータを作成します。
        /// </summary>
        private void MakeSendData(out SmtpClient oSmtp, out MailMessage oMail)
        {
            const string user = "ebifrier.send@gmail.com";
            const string pass = "testsan-xxx";
            var smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true,
            };

            var body =
                "報告。\r\n" +
                "\r\n" +
                ReportText;

            // 送信メールの作成。
            var mail = new MailMessage(
                "ebifrier.send@gmail.com",
                "ebifrier@gmail.com",
                "エラー報告・要望など",
                body);

            if (IsUseErrorLog)
            {
                var data = Encoding.UTF8.GetBytes(ErrorLogText);
                var stream = new MemoryStream(data);

                // 添付ファイルとしてエラーログを送信。
                mail.Attachments.Add(new Attachment(stream, "errorlog.txt"));
            }

            oSmtp = smtp;
            oMail = mail;
        }

        /// <summary>
        /// メールを送信します。
        /// </summary>
        private void SendMail(SmtpClient smtp, MailMessage mail)
        {
            var ev = new AutoResetEvent(false);
            Exception error = null;

            // SendCompletedは成功時/キャンセル時の
            // 両方で呼ばれます。
            smtp.SendCompleted +=
                (sender, args) =>
                {
                    error = args.Error;
                    ev.Set();
                };

            // 送信キャンセルが可能なように、非同期メソッドで
            // 同期的な処理を行っています。
            smtp.SendAsync(mail, null);

            // 待ちのタイムアウト時や失敗時は例外を投げます。
            if (!ev.WaitOne(TimeSpan.FromSeconds(30)))
            {
                throw new TimeoutException(
                    "メール送信がタイムアウトしました。");
            }
            else if (error != null)
            {
                throw new Exception(
                    "メールの送信に失敗しました。", error);
            }
        }

        /// <summary>
        /// ログを送信します。
        /// </summary>
        public bool SendReport()
        {
            ProgressDialog progressDialog = null;

            try
            {
                SmtpClient smtp;
                MailMessage mail;
                MakeSendData(out smtp, out mail);

                progressDialog = new ProgressDialog(
                    () => SendMail(smtp, mail))
                {
                    Topmost = true,
                    Title = "送信中",
                };

                progressDialog.Cancel +=
                    (sender, args) => smtp.SendAsyncCancel();

                progressDialog.ShowDialog();
                if (progressDialog.IsFaulted)
                {
                    DialogUtil.ShowError(
                        progressDialog.Exception,
                        "メール送信に失敗しました。(ﾉд-｡)");

                    return false;
                }
                else if (progressDialog.IsCanceled)
                {
                    DialogUtil.ShowError(
                        "メール送信がキャンセルされました。(-д-｡)");

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "ログの送信に失敗しました。");

                if (progressDialog != null)
                {
                    progressDialog.Close();
                }

                DialogUtil.ShowError(ex,
                    "ログの送信に失敗しました。");

                return false;
            }
        }
    }
}
