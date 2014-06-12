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
        private string[] delimiters = { "," };
        private readonly string text;
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
        /// 区切り文字を設定します。
        /// </summary>
        public void SetDelimiters(params string[] delimiters)
        {
            if (delimiters == null)
            {
                return;
            }

            this.delimiters = delimiters;
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
        /// 最後にデリミタを付加した、正規表現おぶじぇくとを作成します。
        /// </summary>
        private Regex CreateRegexWithDelimiters(string pattern)
        {
            var escapedDelimiters =
                string.Join(
                    "|",
                    this.delimiters.Select(_ => Regex.Escape(_))
                    .ToArray());

            var newPattern = string.Format(
                @"{0}\s*(({1})\s*|$)",
                pattern,
                escapedDelimiters);

            return new Regex(newPattern);
        }

        /// <summary>
        /// 整数をパースします。
        /// </summary>
        public int ParseInt()
        {
            CheckEof();

            var re = CreateRegexWithDelimiters(
                @"\G\s*((\+|\-)?[0-9]+)");
            var m = re.Match(this.text, this.index);
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

            var re = CreateRegexWithDelimiters(
                @"\G\s*((\+|\-)?[0-9]+([.][0-9.]+)?)");
            var m = re.Match(this.text, this.index);
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

            var re = new Regex(
                @"\G\s*(\w+)(\s+|$)");
            var m = re.Match(this.text, this.index);

            if (!m.Success)
            {
                throw new ParseException(
                    "文字列の解析に失敗しました。");
            }

            this.index += m.Length;
            return m.Groups[1].Value;
        }

        /// <summary>
        /// 文字列を解析します。
        /// </summary>
        /// <remarks>
        /// 先頭に"がある場合は、次の"までをまとめて取得します。
        /// </remarks>
        public string ParseText()
        {
            CheckEof();

            string result;

            var re = CreateRegexWithDelimiters(
                @"\G\s*""((\""|[^""])*?)""");
            var m = re.Match(this.text, this.index);
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
                re = CreateRegexWithDelimiters(
                    @"\G\s*(.+?)");
                m = re.Match(this.text, this.index);

                if (!m.Success)
                {
                    throw new ParseException(
                        "文字列の解析に失敗しました。");
                }

                result = m.Groups[1].Value;
            }

            this.index += m.Length;
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

            this.text = text;
            this.index = 0;
        }
    }
}
