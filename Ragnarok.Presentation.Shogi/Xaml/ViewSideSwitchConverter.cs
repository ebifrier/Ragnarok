using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;

using Ragnarok.Shogi;

namespace Ragnarok.Presentation.Shogi.Xaml
{
    /// <summary>
    /// 盤の回転に対応した対局者名などを取得します。
    /// </summary>
    public sealed class ViewSideSwitchConverter : IMultiValueConverter
    {
        /// <summary>
        /// 初期状態の先手側に対応した値を扱うか取得または設定します。
        /// </summary>
        public bool IsBlack
        {
            get;
            set;
        }

        /// <summary>
        /// 盤の回転に対応した対局者名などを取得します。
        /// </summary>
        public object Convert(object[] value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                if (value[0] == DependencyProperty.UnsetValue)
                {
                    return null;
                }

                var viewSide = (BWType)value[0];
                var blackValue = value[1];
                var whiteValue = value[2];

                var flag1 = (IsBlack && viewSide == BWType.Black);
                var flag2 = (!IsBlack && viewSide == BWType.White);

                return (flag1 || flag2 ? blackValue : whiteValue);
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        /// <summary>
        /// 実装していません。
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetType,
                                    object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
