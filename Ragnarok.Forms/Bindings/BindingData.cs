using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Bindings
{
    using Controls;
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
            if (this.targetPropertyObject == null)
            {
                throw new ArgumentException(
                    Binding.BindingPropertyName + ": 指定のプロパティが存在しません。");
            }

            this.sourcePropertyObject = MethodUtil.GetPropertyObject(
                Binding.DataSource.GetType(), Binding.DataSourcePropertyName);
            if (this.sourcePropertyObject == null)
            {
                throw new ArgumentException(
                    Binding.DataSourcePropertyName + ": 指定のプロパティが存在しません。");
            }
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
        private void OnTargetValueChanged(object sender, EventArgs e)
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
            BindInternal();

            // 初期化時は、ソースからコントロールへ値を反映します。
            OnSourceValueChanged(this,
                new PropertyChangedEventArgs(Binding.DataSourcePropertyName));

            Binding.BindableTarget.Disposed += OnUnbind;
            Binding.Control.Disposed += OnUnbind;
            Binding.IsBinding = true;
        }

        /// <summary>
        /// 実際のバインディング処理を行い、失敗であれば偽を返します。
        /// </summary>
        private void BindInternal()
        {
            var propertyObj = Binding.DataSource as INotifyPropertyChanged;
            if (propertyObj != null && IsHandleToTarget(true))
            {
                propertyObj.PropertyChanged += OnSourceValueChanged;
                Unbound += (_, __) => propertyObj.PropertyChanged -= OnSourceValueChanged;
            }

            if (IsHandleToSource(true))
            {
                var control = Binding.BindableTarget;

                // 名前が"PropertyName + Changed"のイベントを検索します。
                var eventName = Binding.BindingPropertyName + "Changed";
                var ev = control.GetType().GetEvent(eventName);

                if (ev != null)
                {
                    var handler = new EventHandler(OnTargetValueChanged);

                    ev.AddEventHandler(control, handler);
                    Unbound += (_, __) => ev.RemoveEventHandler(control, handler);
                }
            }
        }
        #endregion
    }
}
