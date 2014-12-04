using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// Color4bクラスと文字列との変換を行います。
    /// </summary>
    public sealed class Color4bConverter : TypeConverter
    {
        /// <summary>
        /// 他の値をColor4bに変換できるか調べます。
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                            Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// 文字列型をColor4b型に変換します。
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo culture, object value)
        {
            var str = value as string;
            if (str != null)
            {
                return Color4b.Parse(str);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Color4b型を変換できるか調べます。
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context,
                                          Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Color4b型を文字列に変換します。
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context,
                                         CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
