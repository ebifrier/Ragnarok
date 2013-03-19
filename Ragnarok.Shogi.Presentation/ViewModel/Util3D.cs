using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Reflection;

using Ragnarok;
using Ragnarok.Utility;

namespace Ragnarok.Shogi.ViewModel
{
    public static class Util3D_
    {
        public static Vector3D MakeVector3D(Point v, double z)
        {
            return new Vector3D(v.X, v.Y, z);
        }

        public static Vector3D MakeVector3D(Vector v, double z)
        {
            return new Vector3D(v.X, v.Y, z);
        }

        public static Size MakeSizeXY(Size3D s)
        {
            return new Size(s.X, s.Y);
        }

        public static Rect MakeRectXY(Rect3D r)
        {
            return new Rect(r.X, r.Y, r.SizeX, r.SizeY);
        }

        /// <summary>
        /// 単純な四角形のジオメトリを作成します。
        /// </summary>
        public static MeshGeometry3D CreateDefaultMesh(double width, double height,
                                                       double imageWidth,
                                                       double imageHeight)
        {
            var halfPixelW = (imageWidth != 0.0 ? 0.5 / imageWidth : 0.0);
            var halfPixelH = (imageHeight != 0.0 ? 0.5 / imageHeight : 0.0);

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
                    new Point(0.0,              0.0),
                    new Point(1.0 - halfPixelW, 0.0),
                    new Point(0.0,              1.0 - halfPixelH),
                    new Point(1.0 - halfPixelW, 1.0 - halfPixelH),
                },
                TriangleIndices =
                {
                    0, 2, 1,
                    1, 2, 3,
                },
            };
        }

        /// <summary>
        /// 単純な四角形のジオメトリを作成します。
        /// </summary>
        public static MeshGeometry3D CreateCellMesh(IEnumerable<Position> positions,
                                                    double widen = 0.0)
        {
            var points = new Point3DCollection();
            var indices = new Int32Collection();
            var texCoords = new PointCollection();

            foreach (var position in positions)
            {
                var x = Board.BoardSize - position.File;
                var y = position.Rank - 1;
                var c = points.Count;

                // 各マスの座標追加
                points.Add(new Point3D(x + 0 - widen, y + 0 - widen, 0));
                points.Add(new Point3D(x + 1 + widen, y + 0 - widen, 0));
                points.Add(new Point3D(x + 0 - widen, y + 1 + widen, 0));
                points.Add(new Point3D(x + 1 + widen, y + 1 + widen, 0));

                // テクスチャ位置を追加
                texCoords.Add(new Point(0, 0));
                texCoords.Add(new Point(1, 0));
                texCoords.Add(new Point(0, 1));
                texCoords.Add(new Point(1, 1));

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

        #region コリジョン
        /// <summary>
        /// p0とp1で結ばれる直線と点cの距離を計算します。
        /// </summary>
        /// <remarks>
        /// ベクトルを用いて距離を計算します。
        /// ベクトルL = P1 - P0
        /// 
        /// 線分上で点Cと直交する点をP = P0 + t * Lとすると、
        /// (P - C)・L = 0
        ///   (P0 - C + t * L)・L = 0
        ///   t * |L|^2 = - (P0 - C)・L
        /// 
        /// また、
        /// 距離d = |P - C|
        ///       = |(P0 - C) + t * L|
        /// 
        /// 参考：http://homepage2.nifty.com/mathfin/distance/vector.htm
        /// </remarks>
        public static double LineCircleDistance(Vector3D p0, Vector3D p1,
                                                Vector3D c)
        {
            var cp = p0 - c;
            var l = p1 - p0;
            var length2 = l.LengthSquared;

            if (length2 < double.Epsilon)
            {
                return cp.Length;
            }

            var t = - Vector3D.DotProduct(cp, l) / length2;

            // 線分と点の距離なので点Pは端点の外には出れません。
            t = MathEx.Between(0.0, 1.0, t);

            return (cp + t * l).Length;
        }
        #endregion
    }
}
