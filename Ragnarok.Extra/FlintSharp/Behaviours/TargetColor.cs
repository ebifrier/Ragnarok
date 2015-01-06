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

using Ragnarok.Utility;

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The TargetColor action adjusts the color of the particle towards the target color.
    /// </summary>
    public class TargetColor : Behaviour
    {
        private Color4b m_color;
        private double m_rate;

        public TargetColor()
            : this(Color4bs.White)
        {
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
        public TargetColor(Color4b targetColor, double rate)
        {
            m_color = targetColor;
            m_rate = rate;
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
        [CLSCompliant(false)]
        public TargetColor(uint targetColor, double rate)
            : this(Color4b.FromValue(targetColor), rate)
        {
        }

        public TargetColor(Color4b targetColor)
            : this(targetColor, 0.1)
        {
        }

        [CLSCompliant(false)]
        public TargetColor(uint targetColor)
            : this(Color4b.FromValue(targetColor))
        {
        }

        /// <summary>
        /// The target color.
        /// </summary>
        public Color4b Color
        {
            get { return m_color; }
            set { m_color = value; }
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
            double ratio = m_rate * elapsedTime;

            particle.Color = Utils.InterpolateColors(m_color, particle.Color, ratio);

            /*color.Red = (int)(pcolor.R * ratio + m_color.R * inv);
            color.Green = (int)(pcolor.G * ratio + m_color.G * inv);
            color.Blue = (int)(pcolor.B * ratio + m_color.B * inv);
            color.Alpha = (int)(pcolor.A * ratio + m_color.A * inv);

            particle.Color = color.Color;*/
        }
    }
}
