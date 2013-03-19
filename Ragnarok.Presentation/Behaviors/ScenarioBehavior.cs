using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

using Ragnarok.ObjectModel;

namespace Ragnarok.Presentation.Behaviors
{
    using VisualObject;

    /// <summary>
    /// シナリオのXAMLからの開始・終了を行います。
    /// </summary>
    [ContentProperty("ScenarioChildren")]
    public class ScenarioBehavior : Behavior<EntityObject>
    {
        /// <summary>
        /// シナリオを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ScenarioProperty =
            DependencyProperty.Register(
                "Scenario", typeof(Scenario),
                typeof(ScenarioBehavior),
                new UIPropertyMetadata(null));

        /// <summary>
        /// 音声ファイルのパスを取得または設定します。
        /// </summary>
        public Scenario Scenario
        {
            get { return (Scenario)GetValue(ScenarioProperty); }
            set { SetValue(ScenarioProperty, value); }
        }

        /// <summary>
        /// シナリオの各子要素を取得します。
        /// </summary>
        public NotifyCollection<AnimationTimeline> ScenarioChildren
        {
            get { return Scenario.Children; }
        }

        /// <summary>
        /// アタッチ時に呼ばれます。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (Scenario != null)
            {
                Scenario.Begin(this);
            }
        }

        /// <summary>
        /// デタッチ時に呼ばれます。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (Scenario != null)
            {
                Scenario.Stop();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ScenarioBehavior()
        {
            Scenario = new Scenario();
        }
    }
}
