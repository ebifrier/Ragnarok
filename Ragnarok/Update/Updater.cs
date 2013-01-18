using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Ragnarok.Update
{
    /// <summary>
    /// アプリの更新処理を行います。
    /// </summary>
    /// <remarks>
    /// 更新処理には大まかに以下の４つの処理があります。
    /// １、バージョン情報の確認
    /// ２、更新用ファイルのダウンロード
    /// ３、ユーザーに対する更新実行の確認
    /// ４、ファイル更新＆アプリ再起動
    /// 
    /// このクラスでは更新用のファイルを裏でダウンロードし、
    /// ユーザーへの更新確認をダウンロード状態にかかわらず
    /// いつでも出せるようにしています。
    /// </remarks>
    public class Updater : IDisposable
    {
        private Downloader downloader;
        private string appCastUrl;
        private Configuration config;

        private ManualResetEvent latestVersionEvent = new ManualResetEvent(false); 
        private AppCastItem latestVersion;

        private ManualResetEvent downloadDoneEvent = new ManualResetEvent(false);
        private string downloadFilePath;
        private string packFilePath;
        private string packConfigFilePath;

        private ManualResetEvent startedEvent = new ManualResetEvent(false);
        private int isDownloadFailedInt;
        private bool disposed;

        /// <summary>
        /// 新たな更新情報が確認されたときに呼ばれます。
        /// </summary>
        public event EventHandler<UpdateDetectedEventArgs> UpdateDetected;

        /// <summary>
        /// ダウンロードファイルがすべてそろった時に呼ばれます。
        /// </summary>
        public event EventHandler<DownloadDoneEventArgs> DownloadDone;

        /// <summary>
        /// コンフィグ情報を取得します。
        /// </summary>
        public Configuration ApplicationConfig
        {
            get { return this.config; }
        }

        /// <summary>
        /// 更新情報を取得した場合はシグナル状態になります。
        /// </summary>
        public ManualResetEvent LatestVersionEvent
        {
            get { return this.latestVersionEvent; }
        }

        /// <summary>
        /// 更新情報を取得します。
        /// </summary>
        public AppCastItem LatestVersion
        {
            get { return this.latestVersion; }
        }

        /// <summary>
        /// ダウンロードが終了したらシグナル状態になります。
        /// </summary>
        public ManualResetEvent DownloadDoneEvent
        {
            get { return this.downloadDoneEvent; }
        }

        /// <summary>
        /// ダウンロード状態などを扱うオブジェクトを取得します。
        /// </summary>
        public Downloader Downloader
        {
            get { return this.downloader; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Updater(string appCastUrl)
            : this(appCastUrl, null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>        
        public Updater(string appCastUrl, string assemblyName)
        {
            this.appCastUrl = appCastUrl;
            this.config = new Configuration(assemblyName);

            // set the url
            Log.Info("Updater use the following url: {0}", appCastUrl);
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Updater()
        {
            Dispose(false);
        }

        /// <summary>
        /// 更新処理を停止します。
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// 更新処理を停止します。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Stop();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// 更新処理を開始します。
        /// </summary>
        public void Start()
        {
            if (this.startedEvent.WaitOne(0))
            {
                // 既に開始済み
                return;
            }

            try
            {
                Log.Info("Updater starts download.");

                // 更新情報の取得を開始します。
                var web = new WebClient();
                web.DownloadDataCompleted += web_DownloadDataCompleted;
                web.DownloadDataAsync(new Uri(this.appCastUrl));
            }
            finally
            {
                this.latestVersionEvent.Set();
            }

            this.startedEvent.Set();
        }

        /// <summary>
        /// 更新処理を停止します。
        /// </summary>
        public void Stop()
        {
            if (this.startedEvent.WaitOne(0))
            {
                this.downloader.CancelAll();
            }
        }

        #region 更新情報の取得
        /// <summary>
        /// This method checks if an update is required. During this process the appcast
        /// will be downloaded and checked against the reference assembly. Ensure that
        /// the calling process has access to the internet and read access to the 
        /// reference assembly. This method is also called from the background loops.
        /// </summary>
        private bool IsUpdateRequired(AppCastItem latestVersion)
        {
            if (latestVersion == null)
            {
                Log.Info(
                    "Updater: No version information in app cast found.");
                return false;
            }
            else
            {
                Log.Info(
                    "Updater: Lastest version on the server is {0}.",
                    latestVersion.Version);
            }

            // check if the available update has to be skipped
            if (latestVersion.Version.Equals(this.config.SkipThisVersion))
            {
                Log.Info(
                    "Updater: Latest update has to be skipped (user decided to skip version {0})",
                    config.SkipThisVersion);
                return false;
            }

            // check if the version will be the same then the installed version
            var v1 = new Version(this.config.InstalledVersion);
            var v2 = new Version(latestVersion.Version);

            if (v2 <= v1)
            {
                Log.Info(
                    "Updater: Installed version is valid, no update needed. ({0})",
                    this.config.InstalledVersion);
                return false;
            }

            // ok we need an update
            return true;
        }

        /// <summary>
        /// 新たな更新情報が確認された後の手続きを確認します。
        /// </summary>
        private NextUpdateAction OnUpdateDetected(AppCastItem latestVersion)
        {
            var e = new UpdateDetectedEventArgs
            {
                NextAction = NextUpdateAction.ContinueToUpdate,
                ApplicationConfig = config,
                LatestVersion = latestVersion,
            };

            UpdateDetected.SafeRaiseEvent(this, e);
            return e.NextAction;
        }

        /// <summary>
        /// 更新情報のダウンロード後に呼ばれます。
        /// </summary>
        void web_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                Log.ErrorException(e.Error,
                    "更新情報の取得に失敗しました。");

                this.latestVersionEvent.Set();
                return;
            }

            try
            {
                var text = Encoding.UTF8.GetString(e.Result);
                var latestVersion = AppCastItemUtil.GetLatestVersion(text);
                if (!IsUpdateRequired(latestVersion))
                {
                    this.latestVersionEvent.Set();
                    return;
                }

                this.latestVersion = latestVersion;
                this.latestVersionEvent.Set();

                // show the update window
                Log.Info(
                    "Update needed from version {0} to version {1}.",
                    this.config.InstalledVersion,
                    latestVersion.Version);

                switch (OnUpdateDetected(latestVersion))
                {
                    case NextUpdateAction.ContinueToUpdate:
                        Log.Info("Updater: Continue to update");
                        BeginDownload(latestVersion);
                        break;
                    case NextUpdateAction.ProhibitUpdate:
                    default:
                        Log.Info("Updater: Update prohibited");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "ダウンロードに失敗しました。");
            }
        }
        #endregion

        #region ファイルのダウンロード
        /// <summary>
        /// 必要なファイルのダウンロードを開始します。
        /// </summary>
        private void BeginDownload(AppCastItem latestVersion)
        {
            this.downloader = new Downloader();
            this.downloadFilePath = Path.GetTempFileName();
            this.packFilePath = Path.GetTempFileName();
            this.packConfigFilePath = this.packFilePath + ".config";

            this.downloader.BeginDownload(
                new Uri(latestVersion.DownloadLink),
                (_, e) => SaveDownloadFile(this.downloadFilePath, e));
            this.downloader.BeginDownload(
                new Uri(latestVersion.UpdatePackLink),
                (_, e) => SaveDownloadFile(this.packFilePath, e));
            this.downloader.BeginDownload(
                new Uri(latestVersion.UpdatePackLink + ".config"),
                (_, e) => SaveDownloadFile(this.packConfigFilePath, e));
        }

        /// <summary>
        /// ファイルを保存し、成功したかどうかを返します。
        /// </summary>
        /// <returns>
        /// エラーの場合はfalseを返します。
        /// </returns>
        private void SaveDownloadFile(string filename, DownloadDataCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                this.downloader.CancelAll();
                HandleDownloadDone(e.Cancelled, e.Error);
                return;
            }

            try
            {
                Log.Info("{0}: ファイルのダウンロードを終了しました。", filename);

                using (var stream = new FileStream(filename, FileMode.Create))
                {
                    stream.Write(e.Result, 0, e.Result.Count());
                }

                HandleDownloadDone(false, null);
            }
            catch (IOException ex)
            {
                Log.ErrorException(ex,
                    "ダウンロードデータの保存に失敗しました。");

                this.downloader.CancelAll();
                HandleDownloadDone(false, ex);
            }
        }

        /// <summary>
        /// アプリを更新するかどうかイベントを通して確認します。
        /// </summary>
        private bool OnDownloadDone(bool cancelled, Exception ex)
        {
            var e = new DownloadDoneEventArgs
            {
                IsUpdate = false,
                ApplicationConfig = this.config,
                LatestVersion = this.latestVersion,
                IsCancelled = cancelled,
                Error = ex,
            };

            DownloadDone.SafeRaiseEvent(this, e);
            return e.IsUpdate;
        }

        /// <summary>
        /// ダウンロード終了後に呼ばれます。
        /// </summary>
        private void HandleDownloadDone(bool cancelled, Exception ex)
        {
            // エラーの場合は呼びます。
            if (cancelled || ex != null)
            {
                if (Interlocked.Exchange(ref this.isDownloadFailedInt, 1) == 1)
                {
                    return;
                }

                this.downloadDoneEvent.Set();
                OnDownloadDone(cancelled, ex);
                return;
            }

            // すべてのダウンロードが終わった時にも呼びます。
            if (this.downloader.Count == 0)
            {
                Log.Info("すべてのダウンロードを終了しました。");
                this.downloadDoneEvent.Set();

                // MD5のチェックを行います。
                var correctMD5 = this.latestVersion.MD5Signature;
                if (!string.IsNullOrEmpty(correctMD5))
                {
                    var targetMD5 = MD5Verificator.ComputeMD5(this.downloadFilePath);
                    if (!MD5Verificator.CompareMD5(targetMD5, correctMD5))
                    {
                        OnDownloadDone(false, new Exception());
                        return;
                    }
                }

                if (!OnDownloadDone(cancelled, ex))
                {
                    return;
                }

                ExecutePack();
            }
        }
        #endregion

        #region アプリ更新
        /// <summary>
        /// 更新処理が実行可能かどうか取得します。
        /// </summary>
        public bool CanExecutePack()
        {
            return this.downloadDoneEvent.WaitOne(0);
        }

        /// <summary>
        /// 実際の更新処理を行います。
        /// </summary>
        public void ExecutePack()
        {
            if (!CanExecutePack())
            {
                throw new RagnarokUpdateException(
                    "アプリの更新を実行できません。");
            }

            try
            {
                Log.Info("実際のアプリ更新処理を開始します。");

                ExecutePackInternal();
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "アプリの更新処理に失敗しました。");
            }
        }

        /// <summary>
        /// 実際の更新処理を外部exeを呼び出すことで行います。
        /// </summary>
        private void ExecutePackInternal()
        {
            var workingDir = Environment.CurrentDirectory;

            // start update helper
            // 0. this process' id
            // 1. zip file path
            // 2. the top directory of this program
            // 3. the path of the restart program
            var startInfo = new ProcessStartInfo
            {
                FileName = this.packFilePath,
                Arguments = string.Format(
                    @"{0} ""{1}"" ""{2}"" {3}",
                    Process.GetCurrentProcess().Id,
                    this.downloadFilePath,
                    workingDir,
                    Environment.CommandLine),
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            Process.Start(startInfo);
        }
        #endregion
    }
}
