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

    /// <summary>
    /// クロスフェード可能な背景オブジェクトです。
    /// </summary>
    public class BackgroundUIElement3D : UIElement3D
    {
        /// <summary>
        /// 背景の切り替え時、一緒にフェードアウトさせる
        /// コントロールを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty FadeElementProperty =
            DependencyProperty.Register(
                "FadeElement", typeof(UIElement), typeof(BackgroundUIElement3D),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 背景の切り替え時、一緒にフェードアウトさせる
        /// コントロールを取得または設定します。
        /// </summary>
        public UIElement FadeElement
        {
            get { return (UIElement)GetValue(FadeElementProperty); }
            set { SetValue(FadeElementProperty, value); }
        }

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
        private EntityObject currentBg_, nextBg_;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BackgroundUIElement3D()
        {
            this.modelGroup = new Model3DGroup();
            Visual3DModel = this.modelGroup;

            IsHitTestVisible = false;
        }

        /// <summary>
        /// 背景のトランジションを開始します。
        /// </summary>
        private void StartTransition(bool isFadeElement)
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
            {
                if (this.nextBg_ != null)
                {
                    this.modelGroup.Children.Remove(this.nextBg_.ModelGroup);
                    this.nextBg_ = null;
                }
            };

            // 短い時間で背景の切り替えが起こる可能性があるため、
            // ここでnextとcurrentを入れ替えておきます。
            Util.Swap(ref this.currentBg_, ref this.nextBg_);

            // エンティティのクリア
            this.modelGroup.Children.Clear();

            // nextには古いエンティティが入っています。
            // 新しいエンティティのアニメーションが終了すると、
            // nextが消えてしまうため、必ず古いエンティティから
            // アニメーションを開始します。
            if (this.nextBg_ != null)
            {
                //this.modelGroup.Children.Add(this.nextBg_.ModelGroup);
                this.nextBg_.BeginAnimation(UIElement.OpacityProperty, animBack);
            }

            if (this.currentBg_ != null)
            {
                this.modelGroup.Children.Add(this.currentBg_.ModelGroup);
                this.currentBg_.BeginAnimation(UIElement.OpacityProperty, animFore);
            }

            // 他要素のフェードイン/アウトの設定
            if (isFadeElement && FadeElement != null)
            {
                var opacity = FadeElement.Opacity;
                var anim = new DoubleAnimationUsingKeyFrames()
                {
                    FillBehavior = FillBehavior.Stop
                };
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(opacity, fadeTime0));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.4, fadeTime1));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.4, fadeTime2));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(opacity, fadeTime3));

                FadeElement.BeginAnimation(UIElement.OpacityProperty, anim);
            }
        }

        /// <summary>
        /// 次のエフェクトを設定します。
        /// </summary>
        public void AddEntity(EntityObject effect)
        {
            // 同じエフェクトは表示しません。
            if ((this.currentBg_ == null && effect == null) ||
                (this.currentBg_ != null && effect != null &&
                 this.currentBg_.Name == effect.Name))
            {
                return;
            }

            this.nextBg_ = effect;

            // 今も次の背景もあるなら、盤のフェードを行います。
            StartTransition(this.currentBg_ != null && this.nextBg_ != null);
        }

        /// <summary>
        /// 描画処理などを行います。
        /// </summary>
        public void Render(TimeSpan elapsedTime)
        {
            if (this.currentBg_ != null)
            {
                this.currentBg_.DoEnterFrame(elapsedTime);
            }

            if (this.nextBg_ != null)
            {
                this.nextBg_.DoEnterFrame(elapsedTime);
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
