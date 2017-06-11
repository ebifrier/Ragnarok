using System;
using System.Collections.Generic;
using System.Text;

namespace Codehaus.Parsec
{
    public struct Pos : IEquatable<Pos>
    {
        private readonly int lno;
        private readonly int cno;
        public Pos(int lno, int cno)
        {
            this.lno = lno;
            this.cno = cno;
        }

        public int Column
        {
            get { return cno; }
        }
        public int Line
        {
            get { return lno; }
        }


        public bool Equals(Pos other)
        {
            return lno == other.lno && cno == other.cno;
        }

        public override int GetHashCode()
        {
            return lno * 31 + cno;
        }

        public override string ToString()
        {
            return "line " + lno + " column " + cno;
        }

    }
    /// <summary> The interface to find the line number,
    /// column number of a certain position in the source.
    /// 
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// Dec 12, 2004
    /// </author>
    public interface PositionMap
    {
        /// <summary> Get the line/column number of a position identified by an offset number.</summary>
        /// <param name="n">the offset in the source, starting from 0.
        /// </param>
        /// <returns> the Pos object.
        /// </returns>
        Pos ToPos(int n);
    }
}
