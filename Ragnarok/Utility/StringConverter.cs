using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 文字列から各種オブジェクトに変換します。
    /// </summary>
    public static class StringConverter
    {
        /// <summary>
        /// このクラスで変換可能な型か調べます。
        /// </summary>
        public static bool CanConvert(Type type)
        {
            if (ReferenceEquals(type, null))
            {
                throw new ArgumentNullException(nameof(type));
            }

            return (
                typeof(string).Equals(type) || typeof(bool).Equals(type) ||
                typeof(char).Equals(type) || typeof(decimal).Equals(type) ||
                typeof(byte).Equals(type) || typeof(sbyte).Equals(type) ||

                typeof(short).Equals(type) || typeof(ushort).Equals(type) ||
                typeof(int).Equals(type) || typeof(uint).Equals(type) ||
                typeof(long).Equals(type) || typeof(ulong).Equals(type) ||
                typeof(float).Equals(type) || typeof(double).Equals(type) ||

                typeof(DateTime).Equals(type) || typeof(TimeSpan).Equals(type) ||
                typeof(Guid).Equals(type) || type.IsEnum);
        }

        /// <summary>
        /// 文字列を指定の型に変換します。
        /// </summary>
        public static object Convert(Type type, string value)
        {
            if (ReferenceEquals(type, null))
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (typeof(string).Equals(type))
            {
                return value;
            }
            else if (typeof(bool).Equals(type))
            {
                return bool.Parse(value);
            }
            else if (typeof(char).Equals(type))
            {
                return Char.Parse(value);
            }
            else if (typeof(byte).Equals(type))
            {
                return Byte.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(sbyte).Equals(type))
            {
                return SByte.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(decimal).Equals(type))
            {
                return Decimal.Parse(value, CultureInfo.CurrentCulture);
            }

            else if (typeof(ushort).Equals(type))
            {
                return UInt16.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(uint).Equals(type))
            {
                return UInt32.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(ulong).Equals(type))
            {
                return UInt64.Parse(value, CultureInfo.CurrentCulture);
            }

            else if (typeof(short).Equals(type))
            {
                return Int16.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(int).Equals(type))
            {
                return Int32.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(long).Equals(type))
            {
                return Int64.Parse(value, CultureInfo.CurrentCulture);
            }

            else if (typeof(float).Equals(type))
            {
                return Single.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(double).Equals(type))
            {
                return Double.Parse(value, CultureInfo.CurrentCulture);
            }

            else if (typeof(DateTime).Equals(type))
            {
                return DateTime.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(TimeSpan).Equals(type))
            {
                return TimeSpan.Parse(value, CultureInfo.CurrentCulture);
            }
            else if (typeof(Guid).Equals(type))
            {
                return new Guid(value);
            }

            else if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }

            throw new InvalidOperationException(
                $"{type}: 指定の型は変換できません。");
        }
    }
}
