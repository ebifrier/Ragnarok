using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Ragnarok.UpdatePack
{
    /// <summary>
    /// ユーティリティメソッドが含まれています。
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// ログなどの初期化処理を行います。
        /// </summary>
        public static void Initialize()
        {
            // ログをファイルに出力します。
            // これがないと更新失敗の原因が分かりません。
            Trace.Listeners.Add(new TextWriterTraceListener("update.log"));

            Trace.AutoFlush = true;
        }

        /// <summary>
        /// エラーを記録します。
        /// </summary>
        public static void TraceError(string format, params object[] args)
        {
            string message = string.Format(format, args);

            Trace.TraceError(
                string.Format(
                    "{0}",
                    message));
        }

        /// <summary>
        /// エラーを記録します。
        /// </summary>
        public static void TraceError(Exception ex, string format,
                                      params object[] args)
        {
            string message = string.Format(format, args);

            Trace.TraceError(
                string.Format(
                    "{1}{0}" +
                    "　　詳細: {2}{0}" +
                    "{3}{0}",
                    Environment.NewLine,
                    message,
                    ex.Message,
                    ex.StackTrace));
        }
    }
}
