using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using Ragnarok.Utility;

namespace Ragnarok.Presentation.Converter
{
    /// <summary>
    /// 整数値を全角数字の文字列に変換します。
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class IntToFullWidthStringConverter : IValueConverter
    {
        /// <summary>
        /// 整数値を全角数字の文字列に変換します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                var ivalue = (int)value;

                return StringConverter.ConvertInt(NumberType.Big, ivalue);
            }
            catch (InvalidCastException ex)
            {
                Log.ErrorException(ex,
                    "IntToFullWidthString: 大文字に変換できませんでした。");

                return null;
            }
        }

        /// <summary>
        /// 実装されていません。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
