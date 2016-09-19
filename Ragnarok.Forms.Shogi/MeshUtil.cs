using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Ragnarok.Shogi;
using Ragnarok.Utility;
using Ragnarok.Extra.Effect;

namespace Ragnarok.Forms.Shogi
{
    using View;

    /// <summary>
    /// 指し手を表示するための矢印を作成します。
    /// </summary>
    /// <remarks>
    /// 矢印は常に上を向いており、指定の長さの矢印となります。
    /// また、矢印の幅が広い部分(ポイント部分)の幅とサイズは常に同じになります。
    /// </remarks>
    public static class MeshUtil
    {
        /// <summary>
        /// 手番表示用のメッシュを作成します。
        /// </summary>
        public static Mesh CreateTeban(BWType turn)
        {
            // マスの数９×マスのサイズ(1.0)＋左右の幅(0.4 * 2) = 9.8
            var points = new List<Point3d>();
            var indices = new List<int>();
            var len = (float)(GLShogiElement.BoardBorderRate / 9.0f);
            var bds = 9.0f / 9.0f;
            var top = bds * 0.5f;
            var btm = bds * 0.8f;

            var rectList = new List<RectangleF>
            {
                new RectangleF(-len, top, len, btm - top),
                new RectangleF(-len, btm, len, bds - btm + len),

                new RectangleF(1.0f, top, len, btm - top),
                new RectangleF(1.0f, btm, len, bds - btm + len),

                new RectangleF(0.0f, 1.0f, bds, len),
            };

            // テクスチャ座標
            var texCoords = new List<Pointd>
            {
                new Pointd(0.0, 0.0), new Pointd(0.0, 0.5),
                new Pointd(1.0, 0.0), new Pointd(1.0, 0.5),
                new Pointd(1.0, 0.0), new Pointd(1.0, 0.5),
                new Pointd(1.0, 0.0), new Pointd(1.0, 0.5),

                new Pointd(0.0, 0.0), new Pointd(0.0, 0.5),
                new Pointd(1.0, 0.0), new Pointd(1.0, 0.5),
                new Pointd(1.0, 0.0), new Pointd(1.0, 0.5),
                new Pointd(1.0, 0.0), new Pointd(1.0, 0.5),

                new Pointd(1.0, 0.0), new Pointd(1.0, 0.0),
                new Pointd(1.0, 0.5), new Pointd(1.0, 0.5),
            };

            foreach (var rect in rectList)
            {
                var c = points.Count;
                var l = (turn == BWType.Black ? rect.Left : 1.0 - rect.Left);
                var t = (turn == BWType.Black ? rect.Top : 1.0 - rect.Top);
                var r = (turn == BWType.Black ? rect.Right : 1.0 - rect.Right);
                var b = (turn == BWType.Black ? rect.Bottom : 1.0 - rect.Bottom);

                // 各マスの座標追加
                points.Add(new Point3d(l, t, 0));
                points.Add(new Point3d(r, t, 0));
                points.Add(new Point3d(l, b, 0));
                points.Add(new Point3d(r, b, 0));

                // 頂点追加
                indices.Add(c + 0);
                indices.Add(c + 2);
                indices.Add(c + 1);

                indices.Add(c + 1);
                indices.Add(c + 2);
                indices.Add(c + 3);
            }

            return new Mesh(points, texCoords, indices);
        }

        /// <summary>
        /// 矢印用のメッシュを作成します。
        /// </summary>
        /// <remarks>
        /// 作成される矢印は常に上を向いており、全体の幅と高さが1.0です。
        /// （つまり、xの最大値は0.5となります。また矢印の後ろ端がy=0, 先端がy=1.0となります）
        /// </remarks>
        /// <param name="headLength">ヘッド部分の長さ</param>
        /// <param name="shaftWidth2">棒の部分の幅/2</param>
        /// <param name="tailWidth2">棒終端部分の幅/2</param>
        public static Mesh CreateArrow(double headLength,
                                       double shaftWidth2, double tailWidth2)
        {
            var points = new List<Point3d>
            {
                new Point3d(0, 1.0, 0), // ヘッドの先端部分
                new Point3d(-0.5, 1.0 - headLength, 0), // ヘッド部分の左端
                new Point3d(+0.5, 1.0 - headLength, 0), // ヘッド部分の右端
                new Point3d(-shaftWidth2, 1.0 - headLength, 0), // ヘッドとシャフトの交点（左）
                new Point3d(+shaftWidth2, 1.0 - headLength, 0), // ヘッドとシャフトの交点（右）
                new Point3d(-tailWidth2, 0, 0), // シャフトの後端（左）
                new Point3d(+tailWidth2, 0, 0), // シャフトの後端（右）
            };
            var texCoords = new List<Pointd>
            {
                new Pointd(0.5, 0.0),
                new Pointd(0.0, headLength),
                new Pointd(1.0, headLength),
                new Pointd(-shaftWidth2, headLength),
                new Pointd(+shaftWidth2, headLength),
                new Pointd(-tailWidth2, 1.0),
                new Pointd(+tailWidth2, 1.0),
            };
            var indices = new List<int>
            {
                0, 1, 2,
                3, 5, 4,
                4, 5, 6,
            };

            return new Mesh(points, texCoords, indices);
        }
    }
}
