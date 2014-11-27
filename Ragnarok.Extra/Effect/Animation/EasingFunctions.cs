using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Effect.Animation
{
    /// <summary>
    /// キーフレーム補間の計算方法です。
    /// </summary>
    public enum EasingFunctions
    {
        /// <summary>
        /// 線形補間を行います。
        /// </summary>
        Linear,
        /// <summary>
        /// 同じ値で固定させます。
        /// </summary>
        Discreate,
    }

    /// <summary>
    /// 値の補完メソッドなどを定義します。
    /// </summary>
    public static class EasingFunctionsUtil
    {
        /// <summary>
        /// xとyをrを使って補完します。
        /// </summary>
        public static double Interpolate(this EasingFunctions func, double x,
                                         double y, double r)
        {
            switch (func)
            {
                case EasingFunctions.Linear:
                    return (x * (1.0 - r) + y * r);
                case EasingFunctions.Discreate:
                    return (r == 0.0 ? x : y);
            }

            throw new NotSupportedException(
                func + ": 不明なEasingFunctionです。");
        }
    }
}
