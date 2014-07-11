using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Ragnarok.Shogi;
using Ragnarok.Utility;

namespace Ragnarok.Shogi.Csa
{
    using File;

    /// <summary>
    /// CSA形式の駒の移動などを保持します。
    /// </summary>
    public static class CsaUtil
    {
        private static readonly Dictionary<string, Piece> PieceTable =
            new Dictionary<string, Piece>
            {
                { "* ", Piece.None }, // null pruning など
                { "**", Piece.None },
                { "OU", Piece.Gyoku },
                { "HI", Piece.Hisya },
                { "KA", Piece.Kaku },
                { "KI", Piece.Kin },
                { "GI", Piece.Gin },
                { "KE", Piece.Kei},
                { "KY", Piece.Kyo  },
                { "FU", Piece.Hu },
                { "RY", Piece.Ryu },
                { "UM", Piece.Uma },
                { "NG", Piece.NariGin },
                { "NK", Piece.NariKei },
                { "NY", Piece.NariKyo },
                { "TO", Piece.To },
            };

        /// <summary>
        /// ヘッダー部分の正規表現
        /// </summary>
        private static readonly Regex HeaderRegex = new Regex(
            @"^\s*[$](.+)\s*[:]\s*(.*)\s*$",
            RegexOptions.Compiled);

        /// <summary>
        /// CSAファイルのコメント行を判別します。
        /// </summary>
        public static bool IsCommentLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            return (
                line.Length == 0 ||
                line[0] == '\'');
        }

        /// <summary>
        /// CSAファイルのヘッダ行を解析します。
        /// </summary>
        public static HeaderItem ParseHeaderItem(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            var m = HeaderRegex.Match(line);
            if (!m.Success)
            {
                return null;
            }

            var key = m.Groups[1].Value;
            var value = m.Groups[2].Value;
            return new HeaderItem(key, value);
        }

        /// <summary>
        /// 駒のCSA表示文字列を取得します。
        /// </summary>
        public static string PieceToChar(Piece piece)
        {
            if (piece == null || piece.PieceType == PieceType.None)
            {
                return "**";
            }

            foreach (var pair in PieceTable)
            {
                if (pair.Value == piece)
                {
                    return pair.Key;
                }
            }

            return "**";
        }

        /// <summary>
        /// CSA形式の指し手を解析します。
        /// </summary>
        public static Piece StrToPiece(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            Piece piece;
            if (!PieceTable.TryGetValue(str, out piece))
            {
                return null;
            }

            return piece;
        }

        /// <summary>
        /// CSA形式の指し手を解析します。
        /// </summary>
        public static BoardPiece StrToBoardPiece(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            var bwType = (
                str[0] == '+' ? BWType.Black :
                str[0] == '-' ? BWType.White :
                BWType.None);
            var piece = StrToPiece(str.Substring(1));

            return new BoardPiece(piece, bwType);
        }
    }
}
