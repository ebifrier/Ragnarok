using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.ObjectModel;

namespace RagnarokTest.ObjectModel
{
    public class DerivedClass : BaseModel
    {
        [DependOnProperty(typeof(BaseModel), "InheritProperty1")]
        public int DerivedProperty1
        {
            get { return 10; }
        }
    }
}
