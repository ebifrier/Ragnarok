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
    /// The Accelerate Action adjusts the velocity of the particle by a 
    /// constant acceleration. This can be used, for example, to simulate
    /// gravity.
    /// </summary>
    public class Accelerate : Behaviour
    {
        private double m_x;
        private double m_y;

        public Accelerate()
            : this(1.0, 1.0)
        {
        }

        /// <summary>
        /// The constructor creates an Acceleration action for use by 
        /// an emitter. To add an Accelerator to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// 
        /// </summary>
        /// <param name="accelerationX">The x coordinate of the acceleration to apply, in pixels
        /// per second per second.</param>
        /// <param name="accelerationY">The y coordinate of the acceleration to apply, in pixels
        /// per second per second.</param>
        public Accelerate(double accelerationX, double accelerationY)
        {
            m_x = accelerationX;
            m_y = accelerationY;
        }

        /// <summary>
        /// The x coordinate of the acceleration.
        /// </summary>
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        /// <summary>
        /// The y coordinate of the acceleration.
        /// </summary>
        public double Y
        {
            get { return m_x; }
            set { m_y = value; }
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
            particle.VelocityX += m_x * elapsedTime;
            particle.VelocityY += m_y * elapsedTime;
        }
    }
}
