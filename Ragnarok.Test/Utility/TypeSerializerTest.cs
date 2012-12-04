using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok;
using Ragnarok.Utility;

namespace Ragnarok.Test.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeSerializerTest
    {
        public class InnerClass
        {
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
            TypeTest(typeof(global::NUnit.Framework.Assert));

            Assert.Catch(() => TypeTest(null));
        }

        [Test()]
        public void DeserializeTest()
        {
            Assert.AreEqual(typeof(int), TypeSerializer.Deserialize("System.Int32"));
            Assert.AreEqual(typeof(Tuple<>), TypeSerializer.Deserialize("System.Tuple`1"));
            Assert.AreEqual(typeof(Tuple<>), TypeSerializer.Deserialize("System.Tuple`1[]"));

            Assert.Catch(() => TypeSerializer.Deserialize("System.Int32`1"));
            Assert.Catch(() => TypeSerializer.Deserialize("int"));
            Assert.Catch(() => TypeSerializer.Deserialize("System.Tuple`2[System.Int32]"));
            Assert.Catch(() => TypeSerializer.Deserialize("System.Tuple`1[[]]"));
        }
    }
}
