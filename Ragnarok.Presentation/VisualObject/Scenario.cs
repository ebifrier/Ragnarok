using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;

using Ragnarok.ObjectModel;

namespace Ragnarok.Presentation.VisualObject
{
    /// <summary>
    /// オブジェクトの各プロパティの変化を記述します。
    /// </summary>
    [ContentProperty("Children")]
    public class Scenario : DependencyObject
    {
        /// <summary>
        /// アニメーション対象を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached(
                "Target", typeof(IAnimatable), typeof(Scenario),
                new UIPropertyMetadata(null));

        /// <summary>
        /// アニメーション対象を取得します。
        /// </summary>
        public static IAnimatable GetTarget(DependencyObject target)
        {
            return (IAnimatable)target.GetValue(TargetProperty);
        }

        /// <summary>
        /// アニメーション対象を設定します。
        /// </summary>
        public static void SetTarget(DependencyObject target, IAnimatable value)
        {
            target.SetValue(TargetProperty, value);
        }

        /// <summary>
        /// アニメーション対象へのパスを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TargetPathProperty =
            DependencyProperty.RegisterAttached(
                "TargetPath", typeof(string), typeof(Scenario),
                new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// アニメーション対象へのパスを取得します。
        /// </summary>
        public static string GetTargetPath(DependencyObject target)
        {
            return (string)target.GetValue(TargetPathProperty);
        }

        /// <summary>
        /// アニメーション対象へのパスを設定します。
        /// </summary>
        public static void SetTargetPath(DependencyObject target, string value)
        {
            target.SetValue(TargetPathProperty, value);
        }

        /// <summary>
        /// アニメーションの対象プロパティを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TargetPropertyProperty =
            DependencyProperty.RegisterAttached(
                "TargetProperty", typeof(string), typeof(Scenario),
                new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// アニメーションの対象プロパティを取得します。
        /// </summary>
        public static string GetTargetProperty(DependencyObject target)
        {
            return (string)target.GetValue(TargetPropertyProperty);
        }

        /// <summary>
        /// アニメーションの対象プロパティを設定します。
        /// </summary>
        public static void SetTargetProperty(DependencyObject target, string value)
        {
            target.SetValue(TargetPropertyProperty, value);
        }

        /// <summary>
        /// アニメーションリストを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register(
                "Children", typeof(NotifyCollection<AnimationTimeline>),
                typeof(Scenario),
                new UIPropertyMetadata(null));

        /// <summary>
        /// アニメーションリストを取得します。
        /// </summary>
        public NotifyCollection<AnimationTimeline> Children
        {
            get { return (NotifyCollection<AnimationTimeline>)GetValue(ChildrenProperty); }
            private set { SetValue(ChildrenProperty, value); }
        }

        /*private List<Tuple<IAnimatable, DependencyProperty>> animatedPropertyList =
            new List<Tuple<IAnimatable, DependencyProperty>>();*/

        /// <summary>
        /// アニメーションを開始します。
        /// </summary>
        public void Begin(Animatable target)
        {
            foreach (var anim in Children)
            {
                if (anim == null)
                {
                    continue;
                }

                var animTarget = GetTarget(anim);
                if (animTarget == null)
                {
                    animTarget = target;

                    var targetPath = GetTargetPath(anim);
                    if (!string.IsNullOrEmpty(targetPath))
                    {
                        var targetProperty = WPFUtil.GetDependencyProperty(
                            target.GetType(),
                            targetPath);

                        if (targetProperty != null)
                        {
                            var t = target.GetValue(targetProperty) as Animatable;
                            if (t != null)
                            {
                                animTarget = t;
                            }
                        }
                    }
                }

                var property = WPFUtil.GetDependencyProperty(
                    animTarget.GetType(),
                    GetTargetProperty(anim));

                if (property != null)
                {
                    animTarget.BeginAnimation(property, anim);
                }

                /*this.animatedPropertyList.Add(
                    Tuple.Create(animTarget, property));*/
            }
        }

        /// <summary>
        /// すべてのアニメーションを停止します。
        /// </summary>
        public void Stop()
        {
            /*this.animatedPropertyList.ForEach(
                _ => _.Item1.BeginAnimation(_.Item2, null));

            this.animatedPropertyList.Clear();*/
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Scenario()
        {
            // コンストラクタで初期化しないと、インスタンスごとに
            // オブジェクトが作られません。
            Children = new NotifyCollection<AnimationTimeline>();
        }
    }
}
