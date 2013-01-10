#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NUnit.Framework;

namespace Ragnarok.ObjectModel.Tests
{
    internal class SimpleClass
    {
        public int SimpleProperty1
        {
            get { return 1; }
            set { }
        }

        public int SimpleProperty2
        {
            get { return 2; }
            set { }
        }
    }

    internal class SimpleViewModel : DynamicViewModel
    {
        private readonly Dictionary<string, int> countDic =
            new Dictionary<string, int>();

        /// <summary>
        /// 各プロパティの変更通知が何回呼ばれたか取得します。
        /// </summary>
        public int this[string key]
        {
            get
            {
                var count = 0;
                if (this.countDic.TryGetValue(key, out count))
                {
                    return count;
                }

                return 0;
            }
        }

        public void ClearCount()
        {
            this.countDic.Clear();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var count = 0;
            if (this.countDic.TryGetValue(e.PropertyName, out count))
            {
                this.countDic[e.PropertyName] = count + 1;
            }
            else
            {
                this.countDic[e.PropertyName] = 1;
            }
        }

        [DependOnProperty(typeof(SimpleClass), "SimpleProperty1")]
        public int InheritProperty1
        {
            get { return 1; }
        }

        [DependOnProperty(typeof(BaseModel), "BaseProperty1")]
        public int InheritProperty2
        {
            get { return 1; }
        }

        public void Validate(int simpleCount1, int simpleCount2,
                             int inheritCount1, int inheritCount2)
        {
            Assert.AreEqual(simpleCount1, this["SimpleProperty1"]);
            Assert.AreEqual(simpleCount2, this["SimpleProperty2"]);
            Assert.AreEqual(inheritCount1, this["InheritProperty1"]);
            Assert.AreEqual(inheritCount2, this["InheritProperty2"]);
        }
    }

    [TestFixture()]
    internal class ViewModelTest
    {
        [Test()]
        public void BasicTest()
        {
            dynamic obj = new SimpleViewModel();

            obj.AddDependModel(new SimpleClass());
            obj.Validate(1, 1, 1, 0);

            obj.ClearCount();
            obj.SimpleProperty1 = 0;
            obj.Validate(1, 0, 1, 0);

            obj.ClearCount();
            obj.AddDependModel(new BaseModel());
            obj.Validate(0, 0, 1, 2);

            obj.ClearCount();
            obj.BaseProperty1 = 0;
            obj.Validate(0, 0, 1, 1);
        }
    }
}
#endif
