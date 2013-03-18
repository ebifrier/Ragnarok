using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Ragnarok.Presentation.Extension
{
    /// <summary>
    /// フォント一覧を取得します。
    /// </summary>
    [MarkupExtensionReturnType(typeof(List<FontFamily>))]
    public class FontFamilyListExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FontFamilyListExtension()
        {
        }

        /// <summary>
        /// フォント一覧を取得します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            var language = XmlLanguage.GetLanguage("ja-jp");

            // 日本語フォントのみをリストアップします。
            return Fonts.SystemFontFamilies
                .Where(ff => ff.FamilyNames.Any(fn => fn.Key == language))
                .ToList();
        }
    }
}
