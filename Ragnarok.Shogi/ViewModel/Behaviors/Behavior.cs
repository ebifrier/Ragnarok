using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Ragnarok.Shogi.ViewModel.Behaviors
{
    /// <summary>
    /// ビヘイビアを管理します。
    /// </summary>
    public class BehaviorCollection : FreezableCollection<Behavior>
    {
        /// <summary>
        /// コレクションのインスタンスをまとめてアタッチします。
        /// </summary>
        public void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            // フリーズし、インスタンスの追加を抑制します。
            Freeze();

            foreach (var behavior in this)
            {
                behavior.Attach(dependencyObject);
            }
        }

        /// <summary>
        /// コレクションのインスタンスをまとめてデタッチします。
        /// </summary>
        public void Detach()
        {
            foreach (var behavior in this)
            {
                behavior.Detach();
            }
        }
    }

    /// <summary>
    /// アタッチ/デタッチ機能を持つオブジェクトです。
    /// </summary>
    public class Behavior : Animatable
    {
        /// <summary>
        /// 対象となるオブジェクトを取得します。
        /// </summary>
        protected DependencyObject AssociatedObject
        {
            get;
            private set;
        }

        /// <summary>
        /// 対象となるオブジェクトの型を取得します。
        /// </summary>
        protected Type AssociatedType
        {
            get;
            private set;
        }

        /// <summary>
        /// 新たなインスタンスを作成します。
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new Behavior();
        }

        /// <summary>
        /// オブジェクトのアタッチを行います。
        /// </summary>
        public void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            if (AssociatedObject != null)
            {
                throw new InvalidOperationException(
                    "すでにAttachされています。");
            }

            AssociatedObject = dependencyObject;
            AssociatedType = dependencyObject.GetType();

            OnAttached();
        }

        /// <summary>
        /// オブジェクトのデタッチを行います。
        /// </summary>
        public void Detach()
        {
            if (AssociatedObject == null)
            {
                throw new InvalidOperationException(
                    "Attachされていません。");
            }

            OnDetaching();

            AssociatedObject = null;
            AssociatedType = null;
        }

        /// <summary>
        /// アタッチ時に呼ばれます。
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// デタッチ時に呼ばれます。
        /// </summary>
        protected virtual void OnDetaching()
        {
        }
    }

    /// <summary>
    /// 型付のビヘイビアを定義します。
    /// </summary>
    public class Behavior<T> : Behavior
        where T : DependencyObject
    {
        /// <summary>
        /// 対象となるオブジェクトを取得します。
        /// </summary>
        protected new T AssociatedObject
        {
            get { return (T)base.AssociatedObject; }
        }
    }
}
