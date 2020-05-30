using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
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
    [Serializable()]
    public class Move : IEquatable<Move>, ISerializable
    {
        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public Move Clone()
        {
            return (Move)MemberwiseClone();
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
                else if (!SrcSquare.IsEmpty() && !MovePiece.IsNone)
                {
                    return (IsPromote ? ActionType.Promote : ActionType.None);
                }

                throw new InvalidOperationException(
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
                    CultureInfo.CurrentCulture,
                    "{0}{1}",
                    Stringizer.ToString(BWType),
                    EnumUtil.GetLabel(SpecialMoveType));
            }
            else if (ActionType == ActionType.Drop)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{1}{2}{3}打",
                    Stringizer.ToString(BWType),
                    IntConverter.Convert(NumberType.Big, DstSquare.GetFile()),
                    IntConverter.Convert(NumberType.Kanji, DstSquare.GetRank()),
                    Stringizer.ToString(DropPieceType));
            }
            else if (HasSameSquareAsPrev)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}同　{1}{2}({3}{4})",
                    Stringizer.ToString(BWType),
                    MovePiece,
                    Stringizer.ToString(ActionType),
                    SrcSquare.GetFile(),
                    SrcSquare.GetRank());
            }
            else
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{1}{2}{3}{4}({5}{6})",
                    Stringizer.ToString(BWType),
                    IntConverter.Convert(NumberType.Big, DstSquare.GetFile()),
                    IntConverter.Convert(NumberType.Kanji, DstSquare.GetRank()),
                    MovePiece,
                    Stringizer.ToString(ActionType),
                    SrcSquare.GetFile(),
                    SrcSquare.GetRank());
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
                if (!DstSquare.Validate())
                {
                    return false;
                }
                
                if (!SrcSquare.IsEmpty())
                {
                    return false;
                }

                if (!MovePiece.IsNone)
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
                if (!DstSquare.Validate())
                {
                    return false;
                }

                if (!SrcSquare.Validate())
                {
                    return false;
                }

                if (MovePiece.IsNone)
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

            return Equals(obj as Move);
        }

        /// <summary>
        /// オブジェクトの等値性を調べます。
        /// </summary>
        public bool Equals(Move other)
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
        public static bool operator ==(Move x, Move y)
        {
            return Util.GenericEquals(x, y);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(Move x, Move y)
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
                DstSquare.GetHashCode() ^
                SrcSquare.GetHashCode() ^
                MovePiece.GetHashCode() ^
                DropPieceType.GetHashCode() ^
                IsPromote.GetHashCode() ^
                SpecialMoveType.GetHashCode());
        }

        #region シリアライズ/デシリアライズ
        protected Move(SerializationInfo info, StreamingContext text)
        {
            BWType = (BWType)info.GetValue(nameof(BWType), typeof(BWType));
            IsPromote = info.GetBoolean(nameof(IsPromote));
            DstSquare = (Square)info.GetValue(nameof(DstSquare), typeof(Square));
            SrcSquare = (Square)info.GetValue(nameof(SrcSquare), typeof(Square));
            DropPieceType = (PieceType)info.GetInt32(nameof(DropPieceType));
            HasSameSquareAsPrev = info.GetBoolean(nameof(HasSameSquareAsPrev));
            MovePiece = (Piece)info.GetValue(nameof(MovePiece), typeof(Piece));
            TookPiece = (Piece)info.GetValue(nameof(TookPiece), typeof(Piece));
            SpecialMoveType = (SpecialMoveType)info.GetInt32(nameof(SpecialMoveType));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(BWType), BWType);
            info.AddValue(nameof(IsPromote), IsPromote);
            info.AddValue(nameof(DstSquare), DstSquare);
            info.AddValue(nameof(SrcSquare), SrcSquare);
            info.AddValue(nameof(DropPieceType), DropPieceType);
            info.AddValue(nameof(HasSameSquareAsPrev), HasSameSquareAsPrev);
            info.AddValue(nameof(MovePiece), MovePiece);
            info.AddValue(nameof(TookPiece), TookPiece);
            info.AddValue(nameof(SpecialMoveType), SpecialMoveType);
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Move()
        {
            BWType = BWType.None;
            IsPromote = false;
            DropPieceType = PieceType.None;
        }

        /// <summary>
        /// 特殊な指し手を生成します。
        /// </summary>
        public static Move CreateSpecialMove(BWType turn, SpecialMoveType smoveType)
        {
            if (smoveType == SpecialMoveType.None)
            {
                throw new ArgumentException(
                    "Enumの値が正しくありません。", nameof(smoveType));
            }

            return new Move
            {
                BWType = turn,
                SpecialMoveType = smoveType,
            };
        }

        /// <summary>
        /// 移動手を生成します。
        /// </summary>
        public static Move CreateMove(BWType turn, Square src, Square dst,
                                      Piece movePiece, bool isPromote,
                                      Piece tookPiece = default)
        {
            if (turn == BWType.None)
            {
                throw new ArgumentException(
                    "Enumの値が正しくありません。", nameof(turn));
            }

            if (!src.Validate())
            {
                throw new ArgumentException(
                    "移動元のマスが正しくありません。", nameof(src));
            }

            if (!dst.Validate())
            {
                throw new ArgumentException(
                    "移動先のマスが正しくありません。", nameof(dst));
            }

            if (!movePiece.Validate())
            {
                throw new ArgumentException(
                    "動かす駒が正しくありません。", nameof(movePiece));
            }

            if (!tookPiece.IsNone && !tookPiece.Validate())
            {
                throw new ArgumentException(
                    "取った駒が正しくありません。", nameof(tookPiece));
            }

            return new Move
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
        public static Move CreateDrop(BWType turn, Square dst,
                                      PieceType dropPieceType)
        {
            if (turn == BWType.None)
            {
                throw new ArgumentException(
                    "Enumの値が正しくありません。", nameof(turn));
            }

            if (!dst.Validate())
            {
                throw new ArgumentException(
                    "Squareの値が正しくありません。", nameof(dst));
            }

            if (dropPieceType == PieceType.None)
            {
                throw new ArgumentException(
                    "Enumの値が正しくありません。", nameof(dropPieceType));
            }

            return new Move
            {
                BWType = turn,
                DstSquare = dst,
                DropPieceType = dropPieceType,
            };
        }
    }
}
