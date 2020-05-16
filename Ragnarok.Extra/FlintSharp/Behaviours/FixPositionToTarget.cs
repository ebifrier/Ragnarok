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

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The FixPositionToTarget action makes the particle fix if it is
    /// near to the target position.
    /// </summary>
    public class FixPositionToTarget : Behaviour
    {
        private double m_radius = 1.0;

        public FixPositionToTarget()
            : this(1.0)
        {
        }

        /// <summary>
        /// The constructor creates a FixPositionToTarget action for use by 
        /// an emitter. To add a DeathZone to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// @see org.flintparticles.zones
        /// </summary>
        public FixPositionToTarget(double radius)
        {
            m_radius = radius;
        }

        /// <summary>
        /// The collision radius from the target.
        /// </summary>
        public double Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
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
            if (emitter == null)
            {
                return;
            }

            if (particle == null)
            {
                return;
            }

            double dist = Utils.LineCircleDistance(
                new Vector(particle.PreviousX, particle.PreviousY),
                new Vector(particle.X, particle.Y),
                new Vector(particle.TargetX, particle.TargetY));

            if (dist <= m_radius)
            {
                particle.X = particle.TargetX;
                particle.Y = particle.TargetY;
            }
        }
    }
}
