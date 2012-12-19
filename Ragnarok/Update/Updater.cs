using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Ragnarok.Update
{
    /// <summary>
    /// Everytime when netsparkle detects an update the 
    /// consumer can decide what should happen as next with the help 
    /// of the UpdateDatected event
    /// </summary>
    public enum NextUpdateAction
    {
        ShowStandardUserInterface = 1,
        PerformUpdateUnattended = 2,
        ProhibitUpdate = 3
    }

    /// <summary>
    /// Contains all information for the update detected event
    /// </summary>
    public class UpdateDetectedEventArgs : EventArgs
    {
        public NextUpdateAction NextAction
        {
            get;
            set;
        }

        public Configuration ApplicationConfig
        {
            get;
            set;
        }

        public AppCastItem LatestVersion
        {
            get;
            set;
        }
    }

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
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private readonly Downloader downloader = new Downloader();

        private string appCastUrl;
        private Configuration config;
        private DateTime lastCheckTime = DateTime.Now;

        private string downloadFilePath;

        private AutoResetEvent _exitHandle = new AutoResetEvent(false);
        private ManualResetEvent _loopingHandle = new ManualResetEvent(false);
        private AutoResetEvent forceUpdateCheckHandle = new AutoResetEvent(false);
        private ManualResetEvent downloadDoneHandle = new ManualResetEvent(false);

        /// <summary>
        /// This event can be used to override the standard user interface
        /// process when an update is detected
        /// </summary>
        public event EventHandler<UpdateDetectedEventArgs> UpdateDetected;

        public bool IsInitialCheck = true;
        public TimeSpan CheckFrequency = TimeSpan.FromDays(1);

        /// <summary>
        /// Hides the release notes view when an update was found. This 
        /// mode is switched on automatically when no sparkle:releaseNotesLink
        /// tag was found in the app cast         
        /// </summary>
        public bool HideReleaseNotes = false;

        /// <summary>
        /// This property enables the silent mode, this means 
        /// the application will be updated without user interaction
        /// </summary>
        public bool EnableSilentMode
        {
            get;
            set;
        }

        /// <summary>
        /// This property returns true when the update loop is running
        /// and files when the loop is not running
        /// </summary>
        public bool IsUpdateLoopRunning
        {
            get { return _loopingHandle.WaitOne(0); }
        }

        public DateTime NextUpdateTime
        {
            get { return (this.lastCheckTime + CheckFrequency); }
        }

        public TimeSpan NextUpdateInterval
        {
            get { return (NextUpdateTime - DateTime.Now); }
        }

        /// <summary>
        /// ctor which needs the appcast url
        /// </summary>
        public Updater(string appCastUrl)
            : this(appCastUrl, null)
        {
        }

        /// <summary>
        /// ctor which needs the appcast url and a referenceassembly
        /// </summary>        
        public Updater(string appCastUrl, string assemblyName)
        {
            this.appCastUrl = appCastUrl;
            this.config = new Configuration(assemblyName);

            // adjust the delegates
            _worker.DoWork += _worker_DoWork;

            // set the url
            Log.Info("Updater use the following url: {0}", appCastUrl);
        }

        /// <summary>
        /// The method starts a NetSparkle background loop
        /// If NetSparkle is configured to check for updates on startup, proceeds to perform 
        /// the check. You should only call this function when your app is initialized and 
        /// shows its main window.
        /// </summary>
        public void StartLoop()
        {
            if (IsUpdateLoopRunning)
            {
                return;
            }

            // first set the event handle
            _loopingHandle.Set();

            // Start the helper thread as a background worker to 
            // get well ui interaction

            // create and configure the worker
            Log.Info("Updater starts background worker.");

            // start the work
            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// This method will stop the sparkle background loop and is called
        /// through the disposable interface automatically
        /// </summary>
        public void StopLoop()
        {
            // ensure the work will finished
            _exitHandle.Set();
        }

        /// <summary>
        /// Is called in the using context and will stop all background activities
        /// </summary>
        public void Dispose()
        {
            StopLoop();
        }

        private bool IsCheckToUpdateNeeded()
        {
            if (DateTime.Now < NextUpdateTime)
            {
                Log.Info(
                    "Update check performed within the last {0} minutes !",
                    CheckFrequency.TotalMinutes);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the latest version information.
        /// </summary>
        private AppCastItem GetLatestVersion()
        {
            try
            {
                // set the last check time
                Log.Info("Updater: Touch the last check timestamp.");
                this.lastCheckTime = DateTime.Now;

                return AppCastItemUtil.GetLatestVersion(this.appCastUrl);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "Updater: Error during app cast download.");
            }

            return null;
        }

        /// <summary>
        /// This method checks if an update is required. During this process the appcast
        /// will be downloaded and checked against the reference assembly. Ensure that
        /// the calling process has access to the internet and read access to the 
        /// reference assembly. This method is also called from the background loops.
        /// </summary>
        private bool IsUpdateRequired(AppCastItem latestVersion)
        {
            Log.Info("Updater: Downloading and checking appcast");

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

        private AppCastItem GetVersionUpdated()
        {
            // check if it's ok the recheck to software state
            if (!IsCheckToUpdateNeeded())
            {
                return null;
            }

            // check if update is required
            var latestVersion = GetLatestVersion();
            if (!IsUpdateRequired(latestVersion))
            {
                return null;
            }

            // show the update window
            Log.Info(
                "Update needed from version {0} to version {1}.",
                config.InstalledVersion,
                latestVersion.Version);

            return latestVersion;
        }

        /// <summary>
        /// バージョン情報確認後に、アプリの更新処理を行います。
        /// </summary>
        private void BeginUpdate(AppCastItem latestVersion)
        {
            this.downloader.CancelAll();
            this.downloadDoneHandle.Reset();
            this.downloadFilePath = null;

            this.downloader.BeginDownload(
                new Uri(latestVersion.DownloadLink),
                Path.GetTempFileName(),
                DownloadLink_Downloaded);
        }

        private void DownloadLink_Downloaded(object sender, DownloadFileCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                this.downloadFilePath = e.FileName;

                if (this.downloader.Count == 0)
                {
                    this.downloadDoneHandle.Set();
                }
            }
            else
            {
                this.downloader.CancelAll();
                this.downloadDoneHandle.Set();
            }
        }

        private bool WaitDownload()
        {
            // build the event array
            var handles = new WaitHandle[]
            {
                this._exitHandle,
                this.downloadDoneHandle,
            };

            // wait for any
            var i = WaitHandle.WaitAny(handles, NextUpdateInterval);
            if (i == WaitHandle.WaitTimeout)
            {
                Log.Info(
                    "Updater: {0} minutes are over",
                    CheckFrequency.TotalMinutes);
                return true;
            }

            // check the exit hadnle
            if (i == 0)
            {
                Log.Info("Updater: Got exit signal");
                return false;
            }

            // ダウンロードの終了判定ハンドル
            if (i == 1)
            {
                Log.Info("Updater: Download was completed signal");
                return true;
            }

            return true;
        }

        private bool WaitEvent()
        {
            // build the event array
            var handles = new WaitHandle[]
            {
                this._exitHandle,
                this.forceUpdateCheckHandle,
            };

            // wait for any
            var i = WaitHandle.WaitAny(handles, NextUpdateInterval);
            if (i == WaitHandle.WaitTimeout)
            {
                Log.Info(
                    "Updater: {0} minutes are over",
                    CheckFrequency.TotalMinutes);
                return true;
            }

            // check the exit hadnle
            if (i == 0)
            {
                Log.Info("Updater: Got exit signal");
                return false;
            }

            // check an other check needed
            if (i == 1)
            {
                Log.Info("Updater: Got force update check signal");
                return true;
            }

            return true;
        }

        private NextUpdateAction OnUpdateDetected(AppCastItem latestVersion)
        {
            // send notification if needed
            var e = new UpdateDetectedEventArgs()
            {
                NextAction = NextUpdateAction.ShowStandardUserInterface,
                ApplicationConfig = config,
                LatestVersion = latestVersion,
            };

            UpdateDetected.SafeRaiseEvent(this, e);
            return e.NextAction;
        }

        /// <summary>
        /// This method will be executed as worker thread
        /// </summary>
        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            AppCastItem latestVersion;
            var isInitialCheck = IsInitialCheck;

            // start our lifecycles
            do
            {
                // report status
                if (!isInitialCheck)
                {
                    Log.Info("Updater: Initial check prohibited, going to wait");
                    isInitialCheck = true;
                    goto WaitSection;
                }

                // report status
                Log.Info("Starting update loop...");

                // check if update is required
                latestVersion = GetVersionUpdated();
                if (latestVersion == null)
                {
                    goto WaitSection;
                }

                BeginUpdate(latestVersion);

                // check results
                switch (OnUpdateDetected(latestVersion))
                {
                    case NextUpdateAction.PerformUpdateUnattended:
                        Log.Info("Updater: Unattended update whished from consumer");
                        EnableSilentMode = true;
                        break;
                    case NextUpdateAction.ProhibitUpdate:
                        Log.Info("Updater: Update prohibited from consumer");
                        break;
                    case NextUpdateAction.ShowStandardUserInterface:
                    default:
                        Log.Info("Updater: Standard UI update whished from consumer");
                        break;
                }

            WaitSection:
                // report wait statement
                Log.Info(
                    "Sleeping for an other {0} minutes, exit event or force update check event",
                    CheckFrequency.TotalMinutes);
            } while(WaitEvent());

            // reset the islooping handle
            this._loopingHandle.Reset();
        }
    }
}
