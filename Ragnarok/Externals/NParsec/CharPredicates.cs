using CharPredicate = Codehaus.Parsec.Predicate<char>;
namespace Codehaus.Parsec
{
    /// <summary> This class provides some common CharPredicate implementations.</summary>
    /// <author>  Ben Yu
    /// 
    /// Dec 11, 2004
    /// </author>
    public sealed class CharPredicates
    {
        /// <summary> == a.</summary>
        public static CharPredicate IsChar(char a)
        {
            return delegate(char c)
            {
                return a == c;
            };
        }
        /// <summary> != a.</summary>
        public static CharPredicate NotChar(char a)
        {
            return delegate(char c)
            {
                return a != c;
            };
        }
        /// <summary> between a and b inclusive.</summary>
        public static CharPredicate InRange(char a, char b)
        {
            return delegate(char c)
            {
                return c >= a && c <= b;
            };
        }
        /// <summary> between 0 and 9.</summary>
        public static CharPredicate IsDigit()
        {
            return InRange('0', '9');
        }
        /// <summary> not between a and b inclusive.</summary>
        public static CharPredicate NotInRange(char a, char b)
        {
            return delegate(char c)
            {
                return c > b || c < a;
            };
        }
        /// <summary> among chars.</summary>
        public static CharPredicate Among(params char[] chars)
        {
            return delegate(char c)
            {
                return charAmong(c, chars);
            };
        }
        /// <summary> not among chars.</summary>
        public static CharPredicate NotAmong(params char[] chars)
        {
            return delegate(char c)
            {
                return !charAmong(c, chars);
            };
        }
        internal static bool charAmong(char c, char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (c == chars[i])
                    return true;
            }
            return false;
        }
        /// <summary> is hex digit.</summary>
        public static CharPredicate IsHexDigit()
        {
            return delegate(char c)
            {
                return c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';
            };
        }
        /// <summary> [A-Z].</summary>
        public static CharPredicate IsUppercase()
        {
            return delegate(char c)
            {
                return char.IsUpper(c);
            };
        }
        /// <summary> [a-z].</summary>
        public static CharPredicate IsLowercase()
        {
            return delegate(char c)
            {
                return char.IsLower(c);
            };
        }
        /// <summary> is white space.</summary>
        public static CharPredicate IsWhitespace()
        {
            return delegate(char c)
            {
                return char.IsWhiteSpace(c);
            };
        }
        /// <summary> [a-zA-Z].</summary>
        public static CharPredicate IsAlpha()
        {
            return delegate(char c)
            {
                return c <= 'z' && c >= 'a' || c <= 'Z' && c >= 'A';
            };
        }
        /// <summary> [a-zA-Z_].</summary>
        public static CharPredicate IsAlpha_()
        {
            return delegate(char c)
            {
                return c == '_' || c <= 'z' && c >= 'a' || c <= 'Z' && c >= 'A';
            };
        }
        /// <summary> is letter.</summary>
        public static CharPredicate IsLetter()
        {
            return delegate(char c)
            {
                return char.IsLetter(c);
            };
        }
        /// <summary> [a-zA-Z0-9_]</summary>
        public static CharPredicate IsAlphaNumeric()
        {
            return delegate(char c)
            {
                return c == '_' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9';
            };
        }
        /// <summary> Negate a CharPredicate object.</summary>
        public static CharPredicate Not(CharPredicate cp)
        {
            return delegate(char c)
            {
                return !cp(c);
            };
        }
        /// <summary> Logical and of two CharPredicate objects.</summary>
        public static CharPredicate And(CharPredicate cp1, CharPredicate cp2)
        {
            return delegate(char c)
            {
                return cp1(c) && cp2(c);
            };
        }
        /// <summary> Logical or of two CharPredicate objects.</summary>
        public static CharPredicate Or(CharPredicate cp1, CharPredicate cp2)
        {
            return delegate(char c)
            {
                return cp1(c) || cp2(c);
            };
        }

        /// <summary> Logical and of an array of CharPredicate objects.</summary>
        public static CharPredicate And(params CharPredicate[] cps)
        {
            if (cps.Length == 0)
                return Always();
            else if (cps.Length == 1)
                return cps[0];
            return delegate(char c)
            {
                foreach (CharPredicate cp in cps)
                {
                    if (!cp(c))
                        return false;
                }
                return true;
            };
        }
        /// <summary> Logical or of an array of CharPredicate objects.</summary>
        public static CharPredicate Or(params CharPredicate[] cps)
        {
            if (cps.Length == 0)
                return Never();
            else if (cps.Length == 1)
                return cps[0];
            return delegate(char c)
            {
                foreach (CharPredicate cp in cps)
                {
                    if (cp(c))
                        return true;
                }
                return false;
            };
        }
        /// <summary> A predicate that always returns false.</summary>
        public static CharPredicate Never()
        {
            return _never;
        }
        /// <summary> A predicate that always returns true.</summary>
        public static CharPredicate Always()
        {
            return _always;
        }
        private static readonly CharPredicate _never = delegate(char c) { return false; };
        private static readonly CharPredicate _always = delegate(char c) { return true; };
    }
}