using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Ragnarok.Update
{
    /// <summary>
    /// ユーティリティクラスです。
    /// </summary>
    public static class UpdateUtil
    {
#if false
        /// <summary>
        /// This method checks if an update is required. During this process the appcast
        /// will be downloaded and checked against the reference assembly. Ensure that
        /// the calling process has access to the internet and read access to the 
        /// reference assembly. This method is also called from the background loops.
        /// </summary>
        public bool IsUpdateRequired(Configuration config,
                                     out AppCastItem latestVersion)
        {
            Log.Info("Updater: Downloading and checking appcast");

            latestVersion = GetLatestVersion(config);
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

            // set the last check time
            Log.Info("Updater: Touch the last check timestamp.");

            // check if the available update has to be skipped
            if (latestVersion.Version.Equals(config.SkipThisVersion))
            {
                Log.Info(
                    "Updater: Latest update has to be skipped (user decided to skip version {0})",
                    config.SkipThisVersion);
                return false;
            }

            // check if the version will be the same then the installed version
            var v1 = new Version(config.InstalledVersion);
            var v2 = new Version(latestVersion.Version);

            if (v2 <= v1)
            {
                Log.Info(
                    "Updater: Installed version is valid, no update needed. ({0})",
                    config.InstalledVersion);
                return false;
            }

            // ok we need an update
            return true;
        }
#endif

        /// <summary>
        /// 実際の更新処理を外部exeを呼び出すことで行います。
        /// </summary>
        public static void ExecutePack(string exeFileName, string zipFileName)
        {
            var workingDir = Environment.CurrentDirectory;

            // start update helper
            // 0. this process' id
            // 1. zip file path
            // 2. the top directory of this program
            // 3. the path of the restart program
            var startInfo = new ProcessStartInfo()
            {
                FileName = exeFileName,
                Arguments = string.Format(
                    @"{0} ""{1}"" ""{2}"" {3}",
                    Process.GetCurrentProcess().Id,
                    zipFileName,
                    workingDir,
                    Environment.CommandLine),
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            Process.Start(startInfo);
        }
    }
}
