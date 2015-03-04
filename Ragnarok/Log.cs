using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using NLog;
using NLog.Config;

namespace Ragnarok
{
    using Utility;

    /// <summary>
    /// ログの出力を行うクラスです。
    /// </summary>
    /// <remarks>
    /// NLogを使っています。
    /// </remarks>
    public static class Log
    {
        private static readonly Logger logger = null;

        /// <summary>
        /// 未処理の例外が発生したときに呼ばれます。
        /// </summary>
        private static void UnhandledException(object sender,
                                               UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (ex != null)
            {
#if MONO
                // MONOだとアプリ終了時に防ぐのが難しいエラーが出る。
                if (ex.Message ==
                    "A null reference or invalid value was found [GDI+ status: InvalidParameter]")
                {
                    return;
                }
#endif
                Log.FatalException(ex,
                    "未処理の例外が発生しました。");
            }
            else
            {
                Log.Fatal(
                    "未処理の例外が発生しました。");
            }
        }

        /// <summary>
        /// ロガーを初期化します。
        /// </summary>
        static Log()
        {
            try
            {
                var thisAsm = Assembly.GetExecutingAssembly();
                var entryAsm = Assembly.GetEntryAssembly();

                // ロガーを作成する前に、必要なオブジェクトを
                // 読み込んでおきます。
                TargetFactory.AddTargetsFromAssembly(thisAsm, "");
                LayoutFactory.AddLayoutsFromAssembly(thisAsm, "");
                LayoutRendererFactory.AddLayoutRenderersFromAssembly(thisAsm, "");

                // 実行ファイルと同じパスからappname.exe.nlogやNLog.configを検索します。
                var configFileNames = new string[]
                {
                    Path.GetFileName(entryAsm.CodeBase) + ".nlog",
                    "NLog.config",
                };

                var basePath = Path.GetDirectoryName(entryAsm.Location);
                foreach (var configFile in configFileNames)
                {
                    var filename = Path.Combine(basePath, configFile);
                    if (!File.Exists(filename))
                    {
                        continue;
                    }

                    LogManager.Configuration =
                        new XmlLoggingConfiguration(filename);
                    break;
                }

                logger = LogManager.GetCurrentClassLogger();

                // 最後に未処理の例外ハンドラを追加します。
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            }
            catch
            {
                // どうしよう。。。
            }
        }

        #region Fatal
        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void Fatal(string format, params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.Fatal(message);
            }
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void Fatal(ILogObject logObj, string format,
                                 params object[] args)
        {
            var message = string.Format(format, args);

            Log.Fatal(
                "{0}: {1}",
                logObj.LogName, message);
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void FatalException(Exception ex, string format,
                                          params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.FatalException(message, ex);
            }
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void FatalException(ILogObject logObj, Exception ex,
                                          string format, params object[] args)
        {
            var message = string.Format(format, args);

            Log.FatalException(ex,
                "{0}: {1}",
                logObj.LogName, message);
        }
        #endregion

        #region Error
        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void Error(string format, params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.Error(message);
            }
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void Error(ILogObject logObj, string format,
                                 params object[] args)
        {
            var message = string.Format(format, args);

            Log.Error(
                "{0}: {1}",
                logObj.LogName, message);
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void ErrorException(Exception ex, string format,
                                          params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.ErrorException(message, ex);
            }
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void ErrorException(ILogObject logObj, Exception ex,
                                          string format, params object[] args)
        {
            var message = string.Format(format, args);

            Log.ErrorException(ex,
                "{0}: {1}",
                logObj.LogName, message);
        }
        #endregion

        #region Info
        /// <summary>
        /// 情報メッセージを出力します。
        /// </summary>
        public static void Info(string format, params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.Info(message);
            }
        }

        /// <summary>
        /// 情報メッセージを出力します。
        /// </summary>
        public static void Info(ILogObject logObj, string format,
                                params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.Info(
                    "{0}: {1}",
                    logObj.LogName, message);
            }
        }
        #endregion

        #region Debug
        /// <summary>
        /// デバッグメッセージを出力します。
        /// </summary>
        public static void Debug(string format, params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.Debug(message);
            }
        }

        /// <summary>
        /// デバッグ出力を行います。
        /// </summary>
        public static void Debug(ILogObject logObj, string format,
                                 params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.Debug(
                    "{0}: {1}",
                    logObj.LogName, message);
            }
        }
        #endregion

        #region Trace
        /// <summary>
        /// トレースメッセージを出力します。
        /// </summary>
        public static void Trace(string format, params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.Trace(message);
            }
        }

        /// <summary>
        /// トレース出力を行います。
        /// </summary>
        public static void Trace(ILogObject logObj, string format,
                                 params object[] args)
        {
            var message = string.Format(format, args);

            if (logger != null)
            {
                logger.Trace(
                    "{0}: {1}",
                    logObj.LogName, message);
            }
        }
        #endregion
    }
}
