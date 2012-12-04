using Codehaus.Parsec;

namespace Codehaus.Parsec
{
    using CharPredicate = Predicate<char>;
    using Scanner = Parser<D_>;
    using CharPattern = Codehaus.Parsec.Pattern;
    /// <summary> Scanners class provides basic character level parsers
    /// and the combinators that only work on scanners.
    /// </summary>
    /// <author>  Ben Yu
    /// </author>
    public class Scanners
    {
        class PatternScanner : Scanner
        {
            readonly CharPattern pp;
            readonly string expected_name;
            internal PatternScanner(string name, CharPattern pp, string expected_name)
                : base(name)
            {
                this.pp = pp;
                this.expected_name = expected_name;
            }
            internal override bool apply(ParseContext ctxt, ref D_ result, ref AbstractParsecError err)
            {
                int at = ctxt.getAt();
                string src = ctxt.getSource();
                int mlen = pp.Match(src, at, src.Length);
                if (mlen < 0)
                {
                    return Scanners.setErrorExpecting(out err, expected_name, ctxt);
                }
                ctxt.next(mlen);
                return true;
            }
        }
        class AnyCharScanner : Scanner
        {
            readonly string expected_name;
            internal AnyCharScanner(string expected_name)
            {
                this.expected_name = expected_name;
            }
            internal override bool apply(ParseContext ctxt, ref D_ result, ref AbstractParsecError err)
            {
                if (ctxt.isEof())
                {
                    return Scanners.setErrorExpecting(out err, expected_name, ctxt);
                }
                ctxt.next();
                return true;
            }
        }
        class IsCharScanner : Scanner
        {
            readonly string expected_name;
            readonly CharPredicate cp;
            internal IsCharScanner(CharPredicate cp, string expected_name)
            {
                this.expected_name = expected_name;
                this.cp = cp;
            }
            internal override bool apply(ParseContext ctxt, ref D_ result, ref AbstractParsecError err)
            {
                if (ctxt.isEof())
                {
                    return Scanners.setErrorExpecting(out err, expected_name, ctxt);
                }
                char c = ctxt.peekChar();
                if (cp(c))
                {
                    ctxt.next();
                    return true;
                }
                else
                {
                    return Scanners.setErrorExpecting(out err, expected_name, ctxt);
                }
            }
        }
        class NestableBlockComment : Scanner
        {
            readonly Scanner open;
            readonly Scanner close;
            readonly Scanner commented;
            internal NestableBlockComment(Scanner open, Scanner close, Scanner commented)
                : base("nestable block comment")
            {
                this.open = open;
                this.close = close;
                this.commented = commented;
            }
            internal override bool apply(ParseContext ctxt, ref D_ result, ref AbstractParsecError err)
            {
                if (!open.parse(ctxt, ref result, ref err))
                {
                    return false;
                }
                for (int level = 1; level > 0; )
                {
                    int at = ctxt.getAt();
                    if (close.parse(ctxt, ref result, ref err))
                    {
                        if (at == ctxt.getAt())
                        {
                            throw new IllegalParserStateException("closing comment scanner not consuming input.");
                        }
                        level--;
                        continue;
                    }
                    if (at != ctxt.getAt())
                    {
                        return false;
                    }
                    if (open.parse(ctxt, ref result, ref err))
                    {
                        if (at == ctxt.getAt())
                            throw new IllegalParserStateException("opening comment scanner not consuming input.");
                        level++;
                        continue;
                    }
                    if (at != ctxt.getAt())
                    {
                        return false;
                    }
                    if (commented.parse(ctxt, ref result, ref err))
                    {
                        if (at == ctxt.getAt())
                            throw new IllegalParserStateException("commented scanner not consuming input.");
                        continue;
                    }
                    return false;
                }
                return true;
            }
        }

        class NestedScanner : Scanner
        {

            readonly Scanner outer_scanner;
            private string module;
            readonly Scanner nested;
            internal NestedScanner(Scanner outer_scanner, string module, Scanner nested_scanner)
            {
                this.outer_scanner = outer_scanner;
                this.module = module;
                this.nested = nested_scanner;
            }
            internal override bool apply(ParseContext ctxt, ref D_ result, ref AbstractParsecError err)
            {
                int from = ctxt.getAt();
                if (!outer_scanner.parse(ctxt, ref result, ref err))
                    return false;
                ScannerState inner_ctxt = new ScannerState(ctxt.getSource(), from, module, ctxt.getPositionMap(),
                  ctxt.getAt() - from);
                return ParserChores.cont(ctxt, inner_ctxt, nested, ref result, ref err);
            }
        }
        class NotChar2Pattern : CharPattern
        {
            public NotChar2Pattern(char c1, char c2)
            {
                this.c1 = c1;
                this.c2 = c2;
            }
            readonly char c1;
            readonly char c2;
            public override int Match(string src, int begin, int end)
            {
                if (begin >= end - 1)
                    return CharPattern.MISMATCH;
                if (src[begin] == c1 && src[begin + 1] == c2)
                    return CharPattern.MISMATCH;
                return 1;
            }
        }
        /// <summary> Scans greedily for 0 or more characters
        /// that satisfies the given CharPredicate.
        /// </summary>
        /// <param name="cp">the predicate object.
        /// </param>
        /// <returns> the Scanner object.
        /// </returns>
        public static Scanner Many(CharPredicate cp)
        {
            return IsPattern("many chars", Patterns.Many(cp), "many");
        }

        /// <summary> Scans greedily for 0 or more occurrences of the given pattern.</summary>
        /// <param name="pp">the CharPattern object.
        /// </param>
        /// <returns> the Scanner object.
        /// </returns>
        public static Scanner Many(CharPattern pp)
        {
            return IsPattern("many patterns", pp.Many(), "many");
        }

        /// <summary> matches the input against the specified string.</summary>
        /// <param name="str">the string to match
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsString(string str)
        {
            return IsString(str, str);
        }
        /// <summary> matches the input against the specified string.</summary>
        /// <param name="str">the string to match
        /// </param>
        /// <param name="expected_name">the name of the expected pattern.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsString(string str, string expected_name)
        {
            return IsPattern("isString", Patterns.IsString(str), expected_name);
        }
        /// <summary> Scans greedily for 1 or more whitespace characters.</summary>
        /// <returns> the Scanner object.
        /// </returns>
        public static Scanner IsWhitespaces()
        {
            return IsWhitespaces("whitespace");
        }
        /// <summary> Scans greedily for 1 or more whitespace characters.</summary>
        /// <param name="expected_name">the expected message when fails.
        /// </param>
        /// <returns> the Scanner object.
        /// </returns>
        public static Scanner IsWhitespaces(string expected_name)
        {
            return IsPattern("isWhitespaces", Patterns.Many(1, CharPredicates.IsWhitespace()), expected_name);
        }
        /// <summary> Scans the input for an occurrence of a string pattern.</summary>
        /// <param name="name">the name of the result parser. It should be a meaningful name that speaks out the pattern.</param>
        /// <param name="pp">the CharPattern object.
        /// </param>
        /// <param name="expected_name">the expected message when fails.
        /// </param>
        /// <returns> the Scanner object.
        /// </returns>
        public static Scanner IsPattern(string name, CharPattern pp, string expected_name)
        {
            return new PatternScanner(name, pp, expected_name);
        }
        /// <summary> Scans the input for an occurrence of a string pattern.</summary>
        /// <param name="pp">the CharPattern object.
        /// </param>
        /// <param name="expected_name">the expected message when fails.
        /// </param>
        /// <returns> the Scanner object.
        /// </returns>
        public static Scanner IsPattern(CharPattern pp, string expected_name)
        {
            return IsPattern(expected_name, pp, expected_name);
        }
        //case insensitive
        /// <summary> matches the input against the specified string case insensitively.</summary>
        /// <param name="str">the string to match
        /// </param>
        /// <param name="expected_name">the name of the expected pattern.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsStringCI(string str, string expected_name)
        {
            return IsPattern("isStringCI", Patterns.IsStringCI(str), expected_name);
        }

        /// <summary> matches the input against the specified string case insensitively.</summary>
        /// <param name="str">the string to match
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsStringCI(string str)
        {
            return IsStringCI(str, str);
        }
        /// <summary> matches any character in the input.
        /// Different from one(), it fails on EOF. Also it consumes the current character in the input.
        /// </summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner AnyChar()
        {
            return AnyChar("any character");
        }
        /// <summary> matches any character in the input.
        /// Different from one(), it fails on EOF. Also it consumes the current character in the input.
        /// </summary>
        /// <param name="expected_name">the name of the expected pattern.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner AnyChar(string expected_name)
        {
            return new AnyCharScanner(expected_name).Rename("any char");
        }

        /// <summary> succeed and consume the current character if it satisfies the given CharPredicate.</summary>
        /// <param name="cp">the predicate.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsChar(CharPredicate cp)
        {
            return IsChar(cp, "satisfiable char");
        }

        /// <summary> succeed and consume the current character if it satisfies the given CharPredicate.</summary>
        /// <param name="cp">the predicate.
        /// </param>
        /// <param name="expected_name">the error message.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsChar(CharPredicate cp, string expected_name)
        {
            return new IsCharScanner(cp, expected_name).Rename("isChar");
        }


        /// <summary> succeed and consume the current character if it is equal to ch.</summary>
        /// <param name="ch">the expected character.
        /// </param>
        /// <param name="expected_name">the error message.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsChar(char ch, string expected_name)
        {
            return IsChar(CharPredicates.IsChar(ch), expected_name);
        }

        /// <summary> succeed and consume the current character if it is equal to ch.</summary>
        /// <param name="ch">the expected character.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsChar(char ch)
        {
            return IsChar(ch, CharEncoder.Encode(ch));
        }

        /// <summary> succeed and consume the current character if it is equal to ch.</summary>
        /// <param name="ch">the expected character.
        /// </param>
        /// <param name="expected_name">the error message.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner NotChar(char ch, string expected_name)
        {
            return IsChar(CharPredicates.NotChar(ch), expected_name).Rename("not char");
        }

        /// <summary> succeed and consume the current character if it is not equal to ch.</summary>
        /// <param name="ch">the expected character.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner NotChar(char ch)
        {
            return NotChar(ch, "^" + CharEncoder.Encode(ch));
        }


        static System.Text.StringBuilder toString(System.Text.StringBuilder buf, char[] chars)
        {
            buf.Append('[');
            if (chars.Length > 0)
            {
                buf.Append(CharEncoder.Encode(chars[0]));
                for (int i = 1; i < chars.Length; i++)
                {
                    buf.Append(',').Append(CharEncoder.Encode(chars[i]));
                }
            }
            buf.Append(']');
            return buf;
        }
        static Scanner _among(char[] chars, string expected_name)
        {
            return IsChar(CharPredicates.Among(chars), expected_name).Rename("among");
        }
        static Scanner _notAmong(char[] chars, string expected_name)
        {
            return IsChar(CharPredicates.NotAmong(chars), expected_name).Rename("not among");
        }


        /// <summary> succeed and consume the current character if it equals to one of the given characters.
        /// 
        /// </summary>
        /// <param name="chars">the characters.
        /// </param>
        /// <param name="expected_name">the error message when the character is not among the given values.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner Among(char[] chars, string expected_name)
        {
            if (chars.Length == 0)
                return Parsers.Zero<D_>();
            if (chars.Length == 1)
                return IsChar(chars[0], expected_name);
            return _among(chars, expected_name);
        }
        /// <summary> succeed and consume the current character if it equals to one of the given characters.
        /// 
        /// </summary>
        /// <param name="chars">the characters.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner Among(char[] chars)
        {
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            toString(buf, chars);
            return Among(chars, buf.ToString());
        }


        /// <summary> succeed and consume the current character if it is not equal to any of the given characters.
        /// 
        /// </summary>
        /// <param name="chars">the characters.
        /// </param>
        /// <param name="expected_name">the error message when the character is not among the given values.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner NotAmong(char[] chars, string expected_name)
        {
            if (chars.Length == 0)
                return AnyChar();
            if (chars.Length == 1)
                return NotChar(chars[0], expected_name);
            return _notAmong(chars, expected_name);
        }
        /// <summary> succeed and consume the current character if it is not equal to any of the given characters.
        /// 
        /// </summary>
        /// <param name="chars">the characters.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner NotAmong(char[] chars)
        {
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            buf.Append("^");
            toString(buf, chars);
            return NotAmong(chars, buf.ToString());
        }

        /// <summary> if the current input starts with the given string, succeed and consumes all the characters until the end of line '\n character.
        /// It does not consume the end of line character.
        /// </summary>
        /// <param name="start">the start string.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsLineComment(string start)
        {
            return IsPattern("line comment", Patterns.IsLineComment(start), start);
        }

        /// <summary> scanner for c++/java style line comment.</summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsJavaLineComment()
        {
            return IsLineComment("//").Rename("java style line comment");
        }
        /// <summary> scanner for T-SQL style line comment.</summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsSqlLineComment()
        {
            return IsLineComment("--").Rename("sql style line comment");
        }
        /// <summary> scanner for c++/java style block comment. </summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsJavaBlockComment()
        {
            //return isBlockComment("/*", "*/");
            return Parsers.Sequence(IsString("/*"), p_javaBlockCommented(), IsString("*/"));
            //isNestableBlockComment("/*", "*/");
        }
        /// <summary> scanner for haskell style block comment. ({- -})</summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsHaskellBlockComment()
        {
            return Parsers.Seq(IsString("{-"), p_haskellBlockCommented(), IsString("-}"));
        }
        /// <summary> scanner for haskell style line comment. (--)</summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsHaskellLineComment()
        {
            return IsLineComment("--");
        }
        /// <summary> scanner for non-nested block comment.</summary>
        /// <param name="start">the start string of a block comment.
        /// </param>
        /// <param name="end">the end string of a block comment. 
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsBlockComment(string start, string end)
        {
            CharPattern opening = Patterns.IsString(start).Seq(Patterns.NotString(end).Many());
            return IsPattern("block comment before the end", opening, start).Seq(IsString(end))
              .Rename("block comment");
        }

        /// <summary> Scans a non-nestable block comment.</summary>
        /// <param name="open">the opening string.
        /// </param>
        /// <param name="close">the closing string.
        /// </param>
        /// <param name="commented">the commented pattern.
        /// </param>
        /// <returns> the Scanner for the block comment.
        /// </returns>
        public static Scanner IsBlockComment(string open, string close, CharPattern commented)
        {
            CharPattern opening = Patterns.IsString(open)
              .Seq(Patterns.IsString(close).Not().Seq(commented).Many());
            return IsPattern("block comment before the end", opening, open).Seq(IsString(close));
        }
        /// <summary> Scans a non-nestable block comment.</summary>
        /// <param name="open">the opening pattern.
        /// </param>
        /// <param name="close">the closing pattern.
        /// </param>
        /// <param name="commented">the commented pattern.
        /// </param>
        /// <returns> the Scanner for the block comment.
        /// </returns>
        public static Scanner IsBlockComment(Scanner open, Scanner close, Scanner commented)
        {
            return Parsers.Sequence(open, close.Not().Seq(commented).Many_(), close);
        }

        /// <summary> Scans a nestable block comment.
        /// Nested comments and any other characters can be in the comment body.
        /// </summary>
        /// <param name="open">the opening pattern.
        /// </param>
        /// <param name="close">the closing pattern.
        /// </param>
        /// <returns> the block comment scanner.
        /// </returns>
        public static Scanner IsNestableBlockComment(string open, string close)
        {
            return IsNestableBlockComment(open, close, AnyChar());
        }
        /// <summary> Scans a nestable block comment.
        /// Nested comments and some commented CharPattern can be in the comment body.
        /// </summary>
        /// <param name="open">the opening string.
        /// </param>
        /// <param name="close">the closing string.
        /// </param>
        /// <param name="commented">the commented CharPattern except for nested comments.
        /// </param>
        /// <returns> the block comment scanner.
        /// </returns>
        public static Scanner IsNestableBlockComment(string open, string close, Scanner commented)
        {
            return IsNestableBlockComment(IsString(open), IsString(close), commented);
        }
        /// <summary> Scans a nestable block comment.
        /// Nested comments and some commented CharPattern can be in the comment body.
        /// </summary>
        /// <param name="open">the opening pattern.
        /// </param>
        /// <param name="close">the closing pattern.
        /// </param>
        /// <param name="commented">the commented CharPattern except for nested comments.
        /// </param>
        /// <returns> the block comment scanner.
        /// </returns>
        public static Scanner IsNestableBlockComment(Scanner open, Scanner close, Scanner commented)
        {
            return new NestableBlockComment(open, close, commented);
        }

        /// <summary> a scanner with a CharPattern for sql server string literal.
        /// a sql server string literal is a string quoted by single quote, 
        /// a single quote character is escaped by 2 single quotes.
        /// </summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsSqlString()
        {
            /*
            final CharPattern open = Patterns.IsChar('\'').Seq(
            Patterns.or(Patterns.NotChar('\''), Patterns.IsString("''"))
            .Many()
            );
            return isPattern(open, "'").Seq(name, isChar('\''));
            */
            Scanner q = IsChar('\'');
            Scanner qs = IsPattern("sql string quoted", Patterns.Regex("(('')|[^'])*"), "quoted string");
            return qs.Between(q, q).Rename("sql string");
        }


        static readonly CharPattern quoted_str = Patterns.Regex("((\\\\.)|[^\"\\\\])*");
        /// <summary> a scanner with a CharPattern for double quoted string literal.
        /// backslash '\' is used as escape character.
        /// </summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsQuotedString()
        {
            /*
            final CharPattern q = Patterns.IsChar('"');
            final CharPattern open = q.Seq(quoted().Many());
            return isPattern(open, "\"").Seq(name, isPattern(q, "\""));
            */
            Scanner q = Scanners.IsChar('"');
            Scanner qc = IsPattern("string quoted", quoted_str, "quoted string");
            return qc.Between(q, q).Rename("quoted string");
        }

        static readonly CharPattern quoted_char = Patterns.Regex("(\\\\.)|[^'\\\\]");
        /// <summary> scanner for a c/c++/java style character literal. such as 'a' or '\\'.</summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsQuotedChar()
        {
            //final Scanner q = isChar('\'');
            /*
            final CharPattern q = Patterns.IsChar('\'');
            final CharPattern qc = Patterns.or(
            Patterns.IsString("\\'"),
            Patterns.NotChar('\'')
            );
            return isPattern(q.Seq(qc), "'").Seq(name, isPattern(q, "'"));
            */
            Scanner q = Scanners.IsChar('\'');
            Scanner qc = IsPattern("char quoted", quoted_char, "quoted char");
            return qc.Between(q, q).Rename("quoted char");
        }
        /// <summary> scans a quoted string that is opened by c1 and closed by c2. </summary>
        /// <param name="c1">the opening character.
        /// </param>
        /// <param name="c2">the closing character.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsQuotedBy(char c1, char c2)
        {
            return IsPattern("open quote and quoted",
              Patterns.IsChar(c1).Seq(Patterns.Many(CharPredicates.NotChar(c2))), "" + c1)
              .Seq(IsChar(c2)).Rename("quoted");
        }

        /// <summary> scans a quoted string that is opened by c1 and closed by c2.</summary>
        /// <param name="open">the opening character.
        /// </param>
        /// <param name="close">the closing character.
        /// </param>
        /// <param name="quoted_scanner">the scanner quoted character. This scanner will be repeated until the close pattern is met.</param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsQuotedBy(Scanner open, Scanner close, Scanner quoted_scanner)
        {
            return Parsers.Sequence(open, quoted_scanner.Many_(), close).Rename("quoted");
        }

        /// <summary> the c++/java style delimiter of tokens. 
        /// whitespace, line comment, block comment.
        /// </summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsJavaDelimiter()
        {
            return Parsers.Plus(IsWhitespaces(), IsJavaLineComment(), IsJavaBlockComment()).Many_()
              .Rename("java delimiter");
        }

        /// <summary> the haskell style delimiter of tokens. 
        /// whitespace, line comment, block comment.
        /// </summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsHaskellDelimiter()
        {
            return Parsers.Plus(IsWhitespaces(), IsHaskellBlockComment(), IsHaskellLineComment()).Many_()
              .Rename("haskell delimiter");
        }

        /// <summary> the T-SQL style delimiter of tokens. 
        /// whitespace and line comment.
        /// </summary>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsSqlDelimiter()
        {
            return Parsers.Plus(IsWhitespaces(), IsSqlLineComment(), IsJavaBlockComment()).Many_()
              .Rename("sql delimiter");
        }

        /// <summary> Any delimiter with whitespace, non-nested block comment and line comment.</summary>
        /// <param name="lcomment">line comment starting string.
        /// </param>
        /// <param name="openc">block comment opening string.
        /// </param>
        /// <param name="closec">block comment closing string.
        /// </param>
        /// <returns> the scanner.
        /// </returns>
        public static Scanner IsStdDelimiter(string lcomment, string openc, string closec)
        {
            return Parsers.Plus(IsWhitespaces(), IsLineComment(lcomment), IsBlockComment(openc, closec)).Many_()
              .Rename("delimiter");
        }


        /// <summary> If a string is not followed by a alphanumeric character, it is well-delimited.
        /// delimited() make sure the CharPattern represented by scanner s is delimited.
        /// </summary>
        /// <param name="s">the scanner for the to-be-delimited pattern.
        /// </param>
        /// <param name="expected_name">the error message if it is not delimited.
        /// </param>
        /// <returns> the new scanner.
        /// </returns>
        public static Scanner Delimited(Scanner s, string expected_name)
        {
            return s.FollowedBy(IsChar(CharPredicates.IsAlphaNumeric(), expected_name).Not())
              .Rename("delimited");
        }

        /// <summary> If a string is not followed by a alphanumeric character, it is well-delimited.
        /// delimited() make sure the CharPattern represented by scanner s is delimited.
        /// </summary>
        /// <param name="s">the scanner for the to-be-delimited pattern.
        /// </param>
        /// <returns> the new scanner.
        /// </returns>
        public static Scanner Delimited(Scanner s)
        {
            return Delimited(s, "delimiter");
        }

        /// <summary> After character level parser p succeeds,
        /// subsequently feed the recognized characters to the Scanner scanner
        /// for a nested scanning.
        /// </summary>
        /// <param name="p">the first parser object to identify the characters.
        /// </param>
        /// <param name="scanner">the second parser object to scan the characters again.
        /// </param>
        /// <param name="module">the module name.
        /// </param>
        /// <returns> the new Scanner object.
        /// </returns>
        public static Scanner ScanChars(Scanner p, Scanner scanner, string module)
        {
            return ScanChars("scanChars", p, scanner, module);
        }
        /// <summary> After character level parser p succeeds,
        /// subsequently feed the recognized characters to the Scanner scanner
        /// for a nested scanning.
        /// </summary>
        /// <param name="name">the name of the new Scanner object.
        /// </param>
        /// <param name="p">the first parser object to identify the characters.
        /// </param>
        /// <param name="scanner">the second parser object to scan the characters again.
        /// </param>
        /// <param name="module">the module name.
        /// </param>
        /// <returns> the new Scanner object.
        /// </returns>
        public static Scanner ScanChars(string name, Scanner p, Scanner scanner, string module)
        {
            return new NestedScanner(p, module, scanner);
        }
        /// <summary> Matches a character if the input has at least 1 character; 
        /// and if the input has at least 2 characters, 
        /// the first 2 characters are not c1 and c2.
        /// </summary>
        /// <returns> the CharPattern object.
        /// </returns>
        private static CharPattern notChar2(char c1, char c2)
        {
            return new NotChar2Pattern(c1, c2);
        }

        private static Scanner p_javaBlockCommented()
        {
            return IsPattern("java style block commented", notChar2('*', '/').Many(), "commented block");
        }


        private static Scanner p_haskellBlockCommented()
        {
            return IsPattern("haskell style block commented", notChar2('-', '}').Many(), "commented block");
        }
        private static bool setErrorExpecting(out AbstractParsecError err, string msg, ParseContext ctxt)
        {
            err = ParserChores.raiseExpecting(msg, ctxt);
            return false;
        }
    }
    class CharEncoder
    {
        public static string Encode(char c)
        {
            switch (c)
            {
                case ' ': return "SPACE";
                case '\t': return "HTAB";
                case '\r': return "CR";
                case '\n': return "LF";
                default: return "" + c;
            }
        }
    }
}
