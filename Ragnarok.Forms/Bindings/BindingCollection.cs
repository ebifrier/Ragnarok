using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Bindings
{
    /// <summary>
    /// Bindingを扱うコレクションクラスです。
    /// </summary>
    public class BindingsCollection
    {
        private readonly List<BindingData> bindingList = new List<BindingData>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BindingsCollection(Control control)
        {
            Control = control;
        }

        /// <summary>
        /// コレクションを保持するコントロールを取得します。
        /// </summary>
        public Control Control
        {
            get;
            private set;
        }

        /// <summary>
        /// バインディングをすべてクリアします。
        /// </summary>
        public void Clear()
        {
            foreach (var binding in this.bindingList)
            {
                binding.Unbind();
            }

            this.bindingList.Clear();
        }

        /// <summary>
        /// バインディングを追加します。
        /// </summary>
        public Binding Add(object bindableTarget, string bindingPropertyName,
                           object dataSource, string dataSourcePropertyName,
                           string format = null)
        {
            return Add(
                bindableTarget, bindingPropertyName,
                dataSource, dataSourcePropertyName,
                BindingMode.Default, format,
                null, null);
        }

        /// <summary>
        /// バインディングを追加します。
        /// </summary>
        public Binding Add(object bindableTarget, string bindingPropertyName,
                           object dataSource, string dataSourcePropertyName,
                           BindingMode mode, string format = null)
        {
            return Add(
                bindableTarget, bindingPropertyName,
                dataSource, dataSourcePropertyName,
                mode, format, null, null);
        }

        /// <summary>
        /// バインディングを追加します。
        /// </summary>
        public Binding Add(object bindableTarget, string bindingPropertyName,
                           object dataSource, string dataSourcePropertyName,
                           BindingPropertyChangedCallback propertyChanged,
                           CoerceBindingValueCallback coerceValue = null)
        {
            return Add(
                bindableTarget, bindingPropertyName,
                dataSource, dataSourcePropertyName,
                BindingMode.Default,
                propertyChanged, coerceValue);
        }

        /// <summary>
        /// バインディングを追加します。
        /// </summary>
        public Binding Add(object bindableTarget, string bindingPropertyName,
                           object dataSource, string dataSourcePropertyName,
                           BindingMode mode,
                           BindingPropertyChangedCallback propertyChanged,
                           CoerceBindingValueCallback coerceValue = null)
        {
            return Add(
                bindableTarget, bindingPropertyName,
                dataSource, dataSourcePropertyName,
                mode, (Converter.IValueConverter)null,
                propertyChanged, coerceValue);
        }

        /// <summary>
        /// バインディングを追加します。
        /// </summary>
        public Binding Add(object bindableTarget, string bindingPropertyName,
                           object dataSource, string dataSourcePropertyName,
                           Converter.IValueConverter converter,
                           BindingPropertyChangedCallback propertyChanged = null,
                           CoerceBindingValueCallback coerceValue = null)
        {
            return Add(
                bindableTarget, bindingPropertyName,
                dataSource, dataSourcePropertyName,
                BindingMode.Default, converter,
                propertyChanged, coerceValue);
        }

        /// <summary>
        /// バインディングを追加します。
        /// </summary>
        public Binding Add(object bindableTarget, string bindingPropertyName,
                           object dataSource, string dataSourcePropertyName,
                           BindingMode mode,
                           Converter.IValueConverter converter,
                           BindingPropertyChangedCallback propertyChanged = null,
                           CoerceBindingValueCallback coerceValue = null)
        {
            var binding = new Binding
            {
                BindableTarget = bindableTarget,
                BindingPropertyName = bindingPropertyName,
                DataSource = dataSource,
                DataSourcePropertyName = dataSourcePropertyName,
                Mode = mode,
                Converter = converter,
                PropertyChanged = propertyChanged,
                CoerceValue = coerceValue,
            };

            return Add(binding);
        }

        /// <summary>
        /// バインディングを追加します。
        /// </summary>
        public Binding Add(object bindableTarget, string bindingPropertyName,
                           object dataSource, string dataSourcePropertyName,
                           BindingMode mode, string format,
                           BindingPropertyChangedCallback propertyChanged,
                           CoerceBindingValueCallback coerceValue = null)
        {
            var binding = new Binding
            {
                BindableTarget = bindableTarget,
                BindingPropertyName = bindingPropertyName,
                DataSource = dataSource,
                DataSourcePropertyName = dataSourcePropertyName,
                Mode = mode,
                StringFormat = format,
                PropertyChanged = propertyChanged,
                CoerceValue = coerceValue,
            };

            return Add(binding);
        }

        /// <summary>
        /// バインディングを追加します。
        /// </summary>
        public Binding Add(Binding binding)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            binding.Control = Control;

            var data = new BindingData(binding);
            data.Bind();

            this.bindingList.Add(data);
            return binding;
        }
    }
}
