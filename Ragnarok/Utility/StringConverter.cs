using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 数値の種類です。
    /// </summary>
    public enum NumberType
    {
        /// <summary>
        /// 半角数字です。
        /// </summary>
        Normal,
        /// <summary>
        /// 全角数字です。
        /// </summary>
        Big,
        /// <summary>
        /// 漢数字です。
        /// </summary>
        Kanji,
        /// <summary>
        /// 段位などです。(1級 → 初級)
        /// </summary>
        Grade,
    };

    /// <summary>
    /// 文字列から各種オブジェクトに変換します。
    /// </summary>
    public static class StringConverter
    {
        private static readonly string[] NormalNumberTable = new string[]
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "-",
        };

        private static readonly string[] BigNumberTable = new string[]
        {
            "０", "１", "２", "３", "４", "５", "６", "７", "８", "９", "－",
        };

        private static readonly string[] KanjiNumberTable = new string[]
        {
            "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "－",
        };

        /// <summary>
        /// 数字を文字列に変換します。
        /// </summary>
        public static string ConvertInt(NumberType type, int num)
        {
            string[] numberTable;
            var result = new StringBuilder();

            switch (type)
            {
                case NumberType.Normal:
                    numberTable = NormalNumberTable;
                    break;
                case NumberType.Big:
                    numberTable = BigNumberTable;
                    break;
                case NumberType.Kanji:
                    numberTable = KanjiNumberTable;
                    break;
                case NumberType.Grade:
                    numberTable = NormalNumberTable;
                    if (num == 1)
                    {
                        return "初";
                    }
                    break;
                default:
                    return null;
            }

            if (num == 0)
            {
                return numberTable[0];
            }

            // 負数ならマイナス記号をつけます。
            var original = num;
            num = (num > 0 ? num : -num);

            while (num > 0)
            {
                result.Insert(0, numberTable[num % 10]);
                num /= 10;
            }

            if (original < 0)
            {
                result.Insert(0, numberTable[10]);
            }

            return result.ToString();
        }

        /// <summary>
        /// このクラスで変換可能な型か調べます。
        /// </summary>
        public static bool CanConvert(Type type)
        {
            return (
                type == typeof(string) || type == typeof(bool) ||
                type == typeof(char) || type == typeof(decimal) ||
                type == typeof(byte) || type == typeof(sbyte) ||

                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(int) || type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong) ||
                type == typeof(float) || type == typeof(double) ||
                   
                type == typeof(DateTime) || type == typeof(TimeSpan) ||
                type == typeof(Guid) || type.IsEnum);
        }

        /// <summary>
        /// 文字列を指定の型に変換します。
        /// </summary>
        public static object Convert(Type type, string value)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type == typeof(string))
            {
                return value;
            }
            else if (type == typeof(bool))
            {
                return bool.Parse(value);
            }
            else if (type == typeof(char))
            {
                return Char.Parse(value);
            }
            else if (type == typeof(byte))
            {
                return Byte.Parse(value);
            }
            else if (type == typeof(sbyte))
            {
                return SByte.Parse(value);
            }
            else if (type == typeof(decimal))
            {
                return Decimal.Parse(value);
            }

            else if (type == typeof(ushort))
            {
                return UInt16.Parse(value);
            }
            else if (type == typeof(uint))
            {
                return UInt32.Parse(value);
            }
            else if (type == typeof(ulong))
            {
                return UInt64.Parse(value);
            }

            else if (type == typeof(short))
            {
                return Int16.Parse(value);
            }
            else if (type == typeof(int))
            {
                return Int32.Parse(value);
            }
            else if (type == typeof(long))
            {
                return Int64.Parse(value);
            }

            else if (type == typeof(float))
            {
                return Single.Parse(value);
            }
            else if (type == typeof(double))
            {
                return Double.Parse(value);
            }

            else if (type == typeof(DateTime))
            {
                return DateTime.Parse(value);
            }
            else if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(value);
            }
            else if (type == typeof(Guid))
            {
                return new Guid(value);
            }

            else if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }

            throw new InvalidOperationException(
                string.Format("{0}: 指定の型は変換できません。", type));
        }
    }
}
