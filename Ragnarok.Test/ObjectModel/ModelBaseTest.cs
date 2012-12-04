using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NUnit.Framework;

using Ragnarok;
using Ragnarok.ObjectModel;

namespace Ragnarok.Test.ObjectModel
{
    [TestFixture()]
    public class ModelBaseTest
    {
        [Test()]
        public void AffectOtherPropertyTest()
        {
            var obj = new BaseModel();

            obj.RaisePropertyChanged("BaseProperty1");
            obj.RaisePropertyChanged("BaseProperty2");
        }

        [Test()]
        public void DependOnPropertyTest()
        {
            var obj = new AggregateClass(new BaseModel());

            obj.RaisePropertyChanged("BaseProperty1");
            obj.Model.RaisePropertyChanged("BaseProperty1");
            obj.Model.RaisePropertyChanged("BaseProperty2");

            var derived = new DerivedClass();
            derived.RaisePropertyChanged("BaseProperty1");
        }

        public void ViewModelTest()
        {
            dynamic obj = new SimpleViewModel();

            Console.WriteLine();
            obj.AddDependModel(new SimpleClass());
            Console.WriteLine();
            obj.SimpleBaseProperty1 = 0;

            Console.WriteLine();
            obj.AddDependModel(new BaseModel());
            Console.WriteLine();

            obj.BaseProperty1 = 0;
        }

        public void LazyModelTest()
        {
            var obj = new BaseModel();

            using (new LazyModelLock(obj))
            {
                obj.RaisePropertyChanged("BaseProperty1");
            }
        }
    }
}
