using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// オブジェクトの各プロパティの変化を記述します。
    /// </summary>
    [ContentProperty("Children")]
    public class Scenario : NotifyObject, IAnimationTimeline
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Scenario()
        {
            Children = new NotifyCollection<IAnimationTimeline>();
            Target = null;

            Children.CollectionChanged += Children_CollectionChanged;
        }

        /// <summary>
        /// コレクション変更時に呼ばれます。
        /// </summary>
        void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsBegan && e.NewItems != null)
            {
                e.NewItems
                    .OfType<IAnimationTimeline>()
                    .Where(_ => _ != null)
                    .ForEach(_ => _.Begin(Target));
            }
        }

        /// <summary>
        /// シナリオの実行が始まっているかどうかを取得します。
        /// </summary>
        public object Target
        {
            get { return GetValue<object>("Target"); }
            private set { SetValue("Target", value); }
        }

        /// <summary>
        /// シナリオの実行が始まっているかどうかを取得します。
        /// </summary>
        [DependOnProperty("Target")]
        public bool IsBegan
        {
            get { return (Target != null); }
        }

        /// <summary>
        /// フレームごとに更新するオブジェクトのリストを取得します。
        /// </summary>
        public NotifyCollection<IAnimationTimeline> Children
        {
            get { return GetValue<NotifyCollection<IAnimationTimeline>>("Children"); }
            private set { SetValue("Children", value); }
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

            if (Target != null)
            {
                throw new InvalidOperationException(
                    "シナリオはすでに開始されています。");
            }

            foreach(var child in Children)
            {
                child.Begin(target);
            }

            Target = target;
        }

        /// <summary>
        /// すべてのアニメーションを停止します。
        /// </summary>
        public void Stop()
        {
            if (Target == null)
            {
                // シナリオは開始されていません。
                return;
            }

            foreach (var child in Children)
            {
                child.Stop();
            }

            Target = null;
        }

        /// <summary>
        /// フレーム毎に呼ばれ、オブジェクトの更新処理を行います。
        /// </summary>
        public void DoEnterFrame(TimeSpan elapsedTime, object state)
        {
            if (!IsBegan)
            {
                return;
            }

            foreach (var child in Children)
            {
                child.DoEnterFrame(elapsedTime, state);
            }
        }
    }
}
