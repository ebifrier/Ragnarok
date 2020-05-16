﻿using System;
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
        public DoubleKeyFrame(EasingFunction function)
        {
            KeyTime = TimeSpan.Zero;
            Function = function;
        }

        /// <summary>
        /// キーフレームの時刻を取得または設定します。
        /// </summary>
        public TimeSpan KeyTime
        {
            get { return GetValue<TimeSpan>(nameof(KeyTime)); }
            set { SetValue(nameof(KeyTime), value); }
        }

        /// <summary>
        /// 指定時刻に対応する値を取得または設定します。
        /// </summary>
        public double Value
        {
            get { return GetValue<double>(nameof(Value)); }
            set { SetValue(nameof(Value), value); }
        }

        /// <summary>
        /// 補間関数を取得または設定します。
        /// </summary>
        public EasingFunction Function
        {
            get { return GetValue<EasingFunction>(nameof(Function)); }
            set { SetValue(nameof(Function), value); }
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
            : base(EasingFunction.Linear)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LinearDoubleKeyFrame(double value, TimeSpan keyTime)
            : this()
        {
            Value = value;
            KeyTime = keyTime;
        }
    }

    /// <summary>
    /// 固定値を使うキーフレームクラスです。
    /// </summary>
    public class DiscreteDoubleKeyFrame : DoubleKeyFrame
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DiscreteDoubleKeyFrame()
            : base(EasingFunction.Discrete)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DiscreteDoubleKeyFrame(double value, TimeSpan keyTime)
            : this()
        {
            Value = value;
            KeyTime = keyTime;
        }
    }
}
