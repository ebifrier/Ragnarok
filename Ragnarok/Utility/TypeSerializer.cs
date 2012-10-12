using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 型名に含まれる字句の種別です。
    /// </summary>
    internal enum LexicalToken
    {
        /// <summary>
        /// 不明な文字です。
        /// </summary>
        Unknown,
        /// <summary>
        /// 開き中括弧("[")です。
        /// </summary>
        OpenBlanket,
        /// <summary>
        /// 閉じ中括弧("]")です。
        /// </summary>
        CloseBlanket,
        /// <summary>
        /// カンマ(",")です。
        /// </summary>
        Comma,
        /// <summary>
        /// ジェネリックでない型名です。
        /// </summary>
        TypeName,
        /// <summary>
        /// 引数なしのジェネリックな型名("Xxx`n")です。
        /// </summary>
        GenericTypeName,
        /// <summary>
        /// 文字列の終端です。
        /// </summary>
        End,
    }

    /// <summary>
    /// 型名の解析時に使う字句解析機です。
    /// </summary>
    internal class TypeLexer
    {
        /// <summary>
        /// 型名か判断します。
        /// </summary>
        /// <example>
        /// global::TopNamespace.SubNameSpace.ContainingClass+NestedClass
        /// </example>
        private static readonly Regex TypeRegex =
            new Regex(@"\G([\w+.:]+)(`\d+)?", RegexOptions.Compiled);

        /// <summary>
        /// 型名に続くアセンブリ名などを解析します。
        /// </summary>
        /// <example>
        /// [TopNamespace.SubNameSpace.ContainingClass+NestedClass, MyAssembly,
        ///     Version=1.3.0.0, Culture=neutral, PublicKeyToken=b17a5c561934e089]
        /// </example>
        //private static Regex qualifiedRegex =
        //    new Regex(@"\G[\w\s=,.]+", RegexOptions.Compiled);

        private readonly string text;
        private int index = 0;
        private LexicalToken token = LexicalToken.Unknown;
        private string name;

        /// <summary>
        /// 現在のトークンを取得します。
        /// </summary>
        public LexicalToken Token
        {
            get { return this.token; }
            set { this.token = value; }
        }

        /// <summary>
        /// 前回解析した型名を取得します。
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 空白部分をスキップします。
        /// </summary>
        private void SkipWhitespace()
        {
            while (this.index < this.text.Length &&
                   char.IsWhiteSpace(this.text[this.index]))
            {
                this.index += 1;
            }
        }

        /// <summary>
        /// 解析を進め、次のトークンを取得します。
        /// </summary>
        public LexicalToken NextToken()
        {
            Token = ParseToken();
            return Token;
        }

        /// <summary>
        /// 次のトークンを取得します。
        /// </summary>
        private LexicalToken ParseToken()
        {
            SkipWhitespace();
            
            // 文字列の終端に到達しました。
            if (this.index >= this.text.Length)
            {
                return LexicalToken.End;
            }
            
            if (this.text[this.index] == '[')
            {
                this.index += 1;
                return LexicalToken.OpenBlanket;
            }
            else if (this.text[this.index] == ']')
            {
                this.index += 1;
                return LexicalToken.CloseBlanket;
            }
            else if (this.text[this.index] == ',')
            {
                this.index += 1;
                return LexicalToken.Comma;
            }
            else
            {
                // 型に括弧が付いているときは、カンマありの解析を行います。
                var m = TypeRegex.Match(this.text, this.index);
                if (!m.Success)
                {
                    return LexicalToken.Unknown;
                }

                // 名前部分を取得。
                this.name = m.Groups[0].Value;
                this.index += m.Length;

                // "`n"の部分にマッチした場合はジェネリック型
                if (m.Groups[2].Success)
                {
                    return LexicalToken.GenericTypeName;
                }
                else
                {
                    // 非ジェネリック型
                    return LexicalToken.TypeName;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TypeLexer(string text)
        {
            this.text = text;
            this.index = 0;
        }
    }

    /// <summary>
    /// <see cref="Type"/>オブジェクトと文字列の相互変換を行うクラスです。
    /// </summary>
    /// <remarks>
    /// ジェネリッククラスなどをアセンブリバージョンなどを含めずに、
    /// ネームスペースからのフルネームで文字列化するために使います。
    /// </remarks>
    public static class TypeSerializer
    {
        /// <summary>
        /// <see cref="Type"/>オブジェクトを文字列に変換します。
        /// </summary>
        /// <remarks>
        /// 基本的にはアセンブリ名を排したFullNameを返します。
        /// しかし、ジェネリック型ではFullNameでもパラメータ型の中にアセンブリ名が
        /// 入ってしまうため、このクラスではこれを削除しつつ型名を再構築します。
        /// 
        /// ジェネリックパラメータは'`n[ TypeName ]'のように`n[]記号でくくられ、
        /// さらに型名は'[]'でくくられます。
        /// ネストしている場合はさらに内部でくくられます。
        /// </remarks>
        public static string Serialize(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            /*if (type.ContainsGenericParameters)
            {
                throw new InvalidOperationException(
                    "ジェネリックパラメータを含むクラスは扱えません。");
            }*/

            if (!type.IsGenericType)
            {
                return type.FullName;
            }
            else
            {
                var result = new StringBuilder();

                result.Append(type.GetGenericTypeDefinition().FullName);
                if (!type.ContainsGenericParameters)
                {
                    result.Append("[");

                    // ジェネリック型の引数をシリアライズし[]でくくります。
                    // 再帰するのがポイント。
                    var paramNameList = type.GetGenericArguments().Select(
                        param => string.Format("[{0}]", Serialize(param)));
                    result.Append(string.Join(", ", paramNameList.ToArray()));

                    result.Append("]");
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// シリアライズされた型名を<see cref="Type"/>オブジェクトに変換します。
        /// </summary>
        public static Type Deserialize(string serializedTypeName)
        {
            if (string.IsNullOrEmpty(serializedTypeName))
            {
                return null;
            }
            
            // レキサからトークンを取り出しながら構文解析を行います。
            var lexer = new TypeLexer(serializedTypeName);

            // 次のトークンを取得します。
            lexer.NextToken();
            return ParseType(lexer);
        }

        /// <summary>
        /// 通常orジェネリック型をパースします。
        /// </summary>
        private static Type ParseType(TypeLexer lexer)
        {
            var hasBlanket = false;
            
            if (lexer.Token == LexicalToken.OpenBlanket)
            {
                hasBlanket = true;
                lexer.NextToken();
            }

            if (lexer.Token != LexicalToken.GenericTypeName &&
                lexer.Token != LexicalToken.TypeName &&
                lexer.Token != LexicalToken.CloseBlanket)
            {
                throw new TypeLoadException(
                    "型名がありませんでした。");
            }

            // 型無しでフィニッシュです。
            if (lexer.Token == LexicalToken.CloseBlanket)
            {
                lexer.NextToken();
                return null;
            }

            // とりあえず型名から、その型を取り出します。
            var type = Util.FindTypeFromCurrentDomain(lexer.Name);
            if (type == null)
            {
                throw new TypeLoadException(
                    string.Format(
                        "型'{0}'が現在のドメインで見つかりませんでした。",
                        lexer.Name));
            }

            if (lexer.Token == LexicalToken.GenericTypeName)
            {
                lexer.NextToken();

                var args = ParseGenericArguments(lexer);
                if (args != null)
                {
                    // 引数部分を置き換えた型にします。
                    type = type.MakeGenericType(args);
                }
            }
            else
            {
                // LexicalToken.TypeName の場合
                lexer.NextToken();
            }

            // 最後の']'があるかもしれないので。
            if (hasBlanket)
            {
                if (lexer.Token != LexicalToken.CloseBlanket)
                {
                    throw new TypeLoadException(
                        "対応する']'がありません。");
                }

                lexer.NextToken();
            }

            return type;
        }

        /// <summary>
        /// ジェネリック型の引数部分を解析します。
        /// </summary>
        private static Type[] ParseGenericArguments(TypeLexer lexer)
        {
            // '['が無いということは、つまり引数が無いということです。
            if (lexer.Token != LexicalToken.OpenBlanket)
            {
                return null;
            }
            lexer.NextToken();

            // ']'があると言うことは引数無しでフィニッシュです。
            if (lexer.Token == LexicalToken.CloseBlanket)
            {
                lexer.NextToken();
                return null;
            }

            var result = new List<Type>();
            while (true)
            {
                // 次の型をパースします。
                var type = ParseType(lexer);
                if (type == null)
                {
                    throw new TypeLoadException(
                        "ジェネリック型の引数が正しくありません。");
                }
                result.Add(type);

                if (lexer.Token == LexicalToken.CloseBlanket)
                {
                    lexer.NextToken();
                    return result.ToArray();
                }
                else if (lexer.Token == LexicalToken.Comma)
                {
                    // もう一回パース
                    lexer.NextToken();
                }
                else
                {
                    throw new TypeLoadException(
                        "ジェネリック型の引数が正しくありません。");
                }
            }
        }
    }
}
