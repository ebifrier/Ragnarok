using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;

namespace Ragnarok.Presentation.Extension
{
    /// <summary>
    /// 複数スタイルを適用するための拡張構文です。
    /// </summary>
	[MarkupExtensionReturnType(typeof(Style))]
	public class MultiStyleExtension : MarkupExtension
	{
		private string[] resourceKeys;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MultiStyleExtension(string inputResourceKeys)
		{
			if (inputResourceKeys == null)
			{
				throw new ArgumentNullException("inputResourceKeys");
			}

			this.resourceKeys = inputResourceKeys.Split(
                new char[] { ' ', ',' },
                StringSplitOptions.RemoveEmptyEntries);

			if (this.resourceKeys.Length == 0)
			{
				throw new ArgumentException("No input resource keys specified.");
			}
		}

		/// <summary>
		/// Returns a style that merges all styles with the keys specified in the constructor.
		/// </summary>
		public override object ProvideValue(IServiceProvider service)
		{
            if (WPFUtil.IsInDesignMode)
            {
                return null;
            }

			var resultStyle = new Style();

			foreach (string currentResourceKey in resourceKeys)
			{
				var currentStyle = new StaticResourceExtension(currentResourceKey)
                    .ProvideValue(service) as Style;

				if (currentStyle == null)
				{
					throw new InvalidOperationException("Could not find style with resource key " + currentResourceKey + ".");
				}

                Merge(resultStyle, currentStyle);
			}

			return resultStyle;
		}

        /// <summary>
        /// 複数のスタイルをマージします。
        /// </summary>
        private static void Merge(Style style1, Style style2)
        {
            if (style1 == null)
            {
                throw new ArgumentNullException("style1");
            }
            if (style2 == null)
            {
                throw new ArgumentNullException("style2");
            }

            if (style1.TargetType.IsAssignableFrom(style2.TargetType))
            {
                style1.TargetType = style2.TargetType;
            }

            if (style2.BasedOn != null)
            {
                Merge(style1, style2.BasedOn);
            }

            foreach (var currentSetter in style2.Setters)
            {
                style1.Setters.Add(currentSetter);
            }

            foreach (var currentTrigger in style2.Triggers)
            {
                style1.Triggers.Add(currentTrigger);
            }

            // This code is only needed when using DynamicResources.
            foreach (var key in style2.Resources.Keys)
            {
                style1.Resources[key] = style2.Resources[key];
            }
        }
	}
}
