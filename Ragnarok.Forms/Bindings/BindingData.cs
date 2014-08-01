using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Bindings
{
    using Utility;

    internal sealed class BindingData
    {
        private readonly ReentrancyLock recurceLock = new ReentrancyLock();
        private readonly IPropertyObject targetPropertyObject;
        private readonly IPropertyObject sourcePropertyObject;
        private object oldValue;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BindingData(Binding binding)
        {
            Binding = binding;

            this.targetPropertyObject = MethodUtil.GetPropertyObject(
                Binding.BindableTarget.GetType(), Binding.BindingPropertyName);
            this.sourcePropertyObject = MethodUtil.GetPropertyObject(
                Binding.DataSource.GetType(), Binding.DataSourcePropertyName);
        }

        /// <summary>
        /// Unbind時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler Unbound;

        /// <summary>
        /// 操作対象となるBindingを取得します。
        /// </summary>
        public Binding Binding
        {
            get;
            private set;
        }

        #region Utility
        /// <summary>
        /// 値が変わった時、ターゲットコントロールの値の変更するか調べます。
        /// </summary>
        private bool IsHandleToTarget(bool defaultIsHandle)
        {
            return (
                (defaultIsHandle && Binding.Mode == BindingMode.Default) ||
                Binding.Mode == BindingMode.OneWay ||
                Binding.Mode == BindingMode.TwoWay);
        }

        /// <summary>
        /// 値が変わった時、ソースデータの値の変更するか調べます。
        /// </summary>
        private bool IsHandleToSource(bool defaultIsHandle)
        {
            return (
                (defaultIsHandle && Binding.Mode == BindingMode.Default) ||
                Binding.Mode == BindingMode.OneWayToSource ||
                Binding.Mode == BindingMode.TwoWay);
        }

        /// <summary>
        /// ターゲットのプロパティ値を取得します。
        /// </summary>
        private object GetTargetPropertyValue()
        {
            return this.targetPropertyObject.GetValue(Binding.BindableTarget);
        }

        /// <summary>
        /// ターゲットのプロパティ値を設定します。
        /// </summary>
        private void SetTargetPropertyValue(object value)
        {
            var type = this.targetPropertyObject.PropertyType;
            value = (
                type == typeof(string) ? string.Format("{0}", value) :
                value);

            this.targetPropertyObject.SetValue(Binding.BindableTarget, value);
        }

        /// <summary>
        /// データソースのプロパティ値を取得します。
        /// </summary>
        private object GetSourcePropertyValue()
        {
            return this.sourcePropertyObject.GetValue(Binding.DataSource);
        }

        /// <summary>
        /// データソースのプロパティ値を設定します。
        /// </summary>
        private void SetSourcePropertyValue(object value)
        {
            this.sourcePropertyObject.SetValue(Binding.DataSource, value);
        }

        /// <summary>
        /// PropertyChangedイベントを発生させます。
        /// </summary>
        private void FirePropertyChanged(object newValue, object oldValue)
        {
            var handler = Binding.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            Util.SafeCall(() => handler(
                Binding.Control,
                new BindingPropertyChangedEventArgs(
                    Binding.BindingPropertyName, newValue, oldValue)));
        }

        /// <summary>
        /// ターゲットコントロールの値が変更されたときに呼ばれます。
        /// </summary>
        /// <remarks>
        /// ソースデータの値を変更します。
        /// </remarks>
        private void OnTargetValueChanged()
        {
            using (var result = this.recurceLock.Lock())
            {
                if (result == null) return;

                // 新しい値の取得
                var newValue = GetTargetPropertyValue();
                if (Binding.Converter != null)
                {
                    newValue = Binding.Converter.ConvertBack(
                        newValue, this.sourcePropertyObject.PropertyType, null);
                }

                if (Binding.CoerceValue != null)
                {
                    newValue = Binding.CoerceValue(
                        Binding.Control, newValue);
                }

                // 文字列へのフォーマットを行います。
                if (!string.IsNullOrEmpty(Binding.StringFormat))
                {
                    newValue = string.Format(Binding.StringFormat, newValue);
                }

                if (Util.GenericEquals(newValue, this.oldValue) ||
                    ReferenceEquals(newValue, FormsValue.UnsetValue))
                {
                    return;
                }

                // 値の変更後、プロパティ値変更イベントを送ります。
                SetSourcePropertyValue(newValue);

                FirePropertyChanged(newValue, this.oldValue);
                this.oldValue = newValue;
            }
        }

        /// <summary>
        /// ソースデータの値が変更されたときに呼ばれます。
        /// </summary>
        /// <remarks>
        /// ターゲットコントロールの値を変更します。
        /// </remarks>
        private void OnSourceValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != Binding.DataSourcePropertyName)
            {
                return;
            }

            using (var result = this.recurceLock.Lock())
            {
                if (result == null) return;

                // 新しい値の取得
                var newValue = GetSourcePropertyValue();
                if (Binding.Converter != null)
                {
                    newValue = Binding.Converter.Convert(
                        newValue, this.targetPropertyObject.PropertyType, null);
                }

                if (Binding.CoerceValue != null)
                {
                    newValue = Binding.CoerceValue(
                        Binding.Control, newValue);
                }

                // 文字列へのフォーマットを行います。
                if (!string.IsNullOrEmpty(Binding.StringFormat))
                {
                    newValue = string.Format(Binding.StringFormat, newValue);
                }

                if (Util.GenericEquals(newValue, this.oldValue) ||
                    ReferenceEquals(newValue, FormsValue.UnsetValue))
                {
                    return;
                }

                // 値の変更後、プロパティ値変更イベントを送ります。
                SetTargetPropertyValue(newValue);

                FirePropertyChanged(newValue, this.oldValue);
                this.oldValue = newValue;
            }
        }
        #endregion

        #region Bind/Unbind
        /// <summary>
        /// バインディングの解除時に呼ばれます。
        /// </summary>
        private void OnUnbind(object sender, EventArgs e)
        {
            Unbind();
        }

        /// <summary>
        /// バインディングを解除します。
        /// </summary>
        public void Unbind()
        {
            Unbound.SafeRaiseEvent(this, EventArgs.Empty);
            Unbound = null;

            Binding.BindableTarget.Disposed -= OnUnbind;
            Binding.Control.Disposed -= OnUnbind;
            Binding.IsBinding = false;
        }

        /// <summary>
        /// バインディングを行います。
        /// </summary>
        public void Bind()
        {
            if (!BindInternal())
            {
                throw new NotImplementedException();
            }

            // 初期化時は、ソースからコントロールへ値を反映します。
            OnSourceValueChanged(this,
                new PropertyChangedEventArgs(Binding.DataSourcePropertyName));

            Binding.BindableTarget.Disposed += OnUnbind;
            Binding.Control.Disposed += OnUnbind;
            Binding.IsBinding = true;
        }
        #endregion

        #region BindInternal
        /// <summary>
        /// バインド対象となるクラス型やコールバックなどを保持します。
        /// </summary>
        internal sealed class BindableInfo
        {
            public delegate void BindCallback(object target);

            public BindableInfo(Type type, string name, BindCallback callback)
            {
                BindableType = type;
                PropertyName = name;
                Callback = callback;
            }

            public Type BindableType
            {
                get;
                private set;
            }

            public string PropertyName
            {
                get;
                private set;
            }

            public BindCallback Callback
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// 実際のバインディング処理を行い、失敗であれば偽を返します。
        /// </summary>
        private bool BindInternal()
        {
            var BindTable = new BindableInfo[]
            {
                new BindableInfo(typeof(Control), "Enabled", Bind_Control_Enabled),
                new BindableInfo(typeof(TextBox), "Text", Bind_TextBox_Text),
                new BindableInfo(typeof(ListBox), "SelectedIndex", Bind_ListBox_SelectedIndex),
                new BindableInfo(typeof(ListBox), "SelectedItem", Bind_ListBox_SelectedIndex),
                new BindableInfo(typeof(ComboBox), "SelectedIndex", Bind_ComboBox_SelectedIndex),
                new BindableInfo(typeof(ComboBox), "SelectedItem", Bind_ComboBox_SelectedIndex),
                new BindableInfo(typeof(TabControl), "SelectedIndex", Bind_TabControl_SelectedIndex),
                new BindableInfo(typeof(Label), "Text", Bind_Label_Text),
                new BindableInfo(typeof(NumericUpDown), "Value", Bind_NumericUpDown_Value),
                new BindableInfo(typeof(RadioButton), "Checked", Bind_RadioButton_Checked),
                new BindableInfo(typeof(CheckBox), "Checked", Bind_CheckBox_Checked),
            };

            foreach (var item in BindTable)
            {
                if (item.BindableType.IsInstanceOfType(Binding.BindableTarget) &&
                    item.PropertyName == Binding.BindingPropertyName)
                {
                    item.Callback(Binding.BindableTarget);
                    return true;
                }
            }

            return false;
        }

        #region Control
        private void Bind_Control_Enabled(object obj)
        {
            var target = (Control)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(false))
            {
                var handler = new EventHandler((_, __) => OnTargetValueChanged());

                target.EnabledChanged += handler;
                Unbound += (_, __) => target.EnabledChanged -= handler;
            }
        }
        #endregion

        #region TextBox
        private void Bind_TextBox_Text(object obj)
        {
            var target = (TextBox)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(true))
            {
                var handler = new EventHandler((_, __) => OnTargetValueChanged());

                target.TextChanged += handler;
                Unbound += (_, __) => target.TextChanged -= handler;
            }
        }
        #endregion

        #region ListBox
        private void Bind_ListBox_SelectedIndex(object obj)
        {
            var target = (ListBox)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(true))
            {
                var handler = new EventHandler((_, __) => OnTargetValueChanged());

                target.SelectedIndexChanged += handler;
                Unbound += (_, __) => target.SelectedIndexChanged -= handler;
            }
        }
        #endregion

        #region ComboBox
        private void Bind_ComboBox_SelectedIndex(object obj)
        {
            var target = (ComboBox)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(true))
            {
                var handler = new EventHandler((_, __) => OnTargetValueChanged());

                target.SelectedIndexChanged += handler;
                Unbound += (_, __) => target.SelectedIndexChanged -= handler;
            }
        }
        #endregion

        #region TabControl
        private void Bind_TabControl_SelectedIndex(object obj)
        {
            var target = (TabControl)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(true))
            {
                var handler = new EventHandler((_, __) => OnTargetValueChanged());

                target.SelectedIndexChanged += handler;
                Unbound += (_, __) => target.SelectedIndexChanged -= handler;
            }
        }
        #endregion

        #region Label
        private void Bind_Label_Text(object obj)
        {
            var target = (Label)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(false))
            {
                throw new NotSupportedException();
            }
        }
        #endregion

        #region NumericUpDown
        private void Bind_NumericUpDown_Value(object obj)
        {
            var target = (NumericUpDown)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(true))
            {
                var handler = new EventHandler((_, __) => OnTargetValueChanged());

                target.ValueChanged += handler;
                Unbound += (_, __) => target.ValueChanged -= handler;
            }
        }
        #endregion

        #region RadioButton
        private void Bind_RadioButton_Checked(object obj)
        {
            var target = (RadioButton)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(true))
            {
                var handler = new EventHandler((_, __) => OnTargetValueChanged());

                target.CheckedChanged += handler;
                Unbound += (_, __) => target.CheckedChanged -= handler;
            }
        }
        #endregion

        #region CheckBox
        private void Bind_CheckBox_Checked(object obj)
        {
            var target = (CheckBox)obj;

            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(true))
            {
                var handler = new EventHandler((_, __) => OnTargetValueChanged());

                target.CheckedChanged += handler;
                Unbound += (_, __) => target.CheckedChanged -= handler;
            }
        }
        #endregion
        #endregion
    }
}
