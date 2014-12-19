using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;

using Ragnarok.Extra.Effect;
using Ragnarok.Extra.Effect.Animation;
using Ragnarok.Forms.Shogi.Effect;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// OpenGLの背景表示用クラスです。
    /// </summary>
    public class GLBackgroundElement : GLElement
    {
        private EffectObject prevBg;
        private EffectObject nextBg;
        private TimeSpan defaultFadeDuration;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLBackgroundElement()
        {
            DefaultFadeDuration = TimeSpan.FromSeconds(4);
        }

        /// <summary>
        /// デフォルトの背景の切り替え時間を取得または設定します。
        /// </summary>
        public TimeSpan DefaultFadeDuration
        {
            get { return this.defaultFadeDuration;; }
            set { this.defaultFadeDuration = value; }
        }

        /// <summary>
        /// エレメントのアンロードを行います。
        /// </summary>
        protected override void OnTerminate()
        {
            if (this.prevBg != null)
            {
                this.prevBg.Terminate();
                this.prevBg = null;
            }

            if (this.nextBg != null)
            {
                this.nextBg.Terminate();
                this.nextBg = null;
            }

            base.OnTerminate();
        }

        /// <summary>
        /// 次に表示する背景エフェクトを設定します。
        /// </summary>
        public void AddBackgroundEffect(EffectObject effect, TimeSpan? duration = null)
        {
            // 同じエフェクトは表示しません。
            if ((this.prevBg == null && effect == null) ||
                (this.prevBg != null && effect != null &&
                 this.prevBg.Name == effect.Name))
            {
                return;
            }

            var shogi = effect as ShogiObject;
            if (shogi != null)
            {
                shogi.OpenGL = OpenGL;
            }

            // 古い背景エフェクトの廃棄
            if (this.nextBg != null)
            {
                this.nextBg.Terminate();
                this.nextBg = null;
            }

            // 次の背景に設定
            this.nextBg = effect;

            StartTransition(duration);
        }

        /// <summary>
        /// 背景のトランジションを開始します。
        /// </summary>
        private void StartTransition(TimeSpan? duration)
        {
            var timeSpan = (duration != null ? duration.Value : DefaultFadeDuration);

            // animForeはnextBgに対応し、0⇒1でαが変わる。
            // animBackはprevBgに対応し、1⇒0でαが変わる。
            var animFore = new DoubleAnimationUsingKeyFrames
            {
                TargetProperty = "Opacity",
                FillBehavior = FillBehavior.HoldEnd,
            };
            animFore.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, TimeSpan.Zero));
            animFore.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, timeSpan));

            var animBack = new DoubleAnimationUsingKeyFrames
            {
                TargetProperty = "Opacity",
                FillBehavior = FillBehavior.HoldEnd,
            };
            animBack.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, TimeSpan.Zero));
            animBack.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, timeSpan));

            // アニメーション完了時は、nextBg(実際はprevBg)を廃棄します。
            animBack.Completed += (_, __) =>
                this.nextBg = null;

            // prevには古い背景が入っています。
            if (this.prevBg != null)
            {
                this.prevBg.Scenario.Children.Add(animBack);
            }
            if (this.nextBg != null)
            {
                this.nextBg.Scenario.Children.Add(animFore);
            }
            
            // アニメーション開始前にオブジェクトの内容を入れ替えます。
            Util.Swap(ref this.prevBg, ref this.nextBg);
        }

        /// <summary>
        /// 更新処理を行います。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);
            var renderBuffer = (GL.RenderBuffer)e.StateObject;

            if (this.prevBg != null)
            {
                this.prevBg.DoEnterFrame(e.ElapsedTime, renderBuffer);
            }

            if (this.nextBg != null)
            {
                this.nextBg.DoEnterFrame(e.ElapsedTime, renderBuffer);
            }
        }

        /// <summary>
        /// 描画処理などを行います。
        /// </summary>
        protected override void OnRender(EventArgs e)
        {
            base.OnRender(e);

            if (IsVisible)
            {
                if (this.prevBg != null)
                {
                    this.prevBg.DoRender();
                }

                if (this.nextBg != null)
                {
                    this.nextBg.DoRender();
                }
            }
        }
    }
}
