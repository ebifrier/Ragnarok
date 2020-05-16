using System;
using System.Collections.Generic;
using System.Text;

namespace Codehaus.Parsec
{
    /// <summary> This class is an indicator of "no value". It is introduced to overcome the type problem in generics. 
    /// <p />
    /// </summary>
    /// <author>  Ben Yu
    /// Apr 14, 2006 9:18:30 AM
    /// </author>
    /// <since> version 1.0
    /// </since>
    public class D_ { }
    public interface Parser
    {
        string Name
        {
            get;
            set;
        }
    }

    /// <summary> <p>
    /// A parser runs either on character level or token level.
    /// It takes as input a string object or a PostionedToken[] array,
    /// recognizes certain patterns and returns a value.
    /// </p>
    /// <p>
    /// <ul>
    /// <li>A character level parser object that simply recognizes input but not returning token is called a scanner.</li>
    /// <li>A character level parser object that recognizes input and returns a token object is called a lexer.</li>
    /// <li>A token level parser is called a parser. </li>
    /// <li>a parser object can fail, or "return" a value via the retn() function.</li>
    /// <li>a parser object can throw pseudo-exception object for other parsers to catch.</li>
    /// <li>a parser object represents a parsing computation algorithm, not the actual parsing process.</li>
    /// <li>a parser object is executed by Parsers.RunParser() function.</li>
    /// </ul>
    /// </p>
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// 2004-11-11
    /// </author>
    [System.Serializable]
    public abstract class Parser<T> : Parser
    {
        /// <summary>
        /// The Parser name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }
        /// <summary>
        /// Set the parser name and return this parser object.
        /// </summary>
        /// <param name="name">the parser name</param>
        /// <returns>this parser object</returns>
        public Parser<T> Rename(string name)
        {
            Name = name;
            return this;
        }
        /// <summary>
        /// Create a Parser that can calls Tracer when it terminates.
        /// Only enabled when Parsers.Debug is true.
        /// </summary>
        /// <param name="tracer">the tracer object.</param>
        /// <returns>the result Parser object.</returns>
        internal Parser<T> Trace(Tracer<T> tracer)
        {
            if (!Parsers.Debug) return this;
            return new TracedParser<T>(this, tracer);
        }
        /// <summary>
        /// Create a Parser that can calls Tracer when it fails.
        /// Only enabled when Parsers.Debug is true.
        /// </summary>
        /// <param name="tracer">the tracer object.</param>
        /// <returns>the result Parser object.</returns>
        public Parser<T> OnError(ErrorTrace tracer)
        {
            return Trace(delegate(bool ok, T result, object exception,
              string src, int ind, int steps, int offset)
            {
                if (!ok)
                {
                    tracer(exception, src, ind, steps, offset);
                }
            });
        }
        /// <summary>
        /// Create a Parser that can calls Tracer when it succeeds.
        /// Only enabled when Parsers.Debug is true.
        /// </summary>
        /// <param name="tracer">the tracer object.</param>
        /// <returns>the result Parser object.</returns>
        public Parser<T> OnSuccess(GoodTrace<T> tracer)
        {
            return Trace(delegate(bool ok, T result, object exception,
              string src, int ind, int steps, int offset)
            {
                if (ok)
                {
                    tracer(result, src, ind, steps, offset);
                }
            });
        }
        /// <summary>
        /// Print error to standard output when this parser fails.
        /// </summary>
        /// <param name="name">the name displayed in error message. Use a unique name to identify.</param>
        /// <param name="min_steps">the minimal logical steps to trigger the trace message.</param>
        /// <returns>the result parser object.</returns>
        public Parser<T> PrintError(string name, int min_steps)
        {
            return OnError(Traces.PrintError(name, Console.Out, min_steps));
        }
        /// <summary>
        /// Print error to standard output when this parser fails.
        /// the minimal logical steps to trigger the error message is 1.
        /// </summary>
        /// <param name="name">the name displayed in error message. Use a unique name to identify.</param>
        /// <returns>the result parser object.</returns>
        public Parser<T> PrintError(string name)
        {
            return PrintError(name, 1);
        }
        /// <summary>
        /// Print result to standard output when this parser succeeds.
        /// </summary>
        /// <param name="name">the name displayed in trace message. Use a unique name to identify.</param>
        /// <returns>the result parser object.</returns>
        public Parser<T> PrintResult(string name)
        {
            return OnSuccess(Traces.PrintResult<T>(name, Console.Out));
        }
        /// <summary>
        /// Print the error or result to the standard output when this parser terminates.
        /// the minimal logical steps to trigger the error message is 1.
        /// </summary>
        /// <param name="name">the name displayed in trace message. Use a unique name to identify.</param>
        /// <returns>the result parser object.</returns>
        public Parser<T> PrintTrace(string name)
        {
            return PrintError(name, 1).PrintResult(name);
        }
        /// <summary>
        /// Create a Logical Or parser.
        /// When trying to create a Parser with more than 2 alternatives, Parsers.Plus is preferred
        /// for better performance.
        /// </summary>
        /// <param name="p1">the first alternative.</param>
        /// <param name="p2">the second alternative.</param>
        /// <returns>the result parser.</returns>
        public static Parser<T> operator |(Parser<T> p1, Parser<T> p2)
        {
            //try to unwrap the tree so that the parsing is more efficient.
            Parser<T>[] alts1 = p1.Alternatives;
            Parser<T>[] alts2 = p2.Alternatives;
            if (alts1 == null && alts2 == null)
            {
                return Parsers.Plus(p1, p2);
            }
            else
            {
                int len1 = alts1 == null ? 1 : alts1.Length;
                int len2 = alts2 == null ? 1 : alts2.Length;
                Parser<T>[] alts = new Parser<T>[len1 + len2];
                if (alts1 == null)
                {
                    alts[0] = p1;
                }
                else
                {
                    Array.Copy(alts1, alts, len1);
                }
                if (alts2 == null)
                {
                    alts[len1] = p2;
                }
                else
                {
                    Array.Copy(alts2, 0, alts, len1, len2);
                }
                return Parsers.Plus(alts);
            }
        }

        ParsingFrame getErrorFrame(ParseContext ctxt, int ind)
        {
            return new ParsingFrame(ctxt.getModuleName(), ctxt.getIndex(), ctxt.getPositionMap().ToPos(ind), this);
        }
        Exception wrapException(Exception e, ParseContext ctxt, int ind)
        {
            if (e is UserException)
            {
                return (UserException)e;
            }
            else if (e is ParserException)
            {
                ParserException pe = ((ParserException)e);
                pe.pushFrame(getErrorFrame(ctxt, ind));
                return pe;
            }
            else
            {
                ParserException pe = new ParserException(e, null, ctxt.getModuleName(), ctxt.getPositionMap().ToPos(ind));
                return pe;
            }
        }
        internal bool parse(ParseContext ctxt, ref T result, ref AbstractParsecError err)
        {
            int ind = ctxt.getIndex();
            try
            {
                return apply(ctxt, ref result, ref err);
            }
            catch (Exception e)
            {
                throw wrapException(e, ctxt, ind);
            }
        }
        internal abstract bool apply(ParseContext ctxt, ref T result, ref AbstractParsecError err);
        internal virtual bool apply(ParseContext ctxt, int look_ahead, ref T result, ref AbstractParsecError err)
        {
            return apply(ctxt, ref result, ref err);
        }

        internal bool parse(ParseContext ctxt, int look_ahead, ref T result, ref AbstractParsecError err)
        {
            int ind = ctxt.getIndex();
            try
            {
                return apply(ctxt, look_ahead, ref result, ref err);
            }
            catch (Exception e)
            {
                throw wrapException(e, ctxt, ind);
            }
        }
        /// <summary>
        /// If this parser is a composite of some alternative parsers, return them.
        /// Otherwise, return null.
        /// This is to optimize the overloaded '|' operator when used in chain.
        /// </summary>
        internal virtual Parser<T>[] Alternatives
        {
            get { return null; }
        }
        private string name;
        internal Parser(string n)
        {
            this.name = n;
        }
        internal Parser()
        {
            this.name = this.GetType().Name;
        }
        public override string ToString()
        {
            return name;
        }
        /// <summary> if this parser succeeds, the returned value gets passed on to tp.
        /// The monadic bind (product) operation.
        /// </summary>
        /// <param name="binder">the next step.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<To> Bind<To>(Binder<T, To> binder)
        {
            return new BoundParser<T, To>(this, binder);
        }
        /// <summary> if this parser succeeds,
        /// the returned value is discarded and the next parser is excuted.
        /// The monadic seq (>>) operation. 
        /// </summary>
        /// <param name="p">the next parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<R> Seq<R>(Parser<R> p)
        {
            return Parsers.Seq(this, p);
        }

        /// <summary> Run Parser 'this' for n times. 
        /// The return values are discarded.
        /// </summary>
        /// <param name="n">the number of times to run.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> Repeat_(int n)
        {
            return new RepeatParser_<T>(this, n);
        }
        /// <summary> Run Parser 'this' for n times, collect the return values in an array
        /// whose element type is etype.
        /// </summary>
        /// <param name="n">the number of times to run.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> Repeat(int n)
        {
            return Repeat(n, Functors.getSimpleArrayFactory<T>());
        }

        /// <summary> Run Parser 'this' for n times, collect the return values in an array
        /// created by the ArrayFactory object.
        /// </summary>
        /// <param name="n">the number of times to run.
        /// </param>
        /// <param name="factory">the ArrayFactory object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> Repeat(int n, ArrayFactory<T> factory)
        {
            return new RepeatParser<T>(this, factory, n);
        }


        /// <summary> p.Many(factory) is equivalent to p* in EBNF.
        /// The return values are collected and returned in an array
        /// created by the ArrayFactory object.
        /// </summary>
        /// <param name="factory">the ArrayFactory.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T[]> Many(ArrayFactory<T> factory)
        {
            return Many(Functors.ToArrayAccumulatable(factory));
        }

        /// <summary> p.Many(elem_type) is equivalent to p* in EBNF.
        /// The return values are collected and returned in an array.
        /// </summary>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T[]> Many()
        {
            return Many(Functors.getSimpleArrayFactory<T>());
        }

        /// <summary> p.Many() is equivalent to p* in EBNF.
        /// The return values are discarded.
        /// </summary>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T> Many_()
        {
            return new ManyParser_<T>(this);
        }




        /// <summary> Runs this parser greedily for at least min times.
        /// The return values are collected and returned in an array created by the ArrayFactory object.
        /// </summary>
        /// <param name="min">the minimal number of times to run this parser.
        /// </param>
        /// <param name="factory">the ArrayFactory.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T[]> Many(int min, ArrayFactory<T> factory)
        {
            return Many(min, Functors.ToArrayAccumulatable(factory));
        }

        /// <summary> Runs this parser greedily for at least min times.
        /// The return values are collected and returned in an array.
        /// </summary>
        /// <param name="min">the minimal number of times to run this parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T[]> Many(int min)
        {
            return Many(min, Functors.getSimpleArrayFactory<T>());
        }
        /// <summary> Greedily runs this Parser object repeatedly for at least min times 
        /// and collect the result with the Accumulator object created by Accumulatable.
        /// Fails if parser fails and consumes some input or if parser throws a pseudo-exception.
        /// </summary>
        /// <param name="min">the minimum times to repeat.
        /// </param>
        /// <param name="accm">the Accumulatable object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<To> Many<To>(int min, Generator<Accumulator<T, To>> accm)
        {
            if (min < 0)
                throw new System.ArgumentException("min<0");
            return new ManyMinParser<T, To>(accm, min, this);
        }
        /// <summary> Greedily runs this Parser object repeatedly for 0 or more times.
        /// and collect the result with the Accumulator object created by Accumulatable.
        /// Fails if parser fails and consumes some input or if parser throws a pseudo-exception.
        /// </summary>
        /// <param name="accm">the Accumulatable object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<To> Many<To>(Generator<Accumulator<T, To>> accm)
        {
            return new ManyParser<T, To>(accm, this);
        }
        /// <summary> Greedily runs this Parser object repeatedly for at least min times and at most max times, 
        /// collect the result with the Accumulator object created by Accumulatable.
        /// Fails if parser fails and consumes some input or if parser throws a pseudo-exception.
        /// </summary>
        /// <param name="min">the minimum times to repeat.
        /// </param>
        /// <param name="max">the maximum times to repeat.
        /// </param>
        /// <param name="accm">the Accumulatable object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<To> Some<To>(int min, int max, Generator<Accumulator<T, To>> accm)
        {
            if (min < 0 || max < 0 || min > max)
                throw new System.ArgumentException($"invalid repetition bounds: ({min},{max})");
            if (max == 0)
                return Parsers.One<To>();
            return new SomeMinParser<T, To>(accm, min, max, this);
        }
        /// <summary> Greedily runs this Parser object repeatedly for at most max times, 
        /// collect the result with the Accumulator object created by Accumulatable.
        /// Fails if parser fails and consumes some input or if parser throws a pseudo-exception.
        /// </summary>
        /// <param name="accm">the Accumulatable object.
        /// </param>
        /// <param name="max">the maximum times to repeat.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<To> Some<To>(int max, Generator<Accumulator<T, To>> accm)
        {
            if (max < 0)
                throw new System.ArgumentException("max<0");
            if (max == 0)
                return Parsers.One<To>();
            return new SomeParser<T, To>(accm, max, this);
        }
        /// <summary> Runs this parser greedily for at least min times.
        /// The return values are discarded.
        /// </summary>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T> Many_(int min)
        {
            return new ManyMinParser_<T>(min, this);
        }






        /// <summary> Runs this for at least min times and at most max times.
        /// The return values are collected and returned in an array created by the ArrayFactory object.
        /// </summary>
        /// <param name="min">the minimal number of times to run this parser.
        /// </param>
        /// <param name="max">the maximal number of times to run this parser.
        /// </param>
        /// <param name="factory">the ArrayFactory.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T[]> Some(int min, int max, ArrayFactory<T> factory)
        {
            return Some(min, max, Functors.ToArrayAccumulatable(factory));
        }

        /// <summary> Runs this for at least min times and at most max times.
        /// The return values are collected and returned in an array
        /// whose element type is elem_type.
        /// </summary>
        /// <param name="min">the minimal number of times to run this parser.
        /// </param>
        /// <param name="max">the maximal number of times to run this parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T[]> Some(int min, int max)
        {
            return Some(min, max, Functors.getSimpleArrayFactory<T>());
        }

        /// <summary> Runs this for at least min times and at most max times.
        /// The return values are discarded.
        /// </summary>
        /// <param name="min">the minimal number of times to run this parser.
        /// </param>
        /// <param name="max">the maximal number of times to run this parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T> Some_(int min, int max)
        {
            return new SomeMinParser_<T>(min, max, this);
        }


        /// <summary> Runs this for up to max times.
        /// The return values are discarded.
        /// </summary>
        /// <param name="max">the maximal number of times to run.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T> Some_(int max)
        {
            return new SomeParser_<T>(max, this);
        }
        /// <summary> Runs this for up to max times.
        /// The return values are collected and returned in an array
        /// whose element type is etype.
        /// </summary>
        /// <param name="max">the maximal number times to run.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T[]> Some(int max)
        {
            return Some(max, Functors.getSimpleArrayFactory<T>());
        }
        /// <summary> Runs this for up to max times.
        /// The return values are collected and returned in an array
        /// created by factory.
        /// </summary>
        /// <param name="max">the maximal number of times to run.
        /// </param>
        /// <param name="factory">the array factory.</param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T[]> Some(int max, ArrayFactory<T> factory)
        {
            return Some(max, Functors.ToArrayAccumulatable(factory));
        }

        /// <summary> run a series of this Parser pattern that is seperated and optionally ended by Parser sep pattern.
        /// <p /> This Parser object may succeed 0 or more times. <br />
        /// For example: patterns "token; token; token; token" and "token;" are both token.SepEndBy_(semicolon). <br /> 
        /// The return values are discarded.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> SepEndBy_<A>(Parser<A> sep)
        {
            return SepEndBy1_(sep) | Parsers.One<T>();
        }
        /// <summary> run a series of this Parser pattern that is seperated and optionally ended by Parser sep pattern.
        /// <p /> This Parser object may succeed 0 or more times. <br />
        /// For example: patterns "token; token; token; token" and "token;" are both token.SepEndBy(semicolon). <br /> 
        /// The return values are collected in an array created by the ArrayFactory object.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <param name="factory">the ArrayFacory object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> SepEndBy<A>(Parser<A> sep, ArrayFactory<T> factory)
        {
            return SepEndBy1(sep, factory).Option(factory(0));
        }
        /// <summary> run a series of this Parser pattern that is seperated and optionally ended by Parser sep pattern.
        /// <p /> This Parser object may succeed 0 or more times. <br />
        /// For example: patterns "token; token; token; token" and "token;" are both token.SepEndBy(semicolon). <br /> 
        /// The return values are collected in an array.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> SepEndBy<A>(Parser<A> sep)
        {
            return SepEndBy(sep, Functors.getSimpleArrayFactory<T>());
        }

        /// <summary> run a series of this Parser pattern that is seperated and optionally ended by Parser sep pattern.
        /// <p /> This Parser object should succeed at least once. <br />
        /// For example: patterns "token; token; token; token" and "token;" are both token.SepEndBy1_(semicolon). <br /> 
        /// The return values are discarded.
        /// </summary>
        /// <param name="sep">the seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> SepEndBy1_<A>(Parser<A> sep)
        {
            //return this.Seq (sep.step (0).Seq (this).Many_ ());
            object end = new object();
            Parser<T> one = Parsers.One<T>();
            Catch<T> catcher = delegate(T val, object e)
            {
                if (end == e)
                {
                    return one;
                }
                else
                {
                    return Parsers.Raise<T>(e);
                }
            };
            Parser<T> exceptionable = sep.Seq(this | Parsers.Raise<T>(end));
            return this.Seq(exceptionable.Many_().Try(catcher));
        }
        /// <summary> run a series of this Parser pattern that is seperated and optionally ended by Parser sep pattern.
        /// <p /> This Parser object should succeed at least once. <br />
        /// For example: patterns "token; token; token; token" and "token;" are both token.SepEndBy1(semicolon). <br /> 
        /// The return values are collected in an array created by the ArrayFactory object.
        /// </summary>
        /// <param name="factory">the ArrayFacory object.
        /// </param>
        /// <param name="sep">the seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> SepEndBy1<A>(Parser<A> sep, ArrayFactory<T> factory)
        {
            /*
            Binder<T, T[]> array_builder = delegate (T initial_value) {
              return sep.step(0).Seq(this).Many(Functors.getArrayAccumulatable (initial_value, factory));
            };
            return bind (array_builder);*/
            object end = new object();
            Parser<T[]> one = Parsers.One<T[]>();
            Catch<T[]> catcher = delegate(T[] val, object e)
            {
                if (end == e)
                {
                    return one;
                }
                else
                {
                    return Parsers.Raise<T[]>(e);
                }
            };
            Parser<T> exceptionable = sep.Seq(this | Parsers.Raise<T>(end));
            //return this.Seq (exceptionable.Many(factory).Try(catcher));
            Binder<T, T[]> array_builder = delegate(T initial_value)
            {
                return exceptionable.Many(Functors.ToArrayAccumulatable(initial_value, factory)).Try(catcher);
            };
            return Bind(array_builder);
        }

        /// <summary> run a series of this Parser pattern that is seperated and optionally ended by Parser sep pattern.
        /// <p /> This Parser object should succeed at least once. <br />
        /// For example: patterns "token; token; token; token" and "token;" are both token.SepEndBy1(semicolon). <br /> 
        /// The return values are collected in an Object[] array.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> SepEndBy1<A>(Parser<A> sep)
        {
            return SepEndBy1(sep, Functors.getSimpleArrayFactory<T>());
        }
        /// <summary> p.Optional() is equivalent to p? in EBNF. </summary>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T> Optional()
        {
            return this | Parsers.One<T>();
        }

        /// <summary> If this fails with no input consumed, the default value is returned.
        /// p.Option(name, def) = p | retn(def).
        /// </summary>
        /// <param name="def">the default value.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T> Option(T def)
        {
            return this | Parsers.Return(def);
        }

        /// <summary> fails if 'this' succeeds. Input consumption is undone.</summary>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<D_> Not()
        {
            return Not(Name);
        }
        /// <summary> fails if 'this' succeeds. Input consumption is undone.</summary>
        /// <param name="err">the error message if fails.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<D_> Not(string err)
        {
            return Peek().Ifelse(Parsers.Unexpected<D_>(err), Parsers.One<D_>());
        }

        /// <summary> this is a look-ahead operation.
        /// Succeed or not, the input consumption is undone.
        /// </summary>
        /// <returns> the new Parser.
        /// </returns>
        //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        public Parser<T> Peek()
        {
            return new PeekParser<T>(this);
        }

        /// <summary> if this succeeds, the returned value is transformed with m to a new return value.</summary>
        /// <param name="m">the map object to transform return value.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<R> Map<R>(Map<T, R> m)
        {
            return Parsers.Map(this, m);
        }

        /// <summary> it sequentially run this and p, and then transforms the two return values with m to a new return value.</summary>
        /// <param name="p">the next parser to run.
        /// </param>
        /// <param name="m">the transformation.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<R> And<T2, R>(Parser<T2> p, Map<T, T2, R> m)
        {
            return Parsers.Map(this, p, m);
        }

        /// <summary> 'this' and 'sep' are executed sequentially.
        /// The return value of 'this' is returned.
        /// </summary>
        /// <param name="sep">the following parser.
        /// </param>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T> FollowedBy<X>(Parser<X> sep)
        {
            return new FollowedByParser<T, X>(this, sep);
        }


        /// <summary> Make sure 'this' is atomic. When fails, no input is consumed.
        /// For lookahead, a successful atomized operation is considered 
        /// at most one logical step.
        /// </summary>
        /// <returns> the new Parser.
        /// </returns>
        public Parser<T> Atomize()
        {
            return new AtomParser<T>(this);
        }
        /// <summary> run yes if this succeeds, no if this fails without consuming input;
        /// fails otherwise.
        /// </summary>
        /// <param name="yes">the true branch.
        /// </param>
        /// <param name="no">the false branch.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<R> Ifelse<R>(Parser<R> yes, Parser<R> no)
        {
            return Ifelse(Parsers.BindTo<T, R>(yes), no);
        }


        /// <summary> run yes if this succeeds, no if this fails without consuming input;
        /// fails otherwise.
        /// </summary>
        /// <param name="yes">the true branch.
        /// </param>
        /// <param name="no">the false branch.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<R> Ifelse<R>(Binder<T, R> yes, Parser<R> no)
        {
            return new IfElseParser<T, R>(this, yes, no);
        }


        /// <summary> By default, ifelse, plus, sum will not try to run the next branch if the previous branch failed
        /// and consumed some input.
        /// this is because the default look-ahead token is 1.
        /// <br /> by using lookahead, this default behavior can be altered.
        /// Parsers.Plus(p1, p2).Lookahead(3)
        /// will still try p2 even if p1 fails and consumes one or two inputs.
        /// <p /> lookahead only affects one nesting level.
        /// Parsers.Plus(p1,p2).ifelse(yes,no).Lookahead(3)
        /// will not affect the Parsers.Plus(p1,p2) nested within ifelse.
        /// <p /> lookahead directly on top of lookahead will override the previous lookahead.
        /// Parsers.Plus(p1,p2).Lookahead(3).Lookahead(1)
        /// is equivalent as Parsers.Plus(p1, p2).Lookahead(1).
        /// <br />
        /// lookahead looks at logical step.
        /// by default, each terminal is one logical step.
        /// atomize() combinator ensures at most 1 logical step for a parser.
        /// Use step() combinator to fine control logical steps.
        /// </summary>
        /// <param name="toknum">the number of tokens to look ahead.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> Lookahead(int toknum)
        {
            return new LookaheadParser<T>(this, toknum);
        }
        /// <summary> if fails and did not consume input, 
        /// reports an expecting error with the given label.
        /// </summary>
        /// <param name="lbl">the label text.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> Label(string lbl)
        {
            return this | Parsers.Expect<T>(lbl);
        }
        /// <summary> Runs this Parser that is between a pair of other parsers.
        /// First run Parser open, then run this Parser object, finally run Parser close. 
        /// The return value of this parser object is preserved as the return value. <br />
        /// <code>
        /// do {open; x&lt;-p; close; return p}
        /// Parser _ -&gt; Parser a -&gt; Parser _ -&gt; Parser a
        /// </code>
        /// </summary>
        /// <param name="open">the opening parser.
        /// </param>
        /// <param name="close">the closing parser.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> Between<A, B>(Parser<A> open, Parser<B> close)
        {
            return open.Seq(this.FollowedBy(close));
        }

        /// <summary> run a series of this Parser object pattern that is seperated by Parser sep pattern.
        /// <p /> this Parser has to succeed at least once. <br />
        /// For example: pattern "token, token, token, token" is token.SepBy1(comma). <br /> 
        /// The return values are discarded.
        /// <p /> Parser a -> Parser _
        /// </summary>
        /// <param name="sep">the seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> SepBy1_<A>(Parser<A> sep)
        {
            return this.Seq(sep.Seq(this).Many_());
        }
        /// <summary> run a series of this Parser pattern that is seperated by Parser sep pattern.
        /// <p /> this Parser object has to succeed at least once. <br />
        /// For example: pattern "token, token, token, token" is token.SepBy1_(comma). <br /> 
        /// The return values are collected in an array created by the ArrayFactory object af.
        /// </summary>
        /// <param name="sep">the seperator.
        /// </param>
        /// <param name="factory">the ArrayFacory object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> SepBy1<A>(Parser<A> sep, ArrayFactory<T> factory)
        {
            Parser<T> recuring = sep.Seq(this);
            Binder<T, T[]> array_builder = delegate(T v)
            {
                return recuring.Many(Functors.ToArrayAccumulatable(v, factory));
            };
            return this.Bind(array_builder);
        }
        /// <summary> <p /> Parser a -> Parser [a]. <br />
        /// run a series of Parser p pattern that is seperated by Parser sep pattern.
        /// <p /> Parser p has to succeed at least once. <br />
        /// For example: pattern "token, token, token, token" is token.SepBy1(comma). <br /> 
        /// The return values are collected in an array.
        /// </summary>
        /// <param name="sep">the seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> SepBy1<A>(Parser<A> sep)
        {
            return SepBy1(sep, Functors.getSimpleArrayFactory<T>());
        }

        /// <summary> run a series of this Parser pattern that is seperated by Parser sep pattern.
        /// <p /> This Parser object can succeed 0 or more times. <br />
        /// For example: pattern "token, token, token, token" is token.SepBy_(comma). <br /> 
        /// The return values are discarded.
        /// <p /> Parser a -> Parser _
        /// </summary>
        /// <param name="sep">the seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> SepBy_<A>(Parser<A> sep)
        {
            return SepBy1_(sep) | Parsers.One<T>();
        }
        /// <summary> run a series of this Parser pattern that is seperated by Parser sep pattern.
        /// <p /> This Parser object can succeed 0 or more times. <br />
        /// For example: pattern "token, token, token, token" is token.SepBy(comma). <br /> 
        /// The return values are collected in an array whose element type is etype.
        /// <p /> Class -> Parser a -> Parser [Object]
        /// </summary>
        /// <param name="sep">the seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> SepBy<A>(Parser<A> sep)
        {
            return SepBy(sep, Functors.getSimpleArrayFactory<T>());
        }
        /// <summary> run a series of this Parser pattern that is seperated by Parser sep pattern.
        /// <p /> This Parser object can succeed 0 or more times. <br />
        /// For example: pattern "token, token, token, token" is token.SepBy(comma). <br /> 
        /// The return values are collected in an array created by the ArrayFactory object af.
        /// <p /> ArrayFactory a -> Parser a -> Parser [a]
        /// </summary>
        /// <param name="factory">the ArrayFacory object.
        /// </param>
        /// <param name="sep">the seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> SepBy<A>(Parser<A> sep, ArrayFactory<T> factory)
        {
            return SepBy1(sep, factory).Option(factory(0));
        }

        /// <summary> run a series of this Parser pattern ended by Parser sep pattern.
        /// <p /> This Parser object can succeed 0 or more times. <br />
        /// For example: pattern "token; token; token; token;" is token.endBy_(comma). <br /> 
        /// The return values are discarded.
        /// <p /> Class -> Parser a -> Parser _
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> EndBy_<A>(Parser<A> sep)
        {
            return this.FollowedBy(sep).Many_();
        }

        /// <summary> run a series of this Parser pattern ended by Parser sep pattern.
        /// <p /> This Parser object can succeed 0 or more times. <br />
        /// For example: pattern "token; token; token; token;" is endBy(comma, token). <br /> 
        /// The return values are collected in an array whose element type is etype.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> EndBy<A>(Parser<A> sep)
        {
            return EndBy(sep, Functors.getSimpleArrayFactory<T>());
        }
        /// <summary> run a series of Parser p pattern ended by Parser sep pattern.
        /// <p /> Parser p can succeed 0 or more times. <br />
        /// For example: pattern "token; token; token; token;" is endBy(comma, token). <br /> 
        /// The return values are discarded.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <param name="factory">the ArrayFacory object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> EndBy<A>(Parser<A> sep, ArrayFactory<T> factory)
        {
            return this.FollowedBy(sep).Many(factory);
        }
        /// <summary> run a series of this Parser pattern ended by Parser sep pattern.
        /// <p /> This Parser object should succeed for at least once. <br />
        /// For example: pattern "token; token; token; token;" is token.endBy1_(comma). <br /> 
        /// The return values are discarded.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> EndBy1_<A>(Parser<A> sep)
        {
            return this.FollowedBy(sep).Many_(1);
        }
        /// <summary> run a series of this Parser pattern ended by Parser sep pattern.
        /// <p /> This Parser object should succeed for at least once. <br />
        /// For example: pattern "token; token; token; token;" is token.endBy(comma). <br /> 
        /// The return values are collected in an array whose element type is etype.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> EndBy1<A>(Parser<A> sep)
        {
            return EndBy1(sep, Functors.getSimpleArrayFactory<T>());
        }
        /// <summary> run a series of this Parser pattern ended by Parser sep pattern.
        /// <p /> This Parser object should succeed for at least once. <br />
        /// For example: pattern "token; token; token; token;" is token.endBy1(comma). <br /> 
        /// The return values are collected in an array created by the ArrayFactory object af.
        /// </summary>
        /// <param name="sep">the end seperator.
        /// </param>
        /// <param name="factory">the ArrayFacory object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T[]> EndBy1<A>(Parser<A> sep, ArrayFactory<T> factory)
        {
            return this.FollowedBy(sep).Many(1, factory);
        }

        /// <summary> The created Parser object will first run parser p,
        /// if the return value of parser p does not satisify the given predicate,
        /// An alternative Parser object is returned.
        /// </summary>
        /// <param name="pred">the predicate object.
        /// </param>
        /// <param name="otherwise">the alternative Parser object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> IsReturn(Predicate<T> pred, Parser<T> otherwise)
        {
            Binder<T, T> binder = delegate(T v)
            {
                return pred(v) ? Parsers.Return(v) : otherwise;
            };
            return Bind(binder);
        }
        /// <summary> The created Parser object will first run parser p,
        /// if the return value of parser p does not satisify the given predicate,
        /// it fails and the input consumption of parser p is undone.
        /// It is an atomic parser.
        /// </summary>
        /// <param name="pred">the predicate object.
        /// </param>
        /// <param name="expecting">the "expected" error message. 
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> IsReturn(Predicate<T> pred, string expecting)
        {
            return IsReturn(pred, Parsers.Expect<T>(expecting)).Atomize();
        }

        /// <summary> Fails if the return value of this parser does not satisify the given predicate.
        /// No-op otherwise.
        /// </summary>
        /// <param name="pred">the predicate object.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> IsReturn(Predicate<T> pred)
        {
            return IsReturn(pred, Parsers.Zero<T>()).Atomize();
        }
        /// <summary> First run this Parser object, if it succeeds with input consumed, isConsumed() succeeds;
        /// if it fails or did not consume input, isConsumed() fails.
        /// <p /> Parser a -> Parser a
        /// </summary>
        /// <param name="errmsg">the error message when parser succeeds with no input consumed.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> IsConsumed(string errmsg)
        {
            return new IsConsumedParser<T>(this, errmsg);
        }
        /// <summary> First run this Parser object, if it succeeds with input consumed, isConsumed() succeeds;
        /// if it fails or did not consume input, isConsumed() fails.
        /// <p /> Parser a -> Parser a
        /// </summary>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> IsConsumed()
        {
            return IsConsumed("input not consumed");
        }
        /// <summary> First run this Parser object, if it succeeds with no input consumed, notConsumed() succeeds;
        /// if it fails, notConsumed() fails;
        /// if it succeeds and consumes input, the input consumption is undone and notConsumed() fails.
        /// <p /> Parser a -> Parser a
        /// </summary>
        /// <param name="err">the error message when parser succeeds and consumes some input.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser NotConsumed(string err)
        {
            return new NotConsumedParser<T>(this, err);
        }
        /// <summary> First run this Parser object, if it succeeds with no input consumed, notConsumed() succeeds;
        /// if it fails, notConsumed() fails;
        /// if it succeeds and consumes input, the input consumption is undone and notConsumed() fails.
        /// <p /> Parser a -> Parser a
        /// </summary>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser NotConsumed()
        {
            return NotConsumed("input consumed");
        }
        /// <summary> lookahead looks at logical steps.
        /// step(int) runs this parser and sets the number of logical steps.
        /// 
        /// </summary>
        /// <param name="n">the number logical steps. n>=0 has to be true.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> Step(int n)
        {
            if (n < 0)
                throw new System.ArgumentException("" + n + "<0");
            return new SteppedParser<T>(this, n);
        }
        /// <summary> lookahead looks at logical steps.
        /// step() runs this parser and sets 1 logical step.
        /// 
        /// </summary>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> Step()
        {
            return Step(1);
        }

        /// <summary> if this Parser throws a psuedo exception by calling raise(), it is handled by Catch hdl.
        /// <p /> Parser a -> Parser a
        /// </summary>
        /// <param name="hdl">the exception handler.
        /// </param>
        /// <returns> the new Parser object.
        /// </returns>
        public Parser<T> Try(Catch<T> hdl)
        {
            return new TryParser<T>(this, hdl);
        }
        /// <summary> To convert the current Parser to a Parser object that returns any target type.</summary>
        /// <returns> the Parser object that returns the expected type.
        /// R is the target return type.
        /// </returns>
        public Parser<R> Cast<R>() where R : class
        {
            return Map<R>(delegate(T v) { return v as R; });
        }
    }
}
