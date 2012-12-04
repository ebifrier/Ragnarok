using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.ObjectModel;

namespace Ragnarok.Test.ObjectModel
{
    public class SimpleClass
    {
        public int SimpleBaseProperty1
        {
            get { return 1; }
            set { }
        }

        [DependOnProperty("SimpleBaseProperty1")]
        public int SimpleBaseProperty2
        {
            get { return 2; }
            set { }
        }

        [DependOnProperty("SimpleBaseProperty1")]
        public int SimpleInheritProperty1
        {
            get { return 3; }
            set { }
        }

        [DependOnProperty("SimpleBaseProperty2")]
        public int SimpleInheritProperty2
        {
            get { return 4; }
            set { }
        }
    }
}
