using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Forms.Converter
{
    /// <summary>
    /// WinFormsのバインディングコンバーターの基本インターフェースです。
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// 値を変換します。
        /// </summary>
        object Convert(object value, Type targetType, object parameter);

        /// <summary>
        /// 値の逆変換を行います。
        /// </summary>
        object ConvertBack(object value, Type targetType, object parameter);
    }
}
