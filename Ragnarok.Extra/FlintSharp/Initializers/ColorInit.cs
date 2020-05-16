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

namespace FlintSharp.Initializers
{
    /// <summary>
    /// The ColorInit Initializer sets the color of the particle.
    /// </summary>
    public class ColorInit : Initializer
    {
        private Color4b m_min;
        private Color4b m_max;

        public ColorInit()
            : this(Color4bs.White, Color4bs.White)
        {
        }

        /// <summary>
		/// The constructor creates a ColorInit initializer for use by 
		/// an emitter. To add a ColorInit to all particles created by an emitter, use the
		/// emitter's addInitializer method.
		/// 
		/// <p>The color of particles initialized by this public class
		/// will be a random value between the two values pased to
		/// the constructor. For a fixed value, pass the same color
		/// in for both parameters.</p>
        /// </summary>
        /// <param name="color1">the 32bit (ARGB) color at one end of the color range to use.</param>
        /// <param name="color2">the 32bit (ARGB) color at the other end of the color range to use.</param>
        public ColorInit(Color4b color1, Color4b color2)
        {
            m_min = color1;
            m_max = color2;
        }

        /// <summary>
		/// The minimum color value for particles initialised by 
		/// this initializer. Should be between 0 and 1.
        /// </summary>
        public Color4b MinColor
        {
            get { return m_min; }
            set { m_min = value; }
        }

        /// <summary>
		/// The maximum color value for particles initialised by 
		/// this initializer. Should be between 0 and 1.
        /// </summary>
        public Color4b MaxColor
        {
            get { return m_max; }
            set { m_max = value; }
        }

        /// <summary>
		/// When reading, returns the average of minColor and maxColor.
		/// When writing this sets both maxColor and minColor to the 
		/// same color.
        /// </summary>
        public Color4b Color
        {
            get { return m_min == m_max ? m_min : Utils.InterpolateColors(m_max, m_min, 0.5f); }
            set { m_max = m_min = value; }
        }

        /// <summary>
        /// The initialize method is used by the emitter to initialize the particle.
        /// It is called within the emitter's createParticle method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that created the particle.</param>
        /// <param name="particle">The particle to be initialized.</param>
        public override void Initialize(Emitter emitter, Particle particle)
        {
            if (particle == null)
            {
                return;
            }

            if (m_max == m_min)
                particle.Color = m_min;
            else
                particle.Color = Utils.InterpolateColors(m_min, m_max, Utils.Random.NextDouble());
        }
    }
}
