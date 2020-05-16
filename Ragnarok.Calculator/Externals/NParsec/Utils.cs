namespace Codehaus.Parsec
{
    using System.Collections.Generic;
    public static class Misc
    {
        public static int Hashcode(object value)
        {
            return (value == null) ? 0 : value.GetHashCode();
        }
        public static bool AreEqual(object o1, object o2)
        {
            return o1 == null ? o2 == null : o1.Equals(o2);
        }
        internal static T[] Dedup<T>(T[] arr, IComparer<T> comparer)
        {
            if (arr == null || arr.Length <= 1)
                return arr;
            SortedDictionary<T, bool> index = new SortedDictionary<T, bool>(comparer);
            foreach (T k in arr)
            {
                index[k] = true;
            }
            T[] result = new T[index.Count];
            index.Keys.CopyTo(result, 0);
            return result;
        }
    }
    /*
    public struct Maybe
    {
      public static Maybe<T> value<T> (T v) {
        return new Maybe<T> (true, v);
      }
      public static Maybe<T> nil<T> () {
        return new Maybe<T> (false, default (T));
      }
    }
    public struct Maybe<T>
    {
      T val;
      readonly bool ok;
      public T Value {
        get { return val; }
        set { val = value; }
      }
      public bool Ok {
        get { return ok; }
      }
      public bool Failed {
        get { return !ok; }
      }
      internal Maybe(bool ok, T v){
        this.ok = ok;
        this.val = v;
      }
    }*/
    /// <summary> Used to accumulate objects.
    /// Parsers.ManyAccum() use Accumulator to collect return values.
    /// Can be parameterized as Accumulator&lt;T,R&gt;
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// 2004-11-12
    /// </author>
    public interface Accumulator<From, To>
    {
        /// <summary> gets the accumulated result.</summary>
        /// <returns> the result.
        /// </returns>
        To GetResult();
        /// <summary> accumulate one object into the result.</summary>
        /// <param name="obj">the object to be accumulated.
        /// </param>
        void Accumulate(From value);
    }
    public class ArrayAccumulator<T> : Accumulator<T, T[]>
    {
        readonly System.Collections.Generic.IList<T> list;
        readonly ArrayFactory<T> factory;
        public T[] GetResult()
        {
            T[] result = factory(list.Count);
            list.CopyTo(result, 0);
            return result;
        }

        public void Accumulate(T obj)
        {
            list.Add(obj);
        }
        public ArrayAccumulator(ArrayFactory<T> factory, System.Collections.Generic.IList<T> list)
        {
            this.factory = factory;
            this.list = list;
        }
    }
}
