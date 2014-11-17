using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Ragnarok.Utility
{
    /// <summary>
    /// プロパティの最適化されたgetterとsetterを保持します。
    /// </summary>
    public interface IPropertyObject
    {
        /// <summary>
        /// プロパティ情報を取得します。
        /// </summary>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// プロパティ名を取得します。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// プロパティの型を取得します。
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// 読み込み可能かどうかを取得します。
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// 書き込み可能かどうかを取得します。
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// プロパティ値を取得します。
        /// </summary>
        object GetValue(object target);

        /// <summary>
        /// プロパティ値を設定します。
        /// </summary>
        void SetValue(object target, object value);
    }

    /// <summary>
    /// プロパティのgetterとsetterを保持します。
    /// </summary>
    internal sealed class PropertyObject<TTarget, TValue> : IPropertyObject
    {
        private delegate TValue GetterType(TTarget target);
        private delegate void SetterType(TTarget target, TValue value);

        private readonly object syncRoot = new object();

        // monoで使えないので、Lazyクラスは使用しません。
        private bool isGetterCreated;
        private bool isSetterCreated;
        private GetterType getterInternal;
        private SetterType setterInternal;

        /// <summary>
        /// プロパティ情報を取得します。
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// プロパティ名を取得します。
        /// </summary>
        public string Name
        {
            get { return PropertyInfo.Name; }
        }

        /// <summary>
        /// プロパティの型を取得します。
        /// </summary>
        public Type PropertyType
        {
            get { return PropertyInfo.PropertyType; }
        }

        /// <summary>
        /// プロパティ値取得用メソッドを作成or取得します。
        /// </summary>
        private GetterType Getter
        {
            get
            {
                lock(syncRoot)
                {
                    if (!this.isGetterCreated)
                    {
                        this.getterInternal = MakeGetter();
                        this.isGetterCreated = true;
                    }

                    return this.getterInternal;
                }
            }
        }

        /// <summary>
        /// プロパティ値設定用メソッドを作成or取得します。
        /// </summary>
        private SetterType Setter
        {
            get
            {
                lock (syncRoot)
                {
                    if (!this.isSetterCreated)
                    {
                        this.setterInternal = MakeSetter();
                        this.isSetterCreated = true;
                    }

                    return this.setterInternal;
                }
            }
        }

        /// <summary>
        /// 読み込み可能かどうかを取得します。
        /// </summary>
        public bool CanRead
        {
            get
            {
                return (Getter != null);
            }
        }

        /// <summary>
        /// 書き込み可能かどうかを取得します。
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return (Setter != null);
            }
        }

        /// <summary>
        /// 値を取得します。
        /// </summary>
        public object GetValue(object target)
        {
            var getter = Getter;
            if (getter == null)
            {
                throw new InvalidOperationException(
                    "取得不可能なプロパティです。");
            }

            // staticプロパティならtargetはnullに成るはずなので、
            // チェックしません。
            var tTarget = (TTarget)target;

            return getter(tTarget);
        }

        /// <summary>
        /// 値を設定します。
        /// </summary>
        public void SetValue(object target, object value)
        {
            var setter = Setter;
            if (setter == null)
            {
                throw new InvalidOperationException(
                    "設定不可能なプロパティです。");
            }

            // staticプロパティならtargetはnullに成るはずなので、
            // チェックしません。
            var tTarget = (TTarget)target;
            var tValue = (TValue)value;

            setter(tTarget, tValue);
        }

        /// <summary>
        /// 最適化された取得メソッドを作成します。
        /// </summary>
        /// <remarks>
        /// ExpressionTreeを使います。
        /// </remarks>
        private GetterType MakeGetter()
        {
            try
            {
                if (!PropertyInfo.CanRead)
                {
                    return null;
                }

                var method = PropertyInfo.GetGetMethod(false);
                if (method == null)
                {
                    method = PropertyInfo.GetGetMethod(true);
                    if (method == null)
                    {
                        return null;
                    }
                }

                // 作成するラムダ関数
                // target => target.method()

                // "target"という引数を定義。
                var target = Expression.Parameter(typeof(TTarget), "target");
                
                // "target"の"method"メソッドを呼び出します。
                var body = Expression.Call(target, method);

                // bodyをラムダとして定義。
                var e = Expression.Lambda(
                    typeof(GetterType), body, target);

                return (GetterType)e.Compile();
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "{0}.{1}: 取得メソッドのコンパイルに失敗しました。",
                    typeof(TTarget),
                    PropertyInfo.Name);

                return null;
            }
        }

        /// <summary>
        /// 最適化された設定メソッドを作成します。
        /// </summary>
        /// <remarks>
        /// ExpressionTreeを使います。
        /// </remarks>
        private SetterType MakeSetter()
        {
            try
            {
                if (!PropertyInfo.CanWrite)
                {
                    return null;
                }

                var method = PropertyInfo.GetSetMethod(false);
                if (method == null)
                {
                    method = PropertyInfo.GetSetMethod(true);
                    if (method == null)
                    {
                        return null;
                    }
                }

                // 作成するラムダ関数
                // (target, value) => target.method(value)

                // "target", "value"という引数を定義。
                var target = Expression.Parameter(typeof(TTarget), "target");
                var value = Expression.Parameter(typeof(TValue), "value");

                // "value"を引数として、"target"の"method"メソッドを呼び出します。
                var body = Expression.Call(target, method, value);

                // メソッド呼び出しをラムダとして定義。
                var param = new[] { target, value };
                var e = Expression.Lambda(
                    typeof(SetterType), body, param);

                return (SetterType)e.Compile();
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "{0}.{1}: 設定メソッドのコンパイルに失敗しました。",
                    typeof(TTarget),
                    PropertyInfo.Name);

                return null;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PropertyObject(PropertyInfo property)
        {
            PropertyInfo = property;
        }
    }

    /// <summary>
    /// メソッドやプロパティの操作メソッドを提供します。
    /// </summary>
    public static class MethodUtil
    {
        /// <summary>
        /// プロパティ名からプロパティを得るために使います。
        /// </summary>
        /// <remarks>
        /// 同名のプロパティがある場合は派生クラスを優先します。
        /// </remarks>
        private static readonly Dictionary<Type, Dictionary<string, IPropertyObject>>
            TypePropertySet =
                new Dictionary<Type, Dictionary<string, IPropertyObject>>();

        /// <summary>
        /// このクラスと継承クラスのリストを取得します。
        /// </summary>
        /// <remarks>
        /// <paramref name="targetType"/>に近いクラスから順次返されます。
        /// </remarks>
        public static IEnumerable<Type> GetThisAndInheritClasses(
            Type targetType)
        {
            // 継承クラス群を取得します。
            for (var type = targetType;
                 type != typeof(object) && type != null;
                 type = type.BaseType)
            {
                yield return type;
            }
        }

        /// <summary>
        /// PropertyObjectを作成します。
        /// </summary>
        private static IPropertyObject CreatePropertyObject(Type klass,
                                                            PropertyInfo property)
        {
            try
            {
                // PropertyObject<TTarget, TValue>型のオブジェクトを作成します。
                var genericType = typeof(PropertyObject<,>);
                var type = genericType.MakeGenericType(
                    klass,
                    property.PropertyType);

                return (IPropertyObject)
                    Activator.CreateInstance(type, property);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "PropertyObjectの作成に失敗しました。");

                return null;
            }
        }

        /// <summary>
        /// このクラスと継承クラスが持つプロパティを取得します。
        /// </summary>
        private static Dictionary<string, IPropertyObject> GetPropertyDicImpl(
            Type targetType)
        {
            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty |
                BindingFlags.Public |
                BindingFlags.NonPublic;
            var result = new Dictionary<string, IPropertyObject>();

            // ちょっとめんどくさいですが、プロパティの重複を無くすために
            // 必要になります。
            //
            // Linqを使うと順番が保証されなくなってしまうため、使っていません。
            // 派生クラスのプロパティの方が先にある必要があります。
            var classList = GetThisAndInheritClasses(targetType);

            foreach (var klass in classList)
            {
                var properties = klass.GetProperties(flags);

                // 派生クラス側からさかのぼり、すでに登録されているプロパティは
                // 二重に登録しないようにしています。
                foreach (var property in properties)
                {
                    // indexerは無視します。
                    if (property.GetIndexParameters().Any())
                    {
                        continue;
                    }

                    // 派生クラスのプロパティの方が先にあります。
                    if (!result.ContainsKey(property.Name))
                    {
                        var obj = CreatePropertyObject(klass, property);

                        if (obj != null)
                        {
                            result.Add(property.Name, obj);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// キャッシュからある型のプロパティセットを探します。
        /// </summary>
        public static Dictionary<string, IPropertyObject> GetPropertyDic(
            Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            lock (TypePropertySet)
            {
                Dictionary<string, IPropertyObject> propertyDic;

                // 指定の型のプロパティリストを探し、もしなければ
                // それを新たに作成しハッシュに追加します。
                if (!TypePropertySet.TryGetValue(targetType, out propertyDic))
                {
                    propertyDic = GetPropertyDicImpl(targetType);

                    TypePropertySet.Add(targetType, propertyDic);
                }

                return propertyDic;
            }
        }

        /// <summary>
        /// キャッシュからPropertyInfoを探します。
        /// </summary>
        public static IPropertyObject GetPropertyObject(Type targetType,
                                                        string propertyName)
        {
            var propertyDic = GetPropertyDic(targetType);
            IPropertyObject propertyObj;

            // 指定の名前を持つプロパティを探します。
            if (!propertyDic.TryGetValue(propertyName, out propertyObj))
            {
                return null;
            }

            return propertyObj;
        }

        /// <summary>
        /// キャッシュからPropertyInfoを探します。
        /// </summary>
        public static PropertyInfo GetPropertyInfo(Type targetType,
                                                   string propertyName)
        {
            var propertyObj = GetPropertyObject(targetType, propertyName);
            if (propertyObj == null)
            {
                return null;
            }

            return propertyObj.PropertyInfo;
        }

        /// <summary>
        /// プロパティ値を読み出します。
        /// </summary>
        /// <remarks>
        /// PropertyInfoをキャッシュして保存しているので、
        /// 通常よりも速い（はず）です。
        /// </remarks>
        public static T GetPropertyValue<T>(object target,
                                            string propertyName)
        {
            return (T)GetPropertyValue(target, propertyName);
        }

        /// <summary>
        /// プロパティ値を読み出します。
        /// </summary>
        /// <remarks>
        /// PropertyInfoをキャッシュして保存しているので、
        /// 通常よりも速い（はず）です。
        /// </remarks>
        public static object GetPropertyValue(object target,
                                              string propertyName)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            // propertyObjを探します。
            var propertyObj = GetPropertyObject(
                target.GetType(), propertyName);
            if (propertyObj == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "'{0}': 指定のプロパティ名は存在しません。",
                        propertyName));
            }

            return propertyObj.GetValue(target);
        }

        /// <summary>
        /// プロパティ値を書き出します。
        /// </summary>
        /// <remarks>
        /// PropertyInfoをキャッシュして保存しているので、
        /// 通常よりも速い（はず）です。
        /// </remarks>
        public static void SetPropertyValue<T>(object target,
                                               string propertyName,
                                               T value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            var propertyObj = GetPropertyObject(
                target.GetType(), propertyName);
            if (propertyObj == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "'{0}': 指定のプロパティ名は存在しません。",
                        propertyName));
            }

            propertyObj.SetValue(target, value);
        }
    }
}
