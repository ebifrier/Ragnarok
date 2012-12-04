namespace Codehaus.Parsec
{
    using ShowToken = Map<object, string>;
    using IntOrder = Map<int, int, bool>;
    using Lexeme = Parser<Tok[]>;
    /// <summary>
    /// This class contains most commonly used combinator functions.
    /// </summary>
    public class Parsers
    {
        static bool debug = true;
        /// <summary>
        /// To enable or disable debugging support.
        /// Default is true.
        /// </summary>
        public static bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }
        /// <summary> Runs a character level parser with a string input.</summary>
        /// <param name="src">the input source.
        /// </param>
        /// <param name="p">the parser object to run.
        /// </param>
        /// <param name="pmap">the PositionMap object used to map a character index
        /// to line/column number.
        /// </param>
        /// <param name="module">the module name. Use any name that's meaningful.
        /// </param>
        /// <returns> the result object.
        /// </returns>
        public static R RunParser<R>(string src, Parser<R> p, PositionMap pmap, string module)
        {
            ScannerState ctxt = new ScannerState(src, 0, module, pmap);
            return runParser(ctxt, p, pmap);
        }
        /// <summary> Runs a character level parser with a string input.</summary>
        /// <param name="src">the input source.
        /// </param>
        /// <param name="p">the parser object to run.
        /// </param>
        /// <param name="module">the module name. Use any name that's meaningful.
        /// </param>
        /// <returns> the result object.
        /// </returns>
        public static R RunParser<R>(string src, Parser<R> p, string module)
        {
            return RunParser(src, p, new DefaultPositionMap(src, 1, 1), module);
        }
        /// <summary> Runs a token level Parser object with an array of tokens.
        /// <p /> [Tok] -> int -> Parser a -> ShowToken -> String -> PositionMap -> a
        /// </summary>
        /// <param name="toks">the input tokens
        /// </param>
        /// <param name="end_index">the index after the last character in the source.
        /// </param>
        /// <param name="p">the parser object.
        /// </param>
        /// <param name="show">the object to show the tokens.
        /// </param>
        /// <param name="eof_title">the name of "end of file". 
        /// </param>
        /// <param name="module">the module name. Use any name that's meaningful.
        /// This value will be shown in any EOF related messages.
        /// </param>
        /// <param name="pmap">the PositionMap object to map a character index to the line/column number.
        /// </param>
        /// <returns> the return value of the Parser object. (returned by retn() function)
        /// </returns>
        /// <throws>  ParserException when parsing fails. </throws>
        public static R RunParser<R>(Tok[] toks, int end_index, Parser<R> p, ShowToken show, string eof_title, PositionMap pmap, string module)
        {
            ParserState s0 = new ParserState(toks, 0, module, pmap, end_index, eof_title, show);
            return runParser(s0, p, pmap);
        }
        /// <summary> To create a Parser that always succeeds and causes a certain side effect
        /// using a Proc object.
        /// </summary>
        /// <param name="proc">the Proc object.
        /// </param>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<T> Action<T>(Proc proc)
        {
            return new ActionParser<T>("action", proc);
        }
        /// <summary> The created parser object will take as input
        /// the array of Tok returned from the lexer object,
        /// feed it into the Parser object p and run it,
        /// return the result from parser p. <br />
        /// It fails if the lexer fails or parser p fails. 
        /// Parser [Tok] -> Parser a -> Parser a
        /// </summary>
        /// <param name="lexeme">the lexeme object that returns an array of Tok objects.
        /// </param>
        /// <param name="p">the token level parser object.
        /// </param>
        /// <param name="module">the module name. Use any name that's meaningful.
        /// that will parse the array of Tok objects.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> ParseTokens<T>(Lexeme lexeme, Parser<T> p, string module)
        {
            return ParseTokens("EOF", Maps.ToStringMap, lexeme, p, module);
        }

        /// <summary> The created parser object will take as input
        /// the array of Tok returned from the lexer object,
        /// feed it into the Parser object p and run it,
        /// return the result from parser p. <br />
        /// It fails if the lexer fails or parser p fails. 
        /// String -> ShowToken -> Parser [Tok] -> Parser a -> Parser a
        /// </summary>
        /// <param name="eof_title">the name of "end of input"
        /// </param>
        /// <param name="show">the object to transform a token to a string.
        /// </param>
        /// <param name="lexeme">the lexeme object that returns an array of Tok objects.
        /// </param>
        /// <param name="p">the token level parser object.
        /// </param>
        /// <param name="module">the module name. Use any name that's meaningful.
        /// that will parse the array of Tok objects.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> ParseTokens<T>(string eof_title, ShowToken show,
          Lexeme lexeme, Parser<T> p, string module)
        {
            return new LexerThenParser<T>(module, eof_title, show, lexeme, p).Rename(p.Name);
        }

        /// <summary> The parser that always succeed. It does not consume any input.
        /// The default value of the type T is used as the result.
        /// </summary>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<T> One<T>()
        {
            return new OneParser<T>().Rename("1");
        }

        /// <summary> The parser that always fails. It does not consume any input.
        /// <p /> Parser *
        /// </summary>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<T> Zero<T>()
        {
            return new ZeroParser<T>().Rename("0");
        }
        /// <summary> The parser that returns value v. It does not consume any input.
        /// <p /> a -> Parser a
        /// </summary>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<T> Return<T>(T v)
        {
            return new ReturnParser<T>(v).Rename("retn");
        }

        /// <summary> Creates a ToParser object by always returning the same Parser object.</summary>
        /// <param name="p">the parser object.
        /// </param>
        /// <returns> the ToParser object.
        /// </returns>
        /// <since> version 0.4.1
        /// </since>
        public static Binder<From, To> BindTo<From, To>(Parser<To> p)
        {
            return delegate(From v)
            {
                return p;
            };
        }
        /// <summary> Threads an array of ToParser into a single ToParser.
        /// The first return value is passed to the first ToParser,
        /// and the result Parser is executed to get the next return value.
        /// The return value keeps pass down until all ToParser are called.
        /// If any Parser fails, the threading fails.
        /// <p /> [(a->Parser a)] -> a -> Parser a
        /// </summary>
        /// <param name="binders">all the ToParser objects.
        /// </param>
        /// <returns> the new ToParser.
        /// </returns>
        public static Binder<T, T> BindAll<T>(params Binder<T, T>[] binders)
        {
            if (binders.Length == 0)
                return BindTo<T, T>(One<T>());
            else if (binders.Length == 1)
                return binders[0];
            return new Binder<T, T>(new SequenceBinder<T>(binders).bind);
        }
        /// <summary> Sequencing 2 parser objects.
        /// The first Parser is executed, if it succeeds, the second Parser is executed.
        /// <p /> Parser a -> Parser b -> Parser b
        /// </summary>
        /// <param name="p1">1st Parser.
        /// </param>
        /// <param name="p2">2nd Parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public static Parser<R> Seq<A, R>(Parser<A> p1, Parser<R> p2)
        {
            return new SeqParser<A, R>(p1, p2);
        }
        /// <summary> Sequencing 3 parser objects.
        /// <p /> Parser a -> Parser b -> Parser c -> Parser c
        /// </summary>
        /// <param name="p1">1st Parser.
        /// </param>
        /// <param name="p2">2nd Parser.
        /// </param>
        /// <param name="p3">3rd Parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public static Parser<R> Seq<A, B, R>(Parser<A> p1, Parser<B> p2, Parser<R> p3)
        {
            return new SeqParser<A, B, R>(p1, p2, p3);
        }
        /// <summary> Sequencing 4 parser objects.
        /// <p /> Parser a -> Parser b -> Parser c -> Parser c -> Parser d -> Parser d
        /// </summary>
        /// <param name="p1">1st Parser.
        /// </param>
        /// <param name="p2">2nd Parser.
        /// </param>
        /// <param name="p3">3rd Parser.
        /// </param>
        /// <param name="p4">4th Parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public static Parser<R> Seq<A, B, C, R>(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<R> p4)
        {
            return new SeqParser<A, B, C, R>(p1, p2, p3, p4);
        }
        /// <summary> Sequencing 5 parser objects.
        /// <p /> Parser a -> Parser b -> Parser c -> Parser c -> Parser d -> Parser e -> Parser e
        /// </summary>
        /// <param name="p1">1st Parser.
        /// </param>
        /// <param name="p2">2nd Parser.
        /// </param>
        /// <param name="p3">3rd Parser.
        /// </param>
        /// <param name="p4">4th Parser.
        /// </param>
        /// <param name="p5">5th Parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public static Parser<R> Seq<A, B, C, D, R>(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<D> p4, Parser<R> p5)
        {
            return new SeqParser<A, B, C, D, R>(p1, p2, p3, p4, p5);
        }
        /// <summary>
        /// Sequentially run 2 parser objects and collect the results in a Pair object.
        /// </summary>
        /// <param name="p1">1st Parser object.</param>
        /// <param name="p2">2nd Parser object.</param>
        /// <returns>the result Parser object.</returns>
        public static Parser<Pair<A, B>> Pair<A, B>(Parser<A> p1, Parser<B> p2)
        {
            return Map(p1, p2, Maps.ToPair<A, B>());
        }
        /// <summary>
        /// Sequentially run 3 parser objects and collect the results in a Tuple object.
        /// </summary>
        /// <param name="p1">1st Parser object.</param>
        /// <param name="p2">2nd Parser object.</param>
        /// <param name="p3">3rd Parser object.</param>
        /// <returns>the result Parser object.</returns>
        public static Parser<Tuple<A, B, C>> Tuple<A, B, C>(Parser<A> p1, Parser<B> p2, Parser<C> p3)
        {
            return Map(p1, p2, p3, Maps.ToTuple<A, B, C>());
        }
        /// <summary>
        /// Sequentially run 4 parser objects and collect the results in a Tuple object.
        /// </summary>
        /// <param name="p1">1st Parser object.</param>
        /// <param name="p2">2nd Parser object.</param>
        /// <param name="p3">3rd Parser object.</param>
        /// <param name="p4">4th Parser object.</param>
        /// <returns>the result Parser object.</returns>
        public static Parser<Tuple<A, B, C, D>> Tuple<A, B, C, D>(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<D> p4)
        {
            return Map(p1, p2, p3, p4, Maps.ToTuple<A, B, C, D>());
        }
        /// <summary>
        /// Sequentially run 5 parser objects and collect the results in a Tuple object.
        /// </summary>
        /// <param name="p1">1st Parser object.</param>
        /// <param name="p2">2nd Parser object.</param>
        /// <param name="p3">3rd Parser object.</param>
        /// <param name="p4">4th Parser object.</param>
        /// <param name="p5">5th Parser object.</param>
        /// <returns>the result Parser object.</returns>
        public static Parser<Tuple<A, B, C, D, E>> Tuple<A, B, C, D, E>(Parser<A> p1, Parser<B> p2, Parser<C> p3,
          Parser<D> p4, Parser<E> p5)
        {
            return Map(p1, p2, p3, p4, p5, Maps.ToTuple<A, B, C, D, E>());
        }
        /// <summary> Sequencing of an array of Parser objects.
        /// If the array is empty, one() is returned. <br />
        /// The array of Parser objects are executed sequentially until an error occured or all the Parsers are executed.
        /// The return value of the last parser is preserved.
        /// </summary>
        /// <param name="parsers">the array of Parser objects.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Sequence<T>(params Parser<T>[] parsers)
        {
            return new SequenceParser<T>(parsers);
        }
        /// <summary> An array of alternative Parser objects.
        /// zero() is returned if the array is empty.
        /// the returned Parser object will try the Parser objects in the array one by one,
        /// until one of the following conditioins are met:
        /// the Parser succeeds, (plus() succeeds) <br />
        /// the Parser fails with input consumed (plus() fails with input consumed) <br />
        /// the end of array is encountered. (plus() fails with no input consumed).
        /// <p /> [Parser a] -> Parser a
        /// </summary>
        /// <param name="parsers">the array of alternative Parser objects.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Plus<T>(params Parser<T>[] parsers)
        {
            return new SumParser<T>(parsers);
        }
        /// <summary> Transform the return value of Parser p to a different value.
        /// <p /> Parser a -> (a->b) -> Parser b
        /// </summary>
        /// <param name="p">the Parser object.
        /// </param>
        /// <param name="m">the Map object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<R> Map<A, R>(Parser<A> p, Map<A, R> m)
        {
            return new MappedParser<A, R>(p, m);
        }
        /// <summary> Run 2 Parsers sequentially and transform the return values to a new value.
        /// <p /> Parser a->Parser b->(a->b->r)->Parser r
        /// </summary>
        /// <param name="p1">1st parser.
        /// </param>
        /// <param name="p2">2nd parser.
        /// </param>
        /// <param name="m2">the transformer.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<R> Map<A, B, R>(Parser<A> p1, Parser<B> p2, Map<A, B, R> m2)
        {
            return new MappedParser<A, B, R>(p1, p2, m2);
        }
        /// <summary> Run 3 Parsers sequentially and transform the return values to a new value.
        /// <p /> Parser a->Parser b->Parser c->(a->b->c->r)->Parser r
        /// </summary>
        /// <param name="p1">1st parser.
        /// </param>
        /// <param name="p2">2nd parser.
        /// </param>
        /// <param name="p3">3rd parser.
        /// </param>
        /// <param name="m3">the transformer.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<R> Map<A, B, C, R>(Parser<A> p1, Parser<B> p2, Parser<C> p3, Map<A, B, C, R> m3)
        {
            return new MappedParser<A, B, C, R>(p1, p2, p3, m3);
        }
        /// <summary> Run 4 Parsers sequentially and transform the return values to a new value.
        /// <p /> Parser a->Parser b->Parser c->Parser d->(a->b->c->d->r)->Parser r
        /// </summary>
        /// <param name="p1">1st parser.
        /// </param>
        /// <param name="p2">2nd parser.
        /// </param>
        /// <param name="p3">3rd parser.
        /// </param>
        /// <param name="p4">4th parser.
        /// </param>
        /// <param name="m4">the transformer.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<R> Map<A, B, C, D, R>(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<D> p4,
          Map<A, B, C, D, R> m4)
        {
            return new MappedParser<A, B, C, D, R>(p1, p2, p3, p4, m4);
        }
        /// <summary> Run 5 Parsers sequentially and transform the return values to a new value.
        /// <p /> Parser a->Parser b->Parser c->Parser d->Parser e->(a->b->c->d->e->r)->Parser r
        /// </summary>
        /// <param name="p1">1st parser.
        /// </param>
        /// <param name="p2">2nd parser.
        /// </param>
        /// <param name="p3">3rd parser.
        /// </param>
        /// <param name="p4">4th parser.
        /// </param>
        /// <param name="p5">5th parser.
        /// </param>
        /// <param name="m5">the transformer.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<R> Map<A, B, C, D, E, R>(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<D> p4, Parser<E> p5,
          Map<A, B, C, D, E, R> m5)
        {
            return new MappedParser<A, B, C, D, E, R>(p1, p2, p3, p4, p5, m5);
        }
        /// <summary> Sequencing of an array of Parser objects.
        /// The array of Parser objects are executed sequentially until an error occured or all the Parsers are executed.
        /// Return values are collected as a Object[] array and transformed by a Mapn object.
        /// <p /> [Parser a] -> ([a]->r) -> Parser r
        /// </summary>
        /// <param name="mn">the Mapn object.
        /// </param>
        /// <param name="parsers">the array of Parser objects.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<To> Mapn<From, To>(Mapn<From, To> mn, params Parser<From>[] parsers)
        {
            return Mapn(Functors.getSimpleArrayFactory<From>(), mn, parsers);
        }
        /// <summary> Sequencing of an array of Parser objects.
        /// The array of Parser objects are executed sequentially until an error occured or all the Parsers are executed.
        /// Return values are collected and returned as an array created by the ArrayFactory parameter,
        /// and then transformed by a Mapn object.
        /// <p /> [Parser a] -> ([a]->r) -> Parser r
        /// </summary>
        /// <param name="factory">the ArrayFactory object that creates the array object storing the results.
        /// </param>
        /// <param name="parsers">the array of Parser objects.
        /// </param>
        /// <param name="mn">the Mapn object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<To> Mapn<From, To>(ArrayFactory<From> factory, Mapn<From, To> mn, params Parser<From>[] parsers)
        {
            return new MapnParser<From, To>(factory, parsers, mn);
        }

        /// <summary> A parser that always fail with the given error message.
        /// <p /> Parser *
        /// </summary>
        /// <param name="msg">the error message.
        /// </param>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<T> Fail<T>(string msg)
        {
            return new FailingParser<T>(msg);
        }

        /// <summary> Runs an array of alternative parsers.
        /// If more than one succeed, the one with the longest match wins.
        /// If two matches have the same length, the first one is favored.
        /// </summary>
        /// <param name="parsers">the array of alternative parsers.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>

        public static Parser<T> Longest<T>(params Parser<T>[] parsers)
        {
            return new FavoredSumParser<T>(parsers, IntOrders.Greater());
        }
        /// <summary> Runs an array of alternative parsers.
        /// If more than one succeed, the one with the shortest match wins.
        /// If two matches have the same length, the first one is favored.
        /// </summary>
        /// <param name="parsers">the array of alternative parsers.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>

        public static Parser<T> Shortest<T>(params Parser<T>[] parsers)
        {
            return new FavoredSumParser<T>(parsers, IntOrders.Less());
        }
        /// <summary> Runs two alternative parsers.
        /// If both succeed, the one with the longer match wins.
        /// If both matches the same length, the first one is favored.
        /// </summary>
        /// <param name="p1">the 1st alternative parser.
        /// </param>
        /// <param name="p2">the 2nd alternative parser.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Longer<T>(Parser<T> p1, Parser<T> p2)
        {
            return Longest(p1, p2);
        }
        /// <summary> Runs two alternative parsers.
        /// If both succeed, the one with the shorter match wins.
        /// If both matches the same length, the first one is favored.
        /// </summary>
        /// <param name="p1">the 1st alternative parser.
        /// </param>
        /// <param name="p2">the 2nd alternative parser.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>

        public static Parser<T> Shorter<T>(Parser<T> p1, Parser<T> p2)
        {
            return Shortest(p1, p2);
        }
        /// <summary>
        /// Create a Parser object that reports an "something expecting" error.
        /// </summary>
        /// <param name="expected">the text shown in the error message.</param>
        /// <returns>the Parser object.</returns>
        public static Parser<T> Expect<T>(string expected)
        {
            return new ExpectingParser<T>(expected);
        }

        /// <summary> Reports an unexpected error.
        /// <p /> Parser *
        /// </summary>
        /// <param name="msg">the error message.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Unexpected<T>(string msg)
        {
            return new UnexpectedParser<T>(msg);
        }

        /// <summary> Create a lazy evaluated Parser. 
        /// the Generator object is evaluated only when the Parser is actually executed.
        /// <p /> Parser a -> Parser a
        /// </summary>
        /// <param name="generator">the generator object that lazily evaluates to a Parser object.
        /// </param>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<T> Lazy<T>(Generator<Parser<T>> generator)
        {
            return new LazyParser<T>(generator);
        }

        /// <summary> Retrieves the current index in the source.
        /// <p /> Parser Integer
        /// </summary>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<int> GetIndex()
        {
            return new GetIndexParser();
        }
        /// <summary> Token level parser.
        /// checks the current token with the FromToken object.
        /// If the fromToken() method returns null, a system unexpected token error occurs;
        /// if the method returns anything other than null, the token is consumed and token() succeeds.
        /// <p /> (SourcePos->Token->a) -> Parser a
        /// </summary>
        /// <param name="ft">the FromToken object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> FromToken<T>(FromToken<T> ft)
        {
            return new IsTokenParser<T>(ft);
        }
        /// <summary> Token level parser.
        /// checks to see if the current token is token t. (using ==).
        /// If no, a system unexpected token error occurs;
        /// if yes, the token is consumed and token() succeeds.
        /// the token is used as the parse result.
        /// <p /> Object -> Parser SourcePos
        /// </summary>
        /// <param name="t">the expected token object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> IsToken<T>(T t) where T : class
        {
            FromToken<T> ft = delegate(Tok tok, ref T result)
            {
                if (t == tok.Token)
                {
                    result = t;
                    return true;
                }
                else
                {
                    return false;
                }
            };
            return FromToken(ft);
        }
        /// <summary> Consumes a token. The token is used as the return value of the parser.
        /// <p /> Parser Token
        /// </summary>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<object> AnyToken()
        {
            return FromToken(Functors.AnyToken);
        }
        /// <summary> throws a pseudo-exception.
        /// <p /> Parser *
        /// </summary>
        /// <param name="e">the exception object.
        /// </param>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<T> Raise<T>(object e)
        {
            return new ThrowingParser<T>(e);
        }
        /// <summary> Asserts eof is met. Fails otherwise.
        /// </summary>
        /// <param name="msg">the error message if eof is not met.
        /// </param>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<D_> Eof(string msg)
        {
            return new EofParser<D_>(msg).Rename("eof");
        }
        /// <summary> Asserts eof is met. Fails otherwise.
        /// </summary>
        /// <returns> the Parser object.
        /// </returns>
        public static Parser<D_> Eof()
        {
            return Eof("EOF");
        }

        /// <summary> Runs Parser op for 0 or more times greedily. Then run Parser p.
        /// The Map object returned from op are applied from right to left to the return value of p. <br />
        /// prefix(op, p) is equivalent to op* p in EBNF
        /// <p /> Parser (a->a) -> Parser a -> Parser a
        /// </summary>
        /// <param name="op">the operator.
        /// </param>
        /// <param name="operand">the operand.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Prefix<T>(Parser<Map<T, T>> op, Parser<T> operand)
        {
            return Map(op.Many(), operand, getPrefixEvaluator<T>());
        }

        /// <summary> Runs Parser p and then run Parser op for 0 or more times greedily.
        /// The Map object returned from op are applied from left to right to the return value of p. <br />
        /// postfix(op, p) is equivalent to p op* in EBNF
        /// <p /> Parser (a->a) -> Parser a -> Parser a
        /// </summary>
        /// <param name="operand">the operand.
        /// </param>
        /// <param name="op">the operator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Postfix<T>(Parser<T> operand, Parser<Map<T, T>> op)
        {
            return Map(operand, op.Many(), getPostfixEvaluator<T>());
        }
        /// <summary> Non-associative infix operator.
        /// Runs Parser p and then run Parser op and Parser p optionally.
        /// The Map2 object returned from op is applied to the return values of the two p pattern, if any. <br />
        /// infixn(op, p) is equivalent to p (op p)? in EBNF
        /// <p /> Parser (a->a->a) -> Parser a -> Parser a
        /// </summary>
        /// <param name="op">the operator.
        /// </param>
        /// <param name="operand">the operand.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Infixn<T>(Parser<Map<T, T, T>> op, Parser<T> operand)
        {
            return operand.Bind<T>(delegate(T v1)
            {
                Parser<T> shift = op.And<T, T>(operand, delegate(Map<T, T, T> bin, T v2)
                {
                    return bin(v1, v2);
                });
                return shift | Return(v1);
            });
        }
        /// <summary> Left associative infix operator.
        /// Runs Parser p and then run Parser op and Parser p for 0 or more times greedily.
        /// The Map object returned from op are applied from left to right to the return value of p. <br />
        /// for example: a+b+c+d is evaluated as (((a+b)+c)+d). <br />
        /// infixl(op, p) is equivalent to p (op p)* in EBNF.
        /// <p /> Parser (a->a->a) -> Parser a -> Parser a
        /// </summary>
        /// <param name="op">the operator.
        /// </param>
        /// <param name="operand">the operand.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Infixl<T>(Parser<Map<T, T, T>> op, Parser<T> operand)
        {
            Binder<T, T> binder = delegate(T v0)
            {
                Generator<Accumulator<Pair<Map<T, T, T>, T>, T>> accm = delegate()
                {
                    return new LassocAccumulator<T>(v0);
                };
                return Pair(op, operand).Many(accm);
            };
            return operand.Bind(binder);
        }
        /// <summary> Right associative infix operator.
        /// Runs Parser p and then run Parser op and Parser p for 0 or more times greedily.
        /// The Map object returned from op are applied from right to left to the return values of p. <br />
        /// for example: a+b+c+d is evaluated as a+(b+(c+d)). <br />
        /// infixr(op, p) is equivalent to p (op p)* in EBNF.
        /// <p /> Parser (a->a->a) -> Parser a -> Parser a
        /// </summary>
        /// <param name="op">the operator.
        /// </param>
        /// <param name="operand">the operand.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public static Parser<T> Infixr<T>(Parser<Map<T, T, T>> op, Parser<T> operand)
        {
            Parser<Pair<Map<T, T, T>, T>> op_and_operand = Pair(op, operand);
            Map<T, Pair<Map<T, T, T>, T>[], T> eval = delegate(T v0, Pair<Map<T, T, T>, T>[] rhs)
            {
                if (rhs.Length == 0)
                    return v0;
                T o2 = rhs[rhs.Length - 1].V2;
                for (int i = rhs.Length - 1; i > 0; i--)
                {
                    Map<T, T, T> m2 = rhs[i].V1;
                    T o1 = rhs[i - 1].V2;
                    o2 = m2(o1, o2);
                }
                return rhs[0].V1(v0, o2);
            };
            return operand.And(op_and_operand.Many(), eval);
        }
        private static Map<Map<T, T>[], T, T> getPrefixEvaluator<T>()
        {
            return delegate(Map<T, T>[] ops, T operand)
            {
                for (int i = ops.Length - 1; i >= 0; i--)
                {
                    operand = ops[i](operand);
                }
                return operand;
            };
        }
        private static Map<T, Map<T, T>[], T> getPostfixEvaluator<T>()
        {
            return delegate(T operand, Map<T, T>[] ops)
            {
                foreach (Map<T, T> op in ops)
                {
                    operand = op(operand);
                }
                return operand;
            };
        }
        private static R runParser<R>(ParseContext ctxt, Parser<R> p, PositionMap pmap)
        {
            R result = default(R);
            AbstractParsecError err = null;
            if (!parseIt(ctxt, p, pmap, ref result, ref err))
            {
                int ind = err != null ? err.Index : -1;
                throw new ParserException(toErrorStr(err), (err == null ? null : err.render()), ctxt.getModuleName(), pmap.ToPos(ind >= 0 ? ind : ctxt.getIndex()));
            }
            return result;
        }
        private static bool parseIt<R>(ParseContext ctxt, Parser<R> p, PositionMap pmap, ref R result, ref AbstractParsecError err)
        {
            try
            {
                return p.parse(ctxt, ref result, ref err);
            }
            catch (UserException e)
            {
                int ind = e.Index;
                throw new ParserException(e.Message, null, ctxt.getModuleName(), pmap.ToPos(ind >= 0 ? ind : ctxt.getIndex()));
            }
        }
        //performance of this method is insignificant.
        private static string toErrorStr(AbstractParsecError aerr)
        {
            if (aerr == null)
            {
                return "";
            }
            ParsecError err = aerr.render();
            if (err.Exception != null)
            {
                return ("User exception: " + err.Exception.ToString());
            }
            else
                return "";
        }


    }
    class ParserChores
    {
        internal static AbstractParsecError raiseRaw(string msg, ParseContext state)
        {
            return ParsecError.raiseRaw(state.getIndex(), msg);
        }
        internal static AbstractParsecError raiseUnexpected(string msg, ParseContext state)
        {
            return ParsecError.raiseUnexpected(state.getIndex(), msg);
        }
        internal static AbstractParsecError raiseExpecting(string msg, ParseContext state)
        {
            return ParsecError.raiseExpecting(state.getIndex(), msg);
        }
        internal static bool recover<T>(int look_ahead, Parser<T> p,
            ParseContext ctxt, ref T result, ref AbstractParsecError err
          , int original_step, int original_at)
        {
            if (ctxt.getAt() != original_at && ctxt.getStep() - original_step >= look_ahead
              || err != null && err.Thrown)
                return false;
            ctxt.setState(original_step, original_at);
            AbstractParsecError nexterr = null;
            if (!p.parse(ctxt, ref result, ref nexterr))
            {
                err = AbstractParsecError.mergeError(err, nexterr);
                return false;
            }
            return true;
        }
        internal static bool best<T>(IntOrder ord, Parser<T>[] ps, int ind,
          ParseContext ctxt, ref T result, ref AbstractParsecError err
          , int original_step, int original_at)
        {
            if (ind >= ps.Length)
                return true;
            int most = ctxt.getAt();
            int mstep = ctxt.getStep();
            //object mustate = ctxt.getUserState();
            T tmp = default(T);
            foreach (Parser<T> p in ps)
            {
                ctxt.setState(original_step, original_at);
                if (p.parse(ctxt, ref tmp, ref err))
                {
                    int at2 = ctxt.getAt();
                    if (ord(at2, most))
                    {
                        most = at2;
                        mstep = ctxt.getStep();
                        result = tmp;
                        //mustate = ctxt.getUserState();
                    }
                }
                else if (err != null && err.Thrown)
                {
                    return false;
                }
            }
            ctxt.setState(mstep, most);
            return true;
        }
        internal static AbstractParsecError reportErrorExpecting(string s, ParseContext ctxt, AbstractParsecError err)
        {
            return err == null ? raiseExpecting(s, ctxt) : err.setExpecting(s);
        }
        internal static ParsecError newException(object e, ParseContext ctxt)
        {
            return ParsecError.throwException(ctxt.getIndex(), e);
        }
        internal static bool accm_repeat<From, To>(Accumulator<From, To> acc,
          int n, Parser<From> p, ParseContext ctxt, ref AbstractParsecError err)
        {
            From tmp = default(From);
            for (int i = 0; i < n; i++)
            {
                if (p.parse(ctxt, ref tmp, ref err))
                {
                    acc.Accumulate(tmp);
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        internal static bool accm_many<From, To>(Accumulator<From, To> acc, Parser<From> p, ParseContext ctxt,
          ref To result, ref AbstractParsecError err)
        {
            From tmp = default(From);
            int at = ctxt.getAt();
            for (; ; )
            {
                if (p.parse(ctxt, ref tmp, ref err))
                {
                    int at2 = ctxt.getAt();
                    if (isInfiniteLoop(at, at2))
                    {
                        break;
                    }
                    at = at2;
                    acc.Accumulate(tmp);
                    continue;
                }
                else if (err != null && err.Thrown)
                {
                    result = acc.GetResult();
                    return false;
                }
                else if (ctxt.getAt() - at >= 1)
                {
                    return false;
                }
                else
                    break;
            }
            result = acc.GetResult();
            return true;
        }
        internal static bool accm_some<From, To>(Accumulator<From, To> acc, int max,
          Parser<From> p, ParseContext ctxt, ref To result, ref AbstractParsecError err)
        {
            From tmp = default(From);
            for (int i = 0; i < max; i++)
            {
                int at = ctxt.getAt();
                if (p.parse(ctxt, ref tmp, ref err))
                {
                    acc.Accumulate(tmp);
                    continue;
                }
                else if (err != null && err.Thrown)
                {
                    result = acc.GetResult();
                    return false;
                }
                else if (ctxt.getAt() - at >= 1)
                {
                    return false;
                }
                else
                {
                    break;
                }
            }
            result = acc.GetResult();
            return true;
        }
        internal static bool run_repeat<T>(int n, Parser<T> p, ParseContext ctxt,
          ref T result, ref AbstractParsecError err)
        {
            for (int i = 0; i < n; i++)
            {
                if (!p.parse(ctxt, ref result, ref err))
                    return false;
            }
            return true;
        }
        internal static bool run_many<T>(Parser<T> p, ParseContext ctxt,
          ref T result, ref AbstractParsecError err)
        {
            int at = ctxt.getAt();
            for (; ; )
            {
                if (p.parse(ctxt, ref result, ref err))
                {
                    int at2 = ctxt.getAt();
                    if (isInfiniteLoop(at, at2))
                    {
                        return true;
                    }
                    at = at2;
                    continue;
                }
                else if (ctxt.getAt() - at >= 1 || err != null && err.Thrown)
                    return false;
                else
                    return true;
            }
        }
        internal static bool run_some<T>(int max, Parser<T> p, ParseContext ctxt,
          ref T result, ref AbstractParsecError err)
        {
            for (int i = 0; i < max; i++)
            {
                int at = ctxt.getAt();
                if (p.parse(ctxt, ref result, ref err))
                    continue;
                else if (ctxt.getAt() - at >= 1 || err != null && err.Thrown)
                    return false;
                else
                    return true;
            }
            return true;
        }
        internal static bool isInfiniteLoop(int at0, int at1)
        {
            return (at0 == at1);
        }
        internal static T postfix_thread_maps<T>(T a, Map<T, T>[] ms)
        {
            foreach (Map<T, T> m in ms)
            {
                a = m(a);
            }
            return a;
        }
        internal static T prefix_thread_maps<T>(Map<T, T>[] ms, T a)
        {
            for (int i = ms.Length - 1; i >= 0; i--)
            {
                Map<T, T> m = ms[i];
                a = m(a);
            }
            return a;
        }
        /// <summary>
        /// Continue to run a Parser object and populate the outer context with the parse result.
        /// </summary>
        /// <param name="outer_ctxt">the outer context.</param>
        /// <param name="inner_ctxt">the temporary context object to run this parser in.</param>
        /// <param name="nested">the nested parser object to run.</param>
        /// <param name="result">the result of the parsing.</param>
        /// <param name="err">the error during parsing</param>
        /// <returns>succeed or not.</returns>
        internal static bool cont<T>(ParseContext outer_ctxt, ParseContext inner_ctxt, Parser<T> nested,
          ref T result, ref AbstractParsecError err)
        {
            bool ok = nested.parse(inner_ctxt, ref result, ref err);
            //outer_ctxt.setUserState (inner_ctxt.getUserState ());
            if (ok)
            {
                outer_ctxt.setStep(outer_ctxt.getStep() + inner_ctxt.getStep());
            }
            else
            {
                outer_ctxt.setAt(inner_ctxt.getIndex());
            }
            return ok;
        }
    }

    class ActionParser<T> : Parser<T>
    {
        private readonly Proc action;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            action();
            return true;
        }
        internal ActionParser(string n, Proc action)
            : base(n)
        {
            this.action = action;
        }
    }
    class OneParser<T> : Parser<T>
    {
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            return true;
        }
        internal override bool apply(ParseContext ctxt, int look_ahead, ref T result, ref AbstractParsecError err)
        {
            return true;
        }
    }
    class ZeroParser<T> : Parser<T>
    {
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            err = null;
            return false;
        }
        internal override bool apply(ParseContext ctxt, int look_ahead, ref T result, ref AbstractParsecError err)
        {
            err = null;
            return false;
        }
        internal static readonly Parser<T> instance = new ZeroParser<T>();
    }
    class ReturnParser<T> : Parser<T>
    {
        readonly T v;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            result = v;
            return true;
        }
        internal override bool apply(ParseContext ctxt, int look_ahead, ref T result, ref AbstractParsecError err)
        {
            result = v;
            return true;
        }
        internal ReturnParser(T v)
        {
            this.v = v;
        }
    }
    class BoundParser<A, T> : Parser<T>
    {
        readonly Parser<A> p;
        readonly Binder<A, T> f;
        internal BoundParser(Parser<A> p, Binder<A, T> f)
        {
            this.p = p;
            this.f = f;
        }
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (p.parse(ctxt, ref a, ref err))
            {
                return f(a).parse(ctxt, ref result, ref err);
            }
            else
            {
                return false;
            }
        }
    }
    class IdBinder<T>
    {
        static internal Parser<T> bind(T v)
        {
            return Parsers.Return(v);
        }
        internal static Binder<T, T> instance = new Binder<T, T>(bind);
    }
    class SequenceBinder<T>
    {
        static bool run(ref T v, Binder<T, T>[] binders, ParseContext ctxt, ref AbstractParsecError err)
        {
            foreach (Binder<T, T> b in binders)
            {
                if (!b(v).parse(ctxt, ref v, ref err))
                {
                    return false;
                }
            }
            return true;
        }
        readonly Binder<T, T>[] arr;
        internal SequenceBinder(Binder<T, T>[] binders)
        {
            this.arr = binders;
        }
        internal Parser<T> bind(T v)
        {
            return new AllBoundParser(v, arr);
        }
        class AllBoundParser : Parser<T>
        {
            readonly Binder<T, T>[] binders;
            readonly T v;
            internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
            {
                result = v;
                return run(ref result, binders, ctxt, ref err);
            }
            internal AllBoundParser(T v, Binder<T, T>[] binders)
            {
                this.v = v;
                this.binders = binders;
            }
        }
    }

    class FollowedByParser<R, A> : Parser<R>
    {
        readonly Parser<R> p1;
        readonly Parser<A> p2;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            if (!p1.parse(ctxt, ref result, ref err))
            {
                return false;
            }
            A a = default(A);
            return p2.parse(ctxt, ref a, ref err);
        }
        internal FollowedByParser(Parser<R> p1, Parser<A> p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
    class SeqParser<A, R> : Parser<R>
    {
        readonly Parser<A> p1;
        readonly Parser<R> p2;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (!p1.parse(ctxt, ref a, ref err))
            {
                return false;
            }
            return p2.parse(ctxt, ref result, ref err);
        }
        internal SeqParser(Parser<A> p1, Parser<R> p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
    class SeqParser<A, B, R> : Parser<R>
    {
        readonly Parser<A> p1;
        readonly Parser<B> p2;
        readonly Parser<R> p3;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (!p1.parse(ctxt, ref a, ref err))
            {
                return false;
            }
            B b = default(B);
            if (!p2.parse(ctxt, ref b, ref err))
            {
                return false;
            }
            return p3.parse(ctxt, ref result, ref err);
        }
        internal SeqParser(Parser<A> p1, Parser<B> p2, Parser<R> p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }
    class SeqParser<A, B, C, R> : Parser<R>
    {
        readonly Parser<A> p1;
        readonly Parser<B> p2;
        readonly Parser<C> p3;
        readonly Parser<R> p4;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (!p1.parse(ctxt, ref a, ref err))
            {
                return false;
            }
            B b = default(B);
            if (!p2.parse(ctxt, ref b, ref err))
            {
                return false;
            }
            C c = default(C);
            if (!p3.parse(ctxt, ref c, ref err))
            {
                return false;
            }
            return p4.parse(ctxt, ref result, ref err);
        }
        internal SeqParser(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<R> p4)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }
    }
    class SeqParser<A, B, C, D, R> : Parser<R>
    {
        readonly Parser<A> p1;
        readonly Parser<B> p2;
        readonly Parser<C> p3;
        readonly Parser<D> p4;
        readonly Parser<R> p5;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (!p1.parse(ctxt, ref a, ref err))
            {
                return false;
            }
            B b = default(B);
            if (!p2.parse(ctxt, ref b, ref err))
            {
                return false;
            }
            C c = default(C);
            if (!p3.parse(ctxt, ref c, ref err))
            {
                return false;
            }
            D d = default(D);
            if (!p4.parse(ctxt, ref d, ref err))
            {
                return false;
            }
            return p5.parse(ctxt, ref result, ref err);
        }
        internal SeqParser(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<D> p4, Parser<R> p5)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.p5 = p5;
        }
    }
    class SequenceParser<T> : Parser<T>
    {
        readonly Parser<T>[] parsers;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            foreach (Parser<T> p in parsers)
            {
                if (!p.parse(ctxt, ref result, ref err))
                {
                    return false;
                }
            }
            return true;
        }
        internal SequenceParser(Parser<T>[] parsers)
        {
            this.parsers = parsers;
        }
    }
    abstract class DelegatingParser<T> : Parser<T>
    {
        protected readonly Parser<T> parser;
        protected DelegatingParser(Parser<T> parser)
        {
            this.parser = parser;
        }
    }
    class SteppedParser<T> : DelegatingParser<T>
    {
        readonly int n;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            if (!parser.parse(ctxt, ref result, ref err))
            {
                return false;
            }
            ctxt.setStep(original_step + n);
            return true;
        }
        internal SteppedParser(Parser<T> p, int n)
            : base(p)
        {
            this.n = n;
        }
    }

    class LookaheadParser<T> : DelegatingParser<T>
    {
        readonly int la;
        internal LookaheadParser(Parser<T> p, int la)
            : base(p)
        {
            this.la = la;
        }
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            return parser.parse(ctxt, this.la, ref result, ref err);
        }
        internal override bool apply(ParseContext ctxt, int la, ref T result, ref AbstractParsecError err)
        {
            return parser.parse(ctxt, la, ref result, ref err);
        }
    }
    class TracedParser<T> : DelegatingParser<T>
    {
        readonly Tracer<T> tracer;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            bool ok = parser.parse(ctxt, ref result, ref err);
            tracer(ok, result, err == null ? null : err.Exception,
              ctxt.getSource(), ctxt.getIndex(), ctxt.getStep() - original_step, ctxt.getAt() - original_at);
            return ok;
        }
        internal TracedParser(Parser<T> p, Tracer<T> tracer)
            : base(p)
        {
            this.tracer = tracer;
        }
    }
    class MapnParser<From, To> : Parser<To>
    {
        readonly Parser<From>[] parsers;
        readonly Mapn<From, To> map;
        readonly ArrayFactory<From> factory;
        internal override bool apply(ParseContext ctxt, ref To result, ref AbstractParsecError err)
        {
            From[] args = factory(parsers.Length);
            int i = 0;
            foreach (Parser<From> p in parsers)
            {
                if (!p.parse(ctxt, ref args[i++], ref err))
                {
                    return false;
                }
            }
            result = map(args);
            return true;
        }
        internal MapnParser(ArrayFactory<From> factory, Parser<From>[] parsers, Mapn<From, To> map)
        {
            this.factory = factory;
            this.parsers = parsers;
            this.map = map;
        }
    }

    class FailingParser<T> : Parser<T>
    {
        readonly string msg;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            err = ParserChores.raiseRaw(msg, ctxt);
            return false;
        }
        internal FailingParser(string msg)
        {
            this.msg = msg;
        }
    }
    class IfElseParser<C, R> : Parser<R>
    {
        readonly Parser<C> p1;
        readonly Binder<C, R> consequence;
        readonly Parser<R> alternative;
        internal IfElseParser(Parser<C> p1, Binder<C, R> yes, Parser<R> no)
        {
            this.p1 = p1;
            this.consequence = yes;
            this.alternative = no;
        }
        internal override bool apply(ParseContext ctxt, int look_ahead, ref R result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            //object original_userstate = ctxt.getUserState ();
            C tmp = default(C);
            if (p1.parse(ctxt, ref tmp, ref err))
            {
                return consequence(tmp).parse(ctxt, ref result, ref err);
            }
            else
            {
                return ParserChores.recover(look_ahead, alternative, ctxt, ref result, ref err,
                  original_step, original_at);
            }
        }
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            return apply(ctxt, 1, ref result, ref err);
        }
    }
    class SumParser<T> : Parser<T>
    {
        readonly Parser<T>[] parsers;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            return apply(ctxt, 1, ref result, ref err);
        }
        internal override bool apply(ParseContext ctxt, int look_ahead, ref T result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            foreach (Parser<T> p in parsers)
            {
                AbstractParsecError error = err;
                if (p.parse(ctxt, ref result, ref err))
                    return true;
                if (err != null && err.Thrown)
                {
                    return false;
                }
                if (ctxt.getAt() != original_at && ctxt.getStep() - original_step >= look_ahead)
                    return false;
                err = AbstractParsecError.mergeError(error, err);
                ctxt.setState(original_step, original_at);
            }
            return false;
        }
        internal SumParser(Parser<T>[] parsers)
        {
            this.parsers = parsers;
        }
        internal override Parser<T>[] Alternatives
        {
            get { return parsers; }
        }
    }

    class FavoredSumParser<T> : Parser<T>
    {
        readonly Parser<T>[] parsers;
        readonly IntOrder compare;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            //object original_userstate = ctxt.getUserState ();
            AbstractParsecError error = null;
            int i = 0;
            foreach (Parser<T> p in parsers)
            {
                if (p.parse(ctxt, ref result, ref err))
                {
                    return ParserChores.best(compare, parsers, i + 1, ctxt, ref result, ref err,
                      original_step, original_at);
                }
                if (err != null && err.Thrown)
                    return false;
                //in alternate, we do not care partial match.
                error = AbstractParsecError.mergeError(error, err);
                ctxt.setState(original_step, original_at);
                i++;
            }
            err = error;
            return false;
        }
        internal FavoredSumParser(Parser<T>[] parsers, IntOrder order)
        {
            this.parsers = parsers;
            this.compare = order;
        }
    }

    class IsConsumedParser<T> : DelegatingParser<T>
    {
        readonly string msg;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            if (!parser.parse(ctxt, ref result, ref err))
            {
                return false;
            }
            if (ctxt.getAt() != original_at)
            {
                ctxt.setState(original_step, original_at);
                return true;
            }
            err = ParserChores.raiseRaw(msg, ctxt);
            return false;
        }
        internal IsConsumedParser(Parser<T> parser, string err)
            : base(parser)
        {
            this.msg = err;
        }
    }
    class NotConsumedParser<T> : DelegatingParser<T>
    {
        readonly string msg;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            if (!parser.parse(ctxt, ref result, ref err))
            {
                return false;
            }
            if (ctxt.getAt() == original_at)
            {
                return true;
            }
            ctxt.setState(original_step, original_at);
            err = ParserChores.raiseRaw(msg, ctxt);
            return false;
        }
        internal NotConsumedParser(Parser<T> parser, string err)
            : base(parser)
        {
            this.msg = err;
        }
    }
    /*
    class TransformUserStateParser : Parser<object>
    {
      readonly Map<object, object> transform;
      internal override bool apply(ParseContext ctxt, ref object result, ref AbstractParsecError err) {
        object ustate = ctxt.getUserState();
        ctxt.setUserState(transform(ustate));
        result = ustate;
        return true;
      }
      internal TransformUserStateParser (Map<object, object> transformer) {
        this.transform = transformer;
      }
    }*/

    class GetIndexParser : Parser<int>
    {
        internal override bool apply(ParseContext ctxt, ref int result, ref AbstractParsecError err)
        {
            result = ctxt.getIndex();
            return true;
        }
        internal GetIndexParser() { }
    }

    class PeekParser<T> : DelegatingParser<T>
    {
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            bool ok = parser.parse(ctxt, ref result, ref err);
            ctxt.setState(original_step, original_at);
            return ok;
        }
        internal PeekParser(Parser<T> parser) : base(parser) { }
    }

    class AtomParser<T> : DelegatingParser<T>
    {
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            int original_step = ctxt.getStep();
            int original_at = ctxt.getAt();
            if (!parser.parse(ctxt, ref result, ref err))
            {
                ctxt.setState(original_step, original_at);
                return false;
            }
            else
            {
                ctxt.setStep(original_step + 1);
                return true;
            }
        }
        internal AtomParser(Parser<T> parser) : base(parser) { }
    }

    class TryParser<T> : DelegatingParser<T>
    {
        readonly Catch<T> handle;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            AbstractParsecError original_err = err;
            if (parser.parse(ctxt, ref result, ref err))
            {
                return true;
            }
            if (err == null || !err.Thrown)
            {
                return false;
            }
            Parser<T> hdl = handle(result, err.Exception);
            err = original_err;
            return hdl.parse(ctxt, ref result, ref err);
        }
        internal TryParser(Parser<T> parser, Catch<T> handle)
            : base(parser)
        {
            this.handle = handle;
        }
    }

    class MappedParser<A, R> : Parser<R>
    {
        readonly Parser<A> parser;
        readonly Map<A, R> map;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A tmp = default(A);
            if (!parser.parse(ctxt, ref tmp, ref err))
                return false;
            result = map(tmp);
            return true;
        }
        internal MappedParser(Parser<A> parser, Map<A, R> map)
        {
            this.parser = parser;
            this.map = map;
        }
    }

    class MappedParser<A, B, R> : Parser<R>
    {
        readonly Parser<A> p1;
        readonly Parser<B> p2;
        readonly Map<A, B, R> map;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (!p1.parse(ctxt, ref a, ref err))
                return false;
            B b = default(B);
            if (!p2.parse(ctxt, ref b, ref err))
                return false;
            result = map(a, b);
            return true;
        }
        internal MappedParser(Parser<A> p1, Parser<B> p2, Map<A, B, R> map)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.map = map;
        }
    }
    class MappedParser<A, B, C, R> : Parser<R>
    {
        readonly Parser<A> p1;
        readonly Parser<B> p2;
        readonly Parser<C> p3;
        readonly Map<A, B, C, R> map;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (!p1.parse(ctxt, ref a, ref err))
                return false;
            B b = default(B);
            if (!p2.parse(ctxt, ref b, ref err))
                return false;
            C c = default(C);
            if (!p3.parse(ctxt, ref c, ref err))
                return false;
            result = map(a, b, c);
            return true;
        }
        internal MappedParser(Parser<A> p1, Parser<B> p2, Parser<C> p3, Map<A, B, C, R> map)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.map = map;
        }
    }
    class MappedParser<A, B, C, D, R> : Parser<R>
    {
        readonly Parser<A> p1;
        readonly Parser<B> p2;
        readonly Parser<C> p3;
        readonly Parser<D> p4;
        readonly Map<A, B, C, D, R> map;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (!p1.parse(ctxt, ref a, ref err))
                return false;
            B b = default(B);
            if (!p2.parse(ctxt, ref b, ref err))
                return false;
            C c = default(C);
            if (!p3.parse(ctxt, ref c, ref err))
                return false;
            D d = default(D);
            if (!p4.parse(ctxt, ref d, ref err))
                return false;
            result = map(a, b, c, d);
            return true;
        }
        internal MappedParser(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<D> p4,
          Map<A, B, C, D, R> map)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.map = map;
        }
    }
    class MappedParser<A, B, C, D, E, R> : Parser<R>
    {
        readonly Parser<A> p1;
        readonly Parser<B> p2;
        readonly Parser<C> p3;
        readonly Parser<D> p4;
        readonly Parser<E> p5;
        readonly Map<A, B, C, D, E, R> map;
        internal override bool apply(ParseContext ctxt, ref R result, ref AbstractParsecError err)
        {
            A a = default(A);
            if (!p1.parse(ctxt, ref a, ref err))
                return false;
            B b = default(B);
            if (!p2.parse(ctxt, ref b, ref err))
                return false;
            C c = default(C);
            if (!p3.parse(ctxt, ref c, ref err))
                return false;
            D d = default(D);
            if (!p4.parse(ctxt, ref d, ref err))
                return false;
            E e = default(E);
            if (!p5.parse(ctxt, ref e, ref err))
                return false;
            result = map(a, b, c, d, e);
            return true;
        }
        internal MappedParser(Parser<A> p1, Parser<B> p2, Parser<C> p3, Parser<D> p4, Parser<E> p5,
          Map<A, B, C, D, E, R> map)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.p5 = p5;
            this.map = map;
        }
    }

    class IsTokenParser<T> : Parser<T>
    {
        readonly FromToken<T> ft;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            if (ctxt.isEof())
            {
                err = ctxt.getSysUnexpected();
                return false;
            }
            Tok tok = ctxt.getToken();
            if (!ft(tok, ref result))
            {
                err = ctxt.getSysUnexpected();
                return false;
            }
            ctxt.next();
            return true;
        }
        internal IsTokenParser(FromToken<T> ft)
        {
            this.ft = ft;
        }
    }
    /*
    class LabelledParser<T> : DelegatingParser<T>
    {
      readonly string lbl;
      internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err) {
        int at = ctxt.getAt ();
        Either<T> result = parser.parse (ctxt);
        if (ctxt.getAt () != at)
          return result;
        if (result.Ok)
          return result;
        return ParserChores.reportErrorExpecting<T> (lbl, ctxt, result.Error);
      }
      internal LabelledParser (Parser<T> parser, string lbl) : base(parser){
        this.lbl = lbl;
      }
    }*/
    class ExpectingParser<T> : Parser<T>
    {
        readonly string lbl;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            err = ParserChores.raiseExpecting(lbl, ctxt);
            return false;
        }
        internal ExpectingParser(string lbl)
        {
            this.lbl = lbl;
        }
    }
    class UnexpectedParser<T> : Parser<T>
    {
        readonly string errmsg;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            err = ParserChores.raiseUnexpected(errmsg, ctxt);
            return false;
        }
        internal UnexpectedParser(string msg)
        {
            this.errmsg = msg;
        }
    }
    class ThrowingParser<T> : Parser<T>
    {
        readonly object e;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            err = ParserChores.newException(e, ctxt);
            return false;
        }
        internal ThrowingParser(object e)
        {
            this.e = e;
        }
    }
    class RepeatParser_<T> : Parser<T>
    {
        readonly Parser<T> parser;
        readonly int n;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            return ParserChores.run_repeat(n, parser, ctxt, ref result, ref err);
        }
        internal RepeatParser_(Parser<T> parser, int n)
        {
            this.parser = parser;
            this.n = n;
        }
    }
    class RepeatParser<T> : Parser<T[]>
    {
        readonly Parser<T> parser;
        readonly ArrayFactory<T> factory;
        readonly int n;
        internal override bool apply(ParseContext ctxt, ref T[] result, ref AbstractParsecError err)
        {
            T[] ret = factory(n);
            for (int i = 0; i < n; i++)
            {
                if (!parser.parse(ctxt, ref ret[i], ref err))
                {
                    return false;
                }
            }
            result = ret;
            return true;
        }
        internal RepeatParser(Parser<T> parser, ArrayFactory<T> factory, int n)
        {
            this.parser = parser;
            this.factory = factory;
            this.n = n;
        }
    }
    class ManyMinParser<From, To> : Parser<To>
    {
        readonly Generator<Accumulator<From, To>> accm;
        readonly int min;
        readonly Parser<From> parser;
        internal ManyMinParser(Generator<Accumulator<From, To>> accm, int min, Parser<From> parser)
        {
            this.accm = accm;
            this.min = min;
            this.parser = parser;
        }
        internal override bool apply(ParseContext ctxt, ref To result, ref AbstractParsecError err)
        {
            Accumulator<From, To> acc = accm();
            if (!ParserChores.accm_repeat(acc, min, parser, ctxt, ref err))
                return false;
            return ParserChores.accm_many(acc, parser, ctxt, ref result, ref err);
        }
    }
    class ManyMinParser_<T> : Parser<T>
    {
        readonly int min;
        readonly Parser<T> parser;
        internal ManyMinParser_(int min, Parser<T> parser)
        {
            this.min = min;
            this.parser = parser;
        }
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            if (!ParserChores.run_repeat(min, parser, ctxt, ref result, ref err))
                return false;
            return ParserChores.run_many(parser, ctxt, ref result, ref err);
        }
    }
    class ManyParser<From, To> : Parser<To>
    {
        readonly Generator<Accumulator<From, To>> accm;
        readonly Parser<From> parser;
        internal ManyParser(Generator<Accumulator<From, To>> accm, Parser<From> parser)
        {
            this.accm = accm;
            this.parser = parser;
        }
        internal override bool apply(ParseContext ctxt, ref To result, ref AbstractParsecError err)
        {
            Accumulator<From, To> acc = accm();
            return ParserChores.accm_many(acc, parser, ctxt, ref result, ref err);
        }
    }
    class ManyParser_<T> : Parser<T>
    {
        readonly Parser<T> parser;
        internal ManyParser_(Parser<T> parser)
        {
            this.parser = parser;
        }
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            return ParserChores.run_many(parser, ctxt, ref result, ref err);
        }
    }
    class SomeMinParser<From, To> : Parser<To>
    {
        readonly Generator<Accumulator<From, To>> accm;
        readonly int min;
        readonly int max;
        readonly Parser<From> parser;
        internal SomeMinParser(Generator<Accumulator<From, To>> accm, int min, int max, Parser<From> parser)
        {
            this.accm = accm;
            this.min = min;
            this.max = max;
            this.parser = parser;
        }
        internal override bool apply(ParseContext ctxt, ref To result, ref AbstractParsecError err)
        {
            Accumulator<From, To> acc = accm();
            if (!ParserChores.accm_repeat(acc, min, parser, ctxt, ref err))
                return false;
            return ParserChores.accm_some(acc, max - min, parser, ctxt, ref result, ref err);
        }
    }
    class SomeMinParser_<T> : Parser<T>
    {
        readonly int min;
        readonly int max;
        readonly Parser<T> parser;
        internal SomeMinParser_(int min, int max, Parser<T> parser)
        {
            this.min = min;
            this.max = max;
            this.parser = parser;
        }
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            if (!ParserChores.run_repeat(min, parser, ctxt, ref result, ref err))
                return false;
            return ParserChores.run_some(max - min, parser, ctxt, ref result, ref err);
        }
    }
    class SomeParser<From, To> : Parser<To>
    {
        readonly Generator<Accumulator<From, To>> accm;
        readonly int max;
        readonly Parser<From> parser;
        internal SomeParser(Generator<Accumulator<From, To>> accm, int max, Parser<From> parser)
        {
            this.accm = accm;
            this.max = max;
            this.parser = parser;
        }
        internal override bool apply(ParseContext ctxt, ref To result, ref AbstractParsecError err)
        {
            Accumulator<From, To> acc = accm();
            return ParserChores.accm_some(acc, max, parser, ctxt, ref result, ref err);
        }
    }
    class SomeParser_<T> : Parser<T>
    {
        readonly int max;
        readonly Parser<T> parser;
        internal SomeParser_(int max, Parser<T> parser)
        {
            this.max = max;
            this.parser = parser;
        }
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            return ParserChores.run_some(max, parser, ctxt, ref result, ref err);
        }
    }
    class EofParser<T> : Parser<T>
    {
        readonly string msg;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            if (ctxt.isEof())
                return true;
            else
            {
                AbstractParsecError expecting = ParserChores.raiseExpecting(msg, ctxt);
                AbstractParsecError unexpected = ctxt.getSysUnexpected();
                err = AbstractParsecError.mergeError(expecting, unexpected);
                return false;
            }
        }
        internal EofParser(string msg)
        {
            this.msg = msg;
        }
    }
    /*
    class DelimitedParser<D, From, To> : Parser<To>
    {
      readonly Generator<Accumulator<From, To>> accm;
      readonly Parser<D> delim;
      readonly Parser<From> parser;
      internal override bool apply(ParseContext ctxt, ref To result, ref AbstractParsecError err) {
        Accumulator<From,To> acc = accm();
        for (;;) {
          int at0 = ctxt.getAt ();
          Either<D> d = delim.parse (ctxt);
          int at1 = ctxt.getAt ();
          if (d.Failed) {
            if (at0 != at1) {
              return Either.left<To>(d.Error);
            }
            return Either.right (acc.getResult ());
          }
          Either<From> r = parser.parse (ctxt);
          int at2 = ctxt.getAt ();
          if (r.Failed) {
            if (at1 != at2) {
              return Either.left<To> (r.Error);
            }
            else {
              return Either.right (acc.getResult ());
            }
          }
          if (ParserChores.IsInfiniteLoop (at0, at2)) {
            return Either.right(acc.getResult());
          }
          acc.accumulate (r.Value);
        }
      }
      internal DelimitedParser (Generator<Accumulator<From, To>> accm, Parser<D> delim, Parser<From> parser) {
        this.accm = accm;
        this.delim = delim;
        this.parser = parser;
      }
    }
    */
    class LassocAccumulator<T> : Accumulator<Pair<Map<T, T, T>, T>, T>
    {
        T val;
        public T GetResult()
        {
            return val;
        }

        public void Accumulate(Pair<Map<T, T, T>, T> tuple)
        {
            val = tuple.V1(val, tuple.V2);
        }
        internal LassocAccumulator(T v)
        {
            this.val = v;
        }
    }
    class LexerThenParser<T> : Parser<T>
    {
        readonly Lexeme lexeme;
        readonly string module;
        readonly string eof_title;
        readonly ShowToken show;
        readonly Parser<T> parser;
        internal LexerThenParser(string module, string eof_title, ShowToken show,
          Lexeme lexeme, Parser<T> p)
        {
            this.lexeme = lexeme;
            this.module = module;
            this.eof_title = eof_title;
            this.show = show;
            this.parser = p;
        }
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            Tok[] tokens = null;
            if (!lexeme.parse(ctxt, ref tokens, ref err))
            {
                return false;
            }
            ParserState s0 = new ParserState(tokens, 0, module,
              ctxt.getPositionMap(), ctxt.getIndex(), eof_title, show);
            return ParserChores.cont(ctxt, s0, parser, ref result, ref err);
        }
    }
    /*
    class IsUserStateParser<T> : Parser<T>
    {
      readonly Predicate<object> pred;
      internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err) {
        return pred(ctxt.getUserState());
      }
      internal IsUserStateParser (Predicate<object> pred) {
        this.pred = pred;
      }
    }*/
    class LazyParser<T> : Parser<T>
    {
        readonly Generator<Parser<T>> generator;
        internal override bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            return generator().apply(ctxt, ref result, ref err);
        }
        internal override bool apply(ParseContext ctxt, int look_ahead, ref T result, ref AbstractParsecError err)
        {
            return generator().apply(ctxt, look_ahead, ref result, ref err);
        }
        internal LazyParser(Generator<Parser<T>> generator)
        {
            this.generator = generator;
        }
    }
    /*
    class ConvertingParser<From, To> : Parser<To>
      where To:class
    {
      readonly Parser<From> parser;
      internal override bool apply(ParseContext ctxt, ref To result, ref AbstractParsecError err) {
        return parser.apply (ctxt).convert<To>();
      }
      internal override Either<To> apply (ParseContext ctxt, int look_ahead) {
        return parser.apply (ctxt, look_ahead).convert<To>();
      }
      internal ConvertingParser (Parser<From> parser) {
        this.parser = parser;
      }
    }*/
}
