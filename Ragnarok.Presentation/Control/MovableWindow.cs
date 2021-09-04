using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ragnarok.Presentation.Control
{
    using Ragnarok.Presentation.Control.WindowOperation;

    /// <summary>
    /// 枠のない移動可能なウィンドウです。
    /// </summary>
    public partial class MovableWindow : Window
    {
        #region プロパティ
        /// <summary>
        /// ウィンドウを可動状態にするためのコマンドです。
        /// </summary>
        public readonly static ICommand MakeMoveWindow =
            new RoutedUICommand(
                "ウィンドウの移動を可能にします。",
                "MakeMoveWindow",
                typeof(Window));
        /// <summary>
        /// ウィンドウを固定状態にするためのコマンドです。
        /// </summary>
        public readonly static ICommand MakeFixWindow =
            new RoutedUICommand(
                "ウィンドウの移動を停止します。",
                "MakeFixWindow",
                typeof(Window));
        /// <summary>
        /// ウィンドウを最前面に表示するためのコマンドです。
        /// </summary>
        public readonly static ICommand SetTopmost =
            new RoutedUICommand(
                "ウィンドウを最前面に表示します。",
                "SetTopmost",
                typeof(Window));
        /// <summary>
        /// ウィンドウを最前面表示をやめるためのコマンドです。
        /// </summary>
        public readonly static ICommand UnsetTopmost =
            new RoutedUICommand(
                "ウィンドウの最前面表示をやめます。",
                "UnsetTopmost",
                typeof(Window));
        /// <summary>
        /// ウィンドウを最大化するためのコマンドです。
        /// </summary>
        public readonly static ICommand SetMaximized =
            new RoutedUICommand(
                "ウィンドウを最前面に表示します。",
                "SetMaximized",
                typeof(Window));
        /// <summary>
        /// ウィンドウの最大化を解除するためのコマンドです。
        /// </summary>
        public readonly static ICommand UnsetMaximized =
            new RoutedUICommand(
                "ウィンドウの最前面表示をやめます。",
                "UnsetMaximized",
                typeof(Window));

        /// <summary>
        /// ウィンドウが移動可能な時に適用される不透明度を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MovableOpacityProperty =
            DependencyProperty.Register(
                "MovableOpacity", typeof(double), typeof(MovableWindow),
                new FrameworkPropertyMetadata(1.0, OnMovableOpacityChanged));

        private static void OnMovableOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (MovableWindow)d;
            self.UpdateOpacity();
        }

        /// <summary>
        /// ウィンドウが移動可能な時に適用される不透明度を取得または設定します。
        /// </summary>
        public double MovableOpacity
        {
            get { return (double)GetValue(MovableOpacityProperty); }
            set { SetValue(MovableOpacityProperty, value); }
        }

        /// <summary>
        /// ウィンドウが移動可能かどうかを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsMovableProperty =
            DependencyProperty.Register(
                "IsMovable", typeof(bool), typeof(MovableWindow),
                new FrameworkPropertyMetadata(true, OnIsMovableChanged));

        private static void OnIsMovableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (MovableWindow)d;
            self.UpdateOpacity();
        }

        /// <summary>
        /// ウィンドウの不透明度を更新します。
        /// </summary>
        private void UpdateOpacity()
        {
            if (IsMovable)
            {
                Opacity = MovableOpacity;
            }
            else
            {
                Opacity = 1.0;
            }
        }

        /// <summary>
        /// ウィンドウが移動可能かどうかを取得または設定します。
        /// </summary>
        public bool IsMovable
        {
            get { return (bool)GetValue(IsMovableProperty); }
            set { SetValue(IsMovableProperty, value); }
        }

        /// <summary>
        /// サイズを変更するときの枠の長さを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty EdgeLengthProperty =
            DependencyProperty.Register(
                "EdgeLength", typeof(double), typeof(MovableWindow),
                new FrameworkPropertyMetadata(10.0));

        /// <summary>
        /// サイズ変更時の枠の長さを取得または設定します。
        /// </summary>
        public double EdgeLength
        {
            get { return (double)GetValue(EdgeLengthProperty); }
            set { SetValue(EdgeLengthProperty, value); }
        }

        /// <summary>
        /// ウィンドウ固定時のカーソルを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty FixedCursorProperty =
            DependencyProperty.Register(
                "FixedCursor", typeof(Cursor), typeof(MovableWindow),
                new FrameworkPropertyMetadata(Cursors.Arrow));

        /// <summary>
        /// ウィンドウ固定時のカーソルを取得または設定します。
        /// </summary>
        public Cursor FixedCursor
        {
            get { return (Cursor)GetValue(FixedCursorProperty); }
            set { SetValue(FixedCursorProperty, value); }
        }

        /// <summary>
        /// リサイズ時にアスペクト比を保存するかどうかを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty SaveAspectProperty =
            DependencyProperty.Register(
                "SaveAspect", typeof(bool), typeof(MovableWindow),
                new PropertyMetadata(false, (d, e) => ((MovableWindow)d).SaveAspectUpdated()));

        /// <summary>
        /// リサイズ時にアスペクト比を保存するかどうかを取得または設定します。
        /// </summary>
        public bool SaveAspect
        {
            get { return (bool)GetValue(SaveAspectProperty); }
            set { SetValue(SaveAspectProperty, value); }
        }
        #endregion

        #region マウス関連
        /// <summary>
        /// マウスの左ボタン押下時に呼ばれます。
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.operation != null)
            {
                this.operation.End();
                this.operation = null;
                return;
            }

            // 動かせないなら帰ります。
            if (!IsMovable)
            {
                return;
            }

            // 開始可能なオペレーションがあるなら、それを開始します。
            var wp = e.GetPosition(this);
            foreach (var starter in this.starters)
            {
                var op = starter.BeginOperation(wp);

                if (op != null)
                {
                    this.operation = op;
                    break;
                }
            }
        }

        /// <summary>
        /// マウスの移動時に呼ばれます。
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!IsMovable)
            {
                Cursor = FixedCursor;
                return;
            }

            var wp = e.GetPosition(this);
            if (this.operation != null)
            {
                this.operation.Operate(wp);
            }
            else
            {
                // オペレーションがない場合は、
                // 必要に合わせてマウスカーソルを変更します。
                foreach (var starter in this.starters)
                {
                    var cursor = starter.GetCursor(wp);

                    if (cursor != null)
                    {
                        Cursor = cursor;
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// マウスの左ボタン押下後に呼ばれます。
        /// </summary>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (this.operation != null)
            {
                this.operation.End();
                this.operation = null;
            }
        }
        #endregion

        private IWindowOperationStarter[] starters;
        private WindowOperationBase operation;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static MovableWindow()
        {
            WindowStyleProperty.OverrideMetadata(
                typeof(MovableWindow),
                new FrameworkPropertyMetadata(WindowStyle.None));

            AllowsTransparencyProperty.OverrideMetadata(
                typeof(MovableWindow),
                new FrameworkPropertyMetadata(true));

            /*BackgroundProperty.OverrideMetadata(
                typeof(MovableWindow),
                new FrameworkPropertyMetadata(Brushes.Transparent));*/
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MovableWindow()
        {
            InitializeCommand();

            SaveAspectUpdated();
        }

        /// <summary>
        /// startersのリストを更新します。
        /// </summary>
        private void SaveAspectUpdated()
        {
            if (this.operation != null)
            {
                this.operation.End();
                this.operation = null;
            }

            this.starters = (SaveAspect ?
                new IWindowOperationStarter[]
                {
                    new WindowScalerStarter(this, saveAspect:true, withShift:false),
                    new WindowScalerStarter(this, saveAspect:false, withShift:true),
                    new WindowMoverStarter(this),
                } :
                new IWindowOperationStarter[]
                {
                    new WindowScalerStarter(this, saveAspect:false),
                    new WindowMoverStarter(this),
                });
        }

        /// <summary>
        /// コマンドを初期化します。
        /// </summary>
        private void InitializeCommand()
        {
            CommandBindings.Add(
                new CommandBinding(
                    MakeMoveWindow,
                    (_, __) => IsMovable = true,
                    CanExecuteCommand));
            CommandBindings.Add(
                new CommandBinding(
                    MakeFixWindow,
                    (_, __) => IsMovable = false,
                    CanExecuteCommand));
            CommandBindings.Add(
                new CommandBinding(
                    SetTopmost,
                    (_, __) => { Activate(); Topmost = true; },
                    CanExecuteCommand));
            CommandBindings.Add(
                new CommandBinding(
                    UnsetTopmost,
                    (_, __) => Topmost = false,
                    CanExecuteCommand));
            CommandBindings.Add(
                new CommandBinding(
                    SetMaximized,
                    (_, __) => WindowState = WindowState.Maximized,
                    CanExecuteCommand));
            CommandBindings.Add(
                new CommandBinding(
                    UnsetMaximized,
                    (_, __) => WindowState = WindowState.Normal,
                    CanExecuteCommand));
            CommandBindings.Add(
                new CommandBinding(
                    ApplicationCommands.Close,
                    (_, __) => Close(),
                    CanExecuteCommand));
        }

        /// <summary>
        /// コマンドの実行可能性を取得します。
        /// </summary>
        private void CanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == MakeMoveWindow)
            {
                e.CanExecute = !IsMovable;
            }
            else if (e.Command == MakeFixWindow)
            {
                e.CanExecute = IsMovable;
            }
            else if (e.Command == SetTopmost)
            {
                e.CanExecute = !Topmost;
            }
            else if (e.Command == UnsetTopmost)
            {
                e.CanExecute = Topmost;
            }
            else if (e.Command == SetMaximized)
            {
                e.CanExecute = (WindowState != WindowState.Maximized);
            }
            else if (e.Command == UnsetMaximized)
            {
                e.CanExecute = (WindowState != WindowState.Normal);
            }
            else
            {
                e.CanExecute = true;
            }
        }
    }
}
