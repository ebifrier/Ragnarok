#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NUnit.Framework;

namespace Ragnarok.ObjectModel.Tests
{
    internal class BaseModel : PropertyChangedCounter
    {
        public int BaseProperty1
        {
            get { return 1; }
            set { this.RaisePropertyChanged("BaseProperty1"); }
        }

        [DependOnProperty("BaseProperty1")]
        public int BaseProperty2
        {
            get { return 2; }
            set { this.RaisePropertyChanged("BaseProperty2"); }
        }

        [DependOnProperty("BaseProperty1")]
        public int InheritProperty1
        {
            get { return 3; }
            set { this.RaisePropertyChanged("InheritProperty1"); }
        }

        [DependOnProperty("BaseProperty2")]
        public int InheritProperty2
        {
            get { return 4; }
            set { this.RaisePropertyChanged("InheritProperty2"); }
        }

        public void Validate(int baseCount1, int baseCount2,
                             int inheritCount1, int inheritCount2)
        {
            Assert.AreEqual(baseCount1, this["BaseProperty1"]);
            Assert.AreEqual(baseCount2, this["BaseProperty2"]);
            Assert.AreEqual(inheritCount1, this["InheritProperty1"]);
            Assert.AreEqual(inheritCount2, this["InheritProperty2"]);
        }
    }

    internal class DerivedModel : BaseModel
    {
        [DependOnProperty(typeof(BaseModel), "InheritProperty1")]
        public int DerivedProperty1
        {
            get { return 10; }
            set { this.RaisePropertyChanged("DerivedProperty1"); }
        }

        public void Validate(int baseCount1, int baseCount2,
                             int inheritCount1, int inheritCount2,
                             int derivedCount)
        {
            base.Validate(baseCount1, baseCount2, inheritCount1, inheritCount2);

            Assert.AreEqual(derivedCount, this["DerivedProperty1"]);
        }
    }

    [TestFixture()]
    internal class NotifyObjectTest
    {
        [Test()]
        public void DependOnPropertyTest()
        {
            var obj = new BaseModel();

            obj.RaisePropertyChanged("Nothing");
            obj.Validate(0, 0, 0, 0);

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
        public void DependOnPropertyTest2()
        {
            var obj = new DerivedModel();

            obj.InheritProperty1 = 0;
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
    }
}
#endif
