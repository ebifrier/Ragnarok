using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 駒の段方向の動き方の種類です。
    /// </summary>
    [DataContract()]
    public enum RankMoveType
    {
        /// <summary>
        /// 特に無し
        /// </summary>
        [EnumMember()]
        None = 0,
        /// <summary>
        /// 上がる
        /// </summary>
        [EnumMember()]
        Up = 1,
        /// <summary>
        /// 引く
        /// </summary>
        [EnumMember()]
        Back = 2,
        /// <summary>
        /// 寄る
        /// </summary>
        [EnumMember()]
        Sideways = 3,
    }

    /// <summary>
    /// 駒の列方向の相対位置の種類です。
    /// </summary>
    [DataContract()]
    public enum RelFileType
    {
        /// <summary>
        /// 特になし。
        /// </summary>
        [EnumMember()]
        None = 0,
        /// <summary>
        /// 右
        /// </summary>
        [EnumMember()]
        Right = 1,
        /// <summary>
        /// 左
        /// </summary>
        [EnumMember()]
        Left = 2,
        /// <summary>
        /// 直
        /// </summary>
        [EnumMember()]
        Straight = 3,
    }

    /// <summary>
    /// 駒打ちなどのアクションを記述します。
    /// </summary>
    [DataContract()]
    public enum ActionType
    {
        /// <summary>
        /// 特になし。
        /// </summary>
        [EnumMember()]
        None = 0,
        /// <summary>
        /// 成り
        /// </summary>
        [EnumMember()]
        Promote = 1,
        /// <summary>
        /// 不成
        /// </summary>
        [EnumMember()]
        Unpromote = 2,
        /// <summary>
        /// 打ち
        /// </summary>
        [EnumMember()]
        Drop = 3,
    }

    /// <summary>
    /// 将棋の各指し手を保持します。
    /// </summary>
    [DataContract()]
    public class Move : IEquatable<Move>
    {
        /// <summary>
        /// 元の指し手を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public string OriginalText
        {
            get;
            set;
        }

        /// <summary>
        /// 同○○かどうかを取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public bool SameAsOld
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の到達地点の筋を取得または設定します。
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public int File
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の到達地点の段を取得または設定します。
        /// </summary>
        [DataMember(Order = 4, IsRequired = true)]
        public int Rank
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の到達位置を取得または設定します。
        /// </summary>
        public Position NewPosition
        {
            get
            {
                return new Position(File, Rank);
            }
            set
            {
                if ((object)value != null)
                {
                    File = value.File;
                    Rank = value.Rank;
                }
            }
        }

        /// <summary>
        /// 駒の種類を取得または設定します。
        /// </summary>
        [DataMember(Order = 5, IsRequired = true)]
        public Piece Piece
        {
            get;
            set;
        }

        /// <summary>
        /// 移動前の駒の相対位置を取得または設定します。
        /// </summary>
        [DataMember(Order = 6, IsRequired = true)]
        public RelFileType RelFileType
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の動き方の種類を取得または設定します。
        /// </summary>
        [DataMember(Order = 7, IsRequired = true)]
        public RankMoveType RankMoveType
        {
            get;
            set;
        }

        /// <summary>
        /// 駒打ちなどのアクションを取得または設定します。
        /// </summary>
        [DataMember(Order = 8, IsRequired = true)]
        public ActionType ActionType
        {
            get;
            set;
        }

        /// <summary>
        /// 投了かどうかを取得または設定します。
        /// </summary>
        [DataMember(Order = 9, IsRequired = true)]
        public bool IsResigned
        {
            get;
            set;
        }

        /// <summary>
        /// 手番を取得または設定します。
        /// </summary>
        [DataMember(Order = 10, IsRequired = true)]
        public BWType BWType
        {
            get;
            set; 
        }

        /// <summary>
        /// (もしあれば)移動前の情報を取得または設定します。
        /// </summary>
        [DataMember(Order = 11, IsRequired = false)]
        public Position OldPosition
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトの簡易コピーを作成します。
        /// </summary>
        public Move Clone()
        {
            return (Move)this.MemberwiseClone();
        }

        /// <summary>
        /// オブジェクトの各プロパティが正しく設定されているか調べます。
        /// </summary>
        public bool Validate()
        {
            if (IsResigned)
            {
                return true;
            }

            if (!SameAsOld)
            {
                if (File < 1 || 9 < File)
                {
                    return false;
                }

                if (Rank < 1 || 9 < Rank)
                {
                    return false;
                }
            }

            if (Piece == null || !Piece.Validate())
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(RelFileType), RelFileType))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(RankMoveType), RankMoveType))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(ActionType), ActionType))
            {
                return false;
            }

            // 同銀打ち、銀上打つ、などは存在しません。
            if (ActionType == ActionType.Drop &&
                (SameAsOld ||
                 RelFileType != RelFileType.None ||
                 RankMoveType != RankMoveType.None))
            {
                return false;
            }

            // 移動前の位置は、nullでなければ内容を調べます。
            if (OldPosition != null && !OldPosition.Validate())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 指し手を文字列に変換します。
        /// </summary>
        public override string ToString()
        {
            return Stringizer.ToString(this);
        }

        /// <summary>
        /// オブジェクトの比較を行います。
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Move);
        }

        /// <summary>
        /// オブジェクトの比較を行います。
        /// </summary>
        public bool Equals(Move other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (IsResigned != other.IsResigned)
            {
                return false;
            }

            if (BWType != other.BWType)
            {
                return false;
            }

            if (SameAsOld != other.SameAsOld)
            {
                return false;
            }

            // 指し手が同○○でないときだけ、段・列の判定をします。
            if (!SameAsOld &&
                (File != other.File ||
                Rank != other.Rank))
            {
                return false;
            }

            if (RelFileType != other.RelFileType ||
                RankMoveType != other.RankMoveType ||
                ActionType != other.ActionType ||
                Piece != other.Piece)
            {
                return false;
            }

            // TODO: 移動前の位置は判定に使うべきかどうか迷う。
            if (OldPosition != other.OldPosition)
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
            // ValueType.GetHashCodeは遅いらしい。。。
            if (IsResigned)
            {
                return IsResigned.GetHashCode();
            }
            else
            {
                var baseHashCode =
                    IsResigned.GetHashCode() ^
                    BWType.GetHashCode() ^
                    SameAsOld.GetHashCode() ^
                    RelFileType.GetHashCode() ^
                    RankMoveType.GetHashCode() ^
                    ActionType.GetHashCode() ^
                    (Piece != null ? Piece.GetHashCode() : 0) ^
                    (OldPosition != null ? OldPosition.GetHashCode() : 0);

                if (SameAsOld)
                {
                    return baseHashCode;
                }
                else
                {
                    return (
                        baseHashCode ^
                        File.GetHashCode() ^
                        Rank.GetHashCode());
                }
            }
        }

        /// <summary>
        /// == 演算子を実装します。
        /// </summary>
        public static bool operator ==(Move lhs, Move rhs)
        {
            return Ragnarok.Util.GenericClassEquals(lhs, rhs);
        }

        /// <summary>
        /// != 演算子を実装します。
        /// </summary>
        public static bool operator !=(Move lhs, Move rhs)
        {
            return !(lhs == rhs);
        }
    }
}
