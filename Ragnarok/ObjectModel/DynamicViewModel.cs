#if CLR_V4
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
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
        /// 子オブジェクトから取得プロパティを検索しそれを呼びます。
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder,
                                          out object result)
        {
            var propertyName = binder.Name;

            using (LazyLock())
            {
                foreach (var model in this.dependModelList)
                {
                    var property = model.GetType().GetProperty(propertyName);

                    // プロパティの妥当性を検証します。
                    if (property == null || !property.CanRead)
                    {
                        continue;
                    }

                    // プロパティ値を取得します。
                    result = property.GetValue(model, null);
                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// 子オブジェクトから設定プロパティを検索しそれを呼びます。
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyName = binder.Name;

            using (LazyLock())
            {
                foreach (var model in this.dependModelList)
                {
                    var property = model.GetType().GetProperty(propertyName);

                    // プロパティの妥当性を検証します。
                    if (property == null || !property.CanWrite)
                    {
                        continue;
                    }

                    // プロパティ値を設定します。
                    property.SetValue(model, value, null);

                    // INotifyPropertyChangedを継承していれば各モデルの
                    // PropertyChangedイベントが呼ばれるはずなので、
                    // そこでプロパティの変更通知を改めて出します。
                    if (!(model is INotifyPropertyChanged))
                    {
                        DependModel_PropertyChanged(
                            model,
                            new PropertyChangedEventArgs(property.Name));
                    }

                    return true;
                }
            }

            return false;
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
