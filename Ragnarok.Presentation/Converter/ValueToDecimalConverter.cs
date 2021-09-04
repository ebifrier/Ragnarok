using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace Ragnarok.Presentation.Converter
{
    /// <summary>
    /// 値をdecimal型に直します。
    /// </summary>
    public class ValueToDecimalConverter : IValueConverter
    {
        public static ValueToDecimalConverter Default = new ValueToDecimalConverter();

        /// <summary>
        /// 変換元の型を取得または設定します。
        /// </summary>
        public Type SourceType
        {
            get;
            set;
        }

        /// <summary>
        /// decimal値に変換します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "decimal型の変換に失敗しました。");

                return value;
            }
        }

        /// <summary>
        /// 変換元の型に戻します。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, SourceType ?? targetType);
        }
    }
}
