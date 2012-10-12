using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 駒の種類です。
    /// </summary>
    [DataContract()]
    public enum PieceType
    {
        /// <summary>
        /// 特になし。
        /// </summary>
        [EnumMember()]
        None = 0,
        /// <summary>
        /// 玉
        /// </summary>
        [EnumMember()]
        Gyoku = 1,
        /// <summary>
        /// 飛車
        /// </summary>
        [EnumMember()]
        Hisya = 2,
        /// <summary>
        /// 角
        /// </summary>
        [EnumMember()]
        Kaku = 3,
        /// <summary>
        /// 金
        /// </summary>
        [EnumMember()]
        Kin = 4,
        /// <summary>
        /// 銀
        /// </summary>
        [EnumMember()]
        Gin = 5,
        /// <summary>
        /// 桂馬
        /// </summary>
        [EnumMember()]
        Kei = 6,
        /// <summary>
        /// 香車
        /// </summary>
        [EnumMember()]
        Kyo = 7,
        /// <summary>
        /// 歩
        /// </summary>
        [EnumMember()]
        Hu = 8,
    }

    /// <summary>
    /// 成り・不成も含めた駒の種類です。
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class Piece : IEquatable<Piece>
    {
        /// <summary>
        /// 無し
        /// </summary>
        public static Piece None = new Piece(PieceType.None, false);
        /// <summary>
        /// 飛車
        /// </summary>
        public static Piece Hisya = new Piece(PieceType.Hisya, false);
        /// <summary>
        /// 角
        /// </summary>
        public static Piece Kaku = new Piece(PieceType.Kaku, false);
        /// <summary>
        /// 玉
        /// </summary>
        public static Piece Gyoku = new Piece(PieceType.Gyoku, false);
        /// <summary>
        /// 金
        /// </summary>
        public static Piece Kin = new Piece(PieceType.Kin, false);
        /// <summary>
        /// 銀
        /// </summary>
        public static Piece Gin = new Piece(PieceType.Gin, false);
        /// <summary>
        /// 桂馬
        /// </summary>
        public static Piece Kei = new Piece(PieceType.Kei, false);
        /// <summary>
        /// 香車
        /// </summary>
        public static Piece Kyo = new Piece(PieceType.Kyo, false);
        /// <summary>
        /// 歩
        /// </summary>
        public static Piece Hu = new Piece(PieceType.Hu, false);
        /// <summary>
        /// 龍
        /// </summary>
        public static Piece Ryu = new Piece(PieceType.Hisya, true);
        /// <summary>
        /// 馬
        /// </summary>
        public static Piece Uma = new Piece(PieceType.Kaku, true);
        /// <summary>
        /// 成銀
        /// </summary>
        public static Piece NariGin = new Piece(PieceType.Gin, true);
        /// <summary>
        /// 成桂
        /// </summary>
        public static Piece NariKei = new Piece(PieceType.Kei, true);
        /// <summary>
        /// 成香
        /// </summary>
        public static Piece NariKyo = new Piece(PieceType.Kyo, true);
        /// <summary>
        /// と金
        /// </summary>
        public static Piece To = new Piece(PieceType.Hu, true);

        /// <summary>
        /// 基本となる駒の種類を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public PieceType PieceType
        {
            get;
            set;
        }

        /// <summary>
        /// 成り駒かどうかを取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public bool IsPromoted
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトのクローンを取得します。
        /// </summary>
        public Piece Clone()
        {
            return new Piece(PieceType, IsPromoted);
        }

        /// <summary>
        /// オブジェクトの各プロパティが正しく設定されているか調べます。
        /// </summary>
        public bool Validate()
        {
            switch (PieceType)
            {
                case PieceType.Hisya:
                case PieceType.Kaku:
                case PieceType.Gyoku:
                case PieceType.Kin:
                case PieceType.Gin:
                case PieceType.Kei:
                case PieceType.Kyo:
                case PieceType.Hu:
                    break;
                default:
                    // Noneもここになります。
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 駒の種類を文字列で取得します。
        /// </summary>
        public override string ToString()
        {
            return Stringizer.ToString(this);
        }

        /// <summary>
        /// オブジェクトの比較を行います。
        /// </summary>
        public override bool Equals(object other)
        {
            return Equals(other as Piece);
        }

        /// <summary>
        /// オブジェクトの比較を行います。
        /// </summary>
        public bool Equals(Piece other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (PieceType != other.PieceType)
            {
                return false;
            }

            if (IsPromoted != other.IsPromoted)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                PieceType.GetHashCode() ^
                IsPromoted.GetHashCode());
        }

        /// <summary>
        /// == 演算子を実装します。
        /// </summary>
        public static bool operator ==(Piece lhs, Piece rhs)
        {
            return Ragnarok.Util.GenericClassEquals(lhs, rhs);
        }

        /// <summary>
        /// != 演算子を実装します。
        /// </summary>
        public static bool operator !=(Piece lhs, Piece rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Piece(PieceType piece, bool promoted)
        {
            PieceType = piece;
            IsPromoted = promoted;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        [Obsolete("protobuf用のコンストラクタです。")]
        protected Piece()
        {
        }
    }
}
