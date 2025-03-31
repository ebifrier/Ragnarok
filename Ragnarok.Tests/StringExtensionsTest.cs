using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.Tests
{
    [TestFixture]
    public sealed class StringExtensionsTest
    {
        [Test]
        public void SafeSubstrTest()
        {
            Assert.AreEqual("ab", "abcd".SafeSubstr(..2));
            Assert.AreEqual("abcd", "abcd".SafeSubstr(..4));
            Assert.AreEqual("abcd", "abcd".SafeSubstr(..16));

            Assert.AreEqual("bc", "abcd".SafeSubstr(1..3));
            Assert.AreEqual("", "abcd".SafeSubstr(3..3));
            Assert.AreEqual("", "abcd".SafeSubstr(16..3));

            Assert.AreEqual("bcd", "abcd".SafeSubstr(^3..));
            Assert.AreEqual("bc", "abcd".SafeSubstr(^3..^1));
            Assert.AreEqual("", "abcd".SafeSubstr(^1..^3));

            Assert.Catch(() => ((string?)null).SafeSubstr(..));
        }

        [Test]
        public void HankakuLengthTest()
        {
            Assert.AreEqual(0, "".HankakuLength());
            Assert.AreEqual(4, "abcd".HankakuLength());
            Assert.AreEqual(8, "ハンカク".HankakuLength());
            Assert.AreEqual(13, "y漢字+日本語x".HankakuLength());

            Assert.AreEqual(0, ((string?)null).HankakuLength());
        }

        [Test]
        public void HankakuSubstringTest()
        {
            Assert.AreEqual("ハ", "ハンカク".HankakuSubstring(2));
            Assert.AreEqual("ハ", "ハンカク".HankakuSubstring(3));
            Assert.AreEqual("ハン", "ハンカク".HankakuSubstring(4));

            Assert.AreEqual("aテ", "aテbスcト".HankakuSubstring(3));
            Assert.AreEqual("aテb", "aテbスcト".HankakuSubstring(4));
            Assert.AreEqual("aてb", "aてbすcと".HankakuSubstring(5));
            Assert.AreEqual("aてbす", "aてbすcと".HankakuSubstring(6));

            Assert.AreEqual(null, ((string?)null).HankakuSubstring(1));
        }

        [Test]
        public void IsWhiteSpaceExTest()
        {
            Assert.True(' '.IsWhiteSpaceEx());
            Assert.True('　'.IsWhiteSpaceEx());
            Assert.True('\n'.IsWhiteSpaceEx());
            Assert.True('\t'.IsWhiteSpaceEx());
            Assert.True('\r'.IsWhiteSpaceEx());
            Assert.True('\u200c'.IsWhiteSpaceEx());
            Assert.True('\u200e'.IsWhiteSpaceEx());

            Assert.False('a'.IsWhiteSpaceEx());
            Assert.False('ひ'.IsWhiteSpaceEx());
            Assert.False('漢'.IsWhiteSpaceEx());
            Assert.False('\0'.IsWhiteSpaceEx());
        }

        [Test]
        public void IsWhiteSpaceOnlyTest()
        {
            Assert.True("".IsWhiteSpaceOnly());
            Assert.True(" ".IsWhiteSpaceOnly());
            Assert.True("　\r\n\t".IsWhiteSpaceOnly());

            Assert.False("a\r\n\t".IsWhiteSpaceOnly());

            Assert.True(((string?)null).IsWhiteSpaceOnly());
        }

        [Test]
        public void RemoveWhitespaceTest()
        {
            Assert.AreEqual("", "".RemoveWhitespace());
            Assert.AreEqual("abc", "abc".RemoveWhitespace());
            Assert.AreEqual("abc", " a b　c ".RemoveWhitespace());
            Assert.AreEqual("卍怪盗キッド→", "卍　怪 盗\nキッド→".RemoveWhitespace());

            Assert.AreEqual(null, ((string?)null).RemoveWhitespace());
        }

        [Test]
        public void QuoteTest()
        {
            Assert.AreEqual("'x'", "x".Quote("'"));
            Assert.AreEqual("｜鬼太郎｜", "鬼太郎".Quote("｜"));

            Assert.AreEqual("''", "".Quote("'"));
            Assert.AreEqual("''", ((string?)null).Quote("'"));
        }

        [Test]
        public void RemoveQuoteTest()
        {
            Assert.AreEqual("x", "'x'".RemoveQuote('\''));
            Assert.AreEqual("xyz", ",xyz".RemoveQuote(','));
            Assert.AreEqual("鬼太郎", "｜鬼太郎｜".RemoveQuote('｜'));
            Assert.AreEqual("鬼太郎", "鬼太郎｜".RemoveQuote('｜'));
            Assert.AreEqual("TEST", "TEST".RemoveQuote('\''));

            Assert.AreEqual("x", "\"x\"".RemoveQuote());
            Assert.AreEqual("x", " x ".RemoveQuote(' '));
            Assert.AreEqual("x", ",x,".RemoveQuote(' ', ','));

            Assert.AreEqual("", "''".RemoveQuote());
            Assert.AreEqual(null, ((string?)null).RemoveQuote());
        }
    }
}
