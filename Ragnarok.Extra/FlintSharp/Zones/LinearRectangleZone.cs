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
using System.Windows;

using FlintSharp.Behaviours;
using FlintSharp.Activities;
using FlintSharp.Counters;
using FlintSharp.Easing;
using FlintSharp.Emitters;
using FlintSharp.EnergyEasing;
using FlintSharp.Initializers;
using FlintSharp.Particles;
using FlintSharp.Zones;

namespace FlintSharp.Zones
{
    /// <summary>
	/// The LinearRectangleZone defines a rectangle shaped zone in which locations are returned in a 
	/// regular linear pattern, rather than the random pattern of the standard RectangleZone.
    /// </summary>
    public class LinearRectangleZone : RectangleZone
    {
        private ZoneDirection m_direction;
        private ZonePosition m_startPosition;
        private double m_stepX;
        private double m_stepY;
        private double m_x;
        private double m_y;
        private UpdateDelegate UpdateFunction = null;

        public delegate void UpdateDelegate();

        public LinearRectangleZone()
            : this(0, 0, 1, 1)
        {
        }

        public LinearRectangleZone(double left, double top, double right, double bottom)
            : base(left, top, right, bottom)
        {
            m_startPosition = ZonePosition.TopLeft;
            m_direction = ZoneDirection.Horizontal;
            m_stepX = 1;
            m_stepY = 1;
            Init();
        }

        public LinearRectangleZone(double left, double top, double right, double bottom, ZonePosition startPosition, ZoneDirection direction)
            : base(left, top, right, bottom)
        {
            m_startPosition = startPosition;
            m_direction = direction;
            m_stepX = 1;
            m_stepY = 1;
            Init();
        }

        /// <summary>
        /// The constructor creates a LinearRectangleZone zone.
        /// </summary>
        /// <param name="left">The left coordinate of the rectangle defining the region of the zone.</param>
        /// <param name="top">The top coordinate of the rectangle defining the region of the zone.</param>
        /// <param name="right">The right coordinate of the rectangle defining the region of the zone.</param>
        /// <param name="bottom">The bottom coordinate of the rectangle defining the region of the zone.</param>
        /// <param name="start">The position in the zone to start getting locations - may be any corner
        /// of the rectangle.</param>
        /// <param name="direction">The direction to advance first. If ZoneDirection.Horizontal, the locations
        /// advance horizontally across the zone, moving vertically when wrapping around at the end. If
        /// ZoneDirection.Vertical, the locations advance vertically up/down the zone, moving horizontally 
        /// when wrapping around at the top/bottom.</param>
        /// <param name="horizontalStep"></param>
        /// <param name="verticalStep"></param>
        public LinearRectangleZone(double left, double top, double right, double bottom, ZonePosition start, ZoneDirection direction, double horizontalStep, double verticalStep)
            : base(left, top, right, bottom)
        {
            m_startPosition = start;
            m_direction = direction;
            m_stepX = Math.Abs(horizontalStep);
            m_stepY = Math.Abs(verticalStep);
            Init();
        }

        private void Init()
        {
            switch (m_startPosition)
            {
                case ZonePosition.TopLeft:
                    if (m_direction == ZoneDirection.Vertical)
                    {
                        m_x = Left;
                        m_y = Top - m_stepY;
                        UpdateFunction = new UpdateDelegate(UpdateLocationTopLeftVertical);
                    }
                    else
                    {
                        m_x = Left - m_stepX;
                        m_y = Top;
                        UpdateFunction = new UpdateDelegate(UpdateLocationTopLeftHorizontal);
                    }
                    break;
                case ZonePosition.TopRight:
                    if (m_direction == ZoneDirection.Vertical)
                    {
                        m_x = Right - 1;
                        m_y = Top - m_stepY;
                        UpdateFunction = new UpdateDelegate(UpdateLocationTopRightVertical);
                    }
                    else
                    {
                        m_x = Right - 1 + m_stepX;
                        m_y = Top;
                        UpdateFunction = new UpdateDelegate(UpdateLocationTopRightHorizontal);
                    }
                    break;
                case ZonePosition.BottomLeft:
                    if (m_direction == ZoneDirection.Vertical)
                    {
                        m_x = Left;
                        m_y = Bottom - 1 + m_stepY;
                        UpdateFunction = new UpdateDelegate(UpdateLocationBottomLeftVertical);
                    }
                    else
                    {
                        m_x = Left - m_stepX;
                        m_y = Bottom - 1;
                        UpdateFunction = new UpdateDelegate(UpdateLocationBottomLeftHorizontal);
                    }
                    break;
                case ZonePosition.BottomRight:
                    if (m_direction == ZoneDirection.Vertical)
                    {
                        m_x = Right - 1;
                        m_y = Bottom - 1 + m_stepY;
                        UpdateFunction = new UpdateDelegate(UpdateLocationBottomRightVertical);
                    }
                    else
                    {
                        m_x = Right - 1 + m_stepX;
                        m_y = Bottom - 1;
                        UpdateFunction = new UpdateDelegate(UpdateLocationBottomRightHorizontal);
                    }
                    break;
            }
        }

        /// <summary>
        ///
        /// One of these methods will be used in the getLocation function,
        /// depending on the start point and direction.
        ///
        /// </summary>
        private void UpdateLocationTopLeftHorizontal()
        {
            m_x += m_stepX;

            if (m_x >= Right)
            {
                m_x -= Width;
                m_y += m_stepY;

                if (m_y >= Bottom)
                    m_y -= Height;
            }
        }

        private void UpdateLocationTopRightHorizontal()
        {
            m_x -= m_stepX;

            if (m_x < Left)
            {
                m_x += Width;
                m_y += m_stepY;

                if (m_y >= Bottom)
                    m_y -= Height;
            }
        }

        private void UpdateLocationBottomLeftHorizontal()
        {
            m_x += m_stepX;

            if (m_x >= Right)
            {
                m_x -= Width;
                m_y -= m_stepY;

                if (m_y < Top)
                    m_y += Height;
            }
        }

        private void UpdateLocationBottomRightHorizontal()
        {
            m_x -= m_stepX;

            if (m_x < Left)
            {
                m_x += Width;
                m_y -= m_stepY;

                if (m_y < Top)
                    m_y += Height;
            }
        }

        private void UpdateLocationTopLeftVertical()
        {
            m_y += m_stepY;

            if (m_y >= Bottom)
            {
                m_y -= Height;
                m_x += m_stepX;

                if (m_x >= Right)
                    m_x -= Width;
            }
        }

        private void UpdateLocationTopRightVertical()
        {
            m_y += m_stepY;

            if (m_y >= Bottom)
            {
                m_y -= Height;
                m_x -= m_stepX;

                if (m_x < Left)
                    m_x += Width;
            }
        }

        private void UpdateLocationBottomLeftVertical()
        {
            m_y -= m_stepY;

            if (m_y < Top)
            {
                m_y += Height;
                m_x += m_stepX;

                if (m_x >= Right)
                    m_x -= Width;
            }
        }

        private void UpdateLocationBottomRightVertical()
        {
            m_y -= m_stepY;

            if (m_y < Top)
            {
                m_y += Height;
                m_x -= m_stepX;

                if (m_x < Left)
                    m_x += Width;
            }
        }

        /// <summary>
        /// The getArea method returns the size of the zone.
        /// It's used by the MultiZone public class to manage the balancing between the
        /// different zones. Usually, it need not be called directly by the user.
        /// </summary>
        /// <returns>the size of the zone.</returns>
        public new Point GetLocation()
        {
            UpdateFunction();
            return new Point(m_x, m_y);
        }
    }
}
