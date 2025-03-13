using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 重複したキーを許すDictionaryクラスです。 
    /// </summary>
    [Serializable()]
    public sealed class MultiMap<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    {
        /// <summary>
        /// キーと値の組を追加します。
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
            }

            HashSet<TValue> container = null;
            if (!base.TryGetValue(key, out container))
            {
                container = new HashSet<TValue>();
                base.Add(key, container);
            }

            container.Add(value);
        }

        /// <summary>
        /// そのキーと値の組を含むか調べます。
        /// </summary>
        public bool ContainsValue(TKey key, TValue value)
        {
            HashSet<TValue> values = null;
            var toReturn = false;

            if (base.TryGetValue(key, out values))
            {
                toReturn = values.Contains(value);
            }

            return toReturn;
        }

        /// <summary>
        /// キーと値の組を削除します。
        /// </summary>
        public void Remove(TKey key, TValue value)
        {
            if (key == null)
            {
            }

            HashSet<TValue> container = null;

            if (base.TryGetValue(key, out container))
            {
                container.Remove(value);
                if (container.Count <= 0)
                {
                    base.Remove(key);
                }
            }
        }

        /// <summary>
        /// 与えられたマップをこのマップに統合します。
        /// </summary>
        public void Merge(MultiMap<TKey, TValue> toMergeWith)
        {
            if (toMergeWith == null)
            {
                return;
            }

            foreach (var pair in toMergeWith)
            {
                foreach (TValue value in pair.Value)
                {
                    this.Add(pair.Key, value);
                }
            }
        }

        /// <summary>
        /// 指定のキーに登録された値をすべて取得します。
        /// </summary>
        public HashSet<TValue> GetValues(TKey key, bool returnEmptySet)
        {
            HashSet<TValue> toReturn = null;

            if (!base.TryGetValue(key, out toReturn) && returnEmptySet)
            {
                toReturn = new HashSet<TValue>();
            }

            return toReturn;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MultiMap(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MultiMap()
        {
        }
    }
}
