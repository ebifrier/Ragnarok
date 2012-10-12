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
    /// 色をSolidColorBrushに変換します。
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Brush))]
    [ValueConversion(typeof(string), typeof(Brush))]
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        /// <summary>
        /// 色をブラシに変換します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                if (targetType != typeof(Brush) &&
                    targetType != typeof(SolidColorBrush))
                {
                    throw new InvalidOperationException(
                        "変換先のオブジェクトがブラシではありません。");
                }

                if (value.GetType() == typeof(Color))
                {
                    var brush = new SolidColorBrush((Color)value);
                    brush.Freeze();
                    return brush;
                }
                else if (value.GetType() == typeof(string))
                {
                    var converter = new ColorConverter();
                    var color = (Color)converter.ConvertFrom(value);

                    var brush = new SolidColorBrush((Color)value);
                    brush.Freeze();
                    return brush;
                }

                return null;
            }
            catch (InvalidCastException ex)
            {
                Log.ErrorException(ex,
                    "ColorToSolidColorBrushConverter: " +
                    "ブラシに変換できませんでした。");

                return Brushes.Red;
            }
        }

        /// <summary>
        /// 単色ブラシからその色を取得します。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            try
            {
                if (targetType != typeof(Color))
                {
                    return null;
                }

                var brush = value as SolidColorBrush;
                if (brush == null)
                {
                    return null;
                }

                return brush.Color;
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }
    }
}
