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
using System.Windows;

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
	/// The Position Initializer sets the initial location of the particle.
	/// 
	/// <p>The public class uses zones to place the particle. A zone defines a region
	/// in relation to the emitter and the particle is placed at a random point within
	/// that region. For precise placement, the Point zone defines a single
	/// point at which all particles will be placed. Various zones (and the
	/// Zones public interface for use when implementing custom zones) are defined
	/// in the org.flintparticles.zones package.</p>
    /// </summary>
    public class Position : Initializer
    {
        private IZone m_zone = null;

        public Position()
            : this(null)
        {
        }

        /// <summary>
		/// The constructor creates a Position initializer for use by 
		/// an emitter. To add a Position to all particles created by an emitter, use the
		/// emitter's addInitializer method.
        /// </summary>
        /// <param name="zone">The zone to place all particles in.</param>
        public Position(IZone zone)
        {
            m_zone = zone;
        }

        /// <summary>
        /// The zone.
        /// </summary>
        public IZone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
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
            if (emitter == null)
            {
                return;
            }

            if (particle == null)
            {
                return;
            }

            Point loc;

            if (emitter.RotRadians == 0)
            {
                loc = m_zone.GetLocation();

                particle.X = loc.X;
                particle.Y = loc.Y;
            }
            else
            {
                double sin = Math.Sin(emitter.RotRadians);
                double cos = Math.Cos(emitter.RotRadians);

                loc = m_zone.GetLocation();

                particle.X = cos * loc.X - sin * loc.Y;
                particle.Y = cos * loc.Y + sin * loc.X;
            }

            particle.X += emitter.X;
            particle.Y += emitter.Y;

            particle.PreviousX = particle.X;
            particle.PreviousY = particle.Y;
        }
    }
}
