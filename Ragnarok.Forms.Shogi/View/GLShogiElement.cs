using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

using Ragnarok.ObjectModel;
using Ragnarok.Utility;
using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 将棋の盤面を表示するためのコントロールクラスです。
    /// </summary>
    public partial class GLShogiElement : GLElement
    {
        /// <summary>
        /// 各マスに対する上下左右の余白の比です。
        /// </summary>
        public const double BoardBorderRate = 0.4;

        /// <summary>
        /// デフォルトの局面用イメージを取得します。
        /// </summary>
        public static Bitmap DefaultBoardBitmap
        {
            get { return Properties.Resources.ban; }
        }

        /// <summary>
        /// デフォルトの駒箱用イメージを取得します。
        /// </summary>
        public static Bitmap DefaultPieceBoxBitmap
        {
            get { return Properties.Resources.komadai; }
        }

        /// <summary>
        /// デフォルトの駒用イメージを取得します。
        /// </summary>
        public static Bitmap DefaultPieceBitmap
        {
            get { return Properties.Resources.koma; }
        }

        /*private static readonly Brush DefaultAutoPlayBrush =
            new SolidColorBrush(Color.FromArgb(96, 0, 24, 86))
            {
                Opacity = 0.0
            }
            .Apply(_ => _.Freeze());*/

        private Board board;
        private IEffectManager effectManager;
        private AutoPlay autoPlay;
        private Board oldBoard;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLShogiElement()
        {
            AddPropertyChangedHandler("ViewSide", ViewSideUpdated);
            AddPropertyChangedHandler("EditMode", EditModeUpdated);

            Board = new Board();
            ViewSide = BWType.Black;
            EditMode = EditMode.Normal;
            AutoPlayState = AutoPlayState.None;
            BlackPlayerName = "▲先手";
            WhitePlayerName = "△後手";
            BlackTime = TimeSpan.Zero;
            WhiteTime = TimeSpan.Zero;
            InManipulating = false;

            InitializeDraw();
        }

        /// <summary>
        /// コマンドのバインディングを行います。
        /// </summary>
        public void InitializeBindings()
        {
            //ViewModel.ShogiCommands.Binding(this, element.CommandBindings);
            //ViewModel.ShogiCommands.Binding(element.InputBindings);
        }

        /// <summary>
        /// コントロールをアンロードします。
        /// </summary>
        protected override void OnTerminate()
        {
            EndMove();
            StopAutoPlay();

            // エフェクトマネージャへの参照と、マネージャが持つ
            // このオブジェクトへの参照を初期化します。
            EffectManager = null;

            // 描画関係の終了処理
            TerminateDraw();

            // Boardには駒が変化したときのハンドラを設定しているため
            // 最後に必ずそのハンドラを削除する必要があります。
            // しかしながら、ここで値をnullに設定してしまうと、
            // Board依存プロパティに設定されたデータの方もnullクリア
            // されてしまうため、ただ単にイベントを外すだけにします。
            if (Board != null)
            {
                Board.BoardChanged -= OnBoardPieceChanged;
            }

            base.OnTerminate();
        }

        #region イベント
        /// <summary>
        /// 指し手が進む直前に呼ばれるイベントを追加または削除します。
        /// </summary>
        public event EventHandler<BoardPieceEventArgs> BoardPieceChanging;

        /// <summary>
        /// 指し手が進んだ直後に呼ばれるイベントを追加または削除します。
        /// </summary>
        public event EventHandler<BoardPieceEventArgs> BoardPieceChanged;
        #endregion

        #region 基本プロパティ
        /// <summary>
        /// 表示する局面を取得または設定します。
        /// </summary>
        public Board Board
        {
            get { return this.board; }
            set
            {
                if (this.board != value)
                {
                    EndMove();
                    ClosePromoteDialog();
                    StopAutoPlay();

                    if (this.board != null)
                    {
                        this.board.BoardChanged -= OnBoardPieceChanged;
                        this.board.RemovePropertyChangedHandler("Turn", OnTurnChanged);
                    }

                    this.board = value;

                    if (this.board != null)
                    {
                        this.board.BoardChanged += OnBoardPieceChanged;
                        this.board.AddPropertyChangedHandler("Turn", OnTurnChanged);
                    }

                    InitKomaboxPieceCount();
                    InitEffect();

                    this.RaisePropertyChanged("Board");
                }
            }
        }

        /// <summary>
        /// 番の手前側の先後を取得または設定します。
        /// </summary>
        public BWType ViewSide
        {
            get { return GetValue<BWType>("ViewSide"); }
            set { SetValue("ViewSide", value); }
        }

        /// <summary>
        /// ViewSideの更新時に呼ばれ、エフェクトの初期化などを行います。
        /// </summary>
        private void ViewSideUpdated(object sender, PropertyChangedEventArgs e)
        {
            InitEffect();
        }

        /// <summary>
        /// 編集モードを取得または設定します。
        /// </summary>
        public EditMode EditMode
        {
            get { return GetValue<EditMode>("EditMode"); }
            set { SetValue("EditMode", value); }
        }

        /// <summary>
        /// 編集モード変更時に呼ばれ、駒やコマンドの初期化などを行います。
        /// </summary>
        private void EditModeUpdated(object sender, PropertyChangedEventArgs e)
        {
            EndMove();

            FormsUtil.InvalidateCommand();
        }

        /// <summary>
        /// 自動再生の状態を取得します。
        /// </summary>
        public AutoPlayState AutoPlayState
        {
            get { return GetValue<AutoPlayState>("AutoPlayState"); }
            private set { SetValue("AutoPlayState", value); }
        }

        /// <summary>
        /// 先手側の対局者名を取得または設定します。
        /// </summary>
        public string BlackPlayerName
        {
            get { return GetValue<string>("BlackPlayerName"); }
            set
            {
                var newValue = (
                    (string.IsNullOrEmpty(value) || value.StartsWith("▲")) ?
                    value :
                    "▲" + value);

                SetValue("BlackPlayerName", newValue);
            }
        }

        /// <summary>
        /// 後手側の対局者名を取得または設定します。
        /// </summary>
        public string WhitePlayerName
        {
            get { return GetValue<string>("WhitePlayerName"); }
            set
            {
                var newValue = (
                    (string.IsNullOrEmpty(value) || value.StartsWith("△")) ?
                    value :
                    "△" + value);

                SetValue("WhitePlayerName", newValue);
            }
        }
        #endregion

        #region 時間系プロパティ
        /// <summary>
        /// 先手の合計持ち時間（秒読みの場合は秒読み時間）を取得または設定します。
        /// </summary>
        public TimeSpan BlackTotalTime
        {
            get { return GetValue<TimeSpan>("BlackTotalTime"); }
            set { SetValue("BlackTotalTime", value); }
        }

        /// <summary>
        /// 先手の消費時間を取得または設定します。
        /// </summary>
        public TimeSpan BlackTime
        {
            get { return GetValue<TimeSpan>("BlackLeaveTime"); }
            set { SetValue("BlackLeaveTime", value); }
        }

        /// <summary>
        /// 後手の合計持ち時間（秒読みの場合は秒読み時間）を取得または設定します。
        /// </summary>
        public TimeSpan WhiteTotalTime
        {
            get { return GetValue<TimeSpan>("WhiteTotalTime"); }
            set { SetValue("WhiteTotalTime", value); }
        }

        /// <summary>
        /// 後手の消費時間を取得または設定します。
        /// </summary>
        public TimeSpan WhiteTime
        {
            get { return GetValue<TimeSpan>("WhiteTime"); }
            set { SetValue("WhiteTime", value); }
        }
        #endregion

        #region その他のプロパティ
        /// <summary>
        /// エフェクト管理オブジェクトを取得または設定します。
        /// </summary>
        public IEffectManager EffectManager
        {
            get { return this.effectManager; }
            set
            {
                if (this.effectManager != value)
                {
                    if (this.effectManager != null)
                    {
                        this.effectManager.Clear();
                        this.effectManager.Container = null;
                    }

                    this.effectManager = value;

                    if (this.effectManager != null)
                    {
                        this.effectManager.Container = this;
                        this.effectManager.Clear();
                    }

                    InitEffect();
                    this.RaisePropertyChanged("EffectManager");
                }
            }
        }

        /// <summary>
        /// 駒が掴まれているなどするかどうかを取得します。
        /// </summary>
        public bool InManipulating
        {
            get { return GetValue<bool>("InManipulating"); }
            set { SetValue("InManipulating", value); }
        }
        #endregion

        #region 駒台の操作
        /// <summary>
        /// 現在の局面から、駒箱に入るべき駒数を調べます。
        /// </summary>
        private void InitKomaboxPieceCount()
        {
            foreach (var pieceType in EnumEx.GetValues<PieceType>())
            {
                var count = Board.GetLeavePieceCount(pieceType);

                this.komaboxCount[(int)pieceType] = count;
            }
        }

        /// <summary>
        /// 持ち駒や駒箱の駒の数を取得します。
        /// </summary>
        private int GetHandCount(PieceType pieceType, BWType bwType)
        {
            if (bwType == BWType.None)
            {
                // 駒箱の駒の数
                return this.komaboxCount[(int)pieceType];
            }
            else
            {
                // 持ち駒の駒の数
                if (Board == null)
                {
                    return 0;
                }

                return Board.GetHandCount(pieceType, bwType);
            }
        }

        /// <summary>
        /// 持ち駒や駒箱の駒の数を設定します。
        /// </summary>
        private void SetHandCount(PieceType pieceType, BWType bwType,
                                  int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("count");
            }

            if (bwType == 0)
            {
                // 駒箱の駒の数
                this.komaboxCount[(int)pieceType] = count;
            }
            else
            {
                // 持ち駒の駒の数
                Board.SetHandCount(pieceType, bwType, count);
            }
        }

        /// <summary>
        /// 持ち駒や駒箱の駒の数を増やします。
        /// </summary>
        private void IncHandCount(PieceType pieceType, BWType bwType)
        {
            SetHandCount(
                pieceType, bwType,
                GetHandCount(pieceType, bwType) + 1);
        }

        /// <summary>
        /// 持ち駒や駒箱の駒の数を減らします。
        /// </summary>
        private void DecHandCount(PieceType pieceType, BWType bwType)
        {
            SetHandCount(
                pieceType, bwType,
                GetHandCount(pieceType, bwType) - 1);
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

            // 一応
            EndMove();

            // 指し手が進んだときのエフェクトを追加します。
            if (EffectManager != null)
            {
                EffectManager.Moved(move, e.IsUndo);
            }
        }

        /// <summary>
        /// 手番が変わった時もエフェクトの初期化を行います。
        /// </summary>
        private void OnTurnChanged(object sender, PropertyChangedEventArgs e)
        {
            InitEffect();
        }

        /// <summary>
        /// エフェクトを初期化します。
        /// </summary>
        private void InitEffect()
        {
            if (EffectManager == null)
            {
                return;
            }

            var board = Board;
            var bwType = (board != null ? board.Turn : BWType.Black);

            EffectManager.InitEffect(bwType);
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

            // this.autoPlayを変える前に局面を変更します。
            // 局面変更時は自動再生を自動で止めるようになっているので、
            // this.autoPlayフィールドを変更した後に局面を変えると、
            // すぐに止まってしまいます。
            this.oldBoard = Board;
            Board = autoPlay.Board;

            autoPlay.ShogiElement = this;
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

            // コントロールは消去しておきます。
            // autoPlay.ShogiElementをnullに設定すると後のStopが
            // 上手く動かないため、ここでは設定しません。
            var autoPlay = this.autoPlay;
            this.autoPlay = null;

            Board = this.oldBoard;
            this.oldBoard = null;

            AutoPlayState = AutoPlayState.None;

            // Boardが変更されるとAutoPlayはすべてクリアされます。
            // Stopの中でBoardが変更されると少し面倒なことになるため、
            // Stopメソッドはすべての状態が落ち着いた後に呼びます。
            autoPlay.Stop();
        }
        #endregion
    }
}
