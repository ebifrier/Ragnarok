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
    /// The AntiGravity action applies a force on the particle to push it away from
    /// a single point - the center of the effect. The force applied is inversely 
    /// proportional to the square of the distance from the particle to the point.
    /// </summary>
    public class AntiGravity : Behaviour
    {
        private double m_x;
        private double m_y;
        private double m_power;
        private double m_gravityConst = 10000; // this just scales the power so we don't have to use very large numbers
        private double m_epsilonSq;

        public AntiGravity()
            : this(1.0, 1.0, 1.0)
        {
        }

        public AntiGravity(double power, double x, double y)
        {
            m_power = power * m_gravityConst;
            m_x = x;
            m_y = y;
            m_epsilonSq = 1;
        }

        /// <summary>
        /// The constructor creates an AntiGravity action for use by 
        /// an emitter. To add an AntiGravity to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="power">The strength of the force - larger numbers produce a stronger force.</param>
        /// <param name="x">The x coordinate of the point away from which the force pushes the particles.</param>
        /// <param name="y">The y coordinate of the point away from which the force pushes the particles.</param>
        /// <param name="epsilon">The minimum distance for which the anti-gravity force is calculated. 
        /// Particles closer than this distance experience the anti-gravity as it they were 
        /// this distance away. This stops the anti-gravity effect blowing up as distances get 
        /// small.</param>
        public AntiGravity(double power, double x, double y, double epsilon)
        {
            m_power = power * m_gravityConst;
            m_x = x;
            m_y = y;
            m_epsilonSq = epsilon * epsilon;
        }

        /// <summary>
        /// The strength of the anti-gravity force.
        /// </summary>
        public double Power
        {
            get { return m_power / m_gravityConst; }
            set { m_power = value * m_gravityConst; }
        }

        /// <summary>
        /// The x coordinate of the center of the anti-gravity force.
        /// </summary>
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        /// <summary>
        /// The y coordinate of the center of the anti-gravity force.
        /// </summary>
        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        /// <summary>
        /// The minimum distance for which the anti-gravity force is calculated. 
        /// Particles closer than this distance experience the anti-gravity as it they were 
        /// this distance away. This stops the anti-gravity effect blowing up as distances get 
        /// small.
        /// </summary>
        public double Epsilon
        {
            get { return Math.Sqrt(m_epsilonSq); }
            set { m_epsilonSq = value * value; }
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

            double x = particle.X - m_x;
            double y = particle.Y - m_y;
            double dSq = x * x + y * y;

            if (dSq == 0)
                return;

            double d = Math.Sqrt(dSq);

            if (dSq < m_epsilonSq)
                dSq = m_epsilonSq;

            double factor = (m_power * elapsedTime) / (dSq * d);

            particle.VelocityX += x * factor;
            particle.VelocityY += y * factor;
        }
    }
}
