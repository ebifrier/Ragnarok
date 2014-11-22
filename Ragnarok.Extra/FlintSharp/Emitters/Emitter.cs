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
using System.Collections.Generic;
using System.Windows;

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.EnergyEasing;
using FlintSharp.Events;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

namespace FlintSharp.Emitters
{
    /// <summary>
    /// The Emitter public class manages the creation and ongoing state of particles. It uses a number of
    /// utility public classes to customise its behaviour.
    /// 
    /// <p>An emitter uses Initializers to customise the initial state of particles
    /// that it creates, their position, velocity, color etc. These are added to the 
    /// emitter using the addInitializer  method.</p>
    /// 
    /// <p>An emitter uses Actions to customise the behaviour of particles that
    /// it creates, to apply gravity, drag, fade etc. These are added to the emitter 
    /// using the addAction method.</p>
    /// 
    /// <p>An emitter uses Activities to customise its own behaviour in an ongoing manner, to 
    /// make it move or rotate.</p>
    /// 
    /// <p>An emitter uses a Counter to know when and how many particles to emit.</p>
    /// 
    /// <p>An emitter uses a Renderer to display the particles on screen.</p>
    /// 
    /// <p>All timings in the emitter are based on actual time passed, not on frames.</p>
    /// 
    /// <p>Most functionality is best added to an emitter using Actions,
    /// Initializers, Activities, Counters and Renderers. This offers greater 
    /// flexibility to combine behaviours witout needing to subpublic class 
    /// the Emitter itself.</p>
    /// 
    /// <p>The emitter also has position properties - x, y, rotation - that can be used to directly
    /// affect its location in the particle system.
    /// </summary>
    public class Emitter
    {
        private static readonly IParticleFactory m_creator = new ParticleCreator();
        private readonly IParticleFactory m_particleFactory;

        private List<Particle> m_particles = null;
        private SortedCollection<Initializer> m_initializers = null;
        private SortedCollection<Behaviour> m_behaviours = null;
        private SortedCollection<Activity> m_activities = null;
        private ICounter m_counter = null;

        private TimeSpan m_waitTime = TimeSpan.Zero;
        private double m_x = 0;
        private double m_y = 0;
        private double m_rotation = 0;
        private bool m_spaceSort = false;
        private bool m_running = false;

        /// <summary>
        /// Dispatched when a particle is created and has just been added to the emitter.
        /// <summary>
        public event EventHandler<ParticleEventArgs> ParticleCreated;
        
        /// <summary>
        /// Dispatched when a particle is removed by the emitter.
        /// </summary>
        public event EventHandler<ParticleEventArgs> ParticleRemoved;

        /// <summary>
        /// Dispatched when a pre-existing particle is added to the emitter.
        /// <summary>
        public event EventHandler<ParticleEventArgs> ParticleAdded;
        
        /// <summary>
        /// Dispatched when a particle dies and is about to be removed from the system.
        /// As soon as the event has been handled the particle will be removed but at the
        /// time of the event it still exists so its properties (e.g. its location) can be
        /// read from it.
        /// </summary>
        public event EventHandler<ParticleEventArgs> ParticleDied;

        /// <summary>
        /// Dispatched when the particle system has updated and the state of the particles
        /// has changed.
        /// </summary>
        public event EventHandler<EmitterEventArgs> Updated;

        /// <summary>
        /// The constructor creates an emitter.
        /// </summary>
        public Emitter()
        {
            m_particleFactory = m_creator;

            m_particles = new List<Particle>();
            m_behaviours = new SortedCollection<Behaviour>(this);
            m_initializers = new SortedCollection<Initializer>(this);
            m_activities = new SortedCollection<Activity>(this);
            m_counter = new ZeroCounter();
        }

        /// <summary>
        /// Initializers of the Emitter. Initializers set the
        /// initial properties of particles created by the emitter.
        /// </summary>
        public SortedCollection<Initializer> Initializers
        {
            get { return m_initializers; }
        }

        /// <summary>
        /// Behaviours of the Emitter. Actions set the behaviour
        /// of particles created by the emitter.
        /// </summary>
        public SortedCollection<Behaviour> Behaviours
        {
            get { return m_behaviours; }
        }

        /// <summary>
        /// Activities of the Emitter. Activities set the behaviour
        /// of the Emitter.
        /// </summary>
        public SortedCollection<Activity> Activities
        {
            get { return m_activities; }
        }

        /// <summary>
        /// Used internally to create a particle.
        /// </summary>
        protected Particle CreateParticle()
        {
            Particle particle = m_particleFactory.CreateParticle();
            InitParticle(particle);
            for (int i = 0; i < m_initializers.Count; i++)
                m_initializers[i].Initialize(this, particle);

            m_particles.Add(particle);
            
            EventHandler<ParticleEventArgs> handler = ParticleCreated;
            if (handler != null)
                handler(this, new ParticleEventArgs(particle));

            return particle;
        }

        /// <summary>
        /// Emitters do their own particle initialization here - usually involves 
        /// positioning and rotating the particle to match the position and rotation 
        /// of the emitter. This method is called before any initializers that are
        /// assigned to the emitter, so initializers can override any properties set 
        /// here.
        /// </summary>
        protected virtual void InitParticle(Particle particle)
        {
            particle.PreviousX = particle.X = m_x;
            particle.PreviousY = particle.Y = m_y;
            particle.Rotation = m_rotation;
        }

        public void AddParticle(Particle particle)
        {
            Utils.RaiseEvent(ParticleAdded, this, new ParticleEventArgs(particle));
        }

        public void RemoveParticle(Particle particle)
        {
            Utils.RaiseEvent(ParticleRemoved, this, new ParticleEventArgs(particle));
        }

        /// <summary>
        /// Starts the emitter. Until start is called, the emitter will not emit any particles.
        /// </summary>
        public void Start()
        {
            m_running = true;

            uint len = (uint)m_activities.Count;

            for (int i = 0; i < len; i++)
                m_activities[i].Initialize(this);

            len = m_counter.StartEmitter(this);

            for (int i = 0; i < len; i++)
                CreateParticle();
        }

        /// <summary>
        /// Used internally and in derived public classes to update the emitter.
        /// </summary>
        /// <param name="elapsedTime">The duration, in seconds, of the current frame.</param>
        public void OnUpdateFrame(double elapsedTime)
        {
            if (!m_running)
                return;
            
            TimeSpan progress = TimeSpan.FromSeconds(elapsedTime);
            if (m_waitTime > progress)
            {
                m_waitTime -= progress;
                return;
            }

            elapsedTime = (progress - m_waitTime).TotalSeconds;
            m_waitTime = TimeSpan.Zero;

            uint len = m_counter.UpdateEmitter(this, elapsedTime);
            for (int i = 0; i < len; i++)
                CreateParticle();

            if (m_spaceSort)
                m_particles.Sort();

            foreach (Activity activity in m_activities)
                activity.Update(this, elapsedTime);

            m_particleFactory.Update(elapsedTime);

            if (m_particles.Count > 0)
            {
                foreach (Particle particle in m_particles)
                {
                    particle.PreviousX = particle.X;
                    particle.PreviousY = particle.Y;
                }

                foreach (Behaviour action in m_behaviours)
                {
                    foreach (Particle particle in m_particles)
                        action.Update(this, particle, elapsedTime);
                }

                foreach (Particle particle in Utils.Reverse(m_particles))
                {
                    if (particle.IsDead)
                    {
                        Utils.RaiseEvent(ParticleDied, this, new ParticleEventArgs(particle));

                        if (particle.IsDead)
                        {
                            m_particleFactory.DisposeParticle(particle);
                            m_particles.Remove(particle);
                        }
                    }
                }
            }
            
            Utils.RaiseEvent(Updated, this, new EmitterEventArgs(this));
        }

        /// <summary>
        /// Clear emitter settings back to defaults
        /// </summary>
        public void ClearAll()
        {
            m_particles.ForEach(m_particleFactory.DisposeParticle);

            m_behaviours.Clear();
            m_initializers.Clear();
            m_activities.Clear();
            m_particles.Clear();
        }

        /// <summary>
        /// Cleans up the emitter prior to removal. If you don't call this method,
        /// the garbage collector will clean up all the particles in teh usual way.
        /// If you use this method, the particles will be returned to the particle
        /// factory for reuse.
        /// </summary>
        public void Dispose()
        {
            int len = m_particles.Count;

            for (int i = 0; i < len; i++)
                m_particleFactory.DisposeParticle(m_particles[i]);

            m_particles.Clear();
        }

        /// <summary>
        /// Makes the emitter skip forwards a period of time with a single update.
        /// Used when you want the emitter to look like it's been running for a while. 
        /// </summary>
        /// <param name="time">The time, in seconds, to skip ahead.</param>
        public void RunAhead(double time)
        {
            double step = 1f / 30f; // 30 FPS

            while (time > 0)
            {
                time -= step;
                OnUpdateFrame(step);
            }
        }

        /// <summary>
        /// Indicates the wait time of the Emitter.
        /// </summary>
        public TimeSpan WaitTime
        {
            get { return m_waitTime; }
            set { m_waitTime = value; }
        }

        /// <summary>
        /// Indicates the x coordinate of the Emitter within the particle system's coordinate space.
        /// </summary>
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        /// <summary>
        /// Indicates the y coordinate of the Emitter within the particle system's coordinate space.
        /// </summary>
        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        /// <summary>
        /// Indicates the rotation of the Emitter, in degrees, within the particle system's coordinate space.
        /// </summary>
        public double Rotation
        {
            get { return Utils.RadiansToDegrees(m_rotation); }
            set { m_rotation = Utils.DegreesToRadians(value); }
        }

        /// <summary>
        /// Indicates the rotation of the Emitter, in radians, within the particle system's coordinate space.
        /// </summary>
        public double RotRadians
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }

        public bool SpaceSort
        {
            get { return m_spaceSort; }
            set { m_spaceSort = value; }
        }

        /// <summary>
        /// The array of all particles created by this emitter.
        /// </summary>
        public List<Particle> Particles
        {
            get { return m_particles; }
            set { m_particles = value; }
        }

        /// <summary>
        /// The Counter for the Emitter. The counter defines when and
        /// with what frequency the emitter emits particles.
        /// </summary>
        public ICounter Counter
        {
            get { return m_counter; }
            set { m_counter = value; }
        }

        public bool IsRunning
        {
            get { return m_running; }
        }

        /// <summary>
        /// This is the particle factory used by the emitter to create and dispose of particles.
        /// The default value is an instance of the ParticleCreator public class that is shared by all
        /// emitters. You don't usually need to alter this unless you are not using the default
        /// particle type. Any custom particle factory should implement the ParticleFactory public class.
        /// </summary>
        public IParticleFactory ParticleFactory
        {
            get { return m_particleFactory; }
        }
    }
}
