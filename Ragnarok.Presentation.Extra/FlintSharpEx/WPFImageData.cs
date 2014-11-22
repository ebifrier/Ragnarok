using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using FlintSharp;
using FlintSharp.Particles;

namespace Ragnarok.Presentation.Extra.FlintSharpEx
{
    public class WPFImageData : IImageData
    {
        /// <summary>
        /// 単純な四角形のジオメトリを作成します。
        /// </summary>
        public static MeshGeometry3D CreateDefaultMesh(
            double width, double height,
            double imageWidth, double imageHeight)
        {
            double texAdjX = (imageWidth != 0.0 ? 30.0 / imageWidth : 0.0);
            double texAdjY = (imageHeight != 0.0 ? 30.0 / imageHeight : 0.0);

            return new MeshGeometry3D
            {
                Positions =
                {
                    new Point3D(-width / 2, -height / 2, 0),
                    new Point3D(width / 2, -height / 2, 0),
                    new Point3D(-width / 2, height / 2, 0),
                    new Point3D(width / 2, height / 2, 0),
                },
                TextureCoordinates =
                {
                    new Point(0.0,           0.0),
                    new Point(1.0 - texAdjX, 0.0),
                    new Point(0.0,           1.0 - texAdjY),
                    new Point(1.0 - texAdjX, 1.0 - texAdjY),
                },
                TriangleIndices =
                {
                    0, 2, 1,
                    1, 2, 3,
                },
            };
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WPFImageData()
        {
            Brush = new SolidColorBrush(Colors.White);

            Material = new EmissiveMaterial()
            {
                Brush = Brush,
                Color = Colors.White,
            };

            Model = new GeometryModel3D()
            {
                Geometry = CreateDefaultMesh(1.0, 1.0, 0, 0),
                Material = Material,
            };
        }

        public void Reset()
        {
            Model.Transform = null;
        }

        /// <summary>
        /// 描画用のモデルを取得または設定します。
        /// </summary>
        public GeometryModel3D Model
        {
            get;
            set;
        }

        /// <summary>
        /// マテリアルを取得または設定します。
        /// </summary>
        public Material Material
        {
            get;
            set;
        }

        /// <summary>
        /// 描画用のブラシを取得または設定します。
        /// </summary>
        public Brush Brush
        {
            get;
            set;
        }
    }
}
