using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Ragnarok.Shogi.ViewModel
{
    /// <summary>
    /// <see cref="EntityObject"/>クラスから使うときに便利な
    /// バインディングオブジェクトです。
    /// </summary>
    /// <remarks>
    /// <see cref="EntityObject"/>クラスのDataContextを
    /// デフォルトのバインディングソースに指定します。
    /// </remarks>
    [MarkupExtensionReturnType(typeof(object))]
    public class BindingExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BindingExtension()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BindingExtension(PropertyPath path)
        {
            Path = path;
        }

        /// <summary>
        /// データへのパスを取得または設定します。
        /// </summary>
        [ConstructorArgument("path")]
        public PropertyPath Path
        {
            get;
            set;
        }

        /// <summary>
        /// バインディングモードを取得または設定します。
        /// </summary>
        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// バインディングのソースを取得または設定します。
        /// </summary>
        [DefaultValue((string)null)]
        public object Source
        {
            get;
            set;
        }

        /// <summary>
        /// バインディングの要素名を取得または設定します。
        /// </summary>
        [DefaultValue((string)null)]
        public string ElementName
        {
            get;
            set;
        }

        /// <summary>
        /// コンバーターを取得または設定します。
        /// </summary>
        [DefaultValue((IValueConverter)null)]
        public IValueConverter Converter
        {
            get;
            set;
        }

        /// <summary>
        /// コンバーターのパラメータを取得または設定します。
        /// </summary>
        [DefaultValue((object)null)]
        public object ConverterParameter
        {
            get;
            set;
        }

        /// <summary>
        /// 必要なら指定されたパスを修正します。
        /// </summary>
        /// <remarks>
        /// ターゲットがEntityObjectの場合、
        /// パスの先頭にDataContextを付加します。
        /// </remarks>
        private PropertyPath CreatePath(DependencyObject target)
        {
            if (target is EntityObject &&
                Source == null && string.IsNullOrEmpty(ElementName))
            {
                var path = (Path != null ? Path.Path : string.Empty);

                if (string.IsNullOrEmpty(path) || path == ".")
                {
                    return new PropertyPath("DataContext");
                }
                else
                {
                    return new PropertyPath("DataContext." + path);
                }
            }

            return (Path ?? new PropertyPath(".", new object[0]));
        }

        /// <summary>
        /// バインディングを作成します。
        /// </summary>
        private Binding CreateBinding(DependencyObject target)
        {
            var binding = new Binding()
            {
                Path = CreatePath(target),
                Mode = Mode,
                Converter = Converter,
                ConverterParameter = ConverterParameter,
            };

            if (Source != null)
            {
                binding.Source = Source;
            }
            else if (ElementName != null)
            {
                binding.ElementName = ElementName;
            }
            else
            {
                // デフォルトのソース
                binding.Source = target;
            }

            return binding;
        }

        /// <summary>
        /// バインディングした値を返します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            var providerTarget = service.GetService(typeof(IProvideValueTarget))
                as IProvideValueTarget;
            if (providerTarget == null)
            {
                return null; // よくわかんないけど。。。
            }

            var target = providerTarget.TargetObject as DependencyObject;
            if (target == null)
            {
                throw new InvalidOperationException(
                    "正しいターゲットが指定されていません。");
            }

            var property = providerTarget.TargetProperty as DependencyProperty;
            if (property == null)
            {
                throw new InvalidOperationException(
                    "正しいプロパティが指定されていません。");
            }

            // バインディング操作を実行します。
            var binding = CreateBinding(target);
            BindingOperations.SetBinding(target, property, binding);

            return target.GetValue(property);
        }
    }
}
