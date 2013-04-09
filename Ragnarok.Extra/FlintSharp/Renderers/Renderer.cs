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

using System.Collections;
using System.Collections.Generic;

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.Events;
using FlintSharp.EnergyEasing;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

namespace FlintSharp.Renderers
{
    /// <summary>
    /// The base class used by all the Flint renderers. This class manages
    /// various aspects of the rendering process.
    /// 
    /// <p>The class will add every emitter it should renderer to it's internal
    /// array of emitters. It will listen for the appropriate events on the 
    /// emitter and will then call the protected methods addParticle, removeParticle
    /// and renderParticles at the appropriate times. Many derived classes need 
    /// only implement these three methods to manage the rendering of the particles.</p>
    /// </summary>
    abstract public class Renderer
    {
        /// <summary>
        /// We retain assigned emitters in this array merely so the reference exists and they are not
        /// garbage collected. This ensures the expected behaviour is achieved - an emitter that exists
        /// on a renderer is not garbage collected, an emitter that does not exist on a renderer may be 
        /// garbage collected if no other references exist.
        /// </summary>
        private readonly EmitterCollection m_emitters;
        
        /// <summary>
        /// The constructor creates a Renderer class.
        /// </summary>
        public Renderer()
        {
            m_emitters = new EmitterCollection(this);
        }

        internal void particleAdded(object sender, ParticleEventArgs e)
        {
            AddParticle(e.Particle);
        }

        internal void particleRemoved(object sender, ParticleEventArgs e)
        {
            RemoveParticle(e.Particle);
        }

        internal void emitterUpdated(object sender, EmitterEventArgs e)
        {
            RenderParticles(e.Emitter.Particles);
        }

        /// <summary>
        /// The addParticle method is called when a particle is added to one of
        /// the emitters that is being rendered by this renderer.
        /// </summary>
        /// <param name="particle">The particle.</param>
        public virtual void AddParticle(Particle particle)
        {
        }

        /// <summary>
        /// The removeParticle method is called when a particle is removed from one
        /// of the emitters that is being rendered by this renderer.
        /// </summary>
        /// <param name="particle">The particle.</param>
        public virtual void RemoveParticle(Particle particle)
        {
        }

        /// <summary>
        /// The renderParticles method is called during the render phase of 
        /// every frame if the state of one of the emitters being rendered
        /// by this renderer has changed.
        /// </summary>
        /// <param name="particles">The particles being managed by all the emitters
        /// being rendered by this renderer. The particles are in no particular
        /// order.</param>
        public virtual void RenderParticles(IEnumerable<Particle> particles)
        {
        }

        /// <summary>
        /// Used internally and in derived public classes to update the emitter.
        /// </summary>
        /// <param name="elapsedTime">The duration, in seconds, of the current frame.</param>
        public virtual void OnUpdateFrame(double elapsedTime)
        {
            foreach (var emitter in m_emitters)
            {
                emitter.OnUpdateFrame(elapsedTime);
            }
        }

        /// <summary>
        /// The array of all emitters being rendered by this renderer.
        /// </summary>
        public EmitterCollection Emitters
        {
            get { return m_emitters; }
        }
    }
}
