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
	/// The ColorInit Initializer sets the velocity of the particle. It is
	/// usually combined with the Move action to move the particle
	/// using this velocity.
	/// 
	/// <p>The initial velocity is defined using a zone from the 
	/// org.flintparticles.zones package. The use of zones enables diverse 
	/// ranges of velocities. For example, to use a specific velocity,
	/// a Point zone can be used. To use a varied speed in a specific
	/// direction, a LineZone zone can be used. For a fixed speed in
	/// a varied direction, a Disc or DiscSector zone with identical
	/// inner and outer radius can be used. A Disc or DiscSector with
	/// different inner and outer radius produces a range of speeds
    /// in a range of directions.
    /// </summary>
    public class Velocity : Initializer
    {
        private IZone m_zone = null;

        public Velocity()
            : this(null)
        {
        }

        /// <summary>
		/// The constructor creates a Velocity initializer for use by 
		/// an emitter. To add a Velocity to all particles created by an emitter, use the
		/// emitter's addInitializer method.
        /// </summary>
        /// <param name="zone"></param>
        public Velocity(IZone zone)
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
                particle.VelocityX = loc.X;
                particle.VelocityY = loc.Y;
            }
            else
            {
                double sin = Math.Sin(emitter.RotRadians);
                double cos = Math.Cos(emitter.RotRadians);

                loc = m_zone.GetLocation();

                particle.VelocityX = cos * loc.X - sin * loc.Y;
                particle.VelocityY = cos * loc.Y + sin * loc.X;
            }
        }
    }
}
