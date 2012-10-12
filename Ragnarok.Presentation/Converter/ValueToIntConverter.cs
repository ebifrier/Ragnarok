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
    /// 整数値に変換するときの方法です。
    /// </summary>
    public enum RoundingMode
    {
        /// <summary>
        /// 切り下げ
        /// </summary>
        Floor,
        /// <summary>
        /// 切り上げ
        /// </summary>
        Ceil,
        /// <summary>
        /// 四捨五入
        /// </summary>
        Round,
    }

    /// <summary>
    /// 値を整数型に直します。
    /// </summary>
    public class ValueToIntConverter : IValueConverter
    {
        /// <summary>
        /// 整数値に丸める方法を取得または設定します。
        /// </summary>
        public RoundingMode RoundingMode
        {
            get;
            set;
        }

        /// <summary>
        /// 整数値に変換します。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                // 数値型で一番受け入れが広そうなものにキャストする。
                var v = System.Convert.ToDecimal(value);                
                
                switch (RoundingMode)
                {
                    case RoundingMode.Floor:
                        return (int)Math.Floor(v);
                    case RoundingMode.Ceil:
                        return (int)Math.Ceiling(v);
                    case RoundingMode.Round:
                        return (int)Math.Round(v);
                }

                throw new NotImplementedException(
                    "不正な整数値への変換方法です。");
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex, "整数型の変換に失敗しました。");

                return value;
            }
        }

        /// <summary>
        /// 基の値に戻すことはできないので、値をそのまま返す。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ValueToIntConverter()
        {
            RoundingMode = RoundingMode.Floor;
        }
    }
}
