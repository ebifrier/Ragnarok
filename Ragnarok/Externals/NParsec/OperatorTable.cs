namespace Codehaus.Parsec
{
    using System;
    using System.Collections.Generic;
    /// <summary> This class is used to describe operator information.
    /// Operators have precedences. the higher the precedence number, the higher the precedence.
    /// For the same precedence, prefix > postfix > left infix > right infix > non-associative infix.
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// Nov 21, 2004
    /// </author>
    [Serializable]
    public sealed class OperatorTable<T>
    {
        internal Operator[] Operators
        {
            get
            {
                ops.Sort();
                return ops.ToArray();
            }
        }

        //UPGRADE_NOTE: Final was removed from the declaration of 'ops '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private List<Operator> ops = new List<Operator>();
        internal struct Operator : IComparable<Operator>
        {
            internal Parser Op
            {
                get
                {
                    return op;
                }

            }
            internal int Precedence
            {
                get
                {
                    return prec;
                }

            }
            internal OperatorKind Kind
            {
                get
                {
                    return kind;
                }
            }
            private Parser op;
            private int prec;
            private OperatorKind kind;
            internal Operator(Parser op, int prec, OperatorKind k)
            {
                this.op = op;
                this.prec = prec;
                this.kind = k;
            }
            public int CompareTo(Operator other)
            {
                if (prec > other.prec)
                    return -1;
                else if (prec < other.prec)
                    return 1;
                else if (kind < other.kind)
                    return -1;
                else if (kind > other.kind)
                    return 1;
                return 0;
            }
        }
        /// <summary> Adds a prefix unary operator.</summary>
        /// <param name="p">the parser for the operator.
        /// </param>
        /// <param name="precedence">the precedence number.
        /// </param>
        /// <returns> this.
        /// </returns>
        public OperatorTable<T> Prefix(Parser<Map<T, T>> p, int precedence)
        {
            ops.Add(new Operator(p, precedence, OperatorKind.PREFIX));
            return this;
        }
        /// <summary> Adds a postfix unary operator.</summary>
        /// <param name="p">the parser for the operator.
        /// </param>
        /// <param name="precedence">the precedence number.
        /// </param>
        /// <returns> this.
        /// </returns>
        public OperatorTable<T> Postfix(Parser<Map<T, T>> p, int precedence)
        {
            ops.Add(new Operator(p, precedence, OperatorKind.POSTFIX));
            return this;
        }
        /// <summary> Adds a infix left-associative binary operator.</summary>
        /// <param name="p">the parser for the operator.
        /// </param>
        /// <param name="precedence">the precedence number.
        /// </param>
        /// <returns> this.
        /// </returns>
        public OperatorTable<T> Infixl(Parser<Map<T, T, T>> p, int precedence)
        {
            ops.Add(new Operator(p, precedence, OperatorKind.LASSOC));
            return this;
        }
        /// <summary> Adds a infix right-associative binary operator.</summary>
        /// <param name="p">the parser for the operator.
        /// </param>
        /// <param name="precedence">the precedence number.
        /// </param>
        /// <returns> this.
        /// </returns>
        public OperatorTable<T> Infixr(Parser<Map<T, T, T>> p, int precedence)
        {
            ops.Add(new Operator(p, precedence, OperatorKind.RASSOC));
            return this;
        }
        /// <summary> Adds a infix non-associative binary operator.</summary>
        /// <param name="p">the parser for the operator.
        /// </param>
        /// <param name="precedence">the precedence number.
        /// </param>
        /// <returns> this.
        /// </returns>
        public OperatorTable<T> Infixn(Parser<Map<T, T, T>> p, int precedence)
        {
            ops.Add(new Operator(p, precedence, OperatorKind.NASSOC));
            return this;
        }
    }
    enum OperatorKind
    {
        PREFIX, POSTFIX, LASSOC, RASSOC, NASSOC
    }
}
