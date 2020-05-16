using System;

namespace Codehaus.Parsec
{
    /// <summary> This class represents an error frame during parsing.
    /// <p />
    /// </summary>
    /// <author>  Ben Yu
    /// Dec 4, 2005 2:25:47 PM
    /// </author>
    public struct ParsingFrame : IEquatable<ParsingFrame>
    {
        /// <summary> Get the index of the character within the source.</summary>
        public int Index
        {
            get
            {
                return ind;
            }

        }
        /// <summary> Get the module name.</summary>
        public string Module
        {
            get
            {
                return module;
            }

        }
        /// <summary> Get the Parser object executed.</summary>
        public Parser Parser
        {
            get
            {
                return parser;
            }

        }
        /// <summary> Get the position within the source.</summary>
        public Pos Position
        {
            get
            {
                return pos;
            }

        }
        private readonly string module;
        private readonly int ind;
        private readonly Pos pos;
        private readonly Parser parser;
        /// <summary> To create a ParsingFrame object.</summary>
        /// <param name="module">the module name.
        /// </param>
        /// <param name="ind">the index of the character within the source. 
        /// </param>
        /// <param name="pos">the position of the character.
        /// </param>
        /// <param name="parser">the parser executed.
        /// </param>
        public ParsingFrame(string module, int ind, Pos pos, Parser parser)
        {
            this.ind = ind;
            this.module = module;
            this.parser = parser;
            this.pos = pos;
        }
        public override string ToString()
        {
            return module + " - " + pos + ": " + parser.Name;
        }
        public override bool Equals(object obj)
        {
            if (obj is ParsingFrame)
            {
                return Equals((ParsingFrame)obj);
            }
            else
                return false;
        }
        public bool Equals(ParsingFrame other)
        {
            return ind == other.ind && Misc.AreEqual(module, other.module);
        }
        public override int GetHashCode()
        {
            return Misc.Hashcode(module) * 31 + ind;
        }
        public static bool operator==(ParsingFrame x, ParsingFrame y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(ParsingFrame x, ParsingFrame y)
        {
            return !(x == y);
        }
    }
}
