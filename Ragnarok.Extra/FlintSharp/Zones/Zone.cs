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

namespace FlintSharp.Zones
{
    public enum ZonePosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public enum ZoneDirection
    {
        Horizontal,
        Vertical
    }

    /// <summary>
	/// The Zones public interface must be implemented by all zones.
	/// 
	/// <p>A zone is a public class that defined a region in 2d space. The two required methods 
	/// make it easy to get a random point within the zone and to find whether a specific
	/// point is within the zone. Zones are used to define the start location for particles
	/// (in the Position initializer), to define the start velocity for particles (in the
    /// Velocity initializer), and to define zones within which the particles die.
    /// </summary>
    public interface IZone
    {
        /// <summary>
        /// The contains method determines whether a point is inside the zone.
		/// This method is used by the initializers and actions that
		/// use the zone. Usually, it need not be called directly by the user.
        /// </summary>
        /// <param name="x">The x coordinate of the location to test for.</param>
        /// <param name="y">The y coordinate of the location to test for.</param>
        /// <returns>true if point is inside the zone, false if it is outside.</returns>
        bool Contains(double x, double y);

        /// <summary>
		/// The getLocation method returns a random point inside the zone.
		/// This method is used by the initializers and actions that
		/// use the zone. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>a random point inside the zone.</returns>
        Point GetLocation();

        /// <summary>
		/// The getArea method returns the size of the zone.
		/// It's used by the MultiZone public class to manage the balancing between the
		/// different zones. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>the size of the zone.</returns>
        double GetArea();

        /// <summary>
        /// Manages collisions between a particle and the zone. The particle will collide with the line defined
        /// for this zone. In the interests of speed, the collisions are not exactly accurate at the ends of the
        /// line, but are accurate enough to ensure the particle doesn't pass through the line and to look
        /// realistic in most circumstances. The collisionRadius of the particle is used when calculating the collision.
        /// </summary>
        /// <param name="particle">The particle to be tested for collision with the zone.</param>
        /// <param name="bounce">The coefficient of restitution for the collision.</param>
        /// <returns>Whether a collision occured.</returns>
        bool CollideParticle(Particle particle, double bounce = 1);
    }
}
