using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        /// '${name}'や'$name'で指定された変数名を与えられた辞書の値に置き換えます。
        /// </summary>
        public static string NamedFormat<T>(string format,
                                            IDictionary<string, T> args)
            where T : class
        {
            var replacedFormat = format;

            // string.Format に渡すフォーマット作成
            // ついでに string.Format に渡す配列も作成
            var values = args
                .Where(_ => !string.IsNullOrEmpty(_.Key))
                .OrderByDescending(_ => _.Key.Length) // argsを変数名の文字数順に並びかえます。
                .Select((pair, index) =>
                {
                    if (pair.Key.All(_ => char.IsDigit(_)))
                    {
                        throw new FormatException("数字だけの置き換え文字列は許されません。");
                    }

                    // フォーマットを置き換えながら巡回します。
                    var regex = new Regex(@"\$[{]" + pair.Key + "([^}]*)[}]");
                    replacedFormat = regex.Replace(replacedFormat, "{" + index + "$1}");

                    regex = new Regex(@"\$" + pair.Key + "([^\\w]|$)");
                    replacedFormat = regex.Replace(replacedFormat, "{" + index + "}$1");

                    // 配列として使う値一覧を取得します。
                    return pair.Value;
                })
                .ToArray();

            // 整形は string.Format にまかせる
            return string.Format(CultureInfo.CurrentCulture, replacedFormat, values);
        }

        /// <summary>
        /// 匿名クラスのインスタンスを渡します。
        /// </summary>
        public static string NamedFormat(string format, object target)
        {
            if (string.IsNullOrEmpty(format) || target == null)
            {
                return string.Format(CultureInfo.CurrentCulture, format, target);
            }

            // プロパティ名とプロパティ値をセットにした辞書を作り、
            // それを使って文字列埋め込みを行います。
            var dic = target.GetType().GetProperties()
                .Where(_ => _.GetIndexParameters() == null || !_.GetIndexParameters().Any())
                .ToDictionary(p => p.Name, p => p.GetValue(target, null));
            return NamedFormat(format, dic);
        }

        /// <summary>
        /// 匿名クラスのインスタンスを渡します。
        /// </summary>
        /// <remarks>
        /// NotifyObjectはオブジェクト内にプロパティを辞書としてもっているため、
        /// このクラスでは直接その辞書を使って文字列のフォーマットを行います。
        /// </remarks>
        public static string NamedFormat(string format, NotifyObject target)
        {
            if (string.IsNullOrEmpty(format) || target == null)
            {
                return string.Format(CultureInfo.CurrentCulture, format, target);
            }

            // プロパティ名とプロパティ値をセットにした辞書を使い、
            // それを使って文字列埋め込みを行います。
            return NamedFormat(format, target.GetPropertyData());
        }
    }
}
