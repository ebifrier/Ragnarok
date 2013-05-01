using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

using Ragnarok.Shogi;

namespace Ragnarok.Presentation.Shogi
{
    using View;

    /// <summary>
    /// 自動再生の種別です。
    /// </summary>
    public enum AutoPlayType
    {
        /// <summary>
        /// 指し手を動かしません。
        /// </summary>
        None,
        /// <summary>
        /// 与えられた指し手を自動再生します。
        /// </summary>
        Normal,
        /// <summary>
        /// 局面を元に戻しながら自動再生します。
        /// </summary>
        Undo,
        /// <summary>
        /// 局面を次に進めながら自動再生します。
        /// </summary>
        Redo,
    }

    /// <summary>
    /// 指し手の自動再生時に使われます。再生用の変化を保存します。
    /// </summary>
    public class AutoPlay
    {
        private List<BoardMove> moveList;
        private int moveIndex;
        private int maxMoveCount;
        private DateTime prevTime = DateTime.Now;

        /// <summary>
        /// 再生完了時や途中停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// 自動更新に使われる列挙子を取得または設定します。
        /// </summary>
        public IEnumerator<bool> UpdateEnumerator
        {
            get;
            set;
        }

        /// <summary>
        /// 背景を取得します。
        /// </summary>
        public Brush Background
        {
            get;
            set;
        }

        /// <summary>
        /// 開始局面を取得または設定します。
        /// </summary>
        public Board Board
        {
            get;
            private set;
        }

        /// <summary>
        /// 自動再生の種類を取得します。
        /// </summary>
        public AutoPlayType AutoPlayType
        {
            get;
            private set;
        }

        /// <summary>
        /// 現在の再生位置を取得します。
        /// </summary>
        public TimeSpan Position
        {
            get;
            private set;
        }

        /// <summary>
        /// 指し手の再生間隔を取得または設定します。
        /// </summary>
        public TimeSpan Interval
        {
            get;
            set;
        }

        /// <summary>
        /// 背景を変化させるかどうかを取得または設定します。
        /// </summary>
        public bool IsChangeBackground
        {
            get;
            set;
        }

        /// <summary>
        /// 背景がフェードイン／アウトする時間を取得または設定します。
        /// </summary>
        public TimeSpan BackgroundFadeInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 開始までの時間を取得または設定します。
        /// </summary>
        public TimeSpan BeginningInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 終了までの時間を取得または設定します。
        /// </summary>
        public TimeSpan EndingInterval
        {
            get;
            set;
        }
        
        /// <summary>
        /// まだ指し手が残っているか取得します。
        /// </summary>
        private bool HasMove
        {
            get { return (this.moveIndex < this.maxMoveCount); }
        }

        /// <summary>
        /// 現在の再生基準位置を取得します。
        /// </summary>
        protected TimeSpan BasePosition
        {
            get;
            set;
        }

        /// <summary>
        /// 基準時間からの経過時刻を取得します。
        /// </summary>
        protected TimeSpan PositionFromBase
        {
            get { return (Position - BasePosition); }
        }

        /// <summary>
        /// 指定時間だけ待ちます。
        /// </summary>
        protected IEnumerable<bool> WaitExecutor(TimeSpan waitTime)
        {
            while (true)
            {
                if (PositionFromBase >= waitTime)
                {
                    break;
                }

                yield return true;
            }

            BasePosition += waitTime;
        }

        /// <summary>
        /// 背景色変更中の情報を取得します。
        /// </summary>
        private double GetBackgroundOpacity(TimeSpan progress, bool isReverse)
        {
            if (progress >= BackgroundFadeInterval)
            {
                return (isReverse ? 0.0 : 1.0);
            }

            // 背景の不透明度を更新します。
            var progressSeconds = progress.TotalSeconds;
            var totalSeconds = BackgroundFadeInterval.TotalSeconds;
            var rate = (progressSeconds / totalSeconds);

            return MathEx.Between(0.0, 1.0, isReverse ? 1.0 - rate : rate);
        }

        /// <summary>
        /// 背景をフェードインします。
        /// </summary>
        protected IEnumerable<bool> BackgroundFadeInExecutor()
        {
            if (!IsChangeBackground || Background == null)
            {
                yield break;
            }

            while (true)
            {
                var opacity = GetBackgroundOpacity(PositionFromBase, false);
                if (opacity >= 1.0)
                {
                    break;
                }

                Background.Opacity = opacity;
                yield return true;
            }

            BasePosition += BackgroundFadeInterval;
            Background.Opacity = 1.0;
        }

        /// <summary>
        /// 背景をフェードアウトします。
        /// </summary>
        protected IEnumerable<bool> BackgroundFadeOutExecutor()
        {
            if (!IsChangeBackground || Background == null)
            {
                yield break;
            }

            while (true)
            {
                var opacity = GetBackgroundOpacity(PositionFromBase, true);
                if (opacity <= 0.0)
                {
                    break;
                }

                Background.Opacity = opacity;
                yield return true;
            }

            BasePosition += BackgroundFadeInterval;
            Background.Opacity = 0.0;
        }

        /// <summary>
        /// 指し手を一手だけ進めます。
        /// </summary>
        private void DoMove()
        {
            if (!HasMove || Board == null)
            {
                return;
            }

            switch (AutoPlayType)
            {
                case AutoPlayType.Normal:
                    var move = this.moveList[this.moveIndex++];
                    if (move != null)
                    {
                        Board.DoMove(move);
                    }
                    break;
                case AutoPlayType.Undo:
                    this.moveIndex += 1;
                    Board.Undo();
                    break;
                case AutoPlayType.Redo:
                    this.moveIndex += 1;
                    Board.Redo();
                    break;
            }
        }

        /// <summary>
        /// 指し手を進めます。
        /// </summary>
        protected IEnumerable<bool> DoMoveExecutor()
        {
            // 最初の指し手はすぐに表示します。
            DoMove();

            while (HasMove)
            {
                if (PositionFromBase > Interval)
                {
                    BasePosition += Interval;
                    DoMove();
                }

                yield return true;
            }

            // 最後の指し手を動かした後に一手分だけ待ちます。
            // エフェクトを表示するためです。
            while (PositionFromBase < Interval)
            {
                yield return true;
            }
            BasePosition += Interval;
        }

        /// <summary>
        /// コルーチン用のオブジェクトを返します。
        /// </summary>
        protected IEnumerable<bool> GetUpdateEnumerator()
        {
            foreach (var result in WaitExecutor(BeginningInterval))
            {
                yield return result;
            }

            // 最初に背景色のみを更新します。
            foreach (var result in BackgroundFadeInExecutor())
            {
                yield return result;
            }

            // 指し手を進めます。
            foreach (var result in DoMoveExecutor())
            {
                yield return result;
            }

            // 背景色をもとに戻します。
            foreach (var result in BackgroundFadeOutExecutor())
            {
                yield return result;
            }

            foreach (var result in WaitExecutor(EndingInterval))
            {
                yield return result;
            }
        }

        /// <summary>
        /// 更新します。
        /// </summary>
        public bool Update(TimeSpan elapsed)
        {
            if (UpdateEnumerator == null)
            {
                return false;
            }

            // コルーチンを進めます。
            if (!UpdateEnumerator.MoveNext())
            {
                RaiseStopped();

                UpdateEnumerator = null;
                return false;
            }

            // 時間はここで進めます。
            Position += elapsed;
            return UpdateEnumerator.Current;
        }

        /// <summary>
        /// Stoppedイベントを発行します。
        /// </summary>
        private void RaiseStopped()
        {
            var handler = Interlocked.Exchange(ref Stopped, null);

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, EventArgs.Empty));
            }

            WPFUtil.InvalidateCommand();
        }

        /// <summary>
        /// 自動再生の途中停止を行います。
        /// </summary>
        public void Stop()
        {
            if (Background != null)
            {
                Background.Opacity = 0.0;
            }

            RaiseStopped();
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (Board == null || !Board.Validate())
            {
                return false;
            }

            if (AutoPlayType == AutoPlayType.Normal)
            {
                if (this.moveList == null)
                {
                    return false;
                }

                return Board.CanMoveList(this.moveList);
            }

            return true;
        }

        /// <summary>
        /// 共通コンストラクタ
        /// </summary>
        private AutoPlay(Board board)
        {
            UpdateEnumerator = GetUpdateEnumerator().GetEnumerator();
            Board = board;
            Interval = TimeSpan.FromSeconds(1.0);
            BeginningInterval = TimeSpan.Zero;
            EndingInterval = TimeSpan.Zero;
            BackgroundFadeInterval = TimeSpan.FromSeconds(0.5);
            Position = TimeSpan.Zero;
            BasePosition = TimeSpan.Zero;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AutoPlay(Board board, IEnumerable<BoardMove> moveList)
            : this(board)
        {
            if (moveList == null)
            {
                throw new ArgumentNullException("moveList");
            }

            AutoPlayType = AutoPlayType.Normal;

            this.moveList = new List<BoardMove>(moveList);
            this.maxMoveCount = moveList.Count();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AutoPlay(Board board, AutoPlayType autoPlayType)
            : this(board)
        {
            if (autoPlayType != AutoPlayType.Undo &&
                autoPlayType != AutoPlayType.Redo)
            {
                throw new ArgumentException(
                    "アンドゥかリドゥを選択してください。",
                    "autoPlayType");
            }

            AutoPlayType = autoPlayType;

            this.maxMoveCount =
                (autoPlayType == AutoPlayType.Undo ?
                 board.CanUndoCount :
                 board.CanRedoCount);
        }
    }
}
