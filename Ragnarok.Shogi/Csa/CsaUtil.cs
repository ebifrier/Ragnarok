using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
        /// <example>
        /// $NAME:VALUE
        /// $NAME
        /// NAME:VALUE
        /// NAME
        /// </example>
        private static readonly Regex HeaderRegex = new Regex(
            @"^\s*([$]?)(.+?)(\s*[:]\s*(.*))?\s*$",
            RegexOptions.Compiled);

        /// <summary>
        /// CSAファイルのコメント行を判別します。
        /// </summary>
        public static bool IsCommentLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            return (line.Length == 0 || line[0] == '\'');
        }

        /// <summary>
        /// CSAファイルのヘッダ行を解析します。
        /// </summary>
        public static HeaderItem ParseHeaderItem(string line, bool needKeySign = true)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            var m = HeaderRegex.Match(line);
            if (!m.Success)
            {
                return null;
            }

            // ヘッダ行先頭の$はなくてもいい場合があります。
            if (needKeySign && !m.Groups[1].Success)
            {
                return null;
            }

            var key = m.Groups[2].Value;
            var value = (m.Groups[4].Success ? m.Groups[4].Value : null);
            return new HeaderItem(key, value);
        }

        private static readonly Regex ScoreEngineNameRegex =
            new Regex(@"^SCORE_ENGINE(\d+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// CSA形式のヘッダアイテム名から、その種類を判別します。
        /// </summary>
        public static KifuHeaderType? GetHeaderType(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            switch (key.ToUpperInvariant())
            {
                case "EVENT":
                    return KifuHeaderType.Event;
                case "SITE":
                    return KifuHeaderType.Site;
                case "START_TIME":
                    return KifuHeaderType.StartTime;
                case "END_TIME":
                    return KifuHeaderType.EndTime;
                case "TIME_LIMIT":
                    return KifuHeaderType.TimeLimit;
                case "OPENING":
                    return KifuHeaderType.Opening;
                case "SCORE_TYPE":
                    return KifuHeaderType.ScoreType;
            }

            var m = ScoreEngineNameRegex.Match(key);
            if (m.Success)
            {
                var i = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                return (KifuHeaderType.ScoreEngine0 + i);
            }

            return null;
        }

        /// <summary>
        /// ヘッダアイテムの種類から、その名前を取得します。
        /// </summary>
        public static string GetHeaderName(KifuHeaderType type)
        {
            switch (type)
            {
                case KifuHeaderType.BlackName:
                case KifuHeaderType.WhiteName:
                    return null;
                case KifuHeaderType.Event:
                    return "EVENT";
                case KifuHeaderType.Site:
                    return "SITE";
                case KifuHeaderType.StartTime:
                    return "START_TIME";
                case KifuHeaderType.EndTime:
                    return "END_TIME";
                case KifuHeaderType.TimeLimit:
                    return "TIME_LIMIT";
                case KifuHeaderType.Opening:
                    return "OPENING";
                case KifuHeaderType.ScoreType:
                    return "SCORE_TYPE";
            }

            if (KifuHeaderType.ScoreEngine0 + 1 <= type &&
                type <= KifuHeaderType.ScoreEngine0 + 16)
            {
                var i = type - KifuHeaderType.ScoreEngine0;
                return $"SCORE_ENGINE{i}";
            }

            return null;
        }

        /// <summary>
        /// 駒のCSA表示文字列を取得します。
        /// </summary>
        public static string PieceToStr(Piece piece)
        {
            if (piece == null || piece.PieceType == PieceType.None)
            {
                return "* ";
            }

            foreach (var pair in PieceTable)
            {
                if (pair.Value == piece)
                {
                    return pair.Key;
                }
            }

            return "* ";
        }

        /// <summary>
        /// 駒のCSA表示文字列を取得します。
        /// </summary>
        public static string BoardPieceToStr(BoardPiece piece)
        {
            if (piece == null || piece.PieceType == PieceType.None)
            {
                return " * ";
            }

            var turnStr = (piece.BWType == BWType.Black ? '+' : '-');
            var pieceStr = PieceToStr(piece.Piece);
            return $"{turnStr}{pieceStr}";
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

            var piece = StrToPiece(str.Length > 2 ? str.Substring(1) : str);
            if (piece == null)
            {
                return null;
            }

            return new BoardPiece(piece, bwType);
        }
    }
}
