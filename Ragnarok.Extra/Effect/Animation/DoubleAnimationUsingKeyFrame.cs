using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

using Ragnarok.ObjectModel;

namespace Ragnarok.Extra.Effect.Animation
{
    /// <summary>
    /// キーフレームを使ったアニメーションを行います。
    /// </summary>
    [ContentProperty("KeyFrames")]
    public class DoubleAnimationUsingKeyFrames : PropertyAnimation
    {
        /// <summary>
        /// ダミーのキーフレームオブジェクトです。
        /// </summary>
        private static readonly DoubleKeyFrame DoubleKeyFrameZero =
            new DiscreateDoubleKeyFrame
            {
                KeyTime = TimeSpan.Zero,
                Value = 0.0,
            };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DoubleAnimationUsingKeyFrames()
            : base(typeof(double))
        {
            KeyFrames = new NotifyCollection<DoubleKeyFrame>();
        }

        /// <summary>
        /// キーフレームリストを取得します。
        /// </summary>
        public NotifyCollection<DoubleKeyFrame> KeyFrames
        {
            get { return GetValue<NotifyCollection<DoubleKeyFrame>>("KeyFrames"); }
            set { SetValue("KeyFrames", value); }
        }

        /// <summary>
        /// アニメーション開始時に呼ばれます。
        /// </summary>
        protected override void OnBegin(object target)
        {
            // KeyFramesがKeyTime順に並んでいなければ例外を返します。
            KeyFrameUtil.ValidateKeyFrames(KeyFrames);

            // アニメーション期間は最後のキーフレーム時刻とします。
            var keyFrame = KeyFrames.LastOrDefault();
            if (keyFrame != null)
            {
                Duration = keyFrame.KeyTime;
            }

            base.OnBegin(target);
        }

        /// <summary>
        /// 新しいプロパティの値を取得します。
        /// </summary>
        protected override object UpdateProperty(TimeSpan frameTime)
        {
            DoubleKeyFrame prev, curr;

            if (!KeyFrameUtil.FindKeyFrame(KeyFrames, frameTime,
                                           out prev, out curr))
            {
                throw new EffectException(
                    frameTime + ": この時刻に対応するキーフレームが見つかりません。");
            }

            // prevがnullの場合は番兵としてデフォルト値を設定します。
            if (prev == null)
            {
                prev = DoubleKeyFrameZero;
            }

            // 区間が０の場合は新しい方の値を使います。
            if (prev.KeyTime == curr.KeyTime)
            {
                return curr.Value;
            }

            // 進行度などを計算します。
            var progress = (frameTime - prev.KeyTime).TotalSeconds;
            var total = (curr.KeyTime - prev.KeyTime).TotalSeconds;
            var rate = progress / total;

            // 一つ前のキーフレームを使用して補間値を求めます。
            return curr.Interpolate(prev.Value, rate);
        }
    }
}
