using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ragnarok.Presentation.VisualObject.Control
{
    /// <summary>
    /// VisualBackground.xaml の相互作用ロジック
    /// </summary>
    [TemplatePart(Type = typeof(BackgroundCore), Name = "Background1")]
    [TemplatePart(Type = typeof(BackgroundCore), Name = "Background2")]
    public partial class VisualBackground : System.Windows.Controls.Control
    {
        /// <summary>
        /// 背景用のコントロール名。
        /// </summary>
        private const string Background1Name = "Background1";
        /// <summary>
        /// 背景用のコントロール名。
        /// </summary>
        private const string Background2Name = "Background2";

        /*/// <summary>
        /// 背景のビューポートを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register(
                "Viewport", typeof(Rect), typeof(VisualBackground),
                new UIPropertyMetadata(new Rect(0, 0, 100, 100)));

        /// <summary>
        /// 背景のビューポートを取得または設定します。
        /// </summary>
        public Rect Viewport
        {
            get { return (Rect)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }*/

        /// <summary>
        /// 背景の切り替え時に一瞬フェードアウトさせる
        /// コントロールを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty FadeElementProperty =
            DependencyProperty.Register(
                "FadeElement", typeof(UIElement), typeof(VisualBackground),
                new UIPropertyMetadata(null));

        /// <summary>
        /// 背景の切り替え時に一瞬フェードアウトさせる
        /// コントロールを取得または設定します。
        /// </summary>
        public UIElement FadeElement
        {
            get { return (UIElement)GetValue(FadeElementProperty); }
            set { SetValue(FadeElementProperty, value); }
        }

        private BackgroundCore currentBg, nextBg;
        private EntityObject cache;

        /// <summary>
        /// テンプレートが変わったときに呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.currentBg = GetTemplateChild(Background1Name) as BackgroundCore;
            this.nextBg = GetTemplateChild(Background2Name) as BackgroundCore;

            if (this.currentBg != null)
            {
            }

            // キャッシュしたエフェクトを使います。
            if (this.nextBg != null && this.cache != null)
            {
                AddEntity(this.cache);
                this.cache = null;
            }
        }

        /// <summary>
        /// 背景のトランジションを開始します。
        /// </summary>
        private void StartTransition(bool isFadeElement)
        {
            var fadeTime0 = TimeSpan.FromSeconds(0.0);
            var fadeTime1 = TimeSpan.FromSeconds(1.0);
            var fadeTime2 = TimeSpan.FromSeconds(2.0);
            var fadeTime3 = TimeSpan.FromSeconds(3.0);

            var animFore = new DoubleAnimationUsingKeyFrames();
            animFore.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, fadeTime0));
            animFore.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, fadeTime3));

            var animBack = new DoubleAnimationUsingKeyFrames();
            animBack.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, fadeTime0));
            animBack.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, fadeTime3));

            animFore.Completed +=
                (_, __) => this.nextBg.Entity = null;

            this.nextBg.BeginAnimation(UIElement.OpacityProperty, animFore);
            this.currentBg.BeginAnimation(UIElement.OpacityProperty, animBack);

            // 他要素のフェードイン/アウトの設定
            if (isFadeElement && FadeElement != null)
            {
                var opacity = FadeElement.Opacity;
                var anim = new DoubleAnimationUsingKeyFrames()
                {
                    FillBehavior = FillBehavior.Stop,
                };
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(opacity, fadeTime0));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.4, fadeTime1));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.4, fadeTime2));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(opacity, fadeTime3));

                FadeElement.BeginAnimation(UIElement.OpacityProperty, anim);
            }

            // 短い時間で背景の切り替えが起こる可能性があるため、
            // ここでnextとcurrentを入れ替えておきます。
            var tmp = this.currentBg;
            this.currentBg = this.nextBg;
            this.nextBg = tmp;
        }

        /// <summary>
        /// 次のエフェクトを設定します。
        /// </summary>
        public void AddEntity(EntityObject effect)
        {
            if (this.currentBg == null || this.nextBg == null)
            {
                // 使われなかった分は一つだけキャッシュし、後で使います。
                this.cache = effect;
                return;
            }

            // 同じエフェクトは表示しません。
            if ((this.currentBg.Entity == null && effect == null) ||
                (this.currentBg.Entity != null && effect != null &&
                 this.currentBg.Entity.Name == effect.Name))
            {
                return;
            }

            this.nextBg.Entity = effect;

            // 今も次の背景もあるなら、盤のフェードを行います。
            StartTransition(
                (this.currentBg.Entity != null &&
                 this.nextBg.Entity != null));
        }

        /// <summary>
        /// 描画処理などを行います。
        /// </summary>
        public void Render(TimeSpan elapsedTime)
        {
            if (this.currentBg != null)
            {
                this.currentBg.Render(elapsedTime);
            }

            if (this.nextBg != null)
            {
                this.nextBg.Render(elapsedTime);
            }
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static VisualBackground()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(VisualBackground),
                new FrameworkPropertyMetadata(typeof(VisualBackground)));
        }
    }
}
