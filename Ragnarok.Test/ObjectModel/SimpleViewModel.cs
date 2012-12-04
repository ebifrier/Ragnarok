using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Ragnarok.ObjectModel;

namespace Ragnarok.Test.ObjectModel
{
    class SimpleViewModel : DynamicViewModel
    {
        [DependOnProperty(typeof(BaseModel), "BaseProperty1")]
        public int ViewBaseProperty1
        {
            get { return 1; }
        }

        private void This_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(
                "changed {0}, {1}",
                sender.GetType().ToString(),
                e.PropertyName);
        }

        public SimpleViewModel()
        {
            PropertyChanged += This_PropertyChanged;
        }
    }
}
