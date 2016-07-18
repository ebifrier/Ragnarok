using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ragnarok.Forms.Utility
{
    using Ragnarok.Utility;

    /// <summary>
    /// wpfのオブジェクトも扱える設定ファイル用クラスです。
    /// </summary>
    [Serializable()]
    public class FormsSettingsBase : AppSettingsBase
    {
        /// <summary>
        /// 値から文字列への変換を試みます。
        /// </summary>
        protected override bool TryConvertToString(Type type, object value,
                                                   out string result)
        {
            if (type == typeof(Color))
            {
                var c = (Color)value;
                result = $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
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
                var tmp = str.Replace("#", "0x");
                result = ColorTranslator.FromHtml(tmp);
                return true;
            }

            return base.TryConvertToValue(type, str, out result);
        }
    }
}
