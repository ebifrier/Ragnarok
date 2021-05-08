using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    public class NotifyObject : IParentModel, ILazyModel, IEnumerable<KeyValuePair<string, object>>
    {
        [field: NonSerialized]
        private object syncRoot = new();
        [field: NonSerialized]
        private List<object> dependModelList = new();
        [field: NonSerialized]
        private LazyModelObject lazyModelObject = new();
        [field: NonSerialized]
        private Dictionary<string, object> propDic = new();
        [field: NonSerialized]
        private List<PropertyChangedObject> changedList = new();

        /// <summary>
        /// 逆シリアル前に呼ばれます。
        /// </summary>
        /// <remarks>
        /// 逆シリアル時にはコンストラクタが呼ばれないことがあるため、
        /// ここで各オブジェクトの初期化を行っています。
        /// </remarks>
        [OnDeserializing()]
        private void OnBeforeDeselialize(StreamingContext context)
        {
            this.syncRoot = new object();
            this.dependModelList = new List<object>();
            this.lazyModelObject = new LazyModelObject();
            this.propDic = new Dictionary<string, object>();
            this.changedList = new List<PropertyChangedObject>();
        }

        /// <summary>
        /// プロパティ値の変更イベントです。
        /// </summary>
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// LazyLockをExitするときに呼ばれるイベントです。
        /// </summary>
        public event EventHandler FiresOnExit
        {
            add { this.lazyModelObject.FiresOnExit += value; }
            remove { this.lazyModelObject.FiresOnExit -= value; }
        }

        /// <summary>
        /// 依存モデルのリストです。
        /// </summary>
        public List<object> DependModelList
        {
            get { return this.dependModelList; }
        }

        /// <summary>
        /// PropertyChangedイベントの遅延実行オブジェクトを取得します。
        /// </summary>
        public LazyModelObject LazyModelObject
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
        /// プロパティの数を取得します。
        /// </summary>
        public int Count
        {
            get
            {
                using (LazyLock())
                {
                    return this.propDic.Count;
                }
            }
        }

        /// <summary>
        /// 持っているプロパティの巡回用メソッドを定義します。
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetPropertyData().GetEnumerator();
        }

        /// <summary>
        /// 持っているプロパティの巡回用メソッドを定義します。
        /// </summary>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return GetPropertyData().GetEnumerator();
        }

        /// <summary>
        /// オブジェクトに登録されたプロパティデータを取得します。
        /// </summary>
        public Dictionary<string, object> GetPropertyData()
        {
            using (LazyLock())
            {
                return new Dictionary<string, object>(this.propDic);
            }
        }

        /// <summary>
        /// 指定の名前のプロパティが含まれているか調べます。
        /// </summary>
        public bool Contains(string name)
        {
            using (LazyLock())
            {
                return this.propDic.ContainsKey(name);
            }
        }

        /// <summary>
        /// 指定の名前のプロパティの値がもしあれば、それを取得します。
        /// </summary>
        public bool TryGetValue<T>(string name, out T value)
        {
            using (LazyLock())
            {
                if (!this.propDic.TryGetValue(name, out object current))
                {
                    value = default;
                    return false;
                }

                value = (T)current;
                return true;
            }
        }

        /// <summary>
        /// 指定の名前のプロパティの削除を試し、もし削除出来たら真を返します。
        /// </summary>
        public bool Remove(string name)
        {
            using (LazyLock())
            {
                var removed = this.propDic.Remove(name);
                if (removed)
                {
                    this.RaisePropertyChanged(name);
                }

                return removed;
            }
        }

        /// <summary>
        /// 内部辞書に保持されたプロパティ値を取得します。
        /// </summary>
        protected T GetValue<T>(string name)
        {
            using (LazyLock())
            {
                if (!this.propDic.TryGetValue(name, out object current))
                {
                    return default;
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
                if (!this.propDic.TryGetValue(name, out object current) ||
                    !Util.GenericEquals(current, value))
                {
                    this.propDic[name] = value;

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

            var changed = new PropertyChangedObject
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
                this.changedList.Add(changed);

                PropertyChanged += changed.Handler;
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
                var target = this.changedList.FirstOrDefault(obj =>
                    obj.Name == propertyName &&
                    obj.InternalHandler == handler);
                if (target == null)
                {
                    return;
                }

                this.changedList.Remove(target);

                PropertyChanged -= target.Handler;
            }
        }
    }
}
