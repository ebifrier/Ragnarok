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
    /// The MatchVelocity action applies an acceleration to the particle to match
    /// its velocity to that of its nearest neighbours.
    /// </summary>
    public class MatchVelocity : Behaviour
    {
        private double m_max;
        private double m_acc;
        private double m_maxSq;

        public MatchVelocity()
            : this(10.0, 1.0)
        {
        }

        /// <summary>
        /// The constructor creates a MatchVelocity action for use by 
        /// an emitter. To add a MatchVelocity to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="maxDistance">The maximum distance, in pixels, over which this action operates.
        /// The particle will match its velocity other particles that are this close or closer to it.</param>
        /// <param name="acceleration">The acceleration force applied to adjust velocity to match that
        /// of the other particles.</param>
        public MatchVelocity(double maxDistance, double acceleration)
        {
            m_max = maxDistance;
            m_acc = acceleration;
            m_maxSq = maxDistance * maxDistance;
        }

        /// <summary>
        /// The maximum distance, in pixels, over which this action operates.
        /// The particle will match its velocity other particles that are this close or closer to it.
        /// </summary>
        public double MaxDistance
        {
            get { return m_max; }
            set { m_max = value; }
        }

        /// <summary>
        /// The acceleration force applied to adjust velocity to match that
        /// of the other particles.
        /// </summary>
        public double Acceleration
        {
            get { return m_acc; }
            set { m_acc = value; }
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of actions.
        /// It is called within the emitter's addAction method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.common.actions.Action#getDefaultPriority()
        /// </summary>
        /// <returns><p>Returns a value of 10, so that the MutualGravity action executes before other actions.</p></returns>
        public override int GetDefaultPriority()
        {
            return 10;
        }

        /// <summary>
        /// The addedToEmitter method is called by the emitter when the Action is added to it
        /// It is called within the emitter's addAction method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Action was added to.</param>
        public override void AddedToEmitter(Emitter emitter)
        {
            if (emitter == null)
            {
                return;
            }

            emitter.SpaceSort = true;
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

            List<Particle> particles = emitter.Particles;
            Particle other;
            double distanceSq;
            int i;
            int len = particles.Count;
            double dx;
            double dy;
            double velX = 0;
            double velY = 0;
            int count = 0;
            double factor;

            for (i = particles.IndexOf(particle) - 1; i >= 0; i--)
            {
                other = particles[i];

                if ((dx = particle.X - other.X) > m_max)
                    break;

                dy = other.Y - particle.Y;

                if (dy > m_max || dy < -m_max)
                    continue;

                distanceSq = dy * dy + dx * dx;

                if (distanceSq <= m_maxSq && distanceSq > 0)
                {
                    velX += other.VelocityX;
                    velY += other.VelocityY;
                    count++;
                }
            }
            for (i = particles.IndexOf(particle) + 1; i < len; i++)
            {
                other = particles[i];

                if ((dx = other.X - particle.X) > m_max)
                    break;

                dy = other.Y - particle.Y;

                if (dy > m_max || dy < -m_max)
                    continue;

                distanceSq = dy * dy + dx * dx;

                if (distanceSq <= m_maxSq && distanceSq > 0)
                {
                    velX += other.VelocityX;
                    velY += other.VelocityY;
                    count++;
                }
            }

            if (count != 0)
            {
                velX = velX / count - particle.VelocityX;
                velY = velY / count - particle.VelocityY;

                if (velX != 0 || velY != 0)
                {
                    factor = (double)(elapsedTime * m_acc / Math.Sqrt(velX * velX + velY * velY));

                    if (factor > 1)
                        factor = 1;

                    particle.VelocityX += factor * velX;
                    particle.VelocityY += factor * velY;
                }
            }
        }
    }
}
