using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

using FlintSharp;
using FlintSharp.Renderers;
using Ragnarok.Utility;
using Ragnarok.ObjectModel;
using Ragnarok.Extra.Sound;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// 描画方式の識別子です。
    /// </summary>
    public enum BlendType
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
    /// <remarks>
    /// 基本的に描画とサウンド以外のオブジェクトの管理を行います。
    /// </remarks>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Children")]
    public class EffectObject : NotifyObject, IFrameObject, IUriContext
    {
        private readonly ReentrancyLock dataContextSync = new ReentrancyLock();
        private DateTime startTime = DateTime.MinValue;
        private TimeSpan progressSpan = TimeSpan.Zero;
        private bool dataContextInherits = true;
        private bool initialized;
        private bool started;
        private bool terminated;
        
        /// <summary>
        /// サウンド再生用オブジェクトを取得または設定します。
        /// </summary>
        [CLSCompliant(false)]
        public static SoundManager SoundManager
        {
            get;
            set;
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static EffectObject()
        {
            SoundManager = new SoundManager();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectObject()
        {
            Children = new EffectCollection(this);
            BaseScale = new Point3d(1.0, 1.0, 1.0);
            Scale = new Point3d(1.0, 1.0, 1.0);
            AnimationImageCount = 1;
            BlendType = BlendType.Diffuse;
            Opacity = 1.0;
            InheritedOpacity = 1.0;

            // 値をForeverにすると、子要素があってもなくてもオブジェクトが
            // 終了しなくなります。
            // また０にすると最初のフレームでオブジェクトが破棄されてしまうため、
            // 初期化の仕方によってはDurationの設定に失敗することがあります。
            // そのため、ここでは非常に小さいけれど０より大きい値を設定しています。
            Duration = TimeSpan.FromSeconds(0.001);

            this.AddPropertyChangedHandler("Parent", (_, __) =>
            {
                OnDataContextChanged(true);
                UpdateInheritedOpacity();
            });
            this.AddPropertyChangedHandler("DataContext", (_, __) =>
                OnDataContextChanged(false));
            this.AddPropertyChangedHandler("Opacity", (_, __) =>
                UpdateInheritedOpacity());

            lock (instanceList)
            {
                instanceList.Add(new WeakReference(this));
            }
        }

        /// <summary>
        /// 各フレームで呼ばれるイベントです。
        /// </summary>
        public event EventHandler<EnterFrameEventArgs> EnterFrame;

        /// <summary>
        /// 各フレームの描画処理を行うイベントです。
        /// </summary>
        public event EventHandler Render;

        /// <summary>
        /// オブジェクトの初期化時に呼ばれます。
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// オブジェクトの終了時に呼ばれます。
        /// </summary>
        public event EventHandler Terminated;

        #region 基本プロパティ
        /// <summary>
        /// イメージの基本パスを取得または設定します。
        /// </summary>
        /// <remarks>
        /// XAML読み込み時に自動的に設定されます。
        /// </remarks>
        public Uri BaseUri
        {
            get { return GetValue<Uri>("BaseUri"); }
            set { SetValue("BaseUri", value); }
        }

        /// <summary>
        /// 自分を親から外すかどうかを取得または設定します。
        /// </summary>
        public bool RemoveMe
        {
            get { return GetValue<bool>("RemoveMe"); }
            private set { SetValue("RemoveMe", value); }
        }

        /// <summary>
        /// 子があっても期間で自動的に削除するかどうかを取得または設定します。
        /// </summary>
        public bool AutoRemove
        {
            get { return GetValue<bool>("AutoRemove"); }
            set { SetValue("AutoRemove", value); }
        }

        /// <summary>
        /// オブジェクトを表示させる時間間隔を取得または設定します。
        /// </summary>
        [TypeConverter(typeof(DurationConverter))]
        public TimeSpan Duration
        {
            get { return GetValue<TimeSpan>("Duratio"); }
            set { SetValue("Duratio", value); }
        }

        /// <summary>
        /// 初期化から開始までの時間を取得または設定します。
        /// </summary>
        public TimeSpan WaitTime
        {
            get { return GetValue<TimeSpan>("WaitTime"); }
            set { SetValue("WaitTime", value); }
        }

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name
        {
            get { return GetValue<string>("Name"); }
            set { SetValue("Name", value); }
        }

        /// <summary>
        /// 親を取得します。
        /// </summary>
        public EffectObject Parent
        {
            get { return GetValue<EffectObject>("Parent"); }
            private set { SetValue("Parent", value); }
        }

        /// <summary>
        /// 子要素を取得します。
        /// </summary>
        public EffectCollection Children
        {
            get { return GetValue<EffectCollection>("Children"); }
            private set { SetValue("Children", value); }
        }

        /// <summary>
        /// XAMLにデータを渡すためのオブジェクトを取得または設定します。
        /// </summary>
        public object DataContext
        {
            get { return GetValue<object>("DataContext"); }
            set { SetValue("DataContext", value); }
        }

        /// <summary>
        /// DataContextにかかわるプロパティが変更されたときに呼ばれます。
        /// </summary>
        private void OnDataContextChanged(bool parentChanged)
        {
            // OnDataContextChangedは再度呼ばれることがあるため、
            // このようにして無限ループを防いでいます。
            using (var result = this.dataContextSync.Lock())
            {
                if (result == null) return;

                // 親のDataContextを使う設定の時、親が変更された場合は
                // DataContextを新たに設定しなおします。
                if (this.dataContextInherits && parentChanged)
                {
                    var p = Parent;

                    // ここでさらにOnDataContextChangedが呼ばれることがあります。
                    DataContext = (p != null ? p.DataContext : null);
                }
                else if (!parentChanged)
                {
                    // 自分のDataContextが変更された場合は
                    // それを使い続けます。
                    this.dataContextInherits = false;
                }

                // 子のDataContextも再設定します。
                Children.ForEach(_ => _.OnDataContextChanged(true));
            }
        }

        /// <summary>
        /// Initializeを開始フレームと同じフレームで実行するかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// Initializeには時間がかかることがあるため、
        /// 通常は最初のフレームでInitializeを呼び、
        /// 次のフレームから開始処理を行います。
        /// </remarks>
        public bool IsFastInitialize
        {
            get { return GetValue<bool>("IsFastInitialize"); }
            set { SetValue("IsFastInitialize", value); }
        }

        /// <summary>
        /// パーティクルのレンダラーを取得または設定します。
        /// </summary>
        [CLSCompliant(false)]
        public Renderer ParticleRenderer
        {
            get { return GetValue<Renderer>("ParticleRenderer"); }
            set { SetValue("ParticleRenderer", value); }
        }

        /// <summary>
        /// パーティクルの放出元リストを取得します。
        /// </summary>
        [CLSCompliant(false)]
        public EmitterCollection ParticleEmitters
        {
            get
            {
                if (ParticleRenderer == null)
                {
                    return null;
                }

                return ParticleRenderer.Emitters;
            }
        }
        #endregion

        #region Transform Property
        #region 基本変換プロパティ
        /// <summary>
        /// モデル内の座標変換基準位置(0.0～1.0)を取得または設定します。
        /// </summary>
        /// <remarks>
        /// CenterPoint.Xが0.5の場合、X座標の0.5を基準として
        /// 他の座標変換を適用します。
        /// </remarks>
        public Point3d CenterPoint
        {
            get { return GetValue<Point3d>("CenterPoint"); }
            set { SetValue("CenterPoint", value); }
        }

        /// <summary>
        /// 基本となる拡大率を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 主にマスのサイズなどの初期サイズが設定されます。
        /// </remarks>
        public Point3d BaseScale
        {
            get { return GetValue<Point3d>("BaseScale"); }
            set { SetValue("BaseScale", value); }
        }
        #endregion

        #region その他
        /// <summary>
        /// 拡大率を取得または設定します。
        /// </summary>
        public Point3d Scale
        {
            get { return GetValue<Point3d>("Scale"); }
            set { SetValue("Scale", value); }
        }

        /// <summary>
        /// Z軸周りの回転角を度で取得または設定します。
        /// </summary>
        public double RotateZ
        {
            get { return GetValue<double>("RotateZ"); }
            set { SetValue("RotateZ", value); }
        }

        /// <summary>
        /// 表示位置を取得または設定します。
        /// </summary>
        public Point3d Coord
        {
            get { return GetValue<Point3d>("Coord"); }
            set { SetValue("Coord", value); }
        }

        /// <summary>
        /// Z座標を取得または設定します。
        /// </summary>
        public double ZOrder
        {
            get { return GetValue<double>("ZOrder"); }
            set { SetValue("ZOrder", value); }
        }
        #endregion
        #endregion

        #region 不透明度
        /// <summary>
        /// このオブジェクトの不透明度を取得または設定します。
        /// </summary>
        public double Opacity
        {
            get { return GetValue<double>("Opacity"); }
            set { SetValue("Opacity", value); }
        }

        /// <summary>
        /// このオブジェクトの不透明度を取得または設定します。
        /// </summary>
        public double InheritedOpacity
        {
            get { return GetValue<double>("InheritedOpacity"); }
            private set { SetValue("InheritedOpacity", value); }
        }

        /// <summary>
        /// 親の不透明度を含めた実際の不透明度を更新します。
        /// </summary>
        private void UpdateInheritedOpacity()
        {
            var parentOpacity = (
                Parent != null ?
                Parent.InheritedOpacity :
                1.0);

            UpdateInheritedOpacity(parentOpacity);
        }

        /// <summary>
        /// 親の不透明度を含めた実際の不透明度を更新します。
        /// </summary>
        private void UpdateInheritedOpacity(double parentOpacity)
        {
            var inheritedOpacity = parentOpacity * Opacity;
            InheritedOpacity = inheritedOpacity;

            Children.ForEach(_ => _.UpdateInheritedOpacity(inheritedOpacity));
        }
        #endregion

        #region イメージ・アニメーション関係
        /// <summary>
        /// 描画種別を取得または設定します。
        /// </summary>
        public BlendType BlendType
        {
            get { return GetValue<BlendType>("BlendType"); }
            set { SetValue("BlendType", value); }
        }

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

        #region サウンド
        /// <summary>
        /// 開始時に鳴らすサウンドのパスを取得または設定します。
        /// </summary>
        public string StartSoundPath
        {
            get { return GetValue<string>("StartSoundPath"); }
            set { SetValue("StartSoundPath", value); }
        }

        /// <summary>
        /// 開始時に鳴らすサウンドの音量を0.0～1.0の範囲で取得または設定します。
        /// </summary>
        public double StartSoundVolume
        {
            get { return GetValue<double>("StartSoundVolume"); }
            set { SetValue("StartSoundVolume", value); }
        }
        #endregion

        #region その他プロパティ
        /// <summary>
        /// 初期化時に開始するストーリーボードを取得または設定します。
        /// </summary>
        public Scenario Scenario
        {
            get { return GetValue<Scenario>("Scenario"); }
            set { SetValue("Scenario", value); }
        }
        #endregion

        /// <summary>
        /// コンテンツのURIを作成します。
        /// </summary>
        public Uri MakeContentUri(string relativeUri)
        {
            if (BaseUri != null)
            {
                return new Uri(BaseUri, relativeUri);
            }
            else
            {
                return new Uri(relativeUri, UriKind.Relative);
            }
        }

        /// <summary>
        /// 自分を親からはずします。
        /// </summary>
        public void Kill()
        {
            RemoveMe = true;
        }

        /// <summary>
        /// 指定の係数を各オブジェクトの音量にかけて調整します。
        /// </summary>
        /// <remarks>
        /// 子オブジェクトの音量も調整します。
        /// </remarks>
        public void MultiplyStartVolume(double rate)
        {
            StartSoundVolume *= rate;

            foreach (var child in Children)
            {
                if (child == null)
                {
                    continue;
                }

                child.MultiplyStartVolume(rate);
            }
        }

        /// <summary>
        /// 開始時の効果音を再生します。
        /// </summary>
        public bool PlayStartSound(bool isVolumeZero = false)
        {
            var soundManager = SoundManager;
            if (soundManager == null)
            {
                return false;
            }

            // 開始時のサウンドを鳴らします。
            var soundPath = StartSoundPath;
            if (string.IsNullOrEmpty(soundPath))
            {
                return false;
            }

            var uri = MakeContentUri(soundPath);
            soundManager.PlaySE(
                uri.LocalPath,
                (isVolumeZero ? 0.0 : StartSoundVolume),
                false);

            return true;
        }

        /// <summary>
        /// オブジェクトを初期化し、viewportに追加します。
        /// </summary>
        private void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            OnInitialize();
            Initialized.SafeRaiseEvent(this, EventArgs.Empty);

            this.initialized = true;
        }

        /// <summary>
        /// アニメーションなどの処理を開始します。
        /// </summary>
        /// <remarks>
        /// 初期化に時間がかかる場合があるので、
        /// 開始と初期化のフレームを分けます。
        /// </remarks>
        private bool Start()
        {
            if (this.started)
            {
                return true;
            }

            // 初回の呼び出し時に時刻を設定します。
            var now = DateTime.Now;
            if (this.startTime == DateTime.MinValue)
            {
                this.startTime = now;
            }

            // 開始をWaitTime分だけ遅らせます。
            var time = this.startTime + WaitTime;
            if (now < time)
            {
                return false;
            }
            this.progressSpan = now - time;

            // シナリオを開始します。
            if (Scenario != null)
            {
                Scenario.Begin(this);
            }

            OnStart();

            this.started = true;
            return true;
        }

        /// <summary>
        /// オブジェクトの終了処理を行います。
        /// </summary>
        public void Terminate()
        {
            if (this.terminated)
            {
                return;
            }

            RemoveMe = true;
            this.terminated = true;

            Terminated.SafeRaiseEvent(this, EventArgs.Empty);
            OnTerminate();

            // 子要素も終了処理を行います。
            foreach (var child in Children)
            {
                child.Terminate();
            }
            Children.Clear();

            // すべてのバインディングとアニメーションを開放します。
            if (Scenario != null)
            {
                Scenario.Stop();
                Scenario.Children.Clear();
            }

            // 一応開放
            Initialized = null;
            Terminated = null;
            EnterFrame = null;
        }

        /// <summary>
        /// 初期化時に呼ばれます。
        /// </summary>
        protected virtual void OnInitialize()
        {
            // ランダムイメージの設定を行います。
            if (InitialImageUriList != null && InitialImageUriList.Any())
            {
                ImageUri = InitialImageUriList[MathEx.RandInt(0, InitialImageUriList.Length)];
            }
        }

        /// <summary>
        /// 処理開始時に呼ばれます。
        /// </summary>
        protected virtual void OnStart()
        {
            if (ParticleRenderer != null)
            {
                foreach (var emitter in ParticleRenderer.Emitters)
                {
                    emitter.Start();
                }
            }

            PlayStartSound();
        }

        /// <summary>
        /// 終了時に呼ばれます。
        /// </summary>
        protected virtual void OnTerminate()
        {
        }

        /// <summary>
        /// 子要素が追加されたときに呼ばれます。
        /// </summary>
        internal void ParentAdded(EffectObject parent)
        {
            if (Parent != null)
            {
                throw new InvalidOperationException(
                    "EntityObjectにはすでに親が設定されています。");
            }

            Parent = parent;
        }

        /// <summary>
        /// 子要素が削除されたときに呼ばれます。
        /// </summary>
        internal void ParentRemoved(EffectObject parent)
        {
            if (Parent == null)
            {
                throw new InvalidOperationException(
                    "EntityObjectに親が設定されていません。");
            }

            Parent = null;

            // 終了処理を行います。
            Terminate();
        }

        /// <summary>
        /// 追加の子要素があるかどうかを取得します。
        /// </summary>
        private bool HasChildren()
        {
            if (Children.Any())
            {
                return true;
            }

            if (ParticleRenderer == null)
            {
                return true;
            }

            return !ParticleRenderer.Emitters.All(_ => !_.Particles.Any());
        }

        #region EnterFrame
        /// <summary>
        /// フレーム毎に呼ばれ、オブジェクトの更新処理を行います。
        /// </summary>
        public void DoEnterFrame(TimeSpan elapsedTime)
        {
            if (RemoveMe)
            {
                return;
            }

            // 初期化はフレームを分けます。
            if (!this.initialized)
            {
                Initialize();

                // 初期化フレームと開始フレームを分ける場合のみ
                // ここで処理を終了します。
                if (!IsFastInitialize)
                {
                    return;
                }
            }

            if (!this.started)
            {
                if (!Start()) return;
            }
            else
            {
                // もし時間制限があればその時間経過後はオブジェクトを削除します。
                this.progressSpan += elapsedTime;
                if (this.progressSpan >= Duration && (AutoRemove || !HasChildren()))
                {
                    Kill();
                    return;
                }
            }

            var e = new EnterFrameEventArgs(elapsedTime, progressSpan, Duration);
            OnEnterFrame(e);

            UpdateChildren(elapsedTime);
        }

        /// <summary>
        /// フレーム枚の更新処理を行います。
        /// </summary>
        protected virtual void OnEnterFrame(EnterFrameEventArgs e)
        {
            EnterFrame.SafeRaiseEvent(this, e);

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

            // シナリオの更新を行います。
            if (Scenario != null)
            {
                Scenario.DoEnterFrame(e.ElapsedTime);
            }

            // パーティクルの更新を行います。
            if (ParticleRenderer != null)
            {
                ParticleRenderer.OnUpdateFrame(e.ElapsedTime.TotalSeconds);
            }
        }

        /// <summary>
        /// 各子要素を更新します。
        /// </summary>
        private void UpdateChildren(TimeSpan elapsedTime)
        {
            for (var i = 0; i < Children.Count; )
            {
                var entity = Children[i];

                entity.DoEnterFrame(elapsedTime);
                if (entity.RemoveMe)
                {
                    Children.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }
        #endregion

        #region Render
        /// <summary>
        /// 描画メソッドを呼びます。
        /// </summary>
        public void DoRender()
        {
            // 未初期化の場合は描画処理を行いません。
            if (RemoveMe || !this.initialized || !this.started)
            {
                return;
            }

            OnRender(EventArgs.Empty);
            Render.SafeRaiseEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// フレーム毎の描画処理を行います。
        /// </summary>
        protected virtual void OnRender(EventArgs e)
        {
            RenderChildren();
        }

        /// <summary>
        /// パーティクルの描画を行います。
        /// </summary>
        protected void RenderParticles()
        {
            if (ParticleRenderer != null)
            {
                ParticleRenderer.OnRenderFrame();
            }
        }

        /// <summary>
        /// 各子要素の描画処理を行います。
        /// </summary>
        protected void RenderChildren()
        {
            for (var i = 0; i < Children.Count; )
            {
                var entity = Children[i];

                entity.DoRender();
                if (entity.RemoveMe)
                {
                    Children.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }
        #endregion

        /// <summary>
        /// 生存しているオブジェクトのリストです。
        /// </summary>
        private static HashSet<WeakReference> instanceList =
            new HashSet<WeakReference>();

        /// <summary>
        /// 生存しているEntityObjectのインスタンス一覧を取得します。
        /// (デバッグ用)
        /// </summary>
        public static EffectObject[] GetInstanceList()
        {
            lock (instanceList)
            {
                return instanceList
                    .Select(_ => (EffectObject)_.Target)
                    .Where(_ => _ != null)
                    .ToArray();
            }
        }
    }
}
