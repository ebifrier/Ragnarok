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
    /// The ZonedAction Action applies an action to the particle only if it is in the specified zone. 
    /// </summary>
    public class ZonedAction : Behaviour
    {
        private Behaviour m_action;
        private IZone m_zone;
        private bool m_invert;

        public ZonedAction()
            : this(null, null)
        {
        }

        public ZonedAction(Behaviour action, IZone zone)
        {
            m_action = action;
            m_zone = zone;
            m_invert = false;
        }

        /// <summary>
        /// The constructor creates a ZonedAction action for use by 
        /// an emitter. To add a ZonedAction to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="action">The action to apply when inside the zone.</param>
        /// <param name="zone">The zone in which to apply the action.</param>
        /// <param name="invertZone">If false (the default) the action is applied only to particles inside 
        /// the zone. If true the action is applied only to particles outside the zone.</param>
        public ZonedAction(Behaviour action, IZone zone, bool invertZone)
        {
            m_action = action;
            m_zone = zone;
            m_invert = invertZone;
        }

        /// <summary>
        /// The action to apply when inside the zone.
        /// </summary>
        public Behaviour Action
        {
            get { return m_action; }
            set { m_action = value; }
        }

        /// <summary>
        /// The zone in which to apply the acceleration.
        /// </summary>
        public IZone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        /// <summary>
        /// If false (the default), the action is applied to particles inside the zone.
        /// If true, the action is applied to particles outside the zone.
        /// </summary>
        public bool InvertZone
        {
            get { return m_invert; }
            set { m_invert = value; }
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of actions.
        /// It is called within the emitter's addAction method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.common.actions.Action#getDefaultPriority()
        /// </summary>
        /// <returns>The priority value</returns>
        public override int GetDefaultPriority()
        {
            return m_action.GetDefaultPriority();
        }

        /// <summary>
        /// The addedToEmitter method is called by the emitter when the Action is added to it
        /// It is called within the emitter's addAction method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Action was added to.</param>
        public override void AddedToEmitter(Emitter emitter)
        {
            m_action.AddedToEmitter(emitter);
        }

        /// <summary>
        /// The removedFromEmitter method is called by the emitter when the Action is removed from it
        /// It is called within the emitter's removeAction method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Action was removed from.</param>
        public override void RemovedFromEmitter(Emitter emitter)
        {
            m_action.RemovedFromEmitter(emitter);
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
            if (m_zone.Contains(particle.X, particle.Y))
            {
                if (!m_invert)
                    m_action.Update(emitter, particle, elapsedTime);
            }
            else
            {
                if (m_invert)
                    m_action.Update(emitter, particle, elapsedTime);
            }
        }
    }
}
