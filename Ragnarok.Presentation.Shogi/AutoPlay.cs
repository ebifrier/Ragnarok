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
    public sealed class AutoPlay
    {
        private List<BoardMove> moveList;
        private IEnumerator<bool> enumerator;
        private int moveIndex;
        private int maxMoveCount;
        private TimeSpan basePosition;
        private TimeSpan elapsed;

        /// <summary>
        /// 再生完了時や途中停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// 背景を取得します。
        /// </summary>
        public Brush Background
        {
            get;
            internal set;
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
        /// まだ指し手が残っているか取得します。
        /// </summary>
        private bool HasMove
        {
            get { return (this.moveIndex < this.maxMoveCount); }
        }

        /// <summary>
        /// 基準時間からの経過時刻を取得します。
        /// </summary>
        private TimeSpan PositionFromBase
        {
            get { return (Position - this.basePosition); }
        }

        /// <summary>
        /// 自動再生の次の手を取得します。
        /// </summary>
        private BoardMove NextMove()
        {
            if (!HasMove)
            {
                return null;
            }

            // アンドゥやリドゥの場合は、指し手がありません。
            if (AutoPlayType != AutoPlayType.Normal)
            {
                this.moveIndex++;
                return null;
            }

            return this.moveList[this.moveIndex++];
        }

        /// <summary>
        /// 指し手を一手だけ進めます。
        /// </summary>
        private void DoMove()
        {
            if (Board == null)
            {
                return;
            }

            switch (AutoPlayType)
            {
                case AutoPlayType.Normal:
                    var move = NextMove();
                    if (move != null)
                    {
                        Board.DoMove(move);
                    }
                    break;
                case AutoPlayType.Undo:
                    Board.Undo();
                    break;
                case AutoPlayType.Redo:
                    Board.Redo();
                    break;
            }
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
        /// コルーチン用のオブジェクトを返します。
        /// </summary>
        private IEnumerable<bool> GetUpdateEnumerator()
        {
            // 最初に背景色のみを更新します。
            if (IsChangeBackground && Background != null)
            {
                while (true)
                {
                    Position += this.elapsed;
                    var opacity = GetBackgroundOpacity(PositionFromBase, false);
                    if (opacity >= 1.0)
                    {
                        this.basePosition += BackgroundFadeInterval;
                        break;
                    }

                    Background.Opacity = opacity;
                    yield return true;
                }
            }

            // 最後の指し手を動かした後に一手分だけ待ちます。
            // エフェクトを表示するためです。
            var didLastInterval = false;
            while (HasMove || !didLastInterval)
            {
                Position += this.elapsed;
                if (PositionFromBase > Interval)
                {
                    this.basePosition += Interval;
                    didLastInterval = !HasMove; // DoMoveの前に呼ぶ

                    DoMove();
                }

                yield return true;
            }

            // 背景色をもとに戻します。
            if (IsChangeBackground && Background != null)
            {
                while (true)
                {
                    Position += this.elapsed;
                    var opacity = GetBackgroundOpacity(PositionFromBase, true);
                    if (opacity <= 0.0)
                    {
                        break;
                    }

                    Background.Opacity = opacity;
                    yield return true;
                }
            }
        }

        /// <summary>
        /// 更新します。
        /// </summary>
        public bool Update(TimeSpan elapsed)
        {
            if (this.enumerator == null)
            {
                return false;
            }

            // コルーチンを進めます。
            if (!this.enumerator.MoveNext())
            {
                RaiseStopped();

                this.enumerator = null;
                return false;
            }
            
            this.elapsed = elapsed;
            return this.enumerator.Current;
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
            if (Board == null)
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
            Board = board;
            Interval = TimeSpan.FromSeconds(1.0);
            BackgroundFadeInterval = TimeSpan.FromSeconds(0.5);
            Position = TimeSpan.Zero;

            this.basePosition = TimeSpan.Zero;
            this.elapsed = TimeSpan.Zero;
            this.enumerator = GetUpdateEnumerator().GetEnumerator();
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
