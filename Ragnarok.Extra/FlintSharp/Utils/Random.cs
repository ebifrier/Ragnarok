using System;

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.EnergyEasing;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

namespace FlintSharp
{
    public partial class Utils
    {
        public static Random Random = null;

        static Utils()
        {
            Random = new Random();
        }

        public static double RandomDouble(double min, double max)
        {
            return ((max - min) * Random.NextDouble() + min);
        }
    }
}
