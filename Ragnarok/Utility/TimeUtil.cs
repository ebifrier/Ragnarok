using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Reflection;
using System.Globalization;

namespace Ragnarok.Utility
{
    /// <summary>
    /// Unix時間を<see cref="DateTime"/>に変換します。
    /// </summary>
    public static class TimeUtil
    {
        /// <summary>
        /// Unix時間の基準時刻です。
        /// </summary>
        private static readonly DateTime Epoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Unix時間をDateTimeに変換します。
        /// </summary>
        public static DateTime UnixTimeToDateTime(string timeText)
        {
            double seconds = 0.0;
            if (!double.TryParse(timeText, out seconds))
            {
                return DateTime.MinValue; // エラー
            }

            /*// 失敗時は例外を投げます。
            double seconds = double.Parse(timeText);*/

            return UnixTimeToDateTime(seconds);
        }

        /// <summary>
        /// Unix時間をDateTimeに変換します。
        /// </summary>
        public static DateTime UnixTimeToDateTime(double seconds)
        {
            // 基準時刻に加算し、ローカル時刻に直します。
            var utc = Epoch.AddSeconds(seconds);

            return utc.ToLocalTime();
        }

        /// <summary>
        /// DateTimeをUnix時間に変換します。
        /// </summary>
        public static double DateTimeToUnixTime(DateTime date)
        {
            var univ = date.ToUniversalTime();
            var diff = univ - Epoch;

            return diff.TotalSeconds;
        }

        /// <summary>
        /// 時刻をRFC3339形式に直します。
        /// </summary>
        public static string DateTimeToRFC3339(DateTime date)
        {
            return XmlConvert.ToString(
                date, XmlDateTimeSerializationMode.Utc);
        }
    }
}
