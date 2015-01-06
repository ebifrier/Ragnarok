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

using Ragnarok.Utility;

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The ColorRotate action alters the color of the particle as it ages.
    /// It should be used in conjunction with the Age action.
    /// </summary>
    public class ColorRotate : Behaviour
    {
        //private Color4b m_startColor;
        //private Color4b m_endColor;
        private int m_intensityStart;
        private int m_intensityEnd;

        private double m_colorIndex;
        private double m_lastTime;

        public ColorRotate()
            : this(0, 255)
        {
        }

        /// <summary>
        /// The constructor creates a ColorRotate action for use by 
        /// an emitter. To add a ColorRotate to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="startColor">The 32bit (ARGB) color of the particle at the beginning of its life.</param>
        /// <param name="endColor">The 32bit (ARGB) color of the particle at the end of its life.</param>
        public ColorRotate(int intensityStart, int intensityEnd)
        {
            m_intensityStart = intensityStart;
            m_intensityEnd = intensityEnd;
            m_colorIndex = Utils.Random.Next(360);
        }

        /// <summary>
        /// The minimum color value for particles initialised by 
        /// this initializer. Should be between 0 and 1.
        /// </summary>
        public int IntensityStart
        {
            get { return m_intensityStart; }
            set { m_intensityStart = value; }
        }

        /// <summary>
        /// The maximum color value for particles initialised by 
        /// this initializer. Should be between 0 and 1.
        /// </summary>
        public int IntensityEnd
        {
            get { return m_intensityEnd; }
            set { m_intensityEnd = value; }
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
            m_lastTime += elapsedTime;

            if (m_lastTime > 1)
            {
                m_colorIndex++;
                m_lastTime = 0;
            }

            if (m_colorIndex > 360)
                m_colorIndex = 0;

            Color4b startColor = Utils.RotateHue((int)m_colorIndex, m_intensityStart);
            Color4b endColor = Utils.RotateHue((int)m_colorIndex, m_intensityEnd);

            particle.Color = Utils.InterpolateColors(startColor, endColor, particle.Energy);
        }
    }
}
