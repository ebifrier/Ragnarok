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
using System.Drawing;
using System.Drawing.Drawing2D;

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
    /// The TextZone zone defines a text zone. The zone implementation is
    /// almost same as BitmapDataZone.
    /// </summary>
    public class TextZone : IZone
    {
        private List<Point> m_validPoints = new List<Point>();
        private Bitmap m_bitmap;
        private string m_text = string.Empty;
        private Font m_font;
        private FontFamily m_fontFamily;
        private FontStyle m_fontStyle = FontStyle.Regular;
        private double m_fontSize = 12.0;
        private bool m_textChanged = true;
        private bool m_fontChanged = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextZone()
        {
            this.m_fontFamily = System.Drawing.FontFamily.GenericSansSerif;
        }

        /// <summary>
        /// A text string to be drawn.
        /// </summary>
        public string Text
        {
            get { return m_text; }
            set { m_text = value; m_textChanged = true; }
        }

        /// <summary>
        /// A font object used when the text is draw.
        /// </summary>
        public Font Font
        {
            get { return m_font; }
            set { m_font = value; m_textChanged = true; }
        }

        public string FontFamily
        {
            get { return m_fontFamily.Name; }
            set { m_fontFamily = new FontFamily(value); m_fontChanged = true; }
        }

        public FontStyle FontStyle
        {
            get { return m_fontStyle; }
            set { m_fontStyle = value; m_fontChanged = true; }
        }

        public double FontSize
        {
            get { return m_fontSize; }
            set { m_fontSize = value; m_fontChanged = true; }
        }

        /// <summary>
        /// A horizontal offset to apply to the pixels in the BitmapData object 
        /// to reposition the zone
        /// </summary>
        public double OffsetX
        {
            get;
            set;
        }

        /// <summary>
        /// A vertical offset to apply to the pixels in the BitmapData object 
		/// to reposition the zone
        /// </summary>
        public double OffsetY
        {
            get;
            set;
        }

        /// <summary>
        /// A scale factor to stretch the bitmap horizontally
        /// </summary>
        public double ScaleX
        {
            get;
            set;
        }

        /// <summary>
        /// A scale factor to stretch the bitmap vertically
        /// </summary>
        public double ScaleY
        {
            get;
            set;
        }

        public int DivideX
        {
            get;
            set;
        }

        public int DivideY
        {
            get;
            set;
        }

        /// <summary>
        /// Measures the string extent.
        /// </summary>
        private SizeF MeasureString()
        {
            using (Bitmap dummy = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(dummy))
            {
                return g.MeasureString(Text, Font);
            }
        }

        /// <summary>
        /// Creates the bitmap image which the string is drawn.
        /// </summary>
        private Bitmap CreateFontBitmap()
        {
            SizeF size = MeasureString();
            Bitmap bitmap = new Bitmap(
                (int)Math.Ceiling(size.Width) + 1,
                (int)Math.Ceiling(size.Height) + 1);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // For speed
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.Low;
                g.CompositingQuality = CompositingQuality.Default;

                // This line makes DrawString fail.
                //g.CompositingMode = CompositingMode.SourceCopy;

                g.DrawString(Text, Font, Brushes.White, new PointF(0, 0));
                return bitmap;
            }
        }

        /// <summary>
        /// This method forces the zone to revaluate the font object.
        /// </summary>
        private void UpdateFont()
        {
            m_font = new Font(m_fontFamily, (float)m_fontSize, m_fontStyle, GraphicsUnit.Pixel);
            m_fontChanged = false;
            m_textChanged = true;
        }

        /// <summary>
		/// This method forces the zone to revaluate itself. It should be called whenever the 
		/// contents of the Text object change. However, it is an intensive method and 
        /// calling it frequently will likely slow your code down.
        /// </summary>
        private void UpdateBitmap()
        {
            Bitmap bitmap = CreateFontBitmap();
            List<Point> validPoints = new List<Point>();

            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    Color c = bitmap.GetPixel(x, y);

                    if (c.R == 255)
                        validPoints.Add(new Point(x, y));
                }
            }

            m_bitmap = bitmap;
            m_validPoints = validPoints;
            m_textChanged = false;
        }

        /// <summary>
        /// Recreates the bitmap or the font if need.
        /// </summary>
        private void UpdateIfNeed()
        {
            if (m_fontChanged)
                UpdateFont();

            if (m_textChanged)
                UpdateBitmap();
        }

        /// <summary>
        /// The contains method determines whether a point is inside the zone.
		/// </summary>
        /// <param name="x">The x location to test for.</param>
        /// <param name="y">The x location to test for.</param>
        /// <returns>true if point is inside the zone, false if it is outside.</returns>
        public bool Contains(double x, double y)
        {
            UpdateIfNeed();

            if (m_bitmap == null)
                return false;

            double w2 = m_bitmap.Width / 2.0;
            double h2 = m_bitmap.Height / 2.0;
            int newX = (int)Math.Round((x - OffsetX) / ScaleX + w2);
            int newY = (int)Math.Round((y - OffsetY) / ScaleY + h2);

            if (newX < 0 || newX > m_bitmap.Width)
                return false;

            if (newY < 0 || newY > m_bitmap.Height)
                return false;

            Color c = m_bitmap.GetPixel(newX, newY);
            return (c.R == 255);
        }

        private System.Windows.Point CalcPoint(int index, double rx, double ry)
        {
            Point p = m_validPoints[index];

            // Adjust the point to center.
            double w2 = m_bitmap.Width / 2.0;
            double h2 = m_bitmap.Height / 2.0;
            double rateX = 1.0 + rx;
            double rateY = 1.0 + ry;

            return new System.Windows.Point(
                (p.X + rateX - w2) * ScaleX + OffsetX,
                (p.Y + rateY - h2) * ScaleY + OffsetY);
        }

        /// <summary>
        /// Counts of the particles.
        /// </summary>
        public int Count()
        {
            UpdateIfNeed();

            return (m_validPoints.Count * DivideX * DivideY);
        }

        /// <summary>
        /// The list of the particle points.
        /// </summary>
        public IEnumerable<System.Windows.Point> GetLocationList()
        {
            UpdateIfNeed();

            double offsetX = 0.5 / DivideX;
            double offsetY = 0.5 / DivideY;

            for (var index = 0; index < m_validPoints.Count; ++index)
            {
                for (var dx = 0; dx < DivideX; ++dx)
                {
                    for (var dy = 0; dy < DivideY; ++dy)
                    {
                        // offset + 0.0 <= rate < offset + 1.0
                        double rateX = offsetX + (double)dx / DivideX;
                        double rateY = offsetY + (double)dy / DivideY;

                        yield return CalcPoint(index, rateX, rateY);
                    }
                }
            }
        }

        /// <summary>
        /// The getLocation method returns a random point inside the zone.
        /// </summary>
        /// <returns>a random point inside the zone.</returns>
        public System.Windows.Point GetLocation()
        {
            UpdateIfNeed();

            return CalcPoint(
                Utils.Random.Next(m_validPoints.Count),
                (double)Utils.Random.Next(DivideX) / DivideX,
                (double)Utils.Random.Next(DivideY) / DivideY);
        }

        /// <summary>
        /// The getArea method returns the size of the zone.
		/// It's used by the MultiZone class to manage the balancing between the
		/// different zones.
        /// </summary>
        /// <returns>the size of the zone.</returns>
        public double GetArea()
        {
            UpdateIfNeed();

            return m_validPoints.Count * ScaleX * ScaleY;
        }

        /// <summary>
        /// Manages collisions between a particle and the zone. The particle will collide with the line defined
        /// for this zone. In the interests of speed, the collisions are not exactly accurate at the ends of the
        /// line, but are accurate enough to ensure the particle doesn't pass through the line and to look
        /// realistic in most circumstances. The collisionRadius of the particle is used when calculating the collision.
        /// </summary>
        /// <param name="particle">The particle to be tested for collision with the zone.</param>
        /// <param name="bounce">The coefficient of restitution for the collision.</param>
        /// <returns>Whether a collision occured.</returns>
        public bool CollideParticle(Particle particle, double bounce = 1)
        {
            throw new NotImplementedException();
        }
    }
}
