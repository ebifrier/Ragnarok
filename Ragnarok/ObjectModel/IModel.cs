using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// モデルクラスの基底です。
    /// </summary>
    public interface IModel : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティの変更通知を出します。
        /// </summary>
        void NotifyPropertyChanged(PropertyChangedEventArgs e);
    }

    /// <summary>
    /// 子モデルを持つモデルクラスの基底です。
    /// </summary>
    public interface IParentModel : IModel
    {
        /// <summary>
        /// 依存モデルのリストです。
        /// </summary>
        /// <remarks>
        /// ModelExtensions.AddDependModelなどから変更してください。
        /// 初期化は new List&lt;object&gt;() で初期化してください。
        /// </remarks>
        List<object> DependModelList { get; }
    }

    /// <summary>
    /// プロパティの変更通知を遅くするモデルクラスの基底です。
    /// </summary>
    public interface ILazyModel : IModel
    {
        /// <summary>
        /// 変更されたプロパティ一覧などを保存するために必要なオブジェクトです。
        /// </summary>
        LazyModelObject LazyModelObject { get; }
    }
}
