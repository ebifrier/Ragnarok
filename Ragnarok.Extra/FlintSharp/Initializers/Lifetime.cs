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

namespace FlintSharp.Initializers
{
    public class Lifetime : Initializer
    {
        private double m_min;
        private double m_max;

        public Lifetime()
            : this(1.0)
        {
        }

        public Lifetime(double minLifetime)
        {
            m_min = minLifetime;
            m_max = double.NaN;
        }

        public Lifetime(double minLifetime, double maxLifetime)
        {
            m_min = minLifetime;
            m_max = maxLifetime;
        }

        public double MinLifetime
        {
            get { return m_min; }
            set { m_min = value; }
        }

        public double MaxLifetime
        {
            get { return m_max; }
            set { m_max = value; }
        }

        public double Value
        {
            get { return m_min == m_max ? m_min : (m_max + m_min) * 0.5f; }
            set { m_max = m_min = value; }
        }

        public override void Initialize(Emitter emitter, Particle particle)
        {
            if (particle == null)
            {
                return;
            }

            if (double.IsNaN(m_max))
                particle.Lifetime = m_min;
            else
                particle.Lifetime = Utils.RandomDouble(m_min, m_max);
        }
    }
}
