using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    public class ShiftBitsMask
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShiftBitsMask(int shift, int bits)
        {
            Shift = shift;
            Bits = bits;
            Mask = MakeMask(shift, bits);
        }

        private static int MakeMask(int shift, int bits)
        {
            return (((1 << bits) - 1) << shift);
        }

        /// <summary>
        /// マスクのシフト数を取得します。
        /// </summary>
        public int Shift
        {
            get;
            private set;
        }

        /// <summary>
        /// マスクのビット数を取得します。
        /// </summary>
        public int Bits
        {
            get;
            private set;
        }

        /// <summary>
        /// マスクを取得します。
        /// </summary>
        public int Mask
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// 駒箱/駒台に乗っている駒数を管理します。
    /// </summary>
    /// <remarks>
    /// 00TT0HH0 AA0KKK0G GG0EEEE0Y YY0UUUUU
    /// </remarks>
    [DataContract()]
    public class Hand : IEquatable<Hand>
    {
        private static readonly ShiftBitsMask[] SBM = new ShiftBitsMask[]
        {
            new ShiftBitsMask(0, 0),  // None
            new ShiftBitsMask(28, 2), // Gyoku
            new ShiftBitsMask(25, 2), // Hisya
            new ShiftBitsMask(22, 2), // Kaku
            new ShiftBitsMask(18, 3), // Kin
            new ShiftBitsMask(14, 3), // Gin
            new ShiftBitsMask(10, 3), // Kei
            new ShiftBitsMask(6, 3),  // Kyo
            new ShiftBitsMask(0, 5),  // Hu
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Hand(BWType bwType, int mask)
        {
            BWType = bwType;
            Mask = mask;
        }

        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public Hand Clone()
        {
            return new Hand(BWType, Mask);
        }

        /// <summary>
        /// 各駒の持ち駒のマスクを取得します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public int Mask
        {
            get;
            private set;
        }

        /// <summary>
        /// 先手側か後手側かを取得します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public BWType BWType
        {
            get;
            private set;
        }

        /// <summary>
        /// 持ち駒の数を取得します。
        /// </summary>
        public int Get(PieceType pieceType)
        {
            if (pieceType > PieceType.Hu)
            {
                throw new ArgumentException(
                    "pieceTypeの値が不正です。", nameof(pieceType));
            }

            var sbm = SBM[(int)pieceType];
            return ((Mask & sbm.Mask) >> sbm.Shift);
        }

        /// <summary>
        /// 持ち駒の数を設定します。
        /// </summary>
        public void Set(PieceType pieceType, int count)
        {
            if (pieceType > PieceType.Hu)
            {
                throw new ArgumentException(
                    "pieceTypeの値が不正です。", nameof(pieceType));
            }

            if (pieceType == PieceType.Gyoku && count > 0)
            {
                throw new ShogiException(
                    "玉を駒台に乗せることはできません。");
            }

            var sbm = SBM[(int)pieceType];
            if (count < 0 || (1 << sbm.Bits) <= count)
            {
                throw new ShogiException(
                    $"{pieceType}の駒の数が{count}です。");
            }
            
            Mask &= ~sbm.Mask;
            Mask |= (count << sbm.Shift);
        }

        /// <summary>
        /// 持ち駒の数を増やします。
        /// </summary>
        private void Add(PieceType pieceType, int add)
        {
            if (pieceType > PieceType.Hu)
            {
                throw new ArgumentException(
                    "pieceTypeの値が不正です。", nameof(pieceType));
            }

            var count = Get(pieceType);
            Set(pieceType, count + add);
        }

        /// <summary>
        /// 持ち駒の数を増やします。
        /// </summary>
        public void Increment(PieceType pieceType)
        {
            Add(pieceType, +1);
        }

        /// <summary>
        /// 持ち駒の数を減らします。
        /// </summary>
        public void Decrement(PieceType pieceType)
        {
            Add(pieceType, -1);
        }

        /// <summary>
        /// 持ち駒をすべて初期化します。
        /// </summary>
        public void Clear()
        {
            Mask = 0;
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (!Enum.IsDefined(typeof(BWType), BWType))
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

            return Equals(obj as Hand);
        }

        /// <summary>
        /// オブジェクトの等値性を判定します。
        /// </summary>
        public bool Equals(Hand other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (Mask != other.Mask)
            {
                return false;
            }

            if (BWType != other.BWType)
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
                Mask.GetHashCode() ^
                BWType.GetHashCode());
        }
    }
}
