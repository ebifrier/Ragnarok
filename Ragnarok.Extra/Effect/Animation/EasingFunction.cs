using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Effect.Animation
{
    /// <summary>
    /// キーフレーム補間の計算方法です。
    /// </summary>
    public enum EasingFunction
    {
        /// <summary>
        /// 線形補間を行います。
        /// </summary>
        Linear,
        /// <summary>
        /// 同じ値で固定させます。
        /// </summary>
        Discrete,
    }

    /// <summary>
    /// 値の補完メソッドなどを定義します。
    /// </summary>
    public static class EasingFunctionUtil
    {
        /// <summary>
        /// xとyをrを使って補完します。
        /// </summary>
        public static double Interpolate(this EasingFunction func, double x,
                                         double y, double r)
        {
            switch (func)
            {
                case EasingFunction.Linear:
                    return (x * (1.0 - r) + y * r);
                case EasingFunction.Discrete:
                    return (r == 0.0 ? x : y);
            }

            throw new NotSupportedException(
                func + ": 不明なEasingFunctionです。");
        }
    }
}
