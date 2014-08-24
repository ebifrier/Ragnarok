using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

using Ragnarok;
using Ragnarok.Shogi;
using Ragnarok.Utility;
using Ragnarok.ObjectModel;
using Ragnarok.Presentation.Utility;
using Ragnarok.Presentation.Extra.Entity;

namespace Ragnarok.Presentation.Shogi.View
{
    using ViewModel;

    /// <summary>
    /// 将棋の盤面を扱う3D用のエレメントです。
    /// </summary>
    public partial class ShogiUIElement3D : UIElement3D
    {
        /// <summary>
        /// 各マスに対する上下左右の余白の比です。
        /// </summary>
        public const double BanBorderRate = 0.4;
        /// <summary>
        /// 盤エフェクトのＺ座標です。
        /// </summary>
        public const double BanEffectZ = -10.0;
        /// <summary>
        /// 駒のＺ座標です。
        /// </summary>
        public const double PieceZ = -11.0;
        /// <summary>
        /// 移動中の駒のＺ座標です。
        /// </summary>
        public const double MovingPieceZ = -12.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PreEffectZ = -13.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double EffectZ = -14.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PostEffectZ = -15.0;

        private static readonly Brush DefaultBanBrush =
            new ImageBrush(new BitmapImage(
                ImageUtil.GetImageUri(BanImageType.Default)))
                    .Apply(_ => _.Opacity = 0.9)
                    .Apply(_ => _.Freeze());

        private static readonly Brush DefaultPieceBoxBrush =
            new ImageBrush(new BitmapImage(
                ImageUtil.GetImageUri(KomadaiImageType.Komadai1)))
                    .Apply(_ => _.Opacity = 0.9)
                    .Apply(_ => _.Freeze());

        private static readonly BitmapImage DefaultPieceImage =
            new BitmapImage(ImageUtil.GetImageUri(KomaImageType.Kinki))
                .Apply(_ => _.Freeze());

        private static readonly Brush DefaultAutoPlayBrush =
            new SolidColorBrush(Color.FromArgb(96, 0, 24, 86))
            {
                Opacity = 0.0
            }
            .Apply(_ => _.Freeze());

        private Model3DGroup banEffectGroup;
        private Model3DGroup effectGroup;

        private readonly ReentrancyLock renderLock = new ReentrancyLock();
        private EntityObject banEffectObjectRoot = new EntityObject();
        private EntityObject effectObjectRoot = new EntityObject();
        private AutoPlay autoPlay;

        #region イベント
        /// <summary>
        /// 各フレームごとに呼ばれるイベントです。
        /// </summary>
        public static readonly RoutedEvent EnterFrameEvent =
            EventManager.RegisterRoutedEvent(
                "EnterFrameEvent",
                RoutingStrategy.Bubble,
                typeof(EventHandler<RoutedEventArgs>),
                typeof(ShogiUIElement3D));

        /// <summary>
        /// 各フレームごとに呼ばれるイベントを追加または削除します。
        /// </summary>
        public event EventHandler<RoutedEventArgs> EnterFrame
        {
            add { AddHandler(EnterFrameEvent, value); }
            remove { RemoveHandler(EnterFrameEvent, value); }
        }

        /// <summary>
        /// 指し手が進む直前に呼ばれるイベントです。
        /// </summary>
        public static readonly RoutedEvent BoardPieceChangingEvent =
            EventManager.RegisterRoutedEvent(
                "BoardPieceChangingEvent",
                RoutingStrategy.Bubble,
                typeof(EventHandler<BoardPieceRoutedEventArgs>),
                typeof(ShogiUIElement3D));

        /// <summary>
        /// 指し手が進む直前に呼ばれるイベントを追加または削除します。
        /// </summary>
        public event EventHandler<BoardPieceRoutedEventArgs> BoardPieceChanging
        {
            add { AddHandler(BoardPieceChangingEvent, value); }
            remove { RemoveHandler(BoardPieceChangingEvent, value); }
        }

        /// <summary>
        /// 指し手が進んだ直後に呼ばれるイベントです。
        /// </summary>
        public static readonly RoutedEvent BoardPieceChangedEvent =
            EventManager.RegisterRoutedEvent(
                "BoardPieceChangedEvent",
                RoutingStrategy.Bubble,
                typeof(EventHandler<BoardPieceRoutedEventArgs>),
                typeof(ShogiUIElement3D));

        /// <summary>
        /// 指し手が進んだ直後に呼ばれるイベントを追加または削除します。
        /// </summary>
        public event EventHandler<BoardPieceRoutedEventArgs> BoardPieceChanged
        {
            add { AddHandler(BoardPieceChangedEvent, value); }
            remove { RemoveHandler(BoardPieceChangedEvent, value); }
        }
        #endregion

        #region 基本プロパティ
        /// <summary>
        /// 表示する局面を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register(
                "Board", typeof(Board), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(
                    new Board(),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnBoardChanged, OnCoerceBoard));

        /// <summary>
        /// 表示する局面を取得または設定します。
        /// </summary>
        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        /// <summary>
        /// 局面が変更される直前に呼ばれます。
        /// </summary>
        static object OnCoerceBoard(DependencyObject d, object baseValue)
        {
            var self = (ShogiUIElement3D)d;

            if (self != null)
            {
                // 駒の移動処理を停止しないと、局面の内容がおかしくなったり、
                // 二回クリックしないと駒がつかめないなどの現象が現れます。
                self.EndMove();
            }

            return baseValue;
        }

        /// <summary>
        /// 局面が変更されたときに呼ばれます。
        /// </summary>
        static void OnBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ShogiUIElement3D)d;

            if (self != null)
            {
                var oldBoard = (Board)e.OldValue;
                var newBoard = (Board)e.NewValue;

                self.ClosePromoteDialog();
                self.StopAutoPlay();

                if (oldBoard != null)
                {
                    oldBoard.BoardChanged -= self.OnBoardPieceChanged;
                }

                if (newBoard != null)
                {
                    newBoard.BoardChanged += self.OnBoardPieceChanged;
                }

                self.SyncBoard(false, true);
            }
        }

        /// <summary>
        /// 盤の手前側の先後を取得または設定します。
        /// </summary>
        public static readonly DependencyProperty ViewSideProperty =
            DependencyProperty.Register(
                "ViewSide", typeof(BWType), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(BWType.Black,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnViewSideChanged));

        /// <summary>
        /// 番の手前側の先後を取得または設定します。
        /// </summary>
        public BWType ViewSide
        {
            get { return (BWType)GetValue(ViewSideProperty); }
            set { SetValue(ViewSideProperty, value); }
        }

        /// <summary>
        /// 盤面の回転時に呼ばれます。
        /// </summary>
        static void OnViewSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ShogiUIElement3D)d;

            if (self != null)
            {
                // 駒の配置とエフェクトを初期化します。
                self.SyncBoard(true, true);
            }
        }

        /// <summary>
        /// 編集モードを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register(
                "EditMode", typeof(EditMode), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(EditMode.Normal,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnEditModeChanged));

        /// <summary>
        /// 編集モードを取得または設定します。
        /// </summary>
        public EditMode EditMode
        {
            get { return (EditMode)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        static void OnEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ShogiUIElement3D)d;

            if (self != null)
            {
                self.UpdateEditMode();

                WPFUtil.InvalidateCommand();
            }
        }

        private void UpdateEditMode()
        {
            EndMove();
            UpdateIsPieceBoxVisible();
            UpdateIsLeaveTimeVisibleInternal();
            UpdatePieceBoxBrush();

            SyncCapturedPieceObject(true);
        }

        /// <summary>
        /// 将棋盤の状態を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoPlayStateProperty =
            DependencyProperty.Register(
                "AutoPlayState", typeof(AutoPlayState), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(AutoPlayState.None));

        /// <summary>
        /// 将棋盤の状態を取得または設定します。
        /// </summary>
        public AutoPlayState AutoPlayState
        {
            get { return (AutoPlayState)GetValue(AutoPlayStateProperty); }
            private set { SetValue(AutoPlayStateProperty, value); }
        }

        /// <summary>
        /// 先手側の対局者名を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BlackPlayerNameProperty =
            DependencyProperty.Register(
                "BlackPlayerName", typeof(string), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata("▲先手",
                    OnPlayerNameChanged, CoerceBlackPlayerName));

        /// <summary>
        /// 先手側の対局者名を取得または設定します。
        /// </summary>
        public string BlackPlayerName
        {
            get { return (string)GetValue(BlackPlayerNameProperty); }
            set { SetValue(BlackPlayerNameProperty, value); }
        }

        static void OnPlayerNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        static object CoerceBlackPlayerName(DependencyObject d, object oldValue)
        {
            var name = (oldValue ?? string.Empty).ToString();

            if (string.IsNullOrEmpty(name) || name.StartsWith("▲"))
            {
                return name;
            }

            return ("▲" + name);
        }

        /// <summary>
        /// 後手側の対局者名を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty WhitePlayerNameProperty =
            DependencyProperty.Register(
                "WhitePlayerName", typeof(string), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata("△後手",
                    OnPlayerNameChanged, CoerceWhitePlayerName));

        /// <summary>
        /// 後手側の対局者名を取得または設定します。
        /// </summary>
        public string WhitePlayerName
        {
            get { return (string)GetValue(WhitePlayerNameProperty); }
            set { SetValue(WhitePlayerNameProperty, value); }
        }

        static object CoerceWhitePlayerName(DependencyObject d, object oldValue)
        {
            var name = (oldValue ?? string.Empty).ToString();

            if (string.IsNullOrEmpty(name) || name.StartsWith("△"))
            {
                return name;
            }

            return ("△" + name);
        }
        #endregion

        #region 見た目のプロパティ
        /// <summary>
        /// 盤画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BanBrushProperty =
            DependencyProperty.Register(
                "BanBrush", typeof(Brush), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(DefaultBanBrush));

        /// <summary>
        /// 盤画像を取得または設定します。
        /// </summary>
        public Brush BanBrush
        {
            get { return (Brush)GetValue(BanBrushProperty); }
            set { SetValue(BanBrushProperty, value); }
        }

        /// <summary>
        /// 駒台画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty CapturedPieceBoxBrushProperty =
            DependencyProperty.Register(
                "CapturedPieceBoxBrush", typeof(Brush), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(DefaultPieceBoxBrush));

        /// <summary>
        /// 駒台画像を取得または設定します。
        /// </summary>
        public Brush CapturedPieceBoxBrush
        {
            get { return (Brush)GetValue(CapturedPieceBoxBrushProperty); }
            set { SetValue(CapturedPieceBoxBrushProperty, value); }
        }

        /// <summary>
        /// 駒台画像が変更されたときに呼ばれます。
        /// </summary>
        static void OnCapturedPieceBoxBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ShogiUIElement3D)d;

            if (self != null)
            {
                self.UpdatePieceBoxBrush();
            }
        }

        /// <summary>
        /// 駒画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty PieceImageProperty =
            DependencyProperty.Register(
                "PieceImage", typeof(BitmapSource), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(DefaultPieceImage,
                    OnPieceImageChanged));

        /// <summary>
        /// 駒画像を取得または設定します。
        /// </summary>
        public BitmapSource PieceImage
        {
            get { return (BitmapSource)GetValue(PieceImageProperty); }
            set { SetValue(PieceImageProperty, value); }
        }

        /// <summary>
        /// 駒画像が変更されたときに呼ばれます。
        /// </summary>
        static void OnPieceImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ShogiUIElement3D)d;

            if (self != null)
            {
                self.SyncBoard(true, false);
            }
        }

        /// <summary>
        /// 駒箱画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty PieceBoxBrushProperty =
            DependencyProperty.Register(
                "PieceBoxBrush", typeof(Brush), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 駒箱画像を取得または設定します。
        /// </summary>
        public Brush PieceBoxBrush
        {
            get { return (Brush)GetValue(PieceBoxBrushProperty); }
            private set { SetValue(PieceBoxBrushProperty, value); }
        }

        /// <summary>
        /// 駒箱は局面編集モードでは表示させます。
        /// </summary>
        private void UpdatePieceBoxBrush()
        {
            PieceBoxBrush = (
                EditMode == EditMode.Editing ?
                CapturedPieceBoxBrush :
                null);
        }

        /// <summary>
        /// 駒箱を表示するかどうかを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsPieceBoxVisibleProperty =
            DependencyProperty.Register(
                "IsPieceBoxVisible", typeof(bool), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// 駒箱を表示するかどうかを取得します。
        /// </summary>
        public bool IsPieceBoxVisible
        {
            get { return (bool)GetValue(IsPieceBoxVisibleProperty); }
            private set { SetValue(IsPieceBoxVisibleProperty, value); }
        }

        /// <summary>
        /// 駒箱は局面編集モードでは表示させます。
        /// </summary>
        private void UpdateIsPieceBoxVisible()
        {
            IsPieceBoxVisible = (EditMode == EditMode.Editing);
        }

        /// <summary>
        /// 自動再生時のブラシを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoPlayBrushProperty =
            DependencyProperty.Register(
                "AutoPlayBrush", typeof(Brush), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(DefaultAutoPlayBrush));

        /// <summary>
        /// 自動再生時のブラシを取得または設定します。
        /// </summary>
        public Brush AutoPlayBrush
        {
            get { return (Brush)GetValue(AutoPlayBrushProperty); }
            set { SetValue(AutoPlayBrushProperty, value); }
        }

        /// <summary>
        /// 連続して手を戻す／進める時の一手を指す時間間隔を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoPlayIntervalProperty =
            DependencyProperty.Register(
                "AutoPlayInterval", typeof(TimeSpan), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(TimeSpan.FromSeconds(1)));

        /// <summary>
        /// 連続して手を戻す／進める時の一手を指す時間間隔を取得または設定します。
        /// </summary>
        public TimeSpan AutoPlayInterval
        {
            get { return (TimeSpan)GetValue(AutoPlayIntervalProperty); }
            set { SetValue(AutoPlayIntervalProperty, value); }
        }
        #endregion

        #region 時間系プロパティ
        /// <summary>
        /// 残り時間の表示を実際に行うかどうかを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsLeaveTimeVisibleInternalProperty =
            DependencyProperty.Register(
                "IsLeaveTimeVisibleInternal", typeof(bool), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 残り時間の表示を実際に行うかどうかを取得または設定します。
        /// </summary>
        public bool IsLeaveTimeVisibleInternal
        {
            get { return (bool)GetValue(IsLeaveTimeVisibleInternalProperty); }
            private set { SetValue(IsLeaveTimeVisibleInternalProperty, value); }
        }

        /// <summary>
        /// 残り時間は局面編集時は必ず非表示にします。(でないと駒箱とかぶります)
        /// </summary>
        private void UpdateIsLeaveTimeVisibleInternal()
        {
            IsLeaveTimeVisibleInternal = IsLeaveTimeVisible &&
                                         (EditMode != EditMode.Editing);
        }

        /// <summary>
        /// 残り時間の表示を行うかどうかを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsLeaveTimeVisibleProperty =
            DependencyProperty.Register(
                "IsLeaveTimeVisible", typeof(bool), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(true,
                    OnIsLeaveTimeVisibleChanged));

        /// <summary>
        /// 残り時間の表示を行うかどうかを取得または設定します。
        /// </summary>
        public bool IsLeaveTimeVisible
        {
            get { return (bool)GetValue(IsLeaveTimeVisibleProperty); }
            set { SetValue(IsLeaveTimeVisibleProperty, value); }
        }

        /// <summary>
        /// 残り時間の表示が変更されたときに呼ばれます。
        /// </summary>
        static void OnIsLeaveTimeVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ShogiUIElement3D)d;

            if (self != null)
            {
                self.UpdateIsLeaveTimeVisibleInternal();
            }
        }

        /// <summary>
        /// 先手の残り時間を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BlackLeaveTimeProperty =
            DependencyProperty.Register(
                "BlackLeaveTime", typeof(TimeSpan), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// 先手の残り時間を取得または設定します。
        /// </summary>
        public TimeSpan BlackLeaveTime
        {
            get { return (TimeSpan)GetValue(BlackLeaveTimeProperty); }
            set { SetValue(BlackLeaveTimeProperty, value); }
        }

        /// <summary>
        /// 後手の残り時間を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty WhiteLeaveTimeProperty =
            DependencyProperty.Register(
                "WhiteLeaveTime", typeof(TimeSpan), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// 後手の残り時間を取得または設定します。
        /// </summary>
        public TimeSpan WhiteLeaveTime
        {
            get { return (TimeSpan)GetValue(WhiteLeaveTimeProperty); }
            set { SetValue(WhiteLeaveTimeProperty, value); }
        }
        #endregion

        #region その他のプロパティ
        /// <summary>
        /// エフェクト管理オブジェクトを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty EffectManagerProperty =
            DependencyProperty.Register(
                "EffectManager", typeof(IEffectManager), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(
                    default(IEffectManager),
                    OnEffectManagerChanged));

        /// <summary>
        /// エフェクト管理オブジェクトを取得または設定します。
        /// </summary>
        public IEffectManager EffectManager
        {
            get { return (IEffectManager)GetValue(EffectManagerProperty); }
            set { SetValue(EffectManagerProperty, value); }
        }

        /// <summary>
        /// エフェクト管理オブジェクトが変更されたときに呼ばれます。
        /// </summary>
        static void OnEffectManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ShogiUIElement3D)d;

            if (self != null)
            {
                var oldValue = e.OldValue as IEffectManager;
                var newValue = e.NewValue as IEffectManager;

                if (oldValue != null)
                {
                    oldValue.Clear();
                    oldValue.Container = null;
                }

                if (newValue != null)
                {
                    newValue.Container = self;
                    newValue.Clear();
                }
            }
        }

        /// <summary>
        /// マスが含まれる領域の矩形を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BanBoundsProperty =
            DependencyProperty.Register(
                "BanBounds", typeof(Rect), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(default(Rect)));

        /// <summary>
        /// マスが含まれる部分の領域を取得します。
        /// </summary>
        public Rect BanBounds
        {
            get { return (Rect)GetValue(BanBoundsProperty); }
            private set { SetValue(BanBoundsProperty, value); }
        }

        /// <summary>
        /// 各マスのサイズを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register(
                "CellSize", typeof(Size), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(
                    default(Size),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 各マスのサイズを取得または設定します。
        /// </summary>
        public Size CellSize
        {
            get { return (Size)GetValue(CellSizeProperty); }
            private set { SetValue(CellSizeProperty, value); }
        }

        /// <summary>
        /// 駒が掴まれているなどするかどうかを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty InManipulatingProperty =
            DependencyProperty.Register(
                "InManipulating", typeof(bool), typeof(ShogiUIElement3D),
                new FrameworkPropertyMetadata(default(bool)));

        /// <summary>
        /// 駒が掴まれているなどするかどうかを取得します。
        /// </summary>
        public bool InManipulating
        {
            get { return (bool)GetValue(InManipulatingProperty); }
            private set { SetValue(InManipulatingProperty, value); }
        }
        #endregion

        #region 盤とビューとの同期など
        /// <summary>
        /// 盤面の駒が移動したときに呼ばれます。
        /// </summary>
        private void OnBoardPieceChanged(object sender, BoardChangedEventArgs e)
        {
            var move = e.Move;
            if ((object)move == null || !move.Validate())
            {
                return;
            }

            // 短縮形
            var np = move.DstSquare;
            var op = move.SrcSquare;

            // 一応
            EndMove();

            // リドゥ・アンドゥ両方に対応します。
            if (np != null)
            {
                RemovePieceObject(np);
            }
            if (op != null)
            {
                RemovePieceObject(op);
            }

            // 駒打ち
            if (move.DropPieceType != PieceType.None)
            {
                SyncCapturedPieceCount(
                    move.DropPieceType,
                    move.BWType);
            }

            // 駒を取った場合
            if (move.TookPiece != null)
            {
                SyncCapturedPieceCount(
                    move.TookPiece.PieceType,
                    move.BWType);

                // 取った駒を元の位置に戻します。
                if (e.IsUndo)
                {
                    AddPieceObject(new PieceObject(this, Board[np], np));
                }
            }

            // リドゥ時は新しい場所に、アンドゥ時は昔の場所に駒をおきます。
            // アンドゥで駒打ちの場合、追加される駒はありません。
            var square = (e.IsUndo ? move.SrcSquare : move.DstSquare);
            if (square != null)
            {
                AddPieceObject(new PieceObject(this, Board[square], square));
            }

            // 指し手が進んだときのエフェクトを追加します。
            if (EffectManager != null)
            {
                EffectManager.Moved(move, e.IsUndo);
            }
        }

        /// <summary>
        /// 今の局面と画面の表示を合わせます。
        /// </summary>
        private void SyncBoard(bool forceUpdate, bool initEffect)
        {
            // 今の局面と画面の表示を合わせます。
            SyncCapturedPieceObject(forceUpdate);
            SyncBoardPiece(forceUpdate);

            if (initEffect && EffectManager != null)
            {
                var board = Board;
                var bwType = (board != null ? board.Turn : BWType.Black);

                EffectManager.InitEffect(bwType);
            }

            // 駒などをとりあえず表示させます。
            Render(TimeSpan.FromMilliseconds(1));
        }
        #endregion

        #region エフェクトなどの追加・削除
        /// <summary>
        /// 盤のエフェクトを追加します。
        /// </summary>
        public void AddBanEffect(EntityObject effect)
        {
            if (effect == null)
            {
                return;
            }

            if (this.banEffectObjectRoot != null)
            {
                this.banEffectObjectRoot.Children.Add(effect);
            }
        }

        /// <summary>
        /// 盤のエフェクトを削除します。
        /// </summary>
        public void RemoveBanEffect(EntityObject effect)
        {
            if (effect == null)
            {
                return;
            }

            if (this.banEffectObjectRoot != null)
            {
                this.banEffectObjectRoot.Children.Remove(effect);
            }
        }

        /// <summary>
        /// エフェクトを追加します。
        /// </summary>
        public void AddEffect(EntityObject effect)
        {
            if (effect == null)
            {
                return;
            }

            if (this.effectObjectRoot != null)
            {
                this.effectObjectRoot.Children.Add(effect);
            }
        }

        /// <summary>
        /// エフェクトを削除します。
        /// </summary>
        public void RemoveEffect(EntityObject effect)
        {
            if (effect == null)
            {
                return;
            }

            if (this.effectObjectRoot != null)
            {
                this.effectObjectRoot.Children.Remove(effect);
            }
        }
        #endregion

        #region 自動再生
        /// <summary>
        /// 自動再生を開始します。
        /// </summary>
        public void StartAutoPlay(AutoPlay autoPlay)
        {
            if (autoPlay == null)
            {
                return;
            }

            if (AutoPlayState == AutoPlayState.Playing)
            {
                return;
            }

            // コントロールを事前に設定する必要があります。
            var brush =
                (AutoPlayBrush != null && AutoPlayBrush.IsFrozen ?
                 AutoPlayBrush.Clone() :
                 AutoPlayBrush);
            AutoPlayBrush = brush;
            autoPlay.Background = brush;

            // ここで局面を変更します。
            // 局面変更時は自動再生を自動で止めるようになっているので、
            // autoPlayフィールドを変更した後に局面を変えると、
            // すぐに止まってしまいます。
            Board = autoPlay.Board;

            this.autoPlay = autoPlay;
            AutoPlayState = AutoPlayState.Playing;
        }

        /// <summary>
        /// 自動再生を終了します。
        /// </summary>
        public void StopAutoPlay()
        {
            AutoPlay_Ended();
        }

        private void AutoPlay_Ended()
        {
            if (this.autoPlay == null)
            {
                return;
            }

            var autoPlay = this.autoPlay;
            this.autoPlay = null;
            AutoPlayState = AutoPlayState.None;

            // Boardが変更されるとAutoPlayはすべてクリアされます。
            // Stop中にBoardが変更されると少し面倒なことになるため、
            // Stopメソッドはすべての状態が落ち着いた後に呼びます。
            autoPlay.Stop();
        }
        #endregion

        /// <summary>
        /// 各フレームごとに呼ばれます。
        /// </summary>
        public void Render(TimeSpan elapsedTime)
        {
            using (var locker = renderLock.Lock())
            {
                if (locker == null) return;

                var e = new RoutedEventArgs(EnterFrameEvent);
                RaiseEvent(e);

                if (this.banEffectObjectRoot != null)
                {
                    this.banEffectObjectRoot.DoEnterFrame(elapsedTime);
                }

                if (this.effectObjectRoot != null)
                {
                    this.effectObjectRoot.DoEnterFrame(elapsedTime);
                }

                if (this.pieceObjectList != null)
                {
                    // フレーム更新中にオブジェクトが変更されることがあります。
                    this.pieceObjectList.ToArray()
                        .ForEach(_ => _.DoEnterFrame(elapsedTime));
                }

                // 先手・後手盤の駒台上の駒を更新します。
                foreach (var capturedPieceList in this.capturedPieceObjectList)
                {
                    if (capturedPieceList != null)
                    {
                        capturedPieceList.ToArray()
                            .ForEach(_ => _.DoEnterFrame(elapsedTime));
                    }
                }

                if (this.autoPlay != null)
                {
                    if (!this.autoPlay.Update(elapsedTime))
                    {
                        StopAutoPlay();
                    }
                }
            }
        }

        /// <summary>
        /// コマンドのバインディングを行います。
        /// </summary>
        public void InitializeBindings(UIElement element)
        {
            ViewModel.ShogiCommands.Binding(this, element.CommandBindings);
            ViewModel.ShogiCommands.Binding(element.InputBindings);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShogiUIElement3D()
        {
            Initialize();

            BanBrush = DefaultBanBrush.Clone();
            CapturedPieceBoxBrush = DefaultPieceBoxBrush.Clone();
            PieceImage = DefaultPieceImage.Clone();

            Board = new Board();
        }

        /// <summary>
        /// リソースファイルを読み込み、クラスを初期化します。
        /// </summary>
        private void Initialize()
        {
            try
            {
                this.capturedPieceContainer = new Model3DGroup();
                this.pieceContainer = new Model3DGroup();
                this.banEffectGroup = new Model3DGroup();
                this.effectGroup = new Model3DGroup();

                if (WPFUtil.IsInDesignMode)
                {
                    return;
                }

                var dic = LoadXaml();

                // DataContext相当のものを設定
                var proxy = (BindingProxy)dic["proxy"];
                proxy.Data = this;

                // 駒サイズなどの設定
                var banGeometry = (Model3D)dic["banGeometry"];
                var banTitleGeometry = (Model3D)dic["banTitleGeometry"];
                var komadai0Geometry = (Model3D)dic["komadai0Geometry"];
                var komadai1Geometry = (Model3D)dic["komadai1Geometry"];
                var komaboxGeometry = (Model3D)dic["komaboxGeometry"];
                var autoPlayGeometry = (Model3D)dic["autoPlayGeometry"];

                // モデルの設定
                var group = new Model3DGroup();
                group.Children.Add(banGeometry); // 盤
                group.Children.Add(banTitleGeometry); // 盤
                group.Children.Add(komadai0Geometry); // →↓の駒台
                group.Children.Add(komadai1Geometry); // ←↑の駒台
                group.Children.Add(komaboxGeometry);  // ←↓の駒箱
                group.Children.Add(this.banEffectGroup); // 盤のエフェクト
                group.Children.Add(this.capturedPieceContainer); // 取った駒
                group.Children.Add(this.pieceContainer); // 駒
                group.Children.Add(this.effectGroup); // 駒のエフェクト
                group.Children.Add(autoPlayGeometry);
                Visual3DModel = group;

                InitializeBounds(
                    banGeometry.Bounds, komaboxGeometry.Bounds,
                    komadai0Geometry.Bounds, komadai1Geometry.Bounds);

                // エフェクトなどを初期化します。
                this.banEffectObjectRoot.Duration = TimeSpan.MaxValue;
                this.banEffectGroup.Children.Add(this.banEffectObjectRoot.ModelGroup);

                this.effectObjectRoot.Duration = TimeSpan.MaxValue;
                this.effectGroup.Children.Add(this.effectObjectRoot.ModelGroup);

                // 今の局面と画面の表示を合わせます。
                SyncBoard(true, true);
            }
            catch (Exception ex)
            {
                // このエラーはどうにもならない。
                DialogUtil.ShowError(ex,
                    "将棋盤用のリソースファイルの読み込みに失敗しました。");

                throw ex;
            }
        }

        /// <summary>
        /// リソース用のxamlファイルを読み込みます。
        /// </summary>
        private ResourceDictionary LoadXaml()
        {
            var type = GetType();

            var fileUri = new Uri(
                string.Format(
                    "pack://application:,,,/{0};component/View/{1}.xaml",
                    type.Assembly.GetName().Name,
                    type.Name),
                UriKind.Absolute);
            var info = Application.GetResourceStream(fileUri);

            var context = new ParserContext();
            var resource = (ResourceDictionary)
                XamlReader.Load(info.Stream, context);

            return resource;
        }

        /// <summary>
        /// コントロールをアンロードします。
        /// </summary>
        /// <remarks>
        /// Unloededイベントがないので、手動でアンロードします。
        /// </remarks>
        public void Unload()
        {
            EndMove();
            StopAutoPlay();

            // エフェクトマネージャへの参照と、マネージャが持つ
            // このオブジェクトへの参照を初期化します。
            EffectManager = null;

            // Boardには駒が変化したときのハンドラを設定しているため
            // 最後に必ずそのハンドラを削除する必要があります。
            // しかしながら、ここで値をnullに設定してしまうと、
            // Board依存プロパティに設定されたデータの方もnullクリア
            // されてしまうため、ただ単にイベントを外すだけにします。
            if (Board != null)
            {
                Board.BoardChanged -= OnBoardPieceChanged;
            }

            if (this.banEffectObjectRoot != null)
            {
                this.banEffectObjectRoot.Terminate();
                this.banEffectObjectRoot = null;
            }

            if (this.effectObjectRoot != null)
            {
                this.effectObjectRoot.Terminate();
                this.effectObjectRoot = null;
            }

            if (this.pieceObjectList != null)
            {
                this.pieceObjectList.ToArray()
                    .ForEach(_ => _.Terminate());
                this.pieceObjectList.Clear();
            }

            foreach (var capturedPieceList in this.capturedPieceObjectList)
            {
                if (capturedPieceList != null)
                {
                    capturedPieceList.ToArray()
                        .ForEach(_ => _.Terminate());
                    capturedPieceList.Clear();
                }
            }
        }
    }
}
