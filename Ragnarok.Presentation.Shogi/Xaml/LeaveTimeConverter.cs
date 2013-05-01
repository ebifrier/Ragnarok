using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;

using Ragnarok.Shogi;

namespace Ragnarok.Presentation.Shogi.Xaml
{
    /// <summary>
    /// 残り時間をTimeSpanから表示用文字列に変換します。
    /// </summary>
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class LeaveTimeConverter : IValueConverter
    {
        /// <summary>
        /// 残り時間をTimeSpanから表示用文字列に変換します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                var span = (TimeSpan)value;

                return span.ToString(@"hh\:mm\:ss");
            }
            catch (InvalidCastException ex)
            {
                Log.ErrorException(ex,
                    "TimeSpanのキャストに失敗しました。");

                return string.Empty;
            }
        }

        /// <summary>
        /// 実装していません。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
