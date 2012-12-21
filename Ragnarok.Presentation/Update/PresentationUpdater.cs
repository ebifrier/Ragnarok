using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private UpdateWindow window;

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
            if (e.Error != null)
            {
                WpfUtil.UIProcess(() =>
                    DialogUtil.ShowError("更新ファイルのダウンロードに失敗しました。"));
                return;
            }

            WpfUtil.UIProcess(ConfirmUpdate);
            e.IsUpdate = false;
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
        /// ウィンドウを使って更新するかどうか確認します。
        /// </summary>
        private void ConfirmUpdate()
        {
            if (!this.updater.CanExecutePack())
            {
                // 更新できません＞＜
                return;
            }

            if (this.window != null)
            {
                this.window.Activate();
                return;
            }

            this.window = new UpdateWindow
            {
                Topmost = true,
                DataContext = this,
            };
            this.window.Closed += (_, __) => this.window = null;

            var result = this.window.ShowDialog();
            if (result != null && result.Value)
            {
                this.updater.ExecutePack();
            }
        }
    }
}
