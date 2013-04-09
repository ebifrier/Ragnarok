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

namespace FlintSharp.EnergyEasing
{
    public class Bounce
    {
        public static double EaseOut(double age, double lifetime)
        {
            if ((age /= lifetime) < 1 / 2.75)
                return 1 - 7.5625 * age * age;
            else if (age < 2 / 2.75)
                return 0.25 - 7.5625 * (age -= (1.5 / 2.75)) * age;
            else if (age < 2.5 / 2.75)
                return 0.0625 - 7.5625 * (age -= (2.25 / 2.75)) * age;
            else
                return 0.015625 - 7.5625 * (age -= (2.625 / 2.75)) * age;
        }

        public static double EaseIn(double age, double lifetime)
        {
            return 1 - EaseOut(lifetime - age, lifetime);
        }

        public static double EaseInOut(double age, double lifetime)
        {
            if (age < lifetime * 0.5)
                return EaseIn(age * 2, lifetime) * 0.5 + 0.5;
            else
                return EaseOut(age * 2 - lifetime, lifetime) * 0.5;
        }
    }
}
