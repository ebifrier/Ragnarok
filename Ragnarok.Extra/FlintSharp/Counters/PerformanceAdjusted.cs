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
using System.Collections.Generic;

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
    /// The PerformanceAdjusted counter causes the emitter to emit particles continuously
    /// at a steady rate. It then adjusts this rate downwards if the frame rate is below a 
    /// target frame rate.
    /// </summary>
    public class PerformanceAdjusted : ICounter
    {
        private double m_timeToNext;
        private double m_rateMin;
        private double m_rateMax;
        private double m_target;
        private double m_rate;
        private Stack<double> m_times;
        private double m_timeToRateCheck;
        private bool m_stop;

        /// <summary>
        /// The constructor creates a PerformanceAdjusted counter for use by an emitter. To
        /// add a PerformanceAdjusted counter to an emitter use the emitter's counter property.
        /// </summary>
        /// <param name="rateMin">The minimum number of particles to emit per second. The counter
        /// will never drop the rate below this value.</param>
        /// <param name="rateMax">The maximum number of particles to emit per second. the counter
        /// will start at this rate and adjust downwards if the frame rate is too slow.</param>
        /// <param name="targetFrameRate">The frame rate that the counter should aim for. Always set
        /// this slightly below your actual frame rate since flash will drop frames occasionally
        /// even when performance is fine. So, for example, if your movie's frame rate is
        /// 30fps and you want to target this rate, set the target rate to 26fps or so.</param>
        public PerformanceAdjusted(double rateMin, double rateMax, double targetFrameRate)
        {
            m_stop = false;
            m_rateMin = rateMin;
            m_rate = m_rateMax = rateMax;
            m_target = targetFrameRate;
            m_times = new Stack<double>();
            m_timeToRateCheck = 0;
        }

        /// <summary>
        /// The minimum number of particles to emit per second. The counter
        /// will never drop the rate below this value.
        /// </summary>
        public double RateMin
        {
            get { return m_rateMin; }
            set { m_rateMin = value; m_timeToRateCheck = 0; }
        }

        /// <summary>
        /// The maximum number of particles to emit per second. the counter
        /// will start at this rate and adjust downwards if the frame rate is too slow.
        /// </summary>
        public double RateMax
        {
            get { return m_rateMax; }
            set { m_rate = m_rateMax = value; m_timeToRateCheck = 0; }
        }

        /// <summary>
        /// The frame rate that the counter should aim for. Always set
        /// this slightly below your actual frame rate since flash will drop frames occasionally
        /// even when performance is fine. So, for example, if your movie's frame rate is
        /// 30fps and you want to target this rate, set the target rate to 26fps or so.
        /// </summary>
        public double TargetFrameRate
        {
            get { return m_target; }
            set { m_target = value; }
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
        /// The startEmitter method is called when the emitter starts.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <returns>The number of particles the emitter should emit
        /// at the moment it starts.</returns>
        public uint StartEmitter(Emitter emitter)
        {
            NewTimeToNext();
            return 0;
        }

        public void NewTimeToNext()
        {
            m_timeToNext = 1 / m_rate;
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

            if (m_rate > m_rateMin && (m_timeToRateCheck -= elapsedTime) <= 0)
            {
                double t = Utils.Timer.RunningTime;
                m_times.Push(t);

                if (t > 9)
                {
                    double frameRate = Math.Round(10000.0 / (t - m_times.Peek()));

                    if (frameRate < m_target)
                    {
                        m_rate = Math.Floor((m_rate + m_rateMin) * 0.5);
                        m_times.Clear();

                        if ((m_timeToRateCheck = emitter.Particles[0].Lifetime) == 0)
                            m_timeToRateCheck = 2;
                    }
                }
            }

            double emitTime = elapsedTime;
            uint count = 0;

            emitTime -= m_timeToNext;

            while (emitTime >= 0)
            {
                count++;
                NewTimeToNext();
                emitTime -= m_timeToNext;
            }

            m_timeToNext = -emitTime;

            return count;
        }
    }
}
