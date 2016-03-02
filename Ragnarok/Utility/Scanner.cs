using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 解析失敗時に投げられる例外です。
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ParseException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// ""で囲まれた文字列や整数などをパースします。
    /// </summary>
    public class Scanner
    {
        private static readonly string WordRegexPattern = @"\G\s*(\w+)(\s+|$)";
        private static readonly string QuotedTextRegexPattern = @"\G\s*""((\""|[^""])*?)""";
        private static readonly string TextRegexPattern = @"\G\s*(.+?)";
        private static readonly string IntRegexPattern = @"\G\s*((\+|\-)?[0-9]+)";
        private static readonly string DoubleRegexPattern = @"\G\s*((\+|\-)?[0-9]+([.][0-9.]+)?)";

        private static readonly Regex WordRegex = new Regex(
            WordRegexPattern, RegexOptions.Compiled);
        private static readonly Regex CommaQuotedTextRegex = CreateRegexWithDelimiters(
            QuotedTextRegexPattern, RegexOptions.Compiled, ",");
        private static readonly Regex CommaTextRegex = CreateRegexWithDelimiters(
            TextRegexPattern, RegexOptions.Compiled, ",");
        private static readonly Regex CommaIntRegex = CreateRegexWithDelimiters(
            IntRegexPattern, RegexOptions.Compiled, ",");
        private static readonly Regex CommaDoubleRegex = CreateRegexWithDelimiters(
            DoubleRegexPattern, RegexOptions.Compiled, ",");

        private Regex quotedTextRegex;
        private Regex textRegex;        
        private Regex intRegex;
        private Regex doubleRegex;
        private string[] delimiters = { "," };
        private readonly string text;
        private string peek;
        private int index;
        
        /// <summary>
        /// 最初の文字列を取得します。
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        /// <summary>
        /// 未パースの文字列を取得します。
        /// </summary>
        public string LastText
        {
            get { return this.text.Substring(this.index); }
        }

        /// <summary>
        /// 解析文字列が終了しているか取得します。
        /// </summary>
        public bool IsEof
        {
            get { return (this.index >= this.text.Length); }
        }

        /// <summary>
        /// 最後にデリミタを付加した、正規表現おぶじぇくとを作成します。
        /// </summary>
        private static Regex CreateRegexWithDelimiters(string pattern,
                                                       RegexOptions options,
                                                       params string[] delimiters)
        {
            var escapedDelimiters =
                string.Join(
                    "|",
                    delimiters.Select(_ => Regex.Escape(_))
                    .ToArray());

            var newPattern = string.Format(
                @"{0}\s*(({1})\s*|$)",
                pattern,
                escapedDelimiters);

            return new Regex(newPattern, options);
        }

        /// <summary>
        /// 区切り文字を設定します。
        /// </summary>
        public void SetDelimiters(params string[] delimiters)
        {
            if (delimiters == null)
            {
                return;
            }

            this.delimiters = delimiters;

            if (delimiters.Count() == 1 && delimiters[0] == ",")
            {
                this.quotedTextRegex = CommaQuotedTextRegex;
                this.textRegex = CommaTextRegex;
                this.intRegex = CommaIntRegex;
                this.doubleRegex = CommaDoubleRegex;
            }
            else
            {
                this.quotedTextRegex = CreateRegexWithDelimiters(
                    QuotedTextRegexPattern, RegexOptions.None, delimiters);
                this.textRegex = CreateRegexWithDelimiters(
                    TextRegexPattern, RegexOptions.None, delimiters);
                this.intRegex = CreateRegexWithDelimiters(
                    IntRegexPattern, RegexOptions.None, delimiters);
                this.doubleRegex = CreateRegexWithDelimiters(
                    DoubleRegexPattern, RegexOptions.None, delimiters);
            }
        }

        /// <summary>
        /// 解析文字列が終了していれば例外を投げます。
        /// </summary>
        private void CheckEof()
        {
            if (IsEof)
            {
                throw new InvalidOperationException(
                    "文字列は既に終了しています。");
            }
        }

        /// <summary>
        /// 整数をパースします。
        /// </summary>
        public int ParseInt()
        {
            CheckEof();

            var m = this.intRegex.Match(this.text, this.index);
            if (!m.Success)
            {
                throw new ParseException(
                    "整数値の解析に失敗しました。");
            }

            var result = int.Parse(m.Groups[1].Value);
            this.index += m.Length;
            return result;
        }

        /// <summary>
        /// 小数をパースします。
        /// </summary>
        public double ParseDouble()
        {
            CheckEof();

            var m = this.doubleRegex.Match(this.text, this.index);
            if (!m.Success)
            {
                throw new ParseException(
                    "小数値の解析に失敗しました。");
            }

            var result = double.Parse(m.Groups[1].Value);
            this.index += m.Length;
            return result;
        }

        /// <summary>
        /// 次の単語を取得します。
        /// </summary>
        public string ParseWord()
        {
            CheckEof();

            var m = WordRegex.Match(this.text, this.index);
            if (!m.Success)
            {
                throw new ParseException(
                    "文字列の解析に失敗しました。");
            }

            this.index += m.Length;
            return m.Groups[1].Value;
        }

        /// <summary>
        /// 次の文字列を先読みし、解析したものを返します。
        /// </summary>
        /// <remarks>
        /// ここで先読みされた文字列が、次のParseTextでも使われます。
        /// 
        /// 先頭に"がある場合は、次の"までをまとめて取得します。
        /// </remarks>
        public string PeekText()
        {
            if (IsEof)
            {
                return null;
            }

            string result;

            var m = this.quotedTextRegex.Match(this.text, this.index);
            if (m.Success)
            {
                // ""で囲まれた文字列の場合は、
                // 文字のエスケープを行います。
                result = m.Groups[1].Value.Replace(@"\n", "\n");
                result = result.Replace(@"\t", "\t");
                result = result.Replace(@"\\", "\\");
            }
            else
            {
                m = this.textRegex.Match(this.text, this.index);
                if (!m.Success)
                {
                    throw new ParseException(
                        "文字列の解析に失敗しました。");
                }

                result = m.Groups[1].Value;
            }

            this.index += m.Length;
            this.peek = result;
            return result;
        }

        /// <summary>
        /// 文字列を読み込み、その読み込んだ文字列を取得します。
        /// </summary>
        /// <remarks>
        /// 前にPeekTextした場合は、そこで先読みした文字列を取得します。
        /// そうでない場合は、新規に文字列の解析を行います。
        /// 
        /// 先頭に"がある場合は、次の"までをまとめて取得します。
        /// </remarks>
        public string ParseText()
        {
            var result = (this.peek != null ? this.peek : PeekText());

            this.peek = null;
            return result;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Scanner(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            SetDelimiters(",");
            this.text = text;
            this.index = 0;
        }
    }
}
