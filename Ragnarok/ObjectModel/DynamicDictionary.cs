#if CLR_GE_4_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Globalization;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// <see cref="DynamicDictionary"/>のプロパティ情報を保持します。
    /// </summary>
    public sealed class DynamicPropertyInfo
    {
        /// <summary>
        /// プロパティの名前を取得します。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// プロパティの型を取得します。
        /// </summary>
        public Type PropertyType
        {
            get;
            private set;
        }

        /// <summary>
        /// デフォルト値を持つかどうかを取得します。
        /// </summary>
        public bool HasDefaultValue
        {
            get;
            private set;
        }

        /// <summary>
        /// デフォルト値を取得します。
        /// </summary>
        public object DefaultValue
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicPropertyInfo(string name, Type propertyType)
        {
            Name = name;
            PropertyType = propertyType;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicPropertyInfo(string name, Type propertyType,
                                   object defaultValue)
            : this(name, propertyType)
        {
            HasDefaultValue = true;
            DefaultValue = defaultValue;
        }
    }

    /// <summary>
    /// dynamicでアクセスしたすべてのプロパティをプロパティとして扱います。
    /// </summary>
    public class DynamicDictionary : DynamicObject, ILazyModel
    {
        [field: NonSerialized]
        private object syncRoot = new object();
        [field: NonSerialized]
        private LazyModelObject lazyModelObject = new LazyModelObject();
        private Dictionary<string, DynamicPropertyInfo> propInfoDic =
            new Dictionary<string, DynamicPropertyInfo>();
        private Dictionary<string, object> propDic =
            new Dictionary<string, object>();
        private bool isExtendable = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicDictionary()
            : this(null, false)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicDictionary(IEnumerable<DynamicPropertyInfo> propInfoDic,
                                 bool isExtendable)
        {
            this.propInfoDic = (propInfoDic != null ?
                propInfoDic.ToDictionary(_ => _.Name) :
                new Dictionary<string, DynamicPropertyInfo>());
            this.isExtendable = isExtendable;

            // デフォルト値を設定します。
            /*this.propInfoDic
                .Where(_ => _.Value.HasDefaultValue)
                .ForEach(_ => this[_.Key] = _.Value.DefaultValue);*/
        }

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
            this.lazyModelObject = new LazyModelObject();
            this.propInfoDic = new Dictionary<string, DynamicPropertyInfo>();
            this.propDic = new Dictionary<string, object>();
            this.isExtendable = true;
        }

        /// <summary>
        /// プロパティ値の変更イベントです。
        /// </summary>
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

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
        /// プロパティを拡張可能かどうかを取得します。
        /// </summary>
        public bool IsExtendable
        {
            get { return this.isExtendable; }
        }

        /// <summary>
        /// プロパティ名の一覧を取得します。
        /// </summary>
        public List<string> PropertyNames
        {
            get
            {
                using (LazyLock())
                {
                    return this.propInfoDic.Keys
                        .Concat(this.propDic.Keys)
                        .Distinct()
                        .ToList();
                }
            }
        }

        /// <summary>
        /// 変更されたプロパティ名の一覧を取得します。
        /// </summary>
        public List<string> ChangedPropertyNames
        {
            get
            {
                using (LazyLock())
                {
                    return this.propDic.Keys.ToList();
                }
            }
        }

        /// <summary>
        /// 指定の名前のプロパティ値を取得または設定します。
        /// </summary>
        public object this[string key]
        {
            get
            {
                object value;
                if (!TryGetValue(key, out value))
                {
                    throw new KeyNotFoundException(
                        $"{key}: 指定のプロパティは存在しません。");
                }

                return value;
            }
            set
            {
                if (!TrySetValue(key, value))
                {
                    throw new KeyNotFoundException(
                        $"{key}: 指定のプロパティには書き込みできません。");
                }
            }
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
        /// プロパティ値を取得します。
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder,
                                          out object result)
        {
            if (binder == null)
            {
                throw new ArgumentNullException(nameof(binder));
            }

            return TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// プロパティ値を設定します。
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder == null)
            {
                throw new ArgumentNullException(nameof(binder));
            }

            return TrySetValue(binder.Name, value);
        }

        /// <summary>
        /// プロパティ値を取得します。
        /// </summary>
        public bool TryGetValue(string name, out object result)
        {
            using (LazyLock())
            {
                if (this.propDic.TryGetValue(name, out result))
                {
                    return true;
                }

                DynamicPropertyInfo propInfo;
                if (this.propInfoDic.TryGetValue(name, out propInfo))
                {
                    // デフォルト値を返します。
                    result = (propInfo.HasDefaultValue ?
                        propInfo.DefaultValue :
                        Util.GetDefaultValue(propInfo.PropertyType));

                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// プロパティ値を設定します。
        /// </summary>
        public bool TrySetValue(string name, object value)
        {
            using (LazyLock())
            {
                DynamicPropertyInfo propInfo;
                this.propInfoDic.TryGetValue(name, out propInfo);

                // 拡張可能で無ければ、可能なプロパティ名は決まっています。
                if (!this.isExtendable && propInfo == null)
                {
                    return false;
                }

                // valueを所望の方にキャストし直します。
                // 正確に型を合わせるために必要です。
                if (propInfo != null && value is IConvertible)
                {
                    value = Convert.ChangeType(
                        value,
                        propInfo.PropertyType,
                        CultureInfo.CurrentCulture);
                }

                object current;
                if (!this.propDic.TryGetValue(name, out current) ||
                    !Util.GenericEquals(current, value))
                {
                    this.propDic[name] = value;

                    this.RaisePropertyChanged(name);
                }

                return true;
            }
        }
    }
}
#endif
