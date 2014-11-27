using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Extra.Effect.Animation
{
    /// <summary>
    /// Point3d型のキーフレーム補間を行います。
    /// </summary>
    public abstract class Point3dKeyFrame : NotifyObject, IKeyFrame
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Point3dKeyFrame(EasingFunctions function)
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
        /// キーフレーム時刻に対応する値を取得または設定します。
        /// </summary>
        public Point3d Value
        {
            get { return GetValue<Point3d>("Value"); }
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
        public Point3d Interpolate(Point3d startValue, double rate)
        {
            return new Point3d
            {
                X = Function.Interpolate(startValue.X, Value.X, rate),
                Y = Function.Interpolate(startValue.Y, Value.Y, rate),
                Z = Function.Interpolate(startValue.Z, Value.Z, rate),
            };
        }
    }

    /// <summary>
    /// Point3dの線形補間を行うキーフレームクラスです。
    /// </summary>
    public class LinearPoint3dKeyFrame : Point3dKeyFrame
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LinearPoint3dKeyFrame()
            : base(EasingFunctions.Linear)
        {
        }
    }

    /// <summary>
    /// 固定値を使うキーフレームクラスです。
    /// </summary>
    public class DiscreatePoint3dKeyFrame : Point3dKeyFrame
    {
        public DiscreatePoint3dKeyFrame()
            : base(EasingFunctions.Discreate)
        {
        }
    }
}
