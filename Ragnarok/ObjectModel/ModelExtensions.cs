using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Ragnarok.ObjectModel
{
    using Ragnarok.Utility;

    /// <summary>
    /// モデルクラスの各プロパティを実装します。
    /// </summary>
    /// <remarks>
    /// 依存するプロパティとは依存プロパティのことではなく、
    /// 他のプロパティに依存して更新されるプロパティのことです。
    /// </remarks>
    public static class ModelExtensions
    {
        #region copy to DynamicViewModel
        /// <summary>
        /// 型ごとの依存するプロパティの集合を保持します。
        /// </summary>
        /// <remarks>
        /// DependOnPropertyAttribute属性を持つプロパティの集合です。
        /// </remarks>
        private static readonly Dictionary<Type, MultiMap<TypeProperty, string>>
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
        public static void RaiseAllPropertyChanged(this IModel self,
                                                   object target)
        {
            var properties = MethodUtil.GetPropertyDic(target.GetType());

            foreach (var property in properties)
            {
                if (target == self)
                {
                    self.RaisePropertyChanged(property.Key);
                }
                else
                {
                    self.DependModel_PropertyChanged(
                        target, new PropertyChangedEventArgs(property.Key));                    
                }
            }
        }

        /// <summary>
        /// 与えられた全プロパティの更新通知を出します。
        /// </summary>
        public static void RaisePropertyChanged(this IModel self,
                                                IEnumerable<string> propertyList)
        {
            // 変更されたプロパティがあれば、変更通知を送ります。
            if (propertyList == null)
            {
                return;
            }

            foreach (var propertyName in propertyList)
            {
                self.RaisePropertyChangedInternal(
                    self, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// プロパティ値とその関連プロパティ値の変更を通知します。
        /// </summary>
        public static void RaisePropertyChanged(this IModel self,
                                                string propertyName)
        {
            self.RaisePropertyChangedInternal(
                self, new PropertyChangedEventArgs(propertyName));
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
        public static void RaisePropertyChanged(this IModel self,
                                                Expression<Func<object>> expression)
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

            self.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// プロパティ値とその関連プロパティ値の変更を通知します。
        /// </summary>
        /// <remarks>
        /// senderは基本的にはselfのはずです。
        /// </remarks>
        private static void RaisePropertyChangedInternal(
            this IModel self, object sender, PropertyChangedEventArgs e)
        {
#if DEBUG && false
            self.VerifyProperty(sender, e);
#endif

            if (!object.ReferenceEquals(self, sender))
            {
                throw new ArgumentException(
                    "selfとsenderの内容が一致しません。", "sender");
            }

            // このプロパティの変更通知を送ります。
            self.RaiseSimplePropertyChanged(e);

            // このプロパティに依存しているプロパティにも変更通知を送ります。
            self.RaiseDependPropertyChanged(
                sender, e,
                self.RaisePropertyChangedInternal);
        }

        /// <summary>
        /// 依存しているモデルのプロパティ値が変更されたときに呼ばれます。
        /// </summary>
        private static void DependModel_PropertyChanged(this IModel self,
                                                        object sender,
                                                        PropertyChangedEventArgs e)
        {
#if DEBUG && false
            self.VerifyProperty(sender, e);
#endif

            // DynamicViewModelにも全く同じコードを使うので、
            // そのための#ifです。
#if CLR_GE_4_0 && RGN_DYNAMICVIEWMODEL
            if (self is System.Dynamic.DynamicObject)
            {
                // このオブジェクトのプロパティとしてプロパティの
                // 変更通知を出します。
                self.RaiseSimplePropertyChanged(e);
            }
#endif

            // 変更されたプロパティに依存するプロパティがあれば
            // それを更新します。
            self.RaiseDependPropertyChanged(
                sender, e,
                self.RaisePropertyChangedInternal);
        }

        /// <summary>
        /// senderに指定のプロパティが存在するか調べます。
        /// </summary>
        private static void VerifyProperty(this IModel self,
                                           object sender,
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
        private static void LazyModel_RaisePropertyChanged(
            this ILazyModel self, PropertyChangedEventArgs e)
        {
            // 変更プロパティを追加し、もし可能ならそれを実際に通知します。
            self.LazyModelObject.AddChangedProperty(e.PropertyName);

            self.LazyModelObject.RaisePropertyChangedIfNeed(self);
        }

        /// <summary>
        /// プロパティ値の変更を通知します。
        /// </summary>
        private static void RaiseSimplePropertyChanged(
            this IModel self, PropertyChangedEventArgs e)
        {
            var lazyModel = self as ILazyModel;

            if (lazyModel != null)
            {
                LazyModel_RaisePropertyChanged(lazyModel, e);
            }
            else
            {
                self.NotifyPropertyChanged(e);
            }
        }

        /// <summary>
        /// 依存するモデルのプロパティ値が変更されたときに呼ばれます。
        /// </summary>
        private static void RaiseDependPropertyChanged(
            this IModel self, object sender, PropertyChangedEventArgs e,
            PropertyChangedEventHandler callback)
        {
            var key = new TypeProperty(sender.GetType(), e.PropertyName);

            // 変更されたプロパティに依存するプロパティを探し、
            // もしなかったらそのまま帰ります。
            var propertySet = FindDependProperties(self.GetType(), key);
            if (propertySet == null)
            {
                return;
            }

            // 対象モデルのプロパティに依存したすべてのプロパティに
            // 変更通知を出します。
            foreach (var propertyName in propertySet)
            {
                // 依存メソッドは自クラスのもののみ調べているため、
                // 対象オブジェクトはselfになります。
                callback(
                    self,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 他プロパティに依存しているプロパティを計算します。
        /// </summary>
        private static MultiMap<TypeProperty, string> MakeDependPropertyMap(
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
        private static MultiMap<TypeProperty, string> GetDependPropertyMap(
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
        private static HashSet<string> FindDependProperties(
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
        public static void AddDependModel(this IParentModel self, object model)
        {
            self.AddDependModel(model, true);
        }

        /// <summary>
        /// 依存する他のモデルオブジェクトを追加します。
        /// </summary>
        public static void AddDependModel(this IParentModel self, object model,
                                          bool notifyAllPropertyChanged)
        {
            if (model == null)
            {
                return;
            }

            lock (self)
            {
                // もしそのモデルオブジェクトがINotifyPropertyChangedを
                // 継承していれば、そのオブジェクトのプロパティ値変更情報を
                // 使います。
                var notify = model as INotifyPropertyChanged;
                if (notify != null)
                {
                    notify.PropertyChanged += self.DependModel_PropertyChanged;
                }

                self.DependModelList.Add(model);

                // モデルが持つ全プロパティの変更通知を出します。
                // (重複したプロパティに対して通知が出されることがあります)
                if (notifyAllPropertyChanged)
                {
                    self.RaiseAllPropertyChanged(model);
                }
            }
        }

        /// <summary>
        /// 依存するモデルオブジェクトを外します。
        /// </summary>  
        public static void RemoveDependModel(this IParentModel self,
                                             object model)
        {
            if (model == null)
            {
                return;
            }

            lock (self)
            {
                // 参照比較で比較します。
                var index = self.DependModelList.FindIndex(
                    obj => object.ReferenceEquals(obj, model));
                if (index < 0)
                {
                    return;
                }

                self.DependModelList.RemoveAt(index);

                // 必要ならPropertyChangedを外します。
                var notify = model as INotifyPropertyChanged;
                if (notify != null)
                {
                    notify.PropertyChanged -= self.DependModel_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// 指定のモデルオブジェクトが依存リストに追加されたか調べます。
        /// </summary>
        public static bool HasDependModel(this IParentModel self, object model)
        {
            lock (self)
            {
                return (self.DependModelList.FindIndex(
                    obj => object.ReferenceEquals(obj, model)) >= 0);
            }
        }

        /// <summary>
        /// 依存するモデルオブジェクトすべて削除します。
        /// </summary>
        public static void ClearDependModels(this IParentModel self)
        {
            lock (self)
            {
                while (self.DependModelList.Count > 0)
                {
                    self.RemoveDependModel(self.DependModelList[0]);
                }
            }
        }
        #endregion
        #endregion copy to DynamicViewModel
    }
}
