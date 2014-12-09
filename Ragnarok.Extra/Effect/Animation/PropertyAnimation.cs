using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Extra.Effect.Animation
{
    /// <summary>
    /// アニメーション停止時の挙動を指定します。
    /// </summary>
    public enum FillBehavior
    {
        /// <summary>
        /// アニメーション終了時の値を保持し続けます。
        /// </summary>
        HoldEnd,
        /// <summary>
        /// アニメーションを停止し、値を元に戻します。
        /// </summary>
        Stop,
    }

    /// <summary>
    /// プロパティアニメーションに関わる基本クラスです。
    /// </summary>
    public abstract class PropertyAnimation : NotifyObject, IAnimationTimeline
    {
        private bool initialized;
        private IPropertyObject pobj;
        private object startValue;
        private double currentTick;
        private bool isReversing;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PropertyAnimation(Type propertyType)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException("propertyType");
            }

            TargetPropertyType = propertyType;
        }

        /// <summary>
        /// アニメーションの完了時に呼ばれます。
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// アニメーション対象となるプロパティ型を取得します。
        /// </summary>
        public Type TargetPropertyType
        {
            get;
            private set;
        }

        /// <summary>
        /// アニメーションの実行時間を取得または設定します。
        /// </summary>
        public TimeSpan Duration
        {
            get { return GetValue<TimeSpan>("Duration"); }
            set { SetValue("Duration", value); }
        }

        /// <summary>
        /// アニメーションの実行時間[ミリ秒]を取得します。
        /// </summary>
        [DependOnProperty("Duration")]
        public double DurationMilliseconds
        {
            get { return Duration.TotalMilliseconds; }
        }

        /// <summary>
        /// 時間内でアニメーションを無限に繰り返すかどうかを取得または設定します。
        /// </summary>
        public bool IsRepeatForever
        {
            get { return GetValue<bool>("IsRepeatForever"); }
            set { SetValue("IsRepeatForever", value); }
        }

        /// <summary>
        /// 停止時の挙動を取得または設定します。
        /// </summary>
        public FillBehavior FillBehavior
        {
            get { return GetValue<FillBehavior>("FillBehavior"); }
            set { SetValue("FillBehavior", value); }
        }

        /// <summary>
        /// 繰り返し時に往復するかどうかを取得または設定します。
        /// </summary>
        public bool AutoReverse
        {
            get { return GetValue<bool>("AutoReverse"); }
            set { SetValue("AutoReverse", value); }
        }

        /// <summary>
        /// アニメーションが完了したかどうかを取得または設定します。
        /// </summary>
        public bool IsCompleted
        {
            get { return GetValue<bool>("IsCompleted"); }
            private set { SetValue("IsCompleted", value); }
        }

        /// <summary>
        /// アニメーションが停止したかどうかを取得または設定します。
        /// </summary>
        public bool IsStopped
        {
            get { return GetValue<bool>("IsStopped"); }
            private set { SetValue("IsStopped", value); }
        }

        /// <summary>
        /// アニメーション対象となるオブジェクトを取得または設定します。
        /// </summary>
        public object Target
        {
            get { return GetValue<object>("Target"); }
            set { SetValue("Target", value); }
        }

        /// <summary>
        /// アニメーション対象となるプロパティ名を取得または設定します。
        /// </summary>
        public string TargetProperty
        {
            get { return GetValue<string>("TargetProperty"); }
            set { SetValue("TargetProperty", value); }
        }

        /// <summary>
        /// アニメーションを開始します。
        /// </summary>
        public void Begin(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (this.initialized)
            {
                throw new InvalidOperationException(
                    "PropertyAnimationはすでに開始されています。");
            }

            if (IsCompleted)
            {
                throw new InvalidOperationException(
                    "PropertyAnimationはすでに完了されています。");
            }

            // Targetが既に設定されている場合は引数のtargetを無視します。
            if (Target == null)
            {
                Target = target;
            }
            else
            {
                target = Target;
            }

            this.pobj = GetPropertyObject(target);
            this.startValue = this.pobj.GetValue(target);

            OnBegin(target);

            this.currentTick = 0;
            this.initialized = true;
        }

        /// <summary>
        /// プロパティ値の取得・設定用オブジェクトを取得します。
        /// </summary>
        private IPropertyObject GetPropertyObject(object target)
        {
            var pobj = MethodUtil.GetPropertyObject(
                target.GetType(), TargetProperty);
            if (pobj == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0}.{1}: プロパティ名が正しくありません。",
                        target.GetType(), TargetProperty));
            }

            if (pobj.PropertyType != TargetPropertyType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0}.{1}: プロパティの型が正しくありません。",
                        target.GetType(), TargetProperty));
            }

            if (!pobj.CanRead)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0}.{1}: このプロパティは読み込みできません。",
                        target.GetType(), TargetProperty));
            }

            if (!pobj.CanWrite)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0}.{1}: このプロパティには書き込みできません。",
                        target.GetType(), TargetProperty));
            }

            return pobj;
        }

        /// <summary>
        /// アニメーションの開始処理を行います。
        /// </summary>
        protected virtual void OnBegin(object target)
        {
        }

        /// <summary>
        /// アニメーションを停止します。
        /// </summary>
        public void Stop()
        {
            if (!this.initialized)
            {
                return;
            }

            IsStopped = true;

            // 必要であれば値を元に戻します。
            if (FillBehavior == FillBehavior.Stop)
            {
                this.pobj.SetValue(Target, this.startValue);
            }

            OnStop();
        }

        /// <summary>
        /// アニメーションの停止処理を行います。
        /// </summary>
        protected virtual void OnStop()
        {
        }

        /// <summary>
        /// アニメーションが完了したときに呼ばれます。
        /// </summary>
        private void Complete()
        {
            // アニメーション完了時はフラグを立て、イベントを呼びます。
            IsCompleted = true;
            Completed.SafeRaiseEvent(this, EventArgs.Empty);

            Stop();
        }

        /// <summary>
        /// アニメーション完了時の処理を行います。
        /// </summary>
        protected virtual void OnCompleted()
        {
        }

        /// <summary>
        /// アニメーションの更新処理を行います。
        /// </summary>
        public void DoEnterFrame(TimeSpan elapsedTime, object state)
        {
            if (!this.initialized)
            {
                return;
            }

            var durationMillis = DurationMilliseconds;
            var frameTick = 0.0;

            // アニメーション時間を更新します。
            this.currentTick += elapsedTime.TotalMilliseconds;

            // IsRepeatForeverの場合は永遠に繰り返します。
            // AutoReverseの場合は、アニメーションが最後まで行った後
            // 進行度を元に戻しながらアニメーションを続けます。
            if (IsRepeatForever && Duration == TimeSpan.Zero)
            {
                // 永遠に繰り返す場合、Durationが０ならば
                // frameTimeを常に０にします。
                frameTick = 0.0;
            }
            else if (IsRepeatForever)
            {
                // 永遠に繰り返す場合は、進行度を巻き戻しながら繰り返します。
                while (this.currentTick >= durationMillis)
                {
                    this.currentTick -= durationMillis;
                    this.isReversing = !this.isReversing;
                }

                // 繰り返しの進む方向を鑑みながらframeTimeを決定します。
                if (AutoReverse && this.isReversing)
                {
                    // 逆方向に進行度を進める場合
                    frameTick = durationMillis - this.currentTick;
                }
                else
                {
                    // 正方向に進行度を進める場合（通常時）
                    frameTick = this.currentTick;
                }
            }
            else
            {
                // 永遠に繰り返さない場合は、時間が来たら終了します。
                if (this.currentTick >= durationMillis)
                {
                    Complete();
                    return;
                }

                frameTick = this.currentTick;
            }

            // 新たなアニメーション補間値を設定します。
            var frameTime = TimeSpan.FromMilliseconds(frameTick);
            var newValue = UpdateProperty(frameTime);
            this.pobj.SetValue(Target, newValue);
        }

        /// <summary>
        /// アニメーションの更新を行います。
        /// </summary>
        protected abstract object UpdateProperty(TimeSpan frameTime);
    }
}
