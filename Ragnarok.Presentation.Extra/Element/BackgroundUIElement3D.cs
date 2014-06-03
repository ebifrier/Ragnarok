using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ragnarok.Presentation.Extra.Element
{
    using Entity;
    using Effect;

    /// <summary>
    /// クロスフェード可能な背景オブジェクトです。
    /// </summary>
    public class BackgroundUIElement3D : UIElement3D
    {
        /// <summary>
        /// 背景の切り替え時間を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty FadeDurationProperty =
            DependencyProperty.Register(
                "FadeDuration", typeof(Duration), typeof(BackgroundUIElement3D),
                new FrameworkPropertyMetadata(new Duration(TimeSpan.FromSeconds(4))));

        /// <summary>
        /// 背景の切り替え時間を取得または設定します。
        /// </summary>
        public Duration FadeDuration
        {
            get { return (Duration)GetValue(FadeDurationProperty); }
            set { SetValue(FadeDurationProperty, value); }
        }

        private Model3DGroup modelGroup;
        private EffectObject prevBg, nextBg;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BackgroundUIElement3D()
        {
            this.prevBg = new EffectObject()
            {
                Duration = Duration.Forever,
                Opacity = 0.0,
            };
            this.nextBg = new EffectObject()
            {
                Duration = Duration.Forever,
                Opacity = 0.0,
            };

            this.modelGroup = new Model3DGroup();
            this.modelGroup.Children.Add(this.prevBg.ModelGroup);
            this.modelGroup.Children.Add(this.nextBg.ModelGroup);

            Visual3DModel = this.modelGroup;
            IsHitTestVisible = false;
        }

        /// <summary>
        /// エレメントのアンロードを行います。
        /// </summary>
        public void Unload()
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
        }

        /// <summary>
        /// 背景のトランジションを開始します。
        /// </summary>
        private void StartTransition()
        {
            var seconds = FadeDuration.TimeSpan.TotalSeconds;
            var fadeTime0 = TimeSpan.FromSeconds(seconds / 4 * 1);
            var fadeTime1 = TimeSpan.FromSeconds(seconds / 2);
            var fadeTime2 = TimeSpan.FromSeconds(seconds / 4 * 3);
            var fadeTime3 = TimeSpan.FromSeconds(seconds);

            var animFore = new DoubleAnimationUsingKeyFrames();
            animFore.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, fadeTime0));
            animFore.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, fadeTime3));

            var animBack = new DoubleAnimationUsingKeyFrames();
            animBack.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, fadeTime0));
            animBack.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, fadeTime3));

            animFore.Completed += (_, __) =>
                { if (this.nextBg != null) this.nextBg.Children.Clear(); };

            // 短い時間で背景の切り替えが起こる可能性があるため、
            // ここでnextとcurrentを入れ替えておきます。
            Util.Swap(ref this.prevBg, ref this.nextBg);

            // nextには古いエンティティが入っています。
            // 新しいエンティティのアニメーションが終了すると、
            // nextが消えてしまうため、必ず古いエンティティから
            // アニメーションを開始します。
            this.nextBg.BeginAnimation(EffectObject.OpacityProperty, animBack);
            this.prevBg.BeginAnimation(EffectObject.OpacityProperty, animFore);
        }

        /// <summary>
        /// 次のエフェクトを設定します。
        /// </summary>
        public void AddEntity(EntityObject effect)
        {
            if (this.prevBg == null || this.nextBg == null)
            {
                return;
            }

            // 同じエフェクトは表示しません。
            if ((!this.prevBg.Children.Any() && effect == null) ||
                ( this.prevBg.Children.Any() && effect != null &&
                 this.prevBg.Name == effect.Name))
            {
                return;
            }

            //this.nextBg.BeginAnimation(EffectObject.OpacityProperty, null);
            //this.prevBg.BeginAnimation(EffectObject.OpacityProperty, null);

            this.nextBg.Opacity = 0.0;
            this.nextBg.Name = (effect != null ? effect.Name : string.Empty);

            this.nextBg.Children.Clear();
            if (effect != null)
            {
                this.nextBg.Children.Add(effect);
            }

            StartTransition();
        }

        /// <summary>
        /// 描画処理などを行います。
        /// </summary>
        public void Render(TimeSpan elapsedTime)
        {
            if (this.prevBg != null)
            {
                this.prevBg.DoEnterFrame(elapsedTime);
            }

            if (this.nextBg != null)
            {
                this.nextBg.DoEnterFrame(elapsedTime);
            }
        }

        /// <summary>
        /// よくわからないけど、オーバーライドします。
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new UIElement3DAutomationPeer(this);
        }
    }
}
