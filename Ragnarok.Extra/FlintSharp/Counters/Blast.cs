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
    /// The Blast counter causes the emitter to emit a single burst of
    /// particles when it starts and then emit no further particles.
    /// It is used, for example, to simulate an explosion.
    /// </summary>
    public class Blast : ICounter
    {
        private double m_startMin;
        private double m_startMax;

        public Blast()
        {
            m_startMin = 100;
            m_startMax = double.NaN;
        }

        public Blast(double startMin)
        {
            m_startMin = startMin;
            m_startMax = double.NaN;
        }

        /// <summary>
        /// The constructor creates a Blast counter for use by an emitter. To
        /// add a Blast counter to an emitter use the emitter's counter property.
        /// <p>If two parameters are passed to the constructor then a random
        /// value between the two is used. This allows for some variation
        /// between emitters using the same Blast settings. Otherwise the 
        /// single value passed in is used.</p>
        /// </summary>
        /// <param name="startMin">The minimum number of particles to emit
        /// when the emitter starts.</param>
        /// <param name="startMax">The maximum number of particles to emit
        /// when the emitter starts. If not set then the emitter
        /// will emit exactly the startMin number of particles.</param>
        public Blast(double startMin, double startMax)
        {
            m_startMin = startMin;
            m_startMax = startMax;
        }

        /// <summary>
        /// The minimum number of particles to emit
        /// when the emitter starts.
        /// </summary>
        public double StartMin
        {
            get { return m_startMin; }
            set { m_startMin = value; }
        }

        /// <summary>
        /// The maximum number of particles to emit
        /// when the emitter starts.
        /// </summary>
        public double StartMax
        {
            get { return m_startMax; }
            set { m_startMax = value; }
        }

        /// <summary>
        /// When setting, this property sets both startMin and startMax to the same value.
        /// When reading, this property is the average of startMin and startMax.
        /// </summary>
        public double StartCount
        {
            get { return m_startMin == m_startMax ? m_startMin : (m_startMax + m_startMin) * 0.5f; }
            set { m_startMax = m_startMin = value; }
        }

        /// <summary>
        /// The startEmitter method is called when the emitter starts.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <returns>The number of particles the emitter should emit
        /// at the moment it starts.</returns>
        public int StartEmitter(Emitter emitter)
        {
            if (!double.IsNaN(m_startMax))
                return (int)Math.Round(Utils.RandomDouble(m_startMin, m_startMax));
            else
                return (int)Math.Round(m_startMin);
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
            return 0;
        }
    }
}
