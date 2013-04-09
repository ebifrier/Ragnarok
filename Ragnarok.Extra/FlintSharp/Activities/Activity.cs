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

namespace FlintSharp.Activities
{
    /// <summary>
    /// The Activity public class is the abstract base public class for all emitter activities. Instances of the 
    /// Activity public class should not be directly created because the Activity public class itself simply defines 
    /// default methods that do nothing. Classes that extend the Activity public class implement their 
    /// own functionality for the methods they want to use.
    /// 
    /// <p>An Activity is a public class that is used to modify the 
    /// behaviour of the emitter over time. It may, for example, move or
    /// rotate the emitter.</p>
    /// 
    /// <p>Activities are added to the emitter by using its addActivity method.</p> 
    /// 
    /// @see org.flintparticles.emitters.Emitter#addActivity()
    /// </summary>
    abstract public class Activity : IObjectBase
    {
        /// <summary>
        /// The constructor creates an Activity object. But you shouldn't use it because the Activity
        /// public class is abstract.
        /// </summary>
        public Activity()
        {
        }

        /// <summary>
        /// The getDefaultPriority method is used to order the execution of activities.
        /// It is called within the emitter's addActivity method when the user doesn't
        /// manually set a priority. It need not be called directly by the user.
        /// 
        /// @see org.flintparticles.emitters.Emitter#addActivity()
        /// </summary>
        /// <returns></returns>
        public virtual int GetDefaultPriority()
        {
            return 0;
        }

        /// <summary>
        /// The addedToEmitter method is called from the emitter when the Activity is added to it
        /// It is called within the emitter's addActivity method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Activity was added to.</param>
        public virtual void AddedToEmitter(Emitter emitter)
        {
        }

        /// <summary>
        /// The removedFromEmitter method is called by the emitter when the Activity is removed from it
        /// It is called within the emitter's removeActivity method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that the Activity was removed from.</param>
        public virtual void RemovedFromEmitter(Emitter emitter)
        {
        }

        /// <summary>
        /// The initialize method is used by the emitter to start the activity.
        /// It is called within the emitter's start method and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that is using the activity.</param>
        public virtual void Initialize(Emitter emitter)
        {
        }

        /// <summary>
        /// The update method is used by the emitter to apply the activity.
        /// It is called within the emitter's update loop and need not
        /// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that is using the activity.</param>
        /// <param name="elapsedTime">The duration of the frame - used for time based updates.</param>
        public abstract void Update(Emitter emitter, double elapsedTime);
    }
}
