using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;

namespace Ragnarok.UpdatePack
{
    /// <summary>
    /// 自動更新時にexeを交換するためのプログラムです。
    /// </summary>
    /// <remarks>
    /// 注意点
    /// ・ユーザーライブラリに依存するとそのライブラリをコピーできなくなります。
    /// ・このプログラム自体の入れ替えは失敗します。
    /// </remarks>
    class Program
    {
        private static int processId;
        private static string zipFilePath;
        private static string topDir;
        private static string restartExePath;

        /// <summary>
        /// 自動更新時にダウンロードしたzipからexeを交換します。
        /// </summary>
        /// <param name="args">
        /// 0. このプログラムを呼び出したプロセスのID(終了させる)
        /// 1. 解凍するzipファイルのパス
        /// 2. 元プログラムのトップディレクトリ
        /// 3. 自動起動するファイルのパス
        /// </param>
        static void Main(string[] args)
        {
            Utility.Initialize();

            int status = GetOptions(args);
            if (status != 0)
            {
                return;
            }

            // やること
            // 1. zipファイルの解凍
            // 2. 元プログラムの終了
            // 3. ファイルのコピー
            // 4. プログラムの再起動

            Trace.TraceInformation("Start Unzip");
            string unzipDir = Unzip(zipFilePath);
            if (string.IsNullOrEmpty(unzipDir))
            {
                return;
            }
            Trace.TraceInformation("End Unzip");

            Trace.TraceInformation("Start WaitToStopProcess");
            if (WaitToStopProcess(processId) != 0)
            {
                return;
            }
            Trace.TraceInformation("End WaitToStopProcess");

            Trace.TraceInformation("Start CopyFiles");
            if (CopyFiles(unzipDir, topDir) != 0)
            {
                return;
            }
            Trace.TraceInformation("End CopyFiles");

            // コピー終了後に不要なファイルを削除します。
            Trace.TraceInformation("Delete the zip file");
            DeleteFile(zipFilePath);

            Trace.TraceInformation("Delete the unzip directory");
            DeleteDirectory(unzipDir);

            // 再起動用のファイルがある場合は、それを開始します。
            if (!string.IsNullOrEmpty(restartExePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = restartExePath,
                    WorkingDirectory = Path.GetDirectoryName(restartExePath),
                };

                Trace.TraceInformation("Restart the program");
                Process.Start(startInfo);
            }
        }

        /// <summary>
        /// プログラム引数を取得します。
        /// </summary>
        private static int GetOptions(string[] args)
        {
            if (args.Length < 4)
            {
                Utility.TraceError("引数の数が少なすぎます。");
                return -1;
            }

            // このプログラムを呼び出したプロセスのID(終了させる)
            if (!int.TryParse(args[0], out processId))
            {
                Utility.TraceError(
                    "第一引数がプロセスＩＤではありません。({0})",
                    args[0]);
                return -1;
            }
            Trace.TraceInformation("ProcessId = {0}", processId);

            // zipファイル名
            zipFilePath = args[1];
            if (!File.Exists(zipFilePath))
            {
                Utility.TraceError(
                    "第二引数のzipファイルが存在しません。({0})",
                    zipFilePath);
                return -1;
            }
            Trace.TraceInformation("ZipFile Path = {0}", zipFilePath);

            // 元プログラムのトップディレクトリ
            topDir = args[2];
            if (!Directory.Exists(topDir))
            {
                Utility.TraceError(
                    "第三引数のディレクトリが存在しません。({0})",
                    topDir);
                return -1;
            }
            Trace.TraceInformation("Top Directory = {0}", topDir);

            // コピー後の自動起動実行ファイル
            if (args.Length > 3)
            {
                restartExePath = args[3];
                Trace.TraceInformation("Restart Program Path = {0}", restartExePath);
                /*if (!Directory.Exists(restartExePath))
                {
                    Utility.TraceError(
                        "第四引数の実行ファイルが存在しません。({0})",
                        restartExePath);
                    return -1;
                }*/
            }

            return 0;
        }

        /// <summary>
        /// zipファイルを解凍します。
        /// </summary>
        private static string Unzip(string zipFileName)
        {
            try
            {
                string tempDir = Environment.ExpandEnvironmentVariables(
                    "%temp%\\" + Guid.NewGuid() + ".tmp");

                // zipファイルを一時ディレクトリに解凍します。
                Trace.TraceInformation("Extracting zip file to {0}.", tempDir);

                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(zipFileName, tempDir, null);

                return tempDir;
            }
            catch (Exception ex)
            {
                Utility.TraceError(ex,
                    "zipファイルの解凍に失敗しました。");

                return null;
            }
        }

        /// <summary>
        /// 与えられたIDを持つプロセスを検索します。
        /// </summary>
        private static Process GetProcessById(int processId)
        {
            try
            {
                return Process.GetProcessById(processId);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// 元プログラムが停止するまで待ちます。
        /// </summary>
        private static int WaitToStopProcess(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                if (process == null)
                {
                    Utility.TraceError(
                        "ID={0}のプロセスが見つかりませんでした。",
                        processId);

                    return -1;
                }

                Trace.TraceInformation("Kill process");
                process.Kill();

                // 最大、10秒待ちます。
                if (!process.WaitForExit(10 * 1000))
                {
                    Utility.TraceError(
                        "プロセス(ID={0})の停止に失敗しました。",
                        processId);
                    return -1;
                }

                process.Dispose();
                return 0;
            }
            catch (Exception ex)
            {
                Utility.TraceError(ex,
                    "プロセス(ID={0})を停止できませんでした。",
                    processId);

                return -1;
            }
        }

        /// <summary>
        /// 単体のファイルをコピーします。失敗は握りつぶします。
        /// </summary>
        private static void CopyFile(string sourcePath, string targetPath)
        {
            try
            {
                File.Copy(sourcePath, targetPath, true);
            }
            catch (Exception ex)
            {
                Utility.TraceError(ex,
                    "ファイルのコピーに失敗しました。({0} → {1})",
                    sourcePath, targetPath);
            }
        }

        /// <summary>
        /// ディレクトリを作成します。
        /// </summary>
        private static bool CreateDirectory(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                return true;
            }

            try
            {
                // ディレクトリがなければ作ります。
                // もし同名ファイルが存在するなど、作成できない場合は
                // ファイルのコピーを諦めます。
                Directory.CreateDirectory(dirPath);

                return true;
            }
            catch (Exception ex)
            {
                Utility.TraceError(ex,
                    "ディレクトリの作成に失敗しました。({0})",
                    dirPath);

                return false;
            }
        }

        /// <summary>
        /// ファイルとディレクトリのコピーを行います。
        /// </summary>
        private static void CopyFilesInternal(string sourceDir, string targetDir)
        {
            // 最初にファイルをすべてコピーします。
            foreach (string filePath in Directory.EnumerateFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string sourcePath = Path.Combine(sourceDir, fileName);
                string targetPath = Path.Combine(targetDir, fileName);

                CopyFile(sourcePath, targetPath);
            }

            // 次にディレクトリのコピーを行います。
            foreach (string dirPath in Directory.EnumerateDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dirPath);
                string sourcePath = Path.Combine(sourceDir, dirName);
                string targetPath = Path.Combine(targetDir, dirName);

                if (CreateDirectory(targetPath))
                {
                    CopyFilesInternal(sourcePath, targetPath);
                }
            }
        }

        /// <summary>
        /// ファイルをコピーします。
        /// </summary>
        private static int CopyFiles(string sourceDir, string targetDir)
        {
            try
            {
                Trace.TraceInformation(
                    "Copy files ({0} → {1})",
                    sourceDir, targetDir);

                CopyFilesInternal(sourceDir, targetDir);
            }
            catch (Exception ex)
            {
                Utility.TraceError(ex,
                    "ファイルのコピーに失敗しました。({0} → {1})",
                    sourceDir, targetDir);
            }

            return 0;
        }

        /// <summary>
        /// ファイルを削除します。
        /// </summary>
        private static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Utility.TraceError(ex,
                    "ファイルの削除に失敗しました。({0})",
                    filePath);
            }
        }

        /// <summary>
        /// ディレクトリを削除します。
        /// </summary>
        private static void DeleteDirectory(string dirPath)
        {
            try
            {
                if (Directory.Exists(dirPath))
                {
                    Directory.Delete(dirPath, true);
                }
            }
            catch (Exception ex)
            {
                Utility.TraceError(ex,
                    "ディレクトリの削除に失敗しました。({0})",
                    dirPath);
            }
        }
    }
}
