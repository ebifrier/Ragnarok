using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace Ragnarok.Presentation.Converter
{
    /// <summary>
    /// 文字列からフォントファミリに変換します。
    /// </summary>
    [ValueConversion(typeof(string), typeof(FontFamily))]
    public class FontFamilyConverter : IValueConverter
    {
        /// <summary>
        /// 文字列からフォントファミリに変換します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                var name = (string)value;

                return new FontFamily(name);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "フォントファミリの作成に失敗しました。");

                return SystemFonts.MessageFontFamily;
            }
        }

        /// <summary>
        /// フォントファミリから文字列に変換します。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            try
            {
                var ff = (FontFamily)value;

                return ff.Source;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "フォントファミリの名前取得に失敗しました。");

                return string.Empty;
            }
        }
    }
}
