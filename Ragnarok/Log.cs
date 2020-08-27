using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

using NLog;
using NLog.Config;

namespace Ragnarok
{
    /// <summary>
    /// ログの出力を行うクラスです。
    /// </summary>
    /// <remarks>
    /// NLogを使っています。
    /// </remarks>
    public static class Log
    {
        private static ConcurrentDictionary<string, object> targetDic =
             new ConcurrentDictionary<string, object>();
        private static readonly Logger logger = null;

        /// <summary>
        /// 出力対象となるコントロールとその名前を追加。
        /// </summary>
        public static void AddTarget<T>(string name, T target)
            where T : class
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            targetDic.AddOrUpdate(name, target, (_, __) => target);
        }

        /// <summary>
        /// 出力対象となるコントロールを削除。
        /// </summary>
        public static void RemoveTarget(string name)
        {
            targetDic.TryRemove(name, out object value);
        }

        /// <summary>
        /// 名前から出力対象となるコントロールを探します。
        /// </summary>
        public static T FindTarget<T>(string name)
            where T : class
        {
            if (!targetDic.TryGetValue(name, out object target))
            {
                return null;
            }

            return target as T;
        }

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

                // ロガーを作成する前に、必要なオブジェクトを
                // 読み込んでおきます。
                /*TargetFactory.AddTargetsFromAssembly(thisAsm, "");
                LayoutFactory.AddLayoutsFromAssembly(thisAsm, "");
                LayoutRendererFactory.AddLayoutRenderersFromAssembly(thisAsm, "");*/

                // 実行ファイルと同じパスからappname.exe.nlogやNLog.configを検索します。
                var configFileNames = new string[]
                {
                    Path.GetFileName(thisAsm.CodeBase) + ".nlog",
                    "NLog.config",
                };

                var basePath = Path.GetDirectoryName(thisAsm.Location);
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
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

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
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

            Log.Fatal(
                "{0}: {1}",
                logObj?.LogName, message);
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void FatalException(Exception ex, string format,
                                          params object[] args)
        {
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

            if (logger != null)
            {
                logger.Fatal(ex, message);
            }
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void FatalException(ILogObject logObj, Exception ex,
                                          string format, params object[] args)
        {
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

            Log.FatalException(ex,
                "{0}: {1}",
                logObj?.LogName, message);
        }
        #endregion

        #region Error
        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void Error(string format, params object[] args)
        {
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

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
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

            Log.Error(
                "{0}: {1}",
                logObj?.LogName, message);
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void ErrorException(Exception ex, string format,
                                          params object[] args)
        {
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

            if (logger != null)
            {
                logger.Error(ex, message);
            }
        }

        /// <summary>
        /// エラー出力を行います。
        /// </summary>
        public static void ErrorException(ILogObject logObj, Exception ex,
                                          string format, params object[] args)
        {
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

            Log.ErrorException(ex,
                "{0}: {1}",
                logObj?.LogName, message);
        }
        #endregion

        #region Info
        /// <summary>
        /// 情報メッセージを出力します。
        /// </summary>
        public static void Info(string format, params object[] args)
        {
            var message = (
                args != null && args.Length > 0 ?
                string.Format(CultureInfo.CurrentCulture, format, args) :
                format);

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
            var message = (
                args != null && args.Length > 0 ?
                string.Format(CultureInfo.CurrentCulture, format, args) :
                format);

            if (logger != null)
            {
                logger.Info(
                    "{0}: {1}",
                    logObj?.LogName, message);
            }
        }
        #endregion

        #region Debug
        /// <summary>
        /// デバッグメッセージを出力します。
        /// </summary>
        public static void Debug(string format, params object[] args)
        {
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

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
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

            if (logger != null)
            {
                logger.Debug(
                    "{0}: {1}",
                    logObj?.LogName, message);
            }
        }
        #endregion

        #region Trace
        /// <summary>
        /// トレースメッセージを出力します。
        /// </summary>
        public static void Trace(string format, params object[] args)
        {
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

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
            var message = string.Format(CultureInfo.CurrentCulture, format, args);

            if (logger != null)
            {
                logger.Trace(
                    "{0}: {1}",
                    logObj?.LogName, message);
            }
        }
        #endregion
    }
}
