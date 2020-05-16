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

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The Collide action detects collisions between particles and modifies their 
    /// velocities in response to the collision. All particles are approximated to 
    /// a circular shape for the collisions and they are assumed to be of equal 
    /// density.
    /// 
    /// <p>If the particles reach a stationary, or near stationary, state under an 
    /// accelerating force (e.g. gravity) then they will fall through each other. 
    /// This is due to the nature of the alogorithm used, which is designed for 
    /// speed of execution and sufficient accuracy when the particles are in motion, 
    /// not for absolute precision.</p>
    /// </summary>
    public class Collide : Behaviour
    {
        private double m_radius;
        private double m_bounce;

        public Collide()
            : this(1.0)
        {
        }

        public Collide(double radius)
        {
            m_radius = radius;
            m_bounce = 1;
        }

        /// <summary>
        /// The constructor creates a Collide action for use by  an emitter.
        /// To add a Collide to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.common.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="radius">The radius used for the size of each particle when 
        /// calculating the collisions. This radius is multiplied by the scale 
        /// property of each particle to get the particle's actual size and 
        /// relative mass for calculating the collision and response.</param>
        /// <param name="bounce">The coefficient of restitution when the particles collide. 
        /// A value of 1 gives a pure elastic collision, with no energy loss. A 
        /// value between 0 and 1 causes the particles to loose enegy in the 
        /// collision. A value greater than 1 causes the particle to gain energy 
        /// in the collision.</param>
        public Collide(double radius, double bounce)
        {
            m_radius = radius;
            m_bounce = bounce;
        }

        /// <summary>
        /// The radius used for the size of each particle used when calculating the 
        /// collisions. This radius is multiplied by the scale property of each 
        /// particle to get the particle's actual size and relative mass for 
        /// calculating the collision and response.
        /// </summary>
        public double Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }

        /// <summary>
        /// The coefficient of restitution when the particles collide. A value of 
        /// 1 gives a pure elastic collision, with no energy loss. A value
        /// between 0 and 1 causes the particles to loose enegy in the collision. 
        /// A value greater than 1 causes the particles to gain energy in the collision.
        /// </summary>
        public double Bounce
        {
            get { return m_bounce; }
            set { m_bounce = value; }
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of actions.
        /// It is called within the emitter's addAction method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.common.actions.Action#getDefaultPriority()
        /// </summary>
        /// <returns>Returns a value of 10, so that the collide action executes before
        /// other actions that move teh particles independently of each other.</returns>
        public override int GetDefaultPriority()
        {
            return 10;
        }

        /// <summary>
        /// Instructs the emitter to produce a sorted particle array for optimizing
        /// the calculations in the update method of this action.
        /// 
        /// @see update()
        /// </summary>
        /// <param name="emitter"></param>
        public override void AddedToEmitter(Emitter emitter)
        {
            if (emitter == null)
            {
                return;
            }

            emitter.SpaceSort = true;
        }

        /// <summary>
        /// Causes the particle to check for collisions against all other particles.
        /// 
        /// <p>This method is called by the emitter and need not be called by the 
        /// user.</p>
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

            List<Particle> particles = emitter.Particles;
            Particle other;
            int i;
            int len = particles.Count;
            double factor;
            double distanceSq;
            double collisionDist;
            double dx, dy;
            double n1, n2;
            double relN;
            double m1, m2;
            double f1, f2;

            for (i = particles.IndexOf(particle) + 1; i < len; i++)
            {
                other = particles[i];
                //collisionDist = other.Scale * m_radius + particle.Scale * m_radius;
                collisionDist = other.Scale * m_radius;

                if ((dx = other.X - particle.X) > collisionDist)
                    continue;

                dy = other.Y - particle.Y;

                if (dy > collisionDist || dy < -collisionDist)
                    continue;

                distanceSq = dy * dy + dx * dx;

                if (distanceSq <= collisionDist * collisionDist && distanceSq > 0)
                {
                    factor = (1 / Math.Sqrt(distanceSq));
                    dx *= factor;
                    dy *= factor;
                    n1 = dx * particle.VelocityX + dy * particle.VelocityY;
                    n2 = dx * other.VelocityX + dy * other.VelocityY;
                    relN = n1 - n2;

                    if (relN > 0)
                    {
                        m1 = particle.Scale * particle.Scale;
                        m2 = other.Scale * other.Scale;
                        factor = ((1 + m_bounce) * relN) / (m1 + m2);
                        f1 = factor * m2;
                        f2 = -factor * m1;
                        particle.VelocityX -= f1 * dx;
                        particle.VelocityY -= f1 * dy;
                        other.VelocityX -= f2 * dx;
                        other.VelocityY -= f2 * dy;
                    }
                }
            }
        }
    }
}
