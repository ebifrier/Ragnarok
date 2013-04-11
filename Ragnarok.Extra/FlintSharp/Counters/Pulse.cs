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

namespace FlintSharp.Counters
{
    /// <summary>
    /// The Pulse counter causes the emitter to emit pulses of particles at a regular
    /// interval.
    /// </summary>
    public class Pulse : ICounter
    {
        private double m_timeToNext;
        private double m_period;
        private uint m_quantity;
        private bool m_stop;

        public Pulse()
            : this(1.0, 100)
        {
        }

        /// <summary>
        /// The constructor creates a Pulse counter for use by an emitter. To
        /// add a Pulse counter to an emitter use the emitter's counter property.
        /// </summary>
        /// <param name="period">The time, in seconds, between each pulse.</param>
        /// <param name="quantity">The number of particles to emit at each pulse.</param>
        public Pulse(double period, uint quantity)
        {
            m_stop = false;
            m_quantity = quantity;
            m_period = period;
        }

        /// <summary>
        /// Stops the emitter from emitting particles
        /// </summary>
        public void Stop()
        {
            m_stop = true;
        }

        /// <summary>
        /// Resumes the emitter after a stop
        /// </summary>
        public void Resume()
        {
            m_stop = false;
        }

        /// <summary>
        /// The time, in seconds, between each pulse.
        /// </summary>
        public double Period
        {
            get { return m_period; }
            set { m_period = value; }
        }

        /// <summary>
        /// The number of particles to emit at each pulse.
        /// </summary>
        public uint Quantity
        {
            get { return m_quantity; }
            set { m_quantity = value; }
        }

        /// <summary>
        /// The startEmitter method is called when the emitter starts.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <returns>The number of particles the emitter should emit
        /// at the moment it starts.</returns>
        public uint StartEmitter(Emitter emitter)
        {
            m_timeToNext = m_period;
            return m_quantity;
        }

        /// <summary>
        /// The updateEmitter method is called every frame after the
        /// emitter has started.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <param name="elapsedTime">The time, in seconds, since the previous call to this method.</param>
        /// <returns>The number of particles the emitter should emit
        /// at this time.</returns>
        public uint UpdateEmitter(Emitter emitter, double elapsedTime)
        {
            if (m_stop)
                return 0;

            uint count = 0;

            m_timeToNext -= elapsedTime;

            while (m_timeToNext <= 0)
            {
                count += m_quantity;
                m_timeToNext += m_period;
            }

            return count;
        }
    }
}
