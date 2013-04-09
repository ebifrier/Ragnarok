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
    /// The TargetRotateVelocity action adjusts the angular velocity of the particle towards the target angular velocity.
    /// </summary>
    public class TargetRotateVelocity : Behaviour
    {
        private double m_vel;
        private double m_rate;

        public TargetRotateVelocity()
            : this(1.0)
        {
        }

        public TargetRotateVelocity(double targetVelocity)
        {
            m_vel = targetVelocity;
            m_rate = 0.1f;
        }

        /// <summary>
        /// The constructor creates a TargetRotateVelocity action for use by 
        /// an emitter. To add a TargetRotateVelocity to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="targetVelocity">The target angular velocity, in radians per second.</param>
        /// <param name="rate">Adjusts how quickly the particle reaches the target angular velocity.
        /// Larger numbers cause it to approach the target angular velocity more quickly.</param>
        public TargetRotateVelocity(double targetVelocity, double rate)
        {
            m_vel = targetVelocity;
            m_rate = rate;
        }

        /// <summary>
        /// The target angular velocity, in radians per second.
        /// </summary>
        public double TargetVelocity
        {
            get { return m_vel; }
            set { m_vel = value; }
        }

        /// <summary>
        /// Adjusts how quickly the particle reaches the target angular velocity.
        /// Larger numbers cause it to approach the target angular velocity more quickly.
        /// </summary>
        public double Rate
        {
            get { return m_rate; }
            set { m_rate = value; }
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
            particle.AngularVelocity += (m_vel - particle.AngularVelocity) * m_rate * elapsedTime;
        }
    }
}
