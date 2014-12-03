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
    /// The Steady counter causes the emitter to emit particles continuously
    /// at a steady rate. It can be used to simulate any continuous particle
    /// stream. The rate can also be varied by setting a range of value for the
    /// emission rate.
    /// </summary>
    public class Steady : ICounter
    {
        private double m_timeToNext;
        private double m_rateMin;
        private double m_rateMax;
        private bool m_stop;

        public Steady()
            : this(100)
        {
        }

        public Steady(double rateMin)
        {
            m_stop = false;
            m_rateMin = rateMin;
            m_rateMax = m_rateMin;
        }

        /// <summary>
        /// The constructor creates a Steady counter for use by an emitter. To
        /// add a Steady counter to an emitter use the emitter's counter property.
        /// <p>If two parameters are passed to the constructor then a random
        /// value between the two is used. This allows for random variation
        /// in the emission rate over the lifetime of the emitter. Otherwise the 
        /// single value passed in is used.</p>
        /// </summary>
        /// <param name="rateMin">The minimum number of particles to emit
        /// per second.</param>
        /// <param name="rateMax">The maximum number of particles to emit
        /// per second. If not set then the emitter
        /// will emit exactly the rateMin number of particles per second.</param>
        public Steady(double rateMin, double rateMax)
        {
            m_stop = false;
            m_rateMin = rateMin;
            m_rateMax = rateMax;
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
        /// The minimum number of particles to emit per second.
        /// </summary>
        public double RateMin
        {
            get { return m_rateMin; }
            set { m_rateMin = value; }
        }

        /// <summary>
        /// The maximum number of particles to emit per second.
        /// </summary>
        public double RateMax
        {
            get { return m_rateMax; }
            set { m_rateMax = value; }
        }

        /// <summary>
        /// When setting, this property sets both rateMin and rateMax to the same value.
        /// When reading, this property is the average of rateMin and rateMax.
        /// </summary>
        public double Rate
        {
            get { return m_rateMin == m_rateMax ? m_rateMin : (m_rateMax + m_rateMin) * 0.5f; }
            set { m_rateMax = m_rateMin = value; }
        }

        /// <summary>
        /// The startEmitter method is called when the emitter starts.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <returns>The number of particles the emitter should emit
        /// at the moment it starts.</returns>
        public int StartEmitter(Emitter emitter)
        {
            m_timeToNext = NewTimeToNext();

            return 0;
        }

        private double NewTimeToNext()
        {
            double newRate = ((m_rateMin == m_rateMax) ? m_rateMin : m_rateMin + Utils.Random.NextDouble() * (m_rateMax - m_rateMin));

            return 1 / newRate;
        }

        /// <summary>
        /// The updateEmitter method is called every frame after the
        /// emitter has started.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <param name="elapsedTime">The time, in seconds, since the previous call to this method.</param>
        /// <returns>The number of particles the emitter should emit
        /// at this time.</returns>
        public int UpdateEmitter(Emitter emitter, double elapsedTime)
        {
            if (m_stop)
                return 0;

            int count = 0;

            m_timeToNext -= elapsedTime;

            while (m_timeToNext <= 0)
            {
                count++;
                m_timeToNext += NewTimeToNext();
            }

            return count;
        }
    }
}
