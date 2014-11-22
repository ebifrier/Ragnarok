using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using FlintSharp;

namespace Ragnarok.Presentation.Extra.FlintSharpEx
{
    /// <summary>
    /// WPF用のイメージデータを読み込みます。
    /// </summary>
    public sealed class WPFImageLoader : IImageLoader
    {
        private readonly Dictionary<Uri, BitmapImage> imageDic =
            new Dictionary<Uri, BitmapImage>();
        private BitmapImage particleImage;

        public static readonly Uri ParticleImageUri = new Uri(
            "pack://application:,,,/Ragnarok.Extra;component/FlintSharp/Particle.png");

        /// <summary>
        /// Creates a new Image.
        /// </summary>
        private BitmapImage CreateImage(Uri uri)
        {
            try
            {
                var source = new BitmapImage(uri);
                source.Freeze();

                return source;
            }
            catch /*(Exception ex)*/
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the image brush from the cache or creates it if need.
        /// </summary>
        private BitmapImage GetImage(Uri uri)
        {
            lock (imageDic)
            {
                BitmapImage source;

                if (imageDic.TryGetValue(uri, out source))
                {
                    return source;
                }

                source = CreateImage(uri);
                imageDic[uri] = source;
                return source;
            }
        }

        /// <summary>
        /// Returns the image brush from the cache or creates it if need.
        /// </summary>
        private BitmapImage GetParticleImage()
        {
            try
            {
                if (this.particleImage != null)
                {
                    return this.particleImage;
                }

                var source = new BitmapImage(ParticleImageUri);
                source.Freeze();

                this.particleImage = source;
                return source;
            }
            catch /*(Exception ex)*/
            {
                return null;
            }
        }

        /// <summary>
        /// 通常のイメージを読み込みます。
        /// </summary>
        [CLSCompliant(false)]
        public IImageData LoadImage(Uri imageUri, MaterialType materialType)
        {
            var image = GetImage(imageUri);
            var brush = new ImageBrush()
            {
                ImageSource = image,
            };

            var mesh = WPFImageData.CreateDefaultMesh(
                1.0, 1.0, image.Width, image.Height);
            mesh.Freeze();

            var materialWrap = new MaterialWrap
            {
                MaterialType = materialType,
                Brush = brush,
                Color = Colors.White,
            };

            GeometryModel3D model = new GeometryModel3D()
            {
                Geometry = mesh,
                Material = materialWrap.Create(),
            };

            return new WPFImageData
            {
                Model = model,
                Brush = brush,
                Material = model.Material,
            };
        }

        /// <summary>
        /// パーティクル用の画像を読み込みます。
        /// </summary>
        [CLSCompliant(false)]
        public IImageData LoadParticleImage(bool isSingle, MaterialType materialType)
        {
            BitmapImage image = GetParticleImage();
            ImageBrush brush = new ImageBrush(image);

            if (isSingle)
            {
                MaterialWrap materialWrap = new MaterialWrap()
                {
                    MaterialType = materialType,
                    Brush = brush,
                    Color = Colors.White,
                };
                Material material = materialWrap.Create();

                GeometryModel3D model = new GeometryModel3D()
                {
                    Geometry = WPFImageData.CreateDefaultMesh(
                        1.0, 1.0, image.Width, image.Height),
                    Material = material,
                };

                return new WPFImageData
                {
                    Brush = brush,
                    Material = material,
                    Model = model,
                };
            }
            else
            {
                // front
                MaterialWrap materialWrap1 = new MaterialWrap()
                {
                    MaterialType = materialType,
                    Brush = brush,
                    Color = Colors.White,
                };

                // back
                MaterialWrap materialWrap2 = new MaterialWrap()
                {
                    MaterialType = materialType,
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
                    Geometry = WPFImageData.CreateDefaultMesh(
                        1.0, 1.0, image.Width, image.Height),
                    Material = matGroup,
                };

                return new WPFImageData
                {
                    Brush = brush,
                    Material = material1,
                    Model = model,
                };
            }
        }
    }
}
