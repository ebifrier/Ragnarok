using System;
using System.Collections.Generic;
using System.Text;
namespace Codehaus.Parsec
{
    public delegate bool FromToken<T>(Tok tok, ref T result);
    public delegate To Map<From, To>(From from);
    public delegate To Map<A, B, To>(A a, B b);
    public delegate To Map<A, B, C, To>(A a, B b, C c);
    public delegate To Map<A, B, C, D, To>(A a, B b, C c, D d);
    public delegate To Map<A, B, C, D, E, To>(A a, B b, C c, D d, E e);
    public delegate To Map<To>(params object[] args);
    public delegate To Mapn<From, To>(params From[] args);
    public delegate bool Predicate<T>(T t);
    public delegate T[] ArrayFactory<T>(int len);
    public delegate R FromRange<T, R>(int from, int len, T data);
    public delegate R FromRange<A, B, R>(int from, int len, A a, B b);
    public delegate R FromRange<A, B, C, R>(int from, int len, A a, B b, C c);
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
    public delegate void Proc<A, B>(A a, B b);
    public delegate void Proc<A, B, C>(A a, B b, C c);
    public delegate void Proc<A, B, C, D>(A a, B b, C c, D d);
    public delegate void Proc<A, B, C, D, E>(A a, B b, C c, D d, E e);
    public delegate Parser<T> Catch<T>(T val, object e);
    public delegate T Generator<T>();
    public class Tuple
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
    public struct Pair<A, B>
    {
        A a;
        B b;
        public A V1
        {
            get { return a; }
            set { a = value; }
        }
        public B V2
        {
            get { return b; }
            set { b = value; }
        }
        public Pair(A a, B b)
        {
            this.a = a;
            this.b = b;
        }
    }
    public struct Tuple<A, B, C>
    {
        A a;
        B b;
        C c;
        public A V1
        {
            get { return a; }
            set { a = value; }
        }
        public B V2
        {
            get { return b; }
            set { b = value; }
        }
        public C V3
        {
            get { return c; }
            set { c = value; }
        }
        public Tuple(A a, B b, C c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }
    public struct Tuple<A, B, C, D>
    {
        A a;
        B b;
        C c;
        D d;
        public A V1
        {
            get { return a; }
            set { a = value; }
        }
        public B V2
        {
            get { return b; }
            set { b = value; }
        }
        public C V3
        {
            get { return c; }
            set { c = value; }
        }
        public D V4
        {
            get { return d; }
            set { d = value; }
        }
        public Tuple(A a, B b, C c, D d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
    }
    public struct Tuple<A, B, C, D, E>
    {
        A a;
        B b;
        C c;
        D d;
        E e;
        public A V1
        {
            get { return a; }
            set { a = value; }
        }
        public B V2
        {
            get { return b; }
            set { b = value; }
        }
        public C V3
        {
            get { return c; }
            set { c = value; }
        }
        public D V4
        {
            get { return d; }
            set { d = value; }
        }
        public E V5
        {
            get { return e; }
            set { e = value; }
        }
        public Tuple(A a, B b, C c, D d, E e)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
        }
    }
    public class Traces
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
    public class Functors
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
    public class Maps
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
