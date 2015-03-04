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
using System.IO;
using System.Windows;
using System.Windows.Markup;

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
    public class SharedImage : Initializer, IUriContext
    {
        private MaterialType m_materialType;
        private Uri m_baseUri;
        private string m_imagePath;

        public SharedImage()
        {
        }

        /// <summary>
        /// The constructor creates a SharedImage initializer for use by 
        /// an emitter. To add a SharedImage to all particles created by 
        /// an emitter, use the emitter's addInitializer method.
        /// </summary>
        /// <param name="uri"></param>
        public SharedImage(string imagePath)
        {
            m_imagePath = imagePath;
        }

        /// <summary>
        /// Base Uri of the image.
        /// </summary>
        public Uri BaseUri
        {
            get { return m_baseUri; }
            set { m_baseUri = value; }
        }

        /// <summary>
        /// The image path.
        /// </summary>
        public string ImagePath
        {
            get { return m_imagePath; }
            set { m_imagePath = value; }
        }

        /// <summary>
        /// The composition type of the image.
        /// </summary>
        public MaterialType MaterialType
        {
            get { return m_materialType; }
            set { m_materialType = value; }
        }

        public string GetNormalizedImagePath()
        {
            var path = (
                BaseUri == null ?
                m_imagePath :
                Path.Combine(BaseUri.OriginalString, m_imagePath));

            // Normalize separator
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            return path;
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
            if (Utils.ImageLoader == null)
            {
                throw new InvalidOperationException(
                    "Utils.ImageLoader isnt't initialized.");
            }

            var path = GetNormalizedImagePath();
            var data = Utils.ImageLoader.LoadImage(path, m_materialType);
            if (data == null)
            {
                Ragnarok.Log.Error(
                    "{0}: ?C???[?W???????????????s?????????B", path);
                return;
            }

            particle.ImageData = data;
        }
    }
}
