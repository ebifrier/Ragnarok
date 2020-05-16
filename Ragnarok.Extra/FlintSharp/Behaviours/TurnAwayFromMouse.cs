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
    /// The TurnAwayFromMouse action causes the particle to constantly adjust its direction
    /// so that it travels away from the mouse pointer.
    /// </summary>
    public class TurnAwayFromMouse : Behaviour
    {
        private double m_power;

        public TurnAwayFromMouse()
            : this(1.0)
        {
        }

        /// <summary>
        /// The constructor creates a TurnAwayFromMouse action for use by 
        /// an emitter. To add a TurnAwayFromMouse to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="power">The strength of the turn action. Higher values produce a sharper turn.</param>
        public TurnAwayFromMouse(double power)
        {
            m_power = power;
        }

        /// <summary>
        ///  The strength of the turn action. Higher values produce a sharper turn
        /// </summary>
        public double Power
        {
            get { return m_power; }
            set { m_power = value; }
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

            bool turnLeft = ((particle.Y - Utils.MousePos.Y) * particle.VelocityX + (Utils.MousePos.X - particle.X) * particle.VelocityY > 0);
            double newAngle;

            if (turnLeft)
                newAngle = (Math.Atan2(particle.VelocityY, particle.VelocityX) + m_power * elapsedTime);
            else
                newAngle = (Math.Atan2(particle.VelocityY, particle.VelocityX) - m_power * elapsedTime);

            double len = Math.Sqrt(particle.VelocityX * particle.VelocityX + particle.VelocityY * particle.VelocityY);

            particle.VelocityX = (len * Math.Cos(newAngle));
            particle.VelocityY = (len * Math.Sin(newAngle));
        }
    }
}
