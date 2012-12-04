namespace Codehaus.Parsec
{
    using System.Collections.Generic;
    using System;
    using System.Collections;
    using Lexer = Parser<Tok>;
    using Scanner = Parser<D_>;
    /// <author>  Ben Yu
    /// 
    /// Dec 19, 2004
    /// </author>
    [Serializable]
    sealed class Operators
    {

        internal static WordsData instance(string[] names)
        {
            Hashtable operators = new Hashtable();
            string[] ops = sort(names);
            Lexer[] lxs = new Lexer[ops.Length];
            for (int i = 0; i < ops.Length; i++)
            {
                string s = ops[i];
                Scanner scanner = s.Length == 1 ? Scanners.IsChar(s[0]) : Scanners.IsString(s);
                object tok = Tokens.CreateReservedWordToken(s);
                operators[s] = tok;
                Lexer lx = Lexers.Lex(scanner, Tokenizers.ForValue(tok));
                lxs[i] = lx;
            }
            return new WordsData(Functors.AsMap<string, object>(operators), lxs);
        }
        static readonly IComparer<string> comparer = new LengthComparer();
        sealed class Suite : IEnumerable<string>
        {
            readonly List<string> list = new List<string>();
            //contained are behined containers.
            internal Suite(string s)
            {
                if (s.Length > 0)
                    list.Add(s);
            }
            internal bool add(string v)
            {
                if (v.Length == 0)
                    return true;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    string s = list[i];
                    if (s.StartsWith(v))
                    {
                        if (s.Length == v.Length)
                            return true; // ignore duplicates
                        list.Insert(i + 1, v);
                        return true;
                    }
                }
                return false;
            }
            internal int size()
            {
                return list.Count;
            }
            internal string this[int i]
            {
                get { return list[i]; }
            }
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                foreach (string s in list)
                {
                    yield return s;
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (string s in list)
                {
                    yield return s;
                }
            }

        }
        private sealed class Suites
        {
            readonly System.Collections.Generic.List<Suite> suites = new System.Collections.Generic.List<Suite>();
            //bigger suite first
            internal void add(string v)
            {
                foreach (Suite suite in suites)
                {
                    if (suite.add(v))
                        return;
                }
                suites.Add(new Suite(v));
            }
            internal string[] toArray()
            {
                List<string> result = new List<string>();
                for (int i = suites.Count - 1; i >= 0; i--)
                {
                    Suite suite = suites[i];
                    foreach (string n in suite)
                    {
                        result.Add(n);
                    }
                }
                string[] ret = new string[result.Count];
                result.CopyTo(ret, 0);
                return ret;
            }
        }
        public static string[] sort(string[] names)
        {
            //short name first, unless it is fully contained in a longer name
            string[] _names = (string[])names.Clone();
            System.Array.Sort(_names, comparer);
            Suites suites = new Suites();
            for (int i = _names.Length - 1; i >= 0; i--)
            {
                suites.add(_names[i]);
            }
            return suites.toArray();
        }
        class LengthComparer : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                if (a == b)
                    return 0;
                if (a == null)
                    return -1;
                if (b == null)
                    return 1;
                return a.Length - b.Length;
            }
        }
    }
}
