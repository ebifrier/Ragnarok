using System;
using System.Collections.Generic;
using System.Text;
namespace Codehaus.Parsec
{
    public delegate bool FromToken<T>(Tok tok, ref T result);
    public delegate TTo Map<TFrom, TTo>(TFrom from);
    public delegate TTo Map<TA, TB, TTo>(TA a, TB b);
    public delegate TTo Map<TA, TB, TC, TTo>(TA a, TB b, TC c);
    public delegate TTo Map<TA, TB, TC, TD, TTo>(TA a, TB b, TC c, TD d);
    public delegate TTo Map<TA, TB, TC, TD, TE, TTo>(TA a, TB b, TC c, TD d, TE e);
    public delegate TTo Map<TTo>(params object[] args);
    public delegate TTo Mapn<TFrom, TTo>(params TFrom[] args);
    public delegate bool Predicate<T>(T t);
    public delegate T[] ArrayFactory<T>(int len);
    public delegate TR FromRange<T, TR>(int from, int len, T data);
    public delegate TR FromRange<TA, TB, TR>(int from, int len, TA a, TB b);
    public delegate TR FromRange<TA, TB, TC, TR>(int from, int len, TA a, TB b, TC c);
    /// <summary>
    /// Used to trace a parser.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ok">whether the parser succeeded.</param>
    /// <param name="result">the parser result if succeeded.</param>
    /// <param name="except">pseudo exception.</param>
    /// <param name="src">the text being parsed.</param>
    /// <param name="index">the index where the parser terminates.</param>
    /// <param name="steps">logical steps consumed by the parser.</param>
    /// <param name="offset">physical offset consumed by the parser.</param>
    internal delegate void Tracer<T>(bool ok, T result, object except,
      string src, int index, int steps, int offset);
    /// <summary>
    /// Used to trace a parser when it fails.
    /// </summary>
    /// <param name="except">the pseudo exception.</param>
    /// <param name="src">the text being parsed.</param>
    /// <param name="index">the index where the error happens.</param>
    /// <param name="steps">the logical steps consumed.</param>
    /// <param name="offset">the physical offset consumed.</param>
    public delegate void ErrorTrace(object except,
      string src, int index, int steps, int offset);
    /// <summary>
    /// Used to trace a parser when it succeeds.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result">the parser result.</param>
    /// <param name="src">the text being parsed.</param>
    /// <param name="index">the index where the parser succeeds.</param>
    /// <param name="steps">the logical steps consumed.</param>
    /// <param name="offset">the physical offset consumed.</param>
    public delegate void GoodTrace<T>(T result,
      string src, int index, int steps, int offset);


    public delegate Parser<To> Binder<From, To>(From from);
    public delegate void Proc();
    public delegate void Proc<T>(T t);
    public delegate void Proc<TA, TB>(TA a, TB b);
    public delegate void Proc<TA, TB, TC>(TA a, TB b, TC c);
    public delegate void Proc<TA, TB, TC, TD>(TA a, TB b, TC c, TD d);
    public delegate void Proc<TA, TB, TC, TD, TE>(TA a, TB b, TC c, TD d, TE e);
    public delegate Parser<T> Catch<T>(T val, object e);
    public delegate T Generator<T>();
    public static class Tuple
    {
        public static T Create<T>(T v)
        {
            return v;
        }
        public static Pair<A, B> Create<A, B>(A a, B b)
        {
            return new Pair<A, B>(a, b);
        }
        public static Tuple<A, B, C> Create<A, B, C>(A a, B b, C c)
        {
            return new Tuple<A, B, C>(a, b, c);
        }
        public static Tuple<A, B, C, D> Create<A, B, C, D>(A a, B b, C c, D d)
        {
            return new Tuple<A, B, C, D>(a, b, c, d);
        }
        public static Tuple<A, B, C, D, E> Create<A, B, C, D, E>(A a, B b, C c, D d, E e)
        {
            return new Tuple<A, B, C, D, E>(a, b, c, d, e);
        }
    }
    public struct Pair<A, B> : IEquatable<Pair<A, B>>
    {
        public A V1
        {
            get;
            set;
        }
        public B V2
        {
            get;
            set;
        }
        public Pair(A a, B b)
        {
            this.V1 = a;
            this.V2 = b;
        }

        public override bool Equals(object obj)
        {
            return Equals((Pair<A, B>)obj);
        }
        public bool Equals(Pair<A, B> other)
        {
            return V1.Equals(other.V1) && V2.Equals(other.V2);
        }
        public static bool operator ==(Pair<A, B> x, Pair<A, B> y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(Pair<A, B> x, Pair<A, B> y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return V1.GetHashCode() ^ V2.GetHashCode();
        }
    }
    public struct Tuple<A, B, C> : IEquatable<Tuple<A, B, C>>
    {
        public A V1
        {
            get;
            set;
        }
        public B V2
        {
            get;
            set;
        }
        public C V3
        {
            get;
            set;
        }
        public Tuple(A a, B b, C c)
        {
            this.V1 = a;
            this.V2 = b;
            this.V3 = c;
        }

        public override bool Equals(object obj)
        {
            return Equals((Tuple<A, B, C>)obj);
        }
        public bool Equals(Tuple<A, B, C> other)
        {
            return V1.Equals(other.V1) && V2.Equals(other.V2) && V3.Equals(other.V3);
        }
        public static bool operator ==(Tuple<A, B, C> x, Tuple<A, B, C> y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(Tuple<A, B, C> x, Tuple<A, B, C> y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return V1.GetHashCode() ^ V2.GetHashCode() ^ V3.GetHashCode();
        }
    }
    public struct Tuple<A, B, C, D> : IEquatable<Tuple<A, B, C, D>>
    {
        public A V1
        {
            get;
            set;
        }
        public B V2
        {
            get;
            set;
        }
        public C V3
        {
            get;
            set;
        }
        public D V4
        {
            get;
            set;
        }
        public Tuple(A a, B b, C c, D d)
        {
            this.V1 = a;
            this.V2 = b;
            this.V3 = c;
            this.V4 = d;
        }

        public override bool Equals(object obj)
        {
            return Equals((Tuple<A, B, C, D>)obj);
        }
        public bool Equals(Tuple<A, B, C, D> other)
        {
            return V1.Equals(other.V1) && V2.Equals(other.V2) &&
                   V3.Equals(other.V3) && V4.Equals(other.V4);
        }
        public static bool operator ==(Tuple<A, B, C, D> x, Tuple<A, B, C, D> y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(Tuple<A, B, C, D> x, Tuple<A, B, C, D> y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return V1.GetHashCode() ^ V2.GetHashCode() ^
                   V3.GetHashCode() ^ V4.GetHashCode();
        }
    }
    public struct Tuple<A, B, C, D, E> : IEquatable<Tuple<A, B, C, D, E>>
    {
        public A V1
        {
            get;
            set;
        }
        public B V2
        {
            get;
            set;
        }
        public C V3
        {
            get;
            set;
        }
        public D V4
        {
            get;
            set;
        }
        public E V5
        {
            get;
            set;
        }
        public Tuple(A a, B b, C c, D d, E e)
        {
            this.V1 = a;
            this.V2 = b;
            this.V3 = c;
            this.V4 = d;
            this.V5 = e;
        }

        public override bool Equals(object obj)
        {
            return Equals((Tuple<A, B, C, D, E>)obj);
        }
        public bool Equals(Tuple<A, B, C, D, E> other)
        {
            return V1.Equals(other.V1) && V2.Equals(other.V2) &&
                   V3.Equals(other.V3) && V4.Equals(other.V4) &&
                   V5.Equals(other.V5);
        }
        public static bool operator ==(Tuple<A, B, C, D, E> x, Tuple<A, B, C, D, E> y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(Tuple<A, B, C, D, E> x, Tuple<A, B, C, D, E> y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return V1.GetHashCode() ^ V2.GetHashCode() ^
                   V3.GetHashCode() ^ V4.GetHashCode() ^ V5.GetHashCode();
        }
    }
    public static class Traces
    {
        const int LDEADING = 32;
        static string getLeadingChars(string src, int ind, int n)
        {
            int len = src.Length;
            if (ind >= len)
            {
                return "<EOF>";
            }
            if (n + ind >= len)
            {
                n = len - ind;
                return src.Substring(ind, n);
            }
            return src.Substring(ind, n) + "...";
        }
        /// <summary>
        /// Create an ErrorTrace object that prints error message to output.
        /// </summary>
        /// <param name="name">the name in the trace message.</param>
        /// <param name="writer">the writer for the output.</param>
        /// <param name="min_steps">the minimal logical steps to trigger the trace message.</param>
        /// <returns>the ErrorTrace object.</returns>
        public static ErrorTrace PrintError(string name, System.IO.TextWriter writer, int min_steps)
        {
            return delegate(object exception, string src, int ind, int steps, int offset)
            {
                if (steps < min_steps) return;
                writer.Write("{0}: ", name);
                if (exception != null)
                {
                    writer.WriteLine("exception raised.");
                }
                writer.WriteLine("[{0}]", getLeadingChars(src, ind, LDEADING));
                writer.WriteLine("steps={0}, offset={1}", steps, offset);
            };
        }
        /// <summary>
        /// Create a GoodTrace object that prints parse result to output.
        /// </summary>
        /// <param name="name">the name in the trace message.</param>
        /// <param name="writer">the writer for the output.</param>
        /// <returns>the GoodTrace object.</returns>
        public static GoodTrace<T> PrintResult<T>(string name, System.IO.TextWriter writer)
        {
            return delegate(T result, string src, int ind, int steps, int offset)
            {
                writer.WriteLine("{0} => {1}", name, result);
                writer.WriteLine("[{0}]", getLeadingChars(src, ind, LDEADING));
                writer.WriteLine("steps={0}, offset={1}", steps, offset);
            };
        }
    }
    public static class Functors
    {
        public static ArrayFactory<T> getSimpleArrayFactory<T>()
        {
            return delegate(int n)
            {
                return new T[n];
            };
        }
        public static Generator<Accumulator<T, T[]>> ToArrayAccumulatable<T>(ArrayFactory<T> factory)
        {
            return delegate()
            {
                return new ArrayAccumulator<T>(factory, new System.Collections.Generic.List<T>());
            };
        }
        public static Generator<Accumulator<T, T[]>> ToArrayAccumulatable<T>(
            T initial_value, ArrayFactory<T> factory)
        {
            return delegate()
            {
                List<T> list = new System.Collections.Generic.List<T>();
                list.Add(initial_value);
                return new ArrayAccumulator<T>(factory, list);
            };
        }
        static readonly FromToken<object> any_token = delegate(Tok tok, ref object result)
        {
            result = tok.Token;
            return true;
        };
        public static FromToken<object> AnyToken
        {
            get { return any_token; }
        }
        public static Map<From, To> AsMap<From, To>(System.Collections.IDictionary dict) where To : class
        {
            return delegate(From key)
            {
                return dict[key] as To;
            };
        }
    }
    public static class Maps
    {
        public static string ToString(object v)
        {
            return v == null ? "<null>" : v.ToString();
        }
        public static readonly Map<object, string> ToStringMap =
            new Map<object, string>(ToString);
        static readonly Map<object, object> object_id = delegate(object v) { return v; };
        public static Map<object, object> Identity
        {
            get { return object_id; }
        }
        public static Map<T, T> Id<T>()
        {
            return delegate(T v) { return v; };
        }
        public static Map<A, B, Pair<A, B>> ToPair<A, B>()
        {
            return delegate(A a, B b) { return Tuple.Create(a, b); };
        }
        public static Map<A, B, C, Tuple<A, B, C>> ToTuple<A, B, C>()
        {
            return delegate(A a, B b, C c) { return Tuple.Create(a, b, c); };
        }
        public static Map<A, B, C, D, Tuple<A, B, C, D>> ToTuple<A, B, C, D>()
        {
            return delegate(A a, B b, C c, D d) { return Tuple.Create(a, b, c, d); };
        }
        public static Map<A, B, C, D, E, Tuple<A, B, C, D, E>> ToTuple<A, B, C, D, E>()
        {
            return delegate(A a, B b, C c, D d, E e) { return Tuple.Create(a, b, c, d, e); };
        }
    }
    class IntOrders
    {
        public static Map<int, int, bool> Less()
        {
            return delegate(int a, int b)
            {
                return a < b;
            };
        }
        public static Map<int, int, bool> Greater()
        {
            return delegate(int a, int b)
            {
                return a > b;
            };
        }
        public static Map<int, int, bool> LessEqual()
        {
            return delegate(int a, int b)
            {
                return a <= b;
            };
        }
        public static Map<int, int, bool> GreaterEqual()
        {
            return delegate(int a, int b)
            {
                return a >= b;
            };
        }
    }
}
