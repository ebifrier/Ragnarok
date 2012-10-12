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
    /// 真偽値などをVisibilityの値に変換します。
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    [ValueConversion(typeof(int), typeof(Visibility))]
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private Visibility defaultHiddenValue = Visibility.Hidden;

        /// <summary>
        /// 非表示の時の規定値を取得または設定します。
        /// </summary>
        public Visibility DefaultHiddenValue
        {
            get { return this.defaultHiddenValue; }
            set { this.defaultHiddenValue = value; }
        }

        /// <summary>
        /// 真偽値などをVisibilityの値に変換します。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            var isVisible = false;

            if (value is bool)
            {
                isVisible = (bool)value;
            }
            else if (value is int)
            {
                isVisible = ((int)value != 0);
            }
            else
            {
                isVisible = (value != null);
            }

            return (isVisible ? Visibility.Visible : DefaultHiddenValue);
        }

        /// <summary>
        /// Visibilityの値を真偽値に変換します。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            bool isVisible = ((Visibility)value == Visibility.Visible);

            return isVisible;
        }
    }
}
