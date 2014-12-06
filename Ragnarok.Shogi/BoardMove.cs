using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

using Ragnarok.Utility;

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
        /// 特殊な指し手の場合はその種類を取得または設定します。
        /// </summary>
        public SpecialMoveType SpecialMoveType
        {
            get;
            private set;
        }

        /// <summary>
        /// 特殊な指し手であるかどうかを取得します。
        /// </summary>
        public bool IsSpecialMove
        {
            get { return (SpecialMoveType != SpecialMoveType.None); }
        }

        /// <summary>
        /// 駒を移動する場合の対象となる駒を取得または設定します。
        /// </summary>
        public Piece MovePiece
        {
            get;
            private set;
        }

        /// <summary>
        /// 先手の手か後手の手かを取得または設定します。
        /// </summary>
        public BWType BWType
        {
            get;
            private set;
        }
        
        /// <summary>
        /// 駒の移動先を取得または設定します。
        /// </summary>
        public Square DstSquare
        {
            get;
            private set;
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
            private set;
        }

        /// <summary>
        /// 駒を成るかどうかを取得または設定します。
        /// </summary>
        public bool IsPromote
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
            private set;
        }

        /// <summary>
        /// 駒打ち・成りなどの動作を取得します。
        /// </summary>
        public ActionType ActionType
        {
            get
            {
                if (IsSpecialMove)
                {
                    return ActionType.None;
                }
                else if (DropPieceType != PieceType.None)
                {
                    return ActionType.Drop;
                }
                else if (SrcSquare != null && MovePiece != null)
                {
                    return (IsPromote ? ActionType.Promote : ActionType.None);
                }

                throw new ShogiException(
                    "指し手が正しくありません。");
            }
        }

        /// <summary>
        /// 取った駒を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 戻る操作のために必要です。
        /// </remarks>
        public Piece TookPiece
        {
            get;
            set;
        }

        /// <summary>
        /// 一手前の指し手と着手位置が同じかどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 「同飛」などと出力するためのもので、それ以上の意味はありません。
        /// </remarks>
        public bool HasSameSquareAsPrev
        {
            get;
            set;
        }

        /// <summary>
        /// 指し手を文字列化します。
        /// </summary>
        public override string ToString()
        {
            if (SpecialMoveType != SpecialMoveType.None)
            {
                return string.Format(
                    "{0}{1}",
                    Stringizer.ToString(BWType),
                    EnumEx.GetLabel(SpecialMoveType));
            }
            else if (ActionType == ActionType.Drop)
            {
                return string.Format(
                    "{0}{1}{2}{3}打",
                    Stringizer.ToString(BWType),
                    StringConverter.ConvertInt(NumberType.Big, DstSquare.File),
                    StringConverter.ConvertInt(NumberType.Kanji, DstSquare.Rank),
                    Stringizer.ToString(DropPieceType));
            }
            else if (HasSameSquareAsPrev)
            {
                return string.Format(
                    "{0}同　{1}{2}({3}{4})",
                    Stringizer.ToString(BWType),
                    MovePiece,
                    Stringizer.ToString(ActionType),
                    SrcSquare.File,
                    SrcSquare.Rank);
            }
            else
            {
                return string.Format(
                    "{0}{1}{2}{3}{4}({5}{6})",
                    Stringizer.ToString(BWType),
                    StringConverter.ConvertInt(NumberType.Big, DstSquare.File),
                    StringConverter.ConvertInt(NumberType.Kanji, DstSquare.Rank),
                    MovePiece,
                    Stringizer.ToString(ActionType),
                    SrcSquare.File,
                    SrcSquare.Rank);
            }
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

            if (SpecialMoveType != SpecialMoveType.None)
            {
                // 特に確認は行いません。
            }
            else if (ActionType == ActionType.Drop)
            {
                // 駒打ちの場合
                if (DstSquare == null || !DstSquare.Validate())
                {
                    return false;
                }
                
                if (SrcSquare != null)
                {
                    return false;
                }

                if (MovePiece != null)
                {
                    return false;
                }

                if (IsPromote)
                {
                    return false;
                }
            }
            else
            {
                // 駒打ちでない場合
                if (DstSquare == null || !DstSquare.Validate())
                {
                    return false;
                }

                if (SrcSquare == null || !SrcSquare.Validate())
                {
                    return false;
                }

                if (MovePiece == null)
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

            if (IsPromote != other.IsPromote)
            {
                return false;
            }

            if (MovePiece != other.MovePiece)
            {
                return false;
            }

            if (DropPieceType != other.DropPieceType)
            {
                return false;
            }

            if (SpecialMoveType != other.SpecialMoveType)
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
            return Util.GenericEquals(x, y);
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
                (MovePiece != null ? MovePiece.GetHashCode() : 0) ^
                DropPieceType.GetHashCode() ^
                IsPromote.GetHashCode() ^
                SpecialMoveType.GetHashCode());
        }

        #region シリアライズ/デシリアライズ
        [DataMember(Order = 1, IsRequired = true)]
        private uint serializeBits = 0;

        /// <summary>
        /// Squareをシリアライズします。
        /// </summary>
        /// <remarks>
        /// 11～99の値にシリアライズされます。
        /// （0～81の値にシリアライズすると、0の解釈がnull or 11のどちらでも
        /// 可能になってしまうため、値を減らすことはしていません）
        /// </remarks>
        private static byte SerializeSquare(Square square)
        {
            if (square == null)
            {
                return 0;
            }

            // 11 ～ 99
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
            // 1bit
            bits |= ((uint)(IsPromote ? 1 : 0) << 2);
            // 7bit
            bits |= ((uint)SerializeSquare(DstSquare) << 3);
            // 7bit
            bits |= (ActionType == ActionType.Drop ?
                ((uint)(DropPieceType + 100) << 10) :
                ((uint)SerializeSquare(SrcSquare) << 10) );
            // 1bit
            bits |= ((uint)(HasSameSquareAsPrev ? 1 : 0) << 17);

            // 5bit
            if (MovePiece != null)
            {
                bits |= ((uint)MovePiece.Serialize() << 18);
            }

            // 5bit
            if (TookPiece != null)
            {
                bits |= ((uint)TookPiece.Serialize() << 23);
            }

            // 4bit
            if (SpecialMoveType != SpecialMoveType.None)
            {
                bits |= ((uint)SpecialMoveType << 28);
            }

            return bits;
        }

        /// <summary>
        /// デシリアライズを行います。
        /// </summary>
        [CLSCompliant(false)]
        public void Deserialize(uint bits)
        {
            uint tmp;

            // 2bit
            BWType = (BWType)((bits >> 0) & 0x03);
            // 1bit
            IsPromote = (((bits >> 2) & 0x01) != 0);
            // 7bit
            DstSquare = DeserializeSquare((bits >> 3) & 0x7f);
            // 7bit
            tmp = ((bits >> 10) & 0x7f);
            if (tmp >= 100)
                DropPieceType = (PieceType)(tmp - 100);
            else
                SrcSquare = DeserializeSquare(tmp);
            // 1bit
            HasSameSquareAsPrev = (((bits >> 17) & 0x01) != 0);

            // 5bit
            tmp = ((bits >> 18) & 0x1f);
            if (tmp != 0)
            {
                MovePiece = new Piece();
                MovePiece.Deserialize(tmp);
            }

            // 5bit
            tmp = ((bits >> 23) & 0x1f);
            if (tmp != 0)
            {
                TookPiece = new Piece();
                TookPiece.Deserialize(tmp);
            }

            // 4bit
            tmp = ((bits >> 28) & 0x0f);
            if (tmp != 0)
            {
                SpecialMoveType = (SpecialMoveType)tmp;
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
            IsPromote = false;
            DropPieceType = PieceType.None;
        }

        /// <summary>
        /// 特殊な指し手を生成します。
        /// </summary>
        public static BoardMove CreateSpecialMove(BWType turn, SpecialMoveType smoveType)
        {
            if (smoveType == SpecialMoveType.None)
            {
                throw new ArgumentException("smoveType");
            }

            return new BoardMove
            {
                BWType = turn,
                SpecialMoveType = smoveType,
            };
        }

        /// <summary>
        /// 移動手を生成します。
        /// </summary>
        public static BoardMove CreateMove(BWType turn, Square src, Square dst,
                                           Piece movePiece, bool isPromote,
                                           Piece tookPiece = null)
        {
            if (turn == BWType.None)
            {
                throw new ArgumentException("turn");
            }

            if (src == null || !src.Validate())
            {
                throw new ArgumentException("src");
            }

            if (dst == null || !dst.Validate())
            {
                throw new ArgumentException("dst");
            }

            if (movePiece == null || !movePiece.Validate())
            {
                throw new ArgumentException("movePiece");
            }

            if (tookPiece != null && !tookPiece.Validate())
            {
                throw new ArgumentException("tookPiece");
            }

            return new BoardMove
            {
                BWType = turn,
                SrcSquare = src,
                DstSquare = dst,
                MovePiece = movePiece,
                IsPromote = isPromote,
                TookPiece = tookPiece,
            };
        }

        /// <summary>
        /// 駒を打つ手を生成します。
        /// </summary>
        public static BoardMove CreateDrop(BWType turn, Square dst,
                                           PieceType dropPieceType)
        {
            if (turn == BWType.None)
            {
                throw new ArgumentException("turn");
            }

            if (dst == null || !dst.Validate())
            {
                throw new ArgumentException("dst");
            }

            if (dropPieceType == PieceType.None)
            {
                throw new ArgumentException("dropPieceType");
            }

            return new BoardMove
            {
                BWType = turn,
                DstSquare = dst,
                DropPieceType = dropPieceType,
            };
        }
    }
}
