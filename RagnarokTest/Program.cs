using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

using Ragnarok;
using Ragnarok.Utility;

namespace RagnarokTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new ModelBaseTest();

            test.AffectOtherPropertyTest();
            test.DependOnPropertyTest();
            test.ViewModelTest();
            test.LazyModelTest();

            var t = new Utility.TypeSerializerTest();
            t.SimpleTest();
            t.DeserializeTest();

            var t3 = new Utility.ScannerTest();
            t3.Test1();
            t3.Test2();

            var ntpTest = new Net.NtpClientTest();
            ntpTest.Test();

            var calcTest = new Utility.CalclatorTest();
            calcTest.SimpleTest();
            calcTest.FuncTest();

            StringNormalizer.NormalizeText("二十五", NormalizeTextOption.All);
            StringNormalizer.NormalizeText("三千四百万六百十二", NormalizeTextOption.All);
            StringNormalizer.NormalizeText("三千四百十2万四千六百十二", NormalizeTextOption.All);
            StringNormalizer.NormalizeText("1209", NormalizeTextOption.All);
            // 三千四百万六百十二 → 34000612

            StringNormalizer.NormalizeText("1209ｨぅェガＡＢＣｄｅｆdefAA十二");
        }
    }
}
