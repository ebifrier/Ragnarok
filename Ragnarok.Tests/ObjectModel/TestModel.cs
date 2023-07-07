using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.ObjectModel.Tests
{
    internal class BaseModel : PropertyChangedCounter
    {
        public int BaseProperty1
        {
            get { return GetValue<int>(nameof(BaseProperty1)); }
            set { SetValue(nameof(BaseProperty1), value); }
        }

        [DependOn(nameof(BaseProperty1))]
        public int BaseProperty2
        {
            get { return GetValue<int>(nameof(BaseProperty2)); }
            set { SetValue(nameof(BaseProperty2), value); }
        }

        [DependOn(nameof(BaseProperty1))]
        public int InheritProperty1
        {
            get { return GetValue<int>(nameof(InheritProperty1)); }
            set { SetValue(nameof(InheritProperty1), value); }
        }

        [DependOn(nameof(BaseProperty2))]
        public int InheritProperty2
        {
            get { return GetValue<int>(nameof(InheritProperty2)); }
            set { SetValue(nameof(InheritProperty2), value); }
        }

        public void Validate(int baseCount1, int baseCount2,
                             int inheritCount1, int inheritCount2)
        {
            Assert.AreEqual(baseCount1, this[nameof(BaseProperty1)]);
            Assert.AreEqual(baseCount2, this[nameof(BaseProperty2)]);
            Assert.AreEqual(inheritCount1, this[nameof(InheritProperty1)]);
            Assert.AreEqual(inheritCount2, this[nameof(InheritProperty2)]);
        }
    }

    internal class DerivedModel : BaseModel
    {
        [DependOn(typeof(BaseModel), nameof(InheritProperty1))]
        public int DerivedProperty1
        {
            get { return GetValue<int>(nameof(DerivedProperty1)); }
            set { SetValue(nameof(DerivedProperty1), value); }
        }

        public void Validate(int baseCount1, int baseCount2,
                             int inheritCount1, int inheritCount2,
                             int derivedCount)
        {
            base.Validate(baseCount1, baseCount2, inheritCount1, inheritCount2);

            Assert.AreEqual(derivedCount, this[nameof(DerivedProperty1)]);
        }
    }
}
