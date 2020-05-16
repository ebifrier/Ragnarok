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
using System;

namespace FlintSharp.Initializers
{
    /// <summary>
    /// The ScaleInit Initializer sets the size of the particle.
    /// </summary>
    public class ScaleInit : Initializer
    {
        private double m_min;
        private double m_max;

        public ScaleInit()
            : this(1.0)
        {
        }

        public ScaleInit(double minScale)
        {
            m_min = minScale;
            m_max = m_min;
        }

        /// <summary>
		/// The constructor creates a ScaleInit initializer for use by 
		/// an emitter. To add a ScaleInit to all particles created by an emitter, use the
		/// emitter's addInitializer method.
		/// 
		/// <p>The scale factor of particles initialized by this public class
		/// will be a random value between the minimum and maximum
		/// values set. If no maximum value is set, the minimum value
		/// is used with no variation.</p>
        /// </summary>
        /// <param name="minScale"></param>
        /// <param name="maxScale"></param>
        public ScaleInit(double minScale, double maxScale)
        {
            m_min = minScale;
            m_max = maxScale;
        }

        /// <summary>
        /// The minimum scale value for particles initialised by 
        /// this initializer. Should be between 0 and 1.
        /// </summary>
        public double MinScale
        {
            get { return m_min; }
            set { m_min = value; }
        }

        /// <summary>
        /// The maximum scale value for particles initialised by 
        /// this initializer. Should be between 0 and 1.
        /// </summary>
        public double MaxScale
        {
            get { return m_max; }
            set { m_max = value; }
        }

        /// <summary>
        /// When reading, returns the average of minScale and maxScale.
        /// When writing this sets both maxScale and minScale to the 
        /// same scale value.
        /// </summary>
        public double Scale
        {
            get { return m_min == m_max ? m_min : (m_max + m_min) * 0.5f; }
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
                particle.Scale = m_min;
            else
                particle.Scale = Utils.RandomDouble(m_min, m_max);
        }
    }
}
