using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Reflection;

using Ragnarok.Utility;

namespace Ragnarok.Presentation.VisualObject
{
    /// <summary>
    /// 3D用のユーティリティクラスです。
    /// </summary>
    public static class Util3D
    {
        /// <summary>
        /// Vector3Dを作成します。
        /// </summary>
        public static Vector3D MakeVector3D(Point v, double z)
        {
            return new Vector3D(v.X, v.Y, z);
        }

        /// <summary>
        /// Vector3Dを作成します。
        /// </summary>
        public static Vector3D MakeVector3D(Vector v, double z)
        {
            return new Vector3D(v.X, v.Y, z);
        }

        /// <summary>
        /// Sizeを作成します。
        /// </summary>
        public static Size MakeSizeXY(Size3D s)
        {
            return new Size(s.X, s.Y);
        }

        /// <summary>
        /// Rectを作成します。
        /// </summary>
        public static Rect MakeRectXY(Rect3D r)
        {
            return new Rect(r.X, r.Y, r.SizeX, r.SizeY);
        }

        /// <summary>
        /// プロパティ名から依存プロパティを検索します。
        /// </summary>
        private static DependencyProperty GetDepPropertyImpl(Type sourceType,
                                                             string propertyName)
        {
            const BindingFlags flags =
                BindingFlags.Static |
                BindingFlags.GetField |
                BindingFlags.Public;

            var classes = MethodUtil.GetThisAndInheritClasses(sourceType);

            var propertyList = classes
                .SelectMany(_ => _.GetFields(flags))
                .Select(_ => _.GetValue(null) as DependencyProperty)
                .Where(_ => _ != null);

            return propertyList.FirstOrDefault(_ => _.Name == propertyName);
        }

        /// <summary>
        /// プロパティ名から依存プロパティを検索します。
        /// </summary>
        /// <example>
        /// Foo.Bar.X のような指定からX依存プロパティを返します。
        /// </example>
        public static DependencyProperty GetDependencyProperty(Type sourceType,
                                                               string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            var Splitter = new[] { "." };
            var sourceProperties = propertyPath.Split(Splitter, StringSplitOptions.None);
            var propertyType = sourceType;
            DependencyProperty property = null;

            foreach (var name in sourceProperties)
            {
                property = GetDepPropertyImpl(propertyType, name);
                if (property == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "{0}プロパティが見つかりませんでした。", name));
                }

                propertyType = property.PropertyType;
            }

            return property;
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
