#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture()]
    internal class TypeSerializerTest
    {
        /// <summary>
        /// テスト用クラス
        /// </summary>
        private class InnerClass
        {
        }

        [Test()]
        public void RegexTest()
        {
            /*var re = new Regex(
                @"\G\s*(?:(?:[\w+.:]+(?:`\d+)?)|(\])|(\[)|(,))\s*",
                RegexOptions.Compiled);

            var mc = re.Matches(@"System.Tuple`2[System.Int32]");
            Assert.AreEqual(mc, null);*/
        }

        /// <summary>
        /// シリアライズandデシリアライズで同じ型に戻るか調べます。
        /// </summary>
        private void TypeTest(Type type)
        {
            var selialized = TypeSerializer.Serialize(type);

            Assert.AreEqual(type, TypeSerializer.Deserialize(selialized));
        }

        [Test()]
        public void SimpleTest()
        {
            TypeTest(typeof(int));
            TypeTest(typeof(Encoding));
            TypeTest(typeof(Tuple<>));
            TypeTest(typeof(Tuple<int, List<Encoding>>));
            TypeTest(typeof(InnerClass));
            TypeTest(typeof(Assert));

            Assert.Catch(() => TypeTest(null));
        }

        [Test()]
        public void DeserializeTest()
        {
            Assert.AreEqual(typeof(int), TypeSerializer.Deserialize("System.Int32"));
            Assert.AreEqual(typeof(Tuple<>), TypeSerializer.Deserialize("System.Tuple`1"));
            Assert.AreEqual(typeof(Tuple<>), TypeSerializer.Deserialize("System.Tuple`1[]"));
            Assert.AreEqual(
                typeof(Tuple<Tuple<double, Assert>, int>),
                TypeSerializer.Deserialize(
                    "System.Tuple`2[System.Tuple`2[System.Double, NUnit.Framework.Assert], System.Int32]"));

            Assert.Catch(() => TypeSerializer.Deserialize("System.Int32`1"));
            Assert.Catch(() => TypeSerializer.Deserialize("int"));
            Assert.Catch(() => TypeSerializer.Deserialize("System.Tuple`2[System.Int32]"));
            Assert.Catch(() => TypeSerializer.Deserialize("System.Tuple`1[[]]"));
        }
    }
}
#endif
