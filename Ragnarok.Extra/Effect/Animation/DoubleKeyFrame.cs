using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.ObjectModel;

namespace Ragnarok.Extra.Effect.Animation
{
    /// <summary>
    /// double型のキーフレーム補間を行います。
    /// </summary>
    public abstract class DoubleKeyFrame : NotifyObject, IKeyFrame
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DoubleKeyFrame(EasingFunctions function)
        {
            KeyTime = TimeSpan.Zero;
        }

        /// <summary>
        /// キーフレームの時刻を取得または設定します。
        /// </summary>
        public TimeSpan KeyTime
        {
            get { return GetValue<TimeSpan>("KeyTime"); }
            set { SetValue("KeyTime", value); }
        }

        /// <summary>
        /// 指定時刻に対応する値を取得または設定します。
        /// </summary>
        public double Value
        {
            get { return GetValue<double>("Value"); }
            set { SetValue("Value", value); }
        }

        /// <summary>
        /// 補間関数を取得または設定します。
        /// </summary>
        public EasingFunctions Function
        {
            get { return GetValue<EasingFunctions>("Function"); }
            set { SetValue("Function", value); }
        }

        /// <summary>
        /// キーフレームによる補間を行います。
        /// </summary>
        public double Interpolate(double startValue, double rate)
        {
            return Function.Interpolate(startValue, Value, rate);
        }
    }

    /// <summary>
    /// 線形補間を行うキーフレームクラスです。
    /// </summary>
    public class LinearDoubleKeyFrame : DoubleKeyFrame
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LinearDoubleKeyFrame()
            : base(EasingFunctions.Linear)
        {
        }
    }

    /// <summary>
    /// 固定値を使うキーフレームクラスです。
    /// </summary>
    public class DiscreateDoubleKeyFrame : DoubleKeyFrame
    {
        public DiscreateDoubleKeyFrame()
            : base(EasingFunctions.Discreate)
        {
        }
    }
}
