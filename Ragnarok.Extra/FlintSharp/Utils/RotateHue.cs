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
        public static uint RotateHue(double hue, int brightness)
        {
            if (hue >= 360f || hue <= 0)
                hue = 0;
            hue /= 60f;

            int i = (int)Math.Floor(hue);
            double f = hue - i;
            double q = 1.0 - f;
            int r, g, b;

            switch (i)
            {
                case 0:
                    r = 255; g = (int)(255 * f) % 256; b = 0;
                    break;
                case 1:
                    r = (int)(255 * q) % 256; g = 255; b = 0;
                    break;
                case 2:
                    r = 0; g = 255; b = (int)(255 * f) % 256;
                    break;
                case 3:
                    r = 0; g = (int)(255 * q) % 256; b = 255;
                    break;
                case 4:
                    r = (int)(255 * f) % 256; g = 0; b = 255;
                    break;
                default:
                    r = 255; g = 0; b = (int)(255 * q) % 256;
                    break;
            }

            return (
                0xFF000000 |
                ((uint)(r + (brightness - r) / 2) << 16) |
                ((uint)(g + (brightness - g) / 2) << 8) |
                ((uint)(b + (brightness - b) / 2) << 0));
        }
    }
}
