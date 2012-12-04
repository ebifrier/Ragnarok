namespace Codehaus.Parsec
{
    /// <summary> Expressions provides helper functions to build parser for a operator-precedence expression grammar. <br />
    /// It supports prefix unary, postfix unary, infix left associative binary,
    /// infix right associative binary and infix non-associative binary operators.
    /// <br />
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// Nov 19, 2004
    /// </author>
    public sealed class Expressions
    {
        /// <summary> Creates a Parser object based on information described by OperatorTable.</summary>
        /// <param name="term">parser for the terminals.
        /// </param>
        /// <param name="table">the operator table.
        /// </param>
        /// <returns> the expression parser.
        /// </returns>
        public static Parser<T> BuildExpressionParser<T>(Parser<T> term, OperatorTable<T> table)
        {
            OperatorTable<T>.Operator[] ops = table.Operators;
            if (ops.Length == 0)
                return term;
            int begin = 0;
            int prec = ops[0].Precedence;
            OperatorKind kind = ops[0].Kind;
            int end = 0;
            Parser<T> ret = term;
            for (int i = 1; i < ops.Length; i++)
            {
                OperatorTable<T>.Operator op = ops[i];
                end = i;
                if (op.Precedence == prec && op.Kind == kind)
                {
                    continue;
                }
                else
                {
                    end = i;
                    Parser p = slice(kind, ops, begin, end);
                    ret = build(p, kind, ret);
                    begin = i;
                    prec = ops[i].Precedence;
                    kind = ops[i].Kind;
                }
            }
            if (end != ops.Length)
            {
                end = ops.Length;
                kind = ops[begin].Kind;
                Parser p = slice(kind, ops, begin, end);
                ret = build(p, kind, ret);
            }
            return ret;
        }
        private static Parser slice<T>(OperatorKind kind, OperatorTable<T>.Operator[] ops, int begin, int end)
        {
            if (kind == OperatorKind.PREFIX || kind == OperatorKind.POSTFIX)
            {
                Parser<Map<T, T>>[] ps = new Parser<Map<T, T>>[end - begin];
                for (int i = 0; i < ps.Length; i++)
                {
                    ps[i] = ops[i + begin].Op as Parser<Map<T, T>>;
                }
                return Parsers.Plus(ps);
            }
            else
            {
                Parser<Map<T, T, T>>[] ps = new Parser<Map<T, T, T>>[end - begin];
                for (int i = 0; i < ps.Length; i++)
                {
                    ps[i] = ops[i + begin].Op as Parser<Map<T, T, T>>;
                }
                return Parsers.Plus(ps);
            }
        }
        private static Parser<T> build<T>(Parser op, OperatorKind kind, Parser<T> operand)
        {
            switch (kind)
            {
                case OperatorKind.PREFIX:
                    return Parsers.Prefix(op as Parser<Map<T, T>>, operand);

                case OperatorKind.POSTFIX:
                    return Parsers.Postfix(operand, op as Parser<Map<T, T>>);

                case OperatorKind.LASSOC:
                    return Parsers.Infixl(op as Parser<Map<T, T, T>>, operand);

                case OperatorKind.RASSOC:
                    return Parsers.Infixr(op as Parser<Map<T, T, T>>, operand);

                case OperatorKind.NASSOC:
                    return Parsers.Infixn(op as Parser<Map<T, T, T>>, operand);

                default:
                    return operand;

            }
        }
    }
}
