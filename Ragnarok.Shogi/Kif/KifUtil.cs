using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi.Kif
{
    using File;

    /// <summary>
    /// 棋譜コメントに関するデータを扱います。
    /// </summary>
    public sealed class KifCommentData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifCommentData(string comment, bool isMoveComment)
        {
            Comment = comment;
            IsMoveComment = isMoveComment;
        }

        /// <summary>
        /// コメントを取得または設定します。
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// 棋譜ファイルではなく指し手に関わるコメントかどうかを取得または設定します。
        /// </summary>
        public bool IsMoveComment
        {
            get;
            set;
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
                { '玉', Piece.King },
                { '飛', Piece.Rook },
                { '龍', Piece.Dragon },
                { '角', Piece.Bishop },
                { '馬', Piece.Horse },
                { '金', Piece.Gold },
                { '銀', Piece.Silver },
                { '全', Piece.ProSilver },
                { '桂', Piece.Knight },
                { '圭', Piece.ProKnight },
                { '香', Piece.Lance },
                { '杏', Piece.ProLance },
                { '歩', Piece.Pawn },
                { 'と', Piece.ProPawn },
            };

        private static readonly Dictionary<Piece, char> PieceToCharDic =
             CreatePieceToCharDic();

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        private static Dictionary<Piece, char> CreatePieceToCharDic()
        {
            var dic = new Dictionary<Piece, char>();

            // keyとvalueを入れ替えます。
            CharToPieceDic.ForEach(_ => dic.Add(_.Value, _.Key));
            return dic;
        }

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
            SpecialMoveText, RegexOptions.Compiled);

        /// <summary>
        /// コメント行かどうか調べます。
        /// </summary>
        public static bool IsCommentLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            return (
                line.Length == 0 ||
                line[0] == '#' ||
                line[0] == '*');
        }

        /// <summary>
        /// コメント行のパースを行います。
        /// </summary>
        /// <returns>
        /// コメント行である場合はコメント内容（""も含みます）を
        /// そうでない場合はnullを返します。
        /// </returns>
        public static KifCommentData ParseCommentLine(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            // 空行もコメント行として考える
            if (string.IsNullOrEmpty(line))
            {
                return new KifCommentData(string.Empty, false);
            }

            if (line[0] != '#' && line[0] != '*')
            {
                // コメントにあらず
                return null;
            }

            return new KifCommentData(
                line.Substring(1).TrimStart(' ', '　', '\t'),
                line[0] == '*');
        }

        /// <summary>
        /// 特殊な指し手をパースします。
        /// </summary>
        public static LiteralMove ParseSpecialMove(string line)
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

            return new LiteralMove
            {
                SpecialMoveType = smoveType,
            };
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

        private static readonly Regex ScoreEngineNameRegex =
            new Regex(@"^評価値エンジン(\d+)");

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
                case "評価値タイプ":
                    return KifuHeaderType.ScoreType;
            }

            var m = ScoreEngineNameRegex.Match(str);
            if (m.Success)
            {
                var i = int.Parse(m.Groups[1].Value, CultureInfo.CurrentCulture);
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
                case KifuHeaderType.ScoreType:
                    return "評価値タイプ";
            }

            if (KifuHeaderType.ScoreEngine0 + 1 <= type &&
                type <= KifuHeaderType.ScoreEngine0 + 16)
            {
                var i = type - KifuHeaderType.ScoreEngine0;
                return $"評価値エンジン{i}";
            }

            return null;
        }

        /// <summary>
        /// 駒文字を駒に変換します。(先後の情報はなし)
        /// </summary>
        public static Piece? CharToPiece(char pieceCh)
        {
            if (!CharToPieceDic.TryGetValue(pieceCh, out var piece))
            {
                return null;
            }

            return piece;
        }

        /// <summary>
        /// 'vと'などの文字を駒に変換します。(先後の情報ありS)
        /// </summary>
        public static Piece? StrToPiece(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length < 2)
            {
                throw new ArgumentException(
                    "文字列が短すぎます。", nameof(str));
            }

            // まず手番
            var colourCh = char.ToLowerInvariant(str[0]);
            if (colourCh != ' ' && colourCh != 'v')
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

            var colour = (colourCh == 'v' ? Colour.White : Colour.Black);
            return PieceUtil.Modify(piece.Value, colour);
        }

        /// <summary>
        /// 駒文字に駒を変換します。
        /// </summary>
        public static string PieceToChar(Piece piece)
        {
            if (piece.IsNone())
            {
                throw new ArgumentException(
                    "pieceが不正です。", nameof(piece));
            }

            if (!PieceToCharDic.TryGetValue(piece, out var ch))
            {
                return null;
            }

            return new string(ch, 1);
        }

        /// <summary>
        /// 駒文字列に駒を変換します。
        /// </summary>
        public static string PieceToStr(Piece piece)
        {
            if (piece.IsNone())
            {
                return " ・";
            }

            var pieceStr = PieceToChar(piece.GetPieceType());
            if (pieceStr == null)
            {
                return null;
            }

            var turnCh = (piece.GetColour() == Colour.White ? 'v' : ' ');
            return (turnCh + pieceStr);
        }
    }
}
