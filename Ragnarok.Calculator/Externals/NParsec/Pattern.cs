namespace Codehaus.Parsec
{
    /*/// <summary>
    /// Represents a character range.
    /// </summary>
    public struct CharRange
    {
      readonly string text;
      readonly int from;
      readonly int end;
      /// <summary>
      /// The underlying text.
      /// </summary>
      public string Text {
        get { return text; }
      }
      /// <summary>
      /// the starting index of the range.
      /// </summary>
      public int Begin {
        get { return from; }
      }
      /// <summary>
      /// the ending index of the range. Exclusive.
      /// </summary>
      public int End {
        get { return end; }
      }
      public int Length {
        get { return end - from; }
      }
      public bool Eof {
        get { return from >= end; }
      }
      public char this[int i] {
        get { return text[from + i]; }
      }
      public CharRange (string text, int from, int end) {
        this.text = text;
        this.from = from;
        this.end = end;
      }
      /// <summary>
      /// Move the starting index to the right.
      /// </summary>
      /// <param name="n">the number of characters to move over.</param>
      /// <returns>the new Range value.</returns>
      public CharRange move (int n) {
        return new CharRange (text, from + n, end);
      }
    }*/
    /// <summary> A Pattern object encapsulates an algorithm
    /// to recognize certain string pattern.
    /// When fed with a character range,
    /// a Pattern object either fails to match,
    /// or matches with the match length returned.
    /// <p />
    /// Pattern object is used for terminal level parsers. 
    /// A Pattern differs from a Parser in that it does not return object,
    /// and it simply reports mismatch whenenver fails.
    /// While Parser cannot be implemented directly, 
    /// Pattern can be implemented directly by user code.
    /// </summary>
    /// <author>  Ben Yu
    /// 
    /// Dec 16, 2004
    /// </author>
    public abstract class Pattern
    {
        /// <summary> returned by match() method when match fails.</summary>
        public const int MISMATCH = -1;
        /// <summary> The actual length of the pattern string is end - from.</summary>
        /// <param name="underlying_text">the underlying string.</param>
        /// <param name="starting_index">the starting index.</param>
        /// <param name="ending_index">the ending index.</param>
        /// <returns> the number of characters matched. MISMATCH otherwise.
        /// </returns>
        public abstract int Match(string underlying_text, int starting_index, int ending_index);
        /// <summary> First matches this pattern. 
        /// If succeed, match the remaining input against Pattern p2.
        /// Fails if either this or p2 fails.
        /// Succeed with the entire match length,
        /// which is the sum of the match length of this and p2.
        /// </summary>
        /// <param name="p2">the next Pattern object to match.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Seq(Pattern p2)
        {
            return new SeqPattern(this, p2);
        }
        /// <summary> Match with 0 length even if this pattern mismatches.</summary>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Optional()
        {
            return new OptionalPattern(this);
        }
        /// <summary> Matches this pattern for 0 or more times.
        /// Return the total match length.
        /// </summary>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Many()
        {
            return new ManyPattern(this);
        }
        /// <summary> Matches this pattern for at least min times.
        /// Return the total match length.
        /// </summary>
        /// <param name="min">the minimal number of times to match.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Many(int min)
        {
            if (min < 0)
                throw new System.ArgumentException("min<0");
            return new ManyMinPattern(min, this);
        }
        /// <summary> Matches this pattern for up to max times.
        /// Return the total match length.
        /// </summary>
        /// <param name="max">the maximal number of times to match.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Some(int max)
        {
            if (max < 0)
                throw new System.ArgumentException("max<0");
            if (max == 0)
                return Patterns.Always();
            return new SomePattern(max, this);
        }
        /// <summary> Matches this pattern for at least min times
        /// and at most max times.
        /// Return the total match length.
        /// </summary>
        /// <param name="min">the minimal number of times to match.
        /// </param>
        /// <param name="max">the maximal number of times to match.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Some(int min, int max)
        {
            if (min < 0 || max < 0 || min > max)
                throw new System.ArgumentException();
            if (max == 0)
                return Patterns.Always();
            return new SomeMinPattern(min, this, max);
        }
        /// <summary> If this pattern matches, return mismatch.
        /// return match length 0 otherwise.
        /// </summary>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Not()
        {
            return new NotPattern(this);
        }
        /// <summary> Matches with match length 0 if this Pattern object matches.
        /// Mismatch otherwise.
        /// </summary>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Peek()
        {
            return new PeekPattern(this);
        }

        /// <summary> If this pattern matches,
        /// match the remaining input against Pattern object yes.
        /// Otherwise, match the input against Pattern object no.
        /// </summary>
        /// <param name="yes">the true Pattern.
        /// </param>
        /// <param name="no">the false Pattern.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Ifelse(Pattern yes, Pattern no)
        {
            return new IfElsePattern(this, yes, no);
        }
        /// <summary> Matches the input against this pattern for n times.</summary>
        /// <param name="n">the number of times to match.
        /// </param>
        /// <returns> the new Pattern object.
        /// </returns>
        public Pattern Repeat(int n)
        {
            if (n == 0)
                return Patterns.Always();
            if (n == 1)
                return this;
            return new RepeatPattern(n, this);
        }
    }
}
