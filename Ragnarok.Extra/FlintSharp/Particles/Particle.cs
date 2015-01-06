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

namespace FlintSharp.Particles
{
    /// <summary>
	/// The Particle public class is a set of public properties shared by all particles.
	/// It is deliberately lightweight, with only one method. The Initializers
	/// and Actions modify these properties directly. This means that the same
	/// particles can be used in many different emitters, allowing Particle 
	/// objects to be reused.
	/// 
	/// Particles are usually created by the ParticleCreator public class. This public class
	/// just simplifies the reuse of Particle objects which speeds up the
	/// application. 
    /// </summary>
    public class Particle : IComparable<Particle>
    {
        private double m_x = 0;
        private double m_y = 0;
        private double m_previousX = 0;
        private double m_previousY = 0;
        private double m_targetX = 0;
        private double m_targetY = 0;
        private double m_velocityX = 0;
        private double m_velocityY = 0;
        private double m_rotation = 0;
        private double m_angularVelocity = 0;
        private Color4b m_color = Color4bs.White;
        private double m_scale = 1;
        private double m_lifetime = 0;
        private double m_age = 0;
        private double m_energy = 1;
        private bool m_isDead = false;
        private IImageData m_imageData;

        /// <summary>
        /// Creates a particle. Alternatively particles can be reused by using the ParticleCreator to create
        /// and manage them. Usually the emitter will create the particles and the user doesn't need
        /// to create them.
        /// </summary>
        public Particle()
        {
        }

        /// <summary>
        /// Sets the particles properties to their default values.
        /// </summary>
        public void Initialize()
        {
            m_x = 0;
            m_y = 0;
            m_previousX = 0;
            m_previousY = 0;
            m_targetX = 0;
            m_targetY = 0;
            m_velocityX = 0;
            m_velocityY = 0;
            m_rotation = 0;
            m_angularVelocity = 0;
            m_color = Color4bs.White;
            m_scale = 1;
            m_lifetime = 0;
            m_age = 0;
            m_energy = 1;
            m_isDead = false;

            if (m_imageData != null)
            {
                m_imageData.Dispose();
                m_imageData = null;
            }
        }

        /// <summary>
        /// The x coordinate of the particle in pixels.
        /// </summary>
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        /// <summary>
        /// The y coordinate of the particle in pixels.
        /// </summary>
        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        /// <summary>
        /// The previous x coordinate of the particle in pixels.
        /// </summary>
        public double PreviousX
        {
            get { return m_previousX; }
            set { m_previousX = value; }
        }

        /// <summary>
        /// The previous y coordinate of the particle in pixels.
        /// </summary>
        public double PreviousY
        {
            get { return m_previousY; }
            set { m_previousY = value; }
        }

        /// <summary>
        /// The destination x coordinate of the particle in pixels.
        /// </summary>
        public double TargetX
        {
            get { return m_targetX; }
            set { m_targetX = value; }
        }

        /// <summary>
        /// The destination y coordinate of the particle in pixels.
        /// </summary>
        public double TargetY
        {
            get { return m_targetY; }
            set { m_targetY = value; }
        }

        /// <summary>
        /// The x coordinate of the velocity of the particle in pixels per second.
        /// </summary>
        public double VelocityX
        {
            get { return m_velocityX; }
            set { m_velocityX = value; }
        }

        /// <summary>
        /// The y coordinate of the velocity of the particle in pixels per second.
        /// </summary>
        public double VelocityY
        {
            get { return m_velocityY; }
            set { m_velocityY = value; }
        }

        /// <summary>
        /// The rotation of the particle in radians.
        /// </summary>
        public double Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }

        /// <summary>
        /// The angular velocity of the particle in radians per second.
        /// </summary>
        public double AngularVelocity
        {
            get { return m_angularVelocity; }
            set { m_angularVelocity = value; }
        }

        /// <summary>
        /// The 32bit ARGB color of the particle. The initial value is 0xFFFFFFFF (white).
        /// </summary>
        public Color4b Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        /// <summary>
        /// The scale of the particle ( 1 is normal size ).
        /// </summary>
        public double Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        /// <summary>
        /// The lifetime of the particle, in seconds.
        /// </summary>
        public double Lifetime
        {
            get { return m_lifetime; }
            set { m_lifetime = value; }
        }

        /// <summary>
        /// The age of the particle, in seconds.
        /// </summary>
        public double Age
        {
            get { return m_age; }
            set { m_age = value; }
        }

        /// <summary>
        /// The energy of the particle.
        /// </summary>
        public double Energy
        {
            get { return m_energy; }
            set { m_energy = value; }
        }

        /// <summary>
        /// Whether the particle is dead and should be removed from the stage.
        /// </summary>
        public bool IsDead
        {
            get { return m_isDead; }
            set { m_isDead = value; }
        }

        /// <summary>
        /// The image data.
        /// </summary>
        public IImageData ImageData
        {
            get { return m_imageData; }
            set { m_imageData = value; }
        }

        /// <summary>
        /// For sorting the particles by x
        /// </summary>
        public int CompareTo(Particle other)
        {
            return this.X.CompareTo(other.X);
        }
    }
}
