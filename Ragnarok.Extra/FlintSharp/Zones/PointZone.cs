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
    /// The PointZone zone defines a zone that contains a single point.
    /// </summary>
    public class PointZone : IZone
    {
        private Point m_point;

        public PointZone()
            : this(new Point())
        {
        }

        /// <summary>
        /// The constructor defines a PointZone zone.
        /// </summary>
        /// <param name="point">The point that is the zone.</param>
        public PointZone(Point point)
        {
            m_point = point;
        }

        /// <summary>
        /// The point that is the zone.
        /// </summary>
        public Point Point
        {
            get { return m_point; }
            set { m_point = value; }
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
            return m_point.X == x && m_point.Y == y;
        }

        /// <summary>
        /// The getLocation method returns a random point inside the zone.
        /// This method is used by the initializers and actions that
        /// use the zone. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>a random point inside the zone.</returns>
        public Point GetLocation()
        {
            return new Point(m_point.X, m_point.Y);
        }

        /// <summary>
        /// The getArea method returns the size of the zone.
        /// It's used by the MultiZone public class to manage the balancing between the
        /// different zones. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>the size of the zone.</returns>
        public double GetArea()
        {
            return 1;
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
            if (particle == null)
            {
                throw new ArgumentNullException(nameof(particle));
            }

            double dist = Utils.LineCircleDistance(
                new Vector(particle.PreviousX, particle.PreviousY),
                new Vector(particle.X, particle.Y),
                (Vector)m_point);

            return (dist <= double.Epsilon);
        }
    }
}
