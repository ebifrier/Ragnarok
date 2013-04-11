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

namespace FlintSharp.Emitters
{
    /// <summary>
    /// The base collection class which can hook the add/remove actions.
    /// </summary>
    public class SortedCollection<T> : GeneralCollection<T>
        where T : IObjectBase
    {
        private readonly Emitter m_emitter;
        private Comparison<T> m_comparison;

        private static int MyComparison(T x, T y)
        {
            return (x.GetDefaultPriority() - y.GetDefaultPriority());
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SortedCollection(Emitter emitter)
        {
            m_emitter = emitter;
            m_comparison = MyComparison;
        }

        /// <summary>
        /// 比較メソッドを取得または設定します。
        /// </summary>
        public Comparison<T> Comparison
        {
            get { return m_comparison; }
            set { m_comparison = value; }
        }

        /// <summary>
        /// 指定された要素を追加します。
        /// </summary>
        protected override void OnAdded(int index, T element)
        {
            int i = 0;
            for (i = 0; i < m_implList.Count; i++)
                if (m_comparison(m_implList[i], element) < 0)
                    break;

            // indexは無視します。
            m_implList.Insert(i, element);

            if (element != null)
                element.AddedToEmitter(m_emitter);
        }

        /// <summary>
        /// 指定したインデックス位置にある要素を削除します。
        /// </summary>
        protected override void OnRemoved(int index, T element)
        {
            m_implList.RemoveAt(index);

            if (element != null)
                element.RemovedFromEmitter(m_emitter);
        }
    }
}
