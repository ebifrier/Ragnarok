#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

#pragma warning disable 219

namespace Ragnarok.Utility.Tests
{
    internal class CacheObject : ICachable
    {
        private long size;

        public CacheObject(long size)
        {
            this.size = size;
        }

        public virtual long ObjectSize
        {
            get { return this.size; }
        }
    }

    internal sealed class RandomCacheObject : CacheObject
    {
        public RandomCacheObject()
            : base(0)
        {
        }

        public override long ObjectSize
        {
            get { return MathEx.RandInt(1, 10000); }
        }
    }

    internal sealed class ExceptionObject : ICachable
    {
        public ExceptionObject()
        {
            throw new InvalidOperationException();
        }

        public long ObjectSize
        {
            get { return 1; }
        }
    }

    [TestFixture()]
    public sealed class CacheManagerTest
    {
        internal CacheObject CreateObject(int key)
        {
            if (key == int.MaxValue)
            {
                return new RandomCacheObject();
            }

            return new CacheObject(key);
        }

        [Test()]
        public void NormalTest()
        {
            var manager = new CacheCollection<int, CacheObject>(CreateObject, 1000);

            var obj1 = manager.GetOrCreate(100);
            Assert.AreEqual(100, obj1.ObjectSize);
            Assert.AreEqual(1, manager.Count);
            Assert.AreEqual(100, manager.CacheSize);

            var obj1_2 = manager.GetOrCreate(100);
            Assert.AreEqual(obj1, obj1_2);
            Assert.AreEqual(1, manager.Count);
            Assert.AreEqual(100, manager.CacheSize);
            
            // size=500のオブジェクトは追加でキャッシュされるはず。
            var obj5 = manager.GetOrCreate(500);
            Assert.AreEqual(2, manager.Count);
            Assert.AreEqual(600, manager.CacheSize);

            // size=600のオブジェクトはsize100と500をまとめて削除するはず。
            var obj6 = manager.GetOrCreate(600);
            Assert.AreEqual(1, manager.Count);
            Assert.AreEqual(600, manager.CacheSize);

            // もう一回size=100のオブジェクトを作ると、新規作成されるはず。
            var obj1_3 = manager.GetOrCreate(100);
            Assert.AreEqual(2, manager.Count);
            Assert.AreEqual(700, manager.CacheSize);
            Assert.AreNotEqual(obj1, obj1_3);

            // もう一回size=500のオブジェクトを作ると、新規作成されるはず。
            var obj5_2 = manager.GetOrCreate(500);
            Assert.AreEqual(2, manager.Count);
            Assert.AreEqual(600, manager.CacheSize);
            Assert.AreNotEqual(obj5, obj5_2);
        }

        [Test()]
        public void RandomTest()
        {
            var manager = new CacheCollection<int, CacheObject>(CreateObject, 100);

            // ObjectSizeが乱数を返すようなオブジェクトを登録する。
            var robj = manager.GetOrCreate(int.MaxValue);
            Assert.AreEqual(typeof(RandomCacheObject), robj.GetType());
            Assert.AreEqual(1, manager.Count);

            // size=200を登録すると、他の全てのオブジェクトが削除されるため、
            // この状態でCacheSizeが元に戻るか確認する。
            var obj2 = manager.GetOrCreate(200);
            Assert.AreEqual(1, manager.Count);
            Assert.AreEqual(200, manager.CacheSize);
        }

        [Test()]
        public void MinusTest()
        {
            var manager = new CacheCollection<int, CacheObject>(CreateObject, 100);

            // ObjectSizeが負数になるようなオブジェクトを作成した場合
            Assert.Catch(() => manager.GetOrCreate(-100));
        }

        internal ExceptionObject CreateExceptionObject(int key)
        {
            return new ExceptionObject();
        }

        [Test()]
        public void ExceptionTest()
        {
            var manager = new CacheCollection<int, ExceptionObject>(CreateExceptionObject, 100);

            Assert.Catch(typeof(InvalidOperationException), () => manager.GetOrCreate(100));
        }
    }
}
#endif
