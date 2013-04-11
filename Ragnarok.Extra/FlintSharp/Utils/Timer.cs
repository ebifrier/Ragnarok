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

using System.Runtime.InteropServices;

namespace FlintSharp
{
    public partial class Win32
    {
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceCounter(ref long PerformanceCount);
    }

    public class Timer
    {
        private long m_ticksPerSecond;
        private long m_currentTime;
        private long m_lastTime;
        private long m_lastFPSUpdate;
        private long m_FPSUpdateInterval;
        private uint m_numFrames;
        private double m_runningTime;
        private double m_timeElapsed;
        private double m_fps;
        private bool m_timerStopped;

        public Timer()
        {
            Win32.QueryPerformanceFrequency(ref m_ticksPerSecond);

            m_timerStopped = true;
            m_FPSUpdateInterval = m_ticksPerSecond >> 1;
        }

        public void Start()
        {
            if (!Stopped)
                return;

            Win32.QueryPerformanceCounter(ref m_lastTime);
            m_timerStopped = false;
        }

        public void Stop()
        {
            if (Stopped)
                return;

            long stopTime = 0;

            Win32.QueryPerformanceCounter(ref stopTime);

            m_runningTime += (double)(stopTime - m_lastTime) / m_ticksPerSecond;
            m_timerStopped = true;
        }

        public void Update()
        {
            if (Stopped)
                return;

            Win32.QueryPerformanceCounter(ref m_currentTime);

            m_timeElapsed = (double)(m_currentTime - m_lastTime) / m_ticksPerSecond;
            m_runningTime += m_timeElapsed;

            m_numFrames++;

            if (m_currentTime - m_lastFPSUpdate >= m_FPSUpdateInterval)
            {
                double currentTime = (double)m_currentTime / m_ticksPerSecond;
                double lastTime = (double)m_lastFPSUpdate / m_ticksPerSecond;

                m_fps = m_numFrames / (currentTime - lastTime);

                m_lastFPSUpdate = m_currentTime;
                m_numFrames = 0;
            }

            m_lastTime = m_currentTime;
        }

        public bool Stopped
        {
            get { return m_timerStopped; }
        }

        public double FPS
        {
            get { return m_fps; }
        }

        public double ElapsedTime
        {
            get
            {
                if (Stopped)
                    return 0;

                return m_timeElapsed;
            }
        }

        public double RunningTime
        {
            get { return m_runningTime; }
        }
    }
}