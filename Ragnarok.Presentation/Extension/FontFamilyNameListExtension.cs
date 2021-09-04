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
    [MarkupExtensionReturnType(typeof(List<string>))]
    public class FontFamilyNameListExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FontFamilyNameListExtension()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FontFamilyNameListExtension(string language)
        {
            Language = language;
        }

        /// <summary>
        /// 対象言語を取得または設定します。
        /// </summary>
        public string Language
        {
            get;
            set;
        }

        /// <summary>
        /// フォント一覧を取得します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            var language = (Language != null ?
                XmlLanguage.GetLanguage(Language) :
                null);

            // 日本語フォントのみをリストアップします。
            return Fonts.SystemFontFamilies
                .Where(ff => language == null ||
                             ff.FamilyNames.Any(fn => fn.Key == language))
                .Select(_ => _.Source)
                .ToList();
        }
    }
}
