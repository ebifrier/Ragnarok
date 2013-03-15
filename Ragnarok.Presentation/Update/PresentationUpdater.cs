using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Ragnarok.Update;
using Ragnarok.ObjectModel;

namespace Ragnarok.Presentation.Update
{
    /// <summary>
    /// ウィンドウなどを表示しながら、アプリの更新を行います。
    /// </summary>
    public class PresentationUpdater : NotifyObject
    {
        private readonly Updater updater;
        private UpdateWindow updateWindow;
        private DownloadProgressWindow progressWindow;

        /// <summary>
        /// ダウンロード用オブジェクトを取得します。
        /// </summary>
        public Downloader Downloader
        {
            get { return this.updater.Downloader; }
        }

        /// <summary>
        /// アセンブリ名を取得します。
        /// </summary>
        public string AssemblyTitle
        {
            get { return GetValue<string>("AssemblyTitle"); }
            private set { SetValue("AssemblyTitle", value); }
        }

        /// <summary>
        /// 最新のアセンブリバージョンを取得します。
        /// </summary>
        public string LatestVersion
        {
            get { return GetValue<string>("LatestVersion"); }
            private set { SetValue("LatestVersion", value); }
        }

        /// <summary>
        /// インストールされているアセンブリバージョンを取得します。
        /// </summary>
        public string InstalledVersion
        {
            get { return GetValue<string>("InstalledVersion"); }
            private set { SetValue("InstalledVersion", value); }
        }

        /// <summary>
        /// リリースノートのURLを取得します。
        /// </summary>
        public string ReleaseNotesLink
        {
            get { return GetValue<string>("ReleaseNotesLink"); }
            private set { SetValue("ReleaseNotesLink", value); }
        }

        /// <summary>
        /// ファイルのダウンロード中に発生したエラーを取得します。
        /// </summary>
        public Exception DownloadError
        {
            get { return GetValue<Exception>("DownloadError"); }
            private set { SetValue("DownloadError", value); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PresentationUpdater(string appCastUri)
        {
            var updater = new Updater(appCastUri);
            updater.UpdateDetected += updater_UpdateDetected;
            updater.DownloadDone += updater_DownloadDone;

            AssemblyTitle = updater.ApplicationConfig.ApplicationName;
            InstalledVersion = updater.ApplicationConfig.InstalledVersion;

            this.updater = updater;
        }

        void updater_UpdateDetected(object sender, UpdateDetectedEventArgs e)
        {
            LatestVersion = e.LatestVersion.Version;
            ReleaseNotesLink = e.LatestVersion.ReleaseNotesLink;
        }

        void updater_DownloadDone(object sender, DownloadDoneEventArgs e)
        {
            DownloadError = e.Error;

            if (e.IsCancelled)
            {
                return;
            }

            if (e.Error != null)
            {
                WpfUtil.UIProcess(() =>
                    DialogUtil.ShowError("更新ファイルのダウンロードに失敗しました。"));
                return;
            }

            WpfUtil.UIProcess(() =>
            {
                if (!this.updater.CanExecutePack())
                {
                    // 更新できません＞＜
                    return;
                }

                if (this.progressWindow != null)
                {
                    this.progressWindow.Close();
                    this.progressWindow = null;
                }

                if (ConfirmUpdate(true))
                {
                    ExecuteUpdate();
                }
            });
            e.IsUpdate = false;
        }

        /// <summary>
        /// ウィンドウを使って更新するかどうか確認します。
        /// </summary>
        private bool ConfirmUpdate(bool downloaded)
        {
            if (this.updateWindow != null)
            {
                this.updateWindow.Activate();
                return false;
            }

            this.updateWindow = new UpdateWindow
            {
                IsDownloaded = downloaded,
                Topmost = true,
                DataContext = this,
            };
            this.updateWindow.Closed +=
                (_, __) => this.updateWindow = null;

            return (this.updateWindow.ShowDialog() == true);
        }

        /// <summary>
        /// ダウンロードの進行度を表示します。
        /// </summary>
        private void ShowProgress()
        {
            if (this.progressWindow != null)
            {
                this.progressWindow.Activate();
                return;
            }

            if (this.updateWindow != null)
            {
                return;
            }

            this.progressWindow = new DownloadProgressWindow
            {
                Topmost = true,
                DataContext = this,
            };
            this.progressWindow.Closed +=
                (_, __) => this.progressWindow = null;

            this.progressWindow.ShowDialog();
        }

        /// <summary>
        /// 実際の更新処理を行います。
        /// </summary>
        private void ExecuteUpdate()
        {
            if (!this.updater.CanExecutePack())
            {
                // 更新できません＞＜
                return;
            }

            this.updater.ExecutePack();
        }

        /// <summary>
        /// 更新処理を開始します。
        /// </summary>
        public void Start()
        {
            this.updater.Start();
        }

        /// <summary>
        /// 更新処理を停止します。
        /// </summary>
        public void Stop()
        {
            this.updater.Stop();
        }

        /// <summary>
        /// 更新情報の確認やダイアログの表示などを行います。
        /// </summary>
        public void CheckToUpdate(TimeSpan timeout)
        {
            if (this.updateWindow != null || this.progressWindow != null)
            {
                // すでに確認用のウィンドウが起動しています。
                return;
            }

            if (!this.updater.LatestVersionEvent.WaitOne(timeout))
            {
                DialogUtil.ShowError(
                    string.Format(
                        "更新情報の確認ができません（T￢T){0}{0}" +
                        "少し時間をおいてから、もう一度試してみて下さい。",
                        Environment.NewLine));
                return;
            }

            var latestVersion = this.updater.LatestVersion;
            if (latestVersion == null)
            {
                DialogUtil.Show("更新はありません d(-_☆)", "確認", MessageBoxButton.OK);
                return;
            }

            // ファイルのダウンロード中なら
            if (!this.updater.DownloadDoneEvent.WaitOne(0))
            {
                ShowProgress();
            }
            else
            {
                if (ConfirmUpdate(true))
                {
                    ExecuteUpdate();
                }
            }
        }
    }
}
