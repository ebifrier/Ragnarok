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
    /// <example>
    /// System.Tuple`2[System.Int32, System.Double]
    /// </example>
    internal sealed class TypeLexer
    {
        /// <summary>
        /// 単一の型名か判断します。
        /// </summary>
        /// <example>
        /// global::TopNamespace.SubNameSpace.ContainingClass+NestedClass
        /// </example>
        private static readonly Regex TypeRegex = new Regex(
                @"\G\s*(?:([\w+.:]+(`\d+)?)|(\[)|(\])|(,))\s*",
                RegexOptions.Compiled);

        /// <summary>
        /// 型名が長いため作りました。
        /// </summary>
        private static KeyValuePair<LexicalToken, string> MakePair(LexicalToken token,
                                                                   string text)
        {
            return new KeyValuePair<LexicalToken, string>(token, text);
        }

        /// <summary>
        /// トークンと文字列のペアに直します。
        /// </summary>
        private static KeyValuePair<LexicalToken, string> CreateToken(Match m)
        {
            if (m.Groups[1].Success)
            {
                var typename = m.Groups[1].Value;

                if (m.Groups[2].Success)
                {
                    return MakePair(LexicalToken.GenericTypeName, typename);
                }
                else
                {
                    return MakePair(LexicalToken.TypeName, typename);
                }
            }
            else if (m.Groups[3].Success)
            {
                return MakePair(LexicalToken.OpenBlanket, null);
            }
            else if (m.Groups[4].Success)
            {
                return MakePair(LexicalToken.CloseBlanket, null);
            }
            else if (m.Groups[5].Success)
            {
                return MakePair(LexicalToken.Comma, null);
            }
            else
            {
                return MakePair(LexicalToken.Unknown, null);
            }
        }

        private readonly List<KeyValuePair<LexicalToken, string>> tokenList;
        private int index;

        /// <summary>
        /// 現在のトークンを取得します。
        /// </summary>
        public LexicalToken Token
        {
            get
            {
                if (this.index >= this.tokenList.Count())
                {
                    return LexicalToken.End;
                }

                return this.tokenList[this.index].Key;
            }
        }

        /// <summary>
        /// 前回解析した型名を取得します。
        /// </summary>
        public string Name
        {
            get
            {
                if (this.index >= this.tokenList.Count())
                {
                    return null;
                } 
                
                return this.tokenList[this.index].Value;
            }
        }

        /// <summary>
        /// 解析を進め、次のトークンを取得します。
        /// </summary>
        public LexicalToken NextToken()
        {
            ++this.index;
            return Token;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TypeLexer(string text)
        {
            var mc = TypeRegex.Matches(text);
            if (mc == null || mc.Count == 0)
            {
                throw new RagnarokException(
                    string.Format("{0}: 型名のパースに失敗しました。", text));
            }

            var last = mc[mc.Count - 1];
            if (last.Index + last.Length < text.Length)
            {
                throw new RagnarokException(
                    string.Format("{0}: 型名のパースに失敗しました。", text));
            }

            this.tokenList = mc.OfType<Match>()
                .Select(_ => CreateToken(_))
                .ToList();
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
                    // ジェネリック型の引数をシリアライズし[]でくくります。
                    // 再帰するのがポイント。
                    var paramNameList = type.GetGenericArguments()
                        .Select(_ => Serialize(_))
                        .Select(_ => string.Format("[{0}]", _))
                        .ToArray();
                    var paramStr = string.Join(", ", paramNameList);

                    result.Append("[");
                    result.Append(paramStr);
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
                throw new ArgumentNullException("serializedTypeName");
            }
            
            // レキサからトークンを取り出しながら構文解析を行います。
            var lexer = new TypeLexer(serializedTypeName);

            // 最初のトークンを取得します。
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
