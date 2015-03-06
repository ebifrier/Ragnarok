using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;

namespace Ragnarok.Forms.Converter
{
    /// <summary>
    /// bool値を反転します。
    /// </summary>
    public class NegValueConverter : IValueConverter
    {
        /// <summary>
        /// decimal値に変換します。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter)
        {
            try
            {
                return !(bool)value;
            }
            catch (InvalidCastException ex)
            {
                Log.ErrorException(ex,
                    "bool型の変換に失敗しました。");

                return value;
            }
        }

        /// <summary>
        /// 変換元の型に戻します。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter)
        {
            try
            {
                return !(bool)value;
            }
            catch (InvalidCastException ex)
            {
                Log.ErrorException(ex,
                    "bool型の変換に失敗しました。");

                return value;
            }
        }
    }
}
