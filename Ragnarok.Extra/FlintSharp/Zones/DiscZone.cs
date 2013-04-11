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
    /// The DiscZone zone defines a circular zone. The zone may
    /// have a hole in the middle, like a doughnut.
    /// </summary>
    public class DiscZone : IZone
    {
        private Point m_center;
        private double m_innerRadius;
        private double m_outerRadius;
        private double m_innerSq;
        private double m_outerSq;

        private static double TWOPI = Math.PI * 2;

        public DiscZone()
            : this(new Point(), 1)
        {
        }

        public DiscZone(Point center, double outerRadius)
        {
            m_center = center;
            m_innerRadius = 0;
            m_outerRadius = outerRadius;
            m_innerSq = m_innerRadius * m_innerRadius;
            m_outerSq = m_outerRadius * m_outerRadius;
        }

        /// <summary>
        /// The constructor defines a DiscZone zone.
        /// </summary>
        /// <param name="center">The centre of the disc.</param>
        /// <param name="outerRadius">The radius of the outer edge of the disc.</param>
        /// <param name="innerRadius">If set, this defines the radius of the inner
        /// edge of the disc. Points closer to the center than this inner radius
        /// are excluded from the zone. If this parameter is not set then all 
        /// points inside the outer radius are included in the zone.</param>
        public DiscZone(Point center, double outerRadius, double innerRadius)
        {
            m_center = center;
            m_innerRadius = innerRadius;
            m_outerRadius = outerRadius;
            m_innerSq = m_innerRadius * m_innerRadius;
            m_outerSq = m_outerRadius * m_outerRadius;
        }

        /// <summary>
        /// The centre of the disc.
        /// </summary>
        public Point Center
        {
            get { return m_center; }
            set { m_center = value; }
        }

        /// <summary>
        /// The radius of the inner edge of the disc.
        /// </summary>
        public double InnerRadius
        {
            get { return m_innerRadius; }
            set { m_innerRadius = value; m_innerSq = m_innerRadius * m_innerRadius; }
        }

        /// <summary>
        /// The radius of the outer edge of the disc.
        /// </summary>
        public double OuterRadius
        {
            get { return m_outerRadius; }
            set { m_outerRadius = value; m_outerSq = m_outerRadius * m_outerRadius; }
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
            x -= m_center.X;
            y -= m_center.Y;

            double distSq = x * x + y * y;

            return distSq <= m_outerSq && distSq >= m_innerSq;
        }

        /// <summary>
        /// The getLocation method returns a random point inside the zone.
        /// This method is used by the initializers and actions that
        /// use the zone. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>a random point inside the zone.</returns>
        public Point GetLocation()
        {
            double rand = Utils.Random.NextDouble();
            Point point = Utils.PointToPolar(m_innerRadius + (1 - rand * rand) * (m_outerRadius - m_innerRadius), Utils.Random.NextDouble() * TWOPI);

            point.X += m_center.X;
            point.Y += m_center.Y;

            return point;
        }

        /// <summary>
        /// The getArea method returns the size of the zone.
        /// It's used by the MultiZone public class to manage the balancing between the
        /// different zones. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>the size of the zone.</returns>
        public double GetArea()
        {
            return (Math.PI * m_outerSq - Math.PI * m_innerSq);
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
            double dist = Utils.LineCircleDistance(
                new Vector(particle.PreviousX, particle.PreviousY),
                new Vector(particle.X, particle.Y),
                (Vector)m_center);

            return (m_innerRadius <= dist && dist <= m_outerRadius);
        }
    }
}
