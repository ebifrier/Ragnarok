using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// バッファとそのサイズなどを保持します。
    /// </summary>
    public class DataSegment<T>
    {
        /// <summary>
        /// バッファオブジェクトを取得します。
        /// </summary>
        public T[] Array
        {
            get;
            private set;
        }

        /// <summary>
        /// バッファサイズを取得します。
        /// </summary>
        public int Count
        {
            get;
            private set;
        }

        /// <summary>
        /// バッファの先頭インデックスを取得します。
        /// </summary>
        public int Offset
        {
            get;
            private set;
        }

        /// <summary>
        /// バッファの先頭オフセットを進めます。
        /// </summary>
        public void Increment(int inc)
        {
            inc = MathEx.Between(-Offset, Count, inc);

            // 先頭オフセットを移動し、バッファサイズは減らします。
            Offset += inc;
            Count -= inc;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataSegment(T[] array)
        {
            Array = array;
            Count = (array != null ? array.Length : 0);
            Offset = 0;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataSegment(T[] array, int offset, int count)
        {
            Array = array;
            Count = count;
            Offset = offset;
        }

        /// <summary>
        /// 比較オペレータ
        /// </summary>
        public static bool operator ==(DataSegment<T> a, DataSegment<T> b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 比較オペレータ
        /// </summary>
        public static bool operator !=(DataSegment<T> a, DataSegment<T> b)
        {
            return !(a == b);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result != null)
            {
                return result.Value;
            }

            return Equals((ArraySegment<T>)obj);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public bool Equals(ArraySegment<T> obj)
        {
            return (
                Array == obj.Array &&
                Count == obj.Count &&
                Offset == obj.Offset);
        }

        /// <summary>
        /// 現在のインスタンスのハッシュ コードを返します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                (Array != null ? Array.GetHashCode() : 0) ^
                Count.GetHashCode() ^
                Offset.GetHashCode());
        }
    }
}
