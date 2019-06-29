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
    /// フォントスタイル一覧を取得します。
    /// </summary>
    [MarkupExtensionReturnType(typeof(List<FontStyle>))]
    public class FontStyleListExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FontStyleListExtension()
        {
        }

        /// <summary>
        /// フォント一覧を取得します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            return new FontStyle[]
            {
                FontStyles.Normal,
                FontStyles.Italic,
                FontStyles.Oblique,
            };
        }
    }
}
