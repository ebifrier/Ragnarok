using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi
{
    using Ragnarok.Utility;

    /// <summary>
    /// 将棋の各指し手を文字列から解析します。
    /// </summary>
    /// <remarks>
    /// 棋譜上の指し手は以下のような構造で記述されます。
    /// 
    ///  1   2   3   4   5   6
    /// ５	２	銀	右	上	成
    /// 
    /// 1 ･･･ 到達地点の筋
    /// 2 ･･･ 到達地点の段
    /// 3 ･･･ 駒の種類
    /// 4 ･･･ 駒の相対位置 [右・直・左]（複数ある場合）
    /// 5 ･･･ 駒の動作 [上・寄・引]（複数ある場合）
    /// 6 ･･･ 駒の成り不成など [成・不成・打]
    /// 
    /// http://www.shogi.or.jp/faq/kihuhyouki.html
    /// </remarks>
    public static class ShogiParserEx
    {
        /// <summary>
        /// 正規表現の選択用文字列 "～|～|･･･" の形式に変換します。
        /// </summary>
        private static string ConvertToRegexPattern(IEnumerable<string> list)
        {
            return string.Join(
               "|",
               list.OrderByDescending(_ => _.Length)
                   .Select(_ => Regex.Escape(_))
                   .ToArray());
        }

        /// <summary>
        /// 正規表現の選択用文字列 "～|～|･･･" の形式に変換します。
        /// </summary>
        private static string ConvertToRegexPattern<T>(IEnumerable<KeyValuePair<string, T>> table)
        {
            return ConvertToRegexPattern(table.Select(_ => _.Key));
        }

        /// <summary>
        /// 数字文字列を数字に変換するときに使います。
        /// </summary>
        private static readonly Dictionary<string, int> NumberTable =
            new Dictionary<string, int>()
        {
            {"0", 0}, {"０", 0}, {"零", 0}, {"ぜろ", 0}, {"れい", 0},
            {"ZERO", 0},
            {"例", 0}, {"礼", 0}, {"霊", 0}, {"令", 0},
            {"玲", 0}, {"怜", 0}, {"麗", 0},

            {"1", 1}, {"１", 1}, {"一", 1}, {"Ⅰ", 1}, {"①", 1},
            {"いち", 1}, {"いっ", 1}, {"いーち", 1},
            {"初", 1}, {"しょ", 1}, {"はつ", 1},
            {"位置", 1}, {"市", 1}, {"壱", 1},
            {"ONE", 1}, {"わん", 1},

            {"2", 2}, {"２", 2}, {"二", 2}, {"Ⅱ", 2}, {"②", 2},
            {"に", 2}, {"にー", 2}, {"にぃ", 2},
            {"弐", 2}, {"似", 2}, {"煮", 2}, {"荷", 2}, {"児", 2},
            {"TWO", 2}, {"とぅー", 2}, {"つー", 2},

            {"3", 3}, {"３", 3}, {"三", 3}, {"Ⅲ", 3}, {"③", 3},
            {"さん", 3}, {"さーん", 3},
            {"参", 3}, {"酸", 3}, {"産", 3}, {"山", 3}, {"桟", 3},
            {"算", 3}, {"賛", 3}, {"燦", 3}, {"惨", 3}, {"散", 3},
            {"THREE", 3}, {"すりー", 3},

            {"4", 4}, {"４", 4}, {"四", 4}, {"Ⅳ", 4}, {"④", 4},
            {"し", 4}, {"よ", 4}, {"よん", 4}, {"よーん", 4},
            {"詩", 4}, {"死", 4}, {"師", 4}, {"誌", 4}, {"史", 4},
            {"氏", 4}, {"士", 4}, {"資", 4}, {"紙", 4}, {"詞", 4},
            {"志", 4}, {"視", 4}, {"私", 4}, {"支", 4}, {"使", 4},
            {"FOUR", 4}, {"FOR", 4}, {"ふぉー", 4},

            {"5", 5}, {"５", 5}, {"五", 5}, {"Ⅴ", 5}, {"⑤", 5},
            {"ご", 5}, {"ごー", 5}, {"ごお", 5}, {"ごう", 5},
            {"ごぉ", 5}, {"ごぅ", 5}, {"GO", 5},
            {"語", 5}, {"御", 5}, {"後", 5}, {"碁", 5}, {"誤", 5},
            {"呉", 5}, {"伍", 5}, {"娯", 5}, {"午", 5}, {"互", 5},
            {"護", 5}, {"醐", 5},
            {"FIVE", 5}, {"ふぁいぶ", 5},

            {"6", 6}, {"６", 6}, {"六", 6}, {"Ⅵ", 6}, {"⑥", 6},
            {"む", 6}, {"むつ", 6}, {"ろ", 6}, {"ろっ", 6},
            {"ろく", 6}, {"ろっく", 6}, {"ろーく", 6},
            {"禄", 6}, {"碌", 6}, {"録", 6},
            {"SIX", 6}, {"しっくす", 6},

            {"7", 7}, {"７", 7}, {"七", 7}, {"Ⅶ", 7}, {"⑦", 7},
            {"しち", 7}, {"なな", 7}, {"なーな", 7}, {"なぁな", 7}, {"NANA", 7},
            {"奈々", 7}, {"菜々", 7}, {"奈菜", 7}, {"菜奈", 7},
            {"奈奈", 7}, {"菜菜", 7}, {"質", 7},
            {"SEVEN", 7}, {"せぶん", 7},

            {"8", 8}, {"８", 8}, {"八", 8}, {"Ⅷ", 8}, {"⑧", 8},
            {"や", 8}, {"はっ", 8}, {"はち", 8}, {"ぱち", 8}, {"はーち", 8},
            {"鉢", 8}, {"蜂", 8},
            {"EIGHT", 8}, {"EITO", 8}, {"えいと", 8},

            {"9", 9}, {"９", 9}, {"九", 9}, {"Ⅸ", 9}, {"⑨", 9},
            {"く", 9}, {"きゅう", 9}, {"きゅん", 9},
            {"区", 9}, {"句", 9}, {"苦", 9}, {"救", 9},
            {"旧", 9}, {"急", 9}, {"灸", 9}, {"球", 9}, {"給", 9}, {"休", 9}, 
            {"弓", 9}, {"究", 9}, {"求", 9}, {"吸", 9}, {"宮", 9}, {"丘", 9},
            {"NINE", 9}, {"ないん", 9},
        };

        /// <summary>
        /// 級や段などを変換するときに使います。
        /// </summary>
        private static readonly Dictionary<string, SkillKind> SkillKindTable =
            new Dictionary<string, SkillKind>()
        {
            {"級", SkillKind.Kyu},
            {"旧", SkillKind.Kyu},
            {"急", SkillKind.Kyu},
            {"灸", SkillKind.Kyu},
            {"球", SkillKind.Kyu},
            {"給", SkillKind.Kyu},
            {"休", SkillKind.Kyu},
            {"弓", SkillKind.Kyu},
            {"究", SkillKind.Kyu},
            {"求", SkillKind.Kyu},
            {"吸", SkillKind.Kyu},
            {"宮", SkillKind.Kyu},
            {"丘", SkillKind.Kyu},
            {"きゅう", SkillKind.Kyu},
            {"KYU", SkillKind.Kyu},
            {"KYUU", SkillKind.Kyu},

            {"段", SkillKind.Dan},
            {"男", SkillKind.Dan},
            {"弾", SkillKind.Dan},
            {"団", SkillKind.Dan},
            {"談", SkillKind.Dan},
            {"断", SkillKind.Dan},
            {"暖", SkillKind.Dan},
            {"壇", SkillKind.Dan},
            {"だん", SkillKind.Dan},
            {"DAN", SkillKind.Dan},
        };

        /*/// <summary>
        /// ハンゲームの棋力を文字列から変換するときに使います。
        /// </summary>
        private static SortedList<string, HangameSkill> hangameSkillTable =
            new SortedList<string, HangameSkill>(new StringComparer())
        {
            {"いっぱんじん", HangameSkill.NormalPeople},
            {"一般人", HangameSkill.NormalPeople},

            {"しょうぎつう", HangameSkill.ShogiKnower},
            {"将棋通", HangameSkill.ShogiKnower},

            {"しょうぎふぁん", HangameSkill.ShogiFun},
            {"将棋ファン", HangameSkill.ShogiFun},
            {"将棋ふぁん", HangameSkill.ShogiFun},

            {"きしみならい", HangameSkill.Apprenticeship},
            {"棋士見習い", HangameSkill.Apprenticeship},

            {"いっぱんきし", HangameSkill.LowPlayer},
            {"一般棋士", HangameSkill.LowPlayer},

            {"ちゅうきゅうきし", HangameSkill.MiddlePlayer},
            {"中級棋士", HangameSkill.MiddlePlayer},

            {"じょうきゅうきし", HangameSkill.SeniorPlayer},
            {"上級棋士", HangameSkill.SeniorPlayer},

            {"しはん", HangameSkill.Master},
            {"師範", HangameSkill.Master},

            {"たつじんきし", HangameSkill.ExpertPlayer},
            {"達人棋士", HangameSkill.ExpertPlayer},

            {"めいじんきし", HangameSkill.ProfessionalPlayer},
            {"名人棋士", HangameSkill.ProfessionalPlayer},

            {"ぎんしょう", HangameSkill.SilverGeneral},
            {"銀将", HangameSkill.SilverGeneral},

            {"きんしょう", HangameSkill.GoldGeneral},
            {"金将", HangameSkill.GoldGeneral},

            {"かくぎょう", HangameSkill.Bishop},
            {"角行", HangameSkill.Bishop},

            {"ひしょう", HangameSkill.Rook},
            {"飛将", HangameSkill.Rook},

            {"おうしょう", HangameSkill.King},
            {"王将", HangameSkill.King},
            };*/

        private static readonly Dictionary<string, SpecialMoveType> SpecialMoveTable =
            new Dictionary<string, SpecialMoveType>()
        {
            {"中断", SpecialMoveType.Interrupt},
            {"ちゅうだん", SpecialMoveType.Interrupt},
            {"INTERRPUT", SpecialMoveType.Interrupt},

            {"投了", SpecialMoveType.Resign},
            {"統領", SpecialMoveType.Resign},
            {"等量", SpecialMoveType.Resign},
            {"頭領", SpecialMoveType.Resign},
            {"棟梁", SpecialMoveType.Resign},
            {"とうりょう", SpecialMoveType.Resign},
            {"りざいん", SpecialMoveType.Resign},
            {"まけました", SpecialMoveType.Resign},
            {"負けました", SpecialMoveType.Resign},
            {"ありません", SpecialMoveType.Resign},
            {"TORYO", SpecialMoveType.Resign},
            {"RESIGN", SpecialMoveType.Resign},

            {"持将棋", SpecialMoveType.Jishogi},
            {"じしょうぎ", SpecialMoveType.Jishogi},
            {"JISHOGI", SpecialMoveType.Jishogi},

            {"千日手", SpecialMoveType.Sennichite},
            {"せんにちて", SpecialMoveType.Sennichite},
            {"SENNICHITE", SpecialMoveType.Sennichite},

            {"王手の千日手", SpecialMoveType.OuteSennichite},
            {"王手のせんにちて", SpecialMoveType.OuteSennichite},
            {"おうての千日手", SpecialMoveType.OuteSennichite},
            {"おうてのせんにちて", SpecialMoveType.OuteSennichite},
            {"王手千日手", SpecialMoveType.OuteSennichite},
            {"王手せんにちて", SpecialMoveType.OuteSennichite},
            {"おうて千日手", SpecialMoveType.OuteSennichite},
            {"おうてせんにちて", SpecialMoveType.OuteSennichite},
            {"OUTESENNICHITE", SpecialMoveType.OuteSennichite},
            {"OUTE_SENNICHITE", SpecialMoveType.OuteSennichite},

            {"時間切れ", SpecialMoveType.TimeUp},
            {"時間ぎれ", SpecialMoveType.TimeUp},
            {"時間きれ", SpecialMoveType.TimeUp},
            {"時間切", SpecialMoveType.TimeUp},
            {"じかんぎれ", SpecialMoveType.TimeUp},
            {"じかんきれ", SpecialMoveType.TimeUp},
            {"TIMEUP", SpecialMoveType.TimeUp},
            {"TIME_UP", SpecialMoveType.TimeUp},

            {"反則", SpecialMoveType.IllegalMove},
            {"反則手", SpecialMoveType.IllegalMove},
            {"ILLEGALMOVE", SpecialMoveType.IllegalMove},
            {"ILLEGAL_MOVE", SpecialMoveType.IllegalMove},

            {"詰み", SpecialMoveType.CheckMate},
            {"詰", SpecialMoveType.CheckMate},
            {"つみ", SpecialMoveType.CheckMate},
            {"CHECKMATE", SpecialMoveType.CheckMate},
            {"CHECK_MATE", SpecialMoveType.CheckMate},

            {"最大手数", SpecialMoveType.MaxMoves},
            {"MAXMOVES", SpecialMoveType.MaxMoves},
            {"MAX_MOVES", SpecialMoveType.MaxMoves},

            {"封じ手", SpecialMoveType.SealedMove },
            {"ふうじて", SpecialMoveType.SealedMove },

            {"エラー", SpecialMoveType.Error},
            {"ERROR", SpecialMoveType.Error},
        };

        /// <summary>
        /// 文字列を駒に変換するための変換テーブルです。
        /// </summary>
        /// <remarks>
        /// 成銀、成香などを追加するため、readonlyにはできません。
        /// </remarks>
        private static Dictionary<string, Piece> PieceTable =
            new Dictionary<string, Piece>()
        {
            {"飛車", Piece.Rook},
            {"飛者", Piece.Rook},
            {"飛", Piece.Rook},
            {"非", Piece.Rook},
            {"日", Piece.Rook},
            {"比", Piece.Rook},
            {"火", Piece.Rook},
            {"秘", Piece.Rook},
            {"飛び", Piece.Rook},
            {"飛ぶ", Piece.Rook},
            {"跳び", Piece.Rook},
            {"跳ぶ", Piece.Rook},
            {"鳶", Piece.Rook},
            {"ひしゃ", Piece.Rook},
            {"ひ", Piece.Rook},
            {"とび", Piece.Rook},
            {"とぶ", Piece.Rook},
            {"るーく", Piece.Rook},
            {"HISYA", Piece.Rook},
            {"HISHA", Piece.Rook},
            {"HI", Piece.Rook},
            {"ROOK", Piece.Rook},

            //{"角行", Piece.Bishop},
            {"角", Piece.Bishop},
            {"核", Piece.Bishop},
            {"格", Piece.Bishop},
            {"画", Piece.Bishop},
            {"各", Piece.Bishop},
            {"閣", Piece.Bishop},
            {"欠く", Piece.Bishop},
            {"欠", Piece.Bishop},
            {"書く", Piece.Bishop},
            {"書", Piece.Bishop},
            {"描く", Piece.Bishop},
            {"描", Piece.Bishop},
            {"かく", Piece.Bishop},
            {"かど", Piece.Bishop},
            {"つの", Piece.Bishop},
            {"びしょっぷ", Piece.Bishop},
            {"KAKU", Piece.Bishop},
            {"BISHOP", Piece.Bishop},

            {"玉", Piece.King},
            {"王", Piece.King},
            {"球", Piece.King},
            {"弾", Piece.King},
            {"ぎょく", Piece.King},
            {"たま", Piece.King},
            {"おう", Piece.King},
            {"きんぐ", Piece.King},
            {"神", Piece.King},
            {"かみ", Piece.King},
            {"多摩", Piece.King},
            {"GYOKU", Piece.King},
            {"TAMA", Piece.King},
            {"OU", Piece.King},
            {"KAMI", Piece.King},
            {"KING", Piece.King},

            {"金", Piece.Gold},
            {"衾", Piece.Gold},
            {"斤", Piece.Gold},
            {"菌", Piece.Gold},
            {"禁", Piece.Gold},
            {"筋", Piece.Gold},
            {"禽", Piece.Gold},
            {"きん", Piece.Gold},
            {"かね", Piece.Gold},
            {"ごーるど", Piece.Gold},
            {"KIN", Piece.Gold},
            {"KINN", Piece.Gold},
            {"GOLD", Piece.Gold},

            {"銀", Piece.Silver},
            {"吟", Piece.Silver},
            {"ぎん", Piece.Silver},
            {"しるば", Piece.Silver},
            {"GIN", Piece.Silver},
            {"GINN", Piece.Silver},
            {"SILVER", Piece.Silver},

            {"桂馬", Piece.Knight},
            {"桂", Piece.Knight},
            {"系", Piece.Knight},
            {"刑", Piece.Knight},
            {"軽", Piece.Knight},
            {"型", Piece.Knight},
            {"桂魔", Piece.Knight},
            {"けいま", Piece.Knight},
            {"けいば", Piece.Knight},
            {"けい", Piece.Knight},
            {"け", Piece.Knight},
            {"ぴょん", Piece.Knight},
            {"かつら", Piece.Knight},
            {"ないと", Piece.Knight},
            {"KEIMA", Piece.Knight},
            {"KEI", Piece.Knight},
            {"K", Piece.Knight},
            {"KNIGHT", Piece.Knight},

            {"香車", Piece.Lance},
            {"香", Piece.Lance},
            {"香り", Piece.Lance},
            {"薫り", Piece.Lance},
            {"香織", Piece.Lance},
            {"今日", Piece.Lance},
            {"京", Piece.Lance},
            {"恐", Piece.Lance},
            {"強", Piece.Lance},
            {"教", Piece.Lance},
            {"橋", Piece.Lance},
            {"凶", Piece.Lance},
            {"鏡", Piece.Lance},
            {"槍", Piece.Lance},
            {"卿", Piece.Lance},
            {"興", Piece.Lance},
            {"きょうしゃ", Piece.Lance},
            {"きょう", Piece.Lance},
            {"きょ", Piece.Lance},
            {"かおり", Piece.Lance},
            {"やり", Piece.Lance},
            {"KYOUSYA", Piece.Lance},
            {"KYOU", Piece.Lance},
            {"KAORI", Piece.Lance},
            {"KYO", Piece.Lance},
            {"LANCE", Piece.Lance},

            {"歩兵", Piece.Pawn},
            {"歩", Piece.Pawn},
            {"負", Piece.Pawn},
            {"富", Piece.Pawn},
            {"婦", Piece.Pawn},
            {"不", Piece.Pawn},
            {"腐", Piece.Pawn},
            {"府", Piece.Pawn},
            {"符", Piece.Pawn},
            {"ふ", Piece.Pawn},
            {"ぷ", Piece.Pawn},
            {"ほ", Piece.Pawn},
            {"ぽ", Piece.Pawn},
            {"ふう", Piece.Pawn},
            {"ふぅ", Piece.Pawn},
            {"ぽーん", Piece.Pawn},
            {"FU", Piece.Pawn},
            {"HU", Piece.Pawn},
            {"PO", Piece.Pawn},
            {"PU", Piece.Pawn},
            {"PAQN", Piece.Pawn},
            {"POON", Piece.Pawn},
            {"PON", Piece.Pawn},

            /* 以下、成り駒 */
            {"龍", Piece.Dragon},
            {"竜", Piece.Dragon},
            {"流", Piece.Dragon},
            {"粒", Piece.Dragon},
            {"劉", Piece.Dragon},
            {"瑠", Piece.Dragon},
            {"留", Piece.Dragon},
            {"琉", Piece.Dragon},
            {"瘤", Piece.Dragon},
            {"どらごん", Piece.Dragon},
            {"りゅう", Piece.Dragon},
            {"りゅ", Piece.Dragon},
            {"RYU", Piece.Dragon},
            {"RYUU", Piece.Dragon},
            {"DRAGO", Piece.Dragon},
            {"DRAGON", Piece.Dragon},

            {"馬", Piece.Horse},
            {"午", Piece.Horse},
            {"旨", Piece.Horse},
            {"ほーす", Piece.Horse},
            {"うま", Piece.Horse},
            {"UMA", Piece.Horse},
            {"HORSE", Piece.Horse},

            {"と", Piece.ProPawn},
            {"TO", Piece.ProPawn},
        };

        /// <summary>
        /// 同銀などの判別を行うためのテーブルです。
        /// </summary>
        private static readonly List<string> SameAsOldTable = new List<string>()
        {
            "同じく", "おなじく",
            "どう", "とう", "どー",
            "同", "銅", "動", "道", "堂", "胴", "導", "童", "憧",
            "豆", "党", "闘", "燈", "等", "刀", "頭", "島", "塔",
            "棟", "当", "糖", "灯",
            "DOU",
        };

        /// <summary>
        /// 手番を変換するときに使います。
        /// </summary>
        private static readonly Dictionary<string, Colour> ColourTable =
            new Dictionary<string, Colour>()
        {
            {"▲", Colour.Black},
            {"▼", Colour.Black},

            {"△", Colour.White},
            {"▽", Colour.White},
        };

        /// <summary>
        /// 駒の後につけられるポストフィックスのリストです。
        /// </summary>
        private static readonly List<string> PiecePostfixTable = new List<string>
        {
            "さん", "ちゃん", "くん", "君", "さま", "様",
            "ちゅう", "厨", "きち", "吉", "ごん",
        };

        /// <summary>
        /// 駒の相対位置の判別を行うためのテーブルです。
        /// </summary>
        private static readonly Dictionary<string, RelFileType> RelFileTypeTable =
            new Dictionary<string, RelFileType>()
        {
            {"左", RelFileType.Left},
            {"ひだり", RelFileType.Left},
            {"←", RelFileType.Left},
            {"⇐", RelFileType.Left},
            {"⇚", RelFileType.Left},
            {"㊧", RelFileType.Left},
            {"HIDARI", RelFileType.Left},
            {"LEFT", RelFileType.Left},

            {"右", RelFileType.Right},
            {"みぎ", RelFileType.Right},
            {"→", RelFileType.Right},
            {"⇒", RelFileType.Left},
            {"⇛", RelFileType.Left},
            {"㊨", RelFileType.Right},
            {"MIGI", RelFileType.Left},
            {"RIGHT", RelFileType.Left},

            {"直ぐ", RelFileType.Straight},
            {"直", RelFileType.Straight},
            {"勅", RelFileType.Straight},
            {"すぐ", RelFileType.Straight},
            {"ちょく", RelFileType.Straight},
            {"TYOKU", RelFileType.Straight},
            {"CHOKU", RelFileType.Straight},
            {"SUGU", RelFileType.Straight},
        };

        /// <summary>
        /// 駒の動きの判別を行うためのテーブルです。
        /// </summary>
        private static readonly Dictionary<string, RankMoveType> RankMoveTypeTable =
            new Dictionary<string, RankMoveType>()
        {
            {"上がる", RankMoveType.Up},
            {"上", RankMoveType.Up},
            {"あがる", RankMoveType.Up},
            {"うえ", RankMoveType.Up},
            {"↑", RankMoveType.Up},
            {"UP", RankMoveType.Up},
            {"UE", RankMoveType.Up},
            {"AGARU", RankMoveType.Up},
            {"行く", RankMoveType.Up},
            {"行", RankMoveType.Up},
            {"いく", RankMoveType.Up},
            {"ぎょう", RankMoveType.Up},
            {"IKU", RankMoveType.Up},
            {"入る", RankMoveType.Up},
            {"入", RankMoveType.Up},
            {"はいる", RankMoveType.Up},
            {"いる", RankMoveType.Up},
            {"にゅう", RankMoveType.Up},
            {"HAIRU", RankMoveType.Up},
            {"IRI", RankMoveType.Up},
            {"IRU", RankMoveType.Up},

            {"引き", RankMoveType.Back},
            {"引く", RankMoveType.Back},
            {"ひき", RankMoveType.Back},
            {"ひく", RankMoveType.Back},
            {"引", RankMoveType.Back},
            {"↓", RankMoveType.Back},
            {"下がる", RankMoveType.Back},
            {"下", RankMoveType.Back},
            {"さがる", RankMoveType.Back},
            {"した", RankMoveType.Back},
            {"BACK", RankMoveType.Back},
            {"HIKU", RankMoveType.Back},
            {"HIKI", RankMoveType.Back},
            {"SAGARU", RankMoveType.Back},
            {"SITA", RankMoveType.Back},

            {"やどりき", RankMoveType.Sideways},
            {"寄る", RankMoveType.Sideways},
            {"寄り", RankMoveType.Sideways},
            {"寄き", RankMoveType.Sideways},
            {"よる", RankMoveType.Sideways},
            {"より", RankMoveType.Sideways},
            {"寄", RankMoveType.Sideways},
            {"SIDEWAY", RankMoveType.Sideways},
            {"YORU", RankMoveType.Sideways},
            {"YORI", RankMoveType.Sideways},
        };

        /// <summary>
        /// 駒打ちなどのアクションの判別を行うためのテーブルです。
        /// </summary>
        /// <remarks>
        /// "不"成りを追加するため、readonlyにはできません。
        /// </remarks>
        private static Dictionary<string, ActionType> ActionTable =
            new Dictionary<string, ActionType>()
        {
            {"成らず", ActionType.Unpromote},
            {"ならず", ActionType.Unpromote},
            {"成らる", ActionType.Unpromote},
            {"ならる", ActionType.Unpromote},
            {"成らるり", ActionType.Unpromote},
            {"ならるり", ActionType.Unpromote},
            {"しげるらず", ActionType.Unpromote},
            {"しげらず", ActionType.Unpromote},
            {"NARAZU", ActionType.Unpromote},

            {"鳴り", ActionType.Promote},
            {"鳴る", ActionType.Promote},
            {"成り", ActionType.Promote},
            {"なり", ActionType.Promote},
            {"成る", ActionType.Promote},
            {"なる", ActionType.Promote},
            {"成るり", ActionType.Promote},
            {"なるり", ActionType.Promote},
            {"成", ActionType.Promote},
            {"しげる", ActionType.Promote},
            {"NARI", ActionType.Promote},
            {"NARU", ActionType.Promote},

            {"打ち", ActionType.Drop},
            {"打つ", ActionType.Drop},
            {"うち", ActionType.Drop},            
            {"うつ", ActionType.Drop},
            {"打", ActionType.Drop},
            {"撃つ", ActionType.Drop},
            {"討つ", ActionType.Drop},
            {"撃ち", ActionType.Drop},
            {"討ち", ActionType.Drop},
            {"鬱", ActionType.Drop},
            {"田", ActionType.Drop},
            {"駄", ActionType.Drop},
            {"堕", ActionType.Drop},
            {"蛇", ActionType.Drop},
            {"だ", ActionType.Drop},
            {"UTU", ActionType.Drop},
            {"UTI", ActionType.Drop},
            {"DA", ActionType.Drop},
        };

        /// <summary>
        /// 文字の先頭に数字があるか調べ、もしあればその数字を取得します。
        /// </summary>
        private static int? GetNumber(string text)
        {
            int result;
            if (!NumberTable.TryGetValue(text, out result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// 棋力の種別を特定します。
        /// </summary>
        private static SkillKind GetSkillKind(string text)
        {
            SkillKind result;
            if (!SkillKindTable.TryGetValue(text, out result))
            {
                return SkillKind.Unknown;
            }

            return result;
        }

        /// <summary>
        /// 文字列から駒の種類を特定します。
        /// </summary>
        private static Piece GetPiece(string text)
        {
            Piece result;
            if (!PieceTable.TryGetValue(text, out result))
            {
                return Piece.None;
            }

            return result;
        }

        /// <summary>
        /// 文字列から手番を特定します。
        /// </summary>
        private static Colour GetColour(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Colour.None;
            }

            Colour result;
            if (!ColourTable.TryGetValue(text, out result))
            {
                return Colour.None;
            }

            return result;
        }

        /// <summary>
        /// 文字列から駒の相対位置を特定します。
        /// </summary>
        private static RelFileType GetRelFileType(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return RelFileType.None;
            }

            RelFileType result;
            if (!RelFileTypeTable.TryGetValue(text, out result))
            {
                return RelFileType.None;
            }

            return result;
        }

        /// <summary>
        /// 文字列から駒の動きの種類を特定します。
        /// </summary>
        private static RankMoveType GetRankMoveType(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return RankMoveType.None;
            }

            RankMoveType result;
            if (!RankMoveTypeTable.TryGetValue(text, out result))
            {
                return RankMoveType.None;
            }

            return result;
        }

        /// <summary>
        /// 文字列から駒打ちなどのアクションを特定します。
        /// </summary>
        private static ActionType GetActionType(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return ActionType.None;
            }

            ActionType result;
            if (!ActionTable.TryGetValue(text, out result))
            {
                return ActionType.None;
            }

            return result;
        }

        /// <summary>
        /// 投了などの特殊な指し手を取得します。
        /// </summary>
        private static SpecialMoveType GetSpecialMoveType(string text)
        {
            SpecialMoveType smoveType;
            if (SpecialMoveTable.TryGetValue(text, out smoveType))
            {
                return smoveType;
            }

            return SpecialMoveType.None;
        }

        /// <summary>
        /// 投了などの特殊な指し手用の正規表現オブジェクトを作成します。
        /// </summary>
        private static Regex CreateSpecialMoveRegex()
        {
            var pattern = string.Format(
                CultureInfo.CurrentCulture,
                @"^(あ(､|、|っ)?)?({0})?({1})",
                ConvertToRegexPattern(ColourTable),
                ConvertToRegexPattern(SpecialMoveTable));

            return new Regex(pattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// 駒を文字列から解析するためのテーブルを設定します。
        /// </summary>
        private static void ModifyAndSetPieceTable()
        {
            var resultTable = PieceTable;

            var tmpTable = new Dictionary<string, Piece>(resultTable);
            foreach (var pair in tmpTable)
            {
                // 成銀など成り駒を設定します。
                if (!pair.Value.IsPromoted() &&
                    (pair.Value == Piece.Silver ||
                     pair.Value == Piece.Knight ||
                     pair.Value == Piece.Lance))
                {
                    var piece = pair.Value.Promote();
                    
                    foreach (var item in ActionTable)
                    {
                        if (item.Value == ActionType.Promote)
                        {
                            resultTable.Add(item.Key + pair.Key, piece);
                        }                        
                    }
                }
                else if (pair.Value == Piece.Gold)
                {
                    resultTable.Add("と" + pair.Key, Piece.ProPawn);
                }
            }

            PieceTable = resultTable;
        }

        /// <summary>
        /// 駒打ちなどの動作のテーブルを修正した後設定します。
        /// </summary>
        private static void ModifyAndSetActionTable()
        {
            var resultTable = ActionTable;

            var tmpTable = new SortedList<string, ActionType>(resultTable);
            foreach (var pair in tmpTable)
            {
                if (pair.Value == ActionType.Promote)
                {
                    resultTable.Add("ふ" + pair.Key, ActionType.Unpromote);
                    resultTable.Add("不" + pair.Key, ActionType.Unpromote);
                }
            }

            ActionTable = resultTable;
        }

        /// <summary>
        /// 指し手解析用の正規表現オブジェクトを作成します。
        /// </summary>
        /// <remarks>
        ///  1   2   3   4   5   6
        /// ５	２	銀	右	上	成
        /// 
        /// 1 ･･･ 到達地点の筋
        /// 2 ･･･ 到達地点の段
        /// 3 ･･･ 駒の種類
        /// 4 ･･･ 駒の相対位置 [右・直・左]（複数ある場合）
        /// 5 ･･･ 駒の動作 [上・寄・引]（複数ある場合）
        /// 6 ･･･ 駒の成り不成など [成・不成・打]
        /// </remarks>
        private static Regex CreateMoveRegex()
        {
            // 指し手の前に空白があってもおｋとします。
            var moveRegexPattern = string.Format(
                CultureInfo.CurrentCulture,
                @"^\s*({0})?({1})?({1})?(?:({2})\s*)?(?:お)?({3})(?:ー|～)?(?:{4})?(?:ー|～)?({5})?({6})?({7})?([(](\d)(\d)[)])?",
                ConvertToRegexPattern(ColourTable),
                ConvertToRegexPattern(NumberTable),
                ConvertToRegexPattern(SameAsOldTable),
                ConvertToRegexPattern(PieceTable),
                ConvertToRegexPattern(PiecePostfixTable),
                ConvertToRegexPattern(RelFileTypeTable),
                ConvertToRegexPattern(RankMoveTypeTable),
                ConvertToRegexPattern(ActionTable));

            return new Regex(moveRegexPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// 指し手のパースを行います。
        /// </summary>
        public static LiteralMove ParseMove(string text, bool isNeedEnd)
        {
            var tmp = string.Empty;

            return ParseMoveEx(text, isNeedEnd, true, ref tmp);
        }

        /// <summary>
        /// 差し手のパースを行います。
        /// </summary>
        public static LiteralMove ParseMoveEx(string text, bool isNeedEnd,
                                              bool isNormalizeText,
                                              ref string parsedText)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var move = new LiteralMove()
            {
                OriginalText = text.Trim(),
            };

            // 文字列を正規化します。
            if (isNormalizeText)
            {
                text = StringNormalizer.NormalizeText(
                    text,
                    NormalizeTextOptions.All & ~NormalizeTextOptions.KanjiDigit);
            }

            // 投了などの特殊な指し手の判断を行います。
            var m = SpecialMoveRegex.Match(text);
            if (m.Success)
            {
                // 手番が指定されていればそれを取得します。
                move.Colour = GetColour(m.Groups[3].Value);

                // 投了などの指し手の種類を取得します。
                var smoveType = GetSpecialMoveType(m.Groups[4].Value);
                if (smoveType == SpecialMoveType.None)
                {
                    return null;
                }

                move.SpecialMoveType = smoveType;
            }
            else
            {
                m = MoveRegex.Match(text);
                if (!m.Success)
                {
                    return null;
                }

                // 手番が指定されていれば、それを取得します。
                move.Colour = GetColour(m.Groups[1].Value);

                // 33同銀などを有効にします。
                // 筋・段
                var file = GetNumber(m.Groups[2].Value);
                var rank = GetNumber(m.Groups[3].Value);

                // 「同～」であるかどうかを特定します。
                // ３同銀などの指し手を受理しないように、数字があれば、
                // ちゃんと二つとも設定されているか調べます。
                move.SameAsOld = m.Groups[4].Success;
                if (!move.SameAsOld || file != null || rank != null)
                {
                    if (file == null || rank == null)
                    {
                        return null;
                    }
                    move.File = file.Value;
                    move.Rank = rank.Value;
                }

                // 駒の種類
                var piece = GetPiece(m.Groups[5].Value);
                if (piece.IsNone())
                {
                    return null;
                }
                move.Piece = piece;

                // 相対位置
                move.RelFileType = GetRelFileType(m.Groups[6].Value);

                // 駒の移動の種類
                move.RankMoveType = GetRankMoveType(m.Groups[7].Value);

                // 駒打ちなどのアクション
                move.ActionType = GetActionType(m.Groups[8].Value);

                // 移動前の位置
                if (m.Groups[9].Success)
                {
                    move.SrcSquare = SquareUtil.Create(
                        int.Parse(m.Groups[10].Value, CultureInfo.CurrentCulture),
                        int.Parse(m.Groups[11].Value, CultureInfo.CurrentCulture));
                }
            }

            // 解析文字列が正しく終わっているか調べます。
            if (isNeedEnd &&
                (m.Length < text.Length && !char.IsWhiteSpace(text[m.Length])))
            {
                return null;
            }
            if (!move.Validate())
            {
                return null;
            }

            parsedText = m.Value;
            return move;
        }

        /// <summary>
        /// 参加者の情報を解析する正規表現オブジェクトを作成します。
        /// </summary>
        private static Regex CreatePlayerRegex()
        {
            // 棋力文字列の中に空白を含めるのは可能とします。
            const string regexPattern =
                @"^\s*([^\s]+)\s+([\s\S]+)\s*";

            return new Regex(regexPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// 参加者の棋力を解析する正規表現オブジェクトを作成します。
        /// </summary>
        /// <remarks>
        /// 14級などを解析します。
        /// </remarks>
        private static Regex CreateSkillLevelRegex()
        {
            // 棋力は文字列のどこにあってもかまいません。
            var regexPattern = string.Format(
                CultureInfo.InvariantCulture,
                @"({0})?({0})({1})",
                ConvertToRegexPattern(NumberTable),
                ConvertToRegexPattern(SkillKindTable));

            return new Regex(regexPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// プレイヤー文字列のパースを行います。
        /// </summary>
        /// <remarks>
        /// 受け入れ可能な構文。
        /// 
        /// * 名前 メッセージ
        /// </remarks>
        public static ShogiPlayer ParsePlayer(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            SkillLevel skillLevel = null;
            var nickname = string.Empty;

            // 正規化すると十が10に変換されたりしますが、
            // そうなると名前が変わってしまう可能性があります。
            // そのため、参加者のパース時は文字列の正規化を行いません。
            //
            //text = StringNormalizer.NormalizeText(
            //    text, NormalizeTextOptions.Number);
            var m = PlayerRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }

            // 名前の解析します。
            if (m.Groups[1].Success)
            {
                nickname = m.Groups[1].Value;
            }

            // 棋力の解析を行います。
            if (m.Groups[2].Success)
            {
                skillLevel = ParseSkillLevel(m.Groups[2].Value);
            }

            // もし棋力か名前がnullな場合は
            // プレイヤーとして正しくありません。
            if ((skillLevel == null || string.IsNullOrEmpty(skillLevel.OriginalText)) ||
                string.IsNullOrEmpty(nickname))
            {
                return null;
            }
            
            // 棋力はnull以外の値を採用します。
            return new ShogiPlayer()
            {
                OriginalText = (string)text.Clone(),
                SkillLevel = (skillLevel ?? new SkillLevel()),
                Nickname = nickname,
            };
        }

        /// <summary>
        /// 棋力を解析します。
        /// </summary>
        /// <remarks>
        /// <paramref name="text"/>に棋力を表す文字列が含まれていれば
        /// 棋力として認定することにします。
        /// (さすらいの１２級　とかもあり)
        /// </remarks>
        public static SkillLevel ParseSkillLevel(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var skillLevel = new SkillLevel()
            {
                OriginalText = text.Trim()
            };

            var normalized = StringNormalizer.NormalizeText(text);
            var index = 0;
            while (true)
            {
                // マッチする文字列がある間、棋力の検索をします。
                var m = SkillLevelRegex.Match(normalized, index);
                if (!m.Success)
                {
                    break;
                }

                // 棋力の解析を行います。
                var n10 = GetNumber(m.Groups[1].Value);
                var n1 = GetNumber(m.Groups[2].Value);
                if (n1 == null)
                {
                    index = m.Index + m.Length;
                    continue;
                }

                var skillKind = GetSkillKind(m.Groups[3].Value);
                if (skillKind == SkillKind.Unknown)
                {
                    index = m.Index + m.Length;
                    continue;
                }

                // プレイヤーの棋力を設定します。
                skillLevel.Kind = skillKind;
                skillLevel.Grade =
                    ((n10 != null ? n10.Value : 0) * 10 + n1.Value);

                return skillLevel;
            }

            // 棋力不明で登録時の文字列だけ保持します。
            return skillLevel;
        }

        private static readonly Regex SpecialMoveRegex;
        private static readonly Regex MoveRegex;
        private static readonly Regex PlayerRegex;
        private static readonly Regex SkillLevelRegex;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static ShogiParserEx()
        {
            ModifyAndSetPieceTable();
            ModifyAndSetActionTable();

            SpecialMoveRegex = CreateSpecialMoveRegex();
            MoveRegex = CreateMoveRegex();
            PlayerRegex = CreatePlayerRegex();
            SkillLevelRegex = CreateSkillLevelRegex();
        }
    }
}
