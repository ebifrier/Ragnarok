using System;
namespace Codehaus.Parsec
{

    /// <summary> ParserException is thrown when a grammar error happens.
    /// <p />
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// 2004-11-12
    /// </author>
    [Serializable]
    public class ParserException : System.SystemException
    {
        /// <summary> Get the parsing trace.</summary>
        /// <returns> the parsing trace
        /// with objects of {@link ParsingFrame} as the elements.
        /// </returns>
        public System.Collections.ArrayList getParsingTrace()
        {
            return frames;
        }
        /// <summary> Get the ParseError object.</summary>
        /// <returns> Returns the err.
        /// </returns>
        public ParseError getError()
        {
            return err;
        }
        /// <summary> Get the default formatted error message.</summary>
        /// <remarks>
        /// see java.lang.Throwable.getMessage.
        /// </remarks>
        public override string Message
        {
            get
            {
                return getErrorMessage();
            }
        }
        private string getErrorMessage()
        {
            string msg = base.Message;
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            if (msg != null)
                buf.Append(msg).Append("\n");
            if (module != null)
                buf.Append(module).Append(" - ");
            buf.Append(DefaultShowError.Show(err, pos));
            return buf.ToString();
        }
        /// <summary> Gets the module name.</summary>
        /// <returns> the module name.
        /// </returns>
        virtual public string ModuleName
        {
            get
            {
                return module;
            }

        }
        /// <summary> Gets the line number of the error.</summary>
        /// <returns> the line number.
        /// </returns>
        virtual public int Line
        {
            get
            {
                return pos.Line;
            }

        }
        /// <summary> Gets the column number of the error.</summary>
        /// <returns> the column number.
        /// </returns>
        virtual public int Column
        {
            get
            {
                return pos.Column;
            }

        }
        private readonly ParseError err;
        private readonly Pos pos;
        private readonly string module;
        private System.Collections.ArrayList frames = new System.Collections.ArrayList();
        internal virtual void pushFrame(ParsingFrame frame)
        {
            frames.Add(frame);
        }

        /// <summary> Print the resultion trace.</summary>
        /// <param name="writer">the output writer.
        /// </param>
        public void WriteParsingTrace(System.IO.TextWriter writer)
        {
            int size = frames.Count;
            for (int i = 0; i < size; i++)
            {
                writer.WriteLine(frames[i]);
            }
            writer.Flush();
        }
        /// <summary> Prints the parsing trace to the standard error output.</summary>
        public void WriteParsingTrace()
        {
            System.IO.StreamWriter temp_writer;
            temp_writer = new System.IO.StreamWriter(System.Console.OpenStandardError(), System.Console.Error.Encoding);
            temp_writer.AutoFlush = true;
            WriteParsingTrace(temp_writer);
        }
        public override string StackTrace
        {
            get
            {
                System.IO.StringWriter swriter = new System.IO.StringWriter();
                WriteParsingTrace(swriter);
                swriter.WriteLine();
                return swriter.ToString() + base.StackTrace;
            }
        }


        /// <summary> Create a ParserException object.</summary>
        /// <param name="err">the ParseError object.
        /// </param>
        /// <param name="mname">the module name.
        /// </param>
        /// <param name="pos">the position.
        /// </param>
        public ParserException(ParseError err, string mname, Pos pos)
        {
            this.err = err;
            this.pos = pos;
            this.module = mname;
        }

        /// <summary> Create a ParserException object.</summary>
        /// <param name="message">the error message.
        /// </param>
        /// <param name="err">the ParseError object.
        /// </param>
        /// <param name="mname">the module name.
        /// </param>
        /// <param name="pos">the position.
        /// </param>
        public ParserException(string message, ParseError err, string mname, Pos pos)
            : base(message)
        {
            this.err = err;
            this.pos = pos;
            this.module = mname;
        }


        /// <param name="cause">the exception that causes this.
        /// </param>
        /// <param name="err">the ParseError object.
        /// </param>
        /// <param name="mname">the module name.
        /// </param>
        /// <param name="pos">the position.
        /// </param>
        //UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
        public ParserException(System.Exception cause, ParseError err, string mname, Pos pos)
            : base(mname, cause)
        {
            this.err = err;
            this.pos = pos;
            this.module = mname;
        }

        /// <param name="message">the error message.
        /// </param>
        /// <param name="cause">the exception that causes this.
        /// </param>
        /// <param name="err">the ParseError object.
        /// </param>
        /// <param name="mname">the module name.
        /// </param>
        /// <param name="pos">the position.
        /// </param>
        //UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to 'System.Exception' which has different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
        public ParserException(string message, System.Exception cause, ParseError err, string mname, Pos pos)
            : base(message, cause)
        {
            this.err = err;
            this.pos = pos;
            this.module = mname;
        }
    }
}
