using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace Ragnarok.Utility
{
    /// <summary>
    /// スタックトレースの文字列かなどを行います。
    /// </summary>
    public static class StackTraceUtil
    {
        /// <summary>
        /// メソッドの名前などを文字列に直します。
        /// </summary>
        public static string GetMethodString(MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var args = method.GetParameters()
                .Select(paramInfo =>
                    paramInfo.ParameterType.Name + " " +
                    paramInfo.Name);

            var typeName = method.DeclaringType.Name;
            var argName = string.Join(", ", args.ToArray());
            return $"{typeName}.{method.Name}({argName})";
        }

        /// <summary>
        /// スタックトレースを取得します。
        /// </summary>
        public static IEnumerable<string> ToStackTraceString(StackTrace stackTrace)
        {
            if (stackTrace == null)
            {
                throw new ArgumentNullException(nameof(stackTrace));
            }

            var omitted = false;
            return stackTrace.GetFrames()
                .Where(frame =>
                {
                    if (string.IsNullOrEmpty(frame.GetFileName()))
                    {
                        var oldOmitted = omitted;
                        omitted = true;
                        return !oldOmitted;
                    }
                    else
                    {
                        omitted = false;
                        return true;
                    }
                })
                .Select(frame =>
                {
                    if (string.IsNullOrEmpty(frame.GetFileName()))
                    {
                        return "  場所: 省略されますた";
                    }
                    else
                    {
                        return string.Format(
                            CultureInfo.CurrentCulture, 
                            "  場所: {0}({1}): {2}",
                            frame.GetFileName(),
                            frame.GetFileLineNumber(),
                            GetMethodString(frame.GetMethod()));
                    }
                });
        }

        /// <summary>
        /// スタックトレースを取得します。
        /// </summary>
        public static IEnumerable<string> GetStackTrace(Exception e)
        {
            var stackTrace = new StackTrace(e, true);

            return ToStackTraceString(stackTrace);
        }

        /// <summary>
        /// スタックトレースを取得します。
        /// </summary>
        public static IEnumerable<string> GetStackTrace(int skipFrames)
        {
            var stackTrace = new StackTrace(skipFrames, true);

            return ToStackTraceString(stackTrace);
        }
    }
}
