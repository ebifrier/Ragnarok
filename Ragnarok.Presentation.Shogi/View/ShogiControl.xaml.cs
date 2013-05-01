using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Ragnarok;
using Ragnarok.Shogi;
using Ragnarok.Utility;
using Ragnarok.ObjectModel;
using Ragnarok.Presentation.VisualObject;

namespace Ragnarok.Presentation.Shogi.View
{
    using Effects;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ShogiControl : UserControl
    {
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
                    .Apply(_ => _.Opacity = 0.9);

        private static readonly Brush DefaultPieceBoxBrush =
            new ImageBrush(new BitmapImage(
                ImageUtil.GetImageUri(KomadaiImageType.Komadai1)))
                    .Apply(_ => _.Opacity = 0.9);

        private static readonly BitmapImage DefaultPieceImage =
            new BitmapImage(ImageUtil.GetImageUri(KomaImageType.Kinki));

        private static readonly Brush DefaultAutoPlayBrush =
            new SolidColorBrush(Color.FromArgb(96, 0, 24, 86))
            {
                Opacity = 0.0
            };

        private readonly NotifyCollection<PieceObject> pieceObjectList =
            new NotifyCollection<PieceObject>();
        private readonly NotifyCollection<PieceObject>[] capturedPieceObjectList =
            new NotifyCollection<PieceObject>[2];
        private readonly Rect[] capturedPieceBoxBounds = new Rect[2];
        private EntityObject banEffectObjectRoot = new EntityObject();
        private EntityObject effectObjectRoot = new EntityObject();
        private PieceObject movingPiece;
        private AutoPlay autoPlay;
        private Window promoteDialog;
        private ReentrancyLock renderLock = new ReentrancyLock();

        #region イベント
        /// <summary>
        /// 各フレームごとに呼ばれるイベントです。
        /// </summary>
        public static readonly RoutedEvent EnterFrameEvent =
            EventManager.RegisterRoutedEvent(
                "EnterFrameEvent",
                RoutingStrategy.Bubble,
                typeof(EventHandler<RoutedEventArgs>),
                typeof(ShogiControl));

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
                typeof(EventHandler<RoutedEventArgs>),
                typeof(ShogiControl));

        /// <summary>
        /// 指し手が進む直前に呼ばれるイベントを追加または削除します。
        /// </summary>
        public event EventHandler<RoutedEventArgs> BoardPieceChanging
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
                typeof(EventHandler<RoutedEventArgs>),
                typeof(ShogiControl));        

        /// <summary>
        /// 指し手が進んだ直後に呼ばれるイベントを追加または削除します。
        /// </summary>
        public event EventHandler<RoutedEventArgs> BoardPieceChanged
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
                "Board", typeof(Board), typeof(ShogiControl),
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
            var self = (ShogiControl)d;

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
            var self = (ShogiControl)d;

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

                self.SyncBoard(true);
            }
        }

        /// <summary>
        /// 盤の手前側の先後を取得または設定します。
        /// </summary>
        public static readonly DependencyProperty ViewSideProperty =
            DependencyProperty.Register(
                "ViewSide", typeof(BWType), typeof(ShogiControl),
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
            var self = (ShogiControl)d;

            if (self != null)
            {
                // 駒の配置とエフェクトを初期化します。
                self.SyncBoard(true);
            }
        }

        /// <summary>
        /// 編集モードを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register(
                "EditMode", typeof(EditMode), typeof(ShogiControl),
                new FrameworkPropertyMetadata(EditMode.Normal,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (_, __) => WPFUtil.InvalidateCommand()));

        /// <summary>
        /// 編集モードを取得または設定します。
        /// </summary>
        public EditMode EditMode
        {
            get { return (EditMode)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        /// <summary>
        /// 先手の残り時間を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BlackLeaveTimeProperty =
            DependencyProperty.Register(
                "BlackLeaveTime", typeof(TimeSpan), typeof(ShogiControl),
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
                "WhiteLeaveTime", typeof(TimeSpan), typeof(ShogiControl),
                new FrameworkPropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// 後手の残り時間を取得または設定します。
        /// </summary>
        public TimeSpan WhiteLeaveTime
        {
            get { return (TimeSpan)GetValue(WhiteLeaveTimeProperty); }
            set { SetValue(WhiteLeaveTimeProperty, value); }
        }

        /// <summary>
        /// 将棋盤の状態を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoPlayStateProperty =
            DependencyProperty.Register(
                "AutoPlayState", typeof(AutoPlayState), typeof(ShogiControl),
                new FrameworkPropertyMetadata(AutoPlayState.None,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
                "BlackPlayerName", typeof(string), typeof(ShogiControl),
                new FrameworkPropertyMetadata("先手"));

        /// <summary>
        /// 先手側の対局者名を取得または設定します。
        /// </summary>
        public string BlackPlayerName
        {
            get { return (string)GetValue(BlackPlayerNameProperty); }
            set { SetValue(BlackPlayerNameProperty, value); }
        }

        /// <summary>
        /// 後手側の対局者名を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty WhitePlayerNameProperty =
            DependencyProperty.Register(
                "WhitePlayerName", typeof(string), typeof(ShogiControl),
                new FrameworkPropertyMetadata("後手"));

        /// <summary>
        /// 後手側の対局者名を取得または設定します。
        /// </summary>
        public string WhitePlayerName
        {
            get { return (string)GetValue(WhitePlayerNameProperty); }
            set { SetValue(WhitePlayerNameProperty, value); }
        }
        #endregion

        #region 見た目のプロパティ
        /// <summary>
        /// 盤画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BanBrushProperty =
            DependencyProperty.Register(
                "BanBrush", typeof(Brush), typeof(ShogiControl),
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
        public static readonly DependencyProperty PieceBoxBrushProperty =
            DependencyProperty.Register(
                "PieceBoxBrush", typeof(Brush), typeof(ShogiControl),
                new FrameworkPropertyMetadata(DefaultPieceBoxBrush));

        /// <summary>
        /// 駒台画像を取得または設定します。
        /// </summary>
        public Brush PieceBoxBrush
        {
            get { return (Brush)GetValue(PieceBoxBrushProperty); }
            set { SetValue(PieceBoxBrushProperty, value); }
        }

        /// <summary>
        /// 駒画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty PieceImageProperty =
            DependencyProperty.Register(
                "PieceImage", typeof(BitmapSource), typeof(ShogiControl),
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
            var self = (ShogiControl)d;

            if (self != null)
            {
                self.SyncBoard(false);
            }
        }

        /// <summary>
        /// 自動再生時のブラシを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoPlayBrushProperty =
            DependencyProperty.Register(
                "AutoPlayBrush", typeof(Brush), typeof(ShogiControl),
                new FrameworkPropertyMetadata(DefaultAutoPlayBrush));
        
        /// <summary>
        /// 自動再生時のブラシを取得または設定します。
        /// </summary>
        public Brush AutoPlayBrush
        {
            get { return (Brush)GetValue(AutoPlayBrushProperty); }
            set { SetValue(AutoPlayBrushProperty, value); }
        }
        #endregion

        #region その他のプロパティ
        /// <summary>
        /// エフェクト管理オブジェクトを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty EffectManagerProperty =
            DependencyProperty.Register(
                "EffectManager", typeof(IEffectManager), typeof(ShogiControl),
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
            var self = (ShogiControl)d;

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
                "BanBounds", typeof(Rect), typeof(ShogiControl),
                new UIPropertyMetadata(default(Rect)));

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
                "CellSize", typeof(Size), typeof(ShogiControl),
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
        #endregion

        #region Overrides
        /// <summary>
        /// 盤上の駒の移動を開始します。
        /// </summary>
        void PieceObject_MouseDown(PieceObject pieceObject, MouseButtonEventArgs e)
        {
            if (movingPiece == null)
            {
                BeginMovePiece(pieceObject);
                MovePiece(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 駒台の駒の移動を開始します。
        /// </summary>
        void CapturedPieceObject_MouseDown(PieceObject pieceObject, MouseButtonEventArgs e)
        {
            if (movingPiece == null)
            {
                BeginDropPiece(pieceObject);
                MovePiece(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// マウスの右ボタン押下時に呼ばれます。
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (movingPiece != null)
            {
                // 駒の移動を完了します。

                // マウスがどのセルにいるか調べます。
                var boardPos = e.GetPosition(RootViewport);
                var position = GetCell(boardPos);
                if (position == null)
                {
                    EndMove();
                    return;
                }

                DoMove(position);
            }
        }

        /// <summary>
        /// マウス移動時に呼ばれます。
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            MovePiece(e);
        }

        /// <summary>
        /// マウスの右ボタンが上がったときに呼ばれます。
        /// </summary>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }
        #endregion

        #region 駒の移動など
        /// <summary>
        /// 与えられた座標のセルを取得します。
        /// </summary>
        private Position GetCell(Point pos)
        {
            // とりあえず設定します。
            var file = (int)((pos.X - BanBounds.Left) / CellSize.Width);
            var rank = (int)((pos.Y - BanBounds.Top) / CellSize.Height);

            // 正しい位置にありましぇん。
            var position = new Position(Board.BoardSize - file, rank + 1);
            if (!position.Validate())
            {
                return null;
            }

            /*// 各セルの幅と高さを取得します。
            var gridX = pos.X % this.model.CellWidth;
            var gridY = pos.Y % this.model.CellHeight;

            // セルの端ぎりぎりならそのセルにいると判定しません。
            if (gridX < CellWidth * 0.1 || CellWidth * 0.9 < gridX)
            {
                return null;
            }

            if (gridY < CellHeight * 0.1 || CellHeight * 0.9 < gridY)
            {
                return null;
            }*/

            return (ViewSide == BWType.White ? position.Flip() : position);
        }

        /// <summary>
        /// 駒の移動などを開始できるかどうかを取得します。
        /// </summary>
        private bool CanBeginMove(BWType pieceSide)
        {
            if (this.movingPiece != null)
            {
                return false;
            }

            if (AutoPlayState != AutoPlayState.None)
            {
                return false;
            }

            var teban = (Board != null ? Board.Turn : BWType.None);
            if (EditMode == EditMode.NoEdit ||
                (EditMode == EditMode.Normal && teban != pieceSide))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 駒の移動を開始します。
        /// </summary>
        private void BeginMovePiece(PieceObject pieceObject)
        {
            if (!CanBeginMove(pieceObject.Piece.BWType))
            {
                return;
            }

            this.movingPiece = pieceObject;

            // 描画順を変えます。
            PieceContainer.Children.Remove(pieceObject.Element);
            PieceContainer.Children.Add(pieceObject.Element);

            if (EffectManager != null)
            {
                EffectManager.BeginMove(pieceObject.Position, pieceObject.Piece);
            }
        }

        /// <summary>
        /// 駒打ちの処理を開始します。
        /// </summary>
        private void BeginDropPiece(PieceObject pieceObject)
        {
            var boardPiece = pieceObject.Piece;

            if (!CanBeginMove(boardPiece.BWType))
            {
                return;
            }

            if (Board.GetCapturedPieceCount(boardPiece) <= 0)
            {
                return;
            }

            // 表示用の駒を追加します。
            this.movingPiece = new PieceObject(this, boardPiece);
            AddPieceObject(this.movingPiece, true);

            if (EffectManager != null)
            {
                EffectManager.BeginMove(null, boardPiece);
            }
        }

        /// <summary>
        /// 移動中の駒を動かします。
        /// </summary>
        private void MovePiece(MouseEventArgs e)
        {
            if (this.movingPiece == null)
            {
                return;
            }

            // 駒とマウスの位置の差を求めておきます。
            var mousePos = e.GetPosition(RootViewport);

            this.movingPiece.Coord = WPFUtil.MakeVector3D(mousePos, MovingPieceZ);
        }

        /// <summary>
        /// 移動中の駒を新しい位置に移動します。
        /// </summary>
        /// <remarks>
        /// 指せない指し手の場合は、駒の移動を終了します。
        /// </remarks>
        private void DoMove(Position newPosition)
        {
            if (this.movingPiece == null)
            {
                return;
            }

            // 駒を新しい位置に動かします。
            var position = this.movingPiece.Position;
            var piece = this.movingPiece.Piece;

            var move = (position != null ?
                new BoardMove()
                {
                    OldPosition = position,
                    NewPosition = newPosition,
                    BWType = Board.Turn,
                    ActionType = ActionType.None,
                } :
                new BoardMove()
                {
                    NewPosition = newPosition,
                    BWType = Board.Turn,
                    ActionType = ActionType.Drop,
                    DropPieceType = piece.PieceType,
                });

            var isForcePromote =
                Board.IsPromoteForce(move, piece);
            var canPromote = (
                !piece.IsPromoted &&
                Board.CanPromote(move, piece));

            // 移動中の駒を元に戻し、駒の移動を行います。
            // 失敗したら移動中だった駒は元の位置に戻されます。
            if (!Board.CanMove(move))
            {
                EndMove();
                return;
            }

            // 成り駒じゃない駒が成れる可能性があるときは選択します。
            if (isForcePromote)
            {
                move.ActionType = ActionType.Promote;
            }
            else if (canPromote)
            {
                var isPromote = CheckToPromote(piece.BWType, piece.PieceType);

                move.ActionType = (
                    isPromote ?
                    ActionType.Promote :
                    ActionType.Unpromote);
            }

            EndMove();
            DoMove(move);
        }

        /// <summary>
        /// 成り・不成り選択中に外から局面が設定されることがあります。
        /// その場合には選択ダイアログを強制的にクローズします。
        /// </summary>
        private void ClosePromoteDialog()
        {
            if (this.promoteDialog != null)
            {
                this.promoteDialog.Close();
                this.promoteDialog = null;
            }
        }

        /// <summary>
        /// 成るか不成りかダイアログによる選択を行います。
        /// </summary>
        private bool CheckToPromote(BWType bwType, PieceType pieceType)
        {
            var dialog = DialogUtil.CreateDialog(
                null,
                "成りますか？",
                "成り／不成り",
                MessageBoxButton.YesNo,
                MessageBoxResult.Yes);
            dialog.Topmost = true;

            dialog.Loaded += (sender, e) =>
            {
                var p = WPFUtil.GetMousePosition(dialog);
                var screenPos = dialog.PointToScreen(p);

                dialog.WindowStartupLocation = WindowStartupLocation.Manual;
                dialog.Left = screenPos.X - (dialog.ActualWidth / 2);
                dialog.Top = screenPos.Y + CellSize.Height / 2;
                dialog.AdjustInDisplay();
            };

            try
            {
                ClosePromoteDialog();

                // 成り・不成り選択中に外から局面が設定されることがあります。
                // その場合に備えてダイアログ自体を持っておきます。
                this.promoteDialog = dialog;

                var result = dialog.ShowDialog();
                ClosePromoteDialog();

                return (result != null ? result.Value : false);
            }
            finally
            {
                ClosePromoteDialog();
            }
        }

        /// <summary>
        /// 実際に指し手を進めます。
        /// </summary>
        private void DoMove(BoardMove move)
        {
            if (move == null || !move.Validate())
            {
                return;
            }

            this.RaiseEvent(new RoutedEventArgs(BoardPieceChangingEvent));
            Board.DoMove(move);
            this.RaiseEvent(new RoutedEventArgs(BoardPieceChangedEvent));
        }

        /// <summary>
        /// 駒の移動を終了します。
        /// </summary>
        private void EndMove()
        {
            if (this.movingPiece == null)
            {
                return;
            }

            // 移動中の駒の位置を元に戻します。
            var position = this.movingPiece.Position;
            if (position != null)
            {
                var pos = GetPiecePos(position);
                pos.Z = PieceZ;

                this.movingPiece.Coord = pos;
            }
            else
            {
                // 駒うちの場合は、表示用オブジェクトを新規に作成しています。
                RemovePieceObject(this.movingPiece);
            }

            this.movingPiece = null;
            ClosePromoteDialog();

            if (EffectManager != null)
            {
                EffectManager.EndMove();
            }

            //ReleaseMouseCapture();
        }
        #endregion

        #region 駒オブジェクト
        /// <summary>
        /// 画面表示上の位置を取得します。
        /// </summary>
        public Vector3D GetPiecePos(Position position)
        {
            if ((object)position == null)
            {
                return new Vector3D();
            }

            var relative =
                (ViewSide == BWType.Black
                ? new Point(
                    (Board.BoardSize - position.File) * CellSize.Width,
                    (position.Rank - 1) * CellSize.Height)
                : new Point(
                    (position.File - 1) * CellSize.Width,
                    (Board.BoardSize - position.Rank) * CellSize.Height));

            var leftTop = BanBounds.TopLeft;
            return new Vector3D(
                leftTop.X + relative.X + (CellSize.Width / 2.0),
                leftTop.Y + relative.Y + (CellSize.Height / 2.0),
                PieceZ);
        }

        /// <summary>
        /// 駒台上の駒のデフォルト位置を取得します。
        /// </summary>
        public Vector3D GetCapturedPiecePos(BoardPiece piece)
        {
            return GetCapturedPiecePos(piece.BWType, piece.PieceType);
        }

        /// <summary>
        /// 駒台上の駒のデフォルト位置を取得します。
        /// </summary>
        public Vector3D GetCapturedPiecePos(BWType bwType, PieceType pieceType)
        {
            var index = (bwType == ViewSide ? 0 : 1);
            var bounds = this.capturedPieceBoxBounds[index];

            // ○ 駒位置の計算方法
            // 駒台には横に駒を２つ並べます。また、両端と駒と駒の間には駒の幅/2を
            // スペースとして挿入します。
            // そのため、横位置の計算では
            //   2(両端) + 1(駒間) + 4(駒数*2) = 7
            // を基本区間数として、計算します。
            //
            // また、縦の計算では先手・後手などの文字列を表示するため、
            // 手前側は上部、向かい側は下部に余計な空間を作ります。
            //   3(上下端+α) + 3(駒間) + 8(駒数*4) = 14
            var hw = bounds.Width / 7;
            var hh = bounds.Height / 14;
            var x = ((int)pieceType - 2) % 2;
            var y = ((int)pieceType - 2) / 2;

            if (bwType != BWType.Black)
            {
                x = 1 - x;
                y = 3 - y;
            }

            // 駒の中心位置を返すので、左端の駒の位置は基本区間*2となります。
            // また駒の数を右肩に表示するため、少し左にずらしています。
            return new Vector3D(
                bounds.Left + hw * (x * 3 + 2 - 0.2),
                bounds.Top + hh * (y * 3 + 2 + (1 - index)),
                PieceZ);
        }

        /// <summary>
        /// 指定の位置にある駒を取得します。
        /// </summary>
        private PieceObject GetPieceObject(Position position)
        {
            if (position == null || !position.Validate())
            {
                return null;
            }

            return this.pieceObjectList.FirstOrDefault(
                _ => _.Position == position);
        }

        /// <summary>
        /// 駒の表示用オブジェクトを取得します。
        /// </summary>
        private void AddPieceObject(PieceObject value, bool initPos)
        {
            if (value == null)
            {
                return;
            }

            var handler = new MouseButtonEventHandler(
                (sender, e) => PieceObject_MouseDown(value, e));

            value.Element.MouseDown += handler;
            value.Terminated += (sender, e) =>
                value.Element.MouseDown -= handler;

            // 駒をデフォルト位置まで移動させます。
            if (initPos)
            {
                value.Coord =
                    ( value.Position != null
                    ? GetPiecePos(value.Position)
                    : GetCapturedPiecePos(value.Piece));
            }

            PieceContainer.Children.Add(value.Element);
            this.pieceObjectList.Add(value);
        }

        /// <summary>
        /// 指定の位置にある駒を削除します。
        /// </summary>
        private void RemovePieceObject(PieceObject piece)
        {
            if (piece == null)
            {
                return;
            }

            PieceContainer.Children.Remove(piece.Element);
            this.pieceObjectList.Remove(piece);
        }

        /// <summary>
        /// 指定の位置にある駒を削除します。
        /// </summary>
        private void RemovePieceObject(Position position)
        {
            if (position == null || !position.Validate())
            {
                return;
            }

            // 指定のマスにある駒を探します。
            var index = this.pieceObjectList.FindIndex(
                _ => _.Position == position);
            if (index < 0)
            {
                return;
            }

            RemovePieceObject(this.pieceObjectList[index]);
        }

        /// <summary>
        /// 表示されている駒をすべて削除します。
        /// </summary>
        private void ClearPieceObjects()
        {
            this.pieceObjectList.Clear();
            PieceContainer.Children.Clear();
        }

        /// <summary>
        /// 駒台上の表示用の駒を取得します。
        /// </summary>
        private PieceObject GetCapturedPieceObject(BWType bwType, PieceType pieceType)
        {
            var index = (bwType == BWType.Black ? 0 : 1);
            var capturedPieceList = this.capturedPieceObjectList[index];

            return capturedPieceList[(int)pieceType];
        }

        /// <summary>
        /// 駒台上の駒の表示用オブジェクトを取得します。
        /// </summary>
        private PieceObject CreateCapturedPieceObject(BWType bwType, PieceType pieceType)
        {
            var value = new PieceObject(this, new BoardPiece(bwType, pieceType))
            {
                Count = (Board == null ? 0 : Board.GetCapturedPieceCount(bwType, pieceType)),
                IsAlwaysVisible = false,
            };

            var handler = new MouseButtonEventHandler(
                (sender, e) => CapturedPieceObject_MouseDown(value, e));

            value.Element.MouseDown += handler;
            value.Terminated += (sender, e) =>
                value.Element.MouseDown -= handler;

            // 駒をデフォルト位置まで移動させます。
            value.Coord = GetCapturedPiecePos(bwType, pieceType);

            // 玉などは駒台に表示しません。
            if (pieceType != PieceType.None && pieceType != PieceType.Gyoku)
            {
                CapturedPieceContainer.Children.Add(value.Element);
            }

            return value;
        }

        /// <summary>
        /// 局面と表示で駒台の駒の数を合わせます。
        /// </summary>
        private void SyncCapturedPieceCount(BWType bwType, PieceType pieceType)
        {
            var piece = GetCapturedPieceObject(bwType, pieceType);
            var count = Board.GetCapturedPieceCount(bwType, pieceType);

            if (piece != null)
            {
                piece.Count = count;
            }
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
            var np = move.NewPosition;
            var op = move.OldPosition;

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
                    move.BWType,
                    move.DropPieceType);
            }

            // 駒を取った場合
            if (move.TookPiece != null)
            {
                SyncCapturedPieceCount(
                    move.BWType,
                    move.TookPiece.PieceType);

                // 取った駒を元の位置に戻します。
                if (e.IsUndo)
                {
                    AddPieceObject(
                        new PieceObject(this, Board[np], np),
                        true);
                }
            }

            // リドゥ時は新しい場所に、アンドゥ時は昔の場所に駒をおきます。
            // アンドゥで駒打ちの場合、追加される駒はありません。
            var position = (e.IsUndo ? move.OldPosition : move.NewPosition);
            if (position != null)
            {
                AddPieceObject(new PieceObject(this, Board[position], position), true);
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
        private void SyncBoard(bool initEffect)
        {
            // 今の局面と画面の表示を合わせます。
            SyncCapturedPieceObject();
            SyncBoardPiece();

            if (initEffect && EffectManager != null)
            {
                var board = Board;
                var bwType = (board != null ? board.Turn : BWType.Black);

                EffectManager.InitEffect(bwType);
            }

            // 駒などをとりあえず表示させます。
            Render(TimeSpan.FromMilliseconds(1));
        }

        /// <summary>
        /// 駒台上に表示する描画用の駒を設定します。
        /// </summary>
        /// <remarks>
        /// 盤上の駒と違って、駒台上の駒はプロパティで表示・非表示などを
        /// 切り替えるため、駒の移動ごとに追加・削除をする必要はありません。
        /// </remarks>
        private void SyncCapturedPieceObject()
        {
            if (!IsLoaded || Board == null)
            {
                return;
            }

            var bwTypes = new[]
            {
                BWType.Black,
                BWType.White
            };

            // 先に駒を削除します。
            foreach (var capturedPieceList in this.capturedPieceObjectList)
            {
                if (capturedPieceList != null)
                {
                    capturedPieceList.ToArray()
                        .ForEach(_ => _.Terminate());
                    capturedPieceList.Clear();
                }
            }
            CapturedPieceContainer.Children.Clear();

            // 先手・後手用の駒台にある駒を用意します。
            bwTypes.ForEachWithIndex((bwType, index) =>
            {
                var capturedPieceList = EnumEx.GetValues<PieceType>()
                    .Select(_ => CreateCapturedPieceObject(bwType, _));

                this.capturedPieceObjectList[index] =
                    new NotifyCollection<PieceObject>(capturedPieceList);
            });
        }

        /// <summary>
        /// 今の局面と画面の表示を合わせます。
        /// </summary>
        private void SyncBoardPiece()
        {
            if (!IsLoaded || Board == null)
            {
                return;
            }

            ClearPieceObjects();

            // 各マスに対応する描画用の駒を設定します。
            for (var rank = 1; rank <= Board.BoardSize; ++rank)
            {
                for (var file = 1; file <= Board.BoardSize; ++file)
                {
                    var position = new Position(file, rank);
                    var model = Board[position];

                    if ((object)model != null)
                    {
                        AddPieceObject(new PieceObject(this, model, position), true);
                    }
                }
            }
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
        public ShogiControl()
        {
            InitializeComponent();

            BanBrush = DefaultBanBrush.Clone();
            PieceBoxBrush = DefaultPieceBoxBrush.Clone();
            PieceImage = DefaultPieceImage.Clone();

            Loaded += Control_Loaded;
            Unloaded += Control_Unloaded;

            RootViewport.DataContext = this;
            Board = new Board(true);
        }

        /// <summary>
        /// コントロールの読み込み時に呼ばれます。
        /// </summary>
        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            // 各マスのサイズを設定します。
            var bounds = this.banGeometry.Bounds;

            CellSize = new Size(
                bounds.SizeX / (Board.BoardSize + 0.5),
                bounds.SizeY / (Board.BoardSize + 0.5));
            BanBounds = new Rect(
                bounds.X + CellSize.Width / 4.0,
                bounds.Y + CellSize.Height / 4.0,
                CellSize.Width * Board.BoardSize,
                CellSize.Height * Board.BoardSize);

            capturedPieceBoxBounds[0] = WPFUtil.MakeRectXY(this.komadai0Geometry.Bounds);
            capturedPieceBoxBounds[1] = WPFUtil.MakeRectXY(this.komadai1Geometry.Bounds);

            // エフェクトなどを初期化します。
            this.banEffectObjectRoot.Duration = TimeSpan.MaxValue;
            BanEffectGroup.Children.Add(this.banEffectObjectRoot.ModelGroup);

            this.effectObjectRoot.Duration = TimeSpan.MaxValue;
            EffectGroup.Children.Add(this.effectObjectRoot.ModelGroup);

            // 今の局面と画面の表示を合わせます。
            SyncBoard(true);
        }

        void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            ClosePromoteDialog();
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
