using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    [TestFixture()]
    public class StringNormalizerTest
    {
        [Test()]
        public void NumberTest()
        {
            Assert.AreEqual(
                "25",
                StringNormalizer.NormalizeText(
                    "二十五", NormalizeTextOptions.Number | NormalizeTextOptions.KanjiDigit));
            Assert.AreEqual(
                "34000612",
                StringNormalizer.NormalizeText(
                    "三千四百万六百十二", NormalizeTextOptions.Number | NormalizeTextOptions.KanjiDigit));
            Assert.AreEqual(
                "34124612",
                StringNormalizer.NormalizeText(
                    "三千四百十2万四千六百十二", NormalizeTextOptions.Number | NormalizeTextOptions.KanjiDigit));
            Assert.AreEqual(
                "1209",
                StringNormalizer.NormalizeText(
                    "1209", NormalizeTextOptions.Number | NormalizeTextOptions.KanjiDigit));
            Assert.AreEqual(
                "21000005",
                StringNormalizer.NormalizeText(
                    "②千百万Ⅴ", NormalizeTextOptions.Number | NormalizeTextOptions.KanjiDigit));
            Assert.AreEqual(
                "1000000051100",
                StringNormalizer.NormalizeText(
                    "億５１０千１００", NormalizeTextOptions.Number | NormalizeTextOptions.KanjiDigit));

            //StringNormalizer.NormalizeText("1209ｨぅェガＡＢＣｄｅｆdefAA十二");
        }
    }
}
