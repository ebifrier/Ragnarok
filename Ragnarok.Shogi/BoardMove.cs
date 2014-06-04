using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 盤面上での差し手を示します。
    /// </summary>
    /// <remarks>
    /// <para>
    /// シリアライズについて
    /// 
    /// このクラスは<see cref="Board"/>クラスと一緒にシリアライズされますが、
    /// 200手,300手の局面になってくると、シリアライズ後のデータ量が
    /// 3kbや4kbになってしまいます。
    /// そのためこのクラスでは特別に、シリアライズ時にデータの
    /// 圧縮を行っています。
    /// </para>
    /// </remarks>
    [DataContract()]
    public class BoardMove : IEquatable<BoardMove>
    {
        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public BoardMove Clone()
        {
            return (BoardMove)MemberwiseClone();
        }

        /// <summary>
        /// 先手の手か後手の手かを取得または設定します。
        /// </summary>
        public BWType BWType
        {
            get;
            set;
        }
        
        /// <summary>
        /// 駒の移動先を取得または設定します。
        /// </summary>
        public Square DstSquare
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の移動元を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 駒打ちの場合はnullになります。
        /// </remarks>
        public Square SrcSquare
        {
            get;
            set;
        }

        /// <summary>
        /// 駒打ち・成りなどの動作を取得または設定します。
        /// </summary>
        public ActionType ActionType
        {
            get;
            set;
        }

        /// <summary>
        /// 駒を打つ場合の駒を取得または設定します。
        /// </summary>
        public PieceType DropPieceType
        {
            get;
            set;
        }

        /// <summary>
        /// 取った駒を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 戻る操作のために必要です。
        /// </remarks>
        [DataMember(Order = 2, IsRequired = true)]
        public Piece TookPiece
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (BWType == BWType.None)
            {
                return false;
            }

            if (DstSquare == null || !DstSquare.Validate())
            {
                return false;
            }

            if (ActionType == ActionType.Drop)
            {
                // 駒打ちの場合
                if (SrcSquare != null)
                {
                    return false;
                }

                if (DropPieceType == PieceType.None)
                {
                    return false;
                }
            }
            else
            {
                // 駒打ちでない場合
                if (SrcSquare == null || !SrcSquare.Validate())
                {
                    return false;
                }

                if (DropPieceType != PieceType.None)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// オブジェクトの等値性を調べます。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(obj as BoardMove);
        }

        /// <summary>
        /// オブジェクトの等値性を調べます。
        /// </summary>
        public bool Equals(BoardMove other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (BWType != other.BWType)
            {
                return false;
            }

            if (DstSquare != other.DstSquare)
            {
                return false;
            }

            if (SrcSquare != other.SrcSquare)
            {
                return false;
            }

            if (ActionType != other.ActionType)
            {
                return false;
            }

            if (DropPieceType != other.DropPieceType)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator ==(BoardMove x, BoardMove y)
        {
            return Ragnarok.Util.GenericEquals(x, y);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(BoardMove x, BoardMove y)
        {
            return !(x == y);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                BWType.GetHashCode() ^
                (DstSquare != null ? DstSquare.GetHashCode() : 0) ^
                (SrcSquare != null ? SrcSquare.GetHashCode() : 0) ^
                ActionType.GetHashCode() ^
                DropPieceType.GetHashCode());
        }

        #region シリアライズ/デシリアライズ
        [DataMember(Order = 1, IsRequired = true)]
        private uint serializeBits = 0;

        /// <summary>
        /// Squareをシリアライズします。
        /// </summary>
        /// <remarks>
        /// 1*10+1 ～ 9*10+9の値にシリアライズされます。
        /// </remarks>
        private static byte SerializeSquare(Square square)
        {
            if (square == null)
            {
                return 0;
            }

            // 1*10+1 ～ 9*10+9
            return (byte)(
                square.File * (Board.BoardSize + 1) +
                square.Rank);
        }

        /// <summary>
        /// Squareをデシリアライズします。
        /// </summary>
        private static Square DeserializeSquare(uint bits)
        {
            if (bits == 0)
            {
                return null;
            }

            var file = (int)bits / (Board.BoardSize + 1);
            var rank = (int)bits % (Board.BoardSize + 1);
            return new Square(file, rank);
        }

        /// <summary>
        /// シリアライズを行います。
        /// </summary>
        [CLSCompliant(false)]
        public uint Serialize()
        {
            uint bits = 0;

            // 2bit
            bits |= (uint)BWType;
            // 3bit
            bits |= ((uint)ActionType << 2);
            // 4bit
            bits |= ((uint)DropPieceType << 5);
            // 1bit
            bits |= ((uint)(TookPiece != null ? 1 : 0) << 9);
            // 7bit
            bits |= (uint)(SerializeSquare(DstSquare) << 10);
            // 7bit
            bits |= (uint)(SerializeSquare(SrcSquare) << 17);

            if (TookPiece != null)
            {
                bits |= ((uint)TookPiece.Serialize() << 24);
            }

            return bits;
        }

        /// <summary>
        /// デシリアライズを行います。
        /// </summary>
        [CLSCompliant(false)]
        public void Deserialize(uint bits)
        {
            // 2bit
            BWType = (BWType)((bits >> 0) & 0x03);
            // 3bit
            ActionType = (ActionType)((bits >> 2) & 0x07);
            // 4bit
            DropPieceType = (PieceType)((bits >> 5) & 0x0f);
            // 1bit
            var hasTookPiece = (((bits >> 9) & 0x01) != 0);
            // 7bit
            DstSquare = DeserializeSquare((bits >> 10) & 0x7f);
            // 7bit
            SrcSquare = DeserializeSquare((bits >> 17) & 0x7f);

            if (hasTookPiece)
            {
                TookPiece = new Piece();

                TookPiece.Deserialize((bits >> 24) & 0xff);
            }
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
        public BoardMove()
        {
            BWType = BWType.None;
            ActionType = ActionType.None;
            DropPieceType = PieceType.None;
        }
    }
}
