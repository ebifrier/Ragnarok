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
    /// The MutualGravity Action applies forces to attract each particle towards the other particles.
    /// </summary>
    public class MutualGravity : Behaviour
    {
        private double m_power;
        private double m_maxDistance;
        private double m_maxDistanceSq;
        private double m_epsilonSq;
        private double m_gravityConst = 1000;

        public MutualGravity()
            : this(1.0, 10.0)
        {
        }

        public MutualGravity(double power, double maxDistance)
        {
            m_power = power * m_gravityConst;
            m_maxDistance = maxDistance;
            m_maxDistanceSq = maxDistance * maxDistance;
            m_epsilonSq = 1;
        }

        /// <summary>
        /// The constructor creates a MutualGravity action for use by 
        /// an emitter. To add a MutualGravity to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="power">The strength of the gravitational pull between the particles.</param>
        /// <param name="maxDistance">The maximum distance between particles for the gravitational
        /// effect to be calculated. You can speed up this action by reducing the maxDistance
        /// since often only the closest other particles have a significant effect on the 
        /// motion of a particle.</param>
        /// <param name="epsilon">The minimum distance for which the gravity force is calculated. 
        /// Particles closer than this distance experience the gravity as it they were 
        /// this distance away. This stops the gravity effect blowing up as distances get 
        /// small.</param>
        public MutualGravity(double power, double maxDistance, double epsilon)
        {
            m_power = power * m_gravityConst;
            m_maxDistance = maxDistance;
            m_maxDistanceSq = maxDistance * maxDistance;
            m_epsilonSq = epsilon * epsilon;
        }

        /// <summary>
        /// The strength of the gravity force.
        /// </summary>
        public double Power
        {
            get { return m_power / m_gravityConst; }
            set { m_power = value * m_gravityConst; }
        }

        /// <summary>
        /// The maximum distance between particles for the gravitational
        /// effect to be calculated. You can speed up this action by reducing the maxDistance
        /// since often only the closest other particles have a significant effect on the 
        /// motion of a particle.
        /// </summary>
        public double MaxDistance
        {
            get { return m_maxDistance; }
            set { m_maxDistance = value; m_maxDistanceSq = value * value; }
        }

        /// <summary>
        /// The minimum distance for which the gravity force is calculated. 
        /// Particles closer than this distance experience the gravity as it they were 
        /// this distance away. This stops the gravity effect blowing up as distances get 
        /// small.
        /// </summary>
        public double Epsilon
        {
            get { return Math.Sqrt(m_epsilonSq); }
            set { m_epsilonSq = value * value; }
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
            List<Particle> particles = emitter.Particles;
            Particle other;
            int i;
            int len = particles.Count;
            double factor;
            double distance;
            double distanceSq;
            double dx;
            double dy;

            for (i = particles.IndexOf(particle) + 1; i < len; i++)
            {
                other = particles[i];
                dx = other.X - particle.X;

                if (dx > m_maxDistance)
                    break;

                dy = other.Y - particle.Y;

                if (dy > m_maxDistance || dy < -m_maxDistance)
                    continue;

                distanceSq = dy * dy + dx * dx;

                if (distanceSq <= m_maxDistanceSq && distanceSq > 0)
                {
                    distance = Math.Sqrt(distanceSq);

                    if (distanceSq < m_epsilonSq)
                        distanceSq = m_epsilonSq;

                    factor = (m_power * elapsedTime) / (distanceSq * distance);

                    particle.VelocityX += (dx *= factor);
                    particle.VelocityY += (dy *= factor);

                    other.VelocityX -= dx;
                    other.VelocityY -= dy;
                }
            }
        }
    }
}
