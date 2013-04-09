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
using System.Linq;

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
    /// The TextTimePeriod counter causes the emitter to emit particles for a period of time
    /// and then stop. The rate of emission over that period can be modified using
    /// easing equations that conform to the public interface defined in Robert Penner's easing
    /// equations. An update to these equations is included in the 
    /// org.flintparticles.easing package.
    /// 
    /// @see org.flintparticles.easing
    /// </summary>
    public class TextTimePeriod : TimePeriod
    {
        private TextZone m_zone;

        public TextTimePeriod()
            : this(null)
        {
        }

        public TextTimePeriod(TextZone zone)
        {
            Zone = zone;
        }

        /// <summary>
        /// A text zone which decides the particle count.
        /// </summary>
        public TextZone Zone
        {
            get { return m_zone; }
            set
            {
                m_zone = value;

                NumParticles =
                    ( m_zone != null
                    ? NumParticles = (uint)m_zone.Count()
                    : 0);
            }
        }
    }
}
