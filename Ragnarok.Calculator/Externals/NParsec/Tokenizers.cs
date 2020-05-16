namespace Codehaus.Parsec
{
    using System;
    using StdToken = TypedToken<TokenType>;
    /// <summary> The facade class responsible for creating some basic tokens.</summary>
    /// <author>  Ben Yu
    /// 
    /// 2004-11-15
    /// </author>
    public static class Tokens
    {
        /// <summary> create a TypedToken object.</summary>
        /// <param name="type">the data type.</param>
        /// <param name="text">the text.
        /// </param>
        /// <returns> the token object.
        /// </returns>
        public static TypedToken<T> CreateTypedToken<T>(T type, string text)
        {
            return new TypedToken<T>(type, text);
        }
        /// <summary> create a TokenWord object.</summary>
        /// <param name="text">the word.
        /// </param>
        /// <returns> the token object.
        /// </returns>
        public static StdToken CreateWordToken(string text)
        {
            return CreateTypedToken(TokenType.Word, text);
        }
        /// <summary> create a TokenWord object.</summary>
        /// <param name="text">the word.
        /// </param>
        /// <returns> the token object.
        /// </returns>
        public static StdToken CreateReservedWordToken(string text)
        {
            return CreateTypedToken(TokenType.Reserved, text);
        }

        /// <summary> create a decimal literal token object.</summary>
        /// <param name="text">the decimal string representation.
        /// </param>
        /// <returns> the token object.
        /// </returns>
        public static StdToken CreateDecimalToken(string text)
        {
            return CreateTypedToken(TokenType.Decimal, text);
        }
        /// <summary> create an integer literal token object.</summary>
        /// <param name="text">the integer string representation.
        /// </param>
        /// <returns> the token object.
        /// </returns>
        public static StdToken CreateIntegerToken(string text)
        {
            return CreateTypedToken(TokenType.Integer, text);
        }
        /// <summary> create a character literal token.</summary>
        /// <param name="c">the character.
        /// </param>
        /// <returns> the token object.
        /// </returns>
        public static char CreateCharToken(char c)
        {
            return c;
        }
        /// <summary>
        /// create a integer token that's within the range of a long variable.
        /// </summary>
        /// <param name="val">the long value.</param>
        /// <returns></returns>
        public static long CreateLongToken(long val)
        {
            return val;
        }

        /// <summary> Create a quoted string token.</summary>
        /// <param name="open">the open quote
        /// </param>
        /// <param name="close">the close quote
        /// </param>
        /// <param name="s">the quoted string
        /// </param>
        /// <returns> the token object.
        /// </returns>
        public static TokenQuoted CreateQuotedStringToken(string open, string close, string s)
        {
            return new TokenQuoted(open, close, s);
        }
        /// <summary> Create a string literal token.</summary>
        /// <param name="s">the string literal.
        /// </param>
        /// <returns> the token object.
        /// </returns>
        public static string CreateStringToken(string s)
        {
            return s;
        }
        /// <summary>
        /// Get the token for eof.
        /// </summary>
        public static TokenEof Eof
        {
            get { return TokenEof.Instance; }
        }
    }
    /// <summary>
    /// The facade class to create all tokenizers.
    /// </summary>
    public static class Tokenizers
    {
        /// <summary>
        /// Create a Tokenizer that always returns the same value regardless of the input range.
        /// </summary>
        /// <param name="v">The target value.</param>
        /// <returns>the Tokenizer object.</returns>
        public static Tokenizer ForValue<R>(R v)
        {
            return delegate(string src, int begin, int len)
            {
                return v;
            };
        }
        /// <summary>
        /// Get a Tokenizer object that translates the recognized text range to a TypedToken of a certain data type.
        /// </summary>
        /// <param name="type">the data type.</param>
        /// <returns>the Tokenizer object.</returns>
        public static Tokenizer ForTypedToken<T>(T type)
        {
            return delegate(string src, int begin, int len)
            {
                return new TypedToken<T>(type, src.Substring(begin, len));
            };
        }
        /// <summary>
        /// Get the Tokenizer that tokenizes a char literal.
        /// </summary>
        public static Tokenizer ForChar
        {
            get { return TokenAsChar.char_literal; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes a decimal integer literal that's within the range of a long value.
        /// </summary>
        public static Tokenizer ForLong
        {
            get { return TokenAsLong.token_decimal_long; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes an oct integer literal that's within the range of a long value.
        /// </summary>
        public static Tokenizer ForOctLong
        {
            get { return TokenAsLong.token_oct_long; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes a hex integer literal that's within the range of a long value.
        /// </summary>
        public static Tokenizer ForHexLong
        {
            get { return TokenAsLong.token_hex_long; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes any integral number literal.
        /// </summary>
        public static Tokenizer ForInteger
        {
            get { return integer_tokenizer; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes any decimal number literal.
        /// </summary>
        public static Tokenizer ForDecimal
        {
            get { return decimal_tokenizer; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes to a word.
        /// </summary>
        public static Tokenizer ForWord
        {
            get { return word_tokenizer; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes to a reserved word.
        /// </summary>
        public static Tokenizer ForReservedWord
        {
            get { return reserved_word_tokenizer; }
        }
        /// <summary>
        /// The Tokenizer that simply tokenizes the string range to a string object.
        /// </summary>
        public static Tokenizer ForString
        {
            get { return TokenStringLiteral.token_string; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes a sql style string literal quoted by a pair of single quote.
        /// "''" is an escape and considered one single quote character.
        /// </summary>
        public static Tokenizer ForSqlStringLiteral
        {
            get { return TokenStringLiteral.token_sql_string_literal; }
        }
        /// <summary>
        /// The Tokenizer that tokenizes a simple string literal quoted by a pair of double quote characters.
        /// "\" is used to escape.
        /// </summary>
        public static Tokenizer ForSimpleStringLiteral
        {
            get { return TokenStringLiteral.token_simple_string_literal; }
        }
        /// <summary> creates a Tokenizer instance that can parse a string quoted by open and close.</summary>
        /// <param name="open">the open quote
        /// </param>
        /// <param name="close">the close quote
        /// </param>
        /// <returns> the tokenizer.
        /// </returns>
        public static Tokenizer ForQuotedString(char open, char close)
        {
            return TokenQuoted.GetTokenizer(open, close);
        }
        /// <summary> creates a Tokenizer instance that can parse a string quoted by open and close.</summary>
        /// <param name="open">the opening quote
        /// </param>
        /// <param name="close">the closeing quote
        /// </param>
        /// <returns> the tokenizer.
        /// </returns>
        public static Tokenizer ForQuotedString(string open, string close)
        {
            return TokenQuoted.GetTokenizer(open, close);
        }
        static readonly Tokenizer integer_tokenizer = ForTypedToken(TokenType.Integer);
        static readonly Tokenizer decimal_tokenizer = ForTypedToken(TokenType.Decimal);
        static readonly Tokenizer reserved_word_tokenizer = ForTypedToken(TokenType.Reserved);
        static readonly Tokenizer word_tokenizer = ForTypedToken(TokenType.Word);
    }
    /// <summary>
    /// The standard data types pre-built by parsec.
    /// </summary>
    public enum TokenType
    {
        Integer, Decimal, Word, Reserved
    }
    class TokenAsChar
    {
        internal static readonly Tokenizer char_literal = delegate(string src, int begin, int len)
        {
            if (len == 3)
            {
                return src[begin + 1];
            }
            else if (len == 4)
            {
                return src[begin + 2];
            }
            else
                throw new System.ArgumentException("illegal char");
        };
    }
    class TokenStringLiteral
    {
        internal static readonly Tokenizer token_string = delegate(string src, int begin, int len)
        {
            return src.Substring(begin, len);
        };
        internal static readonly Tokenizer token_sql_string_literal = delegate(string src, int begin, int len)
        {
            int end = begin + len - 1;
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            for (int i = begin + 1; i < end; i++)
            {
                char c = src[i];
                if (c != '\'')
                    buf.Append(c);
                else
                {
                    buf.Append('\'');
                    i++;
                }
            }
            return buf.ToString();
        };
        internal static readonly Tokenizer token_simple_string_literal = delegate(string src, int begin, int len)
        {
            int end = begin + len - 1;
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            for (int i = begin + 1; i < end; i++)
            {
                char c = src[i];
                if (c != '\\')
                {
                    buf.Append(c);
                }
                else
                {
                    char c1 = src[++i];
                    buf.Append(escapedChar(c1));
                }
            }
            return buf.ToString();
        };
        static char escapedChar(char c)
        {
            switch (c)
            {
                case 'r':
                    return '\r';

                case 'n':
                    return '\n';

                case 't':
                    return '\t';

                default:
                    return c;

            }
        }
    }
    class TokenAsLong
    {
        internal static readonly Tokenizer token_decimal_long = delegate(string src, int begin, int len)
        {
            int end = begin + len;
            long n = 0;
            for (int i = begin; i < end; i++)
            {
                n = n * 10 + toDecDigit(src[i]);
            }
            return n;
        };
        internal static readonly Tokenizer token_oct_long = delegate(string src, int begin, int len)
        {
            int end = begin + len;
            long n = 0;
            for (int i = begin; i < end; i++)
            {
                n = n * 8 + toOctDigit(src[i]);
            }
            return n;
        };
        internal static readonly Tokenizer token_hex_long = delegate(string src, int begin, int len)
        {
            int end = begin + len;
            long n = 0;
            for (int i = begin; i < end; i++)
            {
                n = n * 16 + toHexDigit(src[i]);
            }
            return n;
        };
        static int toDecDigit(char c)
        {
            return c - '0';
        }
        static int toOctDigit(char c)
        {
            return c - '0';
        }
        static int toHexDigit(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'a' && c <= 'h')
                return c - 'a' + 10;
            else
                return c - 'A' + 10;
        }
    }
    /// <summary>
    /// Represents a token associated with a recognized text.
    /// </summary>
    [Serializable]
    public class TypedToken<T>
    {
        readonly T type;
        readonly string raw;
        /// <summary>
        /// The token text.
        /// </summary>
        public string Text
        {
            get { return raw; }
        }
        /// <summary>
        /// The token type.
        /// </summary>
        public T Type
        {
            get { return type; }
        }
        /// <summary>
        /// Create a TypedToken object.
        /// </summary>
        /// <param name="type">the token type.</param>
        /// <param name="text">the token text.</param>
        public TypedToken(T type, string text)
        {
            this.type = type;
            this.raw = text;
        }
        public override string ToString()
        {
            return raw;
        }
        public override bool Equals(object obj)
        {
            if (obj is TypedToken<T>)
            {
                TypedToken<T> other = obj as TypedToken<T>;
                return type.Equals(other.type) && raw == other.raw;
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            return type.GetHashCode() * 31 + raw.GetHashCode();
        }
    }


    /// <summary> represents a string that is quoted by a open and close string.
    /// Use this token if the value of open quote and close quote matters to the syntax.
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// 2004-11-15
    /// </author>
    [Serializable]
    public class TokenQuoted
    {
        /// <summary> Returns the closing quote.</summary>
        /// <returns> the closing quote
        /// </returns>
        public string Close
        {
            get
            {
                return close;
            }

        }
        /// <summary> Returns the quoted text.</summary>
        /// <returns> the quoted text
        /// </returns>
        public string Quoted
        {
            get
            {
                return quoted;
            }

        }
        /// <summary> Returns the opening quote.</summary>
        /// <returns> the opening quote
        /// </returns>
        public string Open
        {
            get
            {
                return open;
            }

        }
        readonly string open;
        readonly string close;
        readonly string quoted;

        /// <param name="open">the open quote
        /// </param>
        /// <param name="close">the close quote
        /// </param>
        /// <param name="quoted">the quoted string
        /// </param>

        public TokenQuoted(string open, string close, string quoted)
        {
            this.open = open;
            this.close = close;
            this.quoted = quoted;
        }
        public bool Equals(TokenQuoted other)
        {
            return open == other.open && close == other.close && quoted == other.quoted;
        }
        public override bool Equals(object obj)
        {
            if (obj is TokenQuoted)
            {
                return Equals(obj as TokenQuoted);
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            return open.GetHashCode() + quoted.GetHashCode() + close.GetHashCode();
        }
        public override string ToString()
        {
            return open + quoted + close;
        }
        /// <summary> creates a Tokenizer instance that can parse a string quoted by open and close.</summary>
        /// <param name="open">the open quote
        /// </param>
        /// <param name="close">the close quote
        /// </param>
        /// <returns> the tokenizer.
        /// </returns>
        internal static Tokenizer GetTokenizer(char open, char close)
        {
            return GetTokenizer("" + open, "" + close);
        }
        /// <summary> creates a Tokenizer instance that can parse a string quoted by open and close.</summary>
        /// <param name="open">the opening quote
        /// </param>
        /// <param name="close">the closeing quote
        /// </param>
        /// <returns> the tokenizer.
        /// </returns>
        internal static Tokenizer GetTokenizer(string open, string close)
        {
            return delegate(string src, int begin, int len)
            {
                return new TokenQuoted(open, close,
                  src.Substring(begin + open.Length, len - open.Length - close.Length));
            };
        }

    }
    /// <summary> Special token to represent end of input.
    /// 2006-4-28</summary>
    /// <author>Ben Yu</author>
    /// 
    [Serializable]
    public sealed class TokenEof
    {
        TokenEof() { }
        static readonly TokenEof singleton = new TokenEof();
        internal static TokenEof Instance
        {
            get { return singleton; }
        }
    }
}
