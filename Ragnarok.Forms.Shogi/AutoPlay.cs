using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi
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
        /// <summary>
        /// 指し手のデフォルトの再生間隔です。
        /// </summary>
        public static readonly TimeSpan DefaultInterval =
            TimeSpan.FromSeconds(1);
        /// <summary>
        /// 自動再生用エフェクトのデフォルトのフェードイン・アウトの時間です。
        /// </summary>
        public static readonly TimeSpan DefaultEffectFadeInterval =
            TimeSpan.FromSeconds(0.5);

        private readonly List<BoardMove> moveList;
        private int moveIndex;
        private IEnumerator<bool> enumerator;

        /// <summary>
        /// 再生完了時や途中停止時に呼ばれるイベントです。
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// 自動更新に使われる列挙子のファクトリを取得または設定します。
        /// </summary>
        public Func<IEnumerable<bool>> UpdateEnumeratorFactory
        {
            get;
            set;
        }

        /// <summary>
        /// 自動再生を行う将棋エレメントを取得します。
        /// </summary>
        public GLShogiElement ShogiElement
        {
            get;
            internal set;
        }

        /// <summary>
        /// 開始局面を取得または設定します。
        /// </summary>
        public Board StartBoard
        {
            get;
            private set;
        }

        /// <summary>
        /// 現在の局面を取得または設定します。
        /// </summary>
        public Board Board
        {
            get;
            private set;
        }

        /// <summary>
        /// 局面のコピーに対して指し手を動かすかどうかを取得します。
        /// </summary>
        public bool IsCloneBoard
        {
            get;
            private set;
        }

        /// <summary>
        /// 重要な自動再生かどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 真の場合は、GUIのマウス押下で自動再生をキャンセルしません。
        /// </remarks>
        public bool IsImportant
        {
            get { return !IsCloneBoard; }
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
        /// 最後の一手の後、エフェクト分だけ待つかどうかを取得または設定します。
        /// </summary>
        public bool IsWaitForLastMove
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
        /// 自動再生用エフェクトを使用するかどうかを取得または設定します。
        /// </summary>
        public bool IsUseEffect
        {
            get;
            set;
        }

        /// <summary>
        /// 自動再生用エフェクトのフェードイン／アウトする時間を取得または設定します。
        /// </summary>
        public TimeSpan EffectFadeInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 指し手の最大数を取得します。
        /// </summary>
        public int MaxMoveCount
        {
            get;
            set;
        }
        
        /// <summary>
        /// まだ指し手が残っているか取得します。
        /// </summary>
        private bool HasMove
        {
            get { return (this.moveIndex < MaxMoveCount); }
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
        /// 変更中のエフェクトの不透明度を計算します。
        /// </summary>
        private double GetEffectOpacity(TimeSpan progress, bool isReserve)
        {
            if (progress >= EffectFadeInterval)
            {
                return (isReserve ? 0.0 : 1.0);
            }

            var progressSeconds = progress.TotalSeconds;
            var intervalSeconds = EffectFadeInterval.TotalSeconds;
            var rate = progressSeconds / intervalSeconds;

            return MathEx.Between(0.0, 1.0, isReserve ? 1.0 - rate : rate);
        }

        /// <summary>
        /// エフェクトのフェードイン／アウトを処理します。
        /// </summary>
        protected IEnumerable<bool> EffectFadeExecutor(bool isReverse)
        {
            if (!IsUseEffect || ShogiElement == null)
            {
                yield break;
            }

            var target = (isReverse ? 0.0 : 1.0);
            while (true)
            {
                var opacity = GetEffectOpacity(PositionFromBase, isReverse);
                if (opacity == target)
                {
                    break;
                }

                ShogiElement.AutoPlayOpacity = opacity;
                yield return true;
            }

            BasePosition += EffectFadeInterval;
            ShogiElement.AutoPlayOpacity = target;
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

            // 必要なら最後の指し手を動かした後に一手分だけ待ちます。
            // エフェクトを表示するためです。
            if (IsWaitForLastMove)
            {
                while (PositionFromBase < Interval)
                {
                    yield return true;
                }

                BasePosition += Interval;
            }
        }

        /// <summary>
        /// コルーチン用のオブジェクトを返します。
        /// </summary>
        protected IEnumerable<bool> GetUpdateEnumerator()
        {
            foreach (var result in EffectFadeExecutor(false))
            {
                yield return result;
            }

            foreach (var result in WaitExecutor(BeginningInterval))
            {
                yield return result;
            }

            // 指し手を進めます。
            foreach (var result in DoMoveExecutor())
            {
                yield return result;
            }

            foreach (var result in WaitExecutor(EndingInterval))
            {
                yield return result;
            }

            foreach (var result in EffectFadeExecutor(true))
            {
                yield return result;
            }
        }

        /// <summary>
        /// 更新します。
        /// </summary>
        public bool Update(TimeSpan elapsed)
        {
            if (this.enumerator == null)
            {
                var enumerable = UpdateEnumeratorFactory();
                if (enumerable == null)
                {
                    return false;
                }

                this.enumerator = enumerable.GetEnumerator();
            }

            // コルーチンを進めます。
            if (!this.enumerator.MoveNext())
            {
                //RaiseStopped();

                this.enumerator = null;
                return false;
            }

            // 時間はここで進めます。
            Position += elapsed;
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

            FormsUtil.InvalidateCommand();
        }

        /// <summary>
        /// 自動再生の途中停止を行います。
        /// </summary>
        public void Stop()
        {
            if (ShogiElement !=null)
            {
                ShogiElement.AutoPlayOpacity = 0.0;
            }

            Board = StartBoard.Clone();
            Position = TimeSpan.Zero;
            BasePosition = TimeSpan.Zero;
            this.enumerator = null;
            this.moveIndex = 0;

            RaiseStopped();
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (StartBoard == null || !StartBoard.Validate())
            {
                return false;
            }

            if (AutoPlayType == AutoPlayType.Normal)
            {
                if (this.moveList == null)
                {
                    return false;
                }

                return StartBoard.CanMoveList(this.moveList);
            }

            return true;
        }

        /// <summary>
        /// 共通コンストラクタ
        /// </summary>
        private AutoPlay(Board board, bool isCloneBoard)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            UpdateEnumeratorFactory = () => GetUpdateEnumerator();
            StartBoard = board;
            Board = (isCloneBoard ? board.Clone() : board);
            IsCloneBoard = isCloneBoard;
            Interval = DefaultInterval;
            EffectFadeInterval = DefaultEffectFadeInterval;
            BeginningInterval = TimeSpan.Zero;
            EndingInterval = TimeSpan.Zero;
            Position = TimeSpan.Zero;
            BasePosition = TimeSpan.Zero;
            IsWaitForLastMove = true;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AutoPlay(Board board, bool isCloneBoard,
                        IEnumerable<BoardMove> moveList)
            : this(board, isCloneBoard)
        {
            if (moveList == null)
            {
                throw new ArgumentNullException("moveList");
            }

            AutoPlayType = AutoPlayType.Normal;
            MaxMoveCount = moveList.Count();

            this.moveList = new List<BoardMove>(moveList);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AutoPlay(Board board, bool isCloneBoard,
                        AutoPlayType autoPlayType, int maxMoveCount = -1)
            : this(board, isCloneBoard)
        {
            if (autoPlayType != AutoPlayType.Undo &&
                autoPlayType != AutoPlayType.Redo)
            {
                throw new ArgumentException(
                    "アンドゥかリドゥを選択してください。",
                    "autoPlayType");
            }

            AutoPlayType = autoPlayType;
            MaxMoveCount = (
                maxMoveCount >= 0 ?
                maxMoveCount :
                (autoPlayType == AutoPlayType.Undo ?
                    board.CanUndoCount :
                    board.CanRedoCount));
        }
    }
}
