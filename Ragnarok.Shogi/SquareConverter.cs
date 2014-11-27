using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

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
            if (sourceType == typeof(string))
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
                var list = str.Split(new char[] { ',' });
                if (list == null || list.Count() != 2)
                {
                    throw new FormatException(
                        "Square型への変換に失敗しました。");
                }

                var file = int.Parse(list[0]);
                var rank = int.Parse(list[1]);
                return new Square(file, rank);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Square型を変換できるか調べます。
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
        /// Square型を文字列に変換します。
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context,
                                         CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var square = (Square)value;

                return string.Format("{0},{1}", square.File, square.Rank);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
