using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// Pointdクラスと文字列との変換を行います。
    /// </summary>
    public sealed class PointdConverter : TypeConverter
    {
        /// <summary>
        /// 他の値をPointdに変換できるか調べます。
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
        /// 文字列型をPointd型に変換します。
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo culture, object value)
        {
            var str = value as string;
            if (str != null)
            {
                return Pointd.Parse(str);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Point3d型を変換できるか調べます。
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
        /// Pointd型を文字列に変換します。
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
