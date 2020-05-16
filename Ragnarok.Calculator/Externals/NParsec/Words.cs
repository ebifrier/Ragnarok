namespace Codehaus.Parsec
{
    using Lexer = Parser<Tok>;
    using Scanner = Parser<D_>;
    using TokenMap = Map<string, object>;
    using FromString = FromRange<string, object>;
    using System;
    using System.Collections;

    /// <summary> This helper class provides convenient api's to build lexer
    /// and parsers for keywords and operators.
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// Dec 19, 2004
    /// </author>
    [Serializable]
    public sealed class Words
    {
        /// <summary> gets the lexer object.</summary>
        /// <returns> the lexer object.
        /// </returns>
        public Lexer Lexer
        {
            get
            {
                return lexer;
            }

        }
        readonly TokenMap words;
        readonly Lexer lexer;
        Words(Map<string, object> map, Parser<Tok> lexer)
        {
            this.words = map;
            this.lexer = lexer;
        }

        /// <summary> the token object identified by the token text. 
        /// This text is the operator or the keyword.
        /// </summary>
        /// <param name="name">the token text.
        /// </param>
        /// <returns> the token object. 
        /// </returns>
        /// <exception cref="System.ArgumentException">if the token object does not exist.
        /// </exception>
        public object this[string name]
        {
            get
            {
                object p = words(name);
                if (p == null)
                    throw new System.ArgumentException("token " + name + " unavailable");
                return p;
            }
        }
        static bool same(string a, string b, bool cs)
        {
            if (cs)
            {
                return a == b;
            }
            else
                return a.ToUpperInvariant() == b.ToUpperInvariant();
        }
        static void checkDup(string[] a, string[] b, bool cs)
        {
            for (int i = 0; i < a.Length; i++)
            {
                string s1 = a[i];
                for (int j = 0; j < b.Length; j++)
                {
                    string s2 = b[j];
                    if (same(s1, s2, cs))
                        throw new System.ArgumentException(s1 + " duplicated");
                }
            }
        }
        /// <summary> Creates a Words object for lexing the operators with names specified in ops.
        /// Operators are lexed as TokenReserved.
        /// </summary>
        /// <param name="ops">the operator names.
        /// </param>
        /// <returns> the Words instance.
        /// </returns>
        internal static Words getOperators(string[] ops)
        {
            WordsData data = Operators.instance(ops);
            return new Words(data.Tokens, Parsers.Plus(data.Lexers));
        }
        static readonly Scanner default_word = Scanners.IsPattern("word", Patterns.IsWord(), "word");
        /// <summary> Creates a Words object for lexing the operators with names specified in ops,
        /// and for lexing the keywords case insensitively.
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// A word is defined as an alpha numeric string that starts with [_a-zA-Z],
        /// with 0 or more [0-9_a-zA-Z] following. 
        /// </summary>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <returns> the Words instance.
        /// </returns>
        internal static Words getCaseInsensitive(string[] ops, string[] keywords)
        {
            return instance(default_word, ops, keywords, false, String2Word.Instance);
        }
        /// <summary> Creates a Words object for lexing the operators with names specified in ops,
        /// and for lexing the keywords case sensitively. 
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// A word is defined as an alpha numeric string that starts with [_a-zA-Z],
        /// with 0 or more [0-9_a-zA-Z] following.
        /// </summary>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <returns> the Words instance.
        /// </returns>
        internal static Words getCaseSensitive(string[] ops, string[] keywords)
        {
            return instance(default_word, ops, keywords, true, String2Word.Instance);
        }
        /// <summary> Creates a Words object for lexing the operators with names specified in ops,
        /// and for lexing the keywords case insensitively.
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// </summary>
        /// <param name="wscanner">the scanner for a word in the language.
        /// </param>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <returns> the Words instance.
        /// </returns>
        internal static Words getCaseInsensitive(Scanner wscanner, string[] ops, string[] keywords)
        {
            return instance(wscanner, ops, keywords, false, String2Word.Instance);
        }
        /// <summary> Creates a Words object for lexing the operators with names specified in ops,
        /// and for lexing the keywords case sensitively. 
        /// Keywords and operators are lexed as TokenReserved.
        /// Words that are not among the keywords are lexed as TokenWord. 
        /// </summary>
        /// <param name="wscanner">the scanner for a word in the language.
        /// </param>
        /// <param name="ops">the operator names.
        /// </param>
        /// <param name="keywords">the keyword names.
        /// </param>
        /// <returns> the Words instance.
        /// </returns>
        internal static Words getCaseSensitive(Scanner wscanner, string[] ops, string[] keywords)
        {
            return instance(wscanner, ops, keywords, true, String2Word.Instance);
        }

        internal static Words getCaseSensitive<T>(Scanner wscanner, string[] ops, string[] keywords,
          FromRange<string, T> toWord)
        {
            return instance(wscanner, ops, keywords, true, toWord);
        }
        internal static Words getCaseInsensitive<T>(Scanner wscanner, string[] ops, string[] keywords,
          FromRange<string, T> toWord)
        {
            return instance(wscanner, ops, keywords, false, toWord);
        }
        private static Words instance<T>(Scanner wscanner, string[] ops, string[] keywords, bool cs,
          FromRange<string, T> toWord)
        {
            checkDup(ops, keywords, true);
            WordsData data = combine(Operators.instance(ops), Keywords.getInstance("word", wscanner, keywords, cs, toWord));
            return toWords(data);
        }
        private static Words toWords(WordsData data)
        {
            return new Words(data.Tokens, Parsers.Plus(data.Lexers));
        }
        private static WordsData combine(WordsData w1, WordsData w2)
        {
            TokenMap map1 = w1.Tokens;
            TokenMap map2 = w2.Tokens;
            Lexer[] l1 = w1.Lexers;
            Lexer[] l2 = w2.Lexers;
            TokenMap map = delegate(string n)
            {
                object t1 = map1(n);
                if (t1 != null)
                    return t1;
                return map2(n);
            };
            Lexer[] l = new Lexer[l1.Length + l2.Length];
            Array.Copy(l1, 0, l, 0, l1.Length);
            Array.Copy(l2, 0, l, l1.Length, l2.Length);
            return new WordsData(map, l);
        }
    }

    [Serializable]
    sealed class WordsData
    {
        /// <returns> Returns the lexers.
        /// </returns>
        internal Lexer[] Lexers
        {
            get
            {
                return lexers;
            }
        }
        /// <returns> Returns the parsers.
        /// </returns>
        internal TokenMap Tokens
        {
            get
            {
                return toks;
            }
        }
        readonly TokenMap toks;
        private Lexer[] lexers;

        /// <param name="toks">
        /// </param>
        /// <param name="lexers">
        /// </param>
        internal WordsData(TokenMap toks, Lexer[] lexers)
        {
            this.toks = toks;
            this.lexers = lexers;
        }
    }
}
