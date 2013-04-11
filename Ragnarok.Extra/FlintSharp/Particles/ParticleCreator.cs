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
using System.Collections.Generic;

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.EnergyEasing;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

namespace FlintSharp.Particles
{
    /// <summary>
	/// The ParticleCreator is used by the Emitter public class to manage the creation and reuse of particles.
	/// To speed up the particle system, the ParticleCreator public class maintains a pool of dead particles 
	/// and reuses them when a new particle is needed, rather than creating a whole new particle.
    /// </summary>
    public class ParticleCreator : IParticleFactory
    {
        private Stack<Particle> m_particles = new Stack<Particle>();

        /// <summary>
        /// The constructor creates a ParticleCreator object.
        /// </summary>
        public ParticleCreator()
        {
        }

        /// <summary>
        /// Obtains a new Particle object. The createParticle method will return
        /// a dead particle from the poll of dead particles or create a new particle if none are
        /// available.
        /// </summary>
        /// <returns>a Particle object.</returns>
        public Particle CreateParticle()
        {
            if (m_particles.Count > 0)
            {
                return m_particles.Pop();
            }

            return new Particle();
        }

        /// <summary>
        /// Returns a particle to the particle pool for reuse
        /// </summary>
        /// <param name="particle">The particle to return for reuse.</param>
        public void DisposeParticle(Particle particle)
        {
            particle.Initialize();
            m_particles.Push(particle);
        }

        /// <summary>
        /// Empties the particle pool.
        /// </summary>
        public void ClearAllParticles()
        {
            m_particles.Clear();
        }

        /// <summary>
        /// Updates the particles in the pool.
        /// </summary>
        public void Update(double elapsedTime)
        {
        }
    }
}
