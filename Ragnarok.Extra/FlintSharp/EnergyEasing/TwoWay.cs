/*
 * FLINT PARTICLE SYSTEM
 * .....................
 * 
 * This public class is a modified version of Robert Penner's Actionscript 2 easing equations
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

namespace FlintSharp.EnergyEasing
{
    public static class TwoWay
    {
        public static double Linear(double age, double lifetime)
        {
            if ((age = 2 * age / lifetime) <= 1)
                return age;

            return 2 - age;
        }

        public static double Circular(double age, double lifetime)
        {
            age = 1 - (2 * age / lifetime);

            return Math.Sqrt(1 - age * age);
        }

        public static double Sine(double age, double lifetime)
        {
            return Math.Sin(Math.PI * age / lifetime);
        }

        public static double Quadratic(double age, double lifetime)
        {
            age = 1 - (2 * age / lifetime);

            return -(age * age - 1);
        }

        public static double Cubic(double age, double lifetime)
        {
            age = 1 - (2 * age / lifetime);

            if (age < 0) age = -age;
            return -(age * age * age - 1);
        }

        public static double Quartic(double age, double lifetime)
        {
            age = 1 - (2 * age / lifetime);

            return -(age * age * age * age - 1);
        }

        public static double quintic(double age, double lifetime)
        {
            age = 1 - (2 * age / lifetime);

            if (age < 0) age = -age;
            return -(age * age * age * age * age - 1);
        }
    }
}
