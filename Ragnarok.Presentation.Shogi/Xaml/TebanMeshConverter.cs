using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Ragnarok.Shogi;

namespace Ragnarok.Presentation.Shogi.Xaml
{
    /// <summary>
    /// 手番表示のメッシュを作成するコンバーターです。
    /// </summary>
    [ValueConversion(typeof(BWType), typeof(MeshGeometry3D))]
    public sealed class TebanMeshConverter : IValueConverter
    {
        /// <summary>
        /// マスの位置からメッシュを作成します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            // マスの数９×マスのサイズ(1.0)＋左右の幅(0.4 * 2) = 9.8
            var points = new Point3DCollection();
            var indices = new Int32Collection();
            var bwType = (BWType)value;
            var w = 0.4 / 9.0;
            var h = 9.0 / 9.0;
            var t0 = h * 0.5;
            var t1 = h * 0.8;

            var rectList = new List<Rect>
            {
                new Rect(-w, t0, w, t1 - t0),
                new Rect(-w, t1, w, h - t1 + w),

                new Rect(1.0, t0, w, t1 - t0),
                new Rect(1.0, t1, w, h - t1 + w),

                new Rect(0, 1.0, h, w),
            };

            // テクスチャ座標
            var texCoords = new PointCollection
            {
                new Point(0, 0.0), new Point(0, 0.5),
                new Point(1, 0.0), new Point(1, 0.5),
                new Point(1, 0.0), new Point(1, 0.5),
                new Point(1, 0.0), new Point(1, 0.5),

                new Point(0, 0.0), new Point(0, 0.5),
                new Point(1, 0.0), new Point(1, 0.5),
                new Point(1, 0.0), new Point(1, 0.5),
                new Point(1, 0.0), new Point(1, 0.5),

                new Point(1, 0.0), new Point(1, 0.0),
                new Point(1, 0.5), new Point(1, 0.5),
            };

            foreach (var rect in rectList)
            {
                var c = points.Count;
                var l = (bwType == BWType.Black ? rect.Left : 1.0 - rect.Left);
                var t = (bwType == BWType.Black ? rect.Top : 1.0 - rect.Top);
                var r = (bwType == BWType.Black ? rect.Right : 1.0 - rect.Right);
                var b = (bwType == BWType.Black ? rect.Bottom : 1.0 - rect.Bottom);

                // 各マスの座標追加
                points.Add(new Point3D(l, t, 0));
                points.Add(new Point3D(r, t, 0));
                points.Add(new Point3D(l, b, 0));
                points.Add(new Point3D(r, b, 0));

                // 頂点追加
                indices.Add(c + 0);
                indices.Add(c + 2);
                indices.Add(c + 1);

                indices.Add(c + 1);
                indices.Add(c + 2);
                indices.Add(c + 3);
            }

            return new MeshGeometry3D
            {
                Positions = points,
                TriangleIndices = indices,
                TextureCoordinates = texCoords,
            };
        }

        /// <summary>
        /// 実装していません。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
