/*
 * FLINT PARTICLE SYSTEM
 * .....................
 * 
 * Author: Richard Lord (Big Room)
 * C# Port: Ben Baker (HeadSoft)
 * Copyright (c) Big Room Ventures Ltd. 2008
 * http://flintparticles.org
 * 
 * 
 * Licence Agreement
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlintSharp
{
    /// <summary>
    /// The base collection class which can hook the add/remove actions.
    /// </summary>
    public class GeneralCollection<T> :
        IList<T>, ICollection<T>, IEnumerable<T>,
        IList, ICollection, IEnumerable
    {
        protected readonly List<T> m_implList = new List<T>();

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
            get { return m_implList.Capacity; }
            set { m_implList.Capacity = value; }
        }

        /// <summary>
        /// コレクション内の要素の実際の数を取得します。
        /// </summary>
        public int Count
        {
            get { return m_implList.Count; }
        }

        /// <summary>
        /// 0 から始まるインデックス位置に格納されている要素を取得または設定します。
        /// </summary>
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        /// <summary>
        /// 0 から始まるインデックス位置に格納されている要素を取得または設定します。
        /// </summary>
        public T this[int index]
        {
            get { return this.m_implList[index]; }
            set
            {
                throw new NotImplementedException();

                /*OnRemoved(this.m_implList[index]);
                this.m_implList[index] = value;
                OnAdded(value);*/
            }
        }

        /// <summary>
        /// すべての要素を削除します。
        /// </summary>
        public void Clear()
        {
            while (m_implList.Any())
            {
                RemoveAt(m_implList.Count - 1);
            }
        }

        /// <summary>
        /// 指定された要素がコレクション内に存在するかどうかを判定します。
        /// </summary>
        bool IList.Contains(object element)
        {
            return Contains((T)element);
        }

        /// <summary>
        /// 指定された要素がコレクション内に存在するかどうかを判定します。
        /// </summary>
        public bool Contains(T element)
        {
            return m_implList.Contains(element);
        }

        /// <summary>
        /// 指定されたインデックス位置を開始位置として、配列に各要素をコピーします。
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            foreach (var v in m_implList)
            {
                array.SetValue(v, index);
                index = index + 1;
            }
        }

        /// <summary>
        /// 指定されたインデックス位置を開始位置として、配列に各要素をコピーします。
        /// </summary>
        public void CopyTo(T[] array, int index)
        {
            CopyTo((Array)array, index);
        }

        /// <summary>
        /// 反復処理できる列挙子を返します。
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_implList.GetEnumerator();
        }

        /// <summary>
        /// 反復処理できる列挙子を返します。
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return m_implList.GetEnumerator();
        }

        /// <summary>
        /// 指定された要素を追加します。
        /// </summary>
        int IList.Add(object element)
        {
            Add((T)element);

            return (m_implList.Count - 1);
        }

        /// <summary>
        /// 指定された要素を追加します。
        /// </summary>
        public void Add(T element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            //m_implList.Insert(i, element);
            OnAdded(m_implList.Count, element);
        }

        /// <summary>
        /// 指定された要素のインデックス位置を返します。
        /// </summary>
        int IList.IndexOf(object element)
        {
            return IndexOf((T)element);
        }

        /// <summary>
        /// 指定された要素のインデックス位置を返します。
        /// </summary>
        public int IndexOf(T element)
        {
            return m_implList.IndexOf(element);
        }

        /// <summary>
        /// 指定されたインデックス位置に要素を挿入します。
        /// </summary>
        void IList.Insert(int index, object element)
        {
            Insert(index, (T)element);
        }

        /// <summary>
        /// 指定されたインデックス位置に要素を挿入します。
        /// </summary>
        public void Insert(int index, T element)
        {
            //this.m_implList.Insert(index, element);
            OnAdded(index, element);
        }

        /// <summary>
        /// 指定したインデックス位置にある要素を削除します。
        /// </summary>
        public void RemoveAt(int index)
        {
            var item = m_implList[index];

            OnRemoved(index, item);
        }

        /// <summary>
        /// 指定された要素を削除します
        /// </summary>
        void IList.Remove(object element)
        {
            Remove((T)element);
        }

        /// <summary>
        /// 指定された要素を削除します
        /// </summary>
        public bool Remove(T element)
        {
            var index = IndexOf(element);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
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
        protected virtual void OnAdded(int index, T element)
        {
        }

        /// <summary>
        /// 指定したインデックス位置にある要素を削除します。
        /// </summary>
        protected virtual void OnRemoved(int index, T element)
        {
        }
    }
}
