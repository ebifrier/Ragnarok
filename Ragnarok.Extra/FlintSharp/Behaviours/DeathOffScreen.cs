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
    /// The DeathOffStage action marks the particle as dead if it is outside the stage.
    /// This action is only reliable when the display region for the swf is exactly the
    /// same size as the swf itself, so that no scaling occurs.
    /// 
    /// <p>Warning: The DeathOffStage action is very slow. A DeathZone action with an appropriate
    /// RectangleZone is more efficient.</p>
    /// 
    /// <p>This renderer uses properties of the stage object. It throws an exception if it is
    /// not in the same security sandbox as the Stage owner (the main SWF file). To avoid 
    /// this, the Stage owner can grant permission to the domain containing this renderer
    /// by calling the Security.allowDomain() method or the Security.allowInsecureDomain()
    /// method.</p>
    /// </summary>
    public class DeathOffScreen : Behaviour
    {
        private double m_padding;
        private double m_left = double.NaN;
        private double m_right = double.NaN;
        private double m_top = double.NaN;
        private double m_bottom = double.NaN;

        public DeathOffScreen()
        {
            m_padding = 10;
        }

        /// <summary>
        /// The constructor creates a DeathOffStage action for use by 
        /// an emitter. To add a DeathOffStage to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="padding">An additional distance, in pixels, to add around the stage
        /// to allow for the size of the particles.</param>
        public DeathOffScreen(double padding)
        {
            m_padding = padding;
        }

        /// <summary>
        /// An additional distance, in pixels, to add around the stage
        /// to allow for the size of the particles.
        /// </summary>
        public double Padding
        {
            get { return m_padding; }
            set { m_padding = value; }
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of actions.
        /// It is called within the emitter's addAction method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.common.actions.Action#getDefaultPriority()
        /// </summary>
        /// <returns><p>Returns a value of -20, so that the DeathOffStage executes after all movement has occured.</p></returns>
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

            if (double.IsNaN(m_top))
            {
                Point tl = new Point(0, 0);
                Point br = new Point(Utils.ScreenSize.Width, Utils.ScreenSize.Height);
                m_left = tl.X - m_padding;
                m_right = br.X + m_padding;
                m_top = tl.Y - m_padding;
                m_bottom = br.Y + m_padding;
            }

            if (particle.X < m_left || particle.X > m_right || particle.Y < m_top || particle.Y > m_bottom)
                particle.IsDead = true;
        }
    }
}
