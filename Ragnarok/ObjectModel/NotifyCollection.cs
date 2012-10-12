using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// 変更通知をUIスレッド上で出す ObservableCollection です。
    /// </summary>
    public class NotifyCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// コレクションの変更を通知します。
        /// </summary>
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        protected override event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// コレクションの変更通知を出します。
        /// </summary>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (BlockReentrancy())
            {
                Util.CallCollectionChanged(
                    CollectionChanged, this, e);
            }
        }

        /// <summary>
        /// プロパティの変更通知を出します。
        /// </summary>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Util.CallPropertyChanged(
                PropertyChanged, this, e);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NotifyCollection(IEnumerable<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NotifyCollection()
        {
        }
    }
}
