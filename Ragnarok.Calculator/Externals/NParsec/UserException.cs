
using System;

namespace Codehaus.Parsec
{
    /// <summary>
    /// This exception represents an illegal parser state that should not happen.
    /// This exception being thrown means a bug in the parser application.
    /// </summary>
    public class IllegalParserStateException : SystemException
    {
        public IllegalParserStateException() { }
        public IllegalParserStateException(string msg) : base(msg) { }
        public IllegalParserStateException(string msg, Exception innerException)
            : base(msg, innerException) { }
    }


    /// <summary> User code can throw this exception
    /// when a non-recoverable error is encountered.
    /// The framework will transform it to ParserException.
    /// <p /> Zephyr Business Solutions Corp.
    /// 
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// </author>
    public class UserException : Exception
    {
        /// <summary> Get the index in the original source.</summary>
        public int Index
        {
            get
            {
                return ind;
            }
        }
        private readonly int ind;

        public UserException() { }
        public UserException(string msg) : base(msg) { }
        public UserException(string msg, Exception innerException)
            : base(msg, innerException) { }

        /// <summary> Create a UserException object.</summary>
        /// <param name="ind">the index in the original source.
        /// -1 if the index is unknown.
        /// </param>
        public UserException(int ind)
        {
            this.ind = ind;
        }


        /// <summary> Create a UserException object.</summary>
        /// <param name="ind">the index in the original source.
        /// -1 if the index is unknown.
        /// </param>
        /// <param name="msg">the error message.
        /// </param>
        public UserException(int ind, string msg)
            : base(msg)
        {
            this.ind = ind;
        }
        /// <summary> Create a UserException object.</summary>
        /// <param name="ind">the index in the original source.
        /// -1 if the index is unknown.
        /// </param>
        /// <param name="msg">the error message.
        /// </param>
        /// <param name="arg1">the chained exception.
        /// </param>
        public UserException(int ind, string msg, Exception arg1)
            : base(msg, arg1)
        {
            this.ind = ind;
        }
        /// <summary> Create a UserException object.</summary>
        /// <param name="ind">the index in the original source.
        /// -1 if the index is unknown.
        /// </param>
        /// <param name="cause">the chained exception.
        /// </param>
        public UserException(int ind, Exception cause)
            : base("user exception", cause)
        {
            this.ind = ind;
        }
    }
}
