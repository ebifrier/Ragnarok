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
    /// The TweenPosition action adjusts the particle's position between two
    /// locations as it ages. This action
    /// should be used in conjunction with the Age action.
    /// </summary>
    public class TweenPosition : Behaviour
    {
        private double m_diffX;
        private double m_endX;
        private double m_diffY;
        private double m_endY;

        public TweenPosition()
            : this(0, 0, 1, 1)
        {
        }

        /// <summary>
        /// The constructor creates a TweenPosition action for use by 
        /// an emitter. To add a TweenPosition to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="startX">The x value for the particle at the
        /// start of its life.</param>
        /// <param name="startY">The y value for the particle at the
        /// start of its life.</param>
        /// <param name="endX">The x value of the particle at the end of its
        /// life.</param>
        /// <param name="endY">The y value of the particle at the end of its
        /// life.</param>
        public TweenPosition(double startX, double startY, double endX, double endY)
        {
            m_diffX = startX - endX;
            m_endX = endX;
            m_diffY = startY - endY;
            m_endY = endY;
        }

        /// <summary>
        /// The x position for the particle at the start of its life.
        /// </summary>
        public double StartX
        {
            get { return m_endX + m_diffX; }
            set { m_diffX = value - m_endX; }
        }

        /// <summary>
        /// The X value for the particle at the end of its life.
        /// </summary>
        public double EndX
        {
            get { return m_endX; }
            set { m_diffX = m_endX + m_diffX - value; m_endX = value; }
        }

        /// <summary>
        /// The y position for the particle at the start of its life.
        /// </summary>
        public double StartY
        {
            get { return m_endY + m_diffY; }
            set { m_diffY = value - m_endY; }
        }

        /// <summary>
        /// The y value for the particle at the end of its life.
        /// </summary>
        public double EndY
        {
            get { return m_endY; }
            set { m_diffY = m_endY + m_diffY - value; m_endY = value; }
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
            particle.X = m_endX + m_diffX * particle.Energy;
            particle.Y = m_endY + m_diffY * particle.Energy;
        }
    }
}
