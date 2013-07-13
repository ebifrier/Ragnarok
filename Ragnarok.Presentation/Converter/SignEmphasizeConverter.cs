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
    /// +/-符号を目立つように大文字化します。
    /// </summary>
    public class SignEmphasizeConverter : IValueConverter
    {
        /// <summary>
        /// +/-符号を目立つように大文字化します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                var format = parameter as string ?? "{0}";
                var text = string.Format(format, value);

                if (string.IsNullOrEmpty(text))
                {
                    return text;
                }

                return text.Replace("-", "－");
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex, "値のフォーマットに失敗しました。");

                return "0";
            }
        }

        /// <summary>
        /// 基の値に戻すことはできないので、値をそのまま返す。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
