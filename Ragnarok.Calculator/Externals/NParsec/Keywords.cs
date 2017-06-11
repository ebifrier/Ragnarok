namespace Codehaus.Parsec
{
    using System;
    using FromString = FromRange<string, object>;
    using SortedIndex = System.Collections.Generic.SortedDictionary<string, bool>;
    using Lexer = Parser<Tok>;
    using Scanner = Parser<D_>;
    /// <author>  Ben Yu
    /// 
    /// Dec 19, 2004
    /// </author>
#if !NETSTANDARD
    [Serializable]
#endif
    sealed class Keywords
    {

        interface StringCase
        {
            System.Collections.Generic.IComparer<string> Comparator
            {
                get;
            }
            string ToKey(string k);
            Map<string, object> ToMap(System.Collections.IDictionary m);
        }
        class StringComparer : System.Collections.Generic.IComparer<string>
        {
            readonly bool insensitive;
            internal bool CaseInsensitive
            {
                get { return insensitive; }
            }
            internal StringComparer(bool insensitive)
            {
                this.insensitive = insensitive;
            }
            public int Compare(string a, string b)
            {
                if (a == b)
                    return 0;
                else if (a == null)
                    return -1;
                else if (b == null)
                    return 1;
                else
                {
                    return string.Compare(a, b, insensitive);
                }
            }
        }
        static readonly Keywords.StringCase case_sensitive = new SimpleCase(false);
        static readonly Keywords.StringCase case_insensitive = new SimpleCase(true);
        sealed class SimpleCase : Keywords.StringCase
        {
            readonly StringComparer comparer;
            public string ToKey(string k)
            {
                return comparer.CaseInsensitive ? k.ToLower() : k;
            }
            public Map<string, object> ToMap(System.Collections.IDictionary dict)
            {
                if (comparer.CaseInsensitive)
                {
                    return delegate(string k)
                    {
                        return dict[k.ToLower()];
                    };
                }
                else return Functors.AsMap<string, object>(dict);
            }
            public System.Collections.Generic.IComparer<string> Comparator
            {
                get
                {
                    return comparer;
                }
            }
            internal SimpleCase(bool insensitive)
            {
                comparer = new StringComparer(insensitive);
            }
        }

        static Keywords.StringCase getStringCase(bool cs)
        {
            return cs ? case_sensitive : case_insensitive;
        }
        /*
        static string[] dedup (string[] names, System.Collections.Generic.IComparer<string> c) {
          SortedIndex index = new SortedIndex (c);
          foreach (string name in names) {
            index.Add (name, true);
          }
          string[] result = new string[index.Count];
          index.Keys.CopyTo (result, 0);
          return result;
        }*/
        /*
        //this method tries each keyword sequentially. while getInstance() scans a word and then lookup in a hash table.
        static WordsData instance(final String[] names, boolean cs){
        final StringCase scase = getStringCase(cs);
        final String[] _names = nub(names, scase.getComparator());
        final HashMap map = new HashMap();
        final Lexer[] lxs = new Lexer[_names.length];
        for(int i=0; i<_names.length; i++){
        final String n = _names[i];
        final Token tok = new TokenReserved(n);
        map.put(scase.getKey(n), tok);
        final Parser kw = cs?Scanners.IsString(n):Scanners.IsStringCI(n);
        final Lexer lx = Lexers.toLexer(
        Scanners.delimited(kw), ConstTokenizer.instance(tok)
        );
        lxs[i] = lx;
        }
        return new WordsData(scase.getMap(map), lxs);
        }*/
        internal static WordsData getWordsInstance(Scanner wscanner, string[] keywords, bool cs)
        {
            return getWordsInstance("words", wscanner, keywords, cs);
        }
        internal static WordsData getKeywordsInstance(Scanner wscanner, string[] keywords, bool cs)
        {
            return getKeywordsInstance("keywords", wscanner, keywords, cs);
        }
        private static readonly FromString _unknown_keyword = Lexers.ToValue<string, object>(null);
        internal static WordsData getWordsInstance(string name, Scanner wscanner, string[] keywords, bool cs)
        {
            return getInstance(name, wscanner, keywords, cs, String2Word.Instance);
        }
        internal static WordsData getKeywordsInstance(string name, Scanner wscanner, string[] keywords, bool cs)
        {
            return getInstance(name, wscanner, keywords, cs, _unknown_keyword);
        }
        internal static WordsData getInstance<T>(string name, Scanner wscanner, string[] names, bool cs,
          FromRange<string, T> for_unknown)
        {
            Keywords.StringCase scase = getStringCase(cs);
            string[] unique_names = Misc.Dedup(names, scase.Comparator);
            System.Collections.Hashtable map = new System.Collections.Hashtable();
            foreach (string n in unique_names)
            {
                object tok = Tokens.CreateReservedWordToken(n);
                map[scase.ToKey(n)] = tok;
            }
            Map<string, object> fmap = scase.ToMap(map);

            Tokenizer tn = delegate(string src, int from, int len)
            {
                string text = src.Substring(from, len);
                object r = fmap(text);
                if (r != null)
                    return r;
                else
                    return for_unknown(from, len, src);
            };
            Lexer lx = Lexers.Lex(wscanner, tn);
            return new WordsData(fmap, new Lexer[] { lx });
        }
    }
}
