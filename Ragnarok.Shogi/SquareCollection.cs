using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 複数のエンティティを管理するクラスで追加・削除時に親も設定します。
    /// </summary>
    [TypeConverter(typeof(SquareCollectionConverter))]
    public class SquareCollection :
        IList<Square>, ICollection<Square>, IEnumerable<Square>,
        /*IList, ICollection,*/ IEnumerable
    {
        private readonly List<Square> implList = new List<Square>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SquareCollection()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SquareCollection(IEnumerable<Square> elements)
        {
            AddRange(elements);
        }
        
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
        public Square this[int index]
        {
            get { return this.implList[index]; }
            set { this.implList[index] = value; }
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
        public bool Contains(Square element)
        {
            return this.implList.Contains(element);
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
        public void CopyTo(Square[] array, int index)
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
        public IEnumerator<Square> GetEnumerator()
        {
            return this.implList.GetEnumerator();
        }

        /// <summary>
        /// 指定された要素のインデックス位置を返します。
        /// </summary>
        public int IndexOf(Square element)
        {
            return this.implList.IndexOf(element);
        }

        /// <summary>
        /// 指定されたインデックス位置に要素を挿入します。
        /// </summary>
        public void Insert(int index, Square element)
        {
            this.implList.Insert(index, element);
        }

        /// <summary>
        /// 指定された要素を削除します
        /// </summary>
        public bool Remove(Square element)
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
        /// 指定したインデックス位置にある要素を削除します。
        /// </summary>
        public void RemoveAt(int index)
        {
            var item = this.implList[index];

            this.implList.RemoveAt(index);
        }

        /// <summary>
        /// 指定された要素を追加します。
        /// </summary>
        public void Add(Square element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this.implList.Add(element);
        }

        /// <summary>
        /// 指定された要素列を追加します。
        /// </summary>
        public void AddRange(IEnumerable<Square> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            this.implList.AddRange(elements);
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        /// <remarks>
        /// フォーマットは
        /// 34 55 67,...
        /// のように筋段の組が' 'を区切りとして並んでいます。
        /// </remarks>
        public override string ToString()
        {
            return string.Join(
                " ",
                this.implList.Select(_ => _.ToString()).ToArray());
        }

        /// <summary>
        /// SquareCollectionを文字列から作成します。
        /// </summary>
        /// <remarks>
        /// 受け入れるフォーマットはToStringが生成したものと同じです。
        /// </remarks>
        public static SquareCollection Parse(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var list = source.Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
            if (list == null || !list.Any())
            {
                return new SquareCollection();
            }

            var result = new SquareCollection();
            list.Select(_ => Square.Parse(_))
                .ForEach(_ => result.Add(_));
            return result;
        }
    }
}
