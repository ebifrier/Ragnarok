using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

using ProtoBuf;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// ProtoBuf関連の共通コードを持ちます。
    /// </summary>
    public static class PbUtil
    {
        private static MethodInfo serializeMethod;
        private static MethodInfo deserializeMethod;

        /// <summary>
        /// シリアライズ用メソッドを取得します。
        /// </summary>
        /// <remarks>
        /// ジェネリックパラメーターのオーバーロードがあるため、
        /// このような実装になっています。
        /// </remarks>
        public static MethodInfo GetSerializeMethod()
        {
            if (serializeMethod != null)
            {
                return serializeMethod;
            }

            foreach (var method in typeof(Serializer).GetMethods())
            {
                if (method.Name == "Serialize")
                {
                    var paramInfo = method.GetParameters();

                    // 第一引数がStream, 第二引数がGenericであるような
                    // メソッドを検索します。
                    if (paramInfo.Length == 2 &&
                        paramInfo[0].ParameterType == typeof(Stream) &&
                        paramInfo[1].ParameterType.IsGenericParameter)
                    {
                        serializeMethod = method;
                        return method;
                    }
                }
            }

            throw new InvalidOperationException(
                "適切なSerializer.Serializeメソッドが見つかりません。");
        }

        /// <summary>
        /// デシリアライズ用のメソッドを取得します。
        /// </summary>
        public static MethodInfo GetDeserializeMethod()
        {
            if (deserializeMethod != null)
            {
                return deserializeMethod;
            }

            var method = typeof(Serializer).GetMethod("Deserialize");
            if (method != null)
            {
                deserializeMethod = method;
                return method;
            }

            throw new InvalidOperationException(
                "適切なSerializer.Deserializeメソッドが見つかりません。");
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
            var method = GetSerializeMethod();
            var methodImpl = method.MakeGenericMethod(type);

            // オブジェクトをシリアライズします。
            methodImpl.Invoke(
                null,
                new object[] {
                    stream, // 出力先ストリーム
                    value // シリアライズ対象のオブジェクト
                });

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
            var method = GetDeserializeMethod();
            var methodImpl = method.MakeGenericMethod(type);

            // オブジェクトをデシリアライズします。
            var objectValue = methodImpl.Invoke(
                null,
                new object[] {
                    stream // 入力元ストリーム
                });

            return objectValue;
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
