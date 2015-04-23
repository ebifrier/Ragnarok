using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// プロパティごとの変更通知を保持します。
    /// </summary>
    internal sealed class PropertyChangedObject
    {
        /// <summary>
        /// プロパティ名を取得または設定します。
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// コールバックハンドラを取得または設定します。
        /// </summary>
        public PropertyChangedEventHandler Handler
        {
            get;
            set;
        }

        /// <summary>
        /// 内部のコールバックハンドラを取得または設定します。
        /// </summary>
        public PropertyChangedEventHandler InternalHandler
        {
            get;
            set;
        }
    }

    /// <summary>
    /// WPFで動くモデルオブジェクトの基底クラスです。
    /// </summary>
    /// <remarks>
    /// 逆シリアル前には必ず<see cref="OnBeforeDeselialize"/>メソッドを
    /// 呼ぶようにしてください。
    /// </remarks>
    [Serializable()]
    [DataContract()]
    public class NotifyObject : IParentModel, ILazyModel
    {
        #region copy to DynamicViewModel
        [field: NonSerialized]
        private object syncRoot = new object();
        [field: NonSerialized]
        private List<object> dependModelList = new List<object>();
        [field: NonSerialized]
        private LazyModelObject lazyModelObject = new LazyModelObject();
        [field: NonSerialized]
        private Dictionary<string, object> propDic =
            new Dictionary<string, object>();
        [field: NonSerialized]
        private List<PropertyChangedObject> propObjects =
            new List<PropertyChangedObject>();

        /// <summary>
        /// 逆シリアル前に呼ばれます。
        /// </summary>
        /// <remarks>
        /// 逆シリアル時にはコンストラクタが呼ばれないことがあるため、
        /// ここで各オブジェクトの初期化を行っています。
        /// </remarks>
        [OnDeserializing()]
        protected void OnBeforeDeselialize(StreamingContext context)
        {
            this.syncRoot = new object();
            this.dependModelList = new List<object>();
            this.lazyModelObject = new LazyModelObject();
            this.propDic = new Dictionary<string, object>();
            this.propObjects = new List<PropertyChangedObject>();
        }

        /// <summary>
        /// プロパティ値の変更イベントです。
        /// </summary>
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 依存モデルのリストです。
        /// </summary>
        List<object> IParentModel.DependModelList
        {
            get { return this.dependModelList; }
        }

        /// <summary>
        /// PropertyChangedイベントの遅延実行オブジェクトを取得します。
        /// </summary>
        LazyModelObject ILazyModel.LazyModelObject
        {
            get { return this.lazyModelObject; }
        }

        /// <summary>
        /// 同期用のオブジェクトを取得します。
        /// </summary>
        public object SyncRoot
        {
            get { return this.syncRoot; }
        }

        /// <summary>
        /// 専用のロックオブジェクトを作成します。
        /// </summary>
        protected LazyModelLock LazyLock()
        {
            return new LazyModelLock(this, SyncRoot);
        }

        /// <summary>
        /// プロパティの変更通知を出します。
        /// </summary>
        public virtual void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            Util.SafeCall(() => OnPropertyChanged(e));
            Util.CallPropertyChanged(PropertyChanged, this, e);
        }

        /// <summary>
        /// プロパティ変更時に呼ばれます。
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// 指定の名前のプロパティが含まれているか調べます。
        /// </summary>
        protected bool Contains(string name)
        {
            using (LazyLock())
            {
                return this.propDic.ContainsKey(name);
            }
        }

        /// <summary>
        /// 内部辞書に保持されたプロパティ値を取得します。
        /// </summary>
        protected T GetValue<T>(string name)
        {
            using (LazyLock())
            {
                object current;

                if (!this.propDic.TryGetValue(name, out current))
                {
                    return default(T);
                }

                return (T)current;
            }
        }

        /// <summary>
        /// 内部辞書に保持されたプロパティ値を設定します。
        /// </summary>
        protected void SetValue<T>(string name, T value)
        {
            using (LazyLock())
            {
                object current;

                if (!this.propDic.TryGetValue(name, out current) ||
                    !Util.GenericEquals(current, value))
                {
                    this.propDic[name] = value;

                    this.RaisePropertyChanged(name);
                }
            }
        }

        /// <summary>
        /// プロパティに値を設定し、必要ならプロパティ変更通知を出します。
        /// </summary>
        protected void SetValue<T>(string name, T value, ref T property)
        {
            using (LazyLock())
            {
                if (!Util.GenericEquals(property, value))
                {
                    property = value;

                    this.RaisePropertyChanged(name);
                }
            }
        }

        /// <summary>
        /// プロパティごとにプロパティ変更通知を受け取るようにします。
        /// </summary>
        public void AddPropertyChangedHandler(string propertyName,
                                              PropertyChangedEventHandler handler)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (handler == null)
            {
                return;
            }

            var propObj = new PropertyChangedObject
            {
                Name = propertyName,
                InternalHandler = handler,
                Handler = (sender, e) =>
                {
                    if (e.PropertyName != propertyName)
                    {
                        return;
                    }

                    handler(sender, e);
                },
            };

            using (LazyLock())
            {
                this.propObjects.Add(propObj);

                PropertyChanged += propObj.Handler;
            }
        }

        /// <summary>
        /// プロパティごとのプロパティ変更通知を削除します。
        /// </summary>
        public void RemovePropertyChangedHandler(string propertyName,
                                                 PropertyChangedEventHandler handler)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (handler == null)
            {
                return;
            }

            using (LazyLock())
            {
                var target = this.propObjects.FirstOrDefault(obj =>
                    obj.Name == propertyName &&
                    obj.InternalHandler == handler);
                if (target == null)
                {
                    return;
                }

                this.propObjects.Remove(target);

                PropertyChanged -= target.Handler;
            }
        }
        #endregion copy to DynamicViewModel
    }
}
