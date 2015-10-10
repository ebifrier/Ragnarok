using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using ProtoBuf;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// シリアライズ用のメソッド型です。
    /// </summary>
    public delegate void SerializeFunc(Stream stream, object value);

    /// <summary>
    /// デシリアライズ用のメソッド型です。
    /// </summary>
    public delegate object DeserializeFunc(Stream stream);

    /// <summary>
    /// ProtoBuf関連の共通コードを持ちます。
    /// </summary>
    public static class PbUtil
    {
        private static Dictionary<Type, SerializeFunc> serializeFuncDic =
            new Dictionary<Type, SerializeFunc>();
        private static Dictionary<Type, DeserializeFunc> deserializeFuncDic =
            new Dictionary<Type, DeserializeFunc>();

        /// <summary>
        /// シリアライズ用メソッドを取得します。
        /// </summary>
        /// <remarks>
        /// ジェネリックパラメーターのオーバーロードがあるため、
        /// このような実装になっています。
        /// </remarks>
        private static MethodInfo GetGenericSerializeMethod()
        {
            // 第一引数がStream, 第二引数がGenericであるような
            // メソッドを検索します。
            var methods =
                from method in typeof(Serializer).GetMethods()
                where method.Name == "Serialize"
                let paramInfo = method.GetParameters()
                where paramInfo.Length == 2
                where paramInfo[0].ParameterType == typeof(Stream)
                where paramInfo[1].ParameterType.IsGenericParameter
                select method;

            return methods.Single();
        }

        /// <summary>
        /// Serializer.Serializeを行う型付けされたメソッドを作成します。
        /// </summary>
        private static SerializeFunc CreateSerializeMethod(Type type)
        {
            var genericMethod = GetGenericSerializeMethod();
            var method = genericMethod.MakeGenericMethod(type);

            // 作成するラムダ関数
            // (stream, value) => Serializer.Serialize<T>(stream, (T)value)

            // "stream", "value"という引数を定義。
            var stream = Expression.Parameter(typeof(Stream), "stream");
            var value = Expression.Parameter(typeof(object), "value");
            var parameters = new[] { stream, value };

            // "method"メソッドを呼び出します。
            var body = Expression.Call(
                method,
                new Expression[]
                {
                    stream,
                    Expression.Convert(value, type),
                });

            // bodyをラムダとして定義。
            var e = Expression.Lambda(typeof(SerializeFunc), body, parameters);
            return (SerializeFunc)e.Compile();
        }

        /// <summary>
        /// Serializer.Serializeを行う型付けされたメソッドを取得します。
        /// </summary>
        /// <remarks>
        /// 必要なら作成します。
        /// </remarks>
        public static SerializeFunc GetSerializeMethod(Type type)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException("type");
            }

            lock (serializeFuncDic)
            {
                SerializeFunc func;
                if (serializeFuncDic.TryGetValue(type, out func))
                {
                    return func;
                }

                func = CreateSerializeMethod(type);
                serializeFuncDic.Add(type, func);
                return func;
            }
        }

        /// <summary>
        /// デシリアライズ用のメソッドを取得します。
        /// </summary>
        private static MethodInfo GetGenericDeserializeMethod()
        {
            var method = typeof(Serializer).GetMethod("Deserialize");
            if (method != null)
            {
                return method;
            }

            throw new InvalidOperationException(
                "適切なSerializer.Deserializeメソッドが見つかりません。");
        }

        /// <summary>
        /// Serializer.Deserializeを行う型付けされたメソッドを作成します。
        /// </summary>
        private static DeserializeFunc CreateDeserializeMethod(Type type)
        {
            var genericMethod = GetGenericDeserializeMethod();
            var method = genericMethod.MakeGenericMethod(type);

            // 作成するラムダ関数
            // stream => Serializer.Deserialize<T>(stream)

            // "stream"という引数を定義。
            var stream = Expression.Parameter(typeof(Stream), "stream");

            // "method"メソッドを呼び出します。
            var body = Expression.Call(method, stream);

            // bodyをラムダとして定義。
            var e = Expression.Lambda(
                typeof(DeserializeFunc), body, stream);

            return (DeserializeFunc)e.Compile();
        }

        /// <summary>
        /// Serializer.Deserializeを行う型付けされたメソッドを取得します。
        /// </summary>
        /// <remarks>
        /// 必要なら作成します。
        /// </remarks>
        public static DeserializeFunc GetDeserializeMethod(Type type)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException("type");
            }

            lock (deserializeFuncDic)
            {
                DeserializeFunc func;
                if (deserializeFuncDic.TryGetValue(type, out func))
                {
                    return func;
                }

                func = CreateDeserializeMethod(type);
                deserializeFuncDic.Add(type, func);
                return func;
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータに変換します。
        /// </summary>
        public static byte[] Serialize<T>(T value)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize<T>(stream, value);

                stream.Flush();
                return stream.ToArray();
            }
        }

        /// <summary>
        /// バイナリデータからオブジェクトを作成します。
        /// </summary>
        public static T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return (T)Serializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータに変換します。
        /// </summary>
        public static void Serialize(Stream stream, object value, Type type)
        {
            var method = GetSerializeMethod(type);
            
            // オブジェクトをシリアライズします。
            method(stream, value);
            stream.Flush();
        }

        /// <summary>
        /// オブジェクトをバイナリデータに変換します。
        /// </summary>
        public static byte[] Serialize(object value, Type type)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(stream, value, type);

                return stream.ToArray();
            }
        }

        /// <summary>
        /// バイナリデータからオブジェクトを作成します。
        /// </summary>
        public static object Deserialize(Stream stream, Type type)
        {
            var method = GetDeserializeMethod(type);

            return method(stream);
        }

        /// <summary>
        /// バイナリデータからオブジェクトを作成します。
        /// </summary>
        public static object Deserialize(byte[] data, Type type)
        {
            using (var stream = new MemoryStream(data))
            {
                return Deserialize(stream, type);
            }
        }
    }
}
