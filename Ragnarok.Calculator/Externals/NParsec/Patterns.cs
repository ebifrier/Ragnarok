using Codehaus.Parsec;
using CharPredicate = Codehaus.Parsec.Predicate<char>;
using System.Text.RegularExpressions;
namespace Codehaus.Parsec
{


    /// <summary> This class provides all the basic Pattern implementations and all Pattern combinators.</summary>
    /// <author>  Ben Yu
    /// 
    /// Dec 16, 2004
    /// </author>
    public sealed class Patterns
    {

        internal static Pattern getRegularExpressionPattern()
        {
            Pattern quote = IsChar('/');
            Pattern escape = IsChar('\\').Seq(Patterns.HasAtLeast(1));
            char[] not_allowed = new char[] { '/', '\n', '\r', '\\' };
            Pattern content = Patterns.Or(escape, Patterns.NotAmong(not_allowed));
            return quote.Seq(content.Many()).Seq(quote);
        }
        internal static Pattern getModifiersPattern()
        {
            return Patterns.IsChar(CharPredicates.IsAlpha()).Many();
        }
        /// <summary> Ensures the input has at least l characters left.
        /// match length is l if succeed.
        /// </summary>
        /// <param name="l">the number of characters.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern HasAtLeast(int l)
        {
            return new LeastCharNumPattern(l);
        }
        /// <summary> Ensures the input has exactly l characters left.
        /// match length is l if succeed.
        /// </summary>
        /// <param name="l">the number of characters.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern HasExact(int l)
        {
            return new ExactCharNumPattern(l);
        }
        /// <summary> Ensures the input has no character left.
        /// match length is 0 if succeed.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Eof()
        {
            return HasExact(0);
        }
        /// <summary> Succeed with match length 1
        /// if the current character in the input is same as character c.
        /// Mismatch otherwise.
        /// </summary>
        /// <param name="c">the character to compare with.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsChar(char c)
        {
            return new IsCharPattern(c);
        }
        /// <summary> Succeed with match length 1
        /// if the current character in the input is between character c1 and c2.
        /// </summary>
        /// <param name="c1">the first character.
        /// </param>
        /// <param name="c2">the second character.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern InRange(char c1, char c2)
        {
            return new CharRangePattern(c1, c2);
        }
        /// <summary> Succeed with match length 1
        /// if the current character in the input is not between character c1 and c2.
        /// </summary>
        /// <param name="c1">the first character.
        /// </param>
        /// <param name="c2">the second character.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern NotInRange(char c1, char c2)
        {
            return new NotRangePattern(c1, c2);
        }
        /// <summary> Succeed with match length 1
        /// if the current character in the input is among the given characters.
        /// </summary>
        /// <param name="cs">the characters to compare with.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Among(params char[] cs)
        {
            return IsChar(CharPredicates.Among(cs));
        }
        /// <summary> Succeed with match length 1
        /// if the current character in the input is not among the given characters.
        /// </summary>
        /// <param name="cs">the characters to compare with.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern NotAmong(params char[] cs)
        {
            return IsChar(CharPredicates.NotAmong(cs));
        }
        /// <summary> Succeed with match length 1
        /// if the current character in the input is not the same as character c.
        /// Mismatch otherwise.
        /// </summary>
        /// <param name="c">the character to compare with.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern NotChar(char c)
        {
            return new NotCharPattern(c);
        }
        /// <summary> Succeed with match length 1
        /// if the current character in the input satisfies the given predicate.
        /// Mismatch otherwise.
        /// </summary>
        /// <param name="cp">the predicate object.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsChar(CharPredicate cp)
        {
            return new SatisfiableCharPattern(cp);
        }
        /// <summary> Succeed with match length 2
        /// if there are at least 2 characters in the input and the first character is '\'
        /// Mismatch otherwise.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsEscaped()
        {
            return IsEscapedPattern.instance;
        }

        /// <summary> Matches a line comment that starts with a string
        /// and end with EOF or Line Feed character.
        /// </summary>
        /// <param name="open">the line comment starting string.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsLineComment(string open)
        {
            return IsString(open).Seq(Many(CharPredicates.NotChar('\n')));
        }
        /// <summary> Matches a string.</summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsString(string str)
        {
            return new IsStringPattern(str);
        }
        /// <summary> Matches a string case insensitively.</summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsStringCI(string str)
        {
            return new IsStringCaseInsensitivePattern(str);
        }
        /// <summary> Matches a character if the input has at least 1 character 
        /// and does not match the given string.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern NotString(string str)
        {
            return new NotStringPattern(str);
        }
        /// <summary> Matches a character if the input has at least 1 character 
        /// and does not match the given string case insensitively.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern NotStringCI(string str)
        {
            return new NotStringCaseInsensitivePattern(str);
        }

        static bool compareIgnoreCase(char a, char b)
        {
            return char.ToLower(a) == char.ToLower(b);
        }

        internal static int matchString(string str, string underlying_text, int starting_index, int ending_index)
        {
            //UPGRADE_NOTE: Final was removed from the declaration of 'slen '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
            int slen = str.Length;
            if (ending_index - starting_index < slen)
                return Pattern.MISMATCH;
            for (int i = 0; i < slen; i++)
            {
                char exp = str[i];
                char enc = underlying_text[starting_index + i];
                if (exp != enc)
                {
                    return Pattern.MISMATCH;
                }
            }
            return slen;
        }

        internal static int matchStringCI(string str, string underlying_text, int starting_index, int ending_index)
        {
            int slen = str.Length;
            if (ending_index - starting_index < slen)
                return Pattern.MISMATCH;
            for (int i = 0; i < slen; i++)
            {
                char exp = str[i];
                char enc = underlying_text[starting_index + i];
                if (!compareIgnoreCase(exp, enc))
                {
                    return Pattern.MISMATCH;
                }
            }
            return slen;
        }



        /// <summary> if the first Pattern object pp1 mismatches, try the second Pattern object pp2.</summary>
        /// <param name="pp1">the 1st Pattern object.
        /// </param>
        /// <param name="pp2">the 2nd Pattern object.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public static Pattern Or(Pattern pp1, Pattern pp2)
        {
            return new OrPattern(pp1, pp2);
        }


        /// <summary> Matches if the input has at least n characters
        /// and the first n characters all satisfy the given predicate.
        /// </summary>
        /// <param name="n">the number of characters to test.
        /// </param>
        /// <param name="cp">the predicate object.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Repeat(int n, CharPredicate cp)
        {
            if (n == 0)
                return Always();
            if (n == 1)
                return IsChar(cp);
            return new CharRepretitionPattern(n, cp);
        }

        static int min(int a, int b)
        {
            return a > b ? b : a;
        }
        /// <summary> Matches if the input starts with min or more characters
        /// that all satisfy the given predicate,
        /// mismatch otherwise.
        /// </summary>
        /// <param name="min">the minimal number of characters to match.
        /// </param>
        /// <param name="cp">the predicate.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Many(int min, CharPredicate cp)
        {
            if (min < 0)
                throw new System.ArgumentException("min<0");
            return new ManyMinCharsPattern(min, cp);
        }
        /// <summary> Matches 0 or more characters that all satisfy the given predicate.</summary>
        /// <param name="cp">the predicate.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Many(CharPredicate cp)
        {
            return new ManyCharsPattern(cp);
        }


        /// <summary> Matches at least min and at most max number of characters
        /// that satisfies the given predicate,
        /// mismatch otherwise.
        /// </summary>
        /// <param name="min">the minimal number of characters.
        /// </param>
        /// <param name="max">the maximal number of characters.
        /// </param>
        /// <param name="cp">the predicate.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Some(int min, int max, CharPredicate cp)
        {
            if (max < 0 || min < 0 || min > max)
                throw new System.ArgumentException();
            if (max == 0)
                return Always();
            return new SomeMinCharsPattern(min, cp, max);
        }
        /// <summary> Matches at most max number of characters
        /// that satisfies the given predicate.
        /// </summary>
        /// <param name="max">the maximal number of characters.
        /// </param>
        /// <param name="cp">the predicate.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Some(int max, CharPredicate cp)
        {
            if (max < 0)
                throw new System.ArgumentException("max<0");
            if (max == 0)
                return Always();
            return new SomeCharsPattern(max, cp);
        }
        /// <summary> Try two pattern objects, pick the one with the longer match length.
        /// If two pattern objects have the same length, the first one is favored.
        /// </summary>
        /// <param name="p1">the 1st pattern object.
        /// </param>
        /// <param name="p2">the 2nd pattern object.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public static Pattern Longer(Pattern p1, Pattern p2)
        {
            return Longest(p1, p2);
        }
        /// <summary> Try an array of pattern objects, pick the one with the longest match length.
        /// If two pattern objects have the same length, the first one is favored.
        /// </summary>
        /// <param name="pps">the array of Pattern objects.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public static Pattern Longest(params Pattern[] pps)
        {
            return new LongestPattern(pps);
        }
        /// <summary> Try two pattern objects, pick the one with the shorter match length.
        /// If two pattern objects have the same length, the first one is favored.
        /// </summary>
        /// <param name="p1">the 1st pattern object.
        /// </param>
        /// <param name="p2">the 2nd pattern object.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public static Pattern Shorter(Pattern p1, Pattern p2)
        {
            return Shortest(p1, p2);
        }
        /// <summary> Try an array of pattern objects, pick the one with the shortest match length.
        /// If two pattern objects have the same length, the first one is favored.
        /// </summary>
        /// <param name="pps">the array of Pattern objects.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public static Pattern Shortest(params Pattern[] pps)
        {
            return new ShortestPattern(pps);
        }


        /// <summary> A Pattern object that always returns MISMATCH.</summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Never()
        {
            return NeverPattern.instance;
        }
        /// <summary> A Pattern object that always matches with 0 length.</summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Always()
        {
            return AlwaysPattern.instance;
        }



        /// <summary> a decimal number that has at least one number before the decimal point.
        /// the decimal point and the numbers to the right are optional.
        /// 0, 11., 2.3 are all good candidates. While .1, . are not.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsDecimalL()
        {
            CharPredicate cp = CharPredicates.IsDigit();
            return Many(1, cp).Seq(IsChar('.').Seq(Many(cp)).Optional());
        }
        /// <summary> Recognizes a decimal point and 1 or more digits after it.</summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsDecimalR()
        {
            return IsChar('.').Seq(Many(1, CharPredicates.IsDigit()));
        }
        /// <summary> Recognizes a decimal number that can start with a decimal point.</summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsDecimal()
        {
            return Or(IsDecimalL(), IsDecimalR());
        }
        /// <summary> a pattern for a standard english word.
        /// it starts with an underscore or an alphametic character, followed by 0 or more alphanumeric characters.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsWord()
        {
            /*
            return seq(isChar(CharPredicates.IsAlpha_()), 
            many(CharPredicates.IsAlphaNumeric()));
            */
            return Regex("[a-zA-Z_][0-9a-zA-Z_]*");
        }
        /// <summary> pattern for an integer. ([0-9]+) </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsInteger()
        {
            return Many(1, CharPredicates.IsDigit());
        }
        /// <summary> pattern for a octal integer that starts with a 0 and followed by 0 or more [0-7] characters.</summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsOctInteger()
        {
            return IsChar('0').Seq(Many(CharPredicates.InRange('0', '7')));
        }
        /// <summary> pattern for a decimal integer. 
        /// It starts with a non-zero digit and followed by 0 or more digits.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsDecInteger()
        {
            return InRange('1', '9').Seq(Many(CharPredicates.IsDigit()));
        }
        /// <summary> pattern for a hex integer. 
        /// It starts with a 0x or 0X, followed by 1 or more hex digits.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsHexInteger()
        {
            return Or(IsString("0x"), IsString("0X")).Seq(Many(1, CharPredicates.IsHexDigit()));
        }
        /// <summary> Recognizes a the exponent part of a scientific number notation.
        /// It can be e12, E-1, jfun.yan.etc.
        /// </summary>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern IsExponential()
        {
            return Patterns.Sequence(Among(new char[] { 'e', 'E' }), IsChar('-').Optional(), IsInteger());
        }
        /// <summary> Adapt a regular expression pattern to a Pattern;</summary>
        /// <param name="regex">the regular expression pattern.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Regex(Regex regex)
        {
            return new RegexPattern(regex);
        }
        /// <summary> Adapt a regular expression pattern string to a Pattern;</summary>
        /// <param name="s">the regular expression pattern string.
        /// </param>
        /// <returns> the Pattern object.
        /// </returns>
        public static Pattern Regex(string s)
        {
            return Regex(new Regex(s, RegexOptions.Compiled));
        }

        /// <summary> Get the Pattern object that matches any regular expression pattern
        /// string in the form of /some pattern here/.
        /// '\' is used as escape character.
        /// </summary>
        public static Pattern IsRegularExpression()
        {
            return regex_pattern;
        }
        /// <summary> Get the pattern that matches regular expression modifiers.
        /// Basically this is a list of alpha characters.
        /// </summary>
        public static Pattern IsRegularExpressionModifiers()
        {
            return regex_modifiers;
        }
        public static Pattern Any(params Pattern[] pps)
        {
            return new AlternativePattern(pps);
        }
        public static Pattern Sequence(params Pattern[] pps)
        {
            return new SequentialPattern(pps);
        }
        public static Pattern All(params Pattern[] pps)
        {
            return new AllPattern(pps);
        }
        internal static int match_repeat(int n, CharPredicate cp, string underlying_text, int starting_index, int ending_index, int acc)
        {
            if (n > ending_index - starting_index)
            {
                return Pattern.MISMATCH;
            }
            int tail = n + starting_index;
            for (int i = starting_index; i < tail; i++)
            {
                if (!cp(underlying_text[i]))
                {
                    return Pattern.MISMATCH;
                }
            }
            return n + acc;
        }
        internal static int match_repeat(int n, Pattern pp, string underlying_text, int starting_index, int ending_index, int acc)
        {
            int cur = starting_index;
            for (int i = 0; i < n; i++)
            {
                int l = pp.Match(underlying_text, cur, ending_index);
                if (l == Pattern.MISMATCH)
                    return Pattern.MISMATCH;
                cur += l;
            }
            return cur - starting_index + acc;
        }
        internal static int match_some(int max, CharPredicate cp, string underlying_text, int starting_index, int ending_index, int acc)
        {
            int k = min(max + starting_index, ending_index);
            for (int i = starting_index; i < k; i++)
            {
                if (!cp(underlying_text[i]))
                    return i - starting_index + acc;
            }
            return k - starting_index + acc;
        }
        internal static int match_some(int max, Pattern pp, string underlying_text, int starting_index, int ending_index, int acc)
        {
            int cur = starting_index;
            for (int i = 0; i < max; i++)
            {
                int l = pp.Match(underlying_text, cur, ending_index);
                if (Pattern.MISMATCH == l)
                    break;
                cur += l;
            }
            return cur - starting_index + acc;
        }
        internal static int match_many(CharPredicate cp, string underlying_text, int starting_index, int ending_index, int acc)
        {
            for (int i = starting_index; i < ending_index; i++)
            {
                if (!cp(underlying_text[i]))
                    return i - starting_index + acc;
            }
            return ending_index - starting_index + acc;
        }
        internal static int match_many(Pattern pp, string underlying_text, int starting_index, int ending_index, int acc)
        {
            for (int cur = starting_index; ; )
            {
                //UPGRADE_NOTE: Final was removed from the declaration of 'l '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
                int l = pp.Match(underlying_text, cur, ending_index);
                //we simply stop the loop when infinity is found. this may make the parser more user-friendly.
                if (Pattern.MISMATCH == l || l == 0)
                    return cur - starting_index + acc;
                cur += l;
            }
        }

        static readonly Pattern regex_pattern = getRegularExpressionPattern();
        static readonly Pattern regex_modifiers = getModifiersPattern();
    }

    class LeastCharNumPattern : Pattern
    {
        internal LeastCharNumPattern(int l)
        {
            this.l = l;
        }
        readonly int l;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (ending_index - starting_index < l)
                return Pattern.MISMATCH;
            else
                return l;
        }
    }

    class ExactCharNumPattern : Pattern
    {
        internal ExactCharNumPattern(int l)
        {
            this.l = l;
        }
        readonly int l;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (ending_index - starting_index != l)
                return Pattern.MISMATCH;
            else
                return l;
        }
    }

    class IsCharPattern : Pattern
    {
        internal IsCharPattern(char c)
        {
            this.c = c;
        }

        readonly char c;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (starting_index >= ending_index)
                return Pattern.MISMATCH;
            else if (underlying_text[starting_index] != c)
                return Pattern.MISMATCH;
            else
                return 1;
        }
    }

    class CharRangePattern : Pattern
    {
        internal CharRangePattern(char c1, char c2)
        {
            this.c1 = c1;
            this.c2 = c2;
        }
        readonly char c1;
        readonly char c2;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (starting_index >= ending_index)
                return Pattern.MISMATCH;
            char c = underlying_text[starting_index];
            if (c >= c1 && c <= c2)
                return 1;
            else
                return Pattern.MISMATCH;
        }
    }

    class NotRangePattern : Pattern
    {
        internal NotRangePattern(char c1, char c2)
        {
            this.c1 = c1;
            this.c2 = c2;
        }
        readonly char c1;
        readonly char c2;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (starting_index >= ending_index)
                return Pattern.MISMATCH;
            char c = underlying_text[starting_index];
            if (c >= c1 && c <= c2)
                return Pattern.MISMATCH;
            else
                return 1;
        }
    }

    class NotCharPattern : Pattern
    {
        internal NotCharPattern(char c)
        {
            this.c = c;
        }
        readonly char c;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (starting_index >= ending_index)
                return Pattern.MISMATCH;
            else if (underlying_text[starting_index] == c)
                return Pattern.MISMATCH;
            else
                return 1;
        }
    }

    class SatisfiableCharPattern : Pattern
    {
        public SatisfiableCharPattern(CharPredicate cp)
        {
            this.cp = cp;
        }

        readonly CharPredicate cp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (starting_index >= ending_index)
                return Pattern.MISMATCH;
            else if (cp(underlying_text[starting_index]))
                return 1;
            else
                return Pattern.MISMATCH;
        }
        public override string ToString()
        {
            return Maps.ToString(cp);
        }
    }

    class IsEscapedPattern : Pattern
    {
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (ending_index - starting_index < 2)
                return Pattern.MISMATCH;
            else if (underlying_text[starting_index] == '\\')
                return 2;
            else
                return Pattern.MISMATCH;
        }
        IsEscapedPattern() { }
        internal static readonly Pattern instance = new IsEscapedPattern();
    }

    class IsStringPattern : Pattern
    {
        internal IsStringPattern(string str)
        {
            this.str = str;
        }

        readonly string str;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (ending_index - starting_index < str.Length)
                return MISMATCH;
            return Patterns.matchString(str, underlying_text, starting_index, ending_index);
        }
        public override string ToString()
        {
            return str;
        }
    }

    class IsStringCaseInsensitivePattern : Pattern
    {
        internal IsStringCaseInsensitivePattern(string str)
        {
            this.str = str;
        }
        readonly string str;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (ending_index - starting_index < str.Length)
                return MISMATCH;
            return Patterns.matchStringCI(str, underlying_text, starting_index, ending_index);
        }
    }

    class NotStringPattern : Pattern
    {
        internal NotStringPattern(string str)
        {
            this.str = str;
        }
        readonly string str;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (starting_index >= ending_index)
                return MISMATCH;
            if (Patterns.matchString(str, underlying_text, starting_index, ending_index) == Pattern.MISMATCH)
                return 1;
            else
                return MISMATCH;
        }
    }

    class NotStringCaseInsensitivePattern : Pattern
    {
        internal NotStringCaseInsensitivePattern(string str)
        {
            this.str = str;
        }
        readonly string str;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (starting_index >= ending_index)
                return MISMATCH;
            if (Patterns.matchStringCI(str, underlying_text, starting_index, ending_index) == Pattern.MISMATCH)
                return 1;
            else
                return MISMATCH;
        }
    }

    class NotPattern : Pattern
    {
        internal NotPattern(Pattern pp)
        {
            this.pp = pp;
        }
        readonly Pattern pp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (pp.Match(underlying_text, starting_index, ending_index) != Pattern.MISMATCH)
                return Pattern.MISMATCH;
            else
                return 0;
        }
    }

    class PeekPattern : Pattern
    {
        internal PeekPattern(Pattern pp)
        {
            this.pp = pp;
        }
        readonly Pattern pp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (pp.Match(underlying_text, starting_index, ending_index) == Pattern.MISMATCH)
                return Pattern.MISMATCH;
            else
                return 0;
        }
    }

    class OrPattern : Pattern
    {
        internal OrPattern(Pattern pp1, Pattern pp2)
        {
            this.pp1 = pp1;
            this.pp2 = pp2;
        }
        readonly Pattern pp1;
        readonly Pattern pp2;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int l1 = pp1.Match(underlying_text, starting_index, ending_index);
            if (l1 != Pattern.MISMATCH)
                return l1;
            else
                return pp2.Match(underlying_text, starting_index, ending_index);
        }
    }

    class SeqPattern : Pattern
    {
        internal SeqPattern(Pattern pp1, Pattern pp2)
        {
            this.pp1 = pp1;
            this.pp2 = pp2;
        }
        readonly Pattern pp1;
        readonly Pattern pp2;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int l1 = pp1.Match(underlying_text, starting_index, ending_index);
            if (l1 == Pattern.MISMATCH)
                return l1;
            int l2 = pp2.Match(underlying_text, starting_index + l1, ending_index);
            if (l2 == Pattern.MISMATCH)
                return l2;
            return l1 + l2;
        }
    }

    class CharRepretitionPattern : Pattern
    {
        internal CharRepretitionPattern(int n, CharPredicate cp)
        {
            this.n = n;
            this.cp = cp;
        }

        readonly int n;
        readonly CharPredicate cp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            return Patterns.match_repeat(n, cp, underlying_text, starting_index, ending_index, 0);
        }
    }

    class RepeatPattern : Pattern
    {
        internal RepeatPattern(int n, Pattern pp)
        {
            this.n = n;
            this.pp = pp;
        }
        readonly int n;
        readonly Pattern pp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            return Patterns.match_repeat(n, pp, underlying_text, starting_index, ending_index, 0);
        }
    }

    class ManyMinCharsPattern : Pattern
    {
        internal ManyMinCharsPattern(int min, CharPredicate cp)
        {
            this.min = min;
            this.cp = cp;
        }
        readonly int min;
        readonly CharPredicate cp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int minlen = Patterns.match_repeat(min, cp, underlying_text, starting_index, ending_index, 0);
            if (minlen == MISMATCH)
                return MISMATCH;
            return Patterns.match_many(cp, underlying_text, starting_index + minlen, ending_index, minlen);
        }
    }

    class ManyCharsPattern : Pattern
    {
        internal ManyCharsPattern(CharPredicate cp)
        {
            this.cp = cp;
        }
        readonly CharPredicate cp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            return Patterns.match_many(cp, underlying_text, starting_index, ending_index, 0);
        }
    }

    class ManyMinPattern : Pattern
    {
        public ManyMinPattern(int min, Pattern pp)
        {
            this.min = min;
            this.pp = pp;
        }
        readonly int min;
        readonly Pattern pp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int minlen = Patterns.match_repeat(min, pp, underlying_text, starting_index, ending_index, 0);
            if (MISMATCH == minlen)
                return MISMATCH;
            return Patterns.match_many(pp, underlying_text, starting_index + minlen, ending_index, minlen);
        }
    }

    class ManyPattern : Pattern
    {
        internal ManyPattern(Pattern pp)
        {
            this.pp = pp;
        }
        readonly Pattern pp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            return Patterns.match_many(pp, underlying_text, starting_index, ending_index, 0);
        }
    }

    class SomeMinCharsPattern : Pattern
    {
        public SomeMinCharsPattern(int min, CharPredicate cp, int max)
        {
            this.min = min;
            this.cp = cp;
            this.max = max;
        }
        readonly int min;
        readonly CharPredicate cp;
        readonly int max;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            //UPGRADE_NOTE: Final was removed from the declaration of 'minlen '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
            int minlen = Patterns.match_repeat(min, cp, underlying_text, starting_index, ending_index, 0);
            if (minlen == MISMATCH)
                return MISMATCH;
            return Patterns.match_some(max - min, cp, underlying_text, starting_index + minlen, ending_index, minlen);
        }
    }

    class SomeCharsPattern : Pattern
    {
        public SomeCharsPattern(int max, CharPredicate cp)
        {
            this.max = max;
            this.cp = cp;
        }
        readonly int max;
        readonly CharPredicate cp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            return Patterns.match_some(max, cp, underlying_text, starting_index, ending_index, 0);
        }
    }

    class SomeMinPattern : Pattern
    {
        public SomeMinPattern(int min, Pattern pp, int max)
        {
            this.min = min;
            this.pp = pp;
            this.max = max;
        }
        readonly int min;
        readonly Pattern pp;
        readonly int max;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int minlen = Patterns.match_repeat(min, pp, underlying_text, starting_index, ending_index, 0);
            if (MISMATCH == minlen)
                return MISMATCH;
            return Patterns.match_some(max - min, pp, underlying_text, starting_index + minlen, ending_index, minlen);
        }
    }

    class SomePattern : Pattern
    {
        public SomePattern(int max, Pattern pp)
        {
            this.max = max;
            this.pp = pp;
        }
        readonly int max;
        readonly Pattern pp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            return Patterns.match_some(max, pp, underlying_text, starting_index, ending_index, 0);
        }
    }

    class LongestPattern : Pattern
    {
        public LongestPattern(Pattern[] pps)
        {
            this.pps = pps;
        }
        readonly Pattern[] pps;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int r = MISMATCH;
            for (int i = 0; i < pps.Length; i++)
            {
                //UPGRADE_NOTE: Final was removed from the declaration of 'l '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
                int l = pps[i].Match(underlying_text, starting_index, ending_index);
                if (l > r)
                    r = l;
            }
            return r;
        }
    }

    class ShortestPattern : Pattern
    {
        public ShortestPattern(Pattern[] pps)
        {
            this.pps = pps;
        }
        readonly Pattern[] pps;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int r = MISMATCH;
            for (int i = 0; i < pps.Length; i++)
            {
                int l = pps[i].Match(underlying_text, starting_index, ending_index);
                if (l != MISMATCH)
                {
                    if (r == MISMATCH || l < r)
                        r = l;
                }
            }
            return r;
        }
    }

    class IfElsePattern : Pattern
    {
        public IfElsePattern(Pattern cond, Pattern yes, Pattern no)
        {
            this.cond = cond;
            this.no = no;
            this.yes = yes;
        }
        readonly Pattern cond;
        readonly Pattern yes;
        readonly Pattern no;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int lc = cond.Match(underlying_text, starting_index, ending_index);
            if (lc == MISMATCH)
            {
                return no.Match(underlying_text, starting_index, ending_index);
            }
            else
            {
                int ly = yes.Match(underlying_text, starting_index + lc, ending_index);
                if (ly == MISMATCH)
                    return MISMATCH;
                else
                    return lc + ly;
            }
        }
    }

    class OptionalPattern : Pattern
    {
        public OptionalPattern(Pattern pp)
        {
            this.pp = pp;
        }
        readonly Pattern pp;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int l = pp.Match(underlying_text, starting_index, ending_index);
            return (l == Pattern.MISMATCH) ? 0 : l;
        }
    }



    public class NeverPattern : Pattern
    {
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            return Pattern.MISMATCH;
        }
        NeverPattern() { }
        internal static readonly Pattern instance = new NeverPattern();
    }

    public class AlwaysPattern : Pattern
    {
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            return 0;
        }
        AlwaysPattern() { }
        internal static readonly Pattern instance = new AlwaysPattern();
    }

    class AlternativePattern : Pattern
    {
        public AlternativePattern(Pattern[] pps)
        {
            this.pps = pps;
        }
        readonly Pattern[] pps;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            for (int i = 0; i < pps.Length; i++)
            {
                //UPGRADE_NOTE: Final was removed from the declaration of 'l '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
                int l = pps[i].Match(underlying_text, starting_index, ending_index);
                if (l != Pattern.MISMATCH)
                    return l;
            }
            return Pattern.MISMATCH;
        }
    }

    class SequentialPattern : Pattern
    {
        public SequentialPattern(Pattern[] pps)
        {
            this.pps = pps;
        }
        readonly Pattern[] pps;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int cur = starting_index;
            for (int i = 0; i < pps.Length; i++)
            {
                int l = pps[i].Match(underlying_text, cur, ending_index);
                if (l == Pattern.MISMATCH)
                    return l;
                cur += l;
            }
            return cur - starting_index;
        }
    }

    class AllPattern : Pattern
    {
        public AllPattern(Pattern[] pps)
        {
            this.pps = pps;
        }
        readonly Pattern[] pps;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            int ret = 0;
            for (int i = 0; i < pps.Length; i++)
            {
                int l = pps[i].Match(underlying_text, starting_index, ending_index);
                if (l == MISMATCH)
                    return MISMATCH;
                if (l > ret)
                    ret = l;
            }
            return ret;
        }
    }
    class RegexPattern : Pattern
    {
        public RegexPattern(Regex p)
        {
            this.p = p;
        }
        readonly Regex p;
        public override int Match(string underlying_text, int starting_index, int ending_index)
        {
            if (starting_index > ending_index)
                return Pattern.MISMATCH;
            Match mresult = p.Match(underlying_text, starting_index, ending_index - starting_index);
            if (mresult.Success)
            {
                return mresult.Length;
            }
            else
                return Pattern.MISMATCH;
        }
    }
}
