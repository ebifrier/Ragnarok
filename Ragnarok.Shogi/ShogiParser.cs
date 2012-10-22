using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public static class ShogiParser
    {
        /// <summary>
        /// 正規表現の選択用文字列 "～|～|･･･" の形式に変換します。
        /// </summary>
        private static string ConvertToRegexPattern(IEnumerable<string> list)
        {
            return string.Join(
               "|",
               list.OrderByDescending(value => value.Length)
                   .Select<string, string>(Regex.Escape).ToArray());
        }

        /// <summary>
        /// 正規表現の選択用文字列 "～|～|･･･" の形式に変換します。
        /// </summary>
        private static string ConvertToRegexPattern<T>(IEnumerable<KeyValuePair<string, T>> table)
        {
            return string.Join(
               "|",
               table.OrderByDescending(pair => pair.Key.Length)
                    .Select(pair => Regex.Escape(pair.Key))
                       .ToArray());
        }

        /// <summary>
        /// 数字文字列を数字に変換するときに使います。
        /// </summary>
        private static readonly Dictionary<string, int> numberTable =
            new Dictionary<string, int>()
        {
            {"0", 0}, {"０", 0}, {"零", 0}, {"ぜろ", 0}, {"れい", 0},
                {"ZERO", 0},

            {"1", 1}, {"１", 1}, {"一", 1}, {"Ⅰ", 1}, {"①", 1},
                {"いち", 1}, {"いっ", 1}, {"いーち", 1},
                {"初", 1}, {"しょ", 1}, {"はつ", 1},

            {"2", 2}, {"２", 2}, {"二", 2}, {"Ⅱ", 2}, {"②", 2},
                {"に", 2}, {"にー", 2},

            {"3", 3}, {"３", 3}, {"三", 3}, {"Ⅲ", 3}, {"③", 3},
                {"さん", 3}, {"さーん", 3},

            {"4", 4}, {"４", 4}, {"四", 4}, {"Ⅳ", 4}, {"④", 4},
                {"し", 4}, {"よ", 4}, {"よん", 4}, {"よーん", 4},

            {"5", 5}, {"５", 5}, {"五", 5}, {"Ⅴ", 5}, {"⑤", 5},
                {"ご", 5}, {"ごー", 5}, {"ごお", 5}, {"ごう", 5}, {"GO", 5},

            {"6", 6}, {"６", 6}, {"六", 6}, {"Ⅵ", 6}, {"⑥", 6},
                {"む", 6}, {"むつ", 6}, {"ろ", 6}, {"ろっ", 6}, {"ろく", 6}, {"ろーく", 6},

            {"7", 7}, {"７", 7}, {"七", 7}, {"Ⅶ", 7}, {"⑦", 7},
                {"しち", 7}, {"なな", 7}, {"なーな", 7}, {"NANA", 7},

            {"8", 8}, {"８", 8}, {"八", 8}, {"Ⅷ", 8}, {"⑧", 8},
                {"や", 8}, {"はっ", 8}, {"はち", 8}, {"ぱち", 8}, {"はーち", 8},

            {"9", 9}, {"９", 9}, {"九", 9}, {"Ⅸ", 9}, {"⑨", 9},
                {"く", 9},
                // "級"と被ってしまうためひらがなはつけません。
                // {"きゅう", 9}, {"きゅん", 9},
        };

        /// <summary>
        /// 級や段などを変換するときに使います。
        /// </summary>
        private static readonly Dictionary<string, SkillKind> skillKindTable =
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

        /// <summary>
        /// 投了の判定を行うためのテーブルです。
        /// </summary>
        private static readonly List<string> resignTable = new List<string>
        {
            "投了", "頭領", "等量", "棟梁",
            "とうりょう", "とーりょー", "とーりょ",
            "負けました", "まけました",
            "ありません",
            "参りました", "まいりました",
            "降参", "こうさん",
            "TORYO",
        };

        /// <summary>
        /// 文字列を駒に変換するための変換テーブルです。
        /// </summary>
        /// <remarks>
        /// 成銀、成香などを追加するため、readonlyにはできません。
        /// </remarks>
        private static Dictionary<string, Piece> pieceTable =
            new Dictionary<string, Piece>()
        {
            {"飛車", Piece.Hisya},
            {"飛", Piece.Hisya},
            {"非", Piece.Hisya},
            {"日", Piece.Hisya},
            {"比", Piece.Hisya},
            {"火", Piece.Hisya},
            {"秘", Piece.Hisya},
            {"飛び", Piece.Hisya},
            {"飛ぶ", Piece.Hisya},
            {"跳び", Piece.Hisya},
            {"跳ぶ", Piece.Hisya},
            {"鳶", Piece.Hisya},
            {"ひしゃ", Piece.Hisya},
            {"ひ", Piece.Hisya},
            {"とび", Piece.Hisya},
            {"とぶ", Piece.Hisya},
            {"HISYA", Piece.Hisya},
            {"HI", Piece.Hisya},

            {"角行", Piece.Kaku},
            {"角", Piece.Kaku},
            {"核", Piece.Kaku},
            {"格", Piece.Kaku},
            {"画", Piece.Kaku},
            {"各", Piece.Kaku},
            {"閣", Piece.Kaku},
            {"欠く", Piece.Kaku},
            {"欠", Piece.Kaku},
            {"書く", Piece.Kaku},
            {"書", Piece.Kaku},
            {"描く", Piece.Kaku},
            {"描", Piece.Kaku},
            {"かく", Piece.Kaku},
            {"かど", Piece.Kaku},
            {"つの", Piece.Kaku},
            {"KAKU", Piece.Kaku},

            {"玉", Piece.Gyoku},
            {"王", Piece.Gyoku},
            {"球", Piece.Gyoku},
            {"弾", Piece.Gyoku},
            {"ぎょく", Piece.Gyoku},
            {"たま", Piece.Gyoku},
            {"おう", Piece.Gyoku},
            {"神", Piece.Gyoku},
            {"かみ", Piece.Gyoku},
            {"多摩", Piece.Gyoku},
            {"GYOKU", Piece.Gyoku},
            {"TAMA", Piece.Gyoku},
            {"OU", Piece.Gyoku},

            {"金", Piece.Kin},
            {"衾", Piece.Kin},
            {"斤", Piece.Kin},
            {"菌", Piece.Kin},
            {"禁", Piece.Kin},
            {"筋", Piece.Kin},
            {"禽", Piece.Kin},
            {"きん", Piece.Kin},
            {"かね", Piece.Kin},
            {"KIN", Piece.Kin},

            {"銀", Piece.Gin},
            {"吟", Piece.Gin},
            {"ぎん", Piece.Gin},
            {"GIN", Piece.Gin},

            {"桂馬", Piece.Kei},
            {"桂", Piece.Kei},
            {"系", Piece.Kei},
            {"刑", Piece.Kei},
            {"軽", Piece.Kei},
            {"型", Piece.Kei},
            {"桂魔", Piece.Kei},
            {"けいま", Piece.Kei},
            {"けいば", Piece.Kei},
            {"けい", Piece.Kei},
            {"けー", Piece.Kei},
            {"ぴょん", Piece.Kei},
            {"かつら", Piece.Kei},
            {"KEIMA", Piece.Kei},
            {"KEI", Piece.Kei},
            {"K", Piece.Kei},

            {"香車", Piece.Kyo},
            {"香", Piece.Kyo},
            {"香り", Piece.Kyo},
            {"薫り", Piece.Kyo},
            {"香織", Piece.Kyo},
            {"今日", Piece.Kyo},
            {"京", Piece.Kyo},
            {"強", Piece.Kyo},
            {"教", Piece.Kyo},
            {"橋", Piece.Kyo},
            {"凶", Piece.Kyo},
            {"鏡", Piece.Kyo},
            {"槍", Piece.Kyo},
            {"卿", Piece.Kyo},
            {"興", Piece.Kyo},
            {"きょうしゃ", Piece.Kyo},
            {"きょう", Piece.Kyo},
            {"きょー", Piece.Kyo},
            {"かおり", Piece.Kyo},
            {"やり", Piece.Kyo},
            {"KYOUSYA", Piece.Kyo},
            {"KYOU", Piece.Kyo},
            {"KAORI", Piece.Kyo},

            {"歩兵", Piece.Hu},
            {"歩", Piece.Hu},
            {"負", Piece.Hu},
            {"富", Piece.Hu},
            {"婦", Piece.Hu},
            {"腐", Piece.Hu},
            {"府", Piece.Hu},
            {"符", Piece.Hu},
            {"ふ", Piece.Hu},
            {"ほ", Piece.Hu},
            {"FU", Piece.Hu},
            {"HU", Piece.Hu},

            /* 以下、成り駒 */
            {"龍", Piece.Ryu},
            {"竜", Piece.Ryu},
            {"流", Piece.Ryu},
            {"粒", Piece.Ryu},
            {"りゅう", Piece.Ryu},
            {"りゅ", Piece.Ryu},
            {"RYU", Piece.Ryu},

            {"馬", Piece.Uma},
            {"午", Piece.Uma},
            {"旨", Piece.Uma},
            {"うま", Piece.Uma},
            {"UMA", Piece.Uma},

            {"と", Piece.To},
            {"ト", Piece.To},
            {"TO", Piece.To},
        };

        /// <summary>
        /// 同銀などの判別を行うためのテーブルです。
        /// </summary>
        private static readonly List<string> sameAsOldTable = new List<string>()
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
        private static readonly Dictionary<string, BWType> bwTypeTable =
            new Dictionary<string, BWType>()
        {
            {"▲", BWType.Black},
            {"▼", BWType.Black},

            {"△", BWType.White},
            {"▽", BWType.White},
        };

        /// <summary>
        /// 駒の相対位置の判別を行うためのテーブルです。
        /// </summary>
        private static readonly Dictionary<string, RelFileType> relFileTypeTable =
            new Dictionary<string, RelFileType>()
        {
            {"左", RelFileType.Left},
            {"ひだり", RelFileType.Left},
            {"←", RelFileType.Left},
            {"㊧", RelFileType.Left},
            {"HIDARI", RelFileType.Left},

            {"右", RelFileType.Right},
            {"みぎ", RelFileType.Right},
            {"→", RelFileType.Right},
            {"㊨", RelFileType.Right},
            {"MIGI", RelFileType.Left},

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
        private static readonly Dictionary<string, RankMoveType> rankMoveTypeTable =
            new Dictionary<string, RankMoveType>()
        {
            {"上がる", RankMoveType.Up},
            {"上", RankMoveType.Up},
            {"あがる", RankMoveType.Up},
            {"うえ", RankMoveType.Up},
            {"↑", RankMoveType.Up},
            {"UE", RankMoveType.Up},
            {"AGARU", RankMoveType.Up},

            {"引き", RankMoveType.Back},
            {"引く", RankMoveType.Back},
            {"ひき", RankMoveType.Back},
            {"ひく", RankMoveType.Back},
            {"引", RankMoveType.Back},
            {"↓", RankMoveType.Back},
            {"HIKU", RankMoveType.Back},
            {"HIKI", RankMoveType.Back},

            {"やどりき", RankMoveType.Sideways},
            {"寄る", RankMoveType.Sideways},
            {"寄り", RankMoveType.Sideways},
            {"寄き", RankMoveType.Sideways},
            {"よる", RankMoveType.Sideways},
            {"より", RankMoveType.Sideways},
            {"寄", RankMoveType.Sideways},
            {"YORU", RankMoveType.Sideways},
            {"YORI", RankMoveType.Sideways},
        };

        /// <summary>
        /// 駒打ちなどのアクションの判別を行うためのテーブルです。
        /// </summary>
        /// <remarks>
        /// "不"成りを追加するため、readonlyにはできません。
        /// </remarks>
        private static Dictionary<string, ActionType> actionTable =
            new Dictionary<string, ActionType>()
        {
            {"成らず", ActionType.Unpromote},
            {"ならず", ActionType.Unpromote},
            {"しげるらず", ActionType.Unpromote},
            {"しげらず", ActionType.Unpromote},
            {"NARAZU", ActionType.Unpromote},

            {"鳴り", ActionType.Promote},
            {"鳴る", ActionType.Promote},
            {"成り", ActionType.Promote},
            {"なり", ActionType.Promote},
            {"成る", ActionType.Promote},
            {"なる", ActionType.Promote},
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
            if (!numberTable.TryGetValue(text, out result))
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
            if (!skillKindTable.TryGetValue(text, out result))
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
            if (!pieceTable.TryGetValue(text, out result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// 文字列から手番を特定します。
        /// </summary>
        private static BWType GetBWType(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BWType.None;
            }

            BWType result;
            if (!bwTypeTable.TryGetValue(text, out result))
            {
                return BWType.None;
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
            if (!relFileTypeTable.TryGetValue(text, out result))
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
            if (!rankMoveTypeTable.TryGetValue(text, out result))
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
            if (!actionTable.TryGetValue(text, out result))
            {
                return ActionType.None;
            }

            return result;
        }

        /// <summary>
        /// 投了用の正規表現オブジェクトを作成します。
        /// </summary>
        private static Regex CreateResignRegex()
        {
            return new Regex(
                ConvertToRegexPattern(resignTable),
                RegexOptions.Compiled);
        }

        /// <summary>
        /// 駒を文字列から解析するためのテーブルを設定します。
        /// </summary>
        private static void ModifyAndSetPieceTable()
        {
            var resultTable = pieceTable;

            var tmpTable = new Dictionary<string, Piece>(resultTable);
            foreach (var pair in tmpTable)
            {
                // 成銀など成り駒を設定します。
                if (!pair.Value.IsPromoted &&
                    (pair.Value.PieceType == PieceType.Gin ||
                     pair.Value.PieceType == PieceType.Kei ||
                     pair.Value.PieceType == PieceType.Kyo))
                {
                    var piece = new Piece(pair.Value.PieceType, true);
                    
                    foreach (var item in actionTable)
                    {
                        if (item.Value == ActionType.Promote)
                        {
                            resultTable.Add(item.Key + pair.Key, piece);
                        }                        
                    }
                }
                else if (pair.Value.PieceType == PieceType.Kin)
                {
                    resultTable.Add("と" + pair.Key, Piece.To);
                }
            }

            pieceTable = resultTable;
        }

        /// <summary>
        /// 駒打ちなどの動作のテーブルを修正した後設定します。
        /// </summary>
        private static void ModifyAndSetActionTable()
        {
            var resultTable = actionTable;

            var tmpTable = new SortedList<string, ActionType>(resultTable);
            foreach (var pair in tmpTable)
            {
                if (pair.Value == ActionType.Promote)
                {
                    resultTable.Add("ふ" + pair.Key, ActionType.Unpromote);
                    resultTable.Add("不" + pair.Key, ActionType.Unpromote);
                }
            }

            actionTable = resultTable;
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
            var piecePostfixList = new string[]
            {
                "ちゃん", "くん", "さん", "さま","様",
                "厨","ちゅう","きち","吉",
            };

            // 指し手の前に空白があってもおｋとします。
            var moveRegexPattern = string.Format(
                "^\\s*({0})?({1})?({1})?(?:({2})\\s*)?(?:お)?({3})(?:{4})?(?:ー|～)?({5})?({6})?({7})?([(](\\d)(\\d)[)])?",
                ConvertToRegexPattern(bwTypeTable),
                ConvertToRegexPattern(numberTable),
                ConvertToRegexPattern(sameAsOldTable),
                ConvertToRegexPattern(pieceTable),
                ConvertToRegexPattern(piecePostfixList),
                ConvertToRegexPattern(relFileTypeTable),
                ConvertToRegexPattern(rankMoveTypeTable),
                ConvertToRegexPattern(actionTable));

            return new Regex(moveRegexPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// 指し手のパースを行います。
        /// </summary>
        public static Move ParseMove(string text, bool isNeedEnd)
        {
            var tmp = string.Empty;

            return ParseMoveEx(text, isNeedEnd, true, ref tmp);
        }

        /// <summary>
        /// 差し手のパースを行います。
        /// </summary>
        public static Move ParseMoveEx(string text, bool isNeedEnd,
                                       bool isNormalizeText,
                                       ref string parsedText)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var move = new Move()
            {
                OriginalText = text.Trim(),
            };

            // 文字列を正規化します。
            if (isNormalizeText)
            {
                text = StringNormalizer.NormalizeText(text);
            }

            // 投了の判断を行います。
            var m = resignRegex.Match(text);
            if (m.Success)
            {
                move.IsResigned = true;
            }
            else
            {
                m = moveRegex.Match(text);
                if (!m.Success)
                {
                    return null;
                }

                // 手番が指定されていれば、それを取得します。
                move.BWType = GetBWType(m.Groups[1].Value);

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
                    // 筋
                    if (file == null)
                    {
                        return null;
                    }
                    move.File = file.Value;

                    // 段
                    if (rank == null)
                    {
                        return null;
                    }
                    move.Rank = rank.Value;
                }

                // 駒の種類
                var piece = GetPiece(m.Groups[5].Value);
                if (piece == null)
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
                    move.OldPosition = new Position(
                        int.Parse(m.Groups[10].Value),
                        int.Parse(m.Groups[11].Value));
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
        /// <remarks>
        /// 棋力＠名前
        /// </remarks>
        private static Regex CreatePlayerRegex()
        {
            // 棋力文字列の中に空白を含めるのは可能とします。
            const string regexPattern =
                @"^\s*([^@＠]+)?\s*(?:[@＠]\s*([^\s]+))?";

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
                @"({0})?({0})({1})",
                ConvertToRegexPattern(numberTable),
                ConvertToRegexPattern(skillKindTable));

            return new Regex(regexPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// プレイヤー文字列のパースを行います。
        /// </summary>
        /// <remarks>
        /// 受け入れ可能な構文。
        /// 
        /// * 棋力＠名前
        /// * 名前
        /// * @名前
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
            // ので、参加者のパース時には正規化はしないことにしています。
            //
            //text = StringNormalizer.NormalizeText(
            //    text, NormalizeTextOption.Number);
            var m = playerRegex.Match(text);
            if (!m.Success)
            {
                return null;
            }

            // 棋力の解析を行います。
            if (m.Groups[1].Success)
            {
                skillLevel = ParseSkillLevel(m.Groups[1].Value);
            }

            // 名前の解析します。
            if (m.Groups[2].Success)
            {
                nickname = m.Groups[2].Value;
            }

            // もし棋力と名前の両方がnullない場合は
            // プレイヤーとして正しくありません。
            if ((skillLevel == null || string.IsNullOrEmpty(skillLevel.OriginalText)) &&
                string.IsNullOrEmpty(nickname))
            {
                return null;
            }

            // 棋力があって名前がない場合は、それを名前として設定します。
            if (skillLevel != null &&
                string.IsNullOrEmpty(nickname))
            {
                nickname = skillLevel.OriginalText;
                skillLevel = null;
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
                var m = skillLevelRegex.Match(normalized, index);
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

        private static readonly Regex resignRegex;
        private static readonly Regex moveRegex;
        private static readonly Regex playerRegex;
        private static readonly Regex skillLevelRegex;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static ShogiParser()
        {
            ModifyAndSetPieceTable();
            ModifyAndSetActionTable();

            resignRegex = CreateResignRegex();
            moveRegex = CreateMoveRegex();
            playerRegex = CreatePlayerRegex();
            skillLevelRegex = CreateSkillLevelRegex();
        }
    }
}
