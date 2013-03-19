using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Ragnarok.Presentation.VisualObject
{
    /// <summary>
    /// 各フレームで渡される引数です。
    /// </summary>
    public class EnterFrameEventArgs : EventArgs
    {
        /// <summary>
        /// このフレームの時間を取得または設定します。
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get;
            set;
        }

        /// <summary>
        /// 表示期間を取得または設定します。
        /// </summary>
        public TimeSpan Duration
        {
            get;
            set;
        }

        /// <summary>
        /// 表示期間がある場合、その進み具合を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 範囲は0.0～1.0です。
        /// </remarks>
        public double ProgressRate
        {
            get;
            set;
        }

        /// <summary>
        /// 初期化からの経過時間を取得または設定します。
        /// </summary>
        public TimeSpan ProgressSpan
        {
            get;
            set;
        }

        /// <summary>
        /// 初期化からの開始時間を秒で取得または設定します。
        /// </summary>
        public double ProgressSeconds
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnterFrameEventArgs(TimeSpan elapsedTime, TimeSpan progressSpan, Duration duration)
        {
            Duration = (duration.HasTimeSpan ? duration.TimeSpan : TimeSpan.MaxValue);
            ElapsedTime = elapsedTime;
            ProgressSpan = progressSpan;
            ProgressSeconds = ProgressSpan.TotalSeconds;

            ProgressRate = 0.0;
            if (duration != TimeSpan.MaxValue)
            {
                var rate = (double)progressSpan.TotalSeconds / Duration.TotalSeconds;

                ProgressRate = Math.Min(1.0, rate);
            }
        }
    }
}
