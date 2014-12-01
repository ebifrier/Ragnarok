using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Effect
{
    public class AnimationEffect : EffectObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnimationEffect()
        {
            AnimationImageCount = 1;
        }

        #region アニメーションプロパティ
        /// <summary>
        /// 最初に画像をランダムで選ぶ場合、このリストから選択します。
        /// </summary>
        /// <remarks>
        /// XAML上での処理を考え、string型にしています。
        /// </remarks>
        public string[] InitialImageUriList
        {
            get { return GetValue<string[]>("InitialImageUriList"); }
            set { SetValue("InitialImageUriList", value); }
        }

        /// <summary>
        /// 描画するイメージのURIを取得または設定します。
        /// </summary>
        /// <remarks>
        /// XAML上での処理を考え、string型にしています。
        /// </remarks>
        public string ImageUri
        {
            get { return GetValue<string>("ImageUri"); }
            set { SetValue("ImageUri", value); }
        }

        /// <summary>
        /// アニメーションのタイプを取得または設定します。
        /// </summary>
        public AnimationType AnimationType
        {
            get { return GetValue<AnimationType>("AnimationType"); }
            set { SetValue("AnimationType", value); }
        }

        /// <summary>
        /// アニメーションさせる画像の総数を取得または設定します。
        /// </summary>
        public int AnimationImageCount
        {
            get { return GetValue<int>("AnimationImageCount"); }
            set { SetValue("AnimationImageCount", value); }
        }

        /// <summary>
        /// アニメーション画像のインデックスを取得または設定します。
        /// </summary>
        public int AnimationImageIndex
        {
            get { return GetValue<int>("AnimationImageIndex"); }
            set { SetValue("AnimationImageIndex", value); }
        }
        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // ランダムイメージの設定を行います。
            if (InitialImageUriList != null && InitialImageUriList.Any())
            {
                var index = MathEx.RandInt(0, InitialImageUriList.Length);

                ImageUri = InitialImageUriList[index];
            }
        }

        /// <summary>
        /// アニメーションインデックスの更新を行います。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);

            // 必要ならアニメーションインデックスを変更
            switch (AnimationType)
            {
                case AnimationType.Normal:
                    AnimationImageIndex = Math.Min(
                        (int)(AnimationImageCount * e.ProgressRate),
                        AnimationImageCount - 1);
                    break;
                case AnimationType.Random:
                    AnimationImageIndex = MathEx.RandInt(0, AnimationImageCount);
                    break;
            }
        }
    }
}
