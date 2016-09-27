using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico
{
    /// <summary>
    /// getplayerstatusなどの文字列解析時に使います。
    /// </summary>
    public static class StrUtil
    {
        /// <summary>
        /// "0" or "1" をbool型に変換します。
        /// </summary>
        public static bool ToBool(string value, bool defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (value == "1")
            {
                return true;
            }
            else if (value == "0")
            {
                return false;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 文字列を数値に変換します。
        /// </summary>
        public static int ToInt(string value, int defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 文字列を数値に変換します。
        /// </summary>
        public static long ToLong(string value, long defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            long result;
            if (long.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Unix時間形式の文字列をDateTimeに変換します。
        /// </summary>
        public static DateTime ToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DateTime.MinValue;
            }

            return Utility.TimeUtil.UnixTimeToDateTime(value);
        }

        /// <summary>
        /// ユーザーの性別に変換します。
        /// </summary>
        public static Gender ToGender(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Gender.Unknown;
            }

            if (value == "1")
            {
                return Gender.Male;
            }
            else if (value == "2")
            {
                return Gender.Female;
            }

            return Gender.Unknown;
        }

        /// <summary>
        /// 文字列を放送配信コミュニティの種別に変換します。
        /// </summary>
        public static Provider.ProviderType ToProvider(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Provider.ProviderType.Unknown;
            }

            if (value == "community")
            {
                return Provider.ProviderType.Community;
            }
            else if (value == "channel")
            {
                return Provider.ProviderType.Channel;
            }
            else if (value == "official")
            {
                return Provider.ProviderType.Official;
            }

            return Provider.ProviderType.Unknown;
        }
    }
}
