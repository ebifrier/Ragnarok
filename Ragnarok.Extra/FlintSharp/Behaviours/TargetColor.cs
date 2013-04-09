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

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The TargetColor action adjusts the color of the particle towards the target color.
    /// </summary>
    public class TargetColor : Behaviour
    {
        private uint m_red;
        private uint m_green;
        private uint m_blue;
        private uint m_alpha;
        private double m_rate;

        public TargetColor()
            : this(0xFFFFFFFF)
        {
        }

        public TargetColor(uint targetColor)
        {
            m_red = (targetColor >> 16) & 0xFF;
            m_green = (targetColor >> 8) & 0xFF;
            m_blue = (targetColor) & 0xFF;
            m_alpha = (targetColor >> 24) & 0xFF;
            m_rate = 0.1f;
        }

        /// <summary>
        /// The constructor creates a TargetColor action for use by 
        /// an emitter. To add a TargetColor to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="targetColor">The target color. This is a 32 bit color of the form 0xAARRGGBB.</param>
        /// <param name="rate">Adjusts how quickly the particle reaches the target color.
        /// Larger numbers cause it to approach the target color more quickly.</param>
        public TargetColor(uint targetColor, double rate)
        {
            m_red = (targetColor >> 16) & 0xFF;
            m_green = (targetColor >> 8) & 0xFF;
            m_blue = (targetColor) & 0xFF;
            m_alpha = (targetColor >> 24) & 0xFF;
            m_rate = rate;
        }

        /// <summary>
        /// The target color. This is a 32 bit color of the form 0xAARRGGBB.
        /// </summary>
        public uint Color
        {
            get { return (m_alpha << 24) | (m_red << 16) | (m_green << 8) | m_blue; }
            set
            {
                m_red = (value >> 16) & 0xFF;
                m_green = (value >> 8) & 0xFF;
                m_blue = (value) & 0xFF;
                m_alpha = (value >> 24) & 0xFF;
            }
        }

        /// <summary>
        /// Adjusts how quickly the particle reaches the target color.
        /// Larger numbers cause it to approach the target color more quickly.
        /// </summary>
        public double Rate
        {
            get { return m_rate; }
            set { m_rate = value; }
        }

        /// <summary>
        /// The update method is used by the emitter to apply the action
        /// to every particle. It is called within the emitter's update 
        /// loop and need not be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that created the particle.</param>
        /// <param name="particle">The particle to be updated.</param>
        /// <param name="elapsedTime">The duration of the frame - used for time based updates.</param>
        public override void Update(Emitter emitter, Particle particle, double elapsedTime)
        {
            ColorDouble color = new ColorDouble(particle.Color);

            double inv = m_rate * elapsedTime;
            double ratio = 1 - inv;

            color.Red = (uint)(color.Red * ratio + m_red * inv);
            color.Green = (uint)(color.Green * ratio + m_green * inv);
            color.Blue = (uint)(color.Blue * ratio + m_blue * inv);
            color.Alpha = (uint)(color.Alpha * ratio + m_alpha * inv);

            particle.Color = color.Color;
        }
    }

    public class ColorDouble
    {
        public double Red;
        public double Green;
        public double Blue;
        public double Alpha;

        public ColorDouble(uint color)
        {
            Red = (color >> 16) & 0xFF;
            Green = (color >> 8) & 0xFF;
            Blue = (color) & 0xFF;
            Alpha = (color >> 24) & 0xFF;
        }

        public uint Color
        {
            get { return (uint)((uint)Math.Round(Alpha) << 24) | ((uint)Math.Round(Red) << 16) | ((uint)Math.Round(Green) << 8) | (uint)Math.Round(Blue); }
        }
    }
}
