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

        private EyeState eyeState;// 現在の状態
        private long nextBlinkTime ;// 次回眼パチする時刻（msec）
		private long stateStartTime ;// 現在のstateが開始した時刻
			
		// 左右の目のパラメータ
		private string eyeID_L;
		private string eyeID_R;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DEyeBlink()
        {
            // 妥当と思われる値で初期化
            this.eyeState = EyeState.First;

            BlinkIntervalMillis = 4000;// 瞬きの間隔

            ClosingMotionMillis = 100;// 眼が閉じるまでの時間
            ClosedMotionMillis = 50;// 閉じたままでいる時間
            OpeningMotionMillis = 150;// 眼が開くまでの時間

            CloseIfZero = true;

            // 左右の目のパラメータ
            eyeID_L = "PARAM_EYE_L_OPEN";
            eyeID_R = "PARAM_EYE_R_OPEN";
        }

        /// <summary>
        /// 瞬きの間隔を取得または設定します。
        /// </summary>
        public int BlinkIntervalMillis
        {
            get;
            set;
        }

        /// <summary>
        /// 眼が閉じるまでの時間を取得または設定します。
        /// </summary>
        public int ClosingMotionMillis
        {
            get;
            set;
        }

        /// <summary>
        /// 眼が閉じたままでいる時間を取得または設定します。
        /// </summary>
        public int ClosedMotionMillis
        {
            get;
            set;
        }

        /// <summary>
        /// 眼が開くまでの時間を取得または設定します。
        /// </summary>
        public int OpeningMotionMillis
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
		private long CalcNextBlink()
		{
			var time = Environment.TickCount;
            var r = MathEx.RandDouble();

            return (time + (long)(r * (2 * BlinkIntervalMillis - 1)));
		}
		
		/// <summary>
        /// モデルのパラメータを更新
		/// </summary>
        public void SetParam(ALive2DModel model)
        {
            long time = Environment.TickCount;
            double eyeParamValue;// 設定する値
            double t = 0;

            switch (this.eyeState)
            {
                case EyeState.Closing:
                    // 閉じるまでの割合を0..1に直す
                    // (BlinkMotionMillisの半分の時間で閉じる)
                    t = (time - stateStartTime) / (double)ClosingMotionMillis;
                    if (t >= 1)
                    {
                        t = 1;
                        this.eyeState = EyeState.Closed;// 次から開き始める
                        this.stateStartTime = time;
                    }
                    eyeParamValue = 1 - t;
                    break;
                case EyeState.Closed:
                    t = (time - stateStartTime) / (double)ClosedMotionMillis;
                    if (t >= 1)
                    {
                        this.eyeState = EyeState.Opening;// 次から開き始める
                        this.stateStartTime = time;
                    }
                    eyeParamValue = 0;// 閉じた状態
                    break;
                case EyeState.Opening:
                    t = (time - stateStartTime) / (double)OpeningMotionMillis;
                    if (t >= 1)
                    {
                        t = 1;
                        this.eyeState = EyeState.Interval;// 次から開き始める
                        this.nextBlinkTime = CalcNextBlink();// 次回のまばたきのタイミングを始める時刻
                    }
                    eyeParamValue = t;
                    break;
                case EyeState.Interval:
                    // まばたき開始時刻なら
                    if (this.nextBlinkTime < time)
                    {
                        this.eyeState = EyeState.Closing;
                        this.stateStartTime = time;
                    }
                    eyeParamValue = 1;// 開いた状態
                    break;
                case EyeState.First:
                default:
                    this.eyeState = EyeState.Interval;
                    this.nextBlinkTime = CalcNextBlink();// 次回のまばたきのタイミングを始める時刻
                    eyeParamValue = 1;// 開いた状態
                    break;
            }

            if (!CloseIfZero)
            {
                eyeParamValue = -eyeParamValue;
            }

            // ---- 値を設定 ----
            model.setParamFloat(eyeID_L, (float)eyeParamValue, 1.0f);
            model.setParamFloat(eyeID_R, (float)eyeParamValue, 1.0f);
        }
    }
}
