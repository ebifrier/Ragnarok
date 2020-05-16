using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ragnarok.Forms.Converter
{
    /// <summary>
    /// 値をdecimal型に直します。
    /// </summary>
    public class ValueToDecimalConverter : IValueConverter
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ValueToDecimalConverter(Type type = null)
        {
            SourceType = type;
        }

        /// <summary>
        /// 変換元の型を取得または設定します。
        /// </summary>
        public Type SourceType
        {
            get;
            set;
        }

        /// <summary>
        /// decimal値に変換します。
        /// </summary>
        public object Convert(object value, Type targetType, object parameter)
        {
            try
            {
                return System.Convert.ToDecimal(value, CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "decimal型の変換に失敗しました。");

                return value;
            }
        }

        /// <summary>
        /// 変換元の型に戻します。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter)
        {
            return System.Convert.ChangeType(
                value,
                SourceType ?? targetType,
                CultureInfo.CurrentCulture);
        }
    }
}
