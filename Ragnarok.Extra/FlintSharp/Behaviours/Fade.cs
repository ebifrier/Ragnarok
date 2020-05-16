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

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.EnergyEasing;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

using Ragnarok.Utility;

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The Fade action adjusts the particle's alpha as it ages. This action
    /// should be used in conjunction with the Age action.
    /// </summary>
    public class Fade : Behaviour
    {
        private double m_startAlpha;
        private double m_endAlpha;
        private double m_diffAlpha;

        public Fade()
            : this(1.0, 0.0)
        {
        }

        public Fade(double startAlpha)
            : this(startAlpha, 0.0)
        {
        }

        /// <summary>
        /// The constructor creates a Fade action for use by 
        /// an emitter. To add a Fade to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="startAlpha">The alpha value for the particle at the
        /// start of its life. Should be between 0 and 1.</param>
        /// <param name="endAlpha">The alpha value of the particle at the end of its
        /// life. Should be between 0 and 1.</param>
        public Fade(double startAlpha, double endAlpha)
        {
            StartAlpha = startAlpha;
            EndAlpha = endAlpha;
        }

        /// <summary>
        /// life. Should be between 0 and 1.
        /// Should be between 0 and 1.
        /// </summary>
        public double StartAlpha
        {
            get { return m_startAlpha; }
            set { m_startAlpha = value; UpdateAlphaDiff(); }
        }

        /// <summary>
        /// The alpha value for the particle at the end of its life.
        /// Should be between 0 and 1.
        /// </summary>
        public double EndAlpha
        {
            get { return m_endAlpha; }
            set { m_endAlpha = value; UpdateAlphaDiff(); }
        }

        /// <summary>
        /// Updates the difference of the alpha value.
        /// </summary>
        private void UpdateAlphaDiff()
        {
            m_diffAlpha = m_startAlpha - m_endAlpha;
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of actions.
        /// It is called within the emitter's addAction method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.common.actions.Action#getDefaultPriority()
        /// </summary>
        /// <returns><p>Returns a value of -5, so that the Fade executes after color changes.</p></returns>
        public override int GetDefaultPriority()
        {
            return -5;
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

            double alpha = m_endAlpha + m_diffAlpha * particle.Energy;
            int a = Math.Max(0, Math.Min(255, (int)(alpha * 256)));

            particle.Color = Color4b.FromArgb(a, particle.Color);
        }
    }
}
