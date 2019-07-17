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
    [MarkupExtensionReturnType(typeof(List<HorizontalAlignment>))]
    public class HorizontalAlignmentListExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HorizontalAlignmentListExtension()
        {
            ContainsCenter = true;
            ContainsStretch = true;
        }

        /// <summary>
        /// Centerを含めるかどうかを取得または設定します。
        /// </summary>
        public bool ContainsCenter
        {
            get;
            set;
        }

        /// <summary>
        /// Stretchを含めるかどうかを取得または設定します。
        /// </summary>
        public bool ContainsStretch
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクト一覧を取得します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            var list = new List<HorizontalAlignment>();
            list.Add(HorizontalAlignment.Left);

            if (ContainsCenter)
            {
                list.Add(HorizontalAlignment.Center);
            }

            list.Add(HorizontalAlignment.Right);

            if (ContainsStretch)
            {
                list.Add(HorizontalAlignment.Stretch);
            }

            return list;
        }
    }
}
