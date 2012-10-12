using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace Ragnarok.Shogi.ViewModel
{
    /// <summary>
    /// 各マスにフィットするようなメッシュを作成するコンバーターです。
    /// </summary>
    [ValueConversion(typeof(Position), typeof(MeshGeometry3D))]
    [ValueConversion(typeof(IEnumerable<Position>), typeof(MeshGeometry3D))]
    public class CellMeshConverter : IValueConverter
    {
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
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var positions = value as IEnumerable<Position>;
            if (positions == null)
            {
                var position = value as Position;
                if (position == null)
                {
                    return null;
                }

                positions = new[] { position };
            }

            return Util3D.CreateCellMesh(positions, Widen);
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
