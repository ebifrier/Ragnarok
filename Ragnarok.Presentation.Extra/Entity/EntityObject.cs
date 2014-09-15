using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using Ragnarok.Utility;
using Ragnarok.ObjectModel;

namespace Ragnarok.Presentation.Extra.Entity
{
    /// <summary>
    /// エフェクト用のオブジェクトです。
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Children")]
    public class EntityObject : Animatable
    {
        private readonly ReentrancyLock dataContextSync = new ReentrancyLock();
        private DateTime startTime = DateTime.MinValue;
        private TimeSpan progressSpan = TimeSpan.Zero;
        private bool dataContextInherits = true;
        private bool initialized;
        private bool started;
        private bool terminated;

        /// <summary>
        /// 各フレームで呼ばれるイベントです。
        /// </summary>
        public event EventHandler<EnterFrameEventArgs> EnterFrame;

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
        /// ３Ｄのモデルオブジェクトのルートグループを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ModelGroupProperty =
            DependencyProperty.Register(
                "ModelGroup", typeof(Model3DGroup), typeof(EntityObject),
                new UIPropertyMetadata(null));

        /// <summary>
        /// ３Ｄのモデルオブジェクトのルートグループを取得します。
        /// </summary>
        /// <remarks>
        /// グループの中には自分のモデルと子のモデルが含まれます。
        /// </remarks>
        public Model3DGroup ModelGroup
        {
            get { return (Model3DGroup)GetValue(ModelGroupProperty); }
            private set { SetValue(ModelGroupProperty, value); }
        }

        /// <summary>
        /// 自分を親から外すかどうかを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty RemoveMeProperty =
            DependencyProperty.Register(
                "RemoveMe", typeof(bool), typeof(EntityObject),
                new UIPropertyMetadata(false));

        /// <summary>
        /// 自分を親から外すかどうかを取得または設定します。
        /// </summary>
        public bool RemoveMe
        {
            get { return (bool)GetValue(RemoveMeProperty); }
            private set { SetValue(RemoveMeProperty, value); }
        }

        /// <summary>
        /// 子があっても期間で自動的に削除するかどうかを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoRemoveProperty =
            DependencyProperty.Register(
                "AutoRemove", typeof(bool), typeof(EntityObject),
                new UIPropertyMetadata(false));

        /// <summary>
        /// 子があっても期間で自動的に削除するかどうかを取得または設定します。
        /// </summary>
        public bool AutoRemove
        {
            get { return (bool)GetValue(AutoRemoveProperty); }
            set { SetValue(AutoRemoveProperty, value); }
        }

        /// <summary>
        /// オブジェクトを表示させる時間間隔を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(
                "Duration", typeof(Duration), typeof(EntityObject),
                new UIPropertyMetadata(Duration.Forever));

        /// <summary>
        /// オブジェクトを表示させる時間間隔を取得または設定します。
        /// </summary>
        /// <remarks>
        /// Foreverの場合は子要素がなくなるまで表示します。
        /// </remarks>
        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// 初期化から開始までの時間を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty WaitTimeProperty =
            DependencyProperty.Register(
                "WaitTime", typeof(Duration), typeof(EntityObject),
                new UIPropertyMetadata(new Duration(TimeSpan.Zero)));

        /// <summary>
        /// 初期化から開始までの時間を取得または設定します。
        /// </summary>
        public Duration WaitTime
        {
            get { return (Duration)GetValue(WaitTimeProperty); }
            set { SetValue(WaitTimeProperty, value); }
        }

        /// <summary>
        /// 名前を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register(
                "Name", typeof(string), typeof(EntityObject),
                new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// 親を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.Register(
                "Parent", typeof(EntityObject), typeof(EntityObject),
                new UIPropertyMetadata(null, OnParentChanged));

        /// <summary>
        /// 親を取得します。
        /// </summary>
        public EntityObject Parent
        {
            get { return (EntityObject)GetValue(ParentProperty); }
            private set { SetValue(ParentProperty, value); }
        }

        /// <summary>
        /// Parentプロパティが変更されたときに呼ばれます。
        /// </summary>
        static void OnParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as EntityObject;

            if (ReferenceEquals(e.OldValue, e.NewValue))
            {
                return;
            }

            if (self != null)
            {
                self.OnDataContextChanged(true);
                self.UpdateInheritedOpacity();
            }
        }

        /// <summary>
        /// 子要素を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register(
                "Children", typeof(EntityCollection), typeof(EntityObject),
                new UIPropertyMetadata(null));

        /// <summary>
        /// 子要素を取得します。
        /// </summary>
        public EntityCollection Children
        {
            get { return (EntityCollection)GetValue(ChildrenProperty); }
            private set { SetValue(ChildrenProperty, value); }
        }

        /// <summary>
        /// XAMLにデータを渡すためのオブジェクトを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(
                "DataContext", typeof(object), typeof(EntityObject),
                new UIPropertyMetadata(null, OnDataContextChanged));

        /// <summary>
        /// XAMLにデータを渡すためのオブジェクトを取得または設定します。
        /// </summary>
        public object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        /// <summary>
        /// DataContextにかかわるプロパティが変更されたときに呼ばれます。
        /// </summary>
        static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as EntityObject;

            if (self != null)
            {
                self.OnDataContextChanged(false);
            }
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
        /// Initializeを開始フレームと同じフレームで実行するか扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsFastInitializeProperty =
            DependencyProperty.Register(
                "IsFastInitialize", typeof(bool), typeof(EntityObject),
                new UIPropertyMetadata(false));

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
            get { return (bool)GetValue(IsFastInitializeProperty); }
            set { SetValue(IsFastInitializeProperty, value); }
        }
        #endregion

        #region Transform Property
        #region 基本変換プロパティ
        /// <summary>
        /// モデル内の座標変換基準位置(0.0～1.0)を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty CenterPointProperty =
            DependencyProperty.Register(
                "CenterPoint", typeof(Point3D), typeof(EntityObject),
                new UIPropertyMetadata(new Point3D(0, 0, 0)));

        /// <summary>
        /// モデル内の座標変換基準位置(0.0～1.0)を取得または設定します。
        /// </summary>
        public Point3D CenterPoint
        {
            get { return (Point3D)GetValue(CenterPointProperty); }
            set { SetValue(CenterPointProperty, value); }
        }

        /// <summary>
        /// 基本となる拡大率を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BaseScaleProperty =
            DependencyProperty.Register(
                "BaseScale", typeof(Vector3D), typeof(EntityObject),
                new UIPropertyMetadata(new Vector3D(1, 1, 1)));

        /// <summary>
        /// 基本となる拡大率を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 主にマスのサイズなどの初期サイズが設定されます。
        /// </remarks>
        public Vector3D BaseScale
        {
            get { return (Vector3D)GetValue(BaseScaleProperty); }
            set { SetValue(BaseScaleProperty, value); }
        }
        #endregion

        #region 拡大縮小
        /// <summary>
        /// 拡大率を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                "Scale", typeof(Vector3D), typeof(EntityObject),
                new UIPropertyMetadata(new Vector3D(1,1,1)));

        /// <summary>
        /// 拡大率を取得または設定します。
        /// </summary>
        public Vector3D Scale
        {
            get { return (Vector3D)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        #endregion

        #region 回転
        /// <summary>
        /// Z軸周りの回転を度で取得または設定します。
        /// </summary>
        private RotateTransform3D RotateZTransform
        {
            get;
            set;
        }

        /// <summary>
        /// Z軸周りの回転角を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty RotateZProperty =
            DependencyProperty.Register(
                "RotateZ", typeof(double), typeof(EntityObject),
                new UIPropertyMetadata(-1.0, OnRotateZChanged));

        /// <summary>
        /// Z軸周りの回転角を度で取得または設定します。
        /// </summary>
        public double RotateZ
        {
            get { return (double)GetValue(RotateZProperty); }
            set { SetValue(RotateZProperty, value); }
        }

        /// <summary>
        /// Z軸の回転角が変更されたときに呼ばれます。
        /// </summary>
        static void OnRotateZChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as EntityObject;
            if (self != null)
            {
                self.RotateZTransform = new RotateTransform3D(
                    new QuaternionRotation3D(
                        new Quaternion(new Vector3D(0, 0, 1), self.RotateZ)));

                self.UpdateMatrix();
            }
        }
        #endregion

        #region 移動
        /// <summary>
        /// 表示位置を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty CoordProperty =
            DependencyProperty.Register(
                "Coord", typeof(Vector3D), typeof(EntityObject),
                new UIPropertyMetadata(new Vector3D()));

        /// <summary>
        /// 表示位置を取得または設定します。
        /// </summary>
        public Vector3D Coord
        {
            get { return (Vector3D)GetValue(CoordProperty); }
            set { SetValue(CoordProperty, value); }
        }
        #endregion
        #endregion

        #region 不透明度
        /// <summary>
        /// このオブジェクトの不透明度を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                "Opacity", typeof(double), typeof(EntityObject),
                new UIPropertyMetadata(1.0, OnOpacityChanged));

        /// <summary>
        /// このオブジェクトの不透明度を取得または設定します。
        /// </summary>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as EntityObject;
            if (self != null)
            {
                self.UpdateInheritedOpacity();
            }
        }

        /// <summary>
        /// 親の不透明度を考慮した不透明度を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty InheritedOpacityProperty =
            DependencyProperty.Register(
                "InheritedOpacity", typeof(double), typeof(EntityObject),
                new UIPropertyMetadata(1.0, OnInheritedOpacityChanged));

        /// <summary>
        /// このオブジェクトの不透明度を取得または設定します。
        /// </summary>
        public double InheritedOpacity
        {
            get { return (double)GetValue(InheritedOpacityProperty); }
            private set { SetValue(InheritedOpacityProperty, value); }
        }

        static void OnInheritedOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as EntityObject;
            if (self != null)
            {
                self.OnInheritedOpacityUpdated((double)e.OldValue, (double)e.NewValue);
            }
        }

        /// <summary>
        /// 実際の不透明度の更新時に呼ばれます。
        /// </summary>
        protected virtual void OnInheritedOpacityUpdated(double oldValue,
                                                         double newValue)
        {
            // 何もしません。
        }
        #endregion

        #region その他プロパティ
        /// <summary>
        /// 初期化時に開始するストーリーボードを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ScenarioProperty =
            DependencyProperty.Register(
                "Scenario", typeof(Scenario), typeof(EntityObject),
                new UIPropertyMetadata(null));

        /// <summary>
        /// 初期化時に開始するストーリーボードを取得または設定します。
        /// </summary>
        public Scenario Scenario
        {
            get { return (Scenario)GetValue(ScenarioProperty); }
            set { SetValue(ScenarioProperty, value); }
        }
        #endregion

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

        /// <summary>
        /// 変換行列更新時に呼ばれます。
        /// </summary>
        private void UpdateMatrix()
        {
            /*if (ModelGroup != null)
            {
                ModelGroup.Transform = CreateMatrix();
            }*/
        }

        /// <summary>
        /// 変換用の行列を作成します。
        /// </summary>
        protected virtual Transform3DGroup CreateMatrix()
        {
            var result = new Transform3DGroup();

            // 最初に中心位置を移動します。
            var cp = CenterPoint;
            if (cp.X != 0.0 || cp.Y != 0.0 || cp.Z != 0.0)
            {
                result.Children.Add(new TranslateTransform3D((Vector3D)cp));
            }

            // 事前の変換をかけます。
            var bs = BaseScale;
            if (bs.X != 1.0 || bs.Y != 1.0 || bs.Z != 1.0)
            {
                result.Children.Add(new ScaleTransform3D(bs));
            }

            result.Children.Add(new ScaleTransform3D(Scale));
            result.Children.Add(RotateZTransform);
            result.Children.Add(new TranslateTransform3D(Coord));
            return result;
        }

        /// <summary>
        /// 自分を親からはずします。
        /// </summary>
        public void Kill()
        {
            RemoveMe = true;
        }

        /// <summary>
        /// ３Ｄのモデルオブジェクトを読み込みます。
        /// </summary>
        protected virtual GeometryModel3D OnLoadModel()
        {
            return null;
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

            var model = OnLoadModel();
            if (model != null)
            {
                ModelGroup.Children.Add(model);
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
            var time = this.startTime + WaitTime.TimeSpan;
            if (now < time)
            {
                return false;
            }
            this.progressSpan = now - time;

            var behaviors = Interaction.GetBehaviors(this);
            if (behaviors != null)
            {
                behaviors.Attach(this);
            }

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

            if (ModelGroup != null)
            {
                ModelGroup.Children.Clear();
            }

            var behaviors = Interaction.GetBehaviors(this);
            if (behaviors != null)
            {
                behaviors.Detach();

                BindingOperations.ClearAllBindings(behaviors);
                Interaction.ResetBehaviours(this);
            }

            // すべてのバインディングとアニメーションを開放します。
            if (Scenario != null)
            {
                Scenario.Stop();
                Scenario.Children.Clear();

                BindingOperations.ClearAllBindings(Scenario);
                Scenario = null;
            }

            BindingOperations.ClearAllBindings(this);

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
        internal void AddedToParent(EntityObject parent)
        {
            if (Parent != null)
            {
                throw new InvalidOperationException(
                    "EntityObjectにはすでに親が設定されています。");
            }

            Parent = parent;

            // 描画用モデルと子要素のリスト、両方に追加します。
            parent.ModelGroup.Children.Add(ModelGroup);
        }

        /// <summary>
        /// 子要素が削除されたときに呼ばれます。
        /// </summary>
        internal void RemovedFromParent(EntityObject parent)
        {
            if (Parent == null)
            {
                throw new InvalidOperationException(
                    "EntityObjectに親が設定されていません。");
            }

            Parent = null;
            parent.ModelGroup.Children.Remove(ModelGroup);

            // 終了処理を行います。
            Terminate();
        }

        /// <summary>
        /// 追加の子要素があるかどうかを取得します。
        /// </summary>
        protected virtual bool HasChildren()
        {
            return Children.Any();
        }

        /// <summary>
        /// フレーム毎に呼ばれます。
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

            // もし時間制限があればその時間経過後はオブジェクトを削除します。
            this.progressSpan += elapsedTime;
            if (this.progressSpan >= Duration && (AutoRemove || !HasChildren()))
            {
                Kill();
                return;
            }

            var e = new EnterFrameEventArgs(elapsedTime, progressSpan, Duration);
            OnEnterFrame(e);

            // 表示行列を更新します。
            ModelGroup.Transform = CreateMatrix();

            UpdateChildren(elapsedTime);
        }

        /// <summary>
        /// フレーム枚の更新処理を行います。
        /// </summary>
        protected virtual void OnEnterFrame(EnterFrameEventArgs e)
        {
            EnterFrame.SafeRaiseEvent(this, e);
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

        /// <summary>
        /// クローンを作成します。
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new EntityObject();
        }

        /// <summary>
        /// 生存しているオブジェクトのリストです。
        /// </summary>
        private static HashSet<WeakReference> instanceList =
            new HashSet<WeakReference>();

        /// <summary>
        /// 生存しているEntityObjectのインスタンス一覧を取得します。
        /// (デバッグ用)
        /// </summary>
        public static EntityObject[] GetInstanceList()
        {
            lock (instanceList)
            {
                return instanceList
                    .Select(_ => (EntityObject)_.Target)
                    .Where(_ => _ != null)
                    .ToArray();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EntityObject()
        {
            Children = new EntityCollection(this);
            ModelGroup = new Model3DGroup();

            // アニメーション用の行列はここで新たに指定しないと
            // 勝手にFreezeされてしまうようです。
            RotateZTransform = new RotateTransform3D();

            lock (instanceList)
            {
                instanceList.Add(new WeakReference(this));
            }
        }
    }
}
