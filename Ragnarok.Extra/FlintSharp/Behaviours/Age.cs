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
    /// The Age action operates in conjunction with the Lifetime 
    /// initializer. The Lifetime initializer sets the lifetime for
    /// the particle. The Age action then ages the particle over time,
    /// altering its energy to reflect its age. This energy can then
    /// be used by actions like Fade and ColorChange to alter the
    /// appearence of the particle as it ages.
    /// 
    /// <p>When the particle's lifetime is over, this action marks it 
    /// as dead.</p>
    /// 
    /// <p>When adjusting the energy this action can use any of the
    /// easing functions in the org.flintparticles.energy package.</p>
    /// </summary>
    public class Age : Behaviour
    {
        private bool m_isAutoDead;
        private EaseType m_easeType;
        private EaseCategory m_category;
        private EnergyEasingDelegate m_easingFunction;

        public Age()
        {
            IsAutoDead = true;
            Category = EaseCategory.Linear;
            EaseType = EaseType.None;
        }

        /// <summary>
        /// The constructor creates an Age action for use by 
        /// an emitter. To add an Age to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// 
        /// </summary>
        /// <param name="easeCategory">The easing category to use to modify the energy
        /// curve over the lifetime of the particle</param>
        /// <param name="easeType">The easing type to use to modify the energy
        /// curve over the lifetime of the particle</param>
        public Age(EaseCategory easeCategory, EaseType easeType)
        {
            IsAutoDead = true;
            Category = easeCategory;
            EaseType = easeType;
        }

        /// <summary>
        /// Wheather the particle is dead automatically.
        /// </summary>
        public bool IsAutoDead
        {
            get { return m_isAutoDead; }
            set { m_isAutoDead = value; }
        }

        /// <summary>
        /// The easing category used to modify the energy curve over the lifetime of the particle.
        /// </summary>
        public EaseCategory Category
        {
            get { return m_category; }
            set { m_category = value; UpdateFunction(); }
        }

        /// <summary>
        /// The easing type used to modify the energy curve over the lifetime of the particle.
        /// </summary>
        public EaseType EaseType
        {
            get { return m_easeType; }
            set { m_easeType = value; UpdateFunction(); }
        }

        /*/// <summary>
        /// The easing function used to modify the energy curve over the lifetime of the particle.
        /// </summary>
        public EnergyEasingDelegate EasingFunction
        {
            get { return m_easingFunction; }
            set { m_easingFunction = value; }
        }*/

        /// <summary>
        /// Updates the function object;
        /// </summary>
        private void UpdateFunction()
        {
            m_easingFunction = EnergyEasing.EasingFunction.GetEasingFunction(
                m_category, m_easeType);
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
            particle.Age += elapsedTime;

            if (particle.Age >= particle.Lifetime)
            {
                if (IsAutoDead)
                {
                    particle.Energy = 0;
                    particle.IsDead = true;
                }
                else
                {
                    particle.Energy = 1.0;
                }
            }
            else
                particle.Energy = m_easingFunction(particle.Age, particle.Lifetime);
        }
    }
}
