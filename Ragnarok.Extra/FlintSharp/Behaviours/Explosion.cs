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

namespace FlintSharp.Behaviours
{
    /// <summary>
    /// The Explosion action applies a force on the particle to push it away from
    /// a single point - the center of the explosion. The force occurs instantaneously at the central point 
    /// of the explosion and then ripples out in a shock wave.
    /// </summary>
    public class Explosion : Behaviour, IFrameUpdateable
    {
        private UpdateOnFrame m_updateActivity = null;
        private double m_x;
        private double m_y;
        private double m_power;
        private double m_depth;
        private double m_invDepth;
        private double m_epsilonSq;
        private double m_oldRadius = 0;
        private double m_radius = 0;
        private double m_radiusChange = 0;
        private double m_expansionRate = 500;
        private double m_lowerBoundarySq;
        private double m_upperBoundarySq;

        public Explosion()
            : this(1.0, 0.0, 0.0)
        {
        }

        public Explosion(double power, double x, double y)
        {
            m_power = power;
            m_x = x;
            m_y = y;
            m_expansionRate = 300;
            m_depth = 5;
            m_invDepth = 1 / m_depth;
            m_epsilonSq = 1;
        }

        public Explosion(double power, double x, double y, double expansionRate)
        {
            m_power = power;
            m_x = x;
            m_y = y;
            m_expansionRate = expansionRate;
            m_depth = 5;
            m_invDepth = 1 / m_depth;
            m_epsilonSq = 1;
        }

        public Explosion(double power, double x, double y, double expansionRate, double depth)
        {
            m_power = power;
            m_x = x;
            m_y = y;
            m_expansionRate = expansionRate;
            m_depth = depth * 0.5f;
            m_invDepth = 1 / m_depth;
            m_epsilonSq = 1;
        }

        /// <summary>
        /// The constructor creates an Explosion action for use by 
        /// an emitter. To add an Explosion to all particles created by an emitter, use the
        /// emitter's addAction method.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addAction()
        /// </summary>
        /// <param name="power">The strength of the explosion - larger numbers produce a stronger force.</param>
        /// <param name="x">The x coordinate of the center of the explosion.</param>
        /// <param name="y">The y coordinate of the center of the explosion.</param>
        /// <param name="expansionRate">The rate at which the shockwave moves out from the explosion, in pixels per second.</param>
        /// <param name="depth">The depth (front-edge to back-edge) of the shock wave.</param>
        /// <param name="epsilon">The minimum distance for which the explosion force is calculated. 
        /// Particles closer than this distance experience the explosion as it they were 
        /// this distance away. This stops the explosion effect blowing up as distances get 
        /// small.</param>
        public Explosion(double power, double x, double y, double expansionRate, double depth, double epsilon)
        {
            m_power = power;
            m_x = x;
            m_y = y;
            m_expansionRate = expansionRate;
            m_depth = depth * 0.5f;
            m_invDepth = 1 / m_depth;
            m_epsilonSq = epsilon * epsilon;
        }

        /// <summary>
        /// The strength of the explosion - larger numbers produce a stronger force.
        /// </summary>
        public double Power
        {
            get { return m_power; }
            set { m_power = value; }
        }

        /// <summary>
        /// The strength of the explosion - larger numbers produce a stronger force.
        /// </summary>
        public double ExpansionRate
        {
            get { return m_expansionRate; }
            set { m_expansionRate = value; }
        }

        /// <summary>
        /// The depth (front-edge to back-edge) of the shock wave.
        /// </summary>
        public double Depth
        {
            get { return m_depth * 2; }
            set { m_depth = value * 0.5f; m_invDepth = 1 / m_depth; }
        }

        /// <summary>
        /// The x coordinate of the center of the explosion.
        /// </summary>
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        /// <summary>
        /// The y coordinate of the center of the explosion.
        /// </summary>
        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        /// <summary>
        /// The minimum distance for which the explosion force is calculated. 
        /// Particles closer than this distance experience the explosion as it they were 
        /// this distance away. This stops the explosion effect blowing up as distances get 
        /// small.
        /// </summary>
        public double Epsilon
        {
            get { return Math.Sqrt(m_epsilonSq); }
            set { m_epsilonSq = value * value; }
        }

        /// <summary>
        /// The addedToEmitter method is called by the emitter when the Action is added to it
        /// It is called within the emitter's addAction method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Action was added to.</param>
        public override void AddedToEmitter(Emitter emitter)
        {
            m_updateActivity = new UpdateOnFrame(this);
            emitter.Activities.Add(m_updateActivity);
        }

        /// <summary>
        /// The removedFromEmitter method is called by the emitter when the Action is removed from it
        /// It is called within the emitter's removeAction method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Action was removed from.</param>
        public override void RemovedFromEmitter(Emitter emitter)
        {
            if (m_updateActivity != null)
                emitter.Activities.Remove(m_updateActivity);
        }

        /// <summary>
        /// Called every frame before the particles are updated. This method is called via the FrameUpdateable
        /// public interface which is called by the emitter by using an UpdateOnFrame activity.
        /// </summary>
        public void FrameUpdate(Emitter emitter, double elapsedTime)
        {
            m_oldRadius = m_radius;
            m_radiusChange = m_expansionRate * elapsedTime;
            m_radius += m_radiusChange;
            double lowerBoundary = m_oldRadius - m_depth;
            if (lowerBoundary < 0)
                m_lowerBoundarySq = 0;
            else
                m_lowerBoundarySq = lowerBoundary * lowerBoundary;

            double upperBoundary = m_radius + m_depth;
            m_upperBoundarySq = upperBoundary * upperBoundary;
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
            double x = particle.X - m_x;
            double y = particle.Y - m_y;
            double dSq = x * x + y * y;

            if (dSq == 0)
                return;

            if (dSq < m_lowerBoundarySq || dSq > m_upperBoundarySq)
                return;

            double d = Math.Sqrt(dSq);
            double offset = d < m_radius ? m_depth - m_radius + d : m_depth - d + m_radius;
            double oldOffset = d < m_oldRadius ? m_depth - m_oldRadius + d : m_depth - d + m_oldRadius;

            offset *= m_invDepth;
            oldOffset *= m_invDepth;

            if (offset < 0)
            {
                elapsedTime = elapsedTime * (m_radiusChange + offset) / m_radiusChange;
                offset = 0;
            }
            if (oldOffset < 0)
            {
                elapsedTime = elapsedTime * (m_radiusChange + oldOffset) / m_radiusChange;
                oldOffset = 0;
            }

            double factor;

            if (d < m_oldRadius || d > m_radius)
            {
                factor = elapsedTime * m_power * (offset + oldOffset) / (m_radius * 2 * d);
            }
            else
            {
                double ratio = (1 - oldOffset) / m_radiusChange;
                double f1 = ratio * elapsedTime * m_power * (oldOffset + 1) / (m_radius * 2 * d);
                double f2 = (1 - ratio) * elapsedTime * m_power * (offset + 1) / (m_radius * 2 * d);
                factor = f1 + f2;
            }

            particle.VelocityX += x * factor;
            particle.VelocityY += y * factor;
        }
    }
}
