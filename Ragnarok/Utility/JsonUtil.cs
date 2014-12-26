#if CLR_GE_3_5
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace Ragnarok.Utility
{
    /// <summary>
    /// JSON形式のデータを扱います。
    /// </summary>
    public static class JsonUtil
    {
        public static T LoadJson<T>(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("filepath");
            }

            var content = File.ReadAllText(filepath);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var serial = new JavaScriptSerializer();
                serial.RegisterConverters(new[] { new JsonConverter<T>() });
                return (T)serial.Deserialize<T>(content);
            }
        }

        /// <summary>
        /// オブジェクトをJSON形式にシリアライズします。
        /// </summary>
        public static string Serialize<T>(T value)
        {
            try
            {
                var s = new DataContractJsonSerializer(typeof(T));

                using (var stream = new MemoryStream())
                {
                    s.WriteObject(stream, value);

                    stream.Flush();
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "Jsonへのシリアライズに失敗しました。");

                return null;
            }
        }

        /// <summary>
        /// オブジェクトをJSON形式の文字列からデシリアライズします。
        /// </summary>
        public static T Deserialize<T>(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            try
            {
#if false
                var bytes = Encoding.UTF8.GetBytes(text);
                using (var stream = new MemoryStream(bytes))
                {
                    var s = new DataContractJsonSerializer(typeof(T));
                    var obj = s.ReadObject(stream);
                    return (T)obj;
                }
#else
                var s = new JavaScriptSerializer();
                s.RegisterConverters(new[] { new JsonConverter<T>() });
                return (T)s.Deserialize<T>(text);
#endif
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "Jsonへのデシリアライズに失敗しました。");

                return default(T);
            }
        }

        /// <summary>
        /// オブジェクトをファイルにシリアライズします。
        /// </summary>
        public static void SerializeToFile<T>(string filepath, T value)
        {
            try
            {
                var s = new DataContractJsonSerializer(typeof(T));

                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    s.WriteObject(stream, value);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "Jsonへのシリアライズに失敗しました。");
            }
        }

        /// <summary>
        /// オブジェクトをファイルからデシリアライズします。
        /// </summary>
        public static T DeserializeFromFile<T>(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("text");
            }

            try
            {
                using (var stream = new FileStream(filepath, FileMode.Open))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var text = reader.ReadToEnd();

                    return Deserialize<T>(text);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "Jsonからのデシリアライズに失敗しました。");

                return default(T);
            }
        }
    }
}
#endif
