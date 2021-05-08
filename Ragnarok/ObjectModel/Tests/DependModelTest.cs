#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.ObjectModel.Tests
{
    // メンバー Xxx はインスタンス データにアクセスしないため、static (Visual Basic では Shared) に設定できます
    #pragma warning disable CA1822

    internal class AggregateClass : PropertyChangedCounter
    {
        private BaseModel model;

        public BaseModel Model
        {
            get { return this.model; }
        }

        [DependOn(typeof(BaseModel))]
        public int BaseProperty1
        {
            get { return 1; }
        }

        [DependOn(typeof(BaseModel))]
        public int BaseProperty2
        {
            get { return 2; }
        }

        [DependOn(typeof(BaseModel))]
        public int InheritProperty1
        {
            get { return 3; }
        }

        [DependOn(typeof(BaseModel))]
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
    public sealed class DependModelTest
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
