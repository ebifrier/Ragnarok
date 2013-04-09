/*
 * FLINT PARTICLE SYSTEM
 * .....................
 * 
 * Author: Richard Lord (Big Room)
 * C# Port: Ben Baker (HeadSoft)
 * Copyright (c) Big Room Ventures Ltd. 2008
 * http://flintparticles.org
 * 
 * 
 * Licence Agreement
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Windows;

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
    /// <summary>
    /// The material type.
    /// </summary>
    public enum MaterialType
    {
        /// <summary>
        /// Additive material.
        /// </summary>
        Emissive,
        /// <summary>
        /// Normal diffuse material.
        /// </summary>
        Diffuse,
    }

    public partial class Utils
    {
        public static Point MousePos
        {
            get;
            set;
        }

        public static Size ScreenSize
        {
            get;
            set;
        }

        public static Timer Timer
        {
            get;
            set;
        }

        public static IEnumerable<T> Reverse<T>(IList<T> list)
        {
            int i = list.Count;

            while (--i >= 0)
            {
                yield return list[i];
            }
        }

        /// <summary>
        /// Calls the event handler with null check.
        /// </summary>
        public static void RaiseEvent<T>(EventHandler<T> handler, object sender, T e)
            where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Caluculates the distance from the point c and the line
        /// connected p0 and p1.
        /// </summary>
        /// <remarks>
        /// Use vector to caluculate the distance.
        /// Vector: L = P1 - P0
        /// 
        /// 線分上で点Cと直交する点をP = P0 + t * Lとすると、
        /// (P - C)・L = 0
        ///   (P0 - C + t * L)・L = 0
        ///   t * |L|^2 = - (P0 - C)・L
        /// 
        /// distance = |P - C|
        ///          = |(P0 - C) + t * L|
        /// </remarks>
        public static double LineCircleDistance(Vector p0, Vector p1, Vector c)
        {
            var cp = p0 - c;
            var l = p1 - p0;
            var length2 = l.LengthSquared;

            if (length2 < double.Epsilon)
            {
                return cp.Length;
            }

            var t = -Vector.Multiply(cp, l) / length2;

            // Clamp the parameter t.
            t = Math.Max(0.0, Math.Min(1.0, t));

            return (cp + t * l).Length;
        }
    }
}
