using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Ragnarok.Presentation
{
    using Ragnarok.Utility;

    /// <summary>
    /// wpfのオブジェクトも扱える設定ファイル用クラスです。
    /// </summary>
    [Serializable()]
    public class WPFSettingsBase : AppSettingsBase
    {
        /// <summary>
        /// 値から文字列への変換を試みます。
        /// </summary>
        protected override bool TryConvertToString(Type type, object value,
                                                   out string result)
        {
            if (type == typeof(Color))
            {
                result = (value != null ? value.ToString() : "#00000000");
                return true;
            }

            return base.TryConvertToString(type, value, out result);
        }

        /// <summary>
        /// 文字列から値への変換を試みます。
        /// </summary>
        protected override bool TryConvertToValue(Type type, string str,
                                                  out object result)
        {
            if (type == typeof(Color))
            {
                result = ColorConverter.ConvertFromString(str);
                return true;
            }

            return base.TryConvertToValue(type, str, out result);
        }
    }
}
