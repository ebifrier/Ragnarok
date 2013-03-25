using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace Ragnarok.Presentation.Converter
{
    /// <summary>
    /// 列挙子と真偽値の変換を行います。
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnumToBooleanConverter()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnumToBooleanConverter(Type enumType)
        {
            EnumType = enumType;
        }

        /// <summary>
        /// 対象となる列挙型を取得または設定します。
        /// </summary>
        public Type EnumType
        {
            get;
            set;
        }

        /// <summary>
        /// 列挙子の値と<paramref name="parameter"/>で与えられた
        /// 値を比較し、同一なら真を返します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var tmpString = parameter as string;
            if (tmpString != null)
            {
                if (string.IsNullOrEmpty(tmpString))
                {
                    return DependencyProperty.UnsetValue;
                }

                /*if (!Enum.IsDefined(value.GetType(), value))
                {
                    return DependencyProperty.UnsetValue;
                }*/

                // 値がパラメータ値と同一か調べます。
                var parameterValue = Enum.Parse(value.GetType(), tmpString);
                return ((int)value == (int)parameterValue);
            }

            try
            {
                // 列挙値を整数型に変換するときはasやis演算子を使うと
                // 失敗するので、直接キャストして成功するか判断しています。
                //
                // すごく重いので、判定はメソッドの最後で行います。
                return ((int)value == (int)parameter);
            }
            catch (InvalidCastException)
            {
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// 真偽値を列挙子の値に直します。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            // 値が偽ならすぐに処理をやめます。
            if (!(bool)value)
            {
                return DependencyProperty.UnsetValue;
            }

            var tmpString = parameter as string;
            if (tmpString != null)
            {
                if (string.IsNullOrEmpty(tmpString))
                {
                    return DependencyProperty.UnsetValue;
                }

                return Enum.Parse(EnumType ?? targetType, tmpString);
            }

            try
            {
                // 列挙値を整数型に変換するときはasやis演算子を使うと
                // 失敗するので、直接キャストして成功するか判断しています。
                //
                // すごく重いので、判定はメソッドの最後で行います。
                return (int)parameter;
            }
            catch (InvalidCastException)
            {
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
