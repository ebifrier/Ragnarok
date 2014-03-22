#if CLR_V4
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// ビューモデルの基底クラスです。
    /// </summary>
    /// <remarks>
    /// このクラスは依存するモデルクラスのプロパティをダイナミッククラスを
    /// 使って自動的に委譲します。つまり、登録されたモデルクラスにTargetという
    /// プロパティがあれば、このクラスにもTargetというプロパティが
    /// 自動的に追加されます。
    /// 
    /// また、モデルクラスからのプロパティ変更通知もこのクラスの
    /// プロパティ変更通知に変換されて自動的に発行されます。
    /// </remarks>
    public partial class DynamicViewModel
        : DynamicObject, IParentModel, ILazyModel
    {
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
                        string.Format(
                            "{0}: 指定のプロパティは存在しません。",
                            key));
                }

                return value;
            }
            set
            {
                if (!TrySetValue(key, value))
                {
                    throw new KeyNotFoundException(
                        string.Format(
                            "{0}: 指定のプロパティには書き込みできません。",
                            key));
                }
            }
        }

        /// <summary>
        /// 子オブジェクトから取得プロパティを検索しそれを呼びます。
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder,
                                          out object result)
        {
            return TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// 子オブジェクトから取得プロパティを検索しそれを呼びます。
        /// </summary>
        public override bool TryGetIndex(GetIndexBinder binder,
                                         object[] indexes,
                                         out object result)
        {
            var name = indexes[0] as string;

            return TryGetValue(name, out result);
        }

        /// <summary>
        /// 子オブジェクトから取得プロパティを検索しそれを呼びます。
        /// </summary>
        public bool TryGetValue(string propertyName,
                                out object result)
        {
            using (LazyLock())
            {
                // 自クラスのプロパティも検索します。
                var pair = new[] { this }
                    .Concat(this.dependModelList)
                    .Select(_ => new
                    {
                        Model = _,
                        Property = _.GetType().GetProperty(propertyName),
                    })
                    .Where(_ => _.Property != null)
                    .Where(_ => _.Property.CanRead)
                    .FirstOrDefault();

                if (pair != null)
                {
                    result = pair.Property.GetValue(pair.Model, null);
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// 子オブジェクトから設定プロパティを検索しそれを呼びます。
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TrySetValue(binder.Name, value);
        }

        /// <summary>
        /// 子オブジェクトから設定プロパティを検索しそれを呼びます。
        /// </summary>
        public override bool TrySetIndex(SetIndexBinder binder,
                                         object[] indexes, object value)
        {
            var name = indexes[0] as string;

            return TrySetValue(name, value);
        }

        /// <summary>
        /// 子オブジェクトから設定プロパティを検索しそれを呼びます。
        /// </summary>
        public bool TrySetValue(string propertyName, object value)
        {
            using (LazyLock())
            {
                // 自クラスのプロパティも検索します。
                var pair = new[] { this }
                    .Concat(this.dependModelList)
                    .Select(_ => new
                    {
                        Model = _,
                        Property = _.GetType().GetProperty(propertyName),
                    })
                    .Where(_ => _.Property != null)
                    .Where(_ => _.Property.CanWrite)
                    .FirstOrDefault();

                if (pair == null)
                {
                    return false;
                }

                this.RaisePropertyChanging(pair.Model, pair.Property.Name);

                // プロパティ値を設定します。
                pair.Property.SetValue(pair.Model, value, null);

                // INotifyPropertyChangedを継承していれば各モデルの
                // PropertyChangedイベントが呼ばれるはずなので、
                // そこでプロパティの変更通知を改めて出します。
                if (!(pair.Model is INotifyPropertyChanged))
                {
                    DependModel_PropertyChanged(
                        pair.Model,
                        new PropertyChangedEventArgs(pair.Property.Name));
                }

                return true;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicViewModel(object model)
        {
            AddDependModel(model);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicViewModel()
        {
        }
    }
}
#endif
