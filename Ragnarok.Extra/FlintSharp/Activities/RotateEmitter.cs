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

namespace FlintSharp.Activities
{
    /// <summary>
    /// The RotateEmitter activity rotates the emitter at a constant rate.
    /// </summary>
    public class RotateEmitter : Activity
    {
        private double m_angularVelocity;

        public RotateEmitter()
            : this(0.0)
        {
        }

        /// <summary>
        /// The constructor creates a RotateEmitter activity for use by 
        /// an emitter. To add a RotateEmitter to an emitter, use the
        /// emitter's addActvity method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addActivity()
        /// </summary>
        /// <param name="angularVelocity">The angular velocity for the emitter in 
        /// radians per second.</param>
        public RotateEmitter(double angularVelocity)
        {
            m_angularVelocity = angularVelocity;
        }

        /// <summary>
        /// The angular velocity for the emitter in 
        /// radians per second.
        /// </summary>
        public double AngularVelocity
        {
            get { return m_angularVelocity; }
            set { m_angularVelocity = value; }
        }

        /// <summary>
        /// The update method is used by the emitter to apply the activity.
        /// It is called within the emitter's update loop and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that is using the activity.</param>
        /// <param name="elapsedTime">The duration of the frame - used for time based updates.</param>
        public override void Update(Emitter emitter, double elapsedTime)
        {
            emitter.RotRadians += m_angularVelocity * elapsedTime;
        }
    }
}
