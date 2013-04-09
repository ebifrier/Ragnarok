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
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

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
	/// The SharedImage Initializer sets the DisplayObject to use to draw
	/// the particle. It is used with the BitmapRenderer. When using the
	/// DisplayObjectRenderer the ImageClass Initializer must be used.
	/// 
	/// With the BitmapRenderer, the DisplayObject is copied into the bitmap
	/// using the particle's property to place the image correctly. So
	/// many particles can share the same DisplayObject because it is
    /// only indirectly used to display the particle.
	/// </summary>
    public class ParticleImage : Initializer
    {
        private static BitmapImage defaultImage;
        private MaterialType m_materialType = MaterialType.Emissive;

        public ParticleImage()
        {
            IsSingle = false;
        }

        /// <summary>
        /// Wheather the image is single layer.
        /// </summary>
        public bool IsSingle
        {
            get;
            set;
        }

        /// <summary>
        /// The composition type of the image.
        /// </summary>
        public MaterialType MaterialType
        {
            get { return m_materialType; }
            set { m_materialType = value; }
        }

        /// <summary>
        /// Returns the image brush from the cache or creates it if need.
        /// </summary>
        private BitmapImage GetImage()
        {
            try
            {
                if (defaultImage != null)
                {
                    return defaultImage;
                }

                var uri = new Uri("pack://application:,,,/Ragnarok.Extra;component/FlintSharp/Particle.png");
                var source = new BitmapImage(uri);
                source.Freeze();

                defaultImage = source;
                return source;
            }
            catch /*(Exception ex)*/
            {
                return null;
            }
        }

        /// <summary>
		/// The initialize method is used by the emitter to initialize the particle.
		/// It is called within the emitter's createParticle method and need not
		/// be called by the user.
        /// </summary>
        /// <param name="emitter">The Emitter that created the particle.</param>
        /// <param name="particle">The particle to be initialized.</param>
        public override void Initialize(Emitter emitter, Particle particle)
        {
            BitmapImage image = GetImage();
            ImageBrush brush = new ImageBrush(image);

            if (IsSingle)
            {
                MaterialWrap materialWrap = new MaterialWrap()
                {
                    MaterialType = MaterialType,
                    Brush = brush,
                    Color = Colors.White,
                };
                Material material = materialWrap.Create();

                GeometryModel3D model = new GeometryModel3D()
                {
                    Geometry = Particle.CreateDefaultMesh(1.0, 1.0, image.Width, image.Height),
                    Material = material,
                };

                particle.Brush = brush;
                particle.Material = material;
                particle.Model = model;
            }
            else
            {
                // front
                MaterialWrap materialWrap1 = new MaterialWrap()
                {
                    MaterialType = MaterialType,
                    Brush = brush,
                    Color = Colors.White,
                };

                // back
                MaterialWrap materialWrap2 = new MaterialWrap()
                {
                    MaterialType = MaterialType,
                    Brush = brush,
                    Color = Color.FromArgb(0xC0, 0xFF, 0xFF, 0xFF),
                };

                Material material1 = materialWrap1.Create();
                Material material2 = materialWrap2.Create();

                MaterialGroup matGroup = new MaterialGroup();
                matGroup.Children.Add(material2);
                matGroup.Children.Add(material1);

                GeometryModel3D model = new GeometryModel3D()
                {
                    Geometry = Particle.CreateDefaultMesh(1.0, 1.0, image.Width, image.Height),
                    Material = matGroup,
                };

                particle.Brush = brush;
                particle.Material = material1;
                particle.Model = model;
            }
        }
    }
}
