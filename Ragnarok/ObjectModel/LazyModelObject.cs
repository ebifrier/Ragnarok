using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// <see cref="ILazyModel"/>が使用するオブジェクトです。
    /// </summary>
    public sealed class LazyModelObject
    {
        private readonly object SyncRoot = new object();
        private readonly List<string> updatedPropertyList = new List<string>();
        private int count;

        /// <summary>
        /// Exitするときに呼ばれるイベントです。
        /// </summary>
        public event EventHandler FiresOnExit;

        /// <summary>
        /// ロックします。
        /// </summary>
        public void Enter(ILazyModel model)
        {
            lock (SyncRoot)
            {
                this.count += 1;
            }
        }

        /// <summary>
        /// ロック解除します。
        /// </summary>
        public void Exit(ILazyModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            lock (SyncRoot)
            {
                if (this.count == 0)
                {
                    throw new InvalidOperationException(
                        "ロックカウントが０なのでロック解除できません。");
                }

                this.count -= 1;
            }

            RaisePropertyChangedIfNeed(model);
        }

        /// <summary>
        /// もし可能であれば、変更されたプロパティの通知を送ります。
        /// </summary>
        internal void RaisePropertyChangedIfNeed(ILazyModel model)
        {
            string[] propertyArray = null;

            lock (SyncRoot)
            {
                // ロックされているなら、プロパティの変更通知は送りません。
                if (this.count > 0)
                {
                    return;
                }

                if (this.updatedPropertyList.Any())
                {
                    // updatedPropertyListはこの後クリアされるため、
                    // 配列オブジェクトを作っています。
                    propertyArray = this.updatedPropertyList
                        .Distinct().ToArray();

                    this.updatedPropertyList.Clear();
                }
            }

            // 変更されたプロパティがあれば、変更通知を送ります。
            if (propertyArray != null)
            {
                NotifyPropertyChanged(model, propertyArray);
            }

            // イベントを発火します。
            var handler = Interlocked.Exchange(ref FiresOnExit, null);
            if (handler != null)
            {
                Util.SafeCall(() => handler(model, EventArgs.Empty));
            }
        }

        /// <summary>
        /// 与えられた全プロパティの更新通知を出します。
        /// </summary>
        private void NotifyPropertyChanged(ILazyModel model,
                                           IEnumerable<string> propertyList)
        {
            foreach (var propertyName in propertyList)
            {
                model.NotifyPropertyChanged(
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 変更されたプロパティをリストに追加します。
        /// </summary>
        internal void AddChangedProperty(string propertyName)
        {
            lock (SyncRoot)
            {
                this.updatedPropertyList.Add(propertyName);
            }
        }
    }
}
