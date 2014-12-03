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

namespace FlintSharp.Counters
{
    /// <summary>
    /// The Zero counter causes the emitter to emit no particles. Because the emitter
    /// requires a counter, this counter is used as the default and should be used whenever
    /// you don't want a counter.
    /// </summary>
    public class ZeroCounter : ICounter
    {
        /// <summary>
        /// The constructor creates a Zero counter for use by an emitter. To
        /// add a Zero counter to an emitter use the emitter's counter property.
        /// </summary>
        public ZeroCounter()
        {
        }

        /// <summary>
        /// The startEmitter method is called when the emitter starts.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <returns>The number of particles the emitter should emit
        /// at the moment it starts.</returns>
        public int StartEmitter(Emitter emitter)
        {
            return 0;
        }

        /// <summary>
        /// The updateEmitter method is called every frame after the
        /// emitter has started.
        /// </summary>
        /// <param name="emitter">The emitter</param>
        /// <param name="elapsedTime">The time, in seconds, since the previous call to this method.</param>
        /// <returns>The number of particles the emitter should emit
        /// at this time.</returns>
        public int UpdateEmitter(Emitter emitter, double elapsedTime)
        {
            return 0;
        }
    }
}
