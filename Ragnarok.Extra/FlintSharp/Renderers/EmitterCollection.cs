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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FlintSharp.Emitters;

namespace FlintSharp.Renderers
{
    /// <summary>
    /// The base collection class which can hook the add/remove actions.
    /// </summary>
    public class EmitterCollection : GeneralCollection<Emitter>
    {
        private Renderer m_renderer;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EmitterCollection(Renderer renderer)
        {
            m_renderer = renderer;
        }

        /// <summary>
        /// Adds the emitter to the collection. When an emitter is added, the renderer
        /// invalidates its display so the renderParticles method will be called
        /// on the next render event in the frame update.
        /// </summary>
        /// <param name="emitter">The emitter that is added to the renderer.</param>
        protected override void OnAdded(int index, Emitter element)
        {
            // indexは無視します。
            m_implList.Insert(index, element);

            if (element != null)
            {
                element.Updated += m_renderer.emitterUpdated;
                element.ParticleCreated += m_renderer.particleAdded;
                element.ParticleAdded += m_renderer.particleAdded;
                element.ParticleDied += m_renderer.particleRemoved;
                element.ParticleRemoved += m_renderer.particleRemoved;

                foreach (var p in element.Particles)
                {
                    m_renderer.AddParticle(p);
                }
            }
        }

        /// <summary>
        /// Removes the emitter from the collection. When an emitter is removed, the renderer
        /// invalidates its display so the renderParticles method will be called
        /// on the next render event in the frame update.
        /// </summary>
        /// <param name="emitter">The emitter that is removed from the renderer.</param>
        protected override void OnRemoved(int index, Emitter element)
        {
            m_implList.RemoveAt(index);

            if (element != null)
            {
                foreach (var p in element.Particles)
                {
                    m_renderer.RemoveParticle(p);
                }

                element.Updated -= m_renderer.emitterUpdated;
                element.ParticleCreated -= m_renderer.particleAdded;
                element.ParticleAdded -= m_renderer.particleAdded;
                element.ParticleDied -= m_renderer.particleRemoved;
                element.ParticleRemoved -= m_renderer.particleRemoved;
            }
        }
    }
}
