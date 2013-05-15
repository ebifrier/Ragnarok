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
    /// The TurnTowardsPoint action causes the particle to constantly adjust its direction
    /// so that it travels towards a particular point.
    /// </summary>
    public class MissileTowardsTarget : Behaviour
    {
        private double m_power;
        private double m_torelantAngle;
        private double m_maxSpeed;
        private double m_minSpeed;

        public MissileTowardsTarget()
            : this(1.0, 180, 1.0, 1.0)
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
        /// <param name="torelantAngle">The torelance angle of the particle.</param>
        /// <param name="maxSpeed">The maximum speed of the particle.</param>
        /// <param name="minSpeed">The maximum speed of the particle.</param>
        public MissileTowardsTarget(double power, double torelantAngle, double maxSpeed, double minSpeed)
        {
            m_power = Math.Abs(power);
            m_torelantAngle = Utils.DegreesToRadians(torelantAngle);
            m_maxSpeed = maxSpeed;
            m_minSpeed = minSpeed;
        }

        /// <summary>
        /// The strength of the turn action. Higher values produce a sharper turn.
        /// </summary>
        public double Power
        {
            get { return m_power; }
            set { m_power = Math.Abs(value); }
        }

        /// <summary>
        /// The torelance angle of the particle. (degree)
        /// </summary>
        public double TorelantAngle
        {
            get { return m_torelantAngle; }
            set { m_torelantAngle = Utils.DegreesToRadians(value); }
        }

        /// <summary>
        /// The maximum speed of the particle.
        /// </summary>
        public double MaxSpeed
        {
            get { return m_maxSpeed; }
            set { m_maxSpeed = value; }
        }

        /// <summary>
        /// The minimum speed of the particle.
        /// </summary>
        public double MinSpeed
        {
            get { return m_minSpeed; }
            set { m_minSpeed = value; }
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
            double currentAngle = Math.Atan2(particle.VelocityY, particle.VelocityX);
            double targetAngle = Math.Atan2(particle.TargetY - particle.Y, particle.TargetX - particle.X);

            if (currentAngle - Math.PI > targetAngle)
            {
                targetAngle += Math.PI * 2;
            }
            if (currentAngle + Math.PI < targetAngle)
            {
                targetAngle -= Math.PI * 2;
            }
            //c * t / d + b;
            double diffAngle = targetAngle - currentAngle;
            double velAngle =
                Math.Min(Math.Abs(diffAngle), m_power * elapsedTime) *
                Math.Sign(diffAngle);

            double rate = Math.Max(0.0, m_torelantAngle - Math.Abs(diffAngle));
            double length = Easing.Linear.EaseNone(rate, m_minSpeed, m_maxSpeed - m_minSpeed, m_torelantAngle);

            double newAngle = currentAngle + velAngle;
            particle.VelocityX = length * Math.Cos(newAngle);
            particle.VelocityY = length * Math.Sin(newAngle);
        }
    }
}
