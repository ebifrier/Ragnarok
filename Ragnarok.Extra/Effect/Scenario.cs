using System;
using System.Collections.Generic;
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
    public class Scenario : NotifyObject, IAnimationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Scenario()
        {
            Children = new NotifyCollection<IAnimationObject>();
        }

        /// <summary>
        /// フレームごとに更新するオブジェクトのリストを取得します。
        /// </summary>
        public NotifyCollection<IAnimationObject> Children
        {
            get { return GetValue<NotifyCollection<IAnimationObject>>("Children"); }
            private set { SetValue("Children", value); }
        }

        /// <summary>
        /// アニメーションを開始します。
        /// </summary>
        public void Begin(object target)
        {
            foreach(var child in Children)
            {
                child.Begin(target);
            }
        }

        /// <summary>
        /// すべてのアニメーションを停止します。
        /// </summary>
        public void Stop()
        {
            foreach (var child in Children)
            {
                child.Stop();
            }
        }

        /// <summary>
        /// フレーム毎に呼ばれ、オブジェクトの更新処理を行います。
        /// </summary>
        public void DoEnterFrame(TimeSpan elapsedTime)
        {
            foreach (var child in Children)
            {
                child.DoEnterFrame(elapsedTime);
            }
        }
    }
}
