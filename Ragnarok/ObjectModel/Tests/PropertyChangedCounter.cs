#if TESTS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Ragnarok.ObjectModel.Tests
{
    internal class PropertyChangedCounter : NotifyObject
    {
        private readonly Dictionary<string, int> countDic = new();

        /// <summary>
        /// 各プロパティの変更通知が何回呼ばれたか取得します。
        /// </summary>
        public int this[string key]
        {
            get
            {
                if (this.countDic.TryGetValue(key, out int count))
                {
                    return count;
                }

                return 0;
            }
        }

        public void ClearCount()
        {
            this.countDic.Clear();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.countDic.TryGetValue(e.PropertyName, out int count))
            {
                this.countDic[e.PropertyName] = count + 1;
            }
            else
            {
                this.countDic[e.PropertyName] = 1;
            }
        }
    }
}
#endif
