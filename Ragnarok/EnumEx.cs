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
    public static class EnumEx
    {
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
        /// 列挙値の<see ref="LabelDescriptionAttribute"/>属性を取得します。
        /// </summary>
        public static LabelDescriptionAttribute GetEnumAttribute(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
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
                typeof(LabelDescriptionAttribute),
                false);
            if (attributes.Length == 0)
            {
                return null;
            }

            return (LabelDescriptionAttribute)attributes[0];
        }

        /// <summary>
        /// 列挙値の説明文を取得します。
        /// </summary>
        public static string GetEnumDescription(object value)
        {
            var attribute = GetEnumAttribute(value);
            if (attribute == null)
            {
                return null;
            }

            return attribute.Description;
        }

        /// <summary>
        /// 列挙値のラベルを取得します。
        /// </summary>
        public static string GetEnumLabel(object value)
        {
            var attribute = GetEnumAttribute(value);
            if (attribute == null)
            {
                return null;
            }

            return attribute.Label;
        }
    }
}
