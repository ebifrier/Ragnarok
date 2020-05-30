using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

using Ragnarok;
using Ragnarok.ObjectModel;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 盤面上の駒を示します。
    /// </summary>
    /// <remarks>
    /// 盤面上の駒を表現するには駒の情報＋先後が必要です。
    /// このクラスは<see cref="Piece"/>クラスに先後のプロパティを
    /// 追加しただけのものです。
    /// </remarks>
    [DataContract()]
    public class BoardPiece : IEquatable<BoardPiece>
    {
        /// <summary>
        /// 先手の駒か後手の駒かを取得または設定します。
        /// </summary>
        public BWType BWType
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の種類を取得または設定します。
        /// </summary>
        public PieceType PieceType
        {
            get;
            set;
        }

        /// <summary>
        /// 駒が成ってるかどうかを取得または設定します。
        /// </summary>
        public bool IsPromoted
        {
            get;
            set;
        }

        /// <summary>
        /// 符号なしの駒オブジェクトを取得します。
        /// </summary>
        public Piece Piece
        {
            get { return new Piece(PieceType, IsPromoted); }
        }

        /// <summary>
        /// 符号なしの駒オブジェクトを取得します。(null対応版)
        /// </summary>
        public static Piece GetPiece(BoardPiece self)
        {
            return (self != null ?
                new Piece(self.PieceType, self.IsPromoted) :
                Piece.None);
        }

        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public BoardPiece Clone()
        {
            return new BoardPiece(PieceType, IsPromoted, BWType);
        }

        /// <summary>
        /// 駒を文字列化します。
        /// </summary>
        public override string ToString()
        {
            return (Stringizer.ToString(BWType) + Stringizer.ToString(Piece));
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (!EnumUtil.IsDefined(BWType))
            {
                return false;
            }

            if (!Piece.Validate())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトの等値性を判定します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(obj as BoardPiece);
        }

        /// <summary>
        /// オブジェクトの等値性を判定します。
        /// </summary>
        public bool Equals(BoardPiece other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (BWType != other.BWType)
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
        /// ハッシュ値を返します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                BWType.GetHashCode() ^
                PieceType.GetHashCode() ^
                IsPromoted.GetHashCode());
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator ==(BoardPiece x, BoardPiece y)
        {
            return Util.GenericEquals(x, y);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(BoardPiece x, BoardPiece y)
        {
            return !(x == y);
        }

        #region シリアライズ
        [DataMember(Order = 1, IsRequired = true)]
        private byte serializeBits = 0;

        /// <summary>
        /// シリアライズを行います。
        /// </summary>
        [CLSCompliant(false)]
        public byte Serialize()
        {
            byte bits = 0;

            // 2bits
            bits |= (byte)BWType;
            // 4bits
            bits |= (byte)((uint)PieceType << 2);
            // 1bits
            bits |= (byte)((uint)(IsPromoted ? 1 : 0) << 6);

            return bits;
        }

        /// <summary>
        /// デシリアライズを行います。
        /// </summary>
        [CLSCompliant(false)]
        public void Deserialize(uint bits)
        {
            BWType = (BWType)((bits >> 0) & 0x03);
            PieceType = (PieceType)((bits >> 2) & 0x0f);
            IsPromoted = (((bits >> 6) & 0x01) != 0);
        }

        /// <summary>
        /// シリアライズ前に呼ばれます。
        /// </summary>
        [OnSerializing()]
        private void BeforeSerialize(StreamingContext context)
        {
            this.serializeBits = Serialize();
        }

        /// <summary>
        /// デシリアライズ後に呼ばれます。
        /// </summary>
        [OnDeserialized()]
        private void AfterDeserialize(StreamingContext context)
        {
            Deserialize(this.serializeBits);
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardPiece(PieceType pieceType, bool isPromoted, BWType bwType)
        {
            BWType = bwType;
            PieceType = pieceType;
            IsPromoted = isPromoted;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardPiece(PieceType pieceType, BWType bwType)
            : this(pieceType, false, bwType)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardPiece(Piece piece, BWType bwType)
            : this(piece.PieceType, piece.IsPromoted, bwType)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardPiece()
        {
        }
    }
}
