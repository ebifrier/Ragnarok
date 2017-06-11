using System;
using System.Collections.Generic;
using System.Text;

namespace Codehaus.Parsec
{
    public struct Tok
    {
        private readonly int ind;
        private readonly int len;
        private readonly object tok;

        public int Length
        {
            get { return len; }
        }
        public int Index
        {
            get { return ind; }
        }
        public object Token
        {
            get { return tok; }
        }

        public Tok(int i, int l, object tok)
        {
            this.ind = i;
            this.len = l;
            this.tok = tok;
        }
        public override string ToString()
        {
            return Maps.ToString(tok);
        }
    }
}
