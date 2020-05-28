using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok
{
    /// <summary>
    /// Enumの便利メソッドを定義します。
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// Enumが指定のフラグを持っているか調べます。
        /// </summary>
        /// <remarks>
        /// 古い.NETだとEnum.HasFlagが使えないため。
        /// </remarks>
        public static bool HasFlag<T>(this T value, T flag)
            where T : struct
        {
            return (((int)(object)value & (int)(object)flag) != 0);
        }

        /// <summary>
        /// <typeparam name="T"/>型の列挙子の値をすべて取得します。
        /// </summary>
        public static IEnumerable<T> GetValues<T>()
            where T : struct
        {
            return Enum.GetValues(typeof(T)).OfType<T>();
        }

        /// <summary>
        /// <typeparam name="T"/>型の列挙子の名前をすべて取得します。
        /// </summary>
        public static IEnumerable<string> GetNames<T>()
            where T : struct
        {
            return Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// 列挙子の各値をEnumWrapperにラップして返します。
        /// </summary>
        public static IEnumerable<EnumWrapper<T>> GetWrappedValues<T>()
            where T : struct
        {
            return from v in GetValues<T>()
                   select new EnumWrapper<T>(v);
        }

        /// <summary>
        /// <paramref name="value"/>が正しく定義された値か調べます。
        /// </summary>
        public static bool IsDefined<T>(T value)
            where T : struct
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// 列挙型の名前から値を取得します。
        /// </summary>
        public static T Parse<T>(string value)
            where T : struct
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// 列挙型の名前から値を取得します。
        /// </summary>
        public static T Parse<T>(string value, bool ignoreCase)
            where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>
        /// 列挙値の<see ref="TAttribute"/>属性を取得します。
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(object value)
            where TAttribute : Attribute
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // 列挙型の値から、その値のリフレクション情報を得ます。
            // なんかめんどくさい。。。
            var enumType = value.GetType();
            var enumName = Enum.GetName(enumType, value);
            if (string.IsNullOrEmpty(enumName))
            {
                return null;
            }

            // 説明文を取得します。
            var enumInfo = enumType.GetField(enumName);
            var attributes = enumInfo.GetCustomAttributes(
                typeof(TAttribute),
                false);
            if (attributes.Length == 0)
            {
                return null;
            }

            return (TAttribute)attributes[0];
        }

        /// <summary>
        /// 列挙値のラベルを取得します。
        /// </summary>
        public static string GetLabel(object value)
        {
            var labelAttribute = GetAttribute<LabelAttribute>(value);
            if (labelAttribute != null)
            {
                return labelAttribute.Label;
            }

            return null;
        }

        /// <summary>
        /// 列挙値の説明文を取得します。
        /// </summary>
        public static string GetDescription(object value)
        {
            var attribute = GetAttribute<DescriptionAttribute>(value);
            if (attribute == null)
            {
                return null;
            }

            return attribute.Description;
        }
    }
}
