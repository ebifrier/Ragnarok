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
    /// The Friction action applies friction to the particle to slow it down when it's moving.
    /// The frictional force is constant, irrespective of how fast the particle is moving.
    /// For forces proportional to the particle's velocity, use one of the drag effects -
    /// LinearDrag and QuadraticDrag.
    /// 
    /// @see LinearDrag
    /// @see QuadraticDrag
    /// </summary>
    public class Friction : Behaviour
    {
        private double m_friction;

        public Friction()
            : this(1.0)
        {
        }

        /// <summary>
        /// The constructor creates a Friction action for use by 
        /// an emitter. To add a Friction to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="friction">The amount of friction. A higher number produces a stronger frictional force.</param>
        public Friction(double friction)
        {
            m_friction = friction;
        }

        /// <summary>
        /// The amount of friction. A higher number produces a stronger frictional force.
        /// </summary>
        public double FrictionValue
        {
            get { return m_friction; }
            set { m_friction = value; }
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
            if (emitter == null)
            {
                return;
            }

            if (particle == null)
            {
                return;
            }

            double len2 = particle.VelocityX * particle.VelocityX + particle.VelocityY * particle.VelocityY;

            if (len2 == 0)
                return;

            double scale = (1 - m_friction * elapsedTime / Math.Sqrt(len2));

            if (scale < 0)
            {
                particle.VelocityX = 0;
                particle.VelocityY = 0;
            }
            else
            {
                particle.VelocityX *= scale;
                particle.VelocityY *= scale;
            }
        }
    }
}
