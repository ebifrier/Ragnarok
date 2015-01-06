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
using System.Windows.Media;

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.EnergyEasing;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

using Ragnarok.Utility;

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The ColorChange action alters the color of the particle as it ages.
    /// It should be used in conjunction with the Age action.
    /// </summary>
    public class ColorChange : Behaviour
    {
        private Color4b m_startColor;
        private Color4b m_endColor;

        public ColorChange()
            : this(Color4bs.Black, Color4bs.White)
        {
        }

        /// <summary>
        /// The constructor creates a ColorChange action for use by 
        /// an emitter. To add a ColorChange to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        public ColorChange(Color4b startColor, Color4b endColor)
        {
            m_startColor = startColor;
            m_endColor = endColor;
        }

        /// <summary>
        /// The constructor creates a ColorChange action for use by 
        /// an emitter. To add a ColorChange to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="startColor">The 32bit (ARGB) color of the particle at the beginning of its life.</param>
        /// <param name="endColor">The 32bit (ARGB) color of the particle at the end of its life.</param>
        [CLSCompliant(false)]
        public ColorChange(uint startColor, uint endColor)
        {
            m_startColor = Color4b.FromValue(startColor);
            m_endColor = Color4b.FromValue(endColor);
        }

        /// <summary>
        /// The color of the particle at the beginning of its life.
        /// </summary>
        public Color4b StartColor
        {
            get { return m_startColor; }
            set { m_startColor = value; }
        }

        /// <summary>
        /// The color of the particle at the end of its life.
        /// </summary>
        public Color4b EndColor
        {
            get { return m_endColor; }
            set { m_endColor = value; }
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
            particle.Color = Utils.InterpolateColors(m_startColor, m_endColor, particle.Energy);
        }
    }
}
