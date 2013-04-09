using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using FlintSharp.Renderers;
using Ragnarok;
using Ragnarok.Presentation;
using Ragnarok.Presentation.VisualObject;

namespace Ragnarok.Presentation.Shogi.Effects
{
    /// <summary>
    /// 描画方式の識別子です。
    /// </summary>
    public enum MaterialType
    {
        /// <summary>
        /// 一般的な描画方法を使います。
        /// </summary>
        Diffuse,
        /// <summary>
        /// 加算合成による描画を行います。
        /// </summary>
        Emissive,
    }

    /// <summary>
    /// アニメーションのタイプです。
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// 通常のアニメーション
        /// </summary>
        Normal,
        /// <summary>
        /// ランダム
        /// </summary>
        Random,
    }

    /// <summary>
    /// エフェクト用のオブジェクトです。
    /// </summary>
    public class EffectObject : EntityObject, IUriContext
    {
        /// <summary>
        /// エフェクトや画像の基本パスです。
        /// </summary>
        public static readonly Uri DefaultBasePath;

        /// <summary>
        /// 静敵コンストラクタ
        /// </summary>
        static EffectObject()
        {
            if (!Ragnarok.Presentation.WPFUtil.IsInDesignMode)
            {
                DefaultBasePath = new Uri(
                    new Uri(Assembly.GetEntryAssembly().Location),
                    "ShogiData/xxx");
            }
        }

        private GeometryModel3D model;
        private Brush brush;
        private Model3DRenderer renderer;

        #region 描画モデルなど
        /// <summary>
        /// パーティクルの放出元リストを取得します。
        /// </summary>
        [CLSCompliant(false)]
        public EmitterCollection Emitters
        {
            get { return this.renderer.Emitters; }
        }

        /// <summary>
        /// 描画されるジオメトリを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MeshProperty =
            DependencyProperty.Register(
                "Mesh", typeof(Geometry3D), typeof(EffectObject),
                new UIPropertyMetadata(null, OnModelChanged));

        /// <summary>
        /// 描画されるジオメトリを取得します。
        /// </summary>
        public Geometry3D Mesh
        {
            get { return (Geometry3D)GetValue(MeshProperty); }
            set { SetValue(MeshProperty, value); }
        }

        /// <summary>
        /// マテリアルの種別を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MaterialTypeProperty =
            DependencyProperty.Register(
                "MaterialType", typeof(MaterialType), typeof(EffectObject),
                new UIPropertyMetadata(MaterialType.Emissive, OnModelChanged));

        /// <summary>
        /// マテリアルの種別を取得または設定します。
        /// </summary>
        public MaterialType MaterialType
        {
            get { return (MaterialType)GetValue(MaterialTypeProperty); }
            set { SetValue(MaterialTypeProperty, value); }
        }

        /// <summary>
        /// イメージに適用される色を取得または設定します。
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                "Color", typeof(Color), typeof(EffectObject),
                new UIPropertyMetadata(Colors.White, OnModelChanged));

        /// <summary>
        /// イメージに適用される色を取得または設定します。
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as EffectObject;
            if (self != null)
            {
                self.UpdateModel();
            }
        }
        #endregion

        #region イメージ
        /// <summary>
        /// イメージの基本パスを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BaseUriProperty =
            DependencyProperty.Register(
                "BaseUri", typeof(Uri), typeof(EffectObject),
                new UIPropertyMetadata(null));

        /// <summary>
        /// イメージの基本パスを取得または設定します。
        /// </summary>
        /// <remarks>
        /// XAML読み込み時に自動的に設定されます。
        /// </remarks>
        public Uri BaseUri
        {
            get { return (Uri)GetValue(BaseUriProperty); }
            set { SetValue(BaseUriProperty, value); }
        }

        /// <summary>
        /// 描画するイメージのURIを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register(
                "ImageUri", typeof(string), typeof(EffectObject),
                new UIPropertyMetadata(null));

        /// <summary>
        /// 描画するイメージのURIを取得または設定します。
        /// </summary>
        /// <remarks>
        /// XAML上での処理を考え、string型にしています。
        /// </remarks>
        public string ImageUri
        {
            get { return (string)GetValue(ImageUriProperty); }
            set { SetValue(ImageUriProperty, value); }
        }

        /// <summary>
        /// 画像をランダムで選ぶ場合に使われるリストを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ImageUriListProperty =
            DependencyProperty.Register(
                "ImageUriList", typeof(string[]), typeof(EffectObject),
                new UIPropertyMetadata(null));

        /// <summary>
        /// 画像をランダムで選ぶ場合、このリストから選択します。
        /// </summary>
        /// <remarks>
        /// XAML上での処理を考え、string型にしています。
        /// </remarks>
        public string[] ImageUriList
        {
            get { return (string[])GetValue(ImageUriListProperty); }
            set { SetValue(ImageUriListProperty, value); }
        }

        /// <summary>
        /// メッシュをイメージに合わせて自動で更新するかどうかを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoUpdateMeshProperty =
            DependencyProperty.Register(
                "AutoUpdateMesh", typeof(bool), typeof(EffectObject),
                new UIPropertyMetadata(true));

        /// <summary>
        /// メッシュをイメージに合わせて自動で更新するかどうかを取得または設定します。
        /// </summary>
        public bool AutoUpdateMesh
        {
            get { return (bool)GetValue(AutoUpdateMeshProperty); }
            set { SetValue(AutoUpdateMeshProperty, value); }
        }
        #endregion

        #region アニメーション関係
        /// <summary>
        /// アニメーションさせる画像の総数を取得または設定します。
        /// </summary>
        public static readonly DependencyProperty AnimationTypeProperty =
            DependencyProperty.Register(
                "AnimationType", typeof(AnimationType), typeof(EffectObject),
                new UIPropertyMetadata(AnimationType.Normal, OnAnimationIndexChanged));

        /// <summary>
        /// アニメーションのタイプを取得または設定します。
        /// </summary>
        public AnimationType AnimationType
        {
            get { return (AnimationType)GetValue(AnimationTypeProperty); }
            set { SetValue(AnimationTypeProperty, value); }
        }

        /// <summary>
        /// アニメーションさせる画像の総数を取得または設定します。
        /// </summary>
        public static readonly DependencyProperty AnimationImageCountProperty =
            DependencyProperty.Register(
                "AnimationImageCount", typeof(int), typeof(EffectObject),
                new UIPropertyMetadata(1, OnAnimationIndexChanged));

        /// <summary>
        /// アニメーションさせる画像の総数を取得または設定します。
        /// </summary>
        public int AnimationImageCount
        {
            get { return (int)GetValue(AnimationImageCountProperty); }
            set { SetValue(AnimationImageCountProperty, value); }
        }

        /// <summary>
        /// アニメーション画像のインデックスを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty AnimationImageIndexProperty =
            DependencyProperty.Register(
                "AnimationImageIndex", typeof(int), typeof(EffectObject),
                new UIPropertyMetadata(0, OnAnimationIndexChanged));

        /// <summary>
        /// アニメーション画像のインデックスを取得または設定します。
        /// </summary>
        public int AnimationImageIndex
        {
            get { return (int)GetValue(AnimationImageIndexProperty); }
            set { SetValue(AnimationImageIndexProperty, value); }
        }

        static void OnAnimationIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as EffectObject;
            if (self != null)
            {
                self.UpdateAnimationIndex();
            }
        }
        #endregion

        #region 不透明度
        /// <summary>
        /// 不透明度を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                "Opacity", typeof(double), typeof(EffectObject),
                new UIPropertyMetadata(1.0, OnOpacityChanged));

        /// <summary>
        /// 不透明度を取得または設定します。
        /// </summary>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as EffectObject;
            if (self != null)
            {
                self.UpdateBrushOpacity();
            }
        }
        #endregion

        #region サウンド
        /// <summary>
        /// 開始時に鳴らすサウンドのパスを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty StartSoundPathProperty =
            DependencyProperty.Register(
                "StartSoundPath", typeof(string), typeof(EffectObject),
                new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// 開始時に鳴らすサウンドのパスを取得または設定します。
        /// </summary>
        public string StartSoundPath
        {
            get { return (string)GetValue(StartSoundPathProperty); }
            set { SetValue(StartSoundPathProperty, value); }
        }

        /// <summary>
        /// 開始時に鳴らすサウンドの音量を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty StartSoundVolumeProperty =
            DependencyProperty.Register(
                "StartSoundVolume", typeof(double), typeof(EffectObject),
                new UIPropertyMetadata(1.0));

        /// <summary>
        /// 開始時に鳴らすサウンドの音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double StartSoundVolume
        {
            get { return (double)GetValue(StartSoundVolumeProperty); }
            set { SetValue(StartSoundVolumeProperty, value); }
        }
        #endregion

        /// <summary>
        /// コンテンツのURIを作成します。
        /// </summary>
        private Uri MakeContentUri(string relativeUri)
        {
            if (BaseUri != null)
            {
                return new Uri(BaseUri, relativeUri);
            }
            else
            {
                return new Uri(DefaultBasePath, relativeUri);
            }
        }

        /// <summary>
        /// オブジェクトを初期化し、viewportに追加します。
        /// </summary>
        protected override GeometryModel3D OnLoadModel()
        {
            if (ImageUriList != null && ImageUriList.Any())
            {
                ImageUri = ImageUriList[MathEx.RandInt(0, ImageUriList.Length)];
            }

            if (ImageUri != null)
            {
                this.brush = new ImageBrush();

                UpdateAnimationIndex();
            }
            else
            {
                this.brush = new SolidColorBrush();
            }

            this.model = new GeometryModel3D();

            // ブラシ属性などを更新します。
            UpdateBrushOpacity();
            UpdateModel();

            return this.model;
        }

        /// <summary>
        /// 開始時の処理を行います。
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();

            PlayStartSound();

            foreach (var emitter in this.renderer.Emitters)
            {
                emitter.Start();
            }
        }

        /// <summary>
        /// 開始時の効果音を再生します。
        /// </summary>
        public bool PlayStartSound(bool isVolumeZero = false)
        {
            // 開始時のサウンドを鳴らします。
            var soundPath = StartSoundPath;
            if (string.IsNullOrEmpty(soundPath))
            {
                return false;
            }

            var uri = MakeContentUri(soundPath);

            SoundManager.Instance.PlaySE(
                uri.LocalPath,
                (isVolumeZero ? 0.0 : StartSoundVolume),
                false);

            return true;
        }

        /// <summary>
        /// モデルに関係するメッシュやマテリアルなどを更新します。
        /// </summary>
        private void UpdateModel()
        {
            if (this.model == null)
            {
                return;
            }

            // 画像がない場合は、色をブラシで設定します。
            // (Materialの設定は無視されます)
            var colorBrush = this.brush as SolidColorBrush;
            if (colorBrush != null)
            {
                colorBrush.Color = Color;
            }

            this.model.Geometry = Mesh;
            this.model.Material =
                ( MaterialType == MaterialType.Diffuse
                ? (Material)new DiffuseMaterial(this.brush)
                {
                    Color = Color,
                }
                : new EmissiveMaterial(this.brush)
                {
                    Color = Color,
                });
        }

        /// <summary>
        /// エフェクトのアニメーション画像の位置などを修正します。
        /// </summary>
        private void UpdateAnimationIndex()
        {
            var brush = this.brush as ImageBrush;
            if (brush == null)
            {
                return;
            }

            var imageList = EffectImageCache.GetImageList(
                MakeContentUri(ImageUri),
                AnimationImageCount);
            if (imageList == null)
            {
                return;
            }

            var oldImage = brush.ImageSource;
            var newImage = imageList[AnimationImageIndex];

            // 必要ならメッシュの更新を行います。
            if (AutoUpdateMesh)
            {
                if (newImage != null)
                {
                    if (oldImage == null ||
                        oldImage.Width != newImage.Width ||
                        oldImage.Height != newImage.Height)
                    {
                        Mesh = WPFUtil.CreateDefaultMesh(
                            1.0, 1.0, newImage.Width, newImage.Height);
                    }
                }
                else
                {
                    Mesh = null;
                }
            }

            brush.ImageSource = newImage;
        }

        /// <summary>
        /// 不透明度が変わったときに呼ばれます。
        /// </summary>
        private void UpdateBrushOpacity()
        {
            if (this.brush == null)
            {
                return;
            }

            this.brush.Opacity = Opacity;
        }

        /// <summary>
        /// 追加の子要素があるかどうかを取得します。
        /// </summary>
        protected override bool HasChildren()
        {
            if (base.HasChildren())
            {
                return true;
            }

            return !this.renderer.Emitters.All(_ => !_.Particles.Any());
        }

        /// <summary>
        /// フレーム毎に呼ばれます。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);

            if (AnimationType == AnimationType.Normal)
            {
                AnimationImageIndex = Math.Min(
                    (int)(AnimationImageCount * e.ProgressRate),
                    AnimationImageCount - 1);
            }
            else
            {
                AnimationImageIndex = MathEx.RandInt(0, AnimationImageCount);
            }

            foreach (var emitter in this.renderer.Emitters)
            {
                emitter.OnUpdateFrame(e.ElapsedTime.TotalSeconds);
            }
        }

        /// <summary>
        /// コピーを作成します。
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new EffectObject();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectObject()
        {
            this.renderer = new Model3DRenderer(ModelGroup);

            // 値をForeverにすると、子要素があってもなくてもオブジェクトが
            // 終了しなくなります。
            // また０にすると最初のフレームでオブジェクトが破棄されてしまうため、
            // 非常に小さいけれど０より大きい値を設定しています。
            Duration = TimeSpan.FromSeconds(0.01);
        }
    }
}
