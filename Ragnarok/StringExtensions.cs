using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ragnarok.MathEx;

namespace Ragnarok
{
    /// <summary>
    /// ユーティリティクラスです。
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// 文字列長の範囲内で安全に部分文字列を取り出します。
        /// </summary>
        public static string SafeSubstr(this string value, Range range)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            ArgumentNullException.ThrowIfNull(range, nameof(range));

            int start = range.Start.GetOffset(value.Length);
            int end = range.End.GetOffset(value.Length);
            int length = end - start;
            if (length <= 0)
            {
                return string.Empty;
            }

            length = MathUtil.Between(0, value.Length - start, length);
            return value.Substring(start, length);
        }

        /// <summary>
        /// 全角文字を２文字分として文字数をカウントします。
        /// </summary>
        public static int HankakuLength(this string self)
        {
            if (string.IsNullOrEmpty(self))
            {
                return 0;
            }

            try
            {
                // sjisの2バイト文字はすべて全角なのでそれを利用して調べます。
                return Util.SJisEncoding.GetByteCount(self);
            }
            catch (EncoderFallbackException)
            {
                return -1;
            }
        }

        /// <summary>
        /// 全角文字を２文字分として、必要な文字数以下になるように文字列を切り詰めます。
        /// </summary>
        /// <example>
        /// てすと => (3) => て
        /// てすと => (4) => てす
        /// てtt => (3) => てt
        /// oかeri => (3) => oか
        /// </example>
        public static string HankakuSubstring(this string self, int hankakuLength)
        {
            if (string.IsNullOrEmpty(self))
            {
                return self;
            }

            for (var length = Math.Min(hankakuLength, self.Length);
                 length > 0; --length)
            {
                var substr = self[..length];
                var hanLen = substr.HankakuLength();

                // utf8 => sjisの変換に失敗した場合はすべて全角文字であると仮定します。
                hanLen = (hanLen >= 0 ? hanLen : substr.Length * 2);
                if (hanLen <= hankakuLength)
                {
                    return substr;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 空白文字か調べます。
        /// </summary>
        public static bool IsWhiteSpaceEx(this char self)
        {
            return (
                char.IsWhiteSpace(self) ||
                self == '\u200c' ||
                self == '\u200e');
        }

        /// <summary>
        /// 空白文字のみで構成されているか調べます。
        /// </summary>
        public static bool IsWhiteSpaceOnly(this string self)
        {
            if (string.IsNullOrEmpty(self))
            {
                return true;
            }

            return self.All(IsWhiteSpaceEx);
        }

        [GeneratedRegex(@"\s+")]
        private static partial Regex WhitespaceRegex();

        /// <summary>
        /// 空白文字をすべて削除します。
        /// </summary>
        public static string RemoveWhitespace(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return WhitespaceRegex().Replace(text, "");
        }

        /// <summary>
        /// 特定の文字で前後をくくります。
        /// </summary>
        public static string Quote(this string text, string c)
        {
            if (string.IsNullOrEmpty(c))
            {
                return text;
            }

            return (c + text + c);
        }

        /// <summary>
        /// 前後にある特定の文字列を削除します。
        /// </summary>
        public static string RemoveQuote(this string text, params char[] quotes)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (quotes.Length == 0)
            {
                quotes = ['\'', '\"'];
            }

            return text.TrimStart(quotes).TrimEnd(quotes);
        }
    }
}
