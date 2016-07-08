using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi
{
    using View;

    public interface IAutoPlay
    {
        /// <summary>
        /// 自動再生を行う将棋エレメントを取得します。
        /// </summary>
        GLShogiElement ShogiElement { get; set; }
        
        /// <summary>
        /// 重要な自動再生かどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 真の場合は、GUIのマウス押下で自動再生をキャンセルしません。
        /// </remarks>
        bool IsImportant { get; }

        /// <summary>
        /// 更新します。
        /// </summary>
        bool Update(TimeSpan elapsed);

        /// <summary>
        /// 自動再生の途中停止を行います。
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// 指し手の自動再生時に使われます。再生用の変化を保存します。
    /// </summary>
    public class AutoPlayBase : IAutoPlay
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
            set;
        }

        /// <summary>
        /// 重要な自動再生かどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 真の場合は、GUIのマウス押下で自動再生をキャンセルしません。
        /// </remarks>
        public bool IsImportant
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

        protected virtual bool DoMove()
        {
            return false;
        }

        /// <summary>
        /// 指し手を進めます。
        /// </summary>
        protected IEnumerable<bool> DoMoveExecutor()
        {
            // 最初の指し手はすぐに表示します。
            var hasMove = DoMove();

            while (hasMove)
            {
                if (PositionFromBase > Interval)
                {
                    BasePosition += Interval;
                    hasMove = DoMove();
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
                OnStarted();
            }

            // コルーチンを進めます。
            if (!this.enumerator.MoveNext())
            {
                this.enumerator = null;
                return false;
            }

            // 時間はここで進めます。
            Position += elapsed;
            return this.enumerator.Current;
        }

        /// <summary>
        /// 自動再生の途中停止を行います。
        /// </summary>
        public void Stop()
        {
            this.enumerator = null;

            OnStopped();
            RaiseStopped();
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
        /// 自動自動開始時に呼ばれます。
        /// </summary>
        protected virtual void OnStarted()
        {
            if (ShogiElement != null)
            {
                ShogiElement.IsAutoPlaying = true;
            }
        }

        /// <summary>
        /// 自動再生の途中時を行います。
        /// </summary>
        protected virtual void OnStopped()
        {
            if (ShogiElement != null)
            {
                ShogiElement.AutoPlayOpacity = 0.0;
                ShogiElement.IsAutoPlaying = false;
            }

            Position = TimeSpan.Zero;
            BasePosition = TimeSpan.Zero;
        }

        /// <summary>
        /// 共通コンストラクタ
        /// </summary>
        public AutoPlayBase(bool isImportant)
        {
            UpdateEnumeratorFactory = () => GetUpdateEnumerator();
            IsImportant = isImportant;
            Interval = DefaultInterval;
            EffectFadeInterval = DefaultEffectFadeInterval;
            BeginningInterval = TimeSpan.Zero;
            EndingInterval = TimeSpan.Zero;
            Position = TimeSpan.Zero;
            BasePosition = TimeSpan.Zero;
            IsWaitForLastMove = true;
        }
    }
}
