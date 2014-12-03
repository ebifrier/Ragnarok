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

namespace FlintSharp.Counters
{
    /// <summary>
    /// The Sine counter causes the emitter to emit particles continuously
    /// at a rate that varies according to a sine wave.
    /// </summary>
    public class SineCounter : ICounter
    {
        private double m_emitted;
        private double m_rateMin;
        private double m_rateMax;
        private double m_period;
        private bool m_stop;
        private double m_lastTimeUpdate;
        private double m_factor;
        private double m_scale;

        public SineCounter()
            : this(1.0, 100)
        {
        }

        public SineCounter(double period, double rateMax)
        {
            m_stop = false;
            m_period = period;
            m_rateMin = 0;
            m_rateMax = rateMax;
            m_factor = (2 * Math.PI / period);
            m_scale = (0.5 * (m_rateMax - m_rateMin));
        }

        /// <summary>
        /// The constructor creates a SineCounter counter for use by an emitter. To
        /// add a SineCounter counter to an emitter use the emitter's counter property.
        /// </summary>
        /// <param name="period">The period of the sine wave used, in seconds.</param>
        /// <param name="rateMax">The number of particles emitted per second at the peak of
        /// the sine wave.</param>
        /// <param name="rateMin">The number of particles to emit per second at the bottom
        /// of the sine wave.</param>
        public SineCounter(double period, double rateMax, double rateMin)
        {
            m_stop = false;
            m_period = period;
            m_rateMin = rateMin;
            m_rateMax = rateMax;
            m_factor = (2 * Math.PI / period);
            m_scale = (0.5 * (m_rateMax - m_rateMin));
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
            m_emitted = 0;
        }

        /// <summary>
        /// The number of particles to emit per second at the bottom
        /// of the sine wave.
        /// </summary>
        public double RateMin
        {
            get { return m_rateMin; }
            set { m_rateMin = value; m_scale = (0.5 * (m_rateMax - m_rateMin)); }
        }

        /// <summary>
        /// The number of particles emitted per second at the peak of
        /// the sine wave.
        /// </summary>
        public double RateMax
        {
            get { return m_rateMax; }
            set { m_rateMax = value; m_scale = (0.5 * (m_rateMax - m_rateMin)); }
        }

        /// <summary>
        /// The period of the sine wave used, in seconds.
        /// </summary>
        public double Period
        {
            get { return m_period; }
            set { m_period = value; m_factor = (2 * Math.PI / m_period); }
        }

        /// <summary>
        /// The startEmitter method is called when the emitter starts.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <returns>The number of particles the emitter should emit
        /// at the moment it starts.</returns>
        public int StartEmitter(Emitter emitter)
        {
            m_lastTimeUpdate = 0;
            m_emitted = 0;

            return 0;
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

            m_lastTimeUpdate += elapsedTime;

            int count = (int)Math.Floor(m_rateMax * m_lastTimeUpdate + m_scale * (1 - Math.Cos(m_lastTimeUpdate * m_factor)) / m_factor);
            int ret = (int)(count - m_emitted);

            m_emitted = count;

            return ret;
        }
    }
}
