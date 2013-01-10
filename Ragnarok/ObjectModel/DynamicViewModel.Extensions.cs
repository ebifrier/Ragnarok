#if CLR_V4
#define RGN_DYNAMICVIEWMODEL
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Serialization;

// DynamicViewModelの一部です。
// このファイルは自動生成されています。

namespace Ragnarok.ObjectModel
{
    using Ragnarok.Utility;

    public partial class DynamicViewModel
    {
        /// <summary>
        /// 型ごとの依存するプロパティの集合を保持します。
        /// </summary>
        /// <remarks>
        /// DependOnPropertyAttribute属性を持つプロパティの集合です。
        /// </remarks>
        private readonly Dictionary<Type, MultiMap<TypeProperty, string>>
            dependPropertyDic =
            new Dictionary<Type, MultiMap<TypeProperty, string>>();

        /// <summary>
        /// プロパティの比較を行います。
        /// </summary>
        internal class TypePropertyComparer : IEqualityComparer<TypeProperty>
        {
            public bool Equals(TypeProperty attrProp, TypeProperty key)
            {
                if (attrProp.Type != key.Type)
                {
                    // IModelのDependPropertyListはDependOnProperty属性として
                    // 付加された型がキーになります。それがイベントのsenderの型と
                    // 比較されこの型の同値関係が判定されます。
                    // sender.GetType().IsSubclassOf(attrType) と判定されたとき
                    // のみは同値と判断したいため以下のようなコードになっています。
                    if (!key.Type.IsSubclassOf(attrProp.Type) &&
                        key.Type.GetInterface(attrProp.Type.FullName) == null)
                    {
                        return false;
                    }
                }

                if (attrProp.PropertyName != key.PropertyName)
                {
                    return false;
                }

                return true;
            }

            public int GetHashCode(TypeProperty x)
            {
                // 型は継承関係によっては違う型も等しいと判断されるため
                // ハッシュには追加しません。
                return x.PropertyName.GetHashCode();
            }
        }

        /// <summary>
        /// オブジェクトが持つ全プロパティの変更を通知します。
        /// </summary>
        /// <remarks>
        /// 重複したプロパティに対して通知が出されることがあります。
        /// </remarks>
        public void RaiseAllPropertyChanged(object target)
        {
            var properties = MethodUtil.GetPropertyDic(target.GetType());

            foreach (var property in properties)
            {
                if (target == this)
                {
                    this.RaisePropertyChanged(property.Key);
                }
                else
                {
                    this.DependModel_PropertyChanged(
                        target, new PropertyChangedEventArgs(property.Key));                    
                }
            }
        }

        /// <summary>
        /// 与えられた全プロパティの更新通知を出します。
        /// </summary>
        public void RaisePropertyChanged(IEnumerable<string> propertyList)
        {
            // 変更されたプロパティがあれば、変更通知を送ります。
            if (propertyList == null)
            {
                return;
            }

            foreach (var propertyName in propertyList)
            {
                this.RaisePropertyChangedInternal(
                    this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// プロパティ値とその関連プロパティ値の変更を通知します。
        /// </summary>
        public void RaisePropertyChanged(string propertyName)
        {
            this.RaisePropertyChangedInternal(
                this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 与えられたラムダ式からプロパティ名を抜き出して、イベントを発行します。
        /// </summary>
        /// <remarks>
        /// Dateプロパティを更新したい場合は、
        ///   RaisePropertyChanged(() => Date);
        /// としてください。
        /// ただし、すごく遅いので要注意。
        /// </remarks>
        public void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            MemberExpression member;
            UnaryExpression unary;
            string propertyName;

            // 以下のコンパイル結果はclrのバージョンによって
            // 変わる可能性があります。
            if ((member = expression.Body as MemberExpression) != null)
            {
                propertyName = member.Member.Name;
            }
            else if ((unary = expression.Body as UnaryExpression) != null)
            {
                member = unary.Operand as MemberExpression;
                if (member == null)
                {
                    throw new ArgumentException(
                        "メンバを扱う単項式である必要があります。",
                        "expression");
                }

                propertyName = member.Member.Name;
            }
            else
            {
                throw new ArgumentException(
                    "単項式である必要があります。", "expression");
            }

            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// プロパティ値とその関連プロパティ値の変更を通知します。
        /// </summary>
        /// <remarks>
        /// senderは基本的にはthisのはずです。
        /// </remarks>
        private void RaisePropertyChangedInternal(
            object sender, PropertyChangedEventArgs e)
        {
#if DEBUG
            this.VerifyProperty(sender, e);
#endif

            if (!object.ReferenceEquals(this, sender))
            {
                throw new ArgumentException(
                    "thisとsenderの内容が一致しません。", "sender");
            }

            // このプロパティの変更通知を送ります。
            this.RaiseSimplePropertyChanged(e);

            // このプロパティに依存しているプロパティにも変更通知を送ります。
            this.RaiseDependPropertyChanged(
                sender, e,
                this.RaisePropertyChangedInternal);
        }

        /// <summary>
        /// 依存しているモデルのプロパティ値が変更されたときに呼ばれます。
        /// </summary>
        private void DependModel_PropertyChanged(object sender,
                                                        PropertyChangedEventArgs e)
        {
#if DEBUG
            this.VerifyProperty(sender, e);
#endif

            // DynamicViewModelにも全く同じコードを使うので、
            // そのための#ifです。
#if CLR_V4 && RGN_DYNAMICVIEWMODEL
            if (this is System.Dynamic.DynamicObject)
            {
                // このオブジェクトのプロパティとしてプロパティの
                // 変更通知を出します。
                this.RaiseSimplePropertyChanged(e);
            }
#endif

            // 変更されたプロパティに依存するプロパティがあれば
            // それを更新します。
            this.RaiseDependPropertyChanged(
                sender, e,
                this.RaisePropertyChangedInternal);
        }

        /// <summary>
        /// senderに指定のプロパティが存在するか調べます。
        /// </summary>
        private void VerifyProperty(object sender,
                                           PropertyChangedEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            if (e == null || string.IsNullOrEmpty(e.PropertyName))
            {
                throw new ArgumentException(
                    "プロパティ名がありません。", "e");
            }

#if !RGN_DYNAMICVIEWMODEL
            // 変更通知の対象となるプロパティを取得します。
            var propertyDic = MethodUtil.GetPropertyDic(sender.GetType());
            if (propertyDic == null)
            {
                throw new RagnarokException(
                    "プロパティリストが取得できませんでした。");
            }

            if (!propertyDic.ContainsKey(e.PropertyName))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "型'{0}'にプロパティ'{1}'は存在しません。",
                        sender.GetType(), e.PropertyName));
            }
#endif
        }

        /// <summary>
        /// ILazyModelのプロパティ値変更通知を出します。
        /// </summary>
        private void LazyModel_RaisePropertyChanged(
            PropertyChangedEventArgs e)
        {
            // 変更プロパティを追加し、もし可能ならそれを実際に通知します。
            this.lazyModelObject.AddChangedProperty(e.PropertyName);

            this.lazyModelObject.RaisePropertyChangedIfNeed(this);
        }

        /// <summary>
        /// プロパティ値の変更を通知します。
        /// </summary>
        private void RaiseSimplePropertyChanged(
            PropertyChangedEventArgs e)
        {
            var lazyModel = this as ILazyModel;

            if (lazyModel != null)
            {
                LazyModel_RaisePropertyChanged(e);
            }
            else
            {
                this.NotifyPropertyChanged(e);
            }
        }

        /// <summary>
        /// 依存するモデルのプロパティ値が変更されたときに呼ばれます。
        /// </summary>
        private void RaiseDependPropertyChanged(
            object sender, PropertyChangedEventArgs e,
            PropertyChangedEventHandler callback)
        {
            var key = new TypeProperty(sender.GetType(), e.PropertyName);

            // 変更されたプロパティに依存するプロパティを探し、
            // もしなかったらそのまま帰ります。
            var propertySet = FindDependProperties(this.GetType(), key);
            if (propertySet == null)
            {
                return;
            }

            // 対象モデルのプロパティに依存したすべてのプロパティに
            // 変更通知を出します。
            foreach (var propertyName in propertySet)
            {
                // 依存メソッドは自クラスのもののみ調べているため、
                // 対象オブジェクトはthisになります。
                callback(
                    this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 他プロパティに依存しているプロパティを計算します。
        /// </summary>
        private MultiMap<TypeProperty, string> MakeDependPropertyMap(
            Type targetType)
        {
            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty |
                BindingFlags.Public |
                BindingFlags.NonPublic;
            var propertyList = MethodUtil.GetThisAndInheritClasses(targetType)
                .SelectMany(klass => klass.GetProperties(flags));

            var result = new MultiMap<TypeProperty, string>(
                new TypePropertyComparer());

            // このクラスのプロパティから、DependOnPropertyAttributeを持つ
            // ものを抜き出します。
            //
            // 作成するものがMultiMapなので、Linqは使いにくいです。
            foreach (var property in propertyList)
            {
                var attrList = property.GetCustomAttributes(
                    typeof(DependOnPropertyAttribute), true);

                foreach (var attr in attrList)
                {
                    var dependAttr = attr as DependOnPropertyAttribute;
                    if (dependAttr == null)
                    {
                        continue;
                    }

                    // 依存先の型がないときは自分の型と同じになります。
                    var dependType = 
                        (dependAttr.DependType ?? targetType);

                    // 依存先プロパティの名前がないときは、
                    // 依存先のプロパティ名は依存元と同じになります。
                    var dependName =
                        ( !string.IsNullOrEmpty(dependAttr.DependName)
                        ? dependAttr.DependName
                        : property.Name);

                    // 依存元のプロパティに依存先のプロパティを紐つけます。
                    result.Add(
                        new TypeProperty(dependType, dependName),
                        property.Name);
                }
            }

            return result;
        }

        /// <summary>
        /// ある型が持つDependOn属性を持つプロパティの集合を取得します。
        /// </summary>
        private MultiMap<TypeProperty, string> GetDependPropertyMap(
            Type targetType)
        {
            MultiMap<TypeProperty, string> dependMap;

            lock (dependPropertyDic)
            {
                if (dependPropertyDic.TryGetValue(targetType, out dependMap))
                {
                    return dependMap;
                }
            }

            // DependOn属性を持つプロパティの集合を作成し追加します。
            dependMap = MakeDependPropertyMap(targetType);
            lock (dependPropertyDic)
            {
                dependPropertyDic.Add(targetType, dependMap);
            }

            return dependMap;
        }

        /// <summary>
        /// 与えられたプロパティに依存するプロパティのリストを取得します。
        /// </summary>
        private HashSet<string> FindDependProperties(
            Type targetType, TypeProperty property)
        {
            var dependMap = GetDependPropertyMap(targetType);
            if (dependMap == null)
            {
                throw new RagnarokException(
                    "依存モデルのプロパティリストが取得できませんでした。");
            }

            return dependMap.GetValues(property, false);
        }

        #region IParentModelBase
        /// <summary>
        /// 依存する他のモデルオブジェクトを追加します。
        /// </summary>
        public void AddDependModel(object model)
        {
            this.AddDependModel(model, true);
        }

        /// <summary>
        /// 依存する他のモデルオブジェクトを追加します。
        /// </summary>
        public void AddDependModel(object model,
                                          bool notifyAllPropertyChanged)
        {
            if (model == null)
            {
                return;
            }

            using (new DebugLock(SyncRoot))
            {
                // もしそのモデルオブジェクトがINotifyPropertyChangedを
                // 継承していれば、そのオブジェクトのプロパティ値変更情報を
                // 使います。
                var notify = model as INotifyPropertyChanged;
                if (notify != null)
                {
                    notify.PropertyChanged += this.DependModel_PropertyChanged;
                }

                this.dependModelList.Add(model);

                // モデルが持つ全プロパティの変更通知を出します。
                // (重複したプロパティに対して通知が出されることがあります)
                if (notifyAllPropertyChanged)
                {
                    this.RaiseAllPropertyChanged(model);
                }
            }
        }

        /// <summary>
        /// 依存するモデルオブジェクトを外します。
        /// </summary>  
        public void RemoveDependModel(object model)
        {
            if (model == null)
            {
                return;
            }

            using (new DebugLock(SyncRoot))
            {
                // 参照比較で比較します。
                var index = this.dependModelList.FindIndex(
                    obj => object.ReferenceEquals(obj, model));
                if (index < 0)
                {
                    return;
                }

                this.dependModelList.RemoveAt(index);

                // 必要ならPropertyChangedを外します。
                var notify = model as INotifyPropertyChanged;
                if (notify != null)
                {
                    notify.PropertyChanged -= this.DependModel_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// 指定のモデルオブジェクトが依存リストに追加されたか調べます。
        /// </summary>
        public bool HasDependModel(object model)
        {
            using (new DebugLock(SyncRoot))
            {
                return (this.dependModelList.FindIndex(
                    obj => object.ReferenceEquals(obj, model)) >= 0);
            }
        }

        /// <summary>
        /// 依存するモデルオブジェクトすべて削除します。
        /// </summary>
        public void ClearDependModels()
        {
            using (new DebugLock(SyncRoot))
            {
                while (this.dependModelList.Count > 0)
                {
                    this.RemoveDependModel(this.dependModelList[0]);
                }
            }
        }
        #endregion
    }
}
#endif
