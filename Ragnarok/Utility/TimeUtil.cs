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

        /// <summary>
        /// <paramref name="self"/>がMinValueやMaxValueでないなら真を返します。
        /// </summary>
        public static bool IsNormal(this DateTime self)
        {
            return self != DateTime.MinValue && self != DateTime.MaxValue;
        }

        /// <summary>
        /// <paramref name="self"/>がMinValueやMaxValueでないなら真を返します。
        /// </summary>
        public static bool IsNormal(this TimeSpan self)
        {
            return (self != TimeSpan.MinValue && self != TimeSpan.MaxValue);
        }

        /// <summary>
        /// TimeSpanのミリ秒部分を<paramref name="millis"/> / 1000 にします。
        /// </summary>
        public static TimeSpan MillisecondsTo(this TimeSpan self, int millis)
        {
            if (self == TimeSpan.MinValue || self == TimeSpan.MaxValue)
            {
                return self;
            }

            return TimeSpan.FromSeconds(
                Math.Floor(self.TotalSeconds) + ((double)millis / 1000.0));
        }

        /// <summary>
        /// TimeSpanのミリ秒部分を０にします。
        /// </summary>
        public static TimeSpan MillisecondsToZero(this TimeSpan self)
        {
            return MillisecondsTo(self, 0);
        }
    }
}
