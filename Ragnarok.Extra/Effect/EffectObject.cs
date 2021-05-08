using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Markup;

using Ragnarok.MathEx;
using Ragnarok.Utility;
using Ragnarok.ObjectModel;

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
    /// エフェクト用のオブジェクトです。
    /// </summary>
    /// <remarks>
    /// 基本的に描画とサウンド以外のオブジェクトの管理を行います。
    /// </remarks>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Children")]
    public class EffectObject : NotifyObject, IFrameObject
    {
        private readonly ReentrancyLock dataContextSync = new ReentrancyLock();
        private long startTick = 0;
        private TimeSpan progressSpan = TimeSpan.Zero;
        private bool dataContextInherits = true;
        private bool initialized;
        private bool started;
        private bool terminated;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectObject()
        {
            Children = new EffectCollection(this);
            Scenario = new Scenario();
            LocalTransform = new Matrix44d();
            BaseScale = new Point3d(1.0, 1.0, 1.0);
            Scale = new Point3d(1.0, 1.0, 1.0);
            Opacity = 1.0;
            //Transform = new Matrix44d();
            InheritedOpacity = 1.0;
            IsVisible = true;

            // 値をForeverにすると、子要素があってもなくてもオブジェクトが
            // 終了しなくなります。
            // また０にすると最初のフレームでオブジェクトが破棄されてしまうため、
            // 初期化の仕方によってはDurationの設定に失敗することがあります。
            // そのため、ここでは非常に小さいけれど０より大きい値を設定しています。
            Duration = TimeSpan.FromSeconds(0.001);

            this.AddPropertyChangedHandler(nameof(Parent), (_, __) =>
            {
                OnDataContextChanged(true);
                UpdateInheritedOpacity();
            });
            this.AddPropertyChangedHandler(nameof(BasePath), (_, __) =>
                OnBasePathChanged(BasePath));
            this.AddPropertyChangedHandler(nameof(DataContext), (_, __) =>
                OnDataContextChanged(false));
            this.AddPropertyChangedHandler(nameof(Opacity), (_, __) =>
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
        /// コントロールの表示を行うかどうかを取得または設定します。
        /// </summary>
        public bool IsVisible
        {
            get
            {
                var isParentVisible = Parent?.IsVisible;
                if (isParentVisible == false)
                {
                    return false;
                }

                return GetValue<bool>(nameof(IsVisible));
            }
            set
            {
                SetValue(nameof(IsVisible), value);
            }
        }

        /// <summary>
        /// イメージの基本パスを取得または設定します。
        /// </summary>
        public string BasePath
        {
            get { return GetValue<string>(nameof(BasePath)); }
            set { SetValue(nameof(BasePath), value); }
        }

        /// <summary>
        /// BasePathが変更された時に呼ばれます。
        /// </summary>
        protected virtual void OnBasePathChanged(string basePath)
        {
            Children.ForEach(_ => _.BasePath = basePath);
        }

        /// <summary>
        /// 自分を親から外すかどうかを取得または設定します。
        /// </summary>
        public bool RemoveMe
        {
            get { return GetValue<bool>(nameof(RemoveMe)); }
            private set { SetValue(nameof(RemoveMe), value); }
        }

        /// <summary>
        /// 子があっても期間で自動的に削除するかどうかを取得または設定します。
        /// </summary>
        public bool AutoRemove
        {
            get { return GetValue<bool>(nameof(AutoRemove)); }
            set { SetValue(nameof(AutoRemove), value); }
        }

        /// <summary>
        /// オブジェクトを表示させる時間間隔を取得または設定します。
        /// </summary>
        [TypeConverter(typeof(DurationConverter))]
        public TimeSpan Duration
        {
            get { return GetValue<TimeSpan>(nameof(Duration)); }
            set { SetValue(nameof(Duration), value); }
        }

        /// <summary>
        /// 初期化から開始までの時間を取得または設定します。
        /// </summary>
        public TimeSpan WaitTime
        {
            get { return GetValue<TimeSpan>(nameof(WaitTime)); }
            set { SetValue(nameof(WaitTime), value); }
        }

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(nameof(Name)); }
            set { SetValue(nameof(Name), value); }
        }

        /// <summary>
        /// 親を取得します。
        /// </summary>
        public EffectObject Parent
        {
            get { return GetValue<EffectObject>(nameof(Parent)); }
            private set { SetValue(nameof(Parent), value); }
        }

        /// <summary>
        /// 子要素を取得します。
        /// </summary>
        public EffectCollection Children
        {
            get { return GetValue<EffectCollection>(nameof(Children)); }
            private set { SetValue(nameof(Children), value); }
        }

        /// <summary>
        /// Z座標を取得または設定します。
        /// </summary>
        public double ZOrder
        {
            get { return GetValue<double>(nameof(ZOrder)); }
            set { SetValue(nameof(ZOrder), value); }
        }

        /// <summary>
        /// 親のZOrderを考慮したZOrderを取得します。
        /// </summary>
        public double InheritedZOrder
        {
            get
            {
                if (Parent == null)
                {
                    return ZOrder;
                }
                else
                {
                    return (Parent.ZOrder + ZOrder);
                }
            }
        }

        /// <summary>
        /// XAMLにデータを渡すためのオブジェクトを取得または設定します。
        /// </summary>
        public object DataContext
        {
            get { return GetValue<object>(nameof(DataContext)); }
            set { SetValue(nameof(DataContext), value); }
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
            get { return GetValue<bool>(nameof(IsFastInitialize)); }
            set { SetValue(nameof(IsFastInitialize), value); }
        }
        #endregion

        #region Transform Property
        /// <summary>
        /// 自分のローカル座標系から親座標系への変換を行う行列を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 各オブジェクトの内部で使います。
        /// </remarks>
        protected Matrix44d LocalTransform
        {
            get { return GetValue<Matrix44d>(nameof(LocalTransform)); }
            set { SetValue(nameof(LocalTransform), value); }
        }

        /// <summary>
        /// モデル内の座標変換基準位置(0.0～1.0)を取得または設定します。
        /// </summary>
        /// <remarks>
        /// CenterPoint.Xが0.5の場合、X座標の0.5を基準として
        /// 他の座標変換を適用します。
        /// </remarks>
        public Point3d CenterPoint
        {
            get { return GetValue<Point3d>(nameof(CenterPoint)); }
            set { SetValue(nameof(CenterPoint), value); }
        }

        /// <summary>
        /// 基本となる拡大率を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 主にマスのサイズなどの初期サイズが設定されます。
        /// </remarks>
        public Point3d BaseScale
        {
            get { return GetValue<Point3d>(nameof(BaseScale)); }
            set { SetValue(nameof(BaseScale), value); }
        }

        /// <summary>
        /// 表示位置を取得または設定します。
        /// </summary>
        public Point3d Coord
        {
            get { return GetValue<Point3d>(nameof(Coord)); }
            set { SetValue(nameof(Coord), value); }
        }

        /// <summary>
        /// 拡大率を取得または設定します。
        /// </summary>
        public Point3d Scale
        {
            get { return GetValue<Point3d>(nameof(Scale)); }
            set { SetValue(nameof(Scale), value); }
        }

        /// <summary>
        /// Z軸周りの回転角を度で取得または設定します。
        /// </summary>
        public double RotateZ
        {
            get { return GetValue<double>(nameof(RotateZ)); }
            set { SetValue(nameof(RotateZ), value); }
        }

        /// <summary>
        /// このオブジェクトでの座標をワールド座標系に変換するための行列を取得します。
        /// </summary>
        /// <remarks>
        /// ワールド座標系に直すため、親の変換行列なども考慮に入れています。
        /// </remarks>
        [DependOn(nameof(Parent))]
        [DependOn(nameof(LocalTransform))]
        [DependOn(nameof(CenterPoint))]
        [DependOn(nameof(BaseScale))]
        [DependOn(nameof(Coord))]
        [DependOn(nameof(Scale))]
        [DependOn(nameof(RotateZ))]
        public Matrix44d Transform
        {
            get
            {
                var m = GetValue<Matrix44d>(nameof(Transform));
                return (m != null ? m.Clone() : new Matrix44d());
            }
            private set { SetValue(nameof(Transform), value); }
        }

        private void UpdateTransform()
        {
            // 行列変換は以下の操作が逆順で行われます。

            // 親の変換行列を基準に変換を行います。
            // （ワールド座標系から親座標系への変換行列）
            var m = (Parent != null ? Parent.Transform : new Matrix44d());

            // 親座標系からローカル座標系への変換
            m.Multiply(LocalTransform);

            // ローカル座標系での各種変換
            m.Translate(Coord.X, Coord.Y, Coord.Z);
            m.Rotate(RotateZ, 0.0, 0.0, 1.0);
            m.Scale(Scale.X, Scale.Y, Scale.Z);

            m.Scale(BaseScale.X, BaseScale.Y, BaseScale.Z);
            m.Translate(CenterPoint.X, CenterPoint.Y, CenterPoint.Z);

            Transform = m;

            Util.SafeCall(() => OnTransformUpdated());
        }

        /// <summary>
        /// Transformの更新時に呼ばれます。
        /// </summary>
        protected virtual void OnTransformUpdated()
        {
        }
        #endregion

        #region 不透明度
        /// <summary>
        /// このオブジェクトの不透明度を取得または設定します。
        /// </summary>
        public double Opacity
        {
            get { return GetValue<double>(nameof(Opacity)); }
            set { SetValue(nameof(Opacity), value); }
        }

        /// <summary>
        /// このオブジェクトの不透明度を取得または設定します。
        /// </summary>
        public double InheritedOpacity
        {
            get { return GetValue<double>(nameof(InheritedOpacity)); }
            private set { SetValue(nameof(InheritedOpacity), value); }
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

        #region その他プロパティ
        /// <summary>
        /// 初期化時に開始するストーリーボードを取得または設定します。
        /// </summary>
        public Scenario Scenario
        {
            get { return GetValue<Scenario>(nameof(Scenario)); }
            set { SetValue(nameof(Scenario), value); }
        }
        #endregion

        /// <summary>
        /// コンテンツのPathを作成します。
        /// </summary>
        public string MakeContentPath(string relativePath)
        {
            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            var path = (
                BasePath == null ?
                relativePath :
                Path.Combine(BasePath, relativePath));

            // Normalize separator
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            return path;
        }

        /// <summary>
        /// 自分を親からはずします。
        /// </summary>
        public void Kill()
        {
            RemoveMe = true;
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
            var now = Environment.TickCount64;
            if (this.startTick == 0)
            {
                this.startTick = now;
            }

            // 開始をWaitTime分だけ遅らせます。
            var time = this.startTick + WaitTime.TotalMilliseconds;
            if (now < time)
            {
                return false;
            }
            this.progressSpan = TimeSpan.FromMilliseconds(now - time);

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
        }

        /// <summary>
        /// 処理開始時に呼ばれます。
        /// </summary>
        protected virtual void OnStart()
        {
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
            OnParentAdded(parent);
        }

        protected virtual void OnParentAdded(EffectObject parent)
        {
            // 何もしません。
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

            OnParentRemoved(parent);
            Parent = null;
        }

        protected virtual void OnParentRemoved(EffectObject parent)
        {
            // 何もしません。
        }

        /// <summary>
        /// 追加の子要素があるかどうかを取得します。
        /// </summary>
        protected virtual bool HasChildren()
        {
            if (Children.Any())
            {
                return true;
            }

            return false;
        }

        #region EnterFrame
        /// <summary>
        /// フレーム毎に呼ばれ、オブジェクトの更新処理を行います。
        /// </summary>
        public void DoEnterFrame(TimeSpan elapsedTime, object state)
        {
            if (RemoveMe || !IsVisible)
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

            var e = new EnterFrameEventArgs(
                elapsedTime, progressSpan, Duration, state);
            OnEnterFrame(e);

            UpdateChildren(elapsedTime, state);
        }

        /// <summary>
        /// フレーム枚の更新処理を行います。
        /// </summary>
        protected virtual void OnEnterFrame(EnterFrameEventArgs e)
        {
            EnterFrame.SafeRaiseEvent(this, e);

            // シナリオの更新を行います。
            if (Scenario != null)
            {
                Scenario.DoEnterFrame(e.ElapsedTime, e.StateObject);
            }

            // 行列の更新
            UpdateTransform();
        }

        /// <summary>
        /// 各子要素を更新します。
        /// </summary>
        private void UpdateChildren(TimeSpan elapsedTime, object state)
        {
            for (var i = 0; i < Children.Count; )
            {
                var child = Children[i];

                child.DoEnterFrame(elapsedTime, state);
                if (child.RemoveMe)
                {
                    child.Terminate();
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
            if (RemoveMe || !IsVisible || !this.initialized || !this.started)
            {
                return;
            }

            OnRender(EventArgs.Empty);
            RenderChildren();
        }

        /// <summary>
        /// フレーム毎の描画処理を行います。
        /// </summary>
        protected virtual void OnRender(EventArgs e)
        {
            Render.SafeRaiseEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// 各子要素の描画処理を行います。
        /// </summary>
        protected void RenderChildren()
        {
            for (var i = 0; i < Children.Count; )
            {
                var child = Children[i];

                child.DoRender();
                if (child.RemoveMe)
                {
                    child.Terminate();
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
