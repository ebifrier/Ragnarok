using System;
using System.Collections.Generic;

namespace Codehaus.Parsec
{
    public struct Tok : IEquatable<Tok>
    {
        public int Length
        {
            get;
        }
        public int Index
        {
            get;
        }
        public object Token
        {
            get;
        }

        public Tok(int i, int l, object tok)
        {
            this.Length = l;
            this.Index = i;
            this.Token = tok;
        }
        public override string ToString()
        {
            return Maps.ToString(Token);
        }

        public override bool Equals(object obj)
        {
            return Equals((Tok)obj);
        }
        public bool Equals(Tok other)
        {
            return Length == other.Length && Index == other.Index &&
                   Token == other.Token;
        }
        public static bool operator ==(Tok x, Tok y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(Tok x, Tok y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return Length.GetHashCode() ^ Index.GetHashCode() ^ Token.GetHashCode();
        }
    }
}
