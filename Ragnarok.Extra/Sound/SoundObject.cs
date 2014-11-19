using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ragnarok.Extra.Sound
{
    public abstract class SoundObject : MarshalByRefObject
    {
        public double Volume
        {
            get;
            set;
        }

        public void Stop()
        {
        }
    }
}
