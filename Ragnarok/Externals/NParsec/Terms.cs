namespace Codehaus.Parsec
{
    using System;
    using Lexer = Parser<Tok>;
    using FromString = FromRange<string, object>;
    using Scanner = Parser<D_>;
    using Term = Parser<object>;
    /// <summary> This helper class provides convenient api's to build lexer
    /// and parsers for terminals.
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// Nov 20, 2004
    /// </author>
    [Serializable]
    public sealed class Terms
    {
        /// <summary> gets the lexer for the terminals.</summary>
        /// <returns> the lexer.
        /// </returns>
        public Lexer Lexer
        {
            get
            {
                return words.Lexer;
            }

        }
        readonly Words words;
        /// <summary> gets the parser for the terminals identified by tnames.
        /// The Tok object is returned from the result parser.
        /// </summary>
        /// <param name="tnames">the names of the terminals.
        /// </param>
        /// <returns> the Parser.
        /// </returns>
        public Term GetParser(params string[] tnames)
        {
            return GetParser(tnames, toLabel(tnames));
        }
        /// <summary> gets the parser for the terminals identified by tnames.
        /// The Tok object is returned from the result parser.
        /// </summary>
        /// <param name="tnames">the names of the terminals.
        /// </param>
        /// <param name="expected">the label when this parser fails.
        /// </param>
        /// <returns> the Parser.
        /// </returns>
        public Term GetParser(string[] tnames, string expected)
        {
            if (tnames.Length == 0)
                return Parsers.Zero<object>();
            Term[] ps = new Term[tnames.Length];
            for (int i = 0; i < tnames.Length; i++)
            {
                ps[i] = Parsers.FromToken(isToken<object>(words[tnames[i]]));
            }
            return Parsers.Plus(ps).Label(expected);
        }


        /// <summary> gets the parser for a terminal identified by tname.
        /// The Tok object is returned from the result parser.
        /// </summary>
        /// <param name="tname">the name of the terminal.
        /// </param>
        /// <param name="expected">the label when this parser fails.
        /// </param>
        /// <returns> the Parser.
        /// </returns>
        public Term GetParser(string tname, string expected)
        {
            return Parsers.FromToken(isToken<object>(words[tname])).Label(expected)
              .Rename(tname);
        }
        /// <summary> gets the parser for a terminal identified by tname.
        /// The Tok object is returned from the result parser.
        /// </summary>
        /// <param name="tname">the name of the terminal.
        /// </param>
        /// <returns> the Parser.
        /// </returns>
        public Term GetParser(string tname)
        {
            return GetParser(tname, tname);
        }
        /// <summary> Creates a Terms object for lexing and parsing the operators with names specified in ops,
        /// and for lexing and parsing the keywords case insensitively. 
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// A word is defined as an alpha numeric string that starts with [_a-zA-Z],
        /// with 0 or more [0-9_a-zA-Z] following.
        /// </summary>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <returns> the Terms instance.
        /// </returns>
        public static Terms GetCaseInsensitiveInstance(string[] ops, string[] keywords)
        {
            return new Terms(Lexers.GetCaseInsensitive(ops, keywords));
        }
        /// <summary> Creates a Terms object for lexing and parsing the operators with names specified in ops,
        /// and for lexing and parsing the keywords case sensitively. 
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// A word is defined as an alpha numeric string that starts with [_a-zA-Z],
        /// with 0 or more [0-9_a-zA-Z] following.
        /// </summary>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <returns> the Terms instance.
        /// </returns>
        public static Terms GetCaseSensitiveInstance(string[] ops, string[] keywords)
        {
            return new Terms(Lexers.GetCaseSensitive(ops, keywords));
        }
        /// <summary> Creates a Terms object for lexing and parsing the operators with names specified in ops,
        /// and for lexing and parsing the keywords case insensitively. 
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// </summary>
        /// <param name="wscanner">the scanner that identifies a word in the language.
        /// </param>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <returns> the Terms instance.
        /// </returns>
        public static Terms GetCaseInsensitiveInstance(Scanner wscanner, string[] ops, string[] keywords)
        {
            return new Terms(Lexers.GetCaseInsensitive(wscanner, ops, keywords));
        }
        /// <summary> Creates a Terms object for lexing and parsing the operators with names specified in ops,
        /// and for lexing and parsing the keywords case sensitively. 
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// </summary>
        /// <param name="wscanner">the scanner that identifies a word in the language.
        /// </param>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <returns> the Terms instance.
        /// </returns>
        public static Terms GetCaseSensitiveInstance(Scanner wscanner, string[] ops, string[] keywords)
        {
            return new Terms(Lexers.GetCaseSensitive(wscanner, ops, keywords));
        }

        /// <summary> Creates a Terms object for lexing and parsing the operators with names specified in ops,
        /// and for lexing and parsing the keywords case insensitively. 
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// </summary>
        /// <param name="wscanner">the scanner that identifies a word in the language.
        /// </param>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <param name="toWord">the FromString object used to create a token for non-key words recognized by wscanner.
        /// </param>
        /// <returns> the Terms instance.
        /// </returns>
        public static Terms GetCaseInsensitiveInstance(Scanner wscanner, string[] ops, string[] keywords, FromString toWord)
        {
            return new Terms(Lexers.GetCaseInsensitive(wscanner, ops, keywords, toWord));
        }
        /// <summary> Creates a Terms object for lexing and parsing the operators with names specified in ops,
        /// and for lexing and parsing the keywords case sensitively. 
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// </summary>
        /// <param name="wscanner">the scanner that identifies a word in the language.
        /// </param>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <param name="toWord">the FromString object used to create a token for non-key words recognized by wscanner.
        /// </param>
        /// <returns> the Terms instance.
        /// </returns>
        public static Terms GetCaseSensitiveInstance(Scanner wscanner, string[] ops, string[] keywords,
          FromString toWord)
        {
            return new Terms(Lexers.GetCaseSensitive(wscanner, ops, keywords, toWord));
        }
        /// <summary> Creates a Terms object for lexing the operators with names specified in ops.
        /// Operators are lexed as TokenReserved.
        /// </summary>
        /// <param name="ops">the operator names.
        /// </param>
        /// <returns> the Terms instance.
        /// </returns>
        public static Terms GetOperatorsInstance(params string[] ops)
        {
            return new Terms(Lexers.GetOperators(ops));
        }

        /// <summary> gets a Parser object to parse Character token.</summary>
        /// <param name="fc">the mapping to map char to an object returned by the parser.
        /// </param>
        /// <returns> the parser
        /// </returns>
        public static Parser<T> OnChar<T>(FromRange<char, T> fc)
        {
            return OnToken(fc).Rename("char");
        }
        /// <summary> gets a Parser object to parse String token.</summary>
        /// <param name="fc">the mapping to map String to an object returned by the parser.
        /// </param>
        /// <returns> the parser
        /// </returns>
        public static Parser<T> OnString<T>(FromRange<string, T> fc)
        {
            return OnToken(fc).Rename("string");
        }
        /// <summary> gets a Parser object to parse TokenQuoted.</summary>
        /// <param name="fc">the mapping to map the quoted string to an object returned by the parser.
        /// </param>
        /// <returns> the parser
        /// </returns>
        public static Parser<T> OnQuotedWord<T>(FromRange<string, string, string, T> fc)
        {
            FromToken<T> recognizer = delegate(Tok tok, ref T result)
            {
                object obj = tok.Token;
                if (obj is TokenQuoted)
                {
                    TokenQuoted quoted = obj as TokenQuoted;
                    result = fc(tok.Index, tok.Length, quoted.Open, quoted.Quoted, quoted.Close);
                    return true;
                }
                else return false;
            };
            return Parsers.FromToken(recognizer);
        }
        /// <summary> gets a Parser object to parse TokenWord.</summary>
        /// <param name="fc">the mapping to map the word to an object returned by the parser.
        /// </param>
        /// <returns> the parser
        /// </returns>
        public static Parser<T> OnWord<T>(FromRange<string, T> fc)
        {
            return OnTypedToken(TokenType.Word, fc).Rename("word");
        }
        /// <summary> gets a Parser object to parse Long token.</summary>
        /// <param name="fc">the mapping to map the number to an object returned by the parser.
        /// </param>
        /// <returns> the parser
        /// </returns>
        public static Parser<T> OnInteger<T>(FromRange<string, T> fc)
        {
            return OnTypedToken(TokenType.Integer, fc).Rename("integer");
        }
        /// <summary> gets a Parser object to parse TokenDecimal.</summary>
        /// <param name="fc">the mapping to map the decimal to an object returned by the parser.
        /// </param>
        /// <returns> the parser
        /// </returns>
        public static Parser<T> OnDecimal<T>(FromRange<string, T> fc)
        {
            return OnTypedToken(TokenType.Decimal, fc).Rename("decimal");
        }

        public static Parser<R> OnToken<T, R>(FromRange<T, R> f)
        {
            return Parsers.FromToken(FromSimpleToken(f));
        }
        public static Parser<R> OnTypedToken<T, R>(T type, FromRange<string, R> f)
        {
            return Parsers.FromToken(FromTypedToken(type, f));
        }
        /// <summary>
        /// Create a FromToken object that recognizes a token object of any arbitrary type.
        /// </summary>
        /// <typeparam name="T">The token type recognized.</typeparam>
        /// <typeparam name="R">The result type of the FromToken object.</typeparam>
        /// <param name="f">the FromRange object used to translate the recognized range to a certain result.</param>
        /// <returns>The FromToken object.</returns>
        public static FromToken<R> FromSimpleToken<T, R>(FromRange<T, R> f)
        {
            return delegate(Tok tok, ref R result)
            {
                object obj = tok.Token;
                if (obj is T)
                {
                    T t = (T)obj;
                    result = f(tok.Index, tok.Length, t);
                    return true;
                }
                else return false;
            };
        }
        /// <summary>
        /// Create a FromToken object that recognizes TypedToken of a certain type.
        /// </summary>
        /// <typeparam name="T">The token type.</typeparam>
        /// <typeparam name="R">The result type of the FromToken</typeparam>
        /// <param name="type">The token type.</param>
        /// <param name="f">The FromRange object used to translate the recognized range to a certain result.</param>
        /// <returns>The FromToken object</returns>
        public static FromToken<R> FromTypedToken<T, R>(T type, FromRange<string, R> f)
        {
            return delegate(Tok tok, ref R result)
            {
                object t = tok.Token;
                if (t is TypedToken<T>)
                {
                    TypedToken<T> tokobj = t as TypedToken<T>;
                    if (!tokobj.Type.Equals(type))
                        return false;
                    result = f(tok.Index, tok.Length, tokobj.Text);
                    return true;
                }
                else return false;
            };
        }
        Terms(Words words)
        {
            this.words = words;
        }
        static FromToken<T> isToken<T>(object t)
        {
            return delegate(Tok tok, ref T result)
            {
                return (tok.Token == t);
            };
        }
        static string toLabel(string[] keys)
        {
            if (keys.Length == 0)
                return "";
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            buf.Append('[').Append(keys[0]);
            for (int i = 1; i < keys.Length; i++)
            {
                buf.Append(',').Append(keys[i]);
            }
            buf.Append(']');
            return buf.ToString();
        }
    }
}
