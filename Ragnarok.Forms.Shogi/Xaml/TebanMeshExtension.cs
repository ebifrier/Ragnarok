using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Markup;

using Ragnarok.Shogi;
using Ragnarok.Utility;
using Ragnarok.Extra.Effect;

namespace Ragnarok.Forms.Shogi.Xaml
{
    using View;

    /// <summary>
    /// 手番表示のメッシュを作成する拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(Mesh))]
    public sealed class TebanMeshExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TebanMeshExtension()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TebanMeshExtension(BWType turn)
        {
            Turn = turn;
        }

        /// <summary>
        /// 今の手番を取得または設定します。
        /// </summary>
        public BWType Turn
        {
            get;
            set;
        }

        /// <summary>
        /// 手番表示用のメッシュを作成します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
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
                var l = (Turn == BWType.Black ? rect.Left : 1.0 - rect.Left);
                var t = (Turn == BWType.Black ? rect.Top : 1.0 - rect.Top);
                var r = (Turn == BWType.Black ? rect.Right : 1.0 - rect.Right);
                var b = (Turn == BWType.Black ? rect.Bottom : 1.0 - rect.Bottom);

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
    }
}
