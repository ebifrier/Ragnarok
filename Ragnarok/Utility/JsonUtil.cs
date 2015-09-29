#if CLR_GE_3_5
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;

using Newtonsoft.Json;

namespace Ragnarok.Utility
{
    /// <summary>
    /// JSON形式のデータを扱います。
    /// </summary>
    public static class JsonUtil
    {
        /// <summary>
        /// オブジェクトをJSON形式にシリアライズします。
        /// </summary>
        public static string Serialize<T>(T value)
        {
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "'{0}'({1}): Jsonへのシリアライズに失敗しました。",
                    value, typeof(T));

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
                return JsonConvert.DeserializeObject<T>(text);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "'{0}': Jsonから'{1}'型へのデシリアライズに失敗しました。",
                    text, typeof(T));

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
                var text = Serialize(value);

                File.WriteAllText(filepath, text);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "'{0}'({1}): Jsonへのシリアライズに失敗しました。",
                    value, typeof(T));
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
                var text = File.ReadAllText(filepath, Encoding.UTF8);

                return Deserialize<T>(text);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "'{0}': Jsonから'{1}'型へのデシリアライズに失敗しました。",
                    filepath, typeof(T));

                return default(T);
            }
        }
    }
}
#endif
