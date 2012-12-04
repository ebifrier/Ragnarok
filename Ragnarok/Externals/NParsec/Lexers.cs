namespace Codehaus.Parsec
{
    using Scanner = Parser<D_>;
    using Lexer = Parser<Tok>;
    using Lexeme = Parser<Tok[]>;
    using FromString = FromRange<string, object>;
    public delegate object Tokenizer(string src, int begin, int len);
    class ScannerToLexer<S> : Lexer
    {
        readonly Parser<S> scanner;
        readonly Tokenizer tn;
        internal ScannerToLexer(Parser<S> scanner, Tokenizer tn)
        {
            this.scanner = scanner;
            this.tn = tn;
        }
        internal override bool apply(ParseContext ctxt, ref Tok result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            S tmp = default(S);
            if (!scanner.parse(ctxt, ref tmp, ref err))
            {
                return false;
            }
            int cur = ctxt.getAt();
            int len = cur - original_at;
            object tok = tn(ctxt.getSource(), original_at, len);
            //The java version uses null to indicate a failure. It doesn't not seem to be used though. So we keep it simple.
            result = new Tok(original_at, len, tok);
            ctxt.setStep(original_step + 1);
            return true;
        }
    }

    public class Lexers
    {
        /// <summary>
        /// Create a FromRange object that converts any range to a certain value.
        /// </summary>
        /// <param name="v">the target value.</param>
        /// <returns>the FromRange object.</returns>
        public static FromRange<From, To> ToValue<From, To>(To v)
        {
            return delegate(int begin, int len, From data)
            {
                return v;
            };
        }
        /// <summary> Transform the recognized character range of scanner s to a token object
        /// with a Tokenizer. 
        /// If the Tokenizer.toToken() returns null, scan fails.
        /// </summary>
        /// <param name="s">the scanner to transform.
        /// </param>
        /// <param name="tn">the Tokenizer object.
        /// </param>
        /// <returns> the new Scanner.
        /// </returns>
        public static Lexer Lex(Scanner s, Tokenizer tn)
        {
            return new ScannerToLexer<D_>(s, tn);
        }

        /// <summary> Greedily runs a parser token_scanner repeatedly,
        /// and ignores the Pattern recognized by Scanner delim before and after each token_scanner.
        /// The result Tok objects are collected and returned in a Tok[] array.
        /// </summary>
        /// <param name="delim">the delimiter Scanner object.
        /// </param>
        /// <param name="token_scanner">the Scanner object.
        /// </param>
        /// <returns> the new Scanner object.
        /// </returns>
        public static Lexeme Lexeme(Scanner delim, Parser<Tok> token_scanner)
        {
            return delim.Optional().Seq(token_scanner.SepEndBy(delim));
        }

        /// <summary> returns the lexer that's gonna parse single quoted character literal (escaped by '\'),
        /// and then converts the character to a Character.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexCharLiteral()
        {
            return Lex(Scanners.IsQuotedChar(), Tokenizers.ForChar).Rename("char literal");
        }
        /// <summary> returns the lexer that's gonna parse double quoted string literal (escaped by '\'),
        /// and convert the string to a String token.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexSimpleStringLiteral()
        {
            return Lex(Scanners.IsQuotedString(), Tokenizers.ForSimpleStringLiteral)
              .Rename("double quoted string literal");
        }
        /// <summary> returns the lexer that's gonna parse single quoted string literal (single quote is escaped with another single quote),
        /// and convert the string to a String token.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexSqlStringLiteral()
        {
            return Lex(Scanners.IsSqlString(), Tokenizers.ForSqlStringLiteral)
              .Rename("sql style string literal");
        }

        /// <summary> returns the lexer that's gonna parse a decimal number (valid patterns are: 1, 2.3, 000, 0., .23),
        /// and convert the string to a TokenDecimal.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexDecimal()
        {
            string name = "decimal literal";
            return Lex(Scanners.Delimited(
              Scanners.IsPattern(name, Patterns.IsDecimal(), "decimal number")),
              Tokenizers.ForDecimal).Rename(name);
        }
        /// <summary> returns the lexer that's gonna parse a integer number (valid patterns are: 0, 00, 1, 10),
        /// and convert the string to a Long token.
        /// The difference between integer() and decInteger() is that decInteger does not allow a number starting with 0.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexInteger()
        {
            string name = "integer literal";
            return Lex(Scanners.Delimited(Scanners.IsPattern("integer literal", Patterns.IsInteger(), "integer")),
              Tokenizers.ForInteger).Rename(name);
        }
        /// <summary> returns the lexer that's gonna parse a decimal integer number (valid patterns are: 1, 10, 123),
        /// and convert the string to a Long token.
        /// The difference between integer() and decInteger() is that decInteger does not allow a number starting with 0.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexDecimalLong()
        {
            string name = "decimal integer literal";
            return Lex(Scanners.Delimited(Scanners.IsPattern(name, Patterns.IsDecInteger(), "decInteger")),
              Tokenizers.ForLong);
        }

        /// <summary> returns the lexer that's gonna parse a octal integer number (valid patterns are: 0, 07, 017, 0371 jfun.yan.etc.),
        /// and convert the string to a Long token.
        /// an octal number has to start with 0.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexOctLong()
        {
            string name = "oct integer literal";
            return Lex(Scanners.Delimited(Scanners.IsPattern(name, Patterns.IsOctInteger(), "octInteger")),
              Tokenizers.ForOctLong).Rename(name);
        }
        /// <summary> returns the lexer that's gonna parse a hex integer number (valid patterns are: 0x1, 0Xff, 0xFe1 jfun.yan.etc.),
        /// and convert the string to a Long token.
        /// an hex number has to start with either 0x or 0X.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexHexLong()
        {
            string name = "hex integer literal";
            return Lex(Scanners.Delimited(Scanners.IsPattern(name, Patterns.IsHexInteger(), "hexInteger")),
              Tokenizers.ForHexLong).Rename(name);
        }
        /// <summary> returns the lexer that's gonna parse decimal, hex, and octal numbers
        /// and convert the string to a Long token.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexLong()
        {
            return Parsers.Plus(LexHexLong(), LexDecimalLong(), LexOctLong()).Rename("long integer literal");
        }
        /// <summary> returns the lexer that's gonna parse any word.
        /// and convert the string to a TokenWord.
        /// A word starts with an alphametic character, followed by 0 or more alphanumeric characters.
        /// </summary>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexWord()
        {
            string name = "word";
            return Lex(Scanners.Delimited(Scanners.IsPattern(name, Patterns.IsWord(), name)),
              Tokenizers.ForWord).Rename(name);
        }


        /// <summary> Create a lexer that parsers a string literal quoted by open and close,
        /// and then converts it to a String token instance.
        /// </summary>
        /// <param name="open">the opening character.
        /// </param>
        /// <param name="close">the closing character.
        /// </param>
        /// <returns> the lexer.
        /// </returns>
        public static Lexer LexQuotedString(char open, char close)
        {
            return Lex(Scanners.IsQuotedBy(open, close),
              Tokenizers.ForQuotedString(open, close)).Rename("quoted string literal");
        }

        /// <summary> Creates a Words object for lexing the operators with names specified in ops.
        /// Operators are lexed as TokenReserved.
        /// </summary>
        /// <param name="ops">the operator names.
        /// </param>
        /// <returns> the Words instance.
        /// </returns>
        public static Words GetOperators(params string[] ops)
        {
            return Words.getOperators(ops);
        }
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
        public static Words GetCaseInsensitive(string[] ops, string[] keywords)
        {
            return Words.getCaseInsensitive(ops, keywords);
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
        public static Words GetCaseSensitive(string[] ops, string[] keywords)
        {
            return Words.getCaseSensitive(ops, keywords);
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
        public static Words GetCaseInsensitive(Scanner wscanner, string[] ops, string[] keywords)
        {
            return Words.getCaseInsensitive(wscanner, ops, keywords);
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
        public static Words GetCaseSensitive(Scanner wscanner, string[] ops, string[] keywords)
        {
            return Words.getCaseSensitive(wscanner, ops, keywords);
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
        /// <param name="toWord">the FromString object used to create a token for non-key words recognized by wscanner.
        /// </param>
        /// <returns> the Words instance.
        /// </returns>
        public static Words GetCaseInsensitive(Scanner wscanner, string[] ops, string[] keywords,
          FromString toWord)
        {
            return Words.getCaseInsensitive(wscanner, ops, keywords, toWord);
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
        /// <param name="toWord">the FromString object used to create a token for non-key words recognized by wscanner.
        /// </param>
        /// <returns> the Words instance.
        /// </returns>
        public static Words GetCaseSensitive(Scanner wscanner, string[] ops, string[] keywords,
          FromString toWord)
        {
            return Words.getCaseSensitive(wscanner, ops, keywords, toWord);
        }
    }
    class String2Word
    {
        static readonly FromString singleton = delegate(int from, int len, string text)
        {
            return Tokens.CreateWordToken(text);
        };
        internal static FromString Instance
        {
            get { return singleton; }
        }
    }
}
