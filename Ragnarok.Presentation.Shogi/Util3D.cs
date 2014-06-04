using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Ragnarok.Shogi;
using Ragnarok.Utility;

namespace Ragnarok.Presentation.Shogi
{
    public static class Util3D
    {
        /// <summary>
        /// 単純な四角形のジオメトリを作成します。
        /// </summary>
        public static MeshGeometry3D CreateCellMesh(IEnumerable<Square> positions,
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
    }
}
