using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.Kif
{
    /// <summary>
    /// kifファイルの各ヘッダアイテムを保持します。
    /// </summary>
    public sealed class HeaderItem
    {
        /// <summary>
        /// キーを取得します。
        /// </summary>
        public string Key
        {
            get;
            private set;
        }

        /// <summary>
        /// 値を取得します。
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HeaderItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// kifやbod形式で扱う便利クラスです。
    /// </summary>
    public static class KifUtil
    {
        private static readonly Dictionary<char, Piece> CharToPieceDic =
            new Dictionary<char, Piece>
            {
                { '・', Piece.None },
                { '玉', Piece.Gyoku },
                { '飛', Piece.Hisya },
                { '龍', Piece.Ryu },
                { '角', Piece.Kaku },
                { '馬', Piece.Uma },
                { '金', Piece.Kin },
                { '銀', Piece.Gin },
                { '全', Piece.NariGin },
                { '桂', Piece.Kei },
                { '圭', Piece.NariKei },
                { '香', Piece.Kyo },
                { '杏', Piece.NariKyo },
                { '歩', Piece.Hu },
                { 'と', Piece.To },
            };

        private static readonly Dictionary<Piece, char> PieceToCharDic;

        /// <summary>
        /// ヘッダー部分の正規表現
        /// </summary>
        private static readonly Regex HeaderRegex = new Regex(
            @"^(.+?)\s*(?:[：]\s*(.*?))\s*$",
            RegexOptions.Compiled);

        /// <summary>
        /// 特殊な指し手文字列
        /// </summary>
        public static readonly string SpecialMoveText =
            @"中断|投了|持将棋|千日手|詰み|切れ負け|反則勝ち|反則負け";

        /// <summary>
        /// 「まで77手で先手の勝ち」など、特殊なコメントを判別します。
        /// </summary>
        public static readonly Regex SpecialMoveRegex = new Regex(
            string.Format(@"{0}", SpecialMoveText),
            RegexOptions.Compiled);

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static KifUtil()
        {
            PieceToCharDic = new Dictionary<Piece, char>();

            // keyとvalueを入れ替えます。
            CharToPieceDic.ForEach(_ =>
                PieceToCharDic.Add(_.Value, _.Key));
        }

        /// <summary>
        /// コメント行かどうか調べます。
        /// </summary>
        public static bool IsCommentLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            return (
                line.Length == 0 ||
                line[0] == '#' ||
                line[0] == '*');
        }

        /// <summary>
        /// ヘッダ行をパースします。
        /// </summary>
        public static HeaderItem ParseHeaderItem(string line)
        {
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
        /// 駒文字を駒に変換します。(先後の情報はなし)
        /// </summary>
        public static Piece CharToPiece(char pieceCh)
        {
            Piece piece;
            if (!CharToPieceDic.TryGetValue(pieceCh, out piece))
            {
                return null;
            }

            return piece;
        }

        /// <summary>
        /// 'vと'などの文字を駒に変換します。(先後の情報ありS)
        /// </summary>
        public static BoardPiece StrToPiece(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < 2)
            {
                throw new ArgumentException("str");
            }

            // まず手番
            var bwTypeCh = char.ToLower(str[0]);
            if (bwTypeCh != ' ' && bwTypeCh != 'v')
            {
                return null;
            }

            // そのあと駒
            var pieceCh = str[1];
            var piece = CharToPiece(pieceCh);
            if (piece == null)
            {
                return null;
            }

            var bwType = (bwTypeCh == 'v' ? BWType.White : BWType.Black);
            return new BoardPiece(piece, bwType);
        }

        /// <summary>
        /// 駒文字に駒を変換します。
        /// </summary>
        public static string PieceToChar(Piece piece)
        {
            if (piece != null && !piece.Validate())
            {
                throw new ArgumentException("piece");
            }

            if (piece == null)
            {
                return "・";
            }

            char ch;
            if (!PieceToCharDic.TryGetValue(piece, out ch))
            {
                return null;
            }

            return new string(ch, 1);
        }

        /// <summary>
        /// 駒文字列に駒を変換します。
        /// </summary>
        public static string PieceToStr(BoardPiece piece)
        {
            if (piece != null && !piece.Validate())
            {
                throw new ArgumentException("piece");
            }

            if (piece == null)
            {
                return " ・";
            }

            var pieceStr = PieceToChar(piece.Piece);
            if (pieceStr == null)
            {
                return null;
            }

            var turnCh = (piece.BWType == BWType.White ? 'v' : ' ');
            return (turnCh + pieceStr);
        }
    }
}
