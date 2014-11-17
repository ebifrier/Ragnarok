using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.Kif
{
    using File;

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
        /// 「まで77手で先手の反則勝ち」など、特殊なコメントを判別します。
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
        /// 特殊な指し手をパースします。
        /// </summary>
        public static BoardMove ParseSpecialMove(string line)
        {
            var m = SpecialMoveRegex.Match(line);
            if (!m.Success)
            {
                return null;
            }

            var smoveType = SpecialMoveType.None;
            switch (m.Value)
            {
                case "中断":
                    smoveType = SpecialMoveType.Interrupt;
                    break;
                case "投了":
                    smoveType = SpecialMoveType.Resign;
                    break;
                case "持将棋":
                    smoveType = SpecialMoveType.Jishogi;
                    break;
                case "千日手":
                    smoveType = SpecialMoveType.Sennichite;
                    break;
                case "詰み":
                    smoveType = SpecialMoveType.CheckMate;
                    break;
                case "切れ負け":
                    smoveType = SpecialMoveType.TimeUp;
                    break;
                case "反則勝ち":
                case "反則負け":
                    smoveType = SpecialMoveType.IllegalMove;
                    break;
                default:
                    throw new InvalidOperationException(
                        m.Value + ": 対応していない特殊な指し手です。");
            }

            return BoardMove.CreateSpecialMove(BWType.None, smoveType);
        }

        /// <summary>
        /// ヘッダ行をパースし、そのパースしたアイテムを返します。
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
        /// ヘッダアイテム名から、その種類を判別します。
        /// </summary>
        public static KifuHeaderType? GetHeaderType(string str)
        {
            switch (str)
            {
                case "先手":
                case "下手":
                    return KifuHeaderType.BlackName;
                case "後手":
                case "上手":
                    return KifuHeaderType.WhiteName;
                case "棋戦":
                    return KifuHeaderType.Event;
                case "場所":
                    return KifuHeaderType.Site;
                case "持ち時間":
                    return KifuHeaderType.TimeLimit;
                case "開始日時":
                    return KifuHeaderType.StartTime;
                case "終了日時":
                    return KifuHeaderType.EndTime;
                case "戦型":
                    return KifuHeaderType.Opening;
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
                    return "先手";
                case KifuHeaderType.WhiteName:
                    return "後手";
                case KifuHeaderType.Event:
                    return "棋戦";
                case KifuHeaderType.Site:
                    return "場所";
                case KifuHeaderType.TimeLimit:
                    return "持ち時間";
                case KifuHeaderType.StartTime:
                    return "開始日時";
                case KifuHeaderType.EndTime:
                    return "終了日時";
                case KifuHeaderType.Opening:
                    return "戦型";
            }

            return null;
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
