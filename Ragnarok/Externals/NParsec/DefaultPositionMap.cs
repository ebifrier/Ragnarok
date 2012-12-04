using System;
using IntArray = System.Collections.Generic.List<int>;
namespace Codehaus.Parsec
{


    /// <summary> This default implementation of PositionMap.
    /// <p>
    /// This class internally keeps a cache of the positions of
    /// all the line break characters scanned so far,
    /// therefore repeated position lookup can be done in amortized log(n) time.
    /// </p>
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// Dec 12, 2004
    /// </author>
    [Serializable]
    public class DefaultPositionMap : PositionMap
    {
        private readonly string src;
        private readonly int start_lno;
        private readonly IntArray line_breaks = new IntArray(20);
        private int next_ind = 0;
        private int next_col;
        private readonly char line_break;

        private int searchLineIndex(int ind)
        {
            int len = line_breaks.Count;
            int begin = 0;
            int to = len;
            for (; ; )
            {
                if (begin == to)
                    return begin;
                int i = (to + begin) / 2;
                int x = line_breaks[i];
                if (x == ind)
                    return i;
                else if (x > ind)
                {
                    to = i;
                }
                else
                {
                    begin = i + 1;
                }
            }
        }
        private Pos searchPosition(int ind)
        {
            int sz = line_breaks.Count;
            if (sz == 0)
            {
                return newPos(0, ind + 1);
            }
            else
            {
                int last_break = line_breaks[sz - 1];
                if (ind > last_break)
                {
                    return newPos(sz, ind - last_break);
                }
                else
                {
                    int lno = searchLineIndex(ind);
                    if (lno == 0)
                    {
                        return newPos(0, ind + 1);
                    }
                    else
                    {
                        int previous_break = line_breaks[lno - 1];
                        return newPos(lno, ind - previous_break);
                    }
                }
            }
        }
        private Pos searchForward(int ind)
        {
            bool eof = false;
            if (ind == src.Length)
            {
                eof = true;
                ind--;
            }
            int col = next_col;
            for (int i = next_ind; i <= ind; i++)
            {
                char c = src[i];
                if (c == line_break)
                {
                    line_breaks.Add(i);
                    col = 1;
                }
                else
                {
                    col++;
                }
            }
            this.next_ind = ind;
            this.next_col = col;
            int lines = line_breaks.Count;
            if (eof)
            {
                return newPos(lines, col);
            }
            else if (col == 1)
            {
                return getLineBreakPos(lines - 1);
            }
            else
            {
                return newPos(lines, col - 1);
            }
        }
        private int getLineBreakColumn(int lno)
        {
            int line_break = line_breaks[lno];
            if (lno == 0)
                return line_break + 1;
            else
            {
                return line_break - line_breaks[lno - 1];
            }
        }
        private Pos getLineBreakPos(int lno)
        {
            return newPos(lno, getLineBreakColumn(lno));
        }
        private Pos newPos(int l, int c)
        {
            return new Pos(start_lno + l, c);
        }

        public virtual Pos ToPos(int n)
        {
            //return getPos(n, src, start_lno, start_cno);
            if (n < next_ind)
            {
                return searchPosition(n);
            }
            else
            {
                return searchForward(n);
            }
        }

        /// <summary> Create a DefaultPositionMap object.</summary>
        /// <param name="src">the source.
        /// </param>
        /// <param name="lno">the starting line number.
        /// </param>
        /// <param name="cno">the starting column number.
        /// </param>
        /// <param name="line_break">the line break character.
        /// </param>
        public DefaultPositionMap(string src, int lno, int cno, char line_break)
        {
            this.src = src;
            this.start_lno = lno;
            this.next_col = cno;
            this.line_break = line_break;
        }
        /// <summary> Create a DefaultPositionMap object.</summary>
        /// <param name="src">the source.
        /// </param>
        /// <param name="lno">the starting line number.
        /// </param>
        /// <param name="cno">the starting column number.
        /// </param>
        public DefaultPositionMap(string src, int lno, int cno)
            : this(src, lno, cno, '\n')
        {
        }
    }
}
