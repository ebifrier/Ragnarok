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
        /// <summary>
        /// This function is used to find a color between two other colors.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <param name="ratio">The proportion of the first color to use. The rest of the color 
        /// is made from the second color.</param>
        /// <returns>The color created.</returns>
        public static uint InterpolateColors(uint color1, uint color2, double ratio)
        {
            double inv = 1 - ratio;
            uint red = (uint)Math.Round(((color1 >> 16) & 0xFF) * ratio + ((color2 >> 16) & 0xFF) * inv);
            uint green = (uint)Math.Round(((color1 >> 8) & 0xFF) * ratio + ((color2 >> 8) & 0xFF) * inv);
            uint blue = (uint)Math.Round(((color1) & 0xFF) * ratio + ((color2) & 0xFF) * inv);
            uint alpha = (uint)Math.Round(((color1 >> 24) & 0xFF) * ratio + ((color2 >> 24) & 0xFF) * inv);
            return (alpha << 24) | (red << 16) | (green << 8) | blue;
        }
    }
}
