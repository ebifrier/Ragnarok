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
    /// The Scale action adjusts the size of the particle as it ages. This action
    /// should be used in conjunction with the Age action.
    /// </summary>
    public class Scale : Behaviour
    {
        private double m_diffScale;
        private double m_endScale;

        public Scale()
        {
            m_diffScale = 0;
            m_endScale = 1;
        }

        /// <summary>
        /// The constructor creates a Scale action for use by 
        /// an emitter. To add a Scale to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="startScale">The scale factor for the particle at the start of it's life.
        /// 1 is normal size.</param>
        /// <param name="endScale">The scale factor for the particle at the end of it's life.
        /// 1 is normal size.</param>
        public Scale(double startScale, double endScale)
        {
            m_diffScale = startScale - endScale;
            m_endScale = endScale;
        }

        /// <summary>
        /// The alpha value for the particle at the start of its life.
        /// Should be between 0 and 1.
        /// </summary>
        public double StartScale
        {
            get { return m_endScale + m_diffScale; }
            set { m_diffScale = value - m_endScale; }
        }

        /// <summary>
        /// The alpha value for the particle at the end of its life.
        /// Should be between 0 and 1.
        /// </summary>
        public double EndScale
        {
            get { return m_endScale; }
            set { m_diffScale = m_endScale + m_diffScale - value; m_endScale = value; }
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
            particle.Scale = m_endScale + m_diffScale * particle.Energy;
        }
    }
}
