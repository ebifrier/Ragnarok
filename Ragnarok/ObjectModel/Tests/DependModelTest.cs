#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NUnit.Framework;

namespace Ragnarok.ObjectModel.Tests
{
    internal class AggregateClass : PropertyChangedCounter
    {
        private BaseModel model;

        public BaseModel Model
        {
            get { return this.model; }
        }

        [DependOnProperty(typeof(BaseModel))]
        public int BaseProperty1
        {
            get { return 1; }
        }

        [DependOnProperty(typeof(BaseModel))]
        public int BaseProperty2
        {
            get { return 2; }
        }

        [DependOnProperty(typeof(BaseModel))]
        public int InheritProperty1
        {
            get { return 3; }
        }

        [DependOnProperty(typeof(BaseModel))]
        public int InheritProperty2
        {
            get { return 4; }
        }

        public void Validate(int baseCount1, int baseCount2,
                             int inheritCount1, int inheritCount2)
        {
            Assert.AreEqual(baseCount1, this["BaseProperty1"]);
            Assert.AreEqual(baseCount2, this["BaseProperty2"]);
            Assert.AreEqual(inheritCount1, this["InheritProperty1"]);
            Assert.AreEqual(inheritCount2, this["InheritProperty2"]);
        }

        public AggregateClass(BaseModel model)
        {
            this.model = model;

            this.AddDependModel(this.model);
        }
    }

    [TestFixture()]
    internal class DependModelTest
    {
        [Test()]
        public void BaseTest()
        {
            var obj = new AggregateClass(new BaseModel());
            obj.Validate(1, 1, 1, 1);

            obj.ClearCount();
            obj.RaisePropertyChanged("BaseProperty1");
            obj.Validate(1, 0, 0, 0);

            obj.ClearCount();
            obj.Model.RaisePropertyChanged("BaseProperty1");
            obj.Validate(1, 1, 1, 1);

            obj.Model.RaisePropertyChanged("BaseProperty2");
            obj.Validate(1, 2, 1, 2);
        }

        [Test()]
        public void DerivedTest()
        {
            var obj = new AggregateClass(new DerivedModel());
            obj.ClearCount();

            obj.RaisePropertyChanged("BaseProperty1");
            obj.Validate(1, 0, 0, 0);

            obj.ClearCount();
            obj.Model.RaisePropertyChanged("BaseProperty1");
            obj.Validate(1, 1, 1, 1);

            obj.Model.RaisePropertyChanged("BaseProperty2");
            obj.Validate(1, 2, 1, 2);
        }
    }
}
#endif
