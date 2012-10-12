using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Ragnarok
{
    /// <summary>
    /// 数学関数を定義します。
    /// </summary>
    public static class MathEx
    {
        /// <summary>
        /// 乱数用オブジェクトを取得または設定します。
        /// </summary>
        private static readonly Random Rand = new Random();

        /// <summary>
        /// ０以上の乱数を返します。
        /// </summary>
        public static int RandInt()
        {
            lock (Rand)
            {
                return Rand.Next();
            }
        }

        /// <summary>
        /// <paramref name="minValue"/>以上、<paramref name="maxValue"/>
        /// 未満の乱数を返します。
        /// </summary>
        public static int RandInt(int minValue, int maxValue)
        {
            var range = maxValue - minValue;
            if (range < 0)
            {
                throw new ArgumentException("最小値が最大値よりも大きいです。");
            }

            lock (Rand)
            {
                return (minValue + Rand.Next(range));
            }
        }

        /// <summary>
        /// 0.0～1.0の乱数を返します。
        /// </summary>
        public static double RandDouble()
        {
            lock (Rand)
            {
                return Rand.NextDouble();
            }
        }

        /// <summary>
        /// 0以上、<paramref name="maxValue"/>以下の乱数を返します。
        /// </summary>
        public static double RandDouble(double maxValue)
        {
            lock (Rand)
            {
                return (Rand.NextDouble() * maxValue);
            }
        }

        /// <summary>
        /// <paramref name="minValue"/>以上、<paramref name="maxValue"/>以下の
        /// 乱数を返します。
        /// </summary>
        public static double RandDouble(double minValue, double maxValue)
        {
            var range = maxValue - minValue;
            if (range < 0)
            {
                throw new ArgumentException("最小値が最大値よりも大きいです。");
            }

            lock (Rand)
            {
                return (minValue + Rand.NextDouble() * range);
            }
        }

        /// <summary>
        /// 角度をラジアンに直します。
        /// </summary>
        public static double ToRad(double angle)
        {
            return (angle * Math.PI / 180);
        }

        /// <summary>
        /// 角度を度に直します。
        /// </summary>
        public static double ToDeg(double rad)
        {
            return (rad / Math.PI * 180);
        }

        /// <summary>
        /// 値の大きい方を返します。
        /// </summary>
        public static DateTime Max(DateTime x, DateTime y)
        {
            return (x > y ? x : y);
        }

        /// <summary>
        /// 値の小さい方を返します。
        /// </summary>
        public static DateTime Min(DateTime x, DateTime y)
        {
            return (x < y ? x : y);
        }

        /// <summary>
        /// 値の大きい方を返します。
        /// </summary>
        public static TimeSpan Max(TimeSpan x, TimeSpan y)
        {
            return (x > y ? x : y);
        }

        /// <summary>
        /// 値の小さい方を返します。
        /// </summary>
        public static TimeSpan Min(TimeSpan x, TimeSpan y)
        {
            return (x < y ? x : y);
        }

        /// <summary>
        /// <paramref name="currentDirection"/>から<paramref name="targetDirection"/>
        /// に移動するような角度を計算します。
        /// </summary>
        public static double GetHomingDirection(double currentDirection,
                                                double targetDirection,
                                                double rotationSpeed)
        {
            while (currentDirection - 180 > targetDirection)
            {
                targetDirection += 360;
            }
            while (currentDirection + 180 < targetDirection)
            {
                targetDirection -= 360;
            }

            // 角度の差が一定値より大きければ、差分を小さくします。
            var diff = currentDirection - targetDirection;
            if (diff < -rotationSpeed)
            {
                return +rotationSpeed;
            }
            else if (diff > +rotationSpeed)
            {
                return -rotationSpeed;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// a～bに値が収まるようにvを調整します。
        /// </summary>
        public static double Between(double a, double b, double v)
        {
            return Math.Max(a, Math.Min(v, b));
        }

        /// <summary>
        /// a～bに値が収まるようにvを調整します。
        /// </summary>
        public static int Between(int a, int b, int v)
        {
            return Math.Max(a, Math.Min(v, b));
        }

        /// <summary>
        /// <paramref name="progress"/>が<paramref name="duration"/>
        /// 周期で0.0～1.0の値を返すようにします。
        /// </summary>
        public static double Normalize(double progress, double duration,
                                       bool reversible)
        {
            // 往復する場合は、周期を２倍にします。
            var v = progress % (duration * (reversible ? 2 : 1));
            if (v < 0)
            {
                v = -v;
            }
            
            return (reversible && v > duration ? duration * 2 - v : v);
        }

        /// <summary>
        /// 等速補間を行います。
        /// </summary>
        public static double InterpLiner(double a, double b, double r)
        {
            return (a + (b - a) * r);
        }

        /// <summary>
        /// 加速するような補間を行います。
        /// </summary>
        public static double InterpEaseIn(double a, double b, double r)
        {
            var d = b - a;

            return (a + d * r * r);
        }

        /// <summary>
        /// 減速するような補間を行います。
        /// </summary>
        public static double InterpEaseOut(double a, double b, double r)
        {
            var d = b - a;
            r = r - 1.0;

            return (a - d * (r * r - 1.0));
        }
    }
}
