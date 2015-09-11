using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Ragnarok.Utility
{
    /// <summary>
    /// System.Web.Script.Serialization.JavaScriptSerializerによる
    /// jsonファイルの読み書きをサポートするクラスです。
    /// </summary>
    /// <remarks>
    /// Runtime Version 4.0でDictionaryへのデシリアライズを行うために
    /// 作成しました。
    /// </remarks>
    public class JsonConverter<T> : JavaScriptConverter
    {
        private Type[] supportedTypes;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JsonConverter()
        {
            this.supportedTypes =
                MakeSupportedTypes(typeof(T), new HashSet<Type>())
                .ToArray();
        }

        /// <summary>
        /// サポートされる型のリストを作成します。
        /// </summary>
        private IEnumerable<Type> MakeSupportedTypes(Type type, HashSet<Type> typeSet)
        {
            if (typeSet.Contains(type))
            {
                return new Type[0];
            }
            typeSet.Add(type);

            // DataContract属性がある型のみを対象にします。
            var attr = Attribute.GetCustomAttribute(
                type, typeof(DataContractAttribute))
                as DataContractAttribute;
            if (attr == null)
            {
                return new Type[0];
            }

            // プロパティやフィールドの型を取得します。
            var flags = BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static;
            var properties = type.GetProperties(flags)
                .Where(_ => _.CanRead)
                .Where(_ => _.CanWrite)
                .Where(_ => !_.GetIndexParameters().Any());
            var propertyTypes = new List<Type>();
            propertyTypes.AddRange(type.GetFields(flags).Select(_ => _.FieldType));
            propertyTypes.AddRange(properties.Select(_ => _.PropertyType));

            return propertyTypes
                .SelectMany(_ => FilterType(_))
                .SelectMany(_ => MakeSupportedTypes(_, typeSet))
                .Concat(new[] { type });
        }

        /// <summary>
        /// Generic型などから必要な型のリストを取得します。
        /// </summary>
        private Type[] FilterType(Type type)
        {
            if (type.HasElementType)
            {
                return FilterType(type.GetElementType());
            }
            else if (type.IsGenericType)
            {
                return type.GetGenericArguments()
                    .SelectMany(_ => FilterType(_)).ToArray();
            }
            else
            {
                return new[] { type };
            }
        }

        /// <summary>
        /// サポートされる型のリストを取得します。
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return this.supportedTypes; }
        }

        /// <summary>
        /// シリアライズ／デシリアライズの対象となるメンバーリストを取得します。
        /// </summary>
        private List<MemberInfo> GetMemberList(Type type)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static;
            var properties = type.GetProperties(flags)
                .Where(_ => _.CanRead)
                .Where(_ => _.CanWrite)
                .Where(_ => !_.GetIndexParameters().Any());

            var members = new List<MemberInfo>();
            members.AddRange(type.GetFields(flags));
            members.AddRange(properties);
            return members;
        }

        /// <summary>
        /// オブジェクトのデシリアライズを行います。
        /// </summary>
        public override object Deserialize(IDictionary<string, object> dictionary,
                                           Type type,
                                           JavaScriptSerializer serializer)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var obj = Activator.CreateInstance(type);
            var members = GetMemberList(type);
            foreach (var member in members)
            {
                var propertyAttr = Attribute.GetCustomAttribute(
                    member, typeof(DataMemberAttribute))
                    as DataMemberAttribute;

                if (propertyAttr != null && !string.IsNullOrEmpty(propertyAttr.Name) &&
                    dictionary.ContainsKey(propertyAttr.Name))
                {
                    SetMemberValue(serializer, member, obj, dictionary[propertyAttr.Name]);
                }
                else if (dictionary.ContainsKey(member.Name))
                {
                    SetMemberValue(serializer, member, obj, dictionary[member.Name]);
                }
                else
                {
                    var kvp = dictionary.FirstOrDefault(x =>
                        string.Equals(x.Key, member.Name,
                                      StringComparison.InvariantCultureIgnoreCase));

                    if (!kvp.Equals(default(KeyValuePair<string, object>)))
                    {
                        SetMemberValue(serializer, member, obj, kvp.Value);
                    }
                }
            }

            return obj;
        }

        private void SetMemberValue(JavaScriptSerializer serializer,
                                    MemberInfo member,
                                    object obj, object value)
        {
            if (member is PropertyInfo)
            {
                var property = (PropertyInfo)member;
                var converted = serializer.ConvertToType(value, property.PropertyType);
                property.SetValue(obj, converted, null);
            }
            else if (member is FieldInfo)
            {
                var field = (FieldInfo)member;
                var converted = serializer.ConvertToType(value, field.FieldType);
                field.SetValue(obj, converted);
            }
        }

        /// <summary>
        /// オブジェクトのシリアライズを行います。
        /// </summary>
        public override IDictionary<string, object> Serialize(object obj,
                                                              JavaScriptSerializer serializer)
        {
            var values = new Dictionary<string, object>();
            var type = obj.GetType();
            var members = GetMemberList(type);

            foreach (var member in members)
            {
                var propertyAttr = Attribute.GetCustomAttribute(
                    member, typeof(DataMemberAttribute))
                    as DataMemberAttribute;

                if (propertyAttr != null && !string.IsNullOrEmpty(propertyAttr.Name))
                {
                    values[propertyAttr.Name] = GetMemberValue(member, obj);
                }
                else
                {
                    values[member.Name] = GetMemberValue(member, obj);
                }
            }

            return values;
        }

        /// <summary>
        /// オブジェクトから値の取得を行います。
        /// </summary>
        private object GetMemberValue(MemberInfo member, object obj)
        {
            if (member is PropertyInfo)
            {
                var property = (PropertyInfo)member;
                return property.GetValue(obj, null);
            }
            else if (member is FieldInfo)
            {
                var field = (FieldInfo)member;
                return field.GetValue(obj);
            }

            return null;
        }
    }
}
