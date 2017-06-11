using System;
using System.Collections.Generic;
using System.Text;

namespace Codehaus.Parsec
{
    using ShowToken = Map<object, string>;
    /*
    struct State
    {
      internal readonly int at;
      internal readonly int step;
      internal readonly object ustate;
      internal State (int at, int step, object ustate) {
        this.at = at;
        this.step = step;
        this.ustate = ustate;
      }
    }*/
    abstract class ParseContext
    {
        internal string getModuleName() { return module; }
        internal abstract bool isEof();
        internal abstract int getIndex();
        internal abstract Tok getToken();
        internal abstract char peekChar();
        internal abstract string getSource();
        internal abstract ParsecError getSysUnexpected();
        protected int at;
        private int step;
        //user state doesn't seem to be useful. skip it for now.
        //private object userState;
        private readonly string module;
        private readonly PositionMap pmap;
        /*
        internal State getState () {
          return new State (at, step, userState);
        }
        internal void setState (State state) {
          this.at = state.at;
          this.step = state.step;
          this.userState = state.ustate;
        }*/
        internal void setState(int step, int at/*, object ustate*/)
        {
            this.step = step;
            this.at = at;
            //this.userState = ustate;
        }
        internal PositionMap getPositionMap()
        {
            return pmap;
        }
        //internal object getUserState () { return userState; }
        internal int getStep() { return step; }
        internal int getAt() { return at; }
        internal void setAt(int at)
        {
            this.at = at;
        }
        /*
        internal void setAt (State state) {
          this.step = state.step;
          this.at = state.at;
        }
        internal void setAt (int step, int at) {
          this.step = step;
          this.at = at;
        }*/
        internal void setStep(int s)
        {
            this.step = s;
        }
        internal void next()
        {
            at++;
            step++;
        }
        internal void next(int n)
        {
            at += n;
            if (n > 0) step++;
        }
        /*
        internal void setUserState (object obj) {
          userState = obj;
        }*/
        //caller should not change input after it is passed in.
        internal ParseContext(int at, string module, PositionMap pmap)
        {
            //this.userState = us;
            this.step = 0;
            this.at = at;
            this.module = module;
            this.pmap = pmap;
        }
        /*
        internal void prependError (AbstractParsecError err) {
          this.err = AbstractParsecError.mergeError (err, this.err);
        }
        internal void appendError (AbstractParsecError err) {
          this.err = AbstractParsecError.mergeError (this.err, err);
        }
        */
    }

    sealed class ScannerState : ParseContext
    {
        private readonly string src;
        private readonly int len;
        internal ScannerState(string src,
            int a, string module, PositionMap pmap)
            : base(a, module, pmap)
        {
            this.src = src;
            this.len = src.Length;
        }
        internal ScannerState(string src, int a,
            string module, PositionMap pmap,
            int l)
            : base(a, module, pmap)
        {
            this.src = src;
            this.len = l;
        }

        internal override char peekChar()
        {
            return src[at];
        }
        internal override bool isEof()
        {
            return len == at;
        }
        internal override string getSource()
        {
            return src;
        }
        internal int length() { return len; }

        internal override int getIndex()
        {
            return at;
        }

        internal override Tok getToken()
        {
            throw new System.NotSupportedException("Parser not on token level");
        }

        internal override ParsecError getSysUnexpected()
        {
            string msg = (len == at) ? "EOF" : ("" + src[at]);
            return ParsecError.raiseSysUnexpected(getIndex(), msg);
        }
    }

    sealed class ParserState : ParseContext
    {

        private readonly Tok[] input;
        private readonly ParsecError[] sys_unexpected;
        //in case a terminating eof token is not explicitly created,
        //the implicit one is used.
        private readonly int end_index;
        private readonly ParsecError eof_unexpected;


        private readonly ShowToken show;

        internal override bool isEof()
        {
            return at >= input.Length; //|| input[at].getToken()==Tokens.eof();
        }
        internal int length() { return input.Length; }
        internal override int getIndex()
        {
            if (at == input.Length) return end_index;
            return input[at].Index;
        }


        internal override Tok getToken()
        {
            return input[at];
        }
        //caller should not change input after it is passed in.
        internal ParserState(Tok[] input,
            int at, string module, PositionMap pmap,
            int end_index,
            string eof_str, ShowToken show)
            : base(at, module, pmap)
        {
            this.input = input;
            this.sys_unexpected = new ParsecError[input.Length];
            this.show = show;
            this.end_index = end_index;
            this.eof_unexpected = ParsecError.raiseSysUnexpected(
                end_index, eof_str);
        }

        internal override ParsecError getSysUnexpected()
        {
            return getSysUnexpected(at);
        }
        private ParsecError getSysUnexpected(int i)
        {
            if (i >= sys_unexpected.Length) return eof_unexpected;
            ParsecError r = sys_unexpected[i];
            if (r == null)
            {
                Tok ptok = input[i];
                r = ParsecError.raiseSysUnexpected(ptok.Index,
                    show(ptok.Token));
                sys_unexpected[i] = r;
            }
            return r;
        }
        internal override char peekChar()
        {
            throw new NotSupportedException("parser not on char level.");
        }
        internal char peekChar(int i)
        {
            throw new NotSupportedException("parser not on char level.");
        }
        internal override string getSource()
        {
            throw new NotSupportedException("parser not on char level.");
        }
    }
}
