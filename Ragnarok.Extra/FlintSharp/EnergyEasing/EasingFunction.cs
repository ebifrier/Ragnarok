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

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.EnergyEasing;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

namespace FlintSharp.EnergyEasing
{
    public delegate double EnergyEasingDelegate(double age, double lifetime);

    public class EasingFunction
    {
        public static EnergyEasingDelegate GetEasingFunction(EaseCategory easeCategory, EaseType easeType)
        {
            switch (easeCategory)
            {
                case EaseCategory.Linear:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Linear.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Linear.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Linear.EaseInOut);
                        case EaseType.None:
                            return new EnergyEasingDelegate(Linear.EaseNone);
                    }
                    break;
                case EaseCategory.Quadratic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Quadratic.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Quadratic.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Quadratic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Cubic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Cubic.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Cubic.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Cubic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Quartic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Quartic.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Quartic.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Quartic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Quintic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Quintic.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Quintic.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Quintic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Sine:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Sine.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Sine.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Sine.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Exponential:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Exponential.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Exponential.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Exponential.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Circular:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Circular.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Circular.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Circular.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Elastic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Elastic.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Elastic.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Elastic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Back:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Back.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Back.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Back.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Bounce:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EnergyEasingDelegate(Bounce.EaseIn);
                        case EaseType.Out:
                            return new EnergyEasingDelegate(Bounce.EaseOut);
                        case EaseType.InOut:
                            return new EnergyEasingDelegate(Bounce.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
            }

            return new EnergyEasingDelegate(Linear.EaseNone);
        }
    }
}
