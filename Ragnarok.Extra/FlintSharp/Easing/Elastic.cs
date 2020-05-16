/*
 * FLINT PARTICLE SYSTEM
 * .....................
 * 
 * This public class is an update to Actionscript 3 of Robert Penner's Actionscript 2 easing equations
 * which are available under the following licence from http://www.robertpenner.com/easing/
 * 
 * TERMS OF USE - EASING EQUATIONS
 * 
 * Open source under the BSD License.
 * 
 * Copyright (c) 2001 Robert Penner
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are 
 * permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of 
 * conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list 
 * of conditions and the following disclaimer in the documentation and/or other materials 
 * provided with the distribution.
 * Neither the name of the author nor the names of contributors may be used to endorse 
 * or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
 * SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
 * OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR 
 * TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * ==================================================
 * 
 * Modifications:
 * 
 * Author: Richard Lord (Big Room)
 * C# Port: Ben Baker (HeadSoft)
 * Copyright (c) Big Room Ventures Ltd. 2008
 * http://flintparticles.org
 * 
 * 
 * Used in the Flint Particle System which is licenced under the MIT license. As per the
 * original license for Robert Penner's public classes, these specific public classes are released under 
 * the BSD License.
 */

using System;

namespace FlintSharp.Easing
{
    public static class Elastic
    {
        public static double EaseIn(double t, double b, double c, double d)
        {
            return EaseIn(t, b, c, d, 0, 0);
        }

        public static double EaseIn(double t, double b, double c, double d, double a, double p)
        {
            if (t == 0)
                return b;

            if ((t /= d) == 1)
                return b + c;

            if (p == 0)
                p = d * 0.3;

            double s = 0;

            if (a == 0 || a < Math.Abs(c))
            {
                a = c;
                s = p * 0.25;
            }
            else
                s = p / (2 * Math.PI) * Math.Asin(c / a);

            return -(a * Math.Pow(2, 10 * (--t)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
        }

        public static double EaseOut(double t, double b, double c, double d)
        {
            return EaseOut(t, b, c, d, 0, 0);
        }

        public static double EaseOut(double t, double b, double c, double d, double a, double p)
        {
            if (t == 0)
                return b;

            if ((t /= d) == 1)
                return b + c;

            if (p == 0)
                p = d * 0.3;

            double s = 0;

            if (a == 0 || a < Math.Abs(c))
            {
                a = c;
                s = p * 0.25;
            }
            else
                s = p / (2 * Math.PI) * Math.Asin(c / a);

            return a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b;
        }

        public static double EaseInOut(double t, double b, double c, double d)
        {
            return EaseInOut(t, b, c, d, 0, 0);
        }

        public static double EaseInOut(double t, double b, double c, double d, double a, double p)
        {
            if (t == 0)
                return b;

            if ((t /= d * 0.5) == 2)
                return b + c;

            if (p == 0)
                p = d * (0.3 * 1.5);

            double s = 0;

            if (a == 0 || a < Math.Abs(c))
            {
                a = c;
                s = p * 0.25;
            }
            else
                s = p / (2 * Math.PI) * Math.Asin(c / a);

            if (t < 1)
                return -0.5 * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;

            return a * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * 0.5 + c + b;
        }
    }
}
