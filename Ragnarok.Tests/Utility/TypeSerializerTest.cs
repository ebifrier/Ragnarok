using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture()]
    public class TypeSerializerTest
    {
        /// <summary>
        /// テスト用クラス
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1812:インスタンス化されていない内部クラスを使用しないでください")]
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
        private static void TypeTest(Type? type)
        {
            var selialized = TypeSerializer.Serialize(type);

            Assert.AreEqual(type, TypeSerializer.Deserialize(selialized));
        }

        [Test()]
        public void SimpleTest()
        {
            TypeTest(typeof(int));
            TypeTest(typeof(Encoding));
            TypeTest(typeof(Action<>));
            TypeTest(typeof(Action<int, List<Encoding>>));
            TypeTest(typeof(InnerClass));
            TypeTest(typeof(Assert));

            Assert.Catch(() => TypeTest(null));
        }

        [Test()]
        public void DeserializeTest()
        {
            Assert.AreEqual(typeof(int), TypeSerializer.Deserialize("System.Int32"));
            Assert.AreEqual(typeof(Action<>), TypeSerializer.Deserialize("System.Action`1"));
            Assert.AreEqual(typeof(Action<>), TypeSerializer.Deserialize("System.Action`1[]"));
            Assert.AreEqual(
                typeof(Action<Action<double, Assert>, int>),
                TypeSerializer.Deserialize(
                    "System.Action`2[System.Action`2[System.Double, NUnit.Framework.Assert], System.Int32]"));

            Assert.Catch(() => TypeSerializer.Deserialize("System.Int32`1"));
            Assert.Catch(() => TypeSerializer.Deserialize("int"));
            Assert.Catch(() => TypeSerializer.Deserialize("System.Action`2[System.Int32]"));
            Assert.Catch(() => TypeSerializer.Deserialize("System.Action`1[[]]"));
        }
    }
}
