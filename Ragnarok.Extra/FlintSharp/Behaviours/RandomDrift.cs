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
    /// The RandomDrift action moves the particle by a random small amount every frame,
    /// causing the particle to drift around.
    /// </summary>
    public class RandomDrift : Behaviour
    {
        private double m_sizeX;
        private double m_sizeY;

        public RandomDrift()
            : this(0, 0)
        {
        }

        /// <summary>
        /// The constructor creates a RandomDrift action for use by 
        /// an emitter. To add a RandomDrift to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="sizeX">The maximum amount of horizontal drift in pixels per second.</param>
        /// <param name="sizeY">The maximum amount of vertical drift in pixels per second.</param>
        public RandomDrift(double sizeX, double sizeY)
        {
            m_sizeX = sizeX * 2;
            m_sizeY = sizeY * 2;
        }

        /// <summary>
        /// The maximum amount of horizontal drift in pixels per second.
        /// </summary>
        public double DriftX
        {
            get { return m_sizeX / 2; }
            set { m_sizeX = value * 2; }
        }

        /// <summary>
        /// The maximum amount of vertical drift in pixels per second.
        /// </summary>
        public double DriftY
        {
            get { return m_sizeY / 2; }
            set { m_sizeY = value * 2; }
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

            particle.VelocityX += ((Utils.Random.NextDouble() - 0.5) * m_sizeX * elapsedTime);
            particle.VelocityY += ((Utils.Random.NextDouble() - 0.5) * m_sizeY * elapsedTime);
        }
    }
}
