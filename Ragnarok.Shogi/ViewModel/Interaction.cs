using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

using Ragnarok;
using Ragnarok.ObjectModel;

namespace Ragnarok.Shogi.ViewModel
{
    using Behaviors;

    /// <summary>
    /// XAMLからのアクションなどを処理します。
    /// </summary>
    public static class Interaction
    {
        /// <summary>
        /// 名前をShadowBehaviorsにすることで、
        /// GetBehaviorsからのアクセスを強制します。
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached(
                "ShadowBehaviors", typeof(BehaviorCollection),
                typeof(Interaction),
                new FrameworkPropertyMetadata(new BehaviorCollection()));

        /// <summary>
        /// Behaviourを初期化します。
        /// </summary>
        public static void ResetBehaviours(DependencyObject target)
        {
            target.SetValue(BehaviorsProperty, null);
        }

        /// <summary>
        /// Behaviourを取得します。
        /// </summary>
        public static BehaviorCollection GetBehaviors(DependencyObject target)
        {
            // BehaviorsPropertyを使って値を取得してみる
            var ret = (BehaviorCollection)target.GetValue(BehaviorsProperty);

            // 値がnullだったらインスタンスを作ってセットしておく
            if (ret == null)
            {
                ret = new BehaviorCollection();
                target.SetValue(BehaviorsProperty, ret);
            }

            return ret;
        }
    }
}
