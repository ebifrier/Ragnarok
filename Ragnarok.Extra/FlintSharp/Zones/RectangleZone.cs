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

namespace FlintSharp.Zones
{
    /// <summary>
    /// The RectangleZone zone defines a rectangular shaped zone.
    /// </summary>
    public class RectangleZone : IZone
    {
        private double m_left;
        private double m_top;
        private double m_right;
        private double m_bottom;
        private double m_width;
        private double m_height;

        public RectangleZone()
            : this(0, 0, 1, 1)
        {
        }

        /// <summary>
        /// The constructor creates a RectangleZone zone.
        /// </summary>
        /// <param name="left">The left coordinate of the rectangle defining the region of the zone.</param>
        /// <param name="top">The top coordinate of the rectangle defining the region of the zone.</param>
        /// <param name="right">The right coordinate of the rectangle defining the region of the zone.</param>
        /// <param name="bottom">The bottom coordinate of the rectangle defining the region of the zone.</param>
        public RectangleZone(double left, double top, double right, double bottom)
        {
            m_left = left;
            m_top = top;
            m_right = right;
            m_bottom = bottom;
            m_width = right - left;
            m_height = bottom - top;
        }

        /// <summary>
        /// The left coordinate of the rectangle defining the region of the zone.
        /// </summary>
        public double Left
        {
            get { return m_left; }
            set { m_left = value; m_width = m_right - m_left; }
        }

        /// <summary>
        /// The right coordinate of the rectangle defining the region of the zone.
        /// </summary>
        public double Right
        {
            get { return m_right; }
            set { m_right = value; m_width = m_right - m_left; }
        }

        /// <summary>
        /// The top coordinate of the rectangle defining the region of the zone.
        /// </summary>
        public double Top
        {
            get { return m_top; }
            set { m_top = value; m_height = m_bottom - m_top; }
        }

        /// <summary>
        /// The bottom coordinate of the rectangle defining the region of the zone.
        /// </summary>
        public double Bottom
        {
            get { return m_bottom; }
            set { m_bottom = value; m_height = m_bottom - m_top; }
        }

        /// <summary>
        /// The width of the rectangle defining the region of the zone.
        /// </summary>
        public double Width
        {
            get { return m_width; }
        }

        /// <summary>
        /// The height of the rectangle defining the region of the zone.
        /// </summary>
        public double Height
        {
            get { return m_height; }
        }

        /// <summary>
        /// The contains method determines whether a point is inside the zone.
        /// This method is used by the initializers and actions that
        /// use the zone. Usually, it need not be called directly by the user.
        /// </summary>
        /// <param name="x">The x coordinate of the location to test for.</param>
        /// <param name="y">The y coordinate of the location to test for.</param>
        /// <returns>true if point is inside the zone, false if it is outside.</returns>
        public bool Contains(double x, double y)
        {
            return x >= m_left && x < m_right && y >= m_top && y < m_bottom;
        }

        /// <summary>
        /// The getLocation method returns a random point inside the zone.
        /// This method is used by the initializers and actions that
        /// use the zone. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>a random point inside the zone.</returns>
        public Point GetLocation()
        {
            return new Point((m_left + Utils.Random.NextDouble() * m_width), (m_top + Utils.Random.NextDouble() * m_height));
        }

        /// <summary>
        /// The getArea method returns the size of the zone.
        /// It's used by the MultiZone public class to manage the balancing between the
        /// different zones. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>the size of the zone.</returns>
        public double GetArea()
        {
            return m_width * m_height;
        }

        /// <summary>
        /// Manages collisions between a particle and the zone. The particle will collide with the line defined
        /// for this zone. In the interests of speed, the collisions are not exactly accurate at the ends of the
        /// line, but are accurate enough to ensure the particle doesn't pass through the line and to look
        /// realistic in most circumstances. The collisionRadius of the particle is used when calculating the collision.
        /// </summary>
        /// <param name="particle">The particle to be tested for collision with the zone.</param>
        /// <param name="bounce">The coefficient of restitution for the collision.</param>
        /// <returns>Whether a collision occured.</returns>
        public bool CollideParticle(Particle particle, double bounce = 1)
        {
            throw new NotImplementedException();
        }
    }
}
