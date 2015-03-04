using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

using Ragnarok.Utility;

namespace Ragnarok.Xaml
{
    /// <summary>
    /// 端数の処理方法を示します。
    /// </summary>
    public enum RoundingMode
    {
        /// <summary>
        /// 四捨五入
        /// </summary>
        Round,
        /// <summary>
        /// 切り上げ
        /// </summary>
        Ceiling,
        /// <summary>
        /// 切捨て
        /// </summary>
        Floor,
    }

    /// <summary>
    /// xaml中で計算式を使うための拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(double))]
    public class CalcExtension : MarkupExtension
    {
        private static readonly Calculator calculator =
            new Calculator(AngleMode.Degree);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CalcExtension()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CalcExtension(string expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// 式文字列を取得または設定します。
        /// </summary>
        public string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// 式を計算し値を返します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            return calculator.Run(Expression.RemoveQuote());
        }
    }

    /// <summary>
    /// xaml中で計算式を使うための拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(int))]
    public class IntCalcExtension : MarkupExtension
    {
        private static readonly Calculator calculator =
            new Calculator(AngleMode.Degree);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IntCalcExtension()
        {
            Mode = RoundingMode.Round;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IntCalcExtension(string expression)
        {
            Mode = RoundingMode.Round;
            Expression = expression;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IntCalcExtension(string expression, RoundingMode mode)
        {
            Mode = mode;
            Expression = expression;
        }

        /// <summary>
        /// 式文字列を取得または設定します。
        /// </summary>
        public string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// 端数の処理方法を取得または設定します。
        /// </summary>
        public RoundingMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// 式を計算し値を返します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            var value = calculator.Run(Expression.RemoveQuote());

            switch (Mode)
            {                    
                case RoundingMode.Ceiling:
                    return (int)Math.Ceiling(value);
                case RoundingMode.Floor:
                    return (int)Math.Floor(value);
            }

            return (int)Math.Round(value);
        }
    }

    /// <summary>
    /// xaml中で計算式を使うための拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(uint))]
    public class UIntCalcExtension : MarkupExtension
    {
        private static readonly Calculator calculator =
            new Calculator(AngleMode.Degree);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UIntCalcExtension()
        {
            Mode = RoundingMode.Round;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UIntCalcExtension(string expression)
        {
            Mode = RoundingMode.Round;
            Expression = expression;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UIntCalcExtension(string expression, RoundingMode mode)
        {
            Mode = mode;
            Expression = expression;
        }

        /// <summary>
        /// 式文字列を取得または設定します。
        /// </summary>
        public string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// 端数の処理方法を取得または設定します。
        /// </summary>
        public RoundingMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// 式を計算し値を返します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            var value = calculator.Run(Expression.RemoveQuote());

            switch (Mode)
            {
                case RoundingMode.Ceiling:
                    return (uint)Math.Ceiling(value);
                case RoundingMode.Floor:
                    return (uint)Math.Floor(value);
            }

            return (uint)Math.Round(value);
        }
    }
}
