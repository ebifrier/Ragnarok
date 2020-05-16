using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 指し手のスタイルです。
    /// </summary>
    public enum MoveTextStyle
    {
        /// <summary>
        /// ▲３二金 などの通常の表示形式です。
        /// </summary>
        Normal,
        /// <summary>
        /// ３二金 など手番がない通常の表示形式です。
        /// </summary>
        NoTeban,
        /// <summary>
        /// ３二金(22) など移動前の符号が付く.kifファイル用の表示形式です。
        /// </summary>
        KifFile,
        /// <summary>
        /// 45桂 などよりシンプルな表示形式です。
        /// </summary>
        Simple,
    }

    /// <summary>
    /// 指し手などの文字列化を行います。
    /// </summary>
    public static class Stringizer
    {
        private static readonly Dictionary<BWType, string> BWTypeTable =
            new Dictionary<BWType, string>
            {
                {BWType.None, ""},
                {BWType.Black, "▲"},
                {BWType.White, "△"},
            };

        private static readonly Dictionary<RelFileType, string> RelPosTypeTable =
            new Dictionary<RelFileType, string>
            {
                {RelFileType.None, ""},
                {RelFileType.Left, "左"},
                {RelFileType.Right, "右"},
                {RelFileType.Straight, "直"},
            };

        private static readonly Dictionary<RankMoveType, string> RankMoveTypeTable =
            new Dictionary<RankMoveType, string>
            {
                {RankMoveType.None, ""},
                {RankMoveType.Up, "上"},
                {RankMoveType.Back, "引"},
                {RankMoveType.Sideways, "寄"},
            };

        private static readonly Dictionary<ActionType, string> ActionTypeTable =
            new Dictionary<ActionType, string>
            {
                {ActionType.None, ""},
                {ActionType.Promote, "成"},
                {ActionType.Unpromote, "不成"},
                {ActionType.Drop, "打"},
            };

        /// <summary>
        /// 手番を△や▲に変換します。
        /// </summary>
        public static string ToString(BWType bwType)
        {
            return BWTypeTable[bwType];
        }

        /// <summary>
        /// 駒の相対位置を文字列に変換します。
        /// </summary>
        public static string ToString(RelFileType relFileType)
        {
            return RelPosTypeTable[relFileType];
        }

        /// <summary>
        /// 駒の移動種類を文字列に変換します。
        /// </summary>
        public static string ToString(RankMoveType rankMoveType)
        {
            return RankMoveTypeTable[rankMoveType];
        }

        /// <summary>
        /// 駒打ちなどのアクションを文字列に変換します。
        /// </summary>
        public static string ToString(ActionType actionType)
        {
            return ActionTypeTable[actionType];
        }

        /// <summary>
        /// 駒の種類を文字列で取得します。
        /// </summary>
        public static string ToString(PieceType pieceType)
        {
            return ToString(new Piece(pieceType, false));
        }

        /// <summary>
        /// 駒の種類を文字列で取得します。
        /// </summary>
        public static string ToString(Piece piece)
        {
            if (piece == null)
            {
                throw new ArgumentNullException(nameof(piece));
            }

            if (!piece.IsPromoted)
            {
                switch (piece.PieceType)
                {
                    case PieceType.None:
                        return "○";
                    case PieceType.Hisya:
                        return "飛";
                    case PieceType.Kaku:
                        return "角";
                    case PieceType.Gyoku:
                        return "玉";
                    case PieceType.Kin:
                        return "金";
                    case PieceType.Gin:
                        return "銀";
                    case PieceType.Kei:
                        return "桂";
                    case PieceType.Kyo:
                        return "香";
                    case PieceType.Hu:
                        return "歩";
                }
            }
            else
            {
                switch (piece.PieceType)
                {
                    case PieceType.None:
                        return "無";
                    case PieceType.Hisya:
                        return "龍";
                    case PieceType.Kaku:
                        return "馬";
                    case PieceType.Gyoku:
                        return "玉";
                    case PieceType.Kin:
                        return "金";
                    case PieceType.Gin:
                        return "成銀";
                    case PieceType.Kei:
                        return "成桂";
                    case PieceType.Kyo:
                        return "成香";
                    case PieceType.Hu:
                        return "と";
                }
            }

            return "不明";
        }

        /// <summary>
        /// 指し手を文字列に変換します。
        /// </summary>
        /// <remarks>
        /// KifFileに使う指し手は同○○の場合、
        /// 「同」と「○○」の間に空白を入れないと"kif for windows"では
        /// 正しく読み込めなくなります。
        /// </remarks>
        public static string ToString(LiteralMove move,
                                      MoveTextStyle style = MoveTextStyle.Normal)
        {
            if (move == null)
            {
                return null;
            }

            if (move.IsSpecialMove)
            {
                var turnStr = string.Empty;

                // 必要なら▲△を先頭に入れます。
                if (style == MoveTextStyle.Normal)
                {
                    turnStr = ToString(move.BWType);
                }

                return (turnStr + EnumEx.GetLabel(move.SpecialMoveType));
            }

            var result = new StringBuilder();

            result.Append(ToString(move.Piece));
            result.Append(ToString(move.RelFileType));
            result.Append(ToString(move.RankMoveType));
            result.Append(ToString(move.ActionType));

            if (move.SameAsOld)
            {
                var hasSpace = (
                    (style != MoveTextStyle.Simple) &&
                    (result.Length == 1 || style == MoveTextStyle.KifFile));

                // 文字数によって、同の後の空白を入れるか決めます。
                result.Insert(0, (hasSpace ? "同　" : "同"));
            }
            else
            {
                if (style == MoveTextStyle.Simple)
                {
                    result.Insert(0,
                        IntConverter.Convert(NumberType.Normal, move.File));
                    result.Insert(1,
                        IntConverter.Convert(NumberType.Normal, move.Rank));
                }
                else
                {
                    result.Insert(0,
                        IntConverter.Convert(NumberType.Big, move.File));
                    result.Insert(1,
                        IntConverter.Convert(NumberType.Kanji, move.Rank));
                }
            }

            // 必要なら▲△を先頭に入れます。
            if (style == MoveTextStyle.Normal)
            {
                result.Insert(0, ToString(move.BWType));
            }

            if (move.SrcSquare != null && style == MoveTextStyle.KifFile)
            {
                result.AppendFormat("({0}{1})",
                    move.SrcSquare.File,
                    move.SrcSquare.Rank);
            }

            return result.ToString();
        }

        /// <summary>
        /// 棋力を文字列化します。
        /// </summary>
        public static string ToString(SkillLevel skillLevel)
        {
            if (skillLevel == null)
            {
                throw new ArgumentNullException(nameof(skillLevel));
            }

            if (!string.IsNullOrEmpty(skillLevel.OriginalText))
            {
                var text = skillLevel.OriginalText;

                return (text.Length > 12 ?
                    text.Substring(0, 12) :
                    text);
            }

            var n = IntConverter.Convert(NumberType.Grade, skillLevel.Grade);
            switch (skillLevel.Kind)
            {
                case SkillKind.Kyu:
                    return (n + "級");
                case SkillKind.Dan:
                    return (n + "段");
                case SkillKind.Unknown:
                    return "";
            }

            return "";
        }

        /// <summary>
        /// 参加者を文字列化します。
        /// </summary>
        public static string ToString(ShogiPlayer player)
        {
            if (player == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(player.Nickname))
            {
                return ToString(player.SkillLevel);
            }
            else
            {
                var skillName = ToString(player.SkillLevel);

                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{1}",
                    player.Nickname,
                    (string.IsNullOrEmpty(skillName) ? "" : "(" + skillName + ")"));
            }
        }
    }
}
