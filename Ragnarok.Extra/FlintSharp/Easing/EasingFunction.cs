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

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.EnergyEasing;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

namespace FlintSharp.Easing
{
    public enum EaseCategory
    {
        Linear,
        Quadratic,
        Cubic,
        Quartic,
        Quintic,
        Sine,
        Exponential,
        Circular,
        Elastic,
        Back,
        Bounce
    }

    public enum EaseType
    {
        None,
        In,
        Out,
        InOut
    }

    public delegate double EasingDelegate(double start, double end, double duration, double elapsedTime);

    public class EasingFunction
    {
        public static EasingDelegate GetEasingFunction(EaseCategory easeCategory, EaseType easeType)
        {
            switch (easeCategory)
            {
                case EaseCategory.Linear:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Linear.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Linear.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Linear.EaseInOut);
                        case EaseType.None:
                            return new EasingDelegate(Linear.EaseNone);
                    }
                    break;
                case EaseCategory.Quadratic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Quadratic.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Quadratic.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Quadratic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Cubic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Cubic.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Cubic.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Cubic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Quartic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Quartic.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Quartic.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Quartic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Quintic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Quintic.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Quintic.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Quintic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Sine:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Sine.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Sine.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Sine.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Exponential:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Exponential.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Exponential.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Exponential.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Circular:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Circular.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Circular.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Circular.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Elastic:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Elastic.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Elastic.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Elastic.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Back:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Back.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Back.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Back.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
                case EaseCategory.Bounce:
                    switch (easeType)
                    {
                        case EaseType.In:
                            return new EasingDelegate(Bounce.EaseIn);
                        case EaseType.Out:
                            return new EasingDelegate(Bounce.EaseOut);
                        case EaseType.InOut:
                            return new EasingDelegate(Bounce.EaseInOut);
                        case EaseType.None:
                            break;
                    }
                    break;
            }

            return new EasingDelegate(Linear.EaseNone);
        }
    }
}
