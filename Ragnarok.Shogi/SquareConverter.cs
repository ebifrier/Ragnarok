using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// Squareクラスと文字列との変換を行います。
    /// </summary>
    public sealed class SquareConverter : TypeConverter
    {
        /// <summary>
        /// 他の値をSquareに変換できるか調べます。
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                            Type sourceType)
        {
            if (typeof(string).Equals(sourceType))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// 文字列型をSquare型に変換します。
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo culture, object value)
        {
            var str = value as string;
            if (str != null)
            {
                return SquareUtil.Parse(str);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Square型を変換できるか調べます。
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context,
                                          Type destinationType)
        {
            if (typeof(string).Equals(destinationType))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Square型を文字列に変換します。
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context,
                                         CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (typeof(string).Equals(destinationType))
            {
                return value?.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
