using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ragnarok.Utility;

namespace Ragnarok.Forms.Bindings
{
    using Controls;

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

            var component = Binding.BindableTarget as Component;
            if (component != null)
            {
                component.Disposed -= OnUnbind;
            }

            Binding.Control.Disposed -= OnUnbind;
            Binding.IsBinding = false;
        }

        /// <summary>
        /// バインディングを行います。
        /// </summary>
        public void Bind()
        {
            BindDataSource(Binding.DataSource, Binding.DataSourcePropertyName);
            BindBindableTarget(Binding.BindableTarget, Binding.BindingPropertyName);

            // 初期化時は、ソースからコントロールへ値を反映します。
            OnSourceValueChanged(this,
                new PropertyChangedEventArgs(Binding.DataSourcePropertyName));

            var component = Binding.BindableTarget as Component;
            if (component != null)
            {
                component.Disposed += OnUnbind;
            }

            Binding.Control.Disposed += OnUnbind;
            Binding.IsBinding = true;
        }

        /// <summary>
        /// ソースデータをバインディングします。
        /// </summary>
        private void BindDataSource(object dataSource, string propertyName)
        {
            if (!IsHandleToTarget(true))
            {
                // Source -> Targetへの通知が有効でない場合は何も設定しません。
                return;
            }

            var propertyObj = dataSource as INotifyPropertyChanged;
            if (propertyObj != null)
            {
                var handler = new PropertyChangedEventHandler((sender, e) =>
                {
                    if (e.PropertyName == propertyName)
                    {
                        OnSourceValueChanged(sender, e);
                    }
                });

                propertyObj.PropertyChanged += handler;
                Unbound += (_, __) => propertyObj.PropertyChanged -= handler;
                return;
            }
        }

        /// <summary>
        /// 実際のバインディング処理を行い、失敗であれば偽を返します。
        /// </summary>
        private void BindBindableTarget(object bindableTarget, string propertyName)
        {
            if (!IsHandleToSource(true))
            {
                // Source -> Targetへの通知が有効でない場合は何も設定しません。
                return;
            }

            // TargetがComponent型の場合は、'PropertyName'+Changed
            // という名前のイベントでプロパティの変更を把握します。
            var component = bindableTarget as Component;
            if (component != null)
            {
                // 名前が"PropertyName + Changed"のイベントを検索します。
                var eventName = propertyName + "Changed";
                var ev = component.GetType().GetEvent(eventName);
                if (ev != null)
                {
                    var handler = new EventHandler(OnTargetValueChanged);

                    ev.AddEventHandler(component, handler);
                    Unbound += (_, __) => ev.RemoveEventHandler(component, handler);
                    return;
                }
                else if (propertyName == "SelectedItem")
                {
                    // プロパティ名がSelectedItemの場合はイベントがないことがあるので、
                    // SelectedIndexChangedを代りに探してみます。
                    ev = component.GetType().GetEvent("SelectedIndexChanged");
                    if (ev != null)
                    {
                        var handler = new EventHandler(OnTargetValueChanged);

                        ev.AddEventHandler(component, handler);
                        Unbound += (_, __) => ev.RemoveEventHandler(component, handler);
                        return;
                    }
                }

                throw new InvalidOperationException(
                    string.Format(
                        "'{0}': 指定のプロパティかその変更イベントが" +
                        "コントロールに存在しません。",
                        propertyName));
            }

            // TargetがINotifyPropertyChangedの場合は
            // PropertyChangedイベントでプロパティの変更を把握します。
            var propertyObj = bindableTarget as INotifyPropertyChanged;
            if (propertyObj != null)
            {
                var handler = new PropertyChangedEventHandler((sender, e) =>
                {
                    if (e.PropertyName == propertyName)
                    {
                        OnTargetValueChanged(sender, e);
                    }
                });

                propertyObj.PropertyChanged += handler;
                Unbound += (_, __) => propertyObj.PropertyChanged -= handler;
                return;
            }
        }
        #endregion
    }
}
