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

using System.Windows;

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
    /// The BoundingBox action confines each particle to a box. The 
    /// particle bounces back off the side of the box when it reaches 
    /// the edge. The bounce treats the particle as a circular body
    /// and displays no loss of energy in the collision.
    /// </summary>
    public class BoundingBox : Behaviour
    {
        private double m_left;
        private double m_top;
        private double m_right;
        private double m_bottom;

        public BoundingBox()
            : this(0, 0, 1, 1)
        {
        }

        /// <summary>
        /// The constructor creates a BoundingBox action for use by 
        /// an emitter. To add a BoundingBox to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="left">The left coordinate of the box. The coordinates are in the
        /// coordinate space of the object containing the emitter.</param>
        /// <param name="top">The top coordinate of the box. The coordinates are in the
        /// coordinate space of the object containing the emitter.</param>
        /// <param name="right">The right coordinate of the box. The coordinates are in the
        /// coordinate space of the object containing the emitter.</param>
        /// <param name="bottom">The bottom coordinate of the box. The coordinates are in the
        /// coordinate space of the object containing the emitter.</param>
        public BoundingBox(double left, double top, double right, double bottom)
        {
            m_left = left;
            m_top = top;
            m_right = right;
            m_bottom = bottom;
        }

        /// <summary>
        /// The bounding box.
        /// </summary>
        public Rect Box
        {
            get { return new Rect(m_left, m_top, m_right - m_left, m_bottom - m_top); }
            set
            {
                m_left = value.Left;
                m_right = value.Right;
                m_top = value.Top;
                m_bottom = value.Bottom;
            }
        }

        /// <summary>
        /// The left coordinate of the bounding box.
        /// </summary>
        public double Left
        {
            get { return m_left; }
            set { m_left = value; }
        }

        /// <summary>
        /// The top coordinate of the bounding box.
        /// </summary>
        public double Top
        {
            get { return m_top; }
            set { m_top = value; }
        }

        /// <summary>
        /// The right coordinate of the bounding box.
        /// </summary>
        public double Right
        {
            get { return m_right; }
            set { m_right = value; }
        }

        /// <summary>
        /// The bottom coordinate of the bounding box.
        /// </summary>
        public double Bottom
        {
            get { return m_bottom; }
            set { m_bottom = value; }
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of actions.
        /// It is called within the emitter's addAction method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.common.actions.Action#getDefaultPriority()
        /// </summary>
        /// <returns><p>Returns a value of -20, so that the BoundingBox executes after all movement has occured.</p></returns>
        public override int GetDefaultPriority()
        {
            return -20;
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
            if (emitter == null)
            {
                return;
            }

            if (particle == null)
            {
                return;
            }

            double halfWidth;
            double halfHeight;

            halfWidth = (particle.Scale * 0.5);
            halfHeight = (particle.Scale * 0.5);

            double position;

            if (particle.VelocityX > 0 && (position = particle.X + halfWidth) >= m_right)
            {
                particle.VelocityX = -particle.VelocityX;
                particle.X += 2 * (m_right - position);
            }
            else if (particle.VelocityX < 0 && (position = particle.X - halfWidth) <= m_left)
            {
                particle.VelocityX = -particle.VelocityX;
                particle.X += 2 * (m_left - position);
            }
            if (particle.VelocityY > 0 && (position = particle.Y + halfHeight) >= m_bottom)
            {
                particle.VelocityY = -particle.VelocityY;
                particle.Y += 2 * (m_bottom - position);
            }
            else if (particle.VelocityY < 0 && (position = particle.Y - halfHeight) <= m_top)
            {
                particle.VelocityY = -particle.VelocityY;
                particle.Y += 2 * (m_top - position);
            }
        }
    }
}
