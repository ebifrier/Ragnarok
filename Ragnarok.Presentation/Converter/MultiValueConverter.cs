using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Ragnarok.Presentation.Converter
{
    /// <summary>
    /// 複数のパラメーターを与えるときに使います。
    /// </summary>
    public class MultiValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// 複数の値をパラメーター値に変換します。
        /// </summary>
        public object Convert(object[] values, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes,
                                    object parameter, CultureInfo culture)
        {
            return (object[])value;
        }
    }
}
