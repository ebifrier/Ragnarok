using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ragnarok;

namespace Live2DSharp.Framework
{
    /// <summary>
    /// 顔を特定の位置に向けるときに使います。
    /// </summary>
    public sealed class L2DTargetPoint
    {
        public const int FRAME_RATE = 30;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DTargetPoint()
        {
        }

        /// <summary>
        /// 顔の向きのX目標値(この値に近づいていく)を取得または設定します。
        /// </summary>
        public double FaceTargetX
        {
            get;
            set;
        }

        /// <summary>
        /// 顔の向きのY目標値(この値に近づいていく)を取得または設定します。
        /// </summary>
        public double FaceTargetY
        {
            get;
            set;
        }

        /// <summary>
        /// 現在の顔の向きのX座標値を取得します。
        /// </summary>
        public double FaceCurrentX
        {
            get;
            private set;
        }

        /// <summary>
        /// 現在の顔の向きのY座標値を取得します。
        /// </summary>
        public double FaceCurrentY
        {
            get;
            private set;
        }

        /// <summary>
        /// 顔の向きの変化速度Xを取得します。
        /// </summary>
        public double FaceVelocityX
        {
            get;
            private set;
        }

        /// <summary>
        /// 顔の向きの変化速度Yを取得します。
        /// </summary>
        public double FaceVelocityY
        {
            get;
            private set;
        }

        /// <summary>
        /// 更新処理を行います。
        /// </summary>
        /// <remarks>
        /// 首を中央から左右に振るときの平均的な速さは、秒程度。
        /// 加速・減速を考慮して、その２倍を最高速度とする。
        /// 顔のふり具合を、中央(0)から、左右は(±1)とする。
        /// </remarks>
        public void Update(TimeSpan elapsed)
        {
            // 7.5秒間に40分移動（5.3/sc)
            const double FACE_PARAM_MAX_V = 30.0f / 10;

            // 1frameあたりに変化できる速度の上限
            const double MAX_V = FACE_PARAM_MAX_V * 1.0f / FRAME_RATE;

            // 最高速度になるまでの時間を
            double TIME_TO_MAX_SPEED = 0.15;

            // sec*frame/sec
            double FRAME_TO_MAX_SPEED = TIME_TO_MAX_SPEED * FRAME_RATE;

            var deltaTimeWeight = elapsed.TotalMilliseconds * FRAME_RATE;
            var MAX_A = deltaTimeWeight * MAX_V / FRAME_TO_MAX_SPEED;

            // 目指す向きは、tmpdx,tmpdy 方向のベクトルとなる
            var vx = (FaceTargetX - FaceCurrentX);
            var vy = (FaceTargetY - FaceCurrentY);

            // 変化なし
            var tolerance = 1E-8;
            if (MathEx.Compare(vx, 0, tolerance) == 0 &&
                MathEx.Compare(vy, 0, tolerance) == 0)
            {
                return;
            }

            // 速度の最大よりも大きい場合は、速度を落とす
            var v = MathEx.InnerProduct(vx, vy);
            if (v > MAX_V)
            {
                vx *= MAX_V / v;
                vy *= MAX_V / v;
            }

            // 現在の速度から、新規速度への変化（加速度）を求める
            var ax = vx - FaceVelocityX;
            var ay = vy - FaceVelocityY;

            var a = MathEx.InnerProduct(ax, ay);
            if (a > MAX_A)
            {
                ax *= MAX_A / a;
                ay *= MAX_A / a;
            }

            // 加速度を元の速度に足して、新速度とする
            FaceVelocityX += ax;
            FaceVelocityY += ay;

            // 目的の方向に近づいたとき、滑らかに減速するための処理
            // 	設定された加速度で止まることのできる距離と速度の関係から
            // 	現在とりうる最高速度を計算し、それ以上のときは速度を落とす
            // 	※本来、人間は筋力で力（加速度）を調整できるため、
            //          より自由度が高いが、簡単な処理ですませている
            {
                // 加速度、速度、距離の関係式。
                //            2  6           2               3
                //      sqrt(a  t  + 16 a h t  - 8 a h) - a t
                // v = --------------------------------------
                //                    2
                //                 4 t  - 2
                //(t=1)
                // 	時刻tは、あらかじめ加速度、速度を1/60(フレームレート、単位なし)で
                // 	考えているので、t＝１として消してよい（※未検証）

                var term = Math.Sqrt(MAX_A * MAX_A + 16 * MAX_A * v - 8 * MAX_A * v);
                var max_v = 0.5 * (term - MAX_A);
                var cur_v = MathEx.InnerProduct(FaceVelocityX, FaceVelocityY);

                if (cur_v > max_v)
                {
                    // 現在の速度＞最高速度のとき、最高速度まで減速
                    FaceVelocityX *= max_v / cur_v;
                    FaceVelocityY *= max_v / cur_v;
                }
            }

            FaceCurrentX += FaceVelocityX;
            FaceCurrentY += FaceVelocityY;
        }
    }
}
