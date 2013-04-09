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

namespace FlintSharp.Initializers
{
    /// <summary>
    /// The AlphaInit Initializer sets the alpha transparency of the particle.
    /// </summary>
    public class AlphaInit : Initializer
    {
        private double m_min;
        private double m_max;

        public AlphaInit()
            : this(1.0)
        {
        }

        public AlphaInit(double minAlpha)
        {
            m_min = minAlpha;
            m_max = m_min;
        }

        /// <summary>
		/// The constructor creates an AlphaInit initializer for use by 
		/// an emitter. To add an AlphaInit to all particles created by an emitter, use the
		/// emitter's addInitializer method.
		/// 
		/// <p>The alpha of particles initialized by this public class
		/// will be a random value between the minimum and maximum
		/// values set. If no maximum value is set, the minimum value
		/// is used with no variation.</p>
        /// </summary>
        /// <param name="minAlpha">the minimum alpha for particles
        /// initialized by the instance. The value should be between 1 and 0.</param>
        /// <param name="maxAlpha">the maximum alpha for particles
        /// initialized by the instance. The value should be between 1 and 0.</param>
        public AlphaInit(double minAlpha, double maxAlpha)
        {
            m_min = minAlpha;
            m_max = maxAlpha;
        }

        /// <summary>
		/// The minimum alpha value for particles initialised by 
		/// this initializer. Should be between 0 and 1.
        /// </summary>
        public double MinAlpha
        {
            get { return m_min; }
            set { m_min = value; }
        }

        /// <summary>
		/// The maximum alpha value for particles initialised by 
		/// this initializer. Should be between 0 and 1.
        /// </summary>
        public double MaxAlpha
        {
            get { return m_max; }
            set { m_max = value; }
        }

        /// <summary>
		/// When reading, returns the average of minAlpha and maxAlpha.
		/// When writing this sets both maxAlpha and minAlpha to the 
		/// same alpha value.
        /// </summary>
        public double Alpha
        {
            get { return m_min == m_max ? m_min : (m_max + m_min) / 2; }
            set { m_max = m_min = value; }
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of initializers.
        /// It is called within the emitter's addInitializer method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addInitializer()
        /// </summary>
        /// <returns>returns -10 to ensure it occurs after the color assignment 
        /// public classes like ColorInit.</returns>
        public override int GetDefaultPriority()
        {
            return -10;
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
            double alpha;

            if (m_max == m_min)
                alpha = m_min;
            else
                alpha = Utils.RandomDouble(m_min, m_max);

            particle.Color = (uint)((particle.Color & 0xFFFFFF) | ((uint)Math.Round(alpha * 255) << 24));
        }
    }
}
