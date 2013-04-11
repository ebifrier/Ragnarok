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
    /// The TurnTowardsTarget action causes the particle to constantly adjust its direction
    /// so that it travels towards a particular point.
    /// </summary>
    public class TurnTowardsTarget : Behaviour
    {
        private double m_power;

        public TurnTowardsTarget()
            : this(1.0)
        {
        }

        /// <summary>
        /// The constructor creates a TurnTowardsPoint action for use by 
        /// an emitter. To add a TurnTowardsPoint to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="power">The strength of the turn action. Higher values produce a sharper turn.</param>
        public TurnTowardsTarget(double power)
        {
            m_power = Math.Abs(power);
        }

        /// <summary>
        /// The strength of theturn action. Higher values produce a sharper turn.
        /// </summary>
        public double Power
        {
            get { return m_power; }
            set { m_power = Math.Abs(value); }
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
            bool turnLeft = ((particle.Y - particle.TargetY) * particle.VelocityX + (particle.TargetX - particle.X) * particle.VelocityY > 0);
            double newAngle =
                Math.Atan2(particle.VelocityY, particle.VelocityX) +
                (turnLeft ? -m_power : m_power) * elapsedTime;

            double len = Math.Sqrt(particle.VelocityX * particle.VelocityX + particle.VelocityY * particle.VelocityY);
            particle.VelocityX = (len * Math.Cos(newAngle));
            particle.VelocityY = (len * Math.Sin(newAngle));
        }
    }
}
