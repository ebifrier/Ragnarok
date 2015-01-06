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

using Ragnarok.Utility;

namespace FlintSharp
{
    public partial class Utils
    {
        public static Random Random = null;

        static Utils()
        {
            Random = new Random();
        }

        #region Math
        public static double RandomDouble(double min, double max)
        {
            return ((max - min) * Random.NextDouble() + min);
        }
        
        public static double RadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }

        public static double DegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(SqrDistance(p1, p2));
        }

        public static double SqrDistance(Point p1, Point p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        public static Point PointToPolar(double len, double angleInRadians)
        {
            return new Point(
                len * Math.Cos(angleInRadians),
                len * Math.Sin(angleInRadians));
        }
        #endregion

        #region Color
        /// <summary>
        /// This function is used to find a color between two other colors.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <param name="ratio">The proportion of the first color to use. The rest of the color 
        /// is made from the second color.</param>
        /// <returns>The color created.</returns>
        public static Color4b InterpolateColors(Color4b color1, Color4b color2, double ratio)
        {
            double inv = 1.0 - ratio;
#if false
            int red = (int)Math.Round(color1.R * ratio + color2.R * inv);
            int green = (int)Math.Round(color1.G * ratio + color2.G * inv);
            int blue = (int)Math.Round(color1.B * ratio + color2.B * inv);
            int alpha = (int)Math.Round(color1.A * ratio + color2.A * inv);
#else
            int red = (int)(color1.R * ratio + color2.R * inv);
            int green = (int)(color1.G * ratio + color2.G * inv);
            int blue = (int)(color1.B * ratio + color2.B * inv);
            int alpha = (int)(color1.A * ratio + color2.A * inv);
#endif
            return Color4b.FromArgb(alpha, red, green, blue);
        }

        public static Color4b RotateHue(double hue, int brightness)
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

            return Color4b.FromArgb(
                255,
                (int)(r + (brightness - r) / 2),
                (int)(g + (brightness - g) / 2),
                (int)(b + (brightness - b) / 2));
        }

#if false
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
#endif
        #endregion
    }
}
