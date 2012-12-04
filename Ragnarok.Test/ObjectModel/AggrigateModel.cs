using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Ragnarok.ObjectModel;

namespace Ragnarok.Test.ObjectModel
{
    public class AggregateClass : ModelTestBase
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

        public AggregateClass(BaseModel model)
        {
            this.model = model;
            this.PropertyChanged += AggregationClass_PropertyChanged;

            this.AddDependModel(this.model);
        }

        void AggregationClass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(
                "changed {0}, {1}",
                sender.GetType().ToString(),
                e.PropertyName);
        }
    }
}
