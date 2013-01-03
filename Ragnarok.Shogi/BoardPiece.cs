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
    [DataContract()]
    public class BoardPiece : IModel, IEquatable<BoardPiece>
    {
        private BWType bwType = BWType.None;
        private bool isPromoted = false;
        private PieceType pieceType = PieceType.None;

        /// <summary>
        /// プロパティ値の変更を通知します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティ値の変更を通知します。
        /// </summary>
        public void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            Util.CallPropertyChanged(
                PropertyChanged, this, e);
        }

        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        /// <remarks>
        /// 盤の設定は行いません。
        /// </remarks>
        public BoardPiece Clone()
        {
            return new BoardPiece(BWType, PieceType, IsPromoted);
        }

        /// <summary>
        /// 先手の駒か後手の駒かを取得または設定します。
        /// </summary>
        public BWType BWType
        {
            get
            {
                return this.bwType;
            }
            set
            {
                if (this.bwType != value)
                {
                    this.bwType = value;

                    this.RaisePropertyChanged("BWType");
                }
            }
        }

        /// <summary>
        /// 駒の種類を取得または設定します。
        /// </summary>
        public PieceType PieceType
        {
            get
            {
                return this.pieceType;
            }
            set
            {
                if (this.pieceType != value)
                {
                    this.pieceType = value;

                    this.RaisePropertyChanged("PieceType");
                }
            }
        }

        /// <summary>
        /// 駒が成ってるかどうかを取得または設定します。
        /// </summary>
        public bool IsPromoted
        {
            get
            {
                return this.isPromoted;
            }
            set
            {
                if (this.isPromoted != value)
                {
                    this.isPromoted = value;

                    this.RaisePropertyChanged("IsPromoted");
                }
            }
        }

        /// <summary>
        /// 駒を文字列化します。
        /// </summary>
        public override string ToString()
        {
            if (IsPromoted)
            {
                switch (PieceType)
                {
                    case PieceType.Gyoku:
                        return "玉";
                    case PieceType.Hisya:
                        return "龍";
                    case PieceType.Kaku:
                        return "馬";
                    case PieceType.Kin:
                        return "金";
                    case PieceType.Gin:
                        return "全";
                    case PieceType.Kei:
                        return "圭";
                    case PieceType.Kyo:
                        return "杏";
                    case PieceType.Hu:
                        return "と";
                }
            }
            else
            {
                switch (PieceType)
                {
                    case PieceType.Gyoku:
                        return "玉";
                    case PieceType.Hisya:
                        return "飛";
                    case PieceType.Kaku:
                        return "角";
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

            return "不明";
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (!EnumEx.IsDefined(PieceType))
            {
                return false;
            }

            if (!EnumEx.IsDefined(BWType))
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
        public BoardPiece(BWType bwType, PieceType pieceType, bool isPromoted)
        {
            this.bwType = bwType;
            this.pieceType = pieceType;
            this.isPromoted = isPromoted;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardPiece(BWType bwType, PieceType pieceType)
            : this(bwType, pieceType, false)
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
