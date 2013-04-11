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

namespace FlintSharp.Initializers
{
    /// <summary>
	/// The RotationAbsolute Initializer sets the rotation of the particle. The rotation is
	/// independent of the rotation of the emitter.
    /// </summary>
    public class RotationAbsolute : Initializer
    {
        private double m_min;
        private double m_max;

        public RotationAbsolute()
            : this(0.0)
        {
        }

        public RotationAbsolute(double minAngle)
        {
            m_min = Utils.DegreesToRadians(minAngle);
            m_max = double.NaN;
        }

        /// <summary>
		/// The constructor creates a RotationAbsolute initializer for use by 
		/// an emitter. To add a RotationAbsolute to all particles created by an emitter, use the
		/// emitter's addInitializer method.
		/// 
		/// <p>The rotation of particles initialized by this public class
		/// will be a random value between the minimum and maximum
		/// values set. If no maximum value is set, the minimum value
		/// is used with no variation.</p>
        /// </summary>
        /// <param name="minAngle">The minimum angle, in radians, for the particle's rotation.</param>
        /// <param name="maxAngle">The maximum angle, in radians, for the particle's rotation.</param>
        public RotationAbsolute(double minAngle, double maxAngle)
        {
            m_min = Utils.DegreesToRadians(minAngle);
            m_max = Utils.DegreesToRadians(maxAngle);
        }

        /// <summary>
        /// The minimum angle for particles initialised by 
        /// this initializer.
        /// </summary>
        public double MinAngle
        {
            get { return Utils.RadiansToDegrees(m_min); }
            set { m_min = Utils.DegreesToRadians(value); }
        }

        /// <summary>
        /// The maximum angle for particles initialised by 
        /// this initializer.
        /// </summary>
        public double MaxAngle
        {
            get { return Utils.RadiansToDegrees(m_max); }
            set { m_max = Utils.DegreesToRadians(value); }
        }

        /// <summary>
        /// When reading, returns the average of minAngle and maxAngle.
        /// When writing this sets both maxAngle and minAngle to the 
        /// same angle value.
        /// </summary>
        public double Angle
        {
            get { return Utils.RadiansToDegrees(m_min == m_max ? m_min : (m_max + m_min) * 0.5f); }
            set { m_max = m_min = Utils.DegreesToRadians(value); }
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
            if (double.IsNaN(m_max))
                particle.Rotation = m_min;
            else
                particle.Rotation = Utils.RandomDouble(m_min, m_max);
        }
    }
}
