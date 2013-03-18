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
    /// 値を比較し一致するかどうか調べます。
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool))]
    public class ValueCompareToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 真の時使われるオブジェクトを取得または設定します。
        /// </summary>
        public object TrueObject
        {
            get;
            set;
        }

        /// <summary>
        /// 偽の時使われるオブジェクトを取得または設定します。
        /// </summary>
        public object FalseObject
        {
            get;
            set;
        }

        /// <summary>
        /// 値が一致するか調べます。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return Util.GenericEquals(value, TrueObject);
        }

        /// <summary>
        /// 真偽値を対象となる値に直します。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return ((bool)value ? TrueObject : FalseObject);
        }
    }
}
