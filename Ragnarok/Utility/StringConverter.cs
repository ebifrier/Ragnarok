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
                throw new ArgumentNullException("type");
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
                return Byte.Parse(value);
            }
            else if (typeof(sbyte).Equals(type))
            {
                return SByte.Parse(value);
            }
            else if (typeof(decimal).Equals(type))
            {
                return Decimal.Parse(value);
            }

            else if (typeof(ushort).Equals(type))
            {
                return UInt16.Parse(value);
            }
            else if (typeof(uint).Equals(type))
            {
                return UInt32.Parse(value);
            }
            else if (typeof(ulong).Equals(type))
            {
                return UInt64.Parse(value);
            }

            else if (typeof(short).Equals(type))
            {
                return Int16.Parse(value);
            }
            else if (typeof(int).Equals(type))
            {
                return Int32.Parse(value);
            }
            else if (typeof(long).Equals(type))
            {
                return Int64.Parse(value);
            }

            else if (typeof(float).Equals(type))
            {
                return Single.Parse(value);
            }
            else if (typeof(double).Equals(type))
            {
                return Double.Parse(value);
            }

            else if (typeof(DateTime).Equals(type))
            {
                return DateTime.Parse(value);
            }
            else if (typeof(TimeSpan).Equals(type))
            {
                return TimeSpan.Parse(value);
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
                string.Format("{0}: 指定の型は変換できません。", type));
        }
    }
}
