using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// キャッシュ可能なオブジェクトの基本インターフェースです。
    /// </summary>
    public interface ICachable
    {
        /// <summary>
        /// キャッシュ用のオブジェクトサイズをbyte単位で取得します。
        /// </summary>
        long ObjectSize { get; }
    }

    /// <summary>
    /// キャッシュの管理を行うためのオブジェクトです。
    /// </summary>
    /// <remarks>
    /// アルゴリズムは'Least Recently Used'(LRU)を用いており
    /// 登録されたオブジェクトのサイズが一定値を越えたら、
    /// もっとも使われていないデータを捨てるようにしています。
    /// </remarks>
    public class CacheManager<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TValue: ICachable
    {
        /// <summary>
        /// キャッシュ用のキーとデータを保持します。
        /// </summary>
        private sealed class CacheData
        {
            public CacheData(TKey key, TValue value, long size)
            {
                Key = key;
                Value = value;
                ObjectSize = size;
            }
            public TKey Key;
            public TValue Value;
            public long ObjectSize;
        }

        /// <summary>
        /// キーから値を作るためのメソッド型です。
        /// </summary>
        public delegate TValue CreatorFunc(TKey key);

        private readonly object syncRoot = new object();
        private readonly LinkedList<CacheData> usageList;
        private readonly Dictionary<TKey, LinkedListNode<CacheData>> dic;
        private readonly IEqualityComparer<TKey> comparer;
        private readonly CreatorFunc creator;
        private long capacity;
        private long cacheSize;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CacheManager(CreatorFunc creator, long capacity,
                            IEqualityComparer<TKey> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }

            if (capacity < 0)
            {
                throw new ArgumentException("capacity");
            }

            this.usageList = new LinkedList<CacheData>();
            this.dic = new Dictionary<TKey, LinkedListNode<CacheData>>(comparer);
            this.comparer = comparer;
            this.creator = creator;
            this.capacity = capacity;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CacheManager(CreatorFunc creator, long capacity)
            : this(creator, capacity, EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>
        /// 同期用のオブジェクトを取得します。
        /// </summary>
        public object SyncRoot
        {
            get { return this.syncRoot; }
        }

        /// <summary>
        /// オブジェクトの作成用関数を取得します。
        /// </summary>
        public CreatorFunc Creator
        {
            get { return this.creator; }
        }

        /// <summary>
        /// キーの比較用オブジェクトを取得します。
        /// </summary>
        public IEqualityComparer<TKey> Comparer
        {
            get { return this.comparer; }
        }

        /// <summary>
        /// オブジェクトの破棄を行わないキャッシュ容量を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 作成したオブジェクトがこのサイズを超えたら、
        /// 使用されていないオブジェクトの破棄を開始します。
        /// 
        /// また、Capacity=0の時はオブジェクトの破棄を行いません。
        /// </remarks>
        public long Capacity
        {
            get { return this.capacity; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException("value");
                }

                this.capacity = value;
            }
        }

        /// <summary>
        /// 現在キャッシュに含まれているオブジェクトの合計サイズを取得します。
        /// </summary>
        public long CacheSize
        {
            get { return this.cacheSize; }
        }

        /// <summary>
        /// キャッシュされているオブジェクトの数を取得します。
        /// </summary>
        public int Count
        {
            get { return this.usageList.Count(); }
        }

        /// <summary>
        /// IEnumerableオブジェクトを取得します。
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (this.syncRoot)
            {
                // dicが変わるかもしれないので、一応配列にして返します。
                return this.dic
                    .Select(_ => _.Value.Value)
                    .Select(_ => new KeyValuePair<TKey, TValue>(_.Key, _.Value))
                    .ToList()
                    .GetEnumerator();
            }
        }

        /// <summary>
        /// IEnumerableオブジェクトを取得します。
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// キャッシュ内容をすべてクリアします。
        /// </summary>
        public void Clear()
        {
            lock (this.syncRoot)
            {
                this.dic.Clear();
                this.usageList.Clear();
                this.cacheSize = 0;
            }
        }

        /// <summary>
        /// 指定のキーを持つオブジェクトが含まれているか調べます。
        /// </summary>
        public bool Contains(TKey key)
        {
            lock (this.syncRoot)
            {
                return this.dic.ContainsKey(key);
            }
        }

        /// <summary>
        /// キャッシュ容量をチェックし、もしオーバーしていたら
        /// 使われていないオブジェクトの破棄をおこないます。
        /// </summary>
        private void ControlCache()
        {
            lock (this.syncRoot)
            {
                while (true)
                {
                    // Capacity=0の時は削除を行いません。
                    if (CacheSize < Capacity || Capacity == 0)
                    {
                        return;
                    }

                    // オブジェクトが１つしかない時も削除を行いません。
                    if (this.usageList.Count <= 1)
                    {
                        return;
                    }

                    var oldest = this.usageList.Last;
                    if (!Remove(oldest.Value.Key))
                    {
                        // これどうしようか。。。
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// キャッシュにオブジェクトの追加を行います。
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var size = value.ObjectSize;
            if (size < 0)
            {
                throw new ArgumentException(
                    "オブジェクトサイズが負数です。");
            }

            lock (this.syncRoot)
            {
                LinkedListNode<CacheData> node;
                if (this.dic.TryGetValue(key, out node))
                {
                    this.cacheSize -= node.Value.ObjectSize;
                    this.usageList.Remove(node);
                }

                // 新しいオブジェクトの追加
                var data = new CacheData(key, value, size);
                this.dic[key] = this.usageList.AddFirst(data);
                this.cacheSize += size;

                // 先に古いオブジェクトの削除など
                ControlCache();
            }
        }

        /// <summary>
        /// 指定のキーを持つオブジェクトをキャッシュから削除します。
        /// </summary>
        public bool Remove(TKey key)
        {
            lock (this.syncRoot)
            {
                LinkedListNode<CacheData> node;
                if (this.dic.TryGetValue(key, out node))
                {
                    this.cacheSize -= node.Value.ObjectSize;

                    // オブジェクト要素を削除します。
                    this.usageList.Remove(node);
                    this.dic.Remove(key);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// オブジェクトを作成します。
        /// </summary>
        private TValue Create(TKey key)
        {
            var value = this.creator(key);
            if (value == null)
            {
                throw new InvalidOperationException(
                    "キャッシュ可能オブジェクトの作成に失敗しました。");
            }

            // 念のため確認しておく
            if (value.ObjectSize < 0)
            {
                throw new InvalidOperationException(
                    "キャッシュ用のオブジェクトサイズが負の値です。");
            }

            return value;
        }

        /// <summary>
        /// キャッシュからオブジェクトを取得し、もしなければ作成します。
        /// </summary>
        public TValue GetOrCreate(TKey key)
        {
            lock (this.syncRoot)
            {
                // オブジェクトをキャッシュから探し、
                // もしあればそれを返します。
                LinkedListNode<CacheData> node;
                if (this.dic.TryGetValue(key, out node))
                {
                    var value = node.Value; // nodeの中身が変わるかもしれないので

                    // 使用リストだけ更新しておきます。
                    this.usageList.Remove(node);
                    this.dic[key] = this.usageList.AddFirst(value);
                    return value.Value;
                }
                else
                {
                    // オブジェクトがキャッシュになかったので、
                    // 新たに作成します。
                    var value = Create(key);

                    // キャッシュに追加します。
                    Add(key, value);
                    return value;
                }
            }
        }
    }
}
