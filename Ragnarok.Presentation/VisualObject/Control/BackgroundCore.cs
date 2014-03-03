using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ragnarok.Presentation.VisualObject.Control
{
    /// <summary>
    /// BackgroundCore.xaml の相互作用ロジック
    /// </summary>
    [TemplatePart(Type = typeof(Viewport3D), Name = "PART_Viewport3D")]
    [TemplatePart(Type = typeof(OrthographicCamera), Name = "PART_Camera")]
    [TemplatePart(Type = typeof(Model3DGroup), Name = "PART_EffectGroup")]
    internal class BackgroundCore : System.Windows.Controls.Control
    {
        /// <summary>
        /// エフェクト用のコンテナ名。
        /// </summary>
        private const string Viewport3DName = "PART_Viewport3D";
        private const string CameraName = "PART_Camera";
        private const string EffectGroupName = "PART_EffectGroup";

        private Viewport3D viewport3D;
        private OrthographicCamera camera;
        private Model3DGroup effectGroup;
        private EntityObject rootEffect;

        /// <summary>
        /// 背景のビューポートを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register(
                "Viewport", typeof(Rect), typeof(BackgroundCore),
                new FrameworkPropertyMetadata(new Rect(0, 0, 100, 100), OnViewportChanged));

        /// <summary>
        /// 背景のビューポートを取得または設定します。
        /// </summary>
        public Rect Viewport
        {
            get { return (Rect)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        private static void OnViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as BackgroundCore;

            if (self != null)
            {
                self.UpdateViewport((Rect)e.NewValue);
            }
        }

        /// <summary>
        /// 背景エフェクトを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty EntityProperty =
            DependencyProperty.Register(
                "Entity", typeof(EntityObject), typeof(BackgroundCore),
                new FrameworkPropertyMetadata((EntityObject)null, OnEntityChanged));

        /// <summary>
        /// 背景エフェクトの要素を取得または設定します。
        /// </summary>
        public EntityObject Entity
        {
            get { return (EntityObject)GetValue(EntityProperty); }
            set { SetValue(EntityProperty, value); }
        }

        /// <summary>
        /// エフェクトが変わったときに呼ばれます。
        /// </summary>
        static void OnEntityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as BackgroundCore;

            if (self != null)
            {
                self.OnEntityChanged((EntityObject)e.NewValue);
            }
        }

        /// <summary>
        /// 背景用のエミッター付きエフェクトを初期化します。
        /// </summary>
        private void OnEntityChanged(EntityObject effect)
        {
            if (Ragnarok.Presentation.WPFUtil.IsInDesignMode)
            {
                return;
            }

            if (this.effectGroup == null)
            {
                // エフェクト自体は保存し、後で使えるようにします。
                this.rootEffect = effect;
                return;
            }

            if (effect == null)
            {
                this.effectGroup.Children.Clear();
                this.rootEffect = null;
            }
            else
            {
                this.rootEffect = effect;

                this.effectGroup.Children.Clear();
                this.effectGroup.Children.Add(effect.ModelGroup);
            }
        }

        /// <summary>
        /// テンプレートが変わったときに呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.effectGroup != null)
            {
                this.effectGroup.Children.Clear();
            }

            this.viewport3D = GetTemplateChild(Viewport3DName) as Viewport3D;
            this.camera = GetTemplateChild(CameraName) as OrthographicCamera;
            this.effectGroup = GetTemplateChild(EffectGroupName) as Model3DGroup;

            if (this.effectGroup != null && this.rootEffect != null)
            {
                this.effectGroup.Children.Clear();
                this.effectGroup.Children.Add(this.rootEffect.ModelGroup);
            }

            UpdateViewport(Viewport);
        }

        /// <summary>
        /// ビューポートを更新します。
        /// </summary>
        private void UpdateViewport(Rect viewport)
        {
            if (this.viewport3D != null)
            {
                this.viewport3D.Width = viewport.Width;
                this.viewport3D.Height = viewport.Height;
            }

            if (this.camera != null)
            {
                this.camera.Width = viewport.Width;
                this.camera.Position = new Point3D(
                    (viewport.Left + viewport.Right) / 2.0,
                    (viewport.Top + viewport.Bottom) / 2.0,
                    -500.0);
            }
        }

        /// <summary>
        /// 描画処理などを行います。
        /// </summary>
        public void Render(TimeSpan elapsedTime)
        {
            if (this.rootEffect != null)
            {
                this.rootEffect.DoEnterFrame(elapsedTime);
            }
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static BackgroundCore()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(BackgroundCore),
                new FrameworkPropertyMetadata(typeof(BackgroundCore)));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BackgroundCore()
        {
            Unloaded += Control_Unloaded;
        }

        /// <summary>
        /// エフェクトオブジェクトは後片付けしないと、リークします。
        /// </summary>
        void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.rootEffect != null)
            {
                this.rootEffect.Terminate();
                this.rootEffect = null;
            }
        }
    }
}
