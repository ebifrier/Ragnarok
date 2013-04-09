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

namespace FlintSharp.Initializers
{
    /// <summary>
	/// The Initializer public class is the abstract base public class for all particle initializers. Instances of 
	/// the Initializer public class should not be directly created because the Initializer public class itself 
	/// simply defines default methods that do nothing. Classes that extend the Initializer public class 
	/// implement their own functionality for the methods they want to use.
    /// 
	/// The Initializer public interface must be implemented by all particle initializers.
	/// <p>An Initializer is a public class that is used to set an aspect of a particle 
	/// when it is created. Initializers may, for example, set an initial velocity
	/// for a particle.</p>
	/// <p>Initializers are added to all particles created by an emitter by using the emitter's addInitializer
	/// method.<p>
	/// 
	// @see org.flintparticles.emitters.Emitter#addInitializer()
    /// </summary>
    abstract public class Initializer : IObjectBase
    {
        /// <summary>
		/// The constructor creates an Initializer object. But you shouldn't use it because the 
		/// Initializer public class is abstract.
        /// </summary>
        public Initializer()
        {
        }

        /// <summary>
		/// The getDefaultPriority method is used to order the execution of initializers.
		/// It is called within the emitter's addInitializer method when the user doesn't
		/// manually set a priority. It need not be called directly by the user.
	 	/// 
		/// @see org.flintparticles.emitters.Emitter#addInitializer()
        /// </summary>
        public virtual int GetDefaultPriority()
        {
            return 0;
        }

        /// <summary>
		/// The addedToEmitter method is called from the emitter when the Initializer is added to it
		/// It is called within the emitter's addInitializer method and need not
		/// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Initializer was added to.</param>
        public virtual void AddedToEmitter(Emitter emitter)
        {
        }

        /// <summary>
		/// The removedFromEmitter method is called by the emitter when the Initializer is removed from it
		/// It is called within the emitter's removeInitializer method and need not
		/// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Initializer was removed from.</param>
        public virtual void RemovedFromEmitter(Emitter emitter)
        {
        }

        /// <summary>
		/// The initialize method is used by the emitter to initialize the particle.
		/// It is called within the emitter's createParticle method and need not
		/// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that created the particle.</param>
        /// <param name="particle">The particle to be initialized.</param>
        public abstract void Initialize(Emitter emitter, Particle particle);
    }
}
