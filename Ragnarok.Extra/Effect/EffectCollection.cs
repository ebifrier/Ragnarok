using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// 複数のエンティティを管理するクラスで追加・削除時に親も設定します。
    /// </summary>
    public class EffectCollection :
        IList<EffectObject>, ICollection<EffectObject>, IEnumerable<EffectObject>,
        IList, ICollection, IEnumerable
    {
        private readonly List<EffectObject> implList = new List<EffectObject>();
        private readonly EffectObject parent;

        /// <summary>
        /// インターフェイスへのアクセスが同期されている (スレッド セーフである) かどうかを示す値を取得します。
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// インターフェイスへのアクセスを同期するために使用できるオブジェクトを取得します。
        /// </summary>
        public object SyncRoot
        {
            get { return null; }
        }

        /// <summary>
        /// 読み込み専用かどうかを取得します。
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 固定サイズのコレクションかどうかを取得します。
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// コレクションに格納できる要素の数を取得または設定します。
        /// </summary>
        public int Capacity
        {
            get { return this.implList.Capacity; }
            set { this.implList.Capacity = value; }
        }

        /// <summary>
        /// コレクション内の要素の実際の数を取得します。
        /// </summary>
        public int Count
        {
            get { return this.implList.Count; }
        }

        /// <summary>
        /// 0 から始まるインデックス位置に格納されている要素を取得または設定します。
        /// </summary>
        public EffectObject this[int index]
        {
            get { return this.implList[index]; }
            set { this.implList[index] = value; }
        }

        object IList.this[int index]
        {
            get { return this.implList[index]; }
            set { this.implList[index] = (EffectObject)value; }
        }

        /// <summary>
        /// すべての要素を削除します。
        /// </summary>
        public void Clear()
        {
            while (this.implList.Any())
            {
                RemoveAt(this.implList.Count - 1);
            }
        }

        /// <summary>
        /// 指定された要素がコレクション内に存在するかどうかを判定します。
        /// </summary>
        public bool Contains(EffectObject element)
        {
            return this.implList.Contains(element);
        }

        bool IList.Contains(object element)
        {
            return Contains((EffectObject)element);
        }

        /// <summary>
        /// 指定されたインデックス位置を開始位置として、配列に各要素をコピーします。
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            Array.Copy(
                this.implList.ToArray(), 0,
                array, index,
                1);
        }

        /// <summary>
        /// 指定されたインデックス位置を開始位置として、配列に各要素をコピーします。
        /// </summary>
        public void CopyTo(EffectObject[] array, int index)
        {
            CopyTo((Array)array, index);
        }

        /// <summary>
        /// 反復処理できる列挙子を返します。
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.implList.GetEnumerator();
        }

        /// <summary>
        /// 反復処理できる列挙子を返します。
        /// </summary>
        public IEnumerator<EffectObject> GetEnumerator()
        {
            return this.implList.GetEnumerator();
        }

        /// <summary>
        /// 指定された要素のインデックス位置を返します。
        /// </summary>
        public int IndexOf(EffectObject element)
        {
            return this.implList.IndexOf(element);
        }

        int IList.IndexOf(object element)
        {
            return IndexOf((EffectObject)element);
        }

        /// <summary>
        /// 指定されたインデックス位置に要素を挿入します。
        /// </summary>
        public void Insert(int index, EffectObject element)
        {
            this.implList.Insert(index, element);
        }

        void IList.Insert(int index, object element)
        {
            Insert(index, (EffectObject)element);
        }

        /// <summary>
        /// 指定された要素を削除します
        /// </summary>
        public bool Remove(EffectObject element)
        {
            var index = IndexOf(element);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        void IList.Remove(object element)
        {
            Remove((EffectObject)element);
        }

        /// <summary>
        /// コレクションから要素の範囲を削除します。
        /// </summary>
        public void RemoveRange(int index, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                RemoveAt(index + i);
            }
        }

        /// <summary>
        /// 指定された要素を追加します。
        /// </summary>
        public void Add(EffectObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this.implList.Add(element);
            element.ParentAdded(this.parent);
        }

        int IList.Add(object element)
        {
            Add((EffectObject)element);
            return (Count - 1);
        }

        /// <summary>
        /// 指定したインデックス位置にある要素を削除します。
        /// </summary>
        public void RemoveAt(int index)
        {
            var item = this.implList[index];

            this.implList.RemoveAt(index);

            if (item != null)
            {
                item.ParentRemoved(this.parent);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectCollection(EffectObject parent)
        {
            this.parent = parent;
        }
    }
}
