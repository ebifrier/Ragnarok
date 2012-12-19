using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    public class DataSegment<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T[] Array
        {
            get;
            private set;
        }

        public int Count
        {
            get;
            private set;
        }

        public int Offset
        {
            get;
            private set;
        }

        public int LeaveCount
        {
            get { return (Count - Offset); }
        }

        public void Increment(int offset)
        {
            Offset = Math.Min(Offset + offset, Count);
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

        public static bool operator ==(DataSegment<T> a, DataSegment<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DataSegment<T> a, DataSegment<T> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ArraySegment<T>))
            {
                return false;
            }

            return Equals((ArraySegment<T>)obj);
        }

        public bool Equals(ArraySegment<T> obj)
        {
            return (
                Array == obj.Array &&
                Count == obj.Count &&
                Offset == obj.Offset);
        }

        //
        // 概要:
        //     現在のインスタンスのハッシュ コードを返します。
        //
        // 戻り値:
        //     32 ビット符号付き整数ハッシュ コード。
        public override int GetHashCode()
        {
            return (
                (Array != null ? Array.GetHashCode() : 0) ^
                Count.GetHashCode() ^
                Offset.GetHashCode());
        }
    }
}
