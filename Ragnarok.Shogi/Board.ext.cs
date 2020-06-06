using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Shogi
{
    using Csa;
    using Kif;
    using Sfen;

    /// <summary>
    /// 棋譜やusiなどのためのメソッドを追加します。
    /// </summary>
    public partial class Board
    {
        /// <summary>
        /// csa形式の文字列から局面を作成します。
        /// </summary>
        public static Board ParseCsa(string csa)
        {
            return CsaBoard.Parse(csa);
        }

        /// <summary>
        /// csa形式に局面を変換します。
        /// </summary>
        public string ToCsa()
        {
            return CsaBoard.BoardToCsa(this);
        }

        /// <summary>
        /// bod形式の文字列から局面を作成します。
        /// </summary>
        public static Board ParseBod(string bod)
        {
            return BodBoard.Parse(bod);
        }

        /// <summary>
        /// bod形式に局面を変換します。
        /// </summary>
        public string ToBod(int? moveCount = null)
        {
            return BodBoard.BoardToBod(this, moveCount);
        }

        /// <summary>
        /// sfen形式の文字列から局面を作成します。
        /// </summary>
        public static Board ParseSfen(string sfen)
        {
            return SfenBoard.Parse(sfen);
        }

        /// <summary>
        /// sfen形式に局面を変換します。
        /// </summary>
        public string ToSfen()
        {
            return SfenBoard.BoardToSfen(this);
        }
    }
}
