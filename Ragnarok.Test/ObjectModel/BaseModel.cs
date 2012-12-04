using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Ragnarok.ObjectModel;

namespace Ragnarok.Test.ObjectModel
{
    public class ModelTestBase : IParentModel, ILazyModel
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private List<object> dependModelList = new List<object>();
        public List<object> DependModelList
        {
            get { return this.dependModelList; }
        }

        private LazyModelObject lockCount = new LazyModelObject();
        LazyModelObject ILazyModel.LazyModelObject
        {
            get { return this.lockCount; }
        }

        /// <summary>
        /// プロパティの変更通知を出します。
        /// </summary>
        public void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged(this, e);
        }
    }

    public class BaseModel : ModelTestBase
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

        public BaseModel()
        {
            this.PropertyChanged += BaseModel_PropertyChanged;
        }

        void BaseModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(
                "changed {0}, {1}",
                sender.GetType().ToString(),
                e.PropertyName);
        }
    }
}
