#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NUnit.Framework;

namespace Ragnarok.ObjectModel.Tests
{
    internal class PropertyChangedCounter : NotifyObject
    {
        private readonly Dictionary<string, int> countDic =
            new Dictionary<string, int>();

        /// <summary>
        /// 各プロパティの変更通知が何回呼ばれたか取得します。
        /// </summary>
        public int this[string key]
        {
            get
            {
                var count = 0;
                if (this.countDic.TryGetValue(key, out count))
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
            var count = 0;
            if (this.countDic.TryGetValue(e.PropertyName, out count))
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
