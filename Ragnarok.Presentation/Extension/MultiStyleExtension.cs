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

            if (!this.resourceKeys.Any())
            {
                throw new ArgumentException(
                    "No input resource keys specified.");
            }
        }
        
        private static Style GetStyle(IServiceProvider service,
                                      StaticResourceExtension resource)
        {
            var style = resource.ProvideValue(service) as Style;
            if (style == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Could not find style with resource key {0}.",
                        resource.ResourceKey));
            }
            
            return style;
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

            return resourceKeys
                .Select(_ => new StaticResourceExtension(_))
                .Select(_ => GetStyle(service, _))
                .Aggregate(new Style(), (result, _) => Merge(result, _));
        }

        /// <summary>
        /// 複数のスタイルをマージします。
        /// </summary>
        private static Style Merge(Style style1, Style style2)
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

            // setterとtriggerをマージします。
            style2.Setters.ForEach(_ => style1.Setters.Add(_));
            style2.Triggers.ForEach(_ => style1.Triggers.Add(_));

            // DynamicResourcesをマージ。
            style2.Resources.Keys
                .OfType<object>()
                .ForEach(_ => style1.Resources[_] = style2.Resources[_]);

            return style1;
        }
    }
}
