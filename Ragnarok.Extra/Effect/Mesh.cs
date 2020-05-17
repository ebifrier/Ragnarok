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

            if (uvs == null)
            {
                throw new ArgumentNullException(nameof(uvs));
            }

            VertexArray = vertices.ToList();
            IndexArray = indices.ToList();
            TextureUVArray = uvs.ToList();

            if (VertexArray.Count != TextureUVArray.Count)
            {
                throw new ArgumentException(
                    "頂点配列とテクスチャUV配列の数が一致しません。");
            }

            if (IndexArray.Count % 3 != 0)
            {
                throw new ArgumentException(
                    "インデックス配列は三角形による指定をお願いします。");
            }
        }

        /// <summary>
        /// 頂点配列を取得します。
        /// </summary>
        public List<Point3d> VertexArray
        {
            get;
            private set;
        }

        /// <summary>
        /// テクスチャのUV配列を取得します。
        /// </summary>
        public List<Pointd> TextureUVArray
        {
            get;
            private set;
        }

        /// <summary>
        /// インデックス配列を取得します。
        /// </summary>
        public List<int> IndexArray
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
