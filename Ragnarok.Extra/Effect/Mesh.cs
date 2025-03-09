using System;
using System.Collections.Generic;
using System.Linq;

using Ragnarok.MathEx;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// 頂点配列などの情報を管理します。
    /// </summary>
    /// <remarks>
    /// IndexArrayは個別の三角形ごとに指定するため、
    /// 必ず３の倍数である必要があります。
    /// </remarks>
    public class Mesh
    {
        public static readonly Mesh Default = CreateDefaultImpl(1, 1, 0, 0);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Mesh(IEnumerable<Point3d> vertices,
                    IEnumerable<Pointd> uvs,
                    IEnumerable<int> indices)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            if (indices == null)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            VertexArray = GenFloats(vertices, uvs).ToArray();
            IndexArray = indices.ToArray();
        }

        private static IEnumerable<float> GenFloats(IEnumerable<Point3d> vertices,
                                                    IEnumerable<Pointd> uvs)
        {
            if (uvs != null)
            {
                foreach (var (vertex, uv) in vertices.Zip(uvs))
                {
                    yield return (float)vertex.X;
                    yield return (float)vertex.Y;
                    yield return (float)vertex.Z;
                    yield return (float)uv.X;
                    yield return (float)uv.Y;
                }
            }
            else
            {
                foreach (var vertex in vertices)
                {
                    yield return (float)vertex.X;
                    yield return (float)vertex.Y;
                    yield return (float)vertex.Z;
                    yield return 0.0f;
                    yield return 0.0f;
                }
            }
        }

        /// <summary>
        /// 頂点配列を取得します。
        /// </summary>
        public float[] VertexArray
        {
            get;
            private set;
        }

        /// <summary>
        /// インデックス配列を取得します。
        /// </summary>
        public int[] IndexArray
        {
            get;
            private set;
        }

        /// <summary>
        /// 指定の座標を持った矩形モデルを作成します。
        /// </summary>
        public static Mesh CreateDefault(double width, double height,
                                         double imageWidth, double imageHeight)
        {
            if (width == 1.0 && height == 1.0 &&
                imageWidth == 0.0 && imageHeight == 0.0)
            {
                return Default;
            }

            return CreateDefaultImpl(width, height, imageWidth, imageHeight);
        }

        /// <summary>
        /// 指定の座標を持った矩形モデルを作成します。
        /// </summary>
        private static Mesh CreateDefaultImpl(double width, double height,
                                              double imageWidth, double imageHeight)
        {
            // テクスチャのUV座標をイメージの0.5ピクセル分ずらします。
            var halfPixelW = (imageWidth != 0.0 ? 0.5 / imageWidth : 0.0);
            var halfPixelH = (imageHeight != 0.0 ? 0.5 / imageHeight : 0.0);

            return new Mesh(
                // 頂点配列
                new Point3d[]
                {
                    new Point3d(-width / 2, -height / 2, 0.0),
                    new Point3d(+width / 2, -height / 2, 0.0),
                    new Point3d(-width / 2, +height / 2, 0.0),
                    new Point3d(+width / 2, +height / 2, 0.0),
                },
                // テクスチャUV配列
                new Pointd[]
                {
                    new Pointd(0.0,              0.0),
                    new Pointd(1.0 - halfPixelW, 0.0),
                    new Pointd(0.0,              1.0 - halfPixelH),
                    new Pointd(1.0 - halfPixelW, 1.0 - halfPixelH),
                },
                // インデックス配列
                new int[]
                {
                    0, 2, 1,
                    1, 2, 3,
                });
        }
    }
}
