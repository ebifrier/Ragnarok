using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using Ragnarok.Utility;

namespace Ragnarok.Forms.Bindings
{
    internal sealed class BindingData
    {
        private readonly ReentrancyLock recurceLock = new ReentrancyLock();
        private readonly IPropertyObject targetPropertyObject;
        private readonly IPropertyObject sourcePropertyObject;

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
            value = (type == typeof(string) ? $"{value}" : value);

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
                Binding.Component,
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
                        Binding.Component, newValue);
                }

                // 文字列へのフォーマットを行います。
                if (!string.IsNullOrEmpty(Binding.StringFormat))
                {
                    newValue = string.Format(
                        CultureInfo.CurrentCulture,
                        Binding.StringFormat,
                        newValue);
                }

                var oldValue = GetSourcePropertyValue();
                if (Util.GenericEquals(newValue, oldValue) ||
                    ReferenceEquals(newValue, FormsValue.UnsetValue))
                {
                    return;
                }

                // 値の変更後、プロパティ値変更イベントを送ります。
                SetSourcePropertyValue(newValue);

                FirePropertyChanged(newValue, oldValue);
            }
        }

        /// <summary>
        /// ソースデータの値が変更されたときに呼ばれます。
        /// </summary>
        /// <remarks>
        /// ターゲットコントロールの値を変更します。
        /// </remarks>
        private void OnSourceValueChanged(object sender, EventArgs e)
        {
            using (var result = this.recurceLock.Lock())
            {
                if (result == null) return;

                // 新しい値の取得
                var sourceValue = GetSourcePropertyValue();
                var newValue = sourceValue;

                if (Binding.Converter != null)
                {
                    newValue = Binding.Converter.Convert(
                        newValue, this.targetPropertyObject.PropertyType, null);
                }

                if (Binding.CoerceValue != null)
                {
                    newValue = Binding.CoerceValue(
                        Binding.Component, newValue);
                }

                // 文字列へのフォーマットを行います。
                if (!string.IsNullOrEmpty(Binding.StringFormat))
                {
                    newValue = string.Format(
                        CultureInfo.CurrentCulture,
                        Binding.StringFormat,
                        newValue);
                }

                var oldValue = GetTargetPropertyValue();
                if (Util.GenericEquals(newValue, oldValue) ||
                    ReferenceEquals(newValue, FormsValue.UnsetValue))
                {
                    return;
                }

                // 値の変更後、プロパティ値変更イベントを送ります。
                SetTargetPropertyValue(newValue);

                // oldValueにはsource側のオリジナルデータを設定します。
                FirePropertyChanged(newValue, oldValue);                
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

            var target = Binding.BindableTarget as Component;
            if (target != null)
            {
                target.Disposed -= OnUnbind;
            }

            if (Binding.Component != null)
            {
                Binding.Component.Disposed -= OnUnbind;
            }

            Binding.IsBinding = false;
        }

        /// <summary>
        /// バインディングを行います。
        /// </summary>
        public void Bind()
        {
            if (IsHandleToTarget(true))
            {
                // Source -> Targetへの通知が有効でない場合は何も設定しません。
                BindHandler(Binding.DataSource, Binding.DataSourcePropertyName,
                    new EventHandler((_, __) => OnSourceValueChanged(_, __)));
            }

            if (IsHandleToSource(true))
            {
                // Source -> Targetへの通知が有効でない場合は何も設定しません。
                BindHandler(Binding.BindableTarget, Binding.BindingPropertyName,
                    new EventHandler((_, __) => OnTargetValueChanged(_, __)));
            }

            // 初期化時は、ソースからコントロールへ値を反映します。
            OnSourceValueChanged(this, EventArgs.Empty);

            var target = Binding.BindableTarget as Component;
            if (target != null)
            {
                target.Disposed += OnUnbind;
            }

            if (Binding.Component != null)
            {
                Binding.Component.Disposed += OnUnbind;
            }

            Binding.IsBinding = true;
        }

        /// <summary>
        /// 実際のバインディング処理を行い、失敗であれば偽を返します。
        /// </summary>
        private void BindHandler(object target, string propertyName,
                                 EventHandler handler)
        {
            // targetがComponent型の場合は、'PropertyName'+Changed
            // という名前のイベントでプロパティの変更を把握します。
            if (target is IComponent)
            {
                // 名前が"PropertyName + Changed"のイベントを検索します。
                var eventName = propertyName + "Changed";
                var ev = target.GetType().GetEvent(eventName);
                if (ev != null)
                {
                    ev.AddEventHandler(target, handler);
                    Unbound += (_, __) => ev.RemoveEventHandler(target, handler);
                    return;
                }
                else if (propertyName == "SelectedItem")
                {
                    // プロパティ名がSelectedItemの場合はイベントがないことがあるので、
                    // SelectedIndexChangedを代りに探してみます。
                    ev = target.GetType().GetEvent("SelectedIndexChanged");
                    if (ev != null)
                    {
                        ev.AddEventHandler(target, handler);
                        Unbound += (_, __) => ev.RemoveEventHandler(target, handler);
                        return;
                    }
                }

                throw new InvalidOperationException(
                    $"'{propertyName}': 指定のプロパティかその変更イベントが" +
                    "コントロールに存在しません。");
            }

            // TargetがINotifyPropertyChangedの場合は
            // PropertyChangedイベントでプロパティの変更を把握します。
            var propertyObj = target as INotifyPropertyChanged;
            if (propertyObj != null)
            {
                var handler2 = new PropertyChangedEventHandler((sender, e) =>
                {
                    if (e.PropertyName == propertyName)
                    {
                        handler(sender, e);
                    }
                });

                propertyObj.PropertyChanged += handler2;
                Unbound += (_, __) => propertyObj.PropertyChanged -= handler2;
                return;
            }
        }
        #endregion
    }
}
