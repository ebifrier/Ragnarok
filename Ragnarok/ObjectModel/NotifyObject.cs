using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// WPFで動くモデルオブジェクトの基底クラスです。
    /// </summary>
    /// <remarks>
    /// 逆シリアル前には必ず<see cref="OnBeforeDeselialize"/>メソッドを
    /// 呼ぶようにしてください。
    /// </remarks>
    [DataContract()]
    public class NotifyObject : IParentModel, ILazyModel
    {
        [field: NonSerialized]
        private object syncRoot = new object();
        [field: NonSerialized]
        private List<object> dependModelList = new List<object>();
        [field: NonSerialized]
        private LazyModelObject lazyModelObject = new LazyModelObject();
        [field: NonSerialized]
        private Dictionary<string, object> propDic = new Dictionary<string, object>();

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
            Util.CallPropertyChanged(PropertyChanged, this, e);
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
    }
}
