using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ragnarok;

namespace Live2DSharp.Framework
{
    /// <summary>
    /// 眼パチの状態を管理します。
    /// </summary>
    public sealed class L2DEyeBlink
    {
        /// <summary>
        /// 眼の状態定数
        /// </summary>
        private enum EyeState
        {
            First = 0,
            Interval,
            Closing,   // 閉じていく途中
            Closed,    // 閉じている状態
            Opening,   // 開いていく途中
        };

        private EyeState eyeState; // 現在の状態
        private TimeSpan nextBlinkTime; // 次回眼パチする時刻（sec）
		private double stateTime; // 現在のstateが始まってから経過した時刻

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DEyeBlink()
        {
            // 妥当と思われる値で初期化
            this.eyeState = EyeState.First;

            BlinkInterval = TimeSpan.FromMilliseconds(4000);

            ClosingMotionSpan = TimeSpan.FromMilliseconds(200);
            ClosedMotionSpan = TimeSpan.FromMilliseconds(50);
            OpeningMotionSpan = TimeSpan.FromMilliseconds(250);

            CloseIfZero = true;

            // 左右の目のパラメータ
            LeftEyeId = "PARAM_EYE_L_OPEN";
            RightEyeId = "PARAM_EYE_R_OPEN";
        }

        /// <summary>
        /// 左眼のパラメータ名を取得または設定します。
        /// </summary>
        public string LeftEyeId
        {
            get;
            set;
        }

        /// <summary>
        /// 右眼のパラメータ名を取得または設定します。
        /// </summary>
        public string RightEyeId
        {
            get;
            set;
        }

        /// <summary>
        /// 瞬きの間隔を取得または設定します。
        /// </summary>
        public TimeSpan BlinkInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 眼が閉じるまでの時間を取得または設定します。
        /// </summary>
        public TimeSpan ClosingMotionSpan
        {
            get;
            set;
        }

        /// <summary>
        /// 眼が閉じたままでいる時間を取得または設定します。
        /// </summary>
        public TimeSpan ClosedMotionSpan
        {
            get;
            set;
        }

        /// <summary>
        /// 眼が開くまでの時間を取得または設定します。
        /// </summary>
        public TimeSpan OpeningMotionSpan
        {
            get;
            set;
        }

        /// <summary>
        /// 眼の開き状態を取得または設定します。
        /// </summary>
        /// <remarks>
        /// IDで指定された眼のパラメータが、0のときに閉じるならtrue、
        /// 1の時に閉じるならfalseとなります。
        /// </remarks>
        public bool CloseIfZero
        {
            get;
            set;
        }
		
        /// <summary>
        /// 次回の眼パチの時刻を決める。ざっくり
        /// </summary>
		private TimeSpan CalcNextBlink()
		{
            var s = BlinkInterval.TotalSeconds;
            var r = MathEx.RandDouble();

            return TimeSpan.FromSeconds(r * (2 * s - 1));
		}
		
		/// <summary>
        /// モデルのパラメータを更新
		/// </summary>
        public void UpdateParam(ALive2DModel model, TimeSpan elapsed)
        {
            double eyeParamValue;// 設定する値
            double t = 0;

            this.stateTime += elapsed.TotalSeconds;

            switch (this.eyeState)
            {
                case EyeState.First:
                default:
                    this.eyeState = EyeState.Interval;
                    this.nextBlinkTime = CalcNextBlink(); // 次回のまばたきのタイミングを始める時刻
                    this.stateTime = 0;
                    eyeParamValue = 1; // 開いた状態
                    break;

                case EyeState.Interval:
                    // まばたき開始時刻なら
                    if (this.stateTime > this.nextBlinkTime.TotalSeconds)
                    {
                        this.eyeState = EyeState.Closing;
                        this.stateTime = 0;
                    }
                    eyeParamValue = 1; // 開いた状態
                    break;

                case EyeState.Closing:
                    // 閉じるまでの割合を0..1に直す
                    t = this.stateTime / ClosingMotionSpan.TotalSeconds;
                    if (t >= 1)
                    {
                        t = 1;
                        this.eyeState = EyeState.Closed; // 次から開き始める
                        this.stateTime = 0;
                    }
                    eyeParamValue = 1 - t;
                    break;

                case EyeState.Closed:
                    t = this.stateTime / ClosedMotionSpan.TotalSeconds;
                    if (t >= 1)
                    {
                        this.eyeState = EyeState.Opening; // 次から開き始める
                        this.stateTime = 0;
                    }
                    eyeParamValue = 0;// 閉じた状態
                    break;

                case EyeState.Opening:
                    t = this.stateTime / OpeningMotionSpan.TotalSeconds;
                    if (t >= 1)
                    {
                        t = 1;
                        this.eyeState = EyeState.Interval; // 次から開き始める
                        this.nextBlinkTime = CalcNextBlink();
                        this.stateTime = 0;
                    }
                    eyeParamValue = t;
                    break;
            }

            if (!CloseIfZero)
            {
                eyeParamValue = -eyeParamValue;
            }

            // ---- 値を設定 ----
            model.setParamFloat(LeftEyeId, (float)eyeParamValue, 1.0f);
            model.setParamFloat(RightEyeId, (float)eyeParamValue, 1.0f);
        }
    }
}
