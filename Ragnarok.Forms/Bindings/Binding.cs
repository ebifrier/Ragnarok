using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Ragnarok.Forms.Bindings
{
    using Ragnarok.Forms.Converter;
    using Ragnarok.Utility;

    /// <summary>
    /// バインディングされたプロパティ値の変更を扱います。
    /// </summary>
    public delegate void BindingPropertyChangedCallback(IComponent c, BindingPropertyChangedEventArgs e);

    /// <summary>
    /// 値の正規化を行います。
    /// </summary>
    public delegate object CoerceBindingValueCallback(IComponent c, object baseValue);

    /// <summary>
    /// バインディングを扱うクラスです。
    /// </summary>
    public sealed class Binding
    {
        private readonly ReentrancyLock recurceLock = new ReentrancyLock();
        private readonly IPropertyObject targetPropertyObject;
        private readonly IPropertyObject sourcePropertyObject;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Binding(object bindableTarget, string bindingPropertyName,
                       object dataSource, string dataSourcePropertyName)
        {
            this.targetPropertyObject = MethodUtil.GetPropertyObject(
                bindableTarget.GetType(), bindingPropertyName);
            if (this.targetPropertyObject == null)
            {
                throw new ArgumentException(
                    bindingPropertyName + ": 指定のプロパティが存在しません。");
            }

            this.sourcePropertyObject = MethodUtil.GetPropertyObject(
                dataSource.GetType(), dataSourcePropertyName);
            if (this.sourcePropertyObject == null)
            {
                throw new ArgumentException(
                    dataSourcePropertyName + ": 指定のプロパティが存在しません。");
            }

            BindableTarget = bindableTarget;
            BindingPropertyName = bindingPropertyName;
            DataSource = dataSource;
            DataSourcePropertyName = dataSourcePropertyName;
        }

        /// <summary>
        /// Unbind時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler Unbound;

        /// <summary>
        /// バインディングコレクションを保持する親クラスを取得します。
        /// </summary>
        public IComponent Component
        {
            get;
            private set;
        }

        /// <summary>
        /// バインドされるターゲットコントロールを取得します。
        /// </summary>
        public object BindableTarget
        {
            get;
            private set;
        }

        /// <summary>
        /// バインドされるコントロールのプロパティ名を取得します。
        /// </summary>
        public string BindingPropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// バインドするデータソースを取得します。
        /// </summary>
        public object DataSource
        {
            get;
            private set;
        }

        /// <summary>
        /// バインドするデータソースのプロパティ名を取得します。
        /// </summary>
        public string DataSourcePropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// モードを取得または設定します。
        /// </summary>
        public BindingMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// 値の変換を行うコンバーターを取得または設定します。
        /// </summary>
        public IValueConverter Converter
        {
            get;
            set;
        }

        /// <summary>
        /// フォーマット文字列を取得または設定します。
        /// </summary>
        public string StringFormat
        {
            get;
            set;
        }

        /// <summary>
        /// プロパティ値変更時に呼ばれるコールバックを取得または設定します。
        /// </summary>
        public BindingPropertyChangedCallback PropertyChanged
        {
            get;
            set;
        }

        /// <summary>
        /// 値の調整を行うコールバックを取得または設定します。
        /// </summary>
        public CoerceBindingValueCallback CoerceValue
        {
            get;
            set;
        }

        /// <summary>
        /// 今バインド中かどうかを取得します。
        /// </summary>
        public bool IsBinding
        {
            get;
            internal set;
        }

        #region Utility
        /// <summary>
        /// 値が変わった時、ターゲットコントロールの値の変更するか調べます。
        /// </summary>
        private bool IsHandleToTarget(bool defaultIsHandle)
        {
            return (
                (defaultIsHandle && Mode == BindingMode.Default) ||
                Mode == BindingMode.OneWay ||
                Mode == BindingMode.TwoWay);
        }

        /// <summary>
        /// 値が変わった時、ソースデータの値の変更するか調べます。
        /// </summary>
        private bool IsHandleToSource(bool defaultIsHandle)
        {
            return (
                (defaultIsHandle && Mode == BindingMode.Default) ||
                Mode == BindingMode.OneWayToSource ||
                Mode == BindingMode.TwoWay);
        }

        /// <summary>
        /// ターゲットのプロパティ値を取得します。
        /// </summary>
        private object GetTargetPropertyValue()
        {
            return this.targetPropertyObject.GetValue(BindableTarget);
        }

        /// <summary>
        /// ターゲットのプロパティ値を設定します。
        /// </summary>
        private void SetTargetPropertyValue(object value)
        {
            var type = this.targetPropertyObject.PropertyType;
            value = (type == typeof(string) ? $"{value}" : value);

            this.targetPropertyObject.SetValue(BindableTarget, value);
        }

        /// <summary>
        /// データソースのプロパティ値を取得します。
        /// </summary>
        private object GetSourcePropertyValue()
        {
            return this.sourcePropertyObject.GetValue(DataSource);
        }

        /// <summary>
        /// データソースのプロパティ値を設定します。
        /// </summary>
        private void SetSourcePropertyValue(object value)
        {
            this.sourcePropertyObject.SetValue(DataSource, value);
        }

        /// <summary>
        /// PropertyChangedイベントを発生させます。
        /// </summary>
        private void FirePropertyChanged(object newValue, object oldValue)
        {
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            Util.SafeCall(() => handler(
                Component,
                new BindingPropertyChangedEventArgs(
                    BindingPropertyName, newValue, oldValue)));
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
                if (Converter != null)
                {
                    newValue = Converter.ConvertBack(
                        newValue, this.sourcePropertyObject.PropertyType, null);
                }

                if (CoerceValue != null)
                {
                    newValue = CoerceValue(
                        Component, newValue);
                }

                // 文字列へのフォーマットを行います。
                if (!string.IsNullOrEmpty(StringFormat))
                {
                    newValue = string.Format(
                        CultureInfo.CurrentCulture,
                        StringFormat,
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

                if (Converter != null)
                {
                    newValue = Converter.Convert(
                        newValue, this.targetPropertyObject.PropertyType, null);
                }

                if (CoerceValue != null)
                {
                    newValue = CoerceValue(
                        Component, newValue);
                }

                // 文字列へのフォーマットを行います。
                if (!string.IsNullOrEmpty(StringFormat))
                {
                    newValue = string.Format(
                        CultureInfo.CurrentCulture,
                        StringFormat,
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

            var target = BindableTarget as Component;
            if (target != null)
            {
                target.Disposed -= OnUnbind;
            }

            if (Component != null)
            {
                Component.Disposed -= OnUnbind;
            }

            IsBinding = false;
        }

        /// <summary>
        /// バインディングを行います。
        /// </summary>
        public void Bind(IComponent component)
        {
            Component = component;

            if (IsHandleToTarget(true))
            {
                // Source -> Targetへの通知が有効でない場合は何も設定しません。
                BindHandler(DataSource, DataSourcePropertyName,
                    new EventHandler((_, __) => OnSourceValueChanged(_, __)));
            }

            if (IsHandleToSource(true))
            {
                // Source -> Targetへの通知が有効でない場合は何も設定しません。
                BindHandler(BindableTarget, BindingPropertyName,
                    new EventHandler((_, __) => OnTargetValueChanged(_, __)));
            }

            // 初期化時は、ソースからコントロールへ値を反映します。
            OnSourceValueChanged(this, EventArgs.Empty);

            var target = BindableTarget as Component;
            if (target != null)
            {
                target.Disposed += OnUnbind;
            }

            if (Component != null)
            {
                Component.Disposed += OnUnbind;
            }

            IsBinding = true;
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
