using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Ragnarok.Shogi;
using Ragnarok.Utility;

namespace Ragnarok.Shogi.Csa
{
    /// <summary>
    /// CSA形式の駒の移動などを保持します。
    /// </summary>
    public sealed class CsaMove
    {
        /// <summary>
        /// 駒の指定がない場合の駒を表現する文字列です。
        /// </summary>
        private static readonly string NullPieceString = "**";
        /// <summary>
        /// 投了時の文字列です。
        /// </summary>
        private static readonly string ResignedString = "%TORYO";
        /// <summary>
        /// 勝ち宣言の文字列です。
        /// </summary>
        private static readonly string KachiString = "%KACHI";

        private static readonly Dictionary<string, Piece> PieceTable =
            new Dictionary<string, Piece>
            {
                {NullPieceString, Piece.None}, // null pruning など
                {"OU", Piece.Gyoku},
                {"HI", Piece.Hisya},
                {"KA", Piece.Kaku},
                {"KI", Piece.Kin},
                {"GI", Piece.Gin},
                {"KE", Piece.Kei},
                {"KY", Piece.Kyo},
                {"FU", Piece.Hu},
                {"RY", Piece.Ryu},
                {"UM", Piece.Uma},
                {"NG", Piece.NariGin},
                {"NK", Piece.NariKei},
                {"NY", Piece.NariKyo},
                {"TO", Piece.To},
            };

        /// <summary>
        /// 駒の種類を取得または設定します。
        /// </summary>
        public Piece Piece
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の移動後の位置を取得または設定します。
        /// </summary>
        public Position NewPosition
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の移動前の位置を取得または設定します。
        /// </summary>
        public Position OldPosition
        {
            get;
            set;
        }

        /// <summary>
        /// 駒打ちの場合は真を返します。
        /// </summary>
        public bool IsDrop
        {
            get { return (OldPosition == null); }
        }

        /// <summary>
        /// 先後を取得または設定します。
        /// </summary>
        public BWType Side
        {
            get;
            set;
        }

        /// <summary>
        /// 投了かどうかを取得または設定します。
        /// </summary>
        public bool IsResigned
        {
            get;
            set;
        }

        /// <summary>
        /// 勝ち宣言かどうかを取得または設定します。
        /// </summary>
        public bool IsKachi
        {
            get;
            set;
        }

        /// <summary>
        /// 駒のCSA表示文字列を取得します。
        /// </summary>
        private static string GetCsaString(Piece piece)
        {
            if (piece == null)
            {
                return NullPieceString;
            }

            foreach (var pair in PieceTable)
            {
                if (pair.Value == piece)
                {
                    return pair.Key;
                }
            }

            return NullPieceString;
        }

        /// <summary>
        /// CSA形式の指し手に文字列化します。
        /// </summary>
        public string ToCsaString()
        {
            var sb = new StringBuilder();

            if (IsResigned)
            {
                return ResignedString;
            }

            if (IsKachi)
            {
                return KachiString;
            }

            sb.Append(
                Side == BWType.Black ? "+" :
                Side == BWType.White ? "-" :
                "");

            if (OldPosition != null)
            {
                sb.Append(OldPosition.File);
                sb.Append(OldPosition.Rank);
            }
            else
            {
                // 駒打の場合
                sb.Append("00");
            }

            if (NewPosition != null)
            {
                sb.Append(NewPosition.File);
                sb.Append(NewPosition.Rank);
            }
            else
            {
                // ほんとはエラー
                sb.Append("00");
            }

            sb.Append(GetCsaString(Piece));
            return sb.ToString();
        }

        /// <summary>
        /// 人間的にCSA形式より分かりやすい文字列を返します。
        /// </summary>
        public string ToPersonalString()
        {
            var sb = new StringBuilder();
            sb.Append(Stringizer.ToString(Side));

            if (IsResigned)
            {
                return "投了";
            }

            if (IsKachi)
            {
                return "勝ち宣言";
            }

            if (Piece == Piece.None ||
                NewPosition == null)
            {
                sb.Append("○○○");
                return sb.ToString();
            }

            sb.Append(NewPosition.File);
            sb.Append(NewPosition.Rank);
            sb.Append(Stringizer.ToString(Piece));

            if (OldPosition != null)
            {
                sb.Append("(");
                sb.Append(OldPosition.File);
                sb.Append(OldPosition.Rank);
                sb.Append(")");
            }
            else
            {
                sb.Append("打");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            return ToPersonalString();
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result != null)
            {
                return result.Value;
            }

            return Equals((CsaMove)obj);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public bool Equals(CsaMove other)
        {
            return (
                (object)other != null &&
                IsResigned == other.IsResigned &&
                IsKachi == other.IsKachi &&
                Side == other.Side &&
                Util.GenericEquals(Piece, other.Piece) &&
                Util.GenericEquals(NewPosition, other.NewPosition) &&
                Util.GenericEquals(OldPosition, other.OldPosition));
        }

        /// <summary>
        /// ハッシュ値を返します。
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode(); // もはや適当
        }

        private static readonly Regex MoveRegex = new Regex(
            @"^(\+|\-)?(\d)(\d)(\d)(\d)([\w|\*]+)",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// CSA形式の指し手を解析します。
        /// </summary>
        public static CsaMove Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            if (str.StartsWith(ResignedString))
            {
                return new CsaMove
                {
                    IsResigned = true,
                };
            }

            if (str.StartsWith(KachiString))
            {
                return new CsaMove
                {
                    IsKachi = true,
                };
            }

            var m = MoveRegex.Match(str);
            if (!m.Success)
            {
                return null;
            }

            var c = m.Groups[1].Value;
            var side = (
                c == "+" ? BWType.Black :
                c == "-" ? BWType.White :
                BWType.None);

            // 移動前の位置
            var oldFile = int.Parse(m.Groups[2].Value);
            var oldRank = int.Parse(m.Groups[3].Value);
            var oldPosition =
                (oldFile == 0 || oldRank == 0
                ? (Position)null
                : new Position(oldFile, oldRank));

            // 移動後の位置
            var newFile = int.Parse(m.Groups[4].Value);
            var newRank = int.Parse(m.Groups[5].Value);
            var newPosition =
                (newFile == 0 || newRank == 0
                ? (Position)null
                : new Position(newFile, newRank));

            // 駒
            Piece piece;
            if (!PieceTable.TryGetValue(m.Groups[6].Value, out piece))
            {
                return null;
            }

            return new CsaMove
            {
                NewPosition = newPosition,
                OldPosition = oldPosition,
                Piece = piece,
                Side = side,
                IsResigned = false,
            };
        }
    }
}
