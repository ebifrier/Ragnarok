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
    /// ログ出力用のオブジェクトです。
    /// </summary>
    public interface ILogObject
    {
        /// <summary>
        /// ログ出力用の名前を取得します。
        /// </summary>
        string LogName { get; }
    }

    /// <summary>
    /// ログの出力を行うクラスです。
    /// </summary>
    /// <remarks>
    /// NLogを使っています。
    /// </remarks>
    public static class Log
    {
        private static readonly ConcurrentDictionary<string, object> targetDic = new ();
        private static readonly Logger logger;

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
                FatalException(ex,
                    "未処理の例外が発生しました。");
            }
            else
            {
                Fatal(
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
                var baseDir = AppContext.BaseDirectory;
                var asm = Assembly.GetEntryAssembly();

                // 実行ファイルと同じパスからappname.exe.nlogやNLog.configを検索します。
                var configFileNames = new string[]
                {
                    $"{asm.ManifestModule.Name}.nlog",
                    "NLog.config",
                };

                foreach (var configFile in configFileNames)
                {
                    var filePath = Path.Combine(baseDir, configFile);
                    if (!File.Exists(filePath))
                    {
                        continue;
                    }

                    LogManager.Configuration =
                        new XmlLoggingConfiguration(filePath);
                    break;
                }

                // 外部ファイルがない場合は埋め込まれた設定ファイルを使います。
                if (LogManager.Configuration == null)
                {
                    var ragAsm = Assembly.GetExecutingAssembly();
                    var config = Util.GetResourceString(ragAsm, "Ragnarok.NLog.default");
                    LogManager.Configuration =
                        XmlLoggingConfiguration.CreateFromXmlString(config);
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
