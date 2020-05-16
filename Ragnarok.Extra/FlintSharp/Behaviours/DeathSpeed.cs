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

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The DeathSpeed action marks the particle as dead if it is travelling faster than 
    /// the specified speed. The behaviour can be switched to instead mark as dead 
    /// particles travelling slower than the specified speed.
    /// </summary>
    public class DeathSpeed : Behaviour
    {
        private double m_limit;
        private double m_limitSq;
        private bool m_isMinimum;

        public DeathSpeed()
            : this(10.0)
        {
        }

        public DeathSpeed(double speed)
        {
            m_limit = speed;
            m_limitSq = speed * speed;
            m_isMinimum = false;
        }

        /// <summary>
        /// The constructor creates a DeathSpeed action for use by 
        /// an emitter. To add a DeathSpeed to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="speed">The speed limit for the action in pixels per second.</param>
        /// <param name="isMinimum">If true, particles travelling slower than the speed limit
        /// are killed, otherwise particles travelling faster than the speed limit are
        /// killed.</param>
        public DeathSpeed(double speed, bool isMinimum)
        {
            m_limit = speed;
            m_limitSq = speed * speed;
            m_isMinimum = isMinimum;
        }

        /// <summary>
        /// The speed limit beyond which the particle dies
        /// </summary>
        public double Limit
        {
            get { return m_limit; }
            set { m_limit = value; m_limitSq = value * value; }
        }

        /// <summary>
        /// Whether the speed is a minimum (true) or maximum (false) speed.
        /// </summary>
        public bool IsMinimum
        {
            get { return m_isMinimum; }
            set { m_isMinimum = value; }
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

            double speedSq = particle.VelocityX * particle.VelocityX + particle.VelocityY * particle.VelocityY;

            if ((m_isMinimum && speedSq < m_limitSq) || (!m_isMinimum && speedSq > m_limitSq))
            {
                if (speedSq > m_limitSq)
                    particle.IsDead = true;
            }
        }
    }
}
