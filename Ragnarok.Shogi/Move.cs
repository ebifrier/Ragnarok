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
        /// 駒を打つ場合の駒を取得または設定します。
        /// </summary>
        public Piece DropPiece
        {
            get { return MovePiece; }
        }

        /// <summary>
        /// 先手の手か後手の手かを取得または設定します。
        /// </summary>
        public Colour Colour
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
        /// 駒打ちの場合はEmptyになります。
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
                else if (SrcSquare.IsEmpty())
                {
                    return ActionType.Drop;
                }
                else if (!MovePiece.IsNone())
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
        public bool SameAsPrev
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
                    Stringizer.ToString(Colour),
                    EnumUtil.GetLabel(SpecialMoveType));
            }
            else if (ActionType == ActionType.Drop)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{1}{2}{3}打",
                    Stringizer.ToString(Colour),
                    IntConverter.Convert(NumberType.Big, DstSquare.GetFile()),
                    IntConverter.Convert(NumberType.Kanji, DstSquare.GetRank()),
                    Stringizer.ToString(DropPiece));
            }
            else if (SameAsPrev)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}同　{1}{2}({3}{4})",
                    Stringizer.ToString(Colour),
                    Stringizer.ToString(MovePiece),
                    Stringizer.ToString(ActionType),
                    SrcSquare.GetFile(),
                    SrcSquare.GetRank());
            }
            else
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{1}{2}{3}{4}({5}{6})",
                    Stringizer.ToString(Colour),
                    IntConverter.Convert(NumberType.Big, DstSquare.GetFile()),
                    IntConverter.Convert(NumberType.Kanji, DstSquare.GetRank()),
                    Stringizer.ToString(MovePiece),
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
            if (Colour == Colour.None)
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
                if (DropPiece.IsNone())
                {
                    return false;
                }

                if (!DstSquare.Validate())
                {
                    return false;
                }
                
                if (!SrcSquare.IsEmpty())
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
                if (MovePiece.IsNone())
                {
                    return false;
                }

                if (!DstSquare.Validate())
                {
                    return false;
                }

                if (!SrcSquare.Validate())
                {
                    return false;
                }

                if (!TookPiece.IsNone() &&
                    TookPiece.GetColour() == MovePiece.GetColour())
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

            if (MovePiece != other.MovePiece)
            {
                return false;
            }

            if (Colour != other.Colour)
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
                MovePiece.GetHashCode() ^
                Colour.GetHashCode() ^
                DstSquare.GetHashCode() ^
                SrcSquare.GetHashCode() ^
                IsPromote.GetHashCode() ^
                SpecialMoveType.GetHashCode());
        }

        #region シリアライズ/デシリアライズ
        protected Move(SerializationInfo info, StreamingContext text)
        {
            MovePiece = (Piece)info.GetValue(nameof(MovePiece), typeof(Piece));
            Colour = (Colour)info.GetValue(nameof(Colour), typeof(Colour));
            DstSquare = (Square)info.GetValue(nameof(DstSquare), typeof(Square));
            SrcSquare = (Square)info.GetValue(nameof(SrcSquare), typeof(Square));
            IsPromote = info.GetBoolean(nameof(IsPromote));
            TookPiece = (Piece)info.GetValue(nameof(TookPiece), typeof(Piece));
            SameAsPrev = info.GetBoolean(nameof(SameAsPrev));
            SpecialMoveType = (SpecialMoveType)info.GetInt32(nameof(SpecialMoveType));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(MovePiece), MovePiece);
            info.AddValue(nameof(Colour), Colour);
            info.AddValue(nameof(DstSquare), DstSquare);
            info.AddValue(nameof(SrcSquare), SrcSquare);
            info.AddValue(nameof(IsPromote), IsPromote);
            info.AddValue(nameof(TookPiece), TookPiece);
            info.AddValue(nameof(SameAsPrev), SameAsPrev);
            info.AddValue(nameof(SpecialMoveType), SpecialMoveType);
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Move()
        {
            MovePiece = Piece.None;
            IsPromote = false;
        }

        /// <summary>
        /// 特殊な指し手を生成します。
        /// </summary>
        public static Move CreateSpecialMove(Colour turn, SpecialMoveType smoveType)
        {
            if (smoveType == SpecialMoveType.None)
            {
                throw new ArgumentException(
                    "Enumの値が正しくありません。", nameof(smoveType));
            }

            return new Move
            {
                SpecialMoveType = smoveType,
                Colour = turn,
            };
        }

        /// <summary>
        /// 移動手を生成します。
        /// </summary>
        public static Move CreateMove(Piece movePiece, Square src, Square dst,
                                      bool isPromote = false,
                                      Piece tookPiece = Piece.None)
        {
            if (!movePiece.Validate())
            {
                throw new ArgumentException(
                    "動かす駒が正しくありません。", nameof(movePiece));
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

            return new Move
            {
                MovePiece = movePiece,
                Colour = movePiece.GetColour(),
                SrcSquare = src,
                DstSquare = dst,
                IsPromote = isPromote,
                TookPiece = tookPiece,
            };
        }

        /// <summary>
        /// 駒を打つ手を生成します。
        /// </summary>
        public static Move CreateDrop(Piece dropPiece, Square dst)
        {
            if (dropPiece.IsNone())
            {
                throw new ArgumentException(
                    "Enumの値が正しくありません。", nameof(dropPiece));
            }

            if (!dst.Validate())
            {
                throw new ArgumentException(
                    "Squareの値が正しくありません。", nameof(dst));
            }

            return new Move
            {
                MovePiece = dropPiece.Unpromote(),
                Colour = dropPiece.GetColour(),
                DstSquare = dst,
            };
        }
    }
}
