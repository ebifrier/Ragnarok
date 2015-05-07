using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Ragnarok.ObjectModel;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 文字列内への変数埋め込みを実現するためのクラスです。
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        /// '#{name}'で指定された変数名を与えられた辞書の値に置き換えます。
        /// </summary>
        public static string NamedFormat<T>(string format,
                                            IDictionary<string, T> args)
            where T : class
        {
            var replacedFormat = format;

            // string.Format に渡すフォーマット作成
            // ついでに string.Format に渡す配列も作成
            var values = args.Select((pair, index) =>
            {
                var regex = new Regex(
                    @"#[{]" + pair.Key + "([^}]*)[}]");
                //Console.WriteLine(regex);

                replacedFormat = regex.Replace(replacedFormat, "{" + index + "$1}");
                //Console.WriteLine(replacedFormat);

                // 配列として使う値一覧を取得します。
                return pair.Value;
            }).ToArray();

            // 整形は string.Format にまかせる
            return string.Format(replacedFormat, values);
        }

        /// <summary>
        /// 匿名クラスのインスタンスを渡します。
        /// </summary>
        public static string NamedFormat(string format, object obj)
        {
            if (string.IsNullOrEmpty(format) || obj == null)
            {
                return string.Format(format, obj);
            }

            // プロパティ名とプロパティ値をセットにした辞書を作り、
            // それを使って文字列埋め込みを行います。
            var dic = obj.GetType().GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(obj, null));
            return NamedFormat(format, dic);
        }

        /// <summary>
        /// 匿名クラスのインスタンスを渡します。
        /// </summary>
        /// <remarks>
        /// NotifyObjectはオブジェクト内にプロパティを辞書としてもっているため、
        /// このクラスでは直接その辞書を使って文字列のフォーマットを行います。
        /// </remarks>
        public static string NamedFormat(string format, NotifyObject obj)
        {
            if (string.IsNullOrEmpty(format) || obj == null)
            {
                return string.Format(format, obj);
            }

            // プロパティ名とプロパティ値をセットにした辞書を使い、
            // それを使って文字列埋め込みを行います。
            return NamedFormat(format, obj.GetPropertyData());
        }
    }
}
