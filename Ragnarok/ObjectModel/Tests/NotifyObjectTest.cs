#if TESTS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.ObjectModel.Tests
{
    [TestFixture()]
    public class NotifyObjectTest
    {
        [Test()]
        public void DependOnPropertyBaseTest()
        {
            var obj = new BaseModel();

            obj.RaisePropertyChanged("Nothing");
            obj.Validate(0, 0, 0, 0);

            obj.BaseProperty1 = 0;
            obj.Validate(1, 1, 1, 1);

            obj.BaseProperty1 = 0;
            obj.Validate(1, 1, 1, 1);

            obj.BaseProperty2 = 0;
            obj.Validate(1, 2, 1, 2);

            obj.InheritProperty1 = 0;
            obj.Validate(1, 2, 2, 2);

            obj.InheritProperty2 = 0;
            obj.Validate(1, 2, 2, 3);
        }

        [Test()]
        public void DependOnPropertyDerivedTest()
        {
            var obj = new DerivedModel
            {
                InheritProperty1 = 0
            };
            obj.Validate(0, 0, 1, 0, 1);
        }

        [Test()]
        public void AddPropertyChangedTest()
        {
            var obj = new BaseModel();
            var count = 0;

            var handler = new PropertyChangedEventHandler(
                (_, __) => count += 1);

            obj.AddPropertyChangedHandler("InheritProperty1", handler);
            obj.BaseProperty1 = 0;
            Assert.AreEqual(1, count);

            obj.RemovePropertyChangedHandler("InheritProperty1", handler);
            obj.BaseProperty1 = 0;
            Assert.AreEqual(1, count);
        }

        [Test()]
        public void RemoveTest()
        {
            var obj = new BaseModel
            {
                BaseProperty1 = 1
            };
            obj.Validate(1, 1, 1, 1);

            Assert.True(obj.Remove("BaseProperty1"));
            obj.Validate(2, 2, 2, 2);

            Assert.False(obj.Remove("BaseProperty1"));
            obj.Validate(2, 2, 2, 2);

            Assert.False(obj.Remove(""));
            obj.Validate(2, 2, 2, 2);

            Assert.False(obj.Remove("InheritProperty1"));
            obj.Validate(2, 2, 2, 2);

            Assert.AreEqual(0, obj.Count);
        }
    }
}
#endif
