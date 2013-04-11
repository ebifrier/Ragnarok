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
    /// The CollisionZone action marks the particle as dead if it is inside
    /// a zone.
    /// </summary>
    public class CollisionZone : Behaviour
    {
        private IZone m_zone;
        private bool m_invertZone;

        public CollisionZone()
            : this(null)
        {
        }

        public CollisionZone(IZone zone)
        {
            m_zone = zone;
            m_invertZone = false;
        }

        /// <summary>
        /// The constructor creates a DeathZone action for use by 
        /// an emitter. To add a DeathZone to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// @see org.flintparticles.zones
        /// </summary>
        /// <param name="zone">The zone to use. Any item from the org.flintparticles.zones
        /// package can be used.</param>
        /// <param name="zoneIsSafe">If true, the zone is treated as the safe area
        /// and being outside the zone results in the particle dying.</param>
        public CollisionZone(IZone zone, bool zoneIsSafe)
        {
            m_zone = zone;
            m_invertZone = zoneIsSafe;
        }

        /// <summary>
        /// The zone.
        /// </summary>
        public IZone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        /// <summary>
        /// If true, the zone is treated as the safe area and being ouside the zone
        /// results in the particle dying. Otherwise, being inside the zone causes the
        /// particle to die.
        /// </summary>
        public bool ZoneIsSafe
        {
            get { return m_invertZone; }
            set { m_invertZone = value; }
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of actions.
        /// It is called within the emitter's addAction method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.common.actions.Action#getDefaultPriority()
        /// </summary>
        /// <returns><p>Returns a value of -20, so that the DeathZone executes after all movement has occured.</p></returns>
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
            bool inside = m_zone.CollideParticle(particle);

            if (m_invertZone)
                inside = !inside;

            if (inside)
                particle.IsDead = true;
        }
    }
}
