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
    [MarkupExtensionReturnType(typeof(List<FontWeight>))]
    public class FontWeightListExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FontWeightListExtension()
        {
        }

        /// <summary>
        /// フォント一覧を取得します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            return new FontWeight[]
            {
                FontWeights.Thin,
                FontWeights.ExtraLight,
                FontWeights.Light,
                FontWeights.Normal,
                FontWeights.Medium,
                FontWeights.SemiBold,
                FontWeights.Bold,
                FontWeights.ExtraBold,
                FontWeights.Black,
                FontWeights.ExtraBlack,
            };
        }
    }
}
