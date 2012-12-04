using System;
using System.Collections.Generic;
using System.Text;

namespace Codehaus.Parsec
{
    /// <summary> Describes Parse error.</summary>
    /// <author>  Ben Yu
    /// 
    /// Nov 18, 2004
    /// </author>
    public interface ParseError
    {
        /// <summary> Gets the index number in the original source.</summary>
        /// <returns> the index number.
        /// </returns>
        int Index
        {
            get;
        }
        /// <summary> Get the "... encountered" error.</summary>
        /// <returns> the actually encountered token when error happens. 
        /// </returns>
        string getEncountered();
        /// <summary> Get the "unexpected ..." error.</summary>
        /// <returns> all the unexpected.
        /// </returns>
        string[] getUnexpected();
        /// <summary> Get the user error messages.</summary>
        /// <returns> all the user-provided message.
        /// </returns>
        string[] getMessages();
        /// <summary> Get the "expecting ..." errors.</summary>
        /// <returns> all the expectings.
        /// </returns>
        string[] getExpecting();
    }
    abstract class AbstractParsecError
    {
        internal object Exception
        {
            get { return exception; }
        }
        internal bool Thrown
        {
            get { return exception != null; }
        }
        internal abstract ParsecError render();
        internal abstract AbstractParsecError setExpecting(string s);
        internal int Precedence
        {
            get { return precedence; }
        }
        public int Index
        {
            get { return at; }
        }
        internal bool noMerge() { return nomerge; }
        private readonly bool nomerge;
        private readonly int at;
        private readonly int precedence;
        private readonly object exception;
        internal AbstractParsecError(bool nomerge, int at,
            int pred, object exception)
        {
            this.nomerge = nomerge;
            this.at = at;
            this.precedence = pred;
            this.exception = exception;
        }
        internal static AbstractParsecError mergeError(AbstractParsecError e1,
            AbstractParsecError e2)
        {
            if (e1 == null) return e2;
            if (e2 == null) return e1;
            if (e1 == e2) return e1;
            int pred = e1.precedence;
            int pred2 = e2.precedence;
            int at = e1.at;
            int at2 = e2.at;
            if (at == at2)
            {
                if (pred2 > pred)
                {
                    return e2;
                }
                else if (pred > pred2)
                {
                    return e1;
                }
                //else return e1;
            }
            else if (at > at2)
            {
                /*if(pred < pred2){
                  return e2;
                }
                else */
                return e1;
            }
            else if (at < at2)
            {
                /*if(pred > pred2){
                  return e1;
                }
                else */
                return e2;
            }
            if (e1.nomerge && e2.nomerge)
            {
                return e1;
            }
            return new MergedParsecError(at, pred, e1, e2);
        }
    }

    sealed class ParsecErrorExpecting : AbstractParsecError
    {
        internal override ParsecError render()
        {
            return err.render().setExpecting(s).render();
        }

        internal override AbstractParsecError setExpecting(string s)
        {
            return new ParsecErrorExpecting(noMerge(), Index, Precedence,
                Exception, err, s);
        }
        private readonly AbstractParsecError err;
        private readonly string s;

        internal ParsecErrorExpecting(bool nomerge, int at, int pred, object exception,
            AbstractParsecError err, string s)
            : base(nomerge, at, pred, exception)
        {
            this.err = err;
            this.s = s;
        }
    }
    sealed class MergedParsecError : AbstractParsecError
    {
        internal override ParsecError render()
        {
            return getMerged(err1.render(), err2.render());
        }
        internal override AbstractParsecError setExpecting(string s)
        {
            return ParsecError.raiseExpecting(Index, s, this);
        }
        private ParsecError getMerged(ParsecError e1, ParsecError e2)
        {
            return ParsecError.mergeError(Index, Exception, e1, e2);
        }
        private readonly AbstractParsecError err1;
        private readonly AbstractParsecError err2;
        internal MergedParsecError(int ind, int pred,
            AbstractParsecError err1, AbstractParsecError err2)
            : base(false, ind, pred, mergeObj(err1.Exception, err2.Exception))
        {
            this.err1 = err1;
            this.err2 = err2;
        }
        internal static object mergeObj(object a, object b)
        {
            return a == null ? b : a;
        }
    }
    sealed class ParsecError : AbstractParsecError, ParseError
    {
        //private static readonly string[] err0 = new string[0];
        internal override ParsecError render() { return this; }
        private readonly object sys_unexpected;
        private readonly string[] unexpected;
        private readonly string[] expecting;
        private readonly string[] raw;
        internal static int getPrecedenceForExpecting(string s)
        {
            return s != null ? 2 : 1;
        }

        private ParsecError(bool nm, int at,
            object sys, string[] unexpected, string[] expecting, string[] raw,
            object exception)
            : base(nm, at, (expecting != null || unexpected != null || raw != null) ?
                2 : 1, exception)
        {

            this.sys_unexpected = sys;
            this.unexpected = unexpected;
            this.expecting = expecting;
            this.raw = raw;
        }
        internal override AbstractParsecError setExpecting(string s)
        {
            return new ParsecError(false, Index,
                sys_unexpected, unexpected, new string[] { s }, raw, Exception);
        }
        internal static ParsecError raiseRaw(int at,
            string msg)
        {
            return new ParsecError(false, at, null, null, null, new string[] { msg }, null);
        }
        internal static ParsecError raiseSysUnexpected(int at,
            object obj)
        {
            return new ParsecError(true, at, obj, null, null, null, null);
        }
        internal static ParsecError raiseUnexpected(int at
            , string s)
        {
            return new ParsecError(false, at, null, new string[] { s }, null, null, null);
        }
        internal static ParsecError raiseExpecting(int at,
            string s)
        {
            return new ParsecError(false, at, null, null, new string[] { s }, null, null);
        }
        internal static AbstractParsecError raiseExpecting(int at, string s,
            AbstractParsecError err)
        {
            return new ParsecErrorExpecting(false, at,
                myPrecedence(err.Precedence, s), err.Exception,
                err, s);
        }
        private static int max(int a, int b)
        {
            return a > b ? a : b;
        }
        private static int myPrecedence(int pred, string s)
        {
            return max(pred, ParsecError.getPrecedenceForExpecting(s));
        }
        internal static ParsecError throwException(int at,
            object e)
        {
            return new ParsecError(false, at, null, null, null, null, e);
        }
        internal object getSysUnexpected() { return sys_unexpected; }
        public string getEncountered()
        {
            if (sys_unexpected == null) return null;
            else return sys_unexpected.ToString();
        }
        public string[] getUnexpected() { return unexpected; }
        public string[] getExpecting() { return expecting; }
        public string[] getMessages() { return raw; }
        internal static ParsecError noError()
        {
            return null;
        }
        private static string[] mergeMsgs(string[] a, string[] b)
        {
            if (a == null) return b;
            if (b == null) return a;
            if (a == b) return a;
            string[] msgs = new string[a.Length + b.Length];
            /*for(int i=0; i<a.Length; i++){
              msgs[i] = a[i];
            }
            for(int i=0; i<b.Length; i++){
              msgs[i+a.Length] = b[i];
            }*/
            Array.Copy(a, msgs, a.Length);
            //System.arraycopy (a, 0, msgs, 0, a.Length);
            Array.Copy(b, 0, msgs, a.Length, b.Length);
            //System.arraycopy (b, 0, msgs, a.Length, b.Length);
            return msgs;
        }
        internal static ParsecError mergeError(
            int ind, object exception, ParsecError e1, ParsecError e2)
        {
            return new ParsecError(false, ind,
                MergedParsecError.mergeObj(e1.sys_unexpected, e2.sys_unexpected),
                mergeMsgs(e1.unexpected, e2.unexpected),
                mergeMsgs(e1.expecting, e2.expecting),
                mergeMsgs(e1.raw, e2.raw),
                exception
            );
        }
    }
}
