using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi
{
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
            {"0", 0}, {"０", 0},
            {"1", 1}, {"１", 1}, {"一", 1},
            {"2", 2}, {"２", 2}, {"二", 2},
            {"3", 3}, {"３", 3}, {"三", 3},
            {"4", 4}, {"４", 4}, {"四", 4},
            {"5", 5}, {"５", 5}, {"五", 5},
            {"6", 6}, {"６", 6}, {"六", 6},
            {"7", 7}, {"７", 7}, {"七", 7},
            {"8", 8}, {"８", 8}, {"八", 8},
            {"9", 9}, {"９", 9}, {"九", 9},
        };

        private static readonly Dictionary<string, SpecialMoveType> SpecialMoveTable =
            new Dictionary<string, SpecialMoveType>()
        {
            {"中断", SpecialMoveType.Interrupt},
            {"INTERRPUT", SpecialMoveType.Interrupt},

            {"投了", SpecialMoveType.Resign},
            {"TORYO", SpecialMoveType.Resign},
            {"RESIGN", SpecialMoveType.Resign},

            {"持将棋", SpecialMoveType.Jishogi},
            {"JISHOGI", SpecialMoveType.Jishogi},

            {"千日手", SpecialMoveType.Sennichite},
            {"SENNICHITE", SpecialMoveType.Sennichite},
            
            {"王手の千日手", SpecialMoveType.OuteSennichite},
            {"OUTESENNICHITE", SpecialMoveType.OuteSennichite},
            {"OUTE_SENNICHITE", SpecialMoveType.OuteSennichite},

            {"時間切れ", SpecialMoveType.TimeUp},
            {"時間ぎれ", SpecialMoveType.TimeUp},
            {"TIMEUP", SpecialMoveType.TimeUp},
            {"TIME_UP", SpecialMoveType.TimeUp},

            {"反則", SpecialMoveType.IllegalMove},
            {"反則手", SpecialMoveType.IllegalMove},
            {"ILLEGALMOVE", SpecialMoveType.IllegalMove},
            {"ILLEGAL_MOVE", SpecialMoveType.IllegalMove},

            {"詰み", SpecialMoveType.CheckMate},

            {"最大手数", SpecialMoveType.MaxMoves},

            {"封じ手", SpecialMoveType.SealedMove },

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
            {"玉", Piece.King},
            {"王", Piece.King},
            {"飛", Piece.Rook},
            {"角", Piece.Bishop},
            {"金", Piece.Gold},
            {"銀", Piece.Silver},
            {"桂", Piece.Knight},
            {"香", Piece.Lance},
            {"歩", Piece.Pawn},

            /* 以下、成り駒 */
            {"龍", Piece.Dragon},
            {"竜", Piece.Dragon},
            {"馬", Piece.Horse},
            {"と", Piece.ProPawn},
        };

        /// <summary>
        /// 同銀などの判別を行うためのテーブルです。
        /// </summary>
        private static readonly List<string> SameAsOldTable = new List<string>()
        {
            "同",
        };

        /// <summary>
        /// 手番を変換するときに使います。
        /// </summary>
        private static readonly Dictionary<string, BWType> BWTypeTable =
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
        private static readonly Dictionary<string, RelFileType> RelFileTypeTable =
            new Dictionary<string, RelFileType>()
        {
            {"左", RelFileType.Left},
            {"右", RelFileType.Right},
            {"直", RelFileType.Straight},
        };

        /// <summary>
        /// 駒の動きの判別を行うためのテーブルです。
        /// </summary>
        private static readonly Dictionary<string, RankMoveType> RankMoveTypeTable =
            new Dictionary<string, RankMoveType>()
        {
            {"上", RankMoveType.Up},
            {"引", RankMoveType.Back},
            {"寄", RankMoveType.Sideways},
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
            {"不成", ActionType.Unpromote},
            {"成", ActionType.Promote},
            {"打", ActionType.Drop},
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
        private static BWType GetBWType(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BWType.None;
            }

            BWType result;
            if (!BWTypeTable.TryGetValue(text, out result))
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
                @"^({0})?({1})",
                ConvertToRegexPattern(BWTypeTable),
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
                @"^\s*({0})?({1})?({1})?(?:({2})\s*)?({3})({4})?({5})?({6})?([(](\d)(\d)[)])?",
                ConvertToRegexPattern(BWTypeTable),
                ConvertToRegexPattern(NumberTable),
                ConvertToRegexPattern(SameAsOldTable),
                ConvertToRegexPattern(PieceTable),
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

            return ParseMoveEx(text, isNeedEnd, ref tmp);
        }

        /// <summary>
        /// 指し手のパースを行います。
        /// </summary>
        public static LiteralMove ParseMoveEx(string text, bool isNeedEnd,
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

            // 投了などの特殊な指し手の判断を行います。
            var m = SpecialMoveRegex.Match(text);
            if (m.Success)
            {
                // 手番が指定されていればそれを取得します。
                move.BWType = GetBWType(m.Groups[1].Value);

                // 投了などの指し手の種類を取得します。
                var smoveType = GetSpecialMoveType(m.Groups[2].Value);
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
                        int.Parse(m.Groups[10].Value, CultureInfo.InvariantCulture),
                        int.Parse(m.Groups[11].Value, CultureInfo.InvariantCulture));
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

        private static readonly Regex SpecialMoveRegex;
        private static readonly Regex MoveRegex;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static ShogiParser()
        {
            ModifyAndSetPieceTable();

            SpecialMoveRegex = CreateSpecialMoveRegex();
            MoveRegex = CreateMoveRegex();
        }
    }
}
