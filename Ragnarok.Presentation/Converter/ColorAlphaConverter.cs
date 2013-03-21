using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Ragnarok.Presentation.Converter
{
    /// <summary>
    /// 色のアルファ値を変更します。
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Color))]
    public class ColorAlphaConverter : IValueConverter
    {
        /// <summary>
        /// 色のアルファ値を変更します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                var alpha = (int)parameter;
                var color = (Color)value;

                return WPFUtil.MakeColor((byte)alpha, color);
            }
            catch (InvalidCastException ex)
            {
                Log.ErrorException(ex,
                    "ColorAlphaConverter: " +
                    "Colorに変換できませんでした。");

                return Colors.Black;
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
