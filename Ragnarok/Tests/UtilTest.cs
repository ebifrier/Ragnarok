#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Tests
{
    [TestFixture()]
    public sealed class UtilTest
    {
        private readonly string[] FileContents = new string[]
        {
            "test",
            "01 鈴木",
            "02 田中"
        };

        private void MakeFile(string filepath)
        {
            using (var stream = new FileStream(filepath, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                FileContents.ForEach(_ => writer.WriteLine(_));
            }
        }

        [Test()]
        public void ReadLineTest()
        {
            var filepath = Util.GetTempFileName();
            MakeFile(filepath);

            try
            {
                using (var stream = new FileStream(filepath, FileMode.Open))
                {
                    var lines = Util.ReadLines(stream, Encoding.UTF8);
                    var en = lines.GetEnumerator();

                    foreach (var expected in FileContents)
                    {
                        Assert.IsTrue(en.MoveNext());
                        Assert.AreEqual(expected, en.Current);
                    }
                    Assert.IsFalse(en.MoveNext());
                }
            }
            finally
            {
                File.Delete(filepath);
            }
        }
    }
}
#endif
