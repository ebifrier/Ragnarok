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
    /// The TimePeriod counter causes the emitter to emit particles for a period of time
    /// and then stop. The rate of emission over that period can be modified using
    /// easing equations that conform to the public interface defined in Robert Penner's easing
    /// equations. An update to these equations is included in the 
    /// org.flintparticles.easing package.
    /// 
    /// @see org.flintparticles.easing
    /// </summary>
    public class TimePeriod : ICounter
    {
        private uint m_particles;
        private double m_duration;
        private uint m_particlesPassed;
        private double m_lastTimeUpdate;
        private EaseCategory m_category = EaseCategory.Linear;
        private EaseType m_easeType = EaseType.None;
        private EasingDelegate m_easingFunction;

        public TimePeriod()
            : this(100, 1.0)
        {
        }

        public TimePeriod(uint numParticles, double duration)
        {
            m_particles = numParticles;
            m_duration = duration;

            UpdateFunction();
        }

        /// <summary>
        /// The constructor creates a TimePeriod counter for use by an emitter. To
        /// add a TimePeriod counter to an emitter use the emitter's counter property.
        /// </summary>
        /// <param name="numParticles">The number of particles to emit over the full duration
        /// of the time period</param>
        /// <param name="duration">The duration of the time period. After this time is up the
        /// emitter will not release any more particles.</param>
        /// <param name="easeCategory">An easing category used to distribute the emission of the
        /// particles over the time period. If no easing function is passed a simple
        /// linear distribution is used in which particles are emitted at a constant
        /// rate over the time period.</param>
        /// <param name="easeType">An easing type used to distribute the emission of the
        /// particles over the time period. If no easing function is passed a simple
        /// linear distribution is used in which particles are emitted at a constant
        /// rate over the time period.</param>
        public TimePeriod(uint numParticles, double duration, EaseCategory easeCategory, EaseType easeType)
        {
            m_particles = numParticles;
            m_duration = duration;

            Category = easeCategory;
            EaseType = easeType;
        }

        /// <summary>
        /// The number of particles to emit over the full duration
        /// of the time period.
        /// </summary>
        public uint NumParticles
        {
            get { return m_particles; }
            set { m_particles = value; }
        }

        /// <summary>
        /// The duration of the time period. After this time is up the
        /// emitter will not release any more particles.
        /// </summary>
        public double Duration
        {
            get { return m_duration; }
            set { m_duration = value; }
        }

        /// <summary>
        /// An easing function category used to distribute the emission of the
        /// particles over the time period.
        /// </summary>
        public EaseCategory Category
        {
            get { return m_category; }
            set { m_category = value; UpdateFunction(); }
        }

        /// <summary>
        /// An easing function type used to distribute the emission of the
        /// particles over the time period.
        /// </summary>
        public EaseType EaseType
        {
            get { return m_easeType; }
            set { m_easeType = value; UpdateFunction(); }
        }

        /// <summary>
        /// An easing function used to distribute the emission of the
        /// particles over the time period.
        /// </summary>
        public EasingDelegate EasingFunction
        {
            get { return m_easingFunction; }
            set { m_easingFunction = value; }
        }

        /// <summary>
        /// Updates an easing function.
        /// </summary>
        private void UpdateFunction()
        {
            m_easingFunction = Easing.EasingFunction.GetEasingFunction(
                m_category, m_easeType);
        }

        /// <summary>
        /// The startEmitter method is called when the emitter starts.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <returns>The number of particles the emitter should emit
        /// at the moment it starts.</returns>
        public uint StartEmitter(Emitter emitter)
        {
            m_particlesPassed = 0;
            m_lastTimeUpdate = 0;

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
        public uint UpdateEmitter(Emitter emitter, double elapsedTime)
        {
            if (m_particlesPassed == m_particles)
                return 0;

            m_lastTimeUpdate += elapsedTime;

            if (m_lastTimeUpdate >= m_duration)
            {
                uint newParticles = m_particles - m_particlesPassed;

                m_particlesPassed = m_particles;

                return newParticles;
            }

            uint oldParticles = m_particlesPassed;

            m_particlesPassed = (uint)Math.Round(m_easingFunction(m_lastTimeUpdate, 0, m_particles, m_duration));

            return m_particlesPassed - oldParticles;
        }
    }
}
