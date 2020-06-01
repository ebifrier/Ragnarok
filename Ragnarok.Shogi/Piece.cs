using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 駒の種類です。
    /// </summary>
    [DataContract()]
    public enum Piece
    {
        /// <summary>
        /// 特になし。
        /// </summary>
        [EnumMember()]
        None = 0,
        /// <summary>
        /// 歩
        /// </summary>
        [EnumMember()]
        Pawn = 1,
        /// <summary>
        /// 香車
        /// </summary>
        [EnumMember()]
        Lance = 2,
        /// <summary>
        /// 桂馬
        /// </summary>
        [EnumMember()]
        Knight = 3,
        /// <summary>
        /// 銀
        /// </summary>
        [EnumMember()]
        Silver = 4,
        /// <summary>
        /// 角
        /// </summary>
        [EnumMember()]
        Bishop = 5,
        /// <summary>
        /// 飛車
        /// </summary>
        [EnumMember()]
        Rook = 6,
        /// <summary>
        /// 金
        /// </summary>
        [EnumMember()]
        Gold = 7,
        /// <summary>
        /// 玉
        /// </summary>
        [EnumMember()]
        King = 8,
        /// <summary>
        /// と金
        /// </summary>
        [EnumMember()]
        ProPawn = Pawn | Promote,
        /// <summary>
        /// 成香
        /// </summary>
        [EnumMember()]
        ProLance = Lance | Promote,
        /// <summary>
        /// 成桂
        /// </summary>
        [EnumMember()]
        ProKnight = Knight | Promote,
        /// <summary>
        /// 成銀
        /// </summary>
        [EnumMember()]
        ProSilver = Silver | Promote,
        /// <summary>
        /// 馬
        /// </summary>
        [EnumMember()]
        Horse = Bishop | Promote,
        /// <summary>
        /// 竜
        /// </summary>
        [EnumMember()]
        Dragon = Rook | Promote,
        /// <summary>
        /// 未使用
        /// </summary>
        [EnumMember()]
        Queen = Gold | Promote,

        [EnumMember()]
        BlackPawn = Pawn,
        [EnumMember()]
        BlackLance = Lance,
        [EnumMember()]
        BlackKnight = Knight,
        [EnumMember()]
        BlackSilver = Silver,
        [EnumMember()]
        BlackBishop = Bishop,
        [EnumMember()]
        BlackRook = Rook,
        [EnumMember()]
        BlackGold = Gold,
        [EnumMember()]
        BlackKing = King,
        [EnumMember()]
        BlackProPawn = ProPawn,
        [EnumMember()]
        BlackProLance = ProLance,
        [EnumMember()]
        BlackProKnight = ProKnight,
        [EnumMember()]
        BlackProSilver = ProSilver,
        [EnumMember()]
        BlackHorse = Horse,
        [EnumMember()]
        BlackDragon = Dragon,
        [EnumMember()]
        BlackQueen = Queen,

        [EnumMember()]
        WhitePawn = Pawn | White,
        [EnumMember()]
        WhiteLance = Lance | White,
        [EnumMember()]
        WhiteKnight = Knight | White,
        [EnumMember()]
        WhiteSilver = Silver | White,
        [EnumMember()]
        WhiteBishop = Bishop | White,
        [EnumMember()]
        WhiteRook = Rook | White,
        [EnumMember()]
        WhiteGold = Gold | White,
        [EnumMember()]
        WhiteKing = King | White,
        [EnumMember()]
        WhiteProPawn = ProPawn | White,
        [EnumMember()]
        WhiteProLance = ProLance | White,
        [EnumMember()]
        WhiteProKnight = ProKnight | White,
        [EnumMember()]
        WhiteProSilver = ProSilver | White,
        [EnumMember()]
        WhiteHorse = Horse | White,
        [EnumMember()]
        WhiteDragon = Dragon | White,
        [EnumMember()]
        WhiteQueen = Queen | White,

        /// <summary>
        /// 成フラグ
        /// </summary>
        Promote = 8,
        /// <summary>
        /// 白番フラグ
        /// </summary>
        White = 16,
    }

    /// <summary>
    /// 成り・不成も含めた駒の種類です。
    /// </summary>
    public static class PieceUtil
    {
        /// <summary>
        /// 成りを含まない駒をすべて列挙します。
        /// </summary>
        public static IEnumerable<Piece> RawTypes(Colour colour = Colour.None)
        {
            for (var piece = Piece.Pawn; piece <= Piece.King; ++piece)
            {
                yield return (colour != Colour.None
                    ? piece.Modify(colour)
                    : piece);
            }
        }

        /// <summary>
        /// 成りを含む駒をすべて列挙します。
        /// </summary>
        public static IEnumerable<Piece> PieceTypes(Colour colour = Colour.None)
        {
            for (var piece = Piece.Pawn; piece < Piece.Queen; ++piece)
            {
                yield return (colour != Colour.None
                    ? piece.Modify(colour)
                    : piece);
            }
        }

        /// <summary>
        /// 黒番・白番の駒をすべて列挙します。
        /// </summary>
        public static IEnumerable<Piece> BlackWhitePieces()
        {
            foreach (var piece in PieceTypes())
            {
                yield return piece;
            }

            foreach (var piece in PieceTypes())
            {
                yield return Modify(piece, Colour.White);
            }
        }

        /// <summary>
        /// 駒の成り・不成を設定します。
        /// </summary>
        public static Piece Modify(this Piece piece, bool isPromote)
        {
            return (isPromote ? piece.Promote() : piece.Unpromote());
        }

        /// <summary>
        /// 駒の手番を設定します。
        /// </summary>
        public static Piece Modify(this Piece piece, Colour colour)
        {
            return (
                piece == Piece.None ? piece :
                colour == Colour.Black ? piece & ~Piece.White :
                colour == Colour.White ? piece | Piece.White :
                piece);
        }

        /// <summary>
        /// 手番情報を取得します。
        /// </summary>
        public static Colour GetColour(this Piece piece)
        {
            return ((piece & Piece.White) != 0 ? Colour.White : Colour.Black);
        }

        /// <summary>
        /// 駒の手番を反転させます。
        /// </summary>
        public static Piece FlipColor(this Piece piece)
        {
            var colour = GetColour(piece);

            return Modify(piece, colour.Flip());
        }

        /// <summary>
        /// 手番情報を削除した駒情報を取得します。
        /// </summary>
        public static Piece GetPieceType(this Piece piece)
        {
            return (Piece)((int)piece & 15);
        }

        /// <summary>
        /// 手番情報と成り情報を削除した駒情報を取得します。
        /// </summary>
        /// <remarks>
        /// 玉の場合は正しい駒を返しません。
        /// </remarks>
        public static Piece GetRawType(this Piece piece)
        {
            return piece.GetPieceType() == Piece.King
                ? Piece.King
                : (Piece)((int)piece & 7);
        }

        /// <summary>
        /// 成り駒かどうかを取得します。
        /// </summary>
        public static bool IsPromoted(this Piece piece)
        {
            return (
                piece.GetRawType() != Piece.King &&
                (piece & Piece.Promote) != 0);
        }

        /// <summary>
        /// 駒を成ります。
        /// </summary>
        public static Piece Promote(this Piece piece)
        {
            if (piece.GetRawType() == Piece.King ||
                piece.GetRawType() == Piece.Gold)
            {
                return piece;
            }

            return (piece | Piece.Promote);
        }

        /// <summary>
        /// 駒を成らずとします。
        /// </summary>
        public static Piece Unpromote(this Piece piece)
        {
            if (piece.GetRawType() == Piece.King)
            {
                return piece;
            }

            return (piece & ~Piece.Promote);
        }

        /// <summary>
        /// 駒がPiece.Noneかどうか確認します。
        /// </summary>
        public static bool IsNone(this Piece piece)
        {
            return (piece.GetRawType() == Piece.None);
        }

        /// <summary>
        /// 駒が正しいか確認します。
        /// </summary>
        public static bool Validate(this Piece piece)
        {
            return (piece != Piece.None);
        }

        /// <summary>
        /// 駒の種類を文字列で取得します。
        /// </summary>
        public static string ToString(this Piece piece)
        {
            return Stringizer.ToString(piece);
        }
    }
}
