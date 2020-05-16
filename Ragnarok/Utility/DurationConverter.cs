using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ragnarok.Utility
{
    /// <summary>
    /// TimeSpanクラスの'Forever'による指定を可能にします。
    /// </summary>
    public sealed class DurationConverter : TypeConverter
    {
        /// <summary>
        /// 他の値をTimeSpanに変換できるか調べます。
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
        /// 文字列型をTimeSpan型に変換します。
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo culture, object value)
        {
            var str = value as string;
            if (str != null)
            {
                if (str == "Forever")
                {
                    return TimeSpan.MaxValue;
                }

                var span = TimeSpan.Parse(str, culture);
                if (span < TimeSpan.Zero)
                {
                    throw new FormatException(
                        "DurationConverter使用時にTimeSpanの値がマイナスになりました。");
                }

                return span;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// TimeSpan型を変換できるか調べます。
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
        /// TimeSpan型を文字列に変換します。
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context,
                                         CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (typeof(string).Equals(destinationType))
            {
                return $"{value}";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
