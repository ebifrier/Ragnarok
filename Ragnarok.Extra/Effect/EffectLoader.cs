using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// エフェクトを管理します。
    /// </summary>
    public static class EffectLoader
    {
        /// <summary>
        /// エフェクトをファイルから読み込みます。
        /// </summary>
        /// <remarks>
        /// エフェクトファイルは以下のファイルから検索されます。
        /// 
        /// １、{ディレクトリ名}/{エフェクト名}/{エフェクト名}.xaml
        /// ２、{ディレクトリ名}/{エフェクト名}/Effect.xaml
        /// ３、{ディレクトリ名}/{エフェクト名}.xaml
        /// </remarks>
        private static EffectObject LoadInternal(string basePath,
                                                 Dictionary<string, object> args)
        {
            try
            {
                // エフェクトファイルの検索
                var path = EnumeratePath(basePath)
                    .FirstOrDefault(_ => File.Exists(_));
                if (string.IsNullOrEmpty(path))
                {
                    throw new FileNotFoundException(
                        basePath + ": エフェクトが見つかりません。");
                }

                // エフェクト引数の置換
                byte[] bytes = null;
                if (args == null || !args.Any())
                {
                    bytes = Util.ReadFile(path);
                }
                else
                {
                    // ファイル中の変数を置き換えます。
                    var text = Util.ReadFile(path, Encoding.UTF8);
                    text = ReplaceTable(text, args);

                    bytes = Encoding.UTF8.GetBytes(text);
                }

                // xamlの読み込みを開始します。
                using (var stream = new MemoryStream(bytes))
                {
                    var settings = new XamlXmlReaderSettings
                    {
                        BaseUri = new Uri(path, UriKind.Absolute),
                        CloseInput = false,
                    };
                    var reader = new XamlXmlReader(stream, settings);
                    var obj = XamlServices.Load(reader);

                    var result = obj as EffectObject; 
                    if (result != null)
                    {
                        //result.Name = param.Name;
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "'{0}': エフェクトの読み込みに失敗しました。", basePath);

                //return null;
                throw ex;
            }
        }

        /// <summary>
        /// エフェクトファイルの置いてあるパスを順次取得します。
        /// </summary>
        /// <remarks>
        /// １、{ディレクトリ名}/{エフェクト名}/{エフェクト名}.{拡張子}
        /// ２、{ディレクトリ名}/{エフェクト名}/Effect.{拡張子}
        /// ３、{ディレクトリ名}/{エフェクト名}.{拡張子}
        /// </remarks>
        private static IEnumerable<string> EnumeratePath(string basePath)
        {
            var dir = Path.GetDirectoryName(basePath);

            var name = Path.GetFileName(basePath);
            name = Path.ChangeExtension(name, null);

            var ext = Path.GetExtension(basePath);
            if (string.IsNullOrEmpty(ext))
            {
                ext = ".xaml";
            }

            yield return Path.Combine(dir, name, name + ext);
            yield return Path.Combine(dir, name, "Effect" + ext);
            yield return Path.Combine(dir, name + ext);
        }

        /// <summary>
        /// 置換テーブルを使って、テキストを置き換えます。
        /// </summary>
        private static string ReplaceTable(string text,
                                           Dictionary<string, object> table)
        {
            if (table == null)
            {
                return text;
            }

            foreach (var pair in table)
            {
                if (string.IsNullOrEmpty(pair.Key))
                {
                    continue;
                }

                var value = pair.Value ?? string.Empty;
                text = text.Replace("${" + pair.Key + "}", value.ToString());
            }

            return text;
        }

        /// <summary>
        /// EffectArgumentAttributeを持つプロパティなどから
        /// エフェクト引数を作成します。
        /// </summary>
        private static Dictionary<string, object> CreateArguments(object param)
        {
            // 対象はプロパティとフィールドの両方です。
            var flags = /*BindingFlags.GetField |*/ BindingFlags.GetProperty |
                        BindingFlags.Public | BindingFlags.Instance;

            if (param == null)
            {
                return new Dictionary<string, object>();
            }

            var argumentPairs =
                from property in param.GetType().GetProperties(flags)
                let attrs = property.GetCustomAttributes(
                    typeof(EffectArgumentAttribute), true)
                let attr = attrs.FirstOrDefault() as EffectArgumentAttribute
                where attr != null
                select new
                {
                    Name = property.Name,
                    Value = property.GetValue(param, null),
                };

            return argumentPairs.ToDictionary(_ => _.Name, _ => _.Value);
        }

        /// <summary>
        /// EffectArgumentAttributeを持つプロパティなどから
        /// エフェクト引数を作成します。
        /// </summary>
        private static Dictionary<string, object> CreateDefaultArguments(Type paramType)
        {
            // 対象はプロパティとフィールドの両方です。
            var flags = /*BindingFlags.GetField |*/ BindingFlags.GetProperty |
                        BindingFlags.Public | BindingFlags.Instance;

            if (paramType == null)
            {
                return new Dictionary<string, object>();
            }

            var argumentPairs =
                from property in paramType.GetProperties(flags)
                let attrs = property.GetCustomAttributes(
                    typeof(EffectArgumentAttribute), true)
                let attr = attrs.FirstOrDefault() as EffectArgumentAttribute
                where attr != null
                select new
                {
                    Name = property.Name,
                    Value = attr.DefaultValue,
                };

            return argumentPairs.ToDictionary(_ => _.Name, _ => _.Value);
        }

        /// <summary>
        /// エフェクトをファイルから読み込みます。
        /// </summary>
        public static EffectObject Load(string path, object param = null)
        {
            var args = CreateArguments(param);

            return LoadInternal(path, args);
        }

        /// <summary>
        /// 読み込み時間短縮のための事前読み込みを行います。
        /// </summary>
        public static void PreLoad(string path, Type paramType)
        {
            var args = CreateDefaultArguments(paramType);
            
            LoadInternal(path, args);
        }
    }
}
