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
    /// The SpeedLimit action limits the particle's maximum speed to the specified
    /// speed. The behaviour can be switched to instead limit the minimum speed to
    /// the specified speed.
    /// </summary>
    public class SpeedLimit : Behaviour
    {
        private double m_limit;
        private double m_limitSq;
        private bool m_isMinimum;

        public SpeedLimit()
            : this(1.0)
        {
        }

        public SpeedLimit(double speed)
        {
            m_limit = speed;
            m_limitSq = speed * speed;
            m_isMinimum = false;
        }

        /// <summary>
        /// The constructor creates a SpeedLimit action for use by 
        /// an emitter. To add a SpeedLimit to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="speed">The speed limit for the action in pixels per second.</param>
        /// <param name="isMinimum">If true, particles travelling slower than the speed limit
        /// are accelerated to the speed limit, otherwise particles travelling faster
        /// than the speed limit are decelerated to the speed limit.</param>
        public SpeedLimit(double speed, bool isMinimum)
        {
            m_limit = speed;
            m_limitSq = speed * speed;
            m_isMinimum = isMinimum;
        }

        /// <summary>
        /// The speed limit
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
        /// The getDefaultPriority method is used to order the execution of actions.
        /// It is called within the emitter's addAction method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.common.actions.Action#getDefaultPriority()
        /// </summary>
        /// <returns><p>Returns a value of -5, so that the SpeedLimit executes after all accelerations have occured.</p></returns>
        public override int GetDefaultPriority()
        {
            return -5;
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
            if (particle == null)
            {
                return;
            }

            double speedSq = particle.VelocityX * particle.VelocityX + particle.VelocityY * particle.VelocityY;

            if ((m_isMinimum && speedSq < m_limitSq) || (!m_isMinimum && speedSq > m_limitSq))
            {
                double scale = (m_limit / Math.Sqrt(speedSq));

                particle.VelocityX *= scale;
                particle.VelocityY *= scale;
            }
        }
    }
}
