using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

using Ragnarok.Shogi;
using Ragnarok.Utility;
using Ragnarok.Extra.Effect;

namespace Ragnarok.Forms.Shogi.Xaml
{
    /// <summary>
    /// 各マスにフィットするようなメッシュを作成する拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(Mesh))]
    public sealed class SquareMeshExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SquareMeshExtension()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SquareMeshExtension(SquareCollection squares)
        {
            Squares = squares;
        }

        /// <summary>
        /// メッシュを作成するマス一覧を取得または設定します。
        /// </summary>
        public SquareCollection Squares
        {
            get;
            set;
        }

        /// <summary>
        /// どれだけマスからはみ出すかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// ０でマスぴったり、0.5で上下左右にますの半分の長さだけはみ出します。
        /// </remarks>
        public double Widen
        {
            get;
            set;
        }

        /// <summary>
        /// マスの位置からメッシュを作成します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            if (Squares == null)
            {
                throw new InvalidOperationException(
                    "Squaresが設定されていません。");
            }

            return CreateCellMesh(Squares, Widen);
        }

        /// <summary>
        /// 単純な四角形のジオメトリを作成します。
        /// </summary>
        public static Mesh CreateCellMesh(IEnumerable<Square> squares,
                                                double widen = 0.0)
        {
            var points = new List<Point3d>();
            var texCoords = new List<Pointd>();
            var indices = new List<int>();

            foreach (var square in squares.Where(_ => _ != null))
            {
                var x = Board.BoardSize - square.File;
                var y = square.Rank - 1;
                var c = points.Count;

                // 各マスの座標追加
                points.Add(new Point3d(x + 0 - widen, y + 0 - widen, 0));
                points.Add(new Point3d(x + 1 + widen, y + 0 - widen, 0));
                points.Add(new Point3d(x + 0 - widen, y + 1 + widen, 0));
                points.Add(new Point3d(x + 1 + widen, y + 1 + widen, 0));

                // テクスチャ位置を追加
                texCoords.Add(new Pointd(0, 0));
                texCoords.Add(new Pointd(1, 0));
                texCoords.Add(new Pointd(0, 1));
                texCoords.Add(new Pointd(1, 1));

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
